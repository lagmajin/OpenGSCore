using NUnit.Framework;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// The client adopts the health the server reported, and this is the rule
    /// that says what adopting it means.
    /// <para>
    /// The reconciliation used to live inside the player component, so nothing
    /// checked it. The parts worth checking are the ones a malformed or
    /// repeated message can get wrong: a ceiling the client has never seen, a
    /// reported value above that ceiling, and a message that repeats a value the
    /// client already holds.
    /// </para>
    /// </summary>
    public class AuthoritativeHealthTests
    {
        // ---- Ceiling adoption --------------------------------------------

        [Test]
        public void AReportedCeilingIsAdopted()
        {
            var result = AuthoritativeHealth.AdoptCeiling(100, 250);

            Assert.That(result.Ceiling, Is.EqualTo(250));
            Assert.That(result.Changed, Is.True);
        }

        [Test]
        public void ACeilingTheClientAlreadyHasIsNotAChange()
        {
            var result = AuthoritativeHealth.AdoptCeiling(100, 100);

            Assert.That(result.Ceiling, Is.EqualTo(100));
            Assert.That(result.Changed, Is.False);
        }

        [Test]
        public void AMessageWithoutACeilingKeepsTheLocalOne()
        {
            // A death message says who is out but not what the ceiling is.
            var result = AuthoritativeHealth.AdoptCeiling(100, 0);

            Assert.That(result.Ceiling, Is.EqualTo(100));
            Assert.That(result.Changed, Is.False);
        }

        [Test]
        public void ANegativeCeilingKeepsTheLocalOne()
        {
            var result = AuthoritativeHealth.AdoptCeiling(100, -50);

            Assert.That(result.Ceiling, Is.EqualTo(100));
            Assert.That(result.Changed, Is.False);
        }

        [Test]
        public void AnAbsurdCeilingIsPulledIntoRange()
        {
            var result = AuthoritativeHealth.AdoptCeiling(100, AuthoritativeHealth.MaxMaxHealth + 5000);

            Assert.That(result.Ceiling, Is.EqualTo(AuthoritativeHealth.MaxMaxHealth));
        }

        [Test]
        public void ACeilingIsNeverAllowedToDropToZero()
        {
            // A ceiling of zero would make every clamp a kill.
            Assert.That(AuthoritativeHealth.Ceiling(0), Is.EqualTo(AuthoritativeHealth.MinMaxHealth));
            Assert.That(AuthoritativeHealth.Ceiling(-100), Is.EqualTo(AuthoritativeHealth.MinMaxHealth));
        }

        [Test]
        public void ACeilingAtTheBoundsIsKept()
        {
            Assert.That(
                AuthoritativeHealth.Ceiling(AuthoritativeHealth.MinMaxHealth),
                Is.EqualTo(AuthoritativeHealth.MinMaxHealth));
            Assert.That(
                AuthoritativeHealth.Ceiling(AuthoritativeHealth.MaxMaxHealth),
                Is.EqualTo(AuthoritativeHealth.MaxMaxHealth));
        }

        // ---- Remaining value ---------------------------------------------

        [Test]
        public void AReportedValueInsideTheRangeIsKept()
        {
            Assert.That(AuthoritativeHealth.ClampRemaining(60, 100), Is.EqualTo(60));
        }

        [Test]
        public void AValueAboveTheCeilingIsClampedToIt()
        {
            // A message claiming more health than the player can hold must not
            // hand out a free full bar.
            Assert.That(AuthoritativeHealth.ClampRemaining(500, 100), Is.EqualTo(100));
        }

        [Test]
        public void ANegativeValueIsClampedToZero()
        {
            Assert.That(AuthoritativeHealth.ClampRemaining(-30, 100), Is.EqualTo(0));
        }

        [Test]
        public void AValueIsClampedAgainstTheCeilingThatWasJustAdopted()
        {
            // The server raised the ceiling, so the reported value is checked
            // against the new one rather than the one the client had.
            var ceiling = AuthoritativeHealth.AdoptCeiling(100, 250).Ceiling;
            Assert.That(ceiling, Is.EqualTo(250));

            Assert.That(AuthoritativeHealth.ClampRemaining(250, ceiling), Is.EqualTo(250));
        }

        [Test]
        public void AnUnusableCeilingStillBoundsTheValue()
        {
            Assert.That(
                AuthoritativeHealth.ClampRemaining(50, 0),
                Is.EqualTo(AuthoritativeHealth.MinMaxHealth));
        }

        // ---- Change detection --------------------------------------------

        [Test]
        public void ADifferentValueIsAMeaningfulChange()
        {
            Assert.That(AuthoritativeHealth.IsMeaningfulChange(100f, 65), Is.True);
        }

        [Test]
        public void TheSameValueIsNotAChange()
        {
            // A repeated message must not raise a second death.
            Assert.That(AuthoritativeHealth.IsMeaningfulChange(100f, 100), Is.False);
        }

        [Test]
        public void ASmallFractionalDifferenceIsNotAChange()
        {
            // Health is a server unit, so a hair of difference is the same
            // number arriving twice.
            Assert.That(AuthoritativeHealth.IsMeaningfulChange(100f, 100), Is.False);
            Assert.That(AuthoritativeHealth.IsMeaningfulChange(100f, 99), Is.True);
        }

        [Test]
        public void AClientValueBelowZeroIsTreatedAsZero()
        {
            Assert.That(AuthoritativeHealth.IsMeaningfulChange(-5f, 0), Is.False);
        }

        public void GoingFromHalfAPointToZeroIsAMeaningfulChange()
        {
            // A player a fraction of a point from zero is still alive, so the
            // message that finishes them has to register as a change.
            Assert.That(AuthoritativeHealth.IsMeaningfulChange(0.5f, 0), Is.True);
        }

        [Test]
        public void APlayerAlreadyEffectivelyOutIsNotChangedAgain()
        {
            // Under half a point is the same number arriving twice, so a death
            // message about a player already at zero must not raise a second one.
            Assert.That(AuthoritativeHealth.IsMeaningfulChange(0.4f, 0), Is.False);
            Assert.That(AuthoritativeHealth.IsMeaningfulChange(0f, 0), Is.False);
        }
    }
}
