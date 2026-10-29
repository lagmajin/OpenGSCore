using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OpenGSCore;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// The mission mode, which used to be played by the survival rule and settled
    /// as one.
    /// <para>
    /// MissionMatchSetting constructed itself as a survival, so every room built
    /// from it was a survival match with a mission's name on it. The life count
    /// and the objective count were fields on that setting that nothing read, the
    /// result evaluator was never reached, and the life count it would have read
    /// came from a static nobody ever assigned. These pin down what a mission is
    /// played on and how it ends.
    /// </para>
    /// </summary>
    public class MissionMatchTests
    {
        private static MatchRoom NewRoom(int objectiveCount = 1, int lifeCount = 3, bool allowRespawn = false)
        {
            var setting = new MissionMatchSetting
            {
                ObjectiveCount = objectiveCount,
                LifeCount = lifeCount,
                AllowRespawn = allowRespawn
            };
            return new MatchRoom(1, "mission", "host", setting, new MatchRoomEventBus());
        }

        // ---- The mode is a mode ------------------------------------------

        [Test]
        public void AMissionIsBuiltAsAMissionAndNotAsASurvival()
        {
            var setting = new MissionMatchSetting();

            // It declared itself a survival, so the rule, the situation and the
            // evaluator were all somebody else's and its own numbers went nowhere.
            Assert.That(setting.Mode, Is.EqualTo(EGameMode.Mission));
            Assert.That(MatchRuleFactory.CreateMatchRule(setting), Is.InstanceOf<MissionMatchRule>());
            Assert.That(
                MatchResultEvaluatorFactory.CreateEvaluator(EGameMode.Mission),
                Is.InstanceOf<MissionResultEvaluator>());
        }

        [Test]
        public void AMissionRoomHasItsOwnSituation()
        {
            var room = NewRoom();

            // It was given the survival situation, which has a kill count and an
            // alive count, neither of which says how far a mission has got.
            Assert.That(room.CurrentSituation, Is.InstanceOf<MissionMatchSituation>());
        }

        [Test]
        public void AMissionEndsOnAClockLikeEveryOtherMode()
        {
            var rule = (MissionMatchRule)MatchRuleFactory.CreateMatchRule(
                new MissionMatchSetting { ObjectiveCount = 0, LifeCount = 0 });
            var situation = new MissionMatchSituation { RemainingTimeSec = 600f };

            Assert.That(rule.IsMatchFinished(situation), Is.False);

            situation.RemainingTimeSec = 0f;
            Assert.That(rule.IsMatchFinished(situation), Is.True);
        }

        // ---- The objective -----------------------------------------------

        [Test]
        public void ACompletedObjectiveEndsTheMission()
        {
            var rule = (MissionMatchRule)MatchRuleFactory.CreateMatchRule(
                new MissionMatchSetting { ObjectiveCount = 3, LifeCount = 3 });
            var situation = new MissionMatchSituation { RemainingTimeSec = 600f };

            Assert.That(rule.RecordObjectiveProgress(1), Is.False);
            Assert.That(rule.RecordObjectiveProgress(1), Is.False);
            Assert.That(rule.IsMatchFinished(situation), Is.False);

            Assert.That(rule.RecordObjectiveProgress(1), Is.True);
            Assert.That(rule.IsMatchFinished(situation), Is.True);
        }

        [Test]
        public void TheObjectiveCannotBeGonePastItsTarget()
        {
            var rule = (MissionMatchRule)MatchRuleFactory.CreateMatchRule(
                new MissionMatchSetting { ObjectiveCount = 2 });

            rule.RecordObjectiveProgress(10);

            // A mission that reports five of a thing when it asked for two is a
            // mission whose scoreboard is a lie, and the rule would compare against
            // a number nothing else agrees with.
            Assert.That(rule.ObjectiveProgress, Is.EqualTo(2));
        }

        [Test]
        public void AMissionWithNoObjectiveIsNotEndedOnOne()
        {
            var rule = (MissionMatchRule)MatchRuleFactory.CreateMatchRule(
                new MissionMatchSetting { ObjectiveCount = 0, LifeCount = 3 });

            // Otherwise a mission with no objective would be over before it began.
            Assert.That(rule.RecordObjectiveProgress(1), Is.False);
            Assert.That(rule.IsMatchFinished(new MissionMatchSituation { RemainingTimeSec = 600f }), Is.False);
        }

        [Test]
        public void TheRoomRecordsTheObjectiveAndSaysSoMuch()
        {
            var room = NewRoom(objectiveCount: 4);
            room.AddNewPlayer(new PlayerInfo("p1", "Player"));
            room.GameStart();

            room.RecordObjectiveProgress(2);

            var state = room.ToJSon();
            Assert.That(state["ObjectiveCount"]?.ToObject<int>(), Is.EqualTo(4));
            Assert.That(state["ObjectiveProgress"]?.ToObject<int>(), Is.EqualTo(2));

            // The rule and the room state read the same number, so a client
            // showing two of four is showing the match that is being played.
            Assert.That(((MissionMatchRule)room.Rule).ObjectiveProgress, Is.EqualTo(2));
            Assert.That(((MissionMatchSituation)room.CurrentSituation).ObjectiveProgress, Is.EqualTo(2));
        }

        // ---- The lives ---------------------------------------------------

        [Test]
        public void PlayersStartOnTheNumberOfLivesTheMissionWasGiven()
        {
            var room = NewRoom(lifeCount: 5);
            room.AddNewPlayer(new PlayerInfo("p1", "Player"));
            room.AddNewPlayer(new PlayerInfo("p2", "Player"));
            room.GameStart();

            // The count was declared as three on the player and never written for a
            // match, so it kept whatever was left over from the last attempt.
            foreach (var player in room.Players)
            {
                Assert.That(player.LifeCount, Is.EqualTo(5));
            }
        }

        [Test]
        public void GoingDownSpendsOneOfThePartysLives()
        {
            var room = NewRoom(lifeCount: 2);
            room.AddNewPlayer(new PlayerInfo("p1", "Player"));
            room.GameStart();

            room.RecordDeath("p1");

            Assert.That(((MissionMatchRule)room.Rule).LifeRemaining, Is.EqualTo(1));
            Assert.That(((MissionMatchSituation)room.CurrentSituation).LifeRemaining, Is.EqualTo(1));
        }

        [Test]
        public void RunningOutOfLivesEndsTheMission()
        {
            var room = NewRoom(lifeCount: 1, objectiveCount: 0);
            var player = new PlayerInfo("p1", "Player");
            room.AddNewPlayer(player);
            room.GameStart();

            room.RecordDeath("p1");

            Assert.That(player.LifeCount, Is.EqualTo(0));
            Assert.That(room.Rule.IsMatchFinished(room.CurrentSituation), Is.True);
        }

        [Test]
        public void EveryPlayerHasTheSameNumberOfLivesAndTheMissionEndsWithTheLastOne()
        {
            // Each player is given the count, rather than the party sharing one
            // pool. The mission scenes carry a life slot per life per player, so a
            // shared pool would show a player losing lives they had not spent. The
            // mission ends when the last of them is out, which is the whole of the
            // party's lives spent.
            var room = NewRoom(lifeCount: 3);
            room.AddNewPlayer(new PlayerInfo("p1", "Player"));
            room.AddNewPlayer(new PlayerInfo("p2", "Player"));
            room.AddNewPlayer(new PlayerInfo("p3", "Player"));
            room.GameStart();

            var rule = (MissionMatchRule)room.Rule;

            room.RecordDeath("p1");
            room.RecordDeath("p1");
            room.RecordDeath("p1");

            // One player is out. Two are still on three lives each.
            Assert.That(room.Players.Single(p => p.Id == "p1").LifeCount, Is.EqualTo(0));
            Assert.That(room.Players.Single(p => p.Id == "p2").LifeCount, Is.EqualTo(3));
            Assert.That(rule.LifeRemaining, Is.EqualTo(6));
            Assert.That(rule.IsMatchFinished(room.CurrentSituation), Is.False);

            foreach (var id in new[] { "p2", "p3" })
            {
                room.RecordDeath(id);
                room.RecordDeath(id);
                room.RecordDeath(id);
            }

            Assert.That(rule.LifeRemaining, Is.EqualTo(0));
            Assert.That(rule.IsMatchFinished(room.CurrentSituation), Is.True);
        }

        [Test]
        public void AMissionWithNoLivesIsNotEndedOnRunningOut()
        {
            var room = NewRoom(lifeCount: 0, objectiveCount: 0);
            room.AddNewPlayer(new PlayerInfo("p1", "Player"));
            room.GameStart();

            room.RecordDeath("p1");

            // Otherwise a mission configured with no lives would be over the moment
            // anybody went down, which is not what no lives means.
            Assert.That(room.Rule.IsMatchFinished(room.CurrentSituation), Is.False);
        }

        [Test]
        public void RespawningIsOnlyAllowedWhenTheMissionSaysSo()
        {
            var withRespawn = (MissionMatchRule)MatchRuleFactory.CreateMatchRule(
                new MissionMatchSetting { AllowRespawn = true });
            var withoutRespawn = (MissionMatchRule)MatchRuleFactory.CreateMatchRule(
                new MissionMatchSetting { AllowRespawn = false });

            Assert.That(withRespawn.CanReSpawn(), Is.True);
            Assert.That(withoutRespawn.CanReSpawn(), Is.False);
        }

        // ---- The result --------------------------------------------------

        [Test]
        public void ACompletedObjectiveIsAClear()
        {
            var room = NewRoom(objectiveCount: 1, lifeCount: 3);
            room.AddNewPlayer(new PlayerInfo("p1", "Player"));
            room.GameStart();
            room.RecordObjectiveProgress(1);

            var result = MatchResultEvaluatorFactory
                .CreateEvaluator(EGameMode.Mission)
                .Evaluate(room.CurrentSituation, room.Players);

            Assert.That(result["Result"]?.ToString(), Is.EqualTo("Clear"));
            Assert.That(result["Success"]?.ToObject<bool>(), Is.True);
            Assert.That(result["ObjectiveCompleted"]?.ToObject<bool>(), Is.True);
        }

        [Test]
        public void RunningOutOfLivesIsAFail()
        {
            var room = NewRoom(objectiveCount: 0, lifeCount: 1);
            room.AddNewPlayer(new PlayerInfo("p1", "Player"));
            room.GameStart();
            room.RecordDeath("p1");

            var result = MatchResultEvaluatorFactory
                .CreateEvaluator(EGameMode.Mission)
                .Evaluate(room.CurrentSituation, room.Players);

            Assert.That(result["Result"]?.ToString(), Is.EqualTo("Fail"));
            Assert.That(result["Success"]?.ToObject<bool>(), Is.False);
        }

        [Test]
        public void RunningOutOfTheClockIsATimeUpAndNotAFail()
        {
            var room = NewRoom(objectiveCount: 0, lifeCount: 3);
            room.AddNewPlayer(new PlayerInfo("p1", "Player"));
            room.GameStart();

            var result = MatchResultEvaluatorFactory
                .CreateEvaluator(EGameMode.Mission)
                .Evaluate(room.CurrentSituation, room.Players);

            // The party is still alive, so this is not a failure and a mission
            // screen that said it was one would be telling the player their
            // attempt did not count.
            Assert.That(result["Result"]?.ToString(), Is.EqualTo("TimeUp"));
            Assert.That(result["Success"]?.ToObject<bool>(), Is.True);
        }

        [Test]
        public void AMissionWithNobodyInItFailsRatherThanThrowing()
        {
            var result = MatchResultEvaluatorFactory
                .CreateEvaluator(EGameMode.Mission)
                .Evaluate(new MissionMatchSituation(), new List<PlayerInfo>());

            Assert.That(result["Success"]?.ToObject<bool>(), Is.False);
            Assert.That(result["Reason"]?.ToString(), Is.EqualTo("NoPlayers"));
        }

        [Test]
        public void TheMissionSaysHowFarItGotAndWhatItHadLeft()
        {
            var room = NewRoom(objectiveCount: 3, lifeCount: 2);
            var player = new PlayerInfo("p1", "Player");
            room.AddNewPlayer(player);
            room.GameStart();
            room.RecordObjectiveProgress(1);
            room.RecordDeath("p1");

            var result = MatchResultEvaluatorFactory
                .CreateEvaluator(EGameMode.Mission)
                .Evaluate(room.CurrentSituation, room.Players);

            Assert.That(result["ObjectiveCount"]?.ToObject<int>(), Is.EqualTo(3));
            Assert.That(result["ObjectiveProgress"]?.ToObject<int>(), Is.EqualTo(1));
            Assert.That(result["LifeRemaining"]?.ToObject<int>(), Is.EqualTo(1));
        }
    }
}