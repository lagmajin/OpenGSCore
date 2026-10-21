using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace OpenGSCore.Tests
{
    /// <summary>
    /// MatchRoomEventBus の発行経路。
    /// <para>
    /// PublishMatchResult と PublishGameUpdateFromClient は中身が空で、購読者を
    /// 宣言しても永久に呼ばれなかった。ここではその二つが実際に届くことを見る。
    /// </para>
    /// </summary>
    public class MatchRoomEventBusTests
    {
        private static (MatchRoom room, MatchRoomEventBus bus) NewRoom(
            AbstractMatchSetting? setting = null)
        {
            var bus = new MatchRoomEventBus();
            var room = new MatchRoom(1, "room", "host", setting ?? new DeathMatchSetting(20, true), bus);
            return (room, bus);
        }

        // ---- Match result ------------------------------------------------

        [Test]
        public void PublishingAMatchResultReachesTheSubscriber()
        {
            var (_, bus) = NewRoom();
            MatchResult? received = null;
            bus.OnMatchResult += result => received = result;

            bus.PublishMatchResult(new MatchResult(MatchResult.Status.Win));

            Assert.That(received, Is.Not.Null);
            Assert.That(received!.CurrentStatus, Is.EqualTo(MatchResult.Status.Win));
        }

        [Test]
        public void PublishingNoResultReachesNobody()
        {
            // A null result is not a result, and handing null to a subscriber
            // makes it dereference something that was never there.
            var (_, bus) = NewRoom();
            int raised = 0;
            bus.OnMatchResult += result => raised++;

            bus.PublishMatchResult(null!);

            Assert.That(raised, Is.EqualTo(0));
        }

        [Test]
        public void ASubscriberThatCameLateStillSeesALaterResult()
        {
            var (_, bus) = NewRoom();
            MatchResult? received = null;

            bus.PublishMatchResult(new MatchResult(MatchResult.Status.Draw));
            bus.OnMatchResult += result => received = result;
            bus.PublishMatchResult(new MatchResult(MatchResult.Status.Lose));

            Assert.That(received!.CurrentStatus, Is.EqualTo(MatchResult.Status.Lose));
        }

        // ---- Room snapshot -----------------------------------------------

        [Test]
        public void PublishingAGameUpdateHandsOverTheRoomSnapshot()
        {
            var (room, bus) = NewRoom();
            bus.setMatchRoom(room);
            JObject? received = null;
            bus.OnGameUpdateFromClient += json => received = json;

            bus.PublishGameUpdateFromClient();

            Assert.That(received, Is.Not.Null);
            Assert.That(received!["RoomID"]!.ToString(), Is.EqualTo(room.Id));
        }

        [Test]
        public void ABusWithNoRoomPublishesNothing()
        {
            // Without a room there is no snapshot to hand over, and publishing a
            // null one would push the problem to whoever subscribed.
            var bus = new MatchRoomEventBus();
            JObject? received = null;
            bus.OnGameUpdateFromClient += json => received = json;

            bus.PublishGameUpdateFromClient();

            Assert.That(received, Is.Null);
        }

        [Test]
        public void TheSnapshotIsTheRoomStateAtTheTimeItWasAskedFor()
        {
            var (room, bus) = NewRoom();
            bus.setMatchRoom(room);
            room.AddNewPlayer(new PlayerInfo("one", "One"));
            JObject? received = null;
            bus.OnGameUpdateFromClient += json => received = json;

            bus.PublishGameUpdateFromClient();

            Assert.That(received!["PlayerCount"]!.Value<int>(), Is.EqualTo(1));
        }

        // ---- Paths that already worked ------------------------------------

        [Test]
        public void GameStartReachesTheSubscriber()
        {
            var (room, bus) = NewRoom();
            int started = 0;
            bus.OnGameStarted += () => started++;

            room.GameStart();

            Assert.That(started, Is.EqualTo(1));
        }

        [Test]
        public void FinishingAMatchReportsItWithItsResult()
        {
            var (room, bus) = NewRoom();
            int ended = 0;
            JObject? result = null;
            bus.OnGameEnded += () => ended++;
            bus.OnGameEndedWithResult += json => result = json;
            room.GameStart();

            room.Finish();

            Assert.That(ended, Is.EqualTo(1));
            Assert.That(result, Is.Not.Null);
            Assert.That(result!["RoomId"]!.ToString(), Is.EqualTo(room.Id));
        }

        [Test]
        public void FinishingAMatchRunsTheScoreBeforeItReports()
        {
            // The result is read out of the match, so the match has to be settled
            // before the end event goes out or the reported score is a stale one.
            var (room, bus) = NewRoom();
            room.AddNewPlayer(new PlayerInfo("one", "One"));
            room.GameStart();
            JObject result = null;
            bus.OnGameEndedWithResult += json => result = json;

            room.Finish();

            Assert.That(result!["Players"], Is.Not.Null);
        }

        [Test]
        public void TheResultReachesSubscribersBeforeTheMatchIsReportedOver()
        {
            // The two are separate events, so the order is the contract: whoever
            // stores the result has to have stored it by the time the end fires.
            // The server's end handler reads the result the other one stored, and
            // with the end first it read the match before this one, so the
            // notification left with no winner and the client called it a draw.
            var (room, bus) = NewRoom();
            room.GameStart();
            var order = new List<string>();
            bus.OnGameEnded += () => order.Add("end");
            bus.OnGameEndedWithResult += json => order.Add("result");

            room.Finish();

            Assert.That(order, Is.EqualTo(new[] { "result", "end" }));
        }

        [Test]
        public void ItemSpawnAndDespawnReachTheSubscriber()
        {
            var (_, bus) = NewRoom();
            EFieldItemType? spawned = null;
            int despawned = 0;
            bus.OnItemSpawned += (type, id) => spawned = type;
            bus.OnItemDespawned += () => despawned++;

            bus.PublishItemSpawn(EFieldItemType.PowerUpItem, 3);
            bus.PublishItemDespawn();

            Assert.That(spawned, Is.EqualTo(EFieldItemType.PowerUpItem));
            Assert.That(despawned, Is.EqualTo(1));
        }
    }
}
