using System.Linq;
using NUnit.Framework;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// グレネードの状態機械。クライアントが自分の推測で爆発させず、
    /// サーバの報告した状態をそのまま反映できることが要点。
    /// </summary>
    public class GrenadeStateTests
    {
        private class TestGrenade : AbstractGrenade
        {
            public int HitCount { get; private set; }

            public override void Hit()
            {
                HitCount++;
                SetState(GrenadeState.Exploded);
            }
        }

        // ---- Arming -------------------------------------------------------

        [Test]
        public void AFreshGrenadeIsIdle()
        {
            var grenade = new TestGrenade();

            Assert.That(grenade.State, Is.EqualTo(GrenadeState.Idle));
            Assert.That(grenade.IsArmed, Is.False);
        }

        [Test]
        public void ArmingMovesToArmed()
        {
            var grenade = new TestGrenade();

            grenade.Arm();

            Assert.That(grenade.IsArmed, Is.True);
        }

        [Test]
        public void ArmingRaisesTheArmedEvent()
        {
            var grenade = new TestGrenade();
            AbstractGrenade reported = null;
            grenade.Armed += g => reported = g;

            grenade.Arm();

            Assert.That(reported, Is.SameAs(grenade));
        }

        [Test]
        public void ArmingTwiceDoesNotRestartTheFuse()
        {
            var grenade = new TestGrenade { FuseTime = 5f };
            grenade.Arm();
            grenade.UpdateCountdown(2f);

            grenade.Arm();

            Assert.That(grenade.RemainingFuseTime, Is.EqualTo(3f).Within(0.001f));
        }

        // ---- Countdown ---------------------------------------------------

        [Test]
        public void AnUnarmedGrenadeDoesNotCountDown()
        {
            var grenade = new TestGrenade { FuseTime = 3f };

            var exploded = grenade.UpdateCountdown(10f);

            Assert.That(exploded, Is.False);
            Assert.That(grenade.RemainingFuseTime, Is.EqualTo(3f));
        }

        [Test]
        public void CountingDownPastTheFuseReportsTheExplosion()
        {
            var grenade = new TestGrenade { FuseTime = 3f };
            grenade.Arm();

            var exploded = grenade.UpdateCountdown(3.5f);

            Assert.That(exploded, Is.True);
        }

        [Test]
        public void TheFuseDoesNotRunPastItsOwnLength()
        {
            var grenade = new TestGrenade { FuseTime = 3f };
            grenade.Arm();

            grenade.UpdateCountdown(10f);

            Assert.That(grenade.RemainingFuseTime, Is.EqualTo(0f));
        }

        [Test]
        public void AZeroDeltaDoesNotMoveTheFuse()
        {
            var grenade = new TestGrenade { FuseTime = 3f };
            grenade.Arm();

            var exploded = grenade.UpdateCountdown(0f);

            Assert.That(exploded, Is.False);
            Assert.That(grenade.RemainingFuseTime, Is.EqualTo(3f));
        }

        // ---- Explosion ---------------------------------------------------

        [Test]
        public void ExplodingRunsTheHitExactlyOnce()
        {
            var grenade = new TestGrenade();
            grenade.Arm();

            grenade.ForceExplode();
            grenade.ForceExplode();

            Assert.That(grenade.HitCount, Is.EqualTo(1));
        }

        [Test]
        public void ExplodingRaisesTheExplodedEventOnce()
        {
            var grenade = new TestGrenade();
            int raised = 0;
            grenade.Exploded += g => raised++;

            grenade.ForceExplode();
            grenade.ForceExplode();

            Assert.That(raised, Is.EqualTo(1));
        }

        [Test]
        public void ADisposedGrenadeCannotExplode()
        {
            var grenade = new TestGrenade();
            grenade.DisposeGrenade();

            grenade.ForceExplode();

            Assert.That(grenade.HitCount, Is.EqualTo(0));
        }

        [Test]
        public void DisarmingAnArmedGrenadePutsTheFuseBack()
        {
            var grenade = new TestGrenade { FuseTime = 4f };
            grenade.Arm();
            grenade.UpdateCountdown(3f);

            var disarmed = grenade.Disarm();

            Assert.That(disarmed, Is.True);
            Assert.That(grenade.RemainingFuseTime, Is.EqualTo(4f));
        }

        [Test]
        public void DisarmingAGrenadeThatWasNeverArmedFails()
        {
            var grenade = new TestGrenade();

            Assert.That(grenade.Disarm(), Is.False);
        }

        // ---- Applying server state ---------------------------------------

        [Test]
        public void TheReportedStateIsAdopted()
        {
            var grenade = new TestGrenade();
            var json = new Newtonsoft.Json.Linq.JObject
            {
                ["state"] = "Armed"
            };

            grenade.ApplyState(json);

            Assert.That(grenade.IsArmed, Is.True);
        }

        [Test]
        public void AMessageWithoutAStateKeepsTheLocalOne()
        {
            var grenade = new TestGrenade();
            grenade.Arm();

            grenade.ApplyState(new Newtonsoft.Json.Linq.JObject());

            Assert.That(grenade.IsArmed, Is.True);
        }

        [Test]
        public void AnUnreadableStateKeepsTheLocalOne()
        {
            var grenade = new TestGrenade();
            var json = new Newtonsoft.Json.Linq.JObject
            {
                ["state"] = "Detonating"
            };

            grenade.ApplyState(json);

            Assert.That(grenade.State, Is.EqualTo(GrenadeState.Idle));
        }

        [Test]
        public void TheReportedFuseAndRemainingTimeRebuildTheElapsedTime()
        {
            var grenade = new TestGrenade();
            var json = new Newtonsoft.Json.Linq.JObject
            {
                ["fuseTime"] = 10f,
                ["remainingFuseTime"] = 4f
            };

            grenade.ApplyState(json);

            Assert.That(grenade.RemainingFuseTime, Is.EqualTo(4f).Within(0.001f));
        }

        [Test]
        public void ReportedPositionIsAdoptedUnderEitherKey()
        {
            var grenade = new TestGrenade();
            grenade.ApplyState(new Newtonsoft.Json.Linq.JObject
            {
                ["posx"] = 12f,
                ["posy"] = 34f
            });
            grenade.ApplyState(new Newtonsoft.Json.Linq.JObject
            {
                ["PosX"] = 56f,
                ["PosY"] = 78f
            });

            Assert.That(grenade.Posx, Is.EqualTo(56f));
            Assert.That(grenade.Posy, Is.EqualTo(78f));
        }

        [Test]
        public void ApplyingTheServersOwnJsonRoundTripsTheState()
        {
            var grenade = new TestGrenade { FuseTime = 8f };
            grenade.Arm();
            grenade.UpdateCountdown(3f);

            var copy = new TestGrenade();
            copy.ApplyState(grenade.ToJSon());

            Assert.That(copy.IsArmed, Is.True);
            Assert.That(copy.FuseTime, Is.EqualTo(8f));
            Assert.That(copy.RemainingFuseTime, Is.EqualTo(5f).Within(0.001f));
        }

        // ---- Values that cannot be trusted -------------------------------

        [Test]
        public void ANegativeReportedDamageIsPulledUpToZero()
        {
            var grenade = new TestGrenade();
            grenade.ApplyState(new Newtonsoft.Json.Linq.JObject
            {
                ["damage"] = -50
            });

            Assert.That(grenade.Damage, Is.EqualTo(0));
        }

        [Test]
        public void ANegativeFuseIsPulledUpToZero()
        {
            var grenade = new TestGrenade();
            grenade.ApplyState(new Newtonsoft.Json.Linq.JObject
            {
                ["fuseTime"] = -1f
            });

            Assert.That(grenade.FuseTime, Is.EqualTo(0f));
        }

        [Test]
        public void ADamagingMessageDoesNotBlowTheGrenadeUpOnItsOwn()
        {
            // A message reporting full damage is data, not an instruction, so the
            // grenade must wait for the fuse or an explicit explosion.
            var grenade = new TestGrenade { FuseTime = 5f };
            grenade.Arm();
            grenade.ApplyState(new Newtonsoft.Json.Linq.JObject
            {
                ["damage"] = 999,
                ["explosionDamage"] = 999
            });

            Assert.That(grenade.HitCount, Is.EqualTo(0));
            Assert.That(grenade.IsArmed, Is.True);
        }

        // ---- Grenade types -----------------------------------------------

        [Test]
        public void AKnownTypeNameIsUnderstood()
        {
            var type = new GranadeType();

            var understood = type.FromStr("Power");

            Assert.That(understood, Is.True);
            Assert.That(type.Type, Is.EqualTo(EGranadeType.Power));
        }

        [Test]
        public void AnUnknownTypeNameIsReportedAsSuch()
        {
            var type = new GranadeType();

            var understood = type.FromStr("Banana");

            Assert.That(understood, Is.False);
            Assert.That(type.Type, Is.EqualTo(EGranadeType.Unknown));
        }

        // ---- Concrete grenades -------------------------------------------

        [Test]
        public void ANewGrenadeCarriesTheValuesItsTypeIsDefinedBy()
        {
            var grenade = new PowerGrenade();

            Assert.That(grenade.Damage, Is.EqualTo(110));
            Assert.That(grenade.ExplosionDamage, Is.EqualTo(160));
            Assert.That(grenade.ExplosionRadius, Is.EqualTo(4.5f));
            Assert.That(grenade.FuseTime, Is.EqualTo(2.5f));
        }

        [Test]
        public void AClusterGrenadeSpawnsAsManyChildrenAsItWasToldTo()
        {
            var grenade = new ClusterGrenade();
            int reported = 0;
            grenade.ChildGrenadeCreated += child => reported++;

            var children = grenade.SpawnChildren();

            Assert.That(children.Length, Is.EqualTo(3));
            Assert.That(reported, Is.EqualTo(3));
        }

        [Test]
        public void AClusterGrenadeWithNoChildrenSpawnsNothing()
        {
            var grenade = new ClusterGrenade { Child = 0 };
            int reported = 0;
            grenade.ChildGrenadeCreated += child => reported++;

            var children = grenade.SpawnChildren();

            Assert.That(children.Length, Is.EqualTo(0));
            Assert.That(reported, Is.EqualTo(0));
        }

        [Test]
        public void TheChildrenAreScatteredAroundTheParent()
        {
            var grenade = new ClusterGrenade(10f, 10f) { Child = 3 };

            var children = grenade.SpawnChildren();

            var positions = children
                .Select(c => (c.Posx, c.Posy))
                .ToList();

            Assert.That(positions.Distinct().Count(), Is.EqualTo(3));
            Assert.That(children.Select(c => c.Posx), Has.None.EqualTo(grenade.Posx));
        }

        [Test]
        public void TheChildrenInheritTheOwnerOfTheParent()
        {
            var grenade = new ClusterGrenade { OwnerId = "player-7" };

            var children = grenade.SpawnChildren();

            Assert.That(children[0].OwnerId, Is.EqualTo("player-7"));
        }

        [Test]
        public void AClusterGrenadeThatExplodesIsDisposed()
        {
            var grenade = new ClusterGrenade();

            grenade.Hit();

            Assert.That(grenade.IsDisposed, Is.True);
        }

        [Test]
        public void AnExplodedChildGrenadeIsDisposed()
        {
            var grenade = new ChildClusterGrenade();

            grenade.Hit();

            Assert.That(grenade.IsDisposed, Is.True);
        }
    }
}
