using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenGSCore
{
    public interface IMatchRoom
    {
        string RoomName { get; set; }
    }

    public partial class MatchRoom : AbstractGameRoom, IMatchRoom
    {
        private readonly MatchRoomEventBus eventBus;

        AbstractMatchRule? rule;

        public AbstractMatchSetting Setting { get; set; }

        private AbstractMatchSituation Situation { get; set; } = null;
        private readonly object playerSyncLock = new();

        public GameScene GameScene { get; set; } = new();

        public WaitRoom? WaitRoomLink { get; set; } = null;
        public string RoomName { get; set; }

        public bool MatchEnd { get; private set; } = false;
        public bool IsSuddenDeathModeNow { get; private set; } = false;

        public int PlayerCount 
        { 
            get 
            { 
                lock (playerSyncLock)
                {
                    return Players.Count;
                }
            } 
        }

        public int Capacity { get; } = 20;

        public bool Playing { get; private set; } = false;
        public bool Finished { get; private set; } = false;

        private Stopwatch sw = new();

        private HighPrecisionGameTimer timer;

        private FieldItemService itemServiceA = new FieldItemService();

        /// <summary>
        /// プレイヤーをルームから削除する
        /// </summary>
        public void RemovePlayer(string playerId)
        {
            lock (playerSyncLock)
            {
                Players.RemoveAll(p => p.Id == playerId);
            }
        }

        public MatchRoom(int roomNumber, in string roomName, in string roomOwnerId, AbstractMatchSetting setting, MatchRoomEventBus bus) : base(roomNumber, roomOwnerId)
        {
            Setting = setting;
            eventBus = bus;

            // create appropriate rule using factory
            rule = MatchRuleFactory.CreateMatchRule(setting);

            switch (setting.Mode)
            {
                case EGameMode.DeathMatch:
                    if (setting is DeathMatchSetting deathMatchSetting)
                    {

                    }
                    break;

                case EGameMode.TeamDeathMatch:
                    if (setting is TDMMatchSetting teamDeathMatchSetting)
                    {

                    }
                    break;
            }
        }

        public bool ChangeOwnerRandom()
        {
            lock (playerSyncLock)
            {
                if (Players.Count < 1)
                {
                    return false;
                }

                var random = new Random();
                var randomIndex = random.Next(Players.Count);
                var newOwner = Players[randomIndex];
                
                // 新しいオーナーを設定
                OwnerId = newOwner.Id;
                return true;
            }
        }

        public void AddNewPlayer(PlayerInfo info)
        {
            if (Playing)
            {
                // 途中参加の処理は未実装
                return;
            }

            lock (playerSyncLock)
            {
                if (!Players.Exists(p => p.Id == info.Id))
                {
                    Players.Add(info);
                }
            }
        }

        public void AddNewPlayers(List<PlayerInfo> list)
        {
            foreach (var info in list)
            {
                AddNewPlayer(info);
            }
        }

        public void OnGameUpdateFromClient()
        {

        }

        public override void GameUpdate()
        {
            if (!Finished)
            {
                GameScene.UpdateFrame();
            }
        }

        private System.Timers.Timer? statusUpdateTimer;

        public void StartStatusUpdates()
        {
            statusUpdateTimer = new System.Timers.Timer(1000); // 1秒ごとに更新
            statusUpdateTimer.Elapsed += (sender, e) => SendPeriodicStatusUpdate();
            statusUpdateTimer.Start();
        }

        public void StopStatusUpdates()
        {
            statusUpdateTimer?.Stop();
            statusUpdateTimer?.Dispose();
        }

        private void SendPeriodicStatusUpdate()
        {
            if (Playing)
            {
                // アイテムAグループの更新 (1秒おき)
                EFieldItemType? spawnedItem;
                var stateChange = itemServiceA.Update(1.0f, out spawnedItem);

                if (stateChange == FieldItemService.ESpawnState.Active && spawnedItem.HasValue)
                {
                    // アイテム出現を通知
                    eventBus.PublishItemSpawn(spawnedItem.Value, 0); // スポーンポイントIDは暫定で0
                }
                else if (stateChange == FieldItemService.ESpawnState.Waiting)
                {
                    // アイテム消滅を通知
                    eventBus.PublishItemDespawn();
                }

                // 基本的なステータス情報を送信
                var status = $"Match Active - Players: {Players.Count}";
                // ネットワーク送信処理（未実装）
            }
        }

        public void GameStart()
        {
            sw.Start();

            if (Setting.TimeLimit)
            {
                //setting.MatchTime;
            }

            // ステータス更新を開始
            StartStatusUpdates();

            Playing = true;
            eventBus.PublishGameStart();
        }

        public void Finish()
        {
            // ステータス更新を停止
            StopStatusUpdates();

            // マッチ終了イベントを発行
            eventBus.PublishGameEnd();

            Playing = false;

            // WaitRoomに終了を通知
            if (WaitRoomLink != null)
            {
                WaitRoomLink.GameIsOver();
                WaitRoomLink = null;
            }
        }

        public JObject ToJson()
        {
            var json = new JObject();

            json["RoomName"] = RoomName;
            json["RoomID"] = Id.ToString();
            json["MaxCapacity"] = 8;
            json["PlayerCount"] = PlayerCount;

            return json;
        }

        public void Dispose()
        {
        }
    }
}