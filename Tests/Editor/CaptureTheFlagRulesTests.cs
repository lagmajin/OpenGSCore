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
            Assert.That(flags[ETeam.Blue].Drop(0.0d), Is.False);
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
            flags[ETeam.Red].Drop(0.0d);

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
            flags[ETeam.Blue].Drop(0.0d);

            CaptureTheFlagRules.ResetAll(flags);

            Assert.That(flags[ETeam.Red].State, Is.EqualTo(EFlagState.FlagOnStand));
            Assert.That(flags[ETeam.Blue].State, Is.EqualTo(EFlagState.FlagOnStand));
            Assert.That(flags[ETeam.Red].CarrierId, Is.Null);
            Assert.That(flags[ETeam.Blue].CarrierId, Is.Null);
        }

        [Test]
        public void ADroppedFlagWaitsOnTheGroundBeforeItGoesHome()
        {
            var flags = FreshFlags();
            var flag = flags[ETeam.Blue];
            flag.AutoReturnSeconds = 30f;

            flag.PickUp("red-player");
            flag.Drop(1000.0d);

            // The clock starts when the flag is dropped, because a flag on the
            // ground is on a countdown whether or not anybody is around to see it
            // go home.
            Assert.That(flag.HasTimedOutOnGround(1029.0d), Is.False);
            Assert.That(flag.HasTimedOutOnGround(1030.0d), Is.True);
        }

        [Test]
        public void AFlagBeingCarriedIsNotOnATimer()
        {
            var flags = FreshFlags();
            var flag = flags[ETeam.Blue];

            flag.PickUp("red-player");

            // A carrier is waiting for nothing, so a carried flag is never due
            // back however long they hold it.
            Assert.That(flag.HasTimedOutOnGround(100000.0d), Is.False);
        }

        [Test]
        public void ATimedOutFlagGoesHomeByItself()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].AutoReturnSeconds = 10f;
            flags[ETeam.Blue].PickUp("red-player");
            flags[ETeam.Blue].Drop(0.0d);

            var returned = CaptureTheFlagRules.ReturnTimedOutFlags(flags, 20.0d);

            // A flag nobody claims has to come back on its own, or a team that
            // lost a carrier can never pick their flag up again and is out of the
            // match by a rule nobody chose.
            Assert.That(returned, Is.EqualTo(ETeam.Blue));
            Assert.That(flags[ETeam.Blue].State, Is.EqualTo(EFlagState.FlagOnStand));
        }

        [Test]
        public void AFlagThatWasNeverDroppedIsNotSweptUp()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].AutoReturnSeconds = 10f;
            flags[ETeam.Blue].PickUp("red-player");

            Assert.That(
                CaptureTheFlagRules.ReturnTimedOutFlags(flags, 20.0d),
                Is.EqualTo(ETeam.NoTeam));
        }

        [Test]
        public void AFlagPickedUpBeforeItsDeadlineKeepsIt()
        {
            var flags = FreshFlags();
            flags[ETeam.Blue].AutoReturnSeconds = 10f;
            flags[ETeam.Blue].PickUp("red-player");
            flags[ETeam.Blue].Drop(0.0d);
            flags[ETeam.Blue].PickUp("other-red");

            // The clock stops when the flag is taken again, so a carrier who gets
            // to it before the deadline keeps it rather than losing it to a timer
            // that never noticed it had moved.
            Assert.That(
                CaptureTheFlagRules.ReturnTimedOutFlags(flags, 20.0d),
                Is.EqualTo(ETeam.NoTeam));
            Assert.That(flags[ETeam.Blue].CarrierId, Is.EqualTo("other-red"));
        }

        [Test]
        public void TheAutoReturnWaitIsAlwaysSane()
        {
            var flag = new TeamFlag(ETeam.Red);

            flag.AutoReturnSeconds = float.NaN;
            Assert.That(flag.AutoReturnSeconds, Is.EqualTo(TeamFlag.DefaultAutoReturnSeconds));

            flag.AutoReturnSeconds = -5f;
            Assert.That(flag.AutoReturnSeconds, Is.GreaterThan(0f));
        }

        [Test]
        public void TheRoomSaysTheFlagLimitTheServerIsPlayingTo()
        {
            var setting = new CaptureTheFlagMatchSetting(winCondition: 3);
            var room = new MatchRoom(1, "ctf", "host", setting, new MatchRoomEventBus());

            var state = room.ToJSon();

            // A client used to carry its own idea of this and the server carried
            // another, so a scoreboard could say first to five on a match that
            // ends at three. The rule ends the match, so the rule's number is the
            // one worth saying out loud.
            Assert.That(
                state["WinConditionPoint"]?.ToObject<int>(),
                Is.EqualTo(setting.WinConditionPoint));
        }

        [Test]
        public void TheRoomSaysTheClockItsRuleIsRunningOn()
        {
            var setting = new CaptureTheFlagMatchSetting(winCondition: 3);
            var room = new MatchRoom(1, "ctf", "host", setting, new MatchRoomEventBus());
            room.GameStart();

            var state = room.ToJSon();

            // The rule is what ends the match, so the room state reports the
            // clock the rule is running rather than the one a setting carried.
            // A setting's own field defaults to zero, and reporting that would
            // tell a client the match had no time on it.
            Assert.That(
                state["MatchTimeSeconds"]?.ToObject<float>(),
                Is.EqualTo(room.Rule.MatchTimeMSec() / 1000f));
            Assert.That(state["MatchTimeSeconds"]?.ToObject<float>(), Is.GreaterThan(0f));
            Assert.That(state["IsPlaying"]?.ToObject<bool>(), Is.True);
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
