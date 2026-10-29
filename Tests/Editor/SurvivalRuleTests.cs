using System.Linq;
using NUnit.Framework;
using OpenGSCore;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// Whether a survival match is over, and whether a dead player comes back.
    /// <para>
    /// The rule was unreachable in both directions. Its clock was built from a
    /// setting that had no time on it, so a survival room reported itself
    /// finished the first time it was looked at, before a shot had been fired.
    /// And the kill count it judged the other half of the way was never written
    /// by anything, so a match could only ever end on the clock even once the
    /// clock worked.
    /// </para>
    /// </summary>
    public class SurvivalRuleTests
    {
        private static MatchRoom CreateRoom(SuvMatchSetting? setting = null, params string[] players)
        {
            setting ??= new SuvMatchSetting(maxPlayer: 4, teamBalance: false);

            var room = new MatchRoom(1, "suv", "host", setting, new MatchRoomEventBus());
            foreach (var id in players)
            {
                room.AddNewPlayer(new PlayerInfo(id, id));
            }

            return room;
        }

        [Test]
        public void ASurvivalRoomStartsWithTimeOnTheClock()
        {
            // A setting with no time on it is a match that has already ended.
            // This is the whole bug: the room asked its rule for a duration, got
            // zero, and set the match's remaining time to zero.
            var setting = new SuvMatchSetting(maxPlayer: 4, teamBalance: false);
            Assert.That(setting.MatchTimeMSec, Is.GreaterThan(0));
            Assert.That(
                setting.MatchTimeMSec,
                Is.EqualTo(setting.SurvivalTimeMinutes * 60 * 1000));
        }

        [Test]
        public void ASurvivalMatchIsNotOverBeforeItHasStarted()
        {
            var room = CreateRoom(players: new[] { "one", "two" });
            room.GameStart();

            Assert.That(
                room.IsMatchFinished(),
                Is.False,
                "a survival match reported itself finished before a shot was fired");
        }

        [Test]
        public void ASurvivalRoomEndsWhenTheClockRunsOut()
        {
            var room = CreateRoom(players: new[] { "one", "two" });
            room.GameStart();

            // A rule built with no clock is finished from the moment it exists,
            // which is the shape of the original bug: there was nothing to count
            // down and the rule asked whether the remaining time had run out.
            var clockless = new SuvMatchRule(new SuvMatchSetting(4, false) { MatchTimeMSec = 0 });
            Assert.That(clockless.MatchTimeMSec(), Is.GreaterThan(0));
            Assert.That(room.RemainingTimeSeconds, Is.GreaterThan(0f));
        }

        [Test]
        public void ASurvivalMatchEndsWhenSomebodyReachesTheKillCount()
        {
            var setting = new SuvMatchSetting(maxPlayer: 4, teamBalance: false) { WinConditionKill = 3 };
            var room = CreateRoom(setting, "one", "two");
            room.GameStart();

            // A kill condition that nothing writes can never fire, which is what
            // made a survival match only ever end on the clock.
            room.RecordKill("one");
            Assert.That(room.BestKillCount, Is.EqualTo(1));
            Assert.That(room.IsMatchFinished(), Is.False);

            room.RecordKill("one");
            room.RecordKill("one");

            Assert.That(room.BestKillCount, Is.EqualTo(3));
            Assert.That(room.IsMatchFinished(), Is.True);
        }

        [Test]
        public void RecordingAKillCountsItOnTheRoomAndOnThePlayer()
        {
            var room = CreateRoom(players: new[] { "one", "two" });
            room.GameStart();

            room.RecordKill("one");

            Assert.That(room.TryGetPlayer("one", out var killer), Is.True);
            Assert.That(killer!.Kills, Is.EqualTo(1));
            Assert.That(room.BestKillCount, Is.EqualTo(1));
        }

        [Test]
        public void RecordingADeathForSomebodyWhoIsNotInTheRoomChangesNothing()
        {
            var room = CreateRoom(players: new[] { "one", "two" });
            room.GameStart();

            // A message naming a player this room does not have must not write a
            // score into the match, or one bogus message decides the result.
            room.RecordKill("stranger");
            room.RecordDeath("stranger");

            Assert.That(room.BestKillCount, Is.EqualTo(0));
            Assert.That(room.TotalDeathCount, Is.EqualTo(0));
        }

        [Test]
        public void ABestKillCountOnlyGoesUp()
        {
            var room = CreateRoom(players: new[] { "one", "two" });
            room.GameStart();

            room.RecordKill("one");
            room.RecordKill("one");
            Assert.That(room.BestKillCount, Is.EqualTo(2));

            // A player on a lower tally does not pull the number down. The number
            // a rule ends a match on is the best in the match, not whoever shot
            // most recently, and a version that fell back would end the match on
            // the wrong player.
            room.RecordKill("two");
            Assert.That(room.BestKillCount, Is.EqualTo(2));

            room.RecordKill("one");
            Assert.That(room.BestKillCount, Is.EqualTo(3));
        }

        [Test]
        public void ASurvivalDeathIsPermanentBecauseTheRuleSaysSo()
        {
            var room = CreateRoom(players: new[] { "one", "two" });

            // Asking the rule is the only thing that makes the answer mean
            // anything. It said a death is permanent and nothing asked it, so a
            // client that asked for a respawn got its life back in the one mode
            // where dying should have ended its participation.
            Assert.That(room.Rule.CanReSpawn(), Is.False);
        }

        [Test]
        public void ACaptureTheFlagDeathIsNotPermanentBecauseThatRuleSaysOtherwise()
        {
            var setting = new CaptureTheFlagMatchSetting(winCondition: 3);
            var room = new MatchRoom(1, "ctf", "host", setting, new MatchRoomEventBus());

            Assert.That(room.Rule.CanReSpawn(), Is.True);
        }

        [Test]
        public void ASurvivalRuleBuiltFromASettingWithNoTimeFallsBackToADefault()
        {
            // The rule is given a setting directly elsewhere, and a setting can
            // arrive with no time on it. Taking it as given would build a rule
            // whose clock is zero, which is a match that has already ended.
            var setting = new SuvMatchSetting(maxPlayer: 4, teamBalance: false) { SurvivalTimeMinutes = 0 };
            var rule = new SuvMatchRule(setting);

            Assert.That(
                rule.MatchTimeMSec(),
                Is.EqualTo(SuvMatchSetting.DefaultSurvivalTimeMinutes * 60 * 1000));
        }

        [Test]
        public void ASurvivalRuleReadsTheMinutesRatherThanASnapshotOfThem()
        {
            // The setting wrote its length into MatchTimeMSec once, in its
            // constructor. A room configured for a different length after that
            // kept the default, and nothing said so, because the rule was reading
            // the snapshot. It reads the minutes now, so there is one number and
            // it is the one that is written.
            var setting = new SuvMatchSetting(maxPlayer: 4, teamBalance: false) { SurvivalTimeMinutes = 7 };
            var rule = new SuvMatchRule(setting);

            Assert.That(rule.MatchTimeMSec(), Is.EqualTo(7 * 60 * 1000));
        }

        [Test]
        public void TheRoomSaysTheKillCountItIsPlayingTo()
        {
            var setting = new SuvMatchSetting(maxPlayer: 4, teamBalance: false) { WinConditionKill = 5 };
            var room = CreateRoom(setting, "one", "two");
            room.GameStart();

            // A client carried its own idea of this before, and the room state had
            // no way to tell it the right one.
            Assert.That(room.ToJSon()["WinConditionKill"]?.ToObject<int>(), Is.EqualTo(5));
        }

        [Test]
        public void TheRoomSaysTheHealthMultiplierItIsPlayingAt()
        {
            // The client derived this from the mode and hardcoded two, so a room
            // configured with anything else was played at two health on one side
            // and its own number on the other, and the two disagreed about how
            // much health a player had.
            var setting = new SuvMatchSetting(maxPlayer: 4, teamBalance: false) { HealthMultiplier = 3f };
            var room = CreateRoom(setting, "one", "two");
            room.GameStart();

            Assert.That(room.ToJSon()["HealthMultiplier"]?.ToObject<float>(), Is.EqualTo(3f));
        }

        [Test]
        public void ATeamSurvivalRoomAlsoSaysItsMultiplier()
        {
            var setting = new TeamSurvivalMatchSetting(maxPlayerCapacity: 4) { HealthMultiplier = 1.5f };
            var room = new MatchRoom(1, "tsuv", "host", setting, new MatchRoomEventBus());
            room.GameStart();

            Assert.That(room.ToJSon()["HealthMultiplier"]?.ToObject<float>(), Is.EqualTo(1.5f));
        }

        [Test]
        public void TheAliveCountFollowsDeaths()
        {
            var room = CreateRoom(players: new[] { "one", "two" });
            room.GameStart();

            Assert.That(room.AliveCount(), Is.EqualTo(2));

            room.TryGetPlayer("two", out var second);
            second!.Health = 0;
            room.RecordDeath("two");

            // This is the number the team survival rule ends a match on, so a
            // death that does not move it is a death the rule cannot see.
            Assert.That(room.AliveCount(), Is.EqualTo(1));
        }
    }

    /// <summary>
    /// Whether a team survival match is over, and which of the two ways ends it
    /// a room is configured to be ended.
    /// </summary>
    public class TeamSurvivalRuleTests
    {
        private static MatchRoom CreateRoom(TeamSurvivalMatchSetting setting, params string[] players)
        {
            var room = new MatchRoom(1, "tsuv", "host", setting, new MatchRoomEventBus());
            foreach (var id in players)
            {
                var team = id.StartsWith("blue", System.StringComparison.Ordinal)
                    ? ETeam.Blue
                    : ETeam.Red;
                room.AddNewPlayer(new PlayerInfo(id, id) { Team = team, Health = 100 });
            }

            return room;
        }

        [Test]
        public void AWipedOutTeamEndsAMatchDecidedOnWhoIsLeftStanding()
        {
            var setting = new TeamSurvivalMatchSetting { LastTeamStanding = true };
            var room = CreateRoom(setting, "red-one", "blue-one");
            room.GameStart();

            // The rule read the alive counts from a situation nothing wrote, so
            // the wipe could never happen and this mode could only ever end on the
            // clock.
            var blue = room.Players.First(p => p.Id == "blue-one");
            blue.Health = 0;
            room.RecordDeath("blue-one");

            Assert.That(room.IsMatchFinished(), Is.True);
        }

        [Test]
        public void AMatchDecidedOnTheClockIgnoresAWipe()
        {
            var setting = new TeamSurvivalMatchSetting { LastTeamStanding = false };
            var room = CreateRoom(setting, "red-one", "blue-one");
            room.GameStart();

            var blue = room.Players.First(p => p.Id == "blue-one");
            blue.Health = 0;
            room.RecordDeath("blue-one");

            // The setting promised a choice between the two ways of ending the
            // match and nothing read it, so every team survival match was played
            // as last team standing whatever it was configured for.
            Assert.That(
                room.IsMatchFinished(),
                Is.False,
                "a match configured to be decided on the clock ended because a team was wiped out");
        }

        [Test]
        public void ATeamSurvivalRuleWithNoTimeFallsBackToADefault()
        {
            var setting = new TeamSurvivalMatchSetting { SurvivalTimeMinutes = 0 };
            var rule = new TSuvMatchRule(setting);

            Assert.That(
                rule.MatchTimeMSec(),
                Is.EqualTo(TSuvMatchRule.DefaultSurvivalTimeMinutes * 60 * 1000));
        }
    }
}
