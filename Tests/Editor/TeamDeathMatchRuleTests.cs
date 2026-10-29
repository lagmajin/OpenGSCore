using NUnit.Framework;
using OpenGSCore;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// Whether a team death match is over, and who it was won by.
    /// <para>
    /// A team's kill count is how that match is won and how its kill limit is
    /// reached, and the rule and the result both read it from the match. Nothing
    /// ever incremented it. So the rule compared two zeroes, the result found
    /// neither side ahead, and a team death match reported a draw whatever had
    /// happened in it. The rule also threw its setting away and ran on a
    /// hardcoded length and a hardcoded kill limit.
    /// </para>
    /// </summary>
    public class TeamDeathMatchRuleTests
    {
        private static MatchRoom CreateRoom(TDMMatchSetting? setting = null, params string[] players)
        {
            setting ??= new TDMMatchSetting(maxPlayerCapacity: 4);

            var room = new MatchRoom(1, "tdm", "host", setting, new MatchRoomEventBus());
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
        public void AKillIsScoredForTheTeamThatMadeIt()
        {
            var room = CreateRoom(players: new[] { "red-one", "blue-one" });
            room.GameStart();

            room.RecordKill("red-one");
            room.RecordKill("red-one");
            room.RecordKill("blue-one");

            // Nothing incremented these before, so the result found neither side
            // ahead and every team death match was a draw.
            Assert.That(room.TeamKillCount(ETeam.Red), Is.EqualTo(2));
            Assert.That(room.TeamKillCount(ETeam.Blue), Is.EqualTo(1));
        }

        [Test]
        public void ATakingTheLeadEndsAMatchDecidedOnKills()
        {
            var room = CreateRoom(players: new[] { "red-one", "blue-one" });
            room.GameStart();

            room.RecordKill("blue-one");
            room.RecordKill("blue-one");
            room.RecordKill("red-one");

            var result = MatchResultEvaluatorFactory
                .CreateEvaluator(EGameMode.TeamDeathMatch)
                .Evaluate(ProbeSituationOf(room), room.Players);

            Assert.That(result["WinningTeam"]?.ToString(), Is.EqualTo("Blue"));
        }

        [Test]
        public void ATeamDeathMatchWithNoKillsInItIsADraw()
        {
            var room = CreateRoom(players: new[] { "red-one", "blue-one" });
            room.GameStart();

            var result = MatchResultEvaluatorFactory
                .CreateEvaluator(EGameMode.TeamDeathMatch)
                .Evaluate(ProbeSituationOf(room), room.Players);

            // Before anything was counted this was the whole of what the mode
            // ever produced: two zeroes compared, neither ahead, a draw.
            Assert.That(result["WinningTeam"]?.ToString(), Is.EqualTo("Draw"));
        }

        [Test]
        public void ATiedScoreIsADrawAndNotAWinForWhoeverIsFirst()
        {
            var room = CreateRoom(players: new[] { "red-one", "blue-one" });
            room.GameStart();

            room.RecordKill("red-one");
            room.RecordKill("blue-one");

            var result = MatchResultEvaluatorFactory
                .CreateEvaluator(EGameMode.TeamDeathMatch)
                .Evaluate(ProbeSituationOf(room), room.Players);

            Assert.That(result["WinningTeam"]?.ToString(), Is.EqualTo("Draw"));
        }

        [Test]
        public void AKillLimitEndsTheMatchWhenATeamReachesIt()
        {
            var setting = new TDMMatchSetting(maxPlayerCapacity: 4) { WinConditionKill = 2 };
            var room = CreateRoom(setting, "red-one", "blue-one");
            room.GameStart();

            room.RecordKill("red-one");
            Assert.That(room.IsMatchFinished(), Is.False);

            room.RecordKill("red-one");
            Assert.That(room.IsMatchFinished(), Is.True);
        }

        [Test]
        public void ATeamDeathMatchRunsOnItsConfiguredLengthAndLimit()
        {
            // The rule took a hardcoded ten minutes and a hardcoded fifty kills and
            // threw the setting away, so a room configured for a longer match with
            // a lower limit was played on numbers nobody had asked for.
            var setting = new TDMMatchSetting(maxPlayerCapacity: 4)
            {
                WinConditionKill = 3,
                MatchTimeMinutes = 7
            };
            var rule = new TDMMatchRule(setting);

            Assert.That(rule.MatchTimeMSec(), Is.EqualTo(7 * 60 * 1000));
            Assert.That(rule.TeamKillLimit, Is.EqualTo(3));
        }

        [Test]
        public void ATeamDeathMatchSettingWithNothingInItFallsBackToADefault()
        {
            var setting = new TDMMatchSetting
            {
                WinConditionKill = 0,
                MatchTimeMSec = 0
            };
            var rule = new TDMMatchRule(setting);

            Assert.That(
                rule.MatchTimeMSec(),
                Is.EqualTo(TDMMatchRule.DefaultMatchTimeMsec));
            Assert.That(rule.TeamKillLimit, Is.EqualTo(TDMMatchRule.DefaultTeamKillLimit));
        }

        [Test]
        public void TheRoomSaysTheTeamKillLimitItIsPlayingTo()
        {
            var setting = new TDMMatchSetting(maxPlayerCapacity: 4) { WinConditionKill = 25 };
            var room = CreateRoom(setting, "red-one", "blue-one");
            room.GameStart();

            Assert.That(room.ToJSon()["TeamKillLimit"]?.ToObject<int>(), Is.EqualTo(25));
        }

        [Test]
        public void AKillBySomebodyWhoIsNotInTheRoomScoresForNobody()
        {
            var room = CreateRoom(players: new[] { "red-one", "blue-one" });
            room.GameStart();

            // A message naming a player this room does not have must not put a
            // point on the board for whichever team that name maps to.
            room.RecordKill("stranger");

            Assert.That(room.TeamKillCount(ETeam.Red), Is.EqualTo(0));
            Assert.That(room.TeamKillCount(ETeam.Blue), Is.EqualTo(0));
        }

        /// <summary>
        /// The room's own situation, for a caller that has to hand the evaluator
        /// something to judge. The evaluator takes a situation rather than asking
        /// the room, which is the shape it has always had.
        /// </summary>
        private static AbstractMatchSituation ProbeSituationOf(MatchRoom room)
        {
            var situation = new AbstractTeamMatchSituation();
            foreach (var team in new[] { ETeam.Red, ETeam.Blue })
            {
                switch (team)
                {
                    case ETeam.Red:
                        situation.RedTeamKill = room.TeamKillCount(ETeam.Red);
                        break;
                    case ETeam.Blue:
                        situation.BlueTeamKill = room.TeamKillCount(ETeam.Blue);
                        break;
                }
            }

            return situation;
        }
    }
}
