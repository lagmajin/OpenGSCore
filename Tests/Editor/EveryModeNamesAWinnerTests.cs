using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenGSCore;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// Every mode, played, and asked who won.
    /// <para>
    /// Each mode had its own version of the same defect, and each one was found by
    /// reading rather than by playing: a team death match compared two numbers
    /// nothing wrote and reported a draw whatever happened in it; a survival match
    /// named a dead player with no kills as the winner of a match won on kills; a
    /// capture the flag match compared counts the room never kept. The rules and
    /// the result disagreed with each other, and in every case the side that was
    /// wrong was the one nobody was looking at.
    /// </para>
    /// <para>
    /// These are the invariants that hold across all of them, so a mode that
    /// drifts again fails here rather than in a match.
    /// </para>
    /// </summary>
    public class EveryModeNamesAWinnerTests
    {
        private static IEnumerable<AbstractMatchSetting> EveryMode()
        {
            yield return new DeathMatchSetting { WinConditionKill = 3, MatchTimeMinutes = 6 };
            yield return new OneShotKillMatchSetting { WinConditionKill = 2, MatchTimeMinutes = 4 };
            yield return new ArmsRaceMatchSetting { WinConditionKill = 4, MatchTimeMinutes = 8 };
            yield return new TDMMatchSetting { WinConditionKill = 3, MatchTimeMinutes = 7 };
            yield return new SuvMatchSetting(4, false) { WinConditionKill = 2, SurvivalTimeMinutes = 6 };
            yield return new TeamSurvivalMatchSetting { SurvivalTimeMinutes = 6 };
            yield return new CaptureTheFlagMatchSetting(3);
        }

        /// <summary>
        /// A room for this setting with two players, one on each side, both
        /// started and unhurt.
        /// </summary>
        private static MatchRoom CreatePlayedRoom(AbstractMatchSetting setting)
        {
            var room = new MatchRoom(1, "played", "host", setting, new MatchRoomEventBus());
            room.AddNewPlayer(new PlayerInfo("red-one", "Red") { Team = ETeam.Red, Health = 100 });
            room.AddNewPlayer(new PlayerInfo("blue-one", "Blue") { Team = ETeam.Blue, Health = 100 });
            room.GameStart();
            return room;
        }

        private static JObject FinishAndGetResult(MatchRoom room)
        {
            var evaluator = MatchResultEvaluatorFactory.CreateEvaluator(room.Setting.Mode);
            return evaluator.Evaluate(SituationOf(room), room.Players);
        }

        [Test]
        public void APlayedMatchInEveryModeSaysSomebodyWon()
        {
            foreach (var setting in EveryMode())
            {
                var room = CreatePlayedRoom(setting);

                // Give the room something to have been decided on, so this is a
                // played match and not a fresh one.
                room.RecordKill("red-one");
                room.RecordKill("blue-one");
                room.RecordKill("red-one");
                room.RecordKill("red-one");

                var result = FinishAndGetResult(room);

                if (IsTeamMode(setting))
                {
                    var team = result["WinningTeam"]?.ToString();
                    Assert.That(
                        team,
                        Is.AnyOf("Red", "Blue"),
                        $"{setting.Mode} played a match and named {team ?? "nothing"}");
                }
                else
                {
                    // A free for all has no winning team, so it names a player
                    // instead. That is the shape the mode is, not a mode that
                    // failed to pick a side.
                    var player = result["WinningPlayerId"]?.ToString();
                    Assert.That(
                        player,
                        Is.Not.Null.And.Not.EqualTo("None"),
                        $"{setting.Mode} played a match and named no winning player");
                }
            }
        }

        [Test]
        public void APlayedMatchInEveryModeNamesARealPlayer()
        {
            foreach (var setting in EveryMode())
            {
                var room = CreatePlayedRoom(setting);
                room.RecordKill("red-one");
                room.RecordKill("red-one");

                var result = FinishAndGetResult(room);
                var winners = PlayersNamedAsWinners(result, room);

                Assert.That(
                    winners,
                    Is.Not.Empty,
                    $"{setting.Mode} named no player as a winner of a match with a winner in it");
            }
        }

        [Test]
        public void EveryModeResultCarriesTheRoomWithIt()
        {
            foreach (var setting in EveryMode())
            {
                var room = CreatePlayedRoom(setting);
                room.RecordKill("red-one");

                var result = FinishAndGetResult(room);

                // The envelope a client is shown is built from this, and it
                // resolves the winners through the players it carries. A result
                // without them says who won the match and nothing about who was
                // in it.
                Assert.That(
                    result["Players"] as JArray,
                    Is.Not.Null,
                    $"{setting.Mode} returned a result with no players in it");
            }
        }

        [Test]
        public void ARoomSaysHowLongItRunsAndHowMuchOfItIsLeft()
        {
            foreach (var setting in EveryMode())
            {
                var room = CreatePlayedRoom(setting);

                var state = room.ToJSon();
                var length = state["MatchLengthSeconds"]?.ToObject<float>();
                var left = state["MatchTimeSeconds"]?.ToObject<float>();

                Assert.That(length, Is.GreaterThan(0f), $"{setting.Mode} published no match length");
                Assert.That(left, Is.GreaterThan(0f), $"{setting.Mode} published no time on its clock");
                Assert.That(left, Is.LessThanOrEqualTo(length.Value));
            }
        }

        [Test]
        public void ATeamRoomSaysTheScoreItIsActuallyOn()
        {
            // A client counts its own kills and sends its own claims, so a score it
            // worked out for itself is a score of its own making for the whole
            // match. The room keeps the real one and publishes it, so a client is
            // showing a number rather than hoping its own is right.
            var setting = new TDMMatchSetting { WinConditionKill = 25 };
            var room = CreatePlayedRoom(setting);
            room.RecordKill("red-one");
            room.RecordKill("red-one");
            room.RecordKill("blue-one");

            var state = room.ToJSon();
            Assert.That(state["RedTeamKills"]?.ToObject<int>(), Is.EqualTo(2));
            Assert.That(state["BlueTeamKills"]?.ToObject<int>(), Is.EqualTo(1));
        }

        [Test]
        public void ATeamModeSaysWhichTeamWonByItsOwnScore()
        {
            foreach (var setting in EveryMode().Where(IsTeamMode))
            {
                var room = CreatePlayedRoom(setting);
                room.RecordKill("red-one");
                room.RecordKill("red-one");
                room.RecordKill("blue-one");

                var result = FinishAndGetResult(room);

                // The teams are told apart by the name they are given, and the
                // result's own score is what says which is ahead. A result whose
                // team does not match its score is a result nobody can read.
                var declared = result["WinningTeam"]?.ToString();
                var red = RedScoreOf(result);
                var blue = BlueScoreOf(result);

                if (declared == "Red")
                {
                    Assert.That(red, Is.GreaterThanOrEqualTo(blue), $"{setting.Mode} declared Red against its own score");
                }
                else if (declared == "Blue")
                {
                    Assert.That(blue, Is.GreaterThanOrEqualTo(red), $"{setting.Mode} declared Blue against its own score");
                }
            }
        }

        [Test]
        public void ARoomResetForOneModeLeavesAnotherModesTotalsAlone()
        {
            // The shared reset reached for the team scores of any team mode, so a
            // method named for the flag could clear the kill totals a team death
            // match is won and ended on.
            var setting = new TDMMatchSetting { WinConditionKill = 5 };
            var room = CreatePlayedRoom(setting);
            room.RecordKill("red-one");

            room.ResetCaptureTheFlagState();

            Assert.That(
                room.TeamKillCount(ETeam.Red),
                Is.EqualTo(1),
                "resetting the flag state cleared a team death match's kill total");
        }

        private static bool IsTeamMode(AbstractMatchSetting setting)
        {
            return setting.Mode == EGameMode.TeamDeathMatch ||
                setting.Mode == EGameMode.TeamSurvival ||
                setting.Mode == EGameMode.CaptureTheFlag;
        }

        private static int RedScoreOf(JObject result)
        {
            foreach (var key in new[] { "RedScore", "RedTeamScore", "RedTeamKills" })
            {
                if (result[key] != null)
                {
                    return result[key]!.Value<int>();
                }
            }

            return 0;
        }

        private static int BlueScoreOf(JObject result)
        {
            foreach (var key in new[] { "BlueScore", "BlueTeamScore", "BlueTeamKills" })
            {
                if (result[key] != null)
                {
                    return result[key]!.Value<int>();
                }
            }

            return 0;
        }

        /// <summary>
        /// The players a result names as having won, resolved the way the server
        /// resolves them: by a named winner, or by the team it declares.
        /// </summary>
        private static List<string> PlayersNamedAsWinners(JObject result, MatchRoom room)
        {
            var winners = new List<string>();

            var named = result["WinningPlayerId"]?.ToString();
            if (!string.IsNullOrWhiteSpace(named) &&
                !named.Equals("None", System.StringComparison.OrdinalIgnoreCase) &&
                !named.Equals("Draw", System.StringComparison.OrdinalIgnoreCase))
            {
                winners.Add(named!);
            }

            var winningTeam = result["WinningTeam"]?.ToString();
            if (!string.IsNullOrWhiteSpace(winningTeam) &&
                !winningTeam.Equals("Draw", System.StringComparison.OrdinalIgnoreCase) &&
                !winningTeam.Equals("None", System.StringComparison.OrdinalIgnoreCase))
            {
                foreach (var player in room.Players.Where(p =>
                    string.Equals(p.Team.ToString(), winningTeam, System.StringComparison.OrdinalIgnoreCase)))
                {
                    if (!winners.Contains(player.Id))
                    {
                        winners.Add(player.Id);
                    }
                }
            }

            return winners;
        }

        /// <summary>
        /// The room's own situation, for a caller that has to hand the evaluator
        /// something to judge. The evaluator takes a situation rather than asking
        /// the room, which is the shape it has always had.
        /// </summary>
        private static AbstractMatchSituation SituationOf(MatchRoom room)
        {
            var team = new TeamSurvivalMatchSituation();
            team.RedTeamKill = room.TeamKillCount(ETeam.Red);
            team.BlueTeamKill = room.TeamKillCount(ETeam.Blue);
            team.SetAliveCount(ETeam.Red, room.AliveCountOn(ETeam.Red));
            team.SetAliveCount(ETeam.Blue, room.AliveCountOn(ETeam.Blue));
            return team;
        }
    }
}
