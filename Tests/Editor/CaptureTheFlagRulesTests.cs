using System.Collections.Generic;
using NUnit.Framework;
using OpenGSCore;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// The capture rule, which is what the mode turns on.
    /// <para>
    /// The rule says a team scores when it brings the enemy flag to its own stand
    /// while its own flag is still at base. The second half was missing: a capture
    /// was believed as long as the player had once claimed a pickup, so a team
    /// whose own flag was in the other side's hands could still score. These pin
    /// the rule down where it can be run without a client and without a server.
    /// </para>
    /// </summary>
    public class CaptureTheFlagRulesTests
    {
        private static Dictionary<ETeam, TeamFlag> FreshFlags()
        {
            return new Dictionary<ETeam, TeamFlag>
            {
                [ETeam.Red] = new TeamFlag(ETeam.Red),
                [ETeam.Blue] = new TeamFlag(ETeam.Blue)
            };
        }

        [Test]
        public void AFlagStartsOnItsOwnStand()
        {
            var flags = FreshFlags();

            Assert.That(flags[ETeam.Red].State, Is.EqualTo(EFlagState.FlagOnStand));
            Assert.That(flags[ETeam.Red].IsAtBase, Is.True);
            Assert.That(flags[ETeam.Red].CarrierId, Is.Null);
        }

        [Test]
        public void AFlagThatIsCarriedIsNamedByTheCarrier()
        {
            var flags = FreshFlags();

            Assert.That(flags[ETeam.Blue].PickUp("red-player"), Is.True);

            Assert.That(flags[ETeam.Blue].State, Is.EqualTo(EFlagState.FlagCapturedPlayer));
            Assert.That(flags[ETeam.Blue].CarrierId, Is.EqualTo("red-player"));
        }

        [Test]
        public void AFlagIsOneObjectSoItCannotBeTakenTwice()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].PickUp("red-player");

            // The second claim names a flag that is not on the ground. Accepting
            // it would hand the same flag to two players.
            Assert.That(flags[ETeam.Blue].PickUp("other-red"), Is.False);
            Assert.That(flags[ETeam.Blue].CarrierId, Is.EqualTo("red-player"));
        }

        [Test]
        public void AFlagThatIsNotBeingCarriedCannotBeDropped()
        {
            var flags = FreshFlags();

            // Nothing is in anybody's hands, so a claim that a carrier dropped is
            // a claim about a carrier that does not exist.
            Assert.That(flags[ETeam.Blue].Drop(), Is.False);
            Assert.That(flags[ETeam.Blue].State, Is.EqualTo(EFlagState.FlagOnStand));
        }

        [Test]
        public void AFlagAtBaseIsNotPutBack()
        {
            var flags = FreshFlags();

            // A flag already home is not a change, and reporting one would make a
            // reset look like something happening.
            Assert.That(flags[ETeam.Blue].Return(EFlagReturnReason.AutoReturn), Is.False);
        }

        [Test]
        public void ADeliveryScoresWhenBothFlagsAreWhereTheRuleSays()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].PickUp("red-player");

            Assert.That(
                CaptureTheFlagRules.RefusalFor(ETeam.Red, flags),
                Is.EqualTo(EFlagRefusal.None));
        }

        [Test]
        public void ADeliveryDoesNotScoreWhileTheOwnFlagIsInEnemyHands()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].PickUp("red-player");

            // Red's own flag is in blue's hands. This is the rule the mode turns
            // on and it was not checked at all: a capture was believed as long as
            // the claiming player had once claimed a pickup.
            flags[ETeam.Red].PickUp("blue-player");

            Assert.That(
                CaptureTheFlagRules.RefusalFor(ETeam.Red, flags),
                Is.EqualTo(EFlagRefusal.OwnFlagNotAtBase));
            Assert.That(CaptureTheFlagRules.CanScore(ETeam.Red, flags), Is.False);
        }

        [Test]
        public void ADeliveryDoesNotScoreWhileTheOwnFlagIsOnTheGround()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].PickUp("red-player");
            flags[ETeam.Red].PickUp("blue-player");
            flags[ETeam.Red].Drop();

            // A flag on the ground is not at base either, so the same refusal
            // applies: a team whose flag is loose cannot defend.
            Assert.That(
                CaptureTheFlagRules.RefusalFor(ETeam.Red, flags),
                Is.EqualTo(EFlagRefusal.OwnFlagNotAtBase));
        }

        [Test]
        public void ADeliveryDoesNotScoreWithNoEnemyFlagCarried()
        {
            var flags = FreshFlags();

            // Both flags are home. There is nothing to deliver, so this is refused
            // for the absence of the other flag rather than for the own one.
            Assert.That(
                CaptureTheFlagRules.RefusalFor(ETeam.Red, flags),
                Is.EqualTo(EFlagRefusal.NoEnemyFlagCarried));
        }

        [Test]
        public void APlayerOnNoTeamCannotScore()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].PickUp("red-player");

            Assert.That(
                CaptureTheFlagRules.RefusalFor(ETeam.NoTeam, flags),
                Is.EqualTo(EFlagRefusal.NotATeam));
        }

        [Test]
        public void ATeamWhoseOwnFlagTheServerHasNeverHeardOfCannotScore()
        {
            // The dangerous case: a missing flag is not a safe flag. Treating an
            // unknown flag as one on its stand is the hole the rule is here to close.
            var flags = new Dictionary<ETeam, TeamFlag>
            {
                [ETeam.Blue] = new TeamFlag(ETeam.Blue)
            };
            flags[ETeam.Blue].PickUp("red-player");

            Assert.That(
                CaptureTheFlagRules.RefusalFor(ETeam.Red, flags),
                Is.EqualTo(EFlagRefusal.OwnFlagNotAtBase));
        }

        [Test]
        public void ATeamCanScoreAgainOnceItsFlagIsBack()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].PickUp("red-player");
            flags[ETeam.Red].PickUp("blue-player");

            Assert.That(CaptureTheFlagRules.CanScore(ETeam.Red, flags), Is.False);

            flags[ETeam.Red].Return(EFlagReturnReason.FriendlyRecovered);

            // A friendly recovery is a state reset and not a score, which is what
            // makes the second delivery legitimate.
            Assert.That(CaptureTheFlagRules.CanScore(ETeam.Red, flags), Is.True);
        }

        [Test]
        public void AResetPutsBothFlagsBackAndLeavesThemThere()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].PickUp("red-player");
            flags[ETeam.Red].PickUp("blue-player");
            flags[ETeam.Blue].Drop();

            CaptureTheFlagRules.ResetAll(flags);

            Assert.That(flags[ETeam.Red].State, Is.EqualTo(EFlagState.FlagOnStand));
            Assert.That(flags[ETeam.Blue].State, Is.EqualTo(EFlagState.FlagOnStand));
            Assert.That(flags[ETeam.Red].CarrierId, Is.Null);
            Assert.That(flags[ETeam.Blue].CarrierId, Is.Null);
        }

        [Test]
        public void ATeamWithNoOpponentHasNoEnemyFlag()
        {
            var flags = FreshFlags();

            Assert.That(CaptureTheFlagRules.OpposingTeam(ETeam.NoTeam), Is.EqualTo(ETeam.NoTeam));
            Assert.That(CaptureTheFlagRules.OpposingFlag(ETeam.NoTeam, flags), Is.Null);
            Assert.That(
                CaptureTheFlagRules.OpposingFlag(ETeam.Red, flags),
                Is.SameAs(flags[ETeam.Blue]));
        }

        [Test]
        public void TheSharedFlagStateIsTheOneBothSidesName()
        {
            // The client carried a second, differently spelled enum next to this
            // one: FlagOnStand here and AtBase there for the same state. The
            // extension helpers are the shared spelling, and the client's copy
            // was the one that had to go.
            Assert.That(EFlagState.FlagOnStand.IsStable(), Is.True);
            Assert.That(EFlagState.FlagCapturedPlayer.IsCarried(), Is.True);
            Assert.That(EFlagState.FlagOnGround.IsDropped(), Is.True);
            Assert.That(EFlagState.FlagOnStand.IsCarried(), Is.False);
            Assert.That(EFlagState.FlagOnGround.IsStable(), Is.False);
        }
    }
}
