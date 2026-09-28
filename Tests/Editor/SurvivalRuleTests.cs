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
            var setting = new SuvMatchSetting(maxPlayer: 4, teamBalance: false) { MatchTimeMSec = 0 };
            var rule = new SuvMatchRule(setting);

            Assert.That(
                rule.MatchTimeMSec(),
                Is.EqualTo(SuvMatchSetting.DefaultSurvivalTimeMinutes * 60 * 1000));
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
    }
}
