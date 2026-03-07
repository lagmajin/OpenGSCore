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
        private AbstractMatchSituation situation;

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

            // ルールと状況の初期化
            rule = MatchRuleFactory.CreateMatchRule(setting);
            
            // モードに応じて適切な Situation を作成
            if (setting.Mode == EGameMode.TeamDeathMatch || setting.Mode == EGameMode.CaptureTheFlag)
            {
                situation = new AbstractTeamMatchSituation();
            }
            else
            {
                situation = new AbstractMatchSituation();
            }
            
            situation.RemainingTimeSec = (rule != null) ? rule.MatchTimeMSec() / 1000f : 300f;

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
            if (Playing && !Finished)
            {
                float deltaTime = 1.0f; // 1秒周期

                // マッチ状況の更新
                situation.UpdateTime(deltaTime);

                // アイテムAグループの更新
                EFieldItemType? spawnedItem;
                var stateChange = itemServiceA.Update(deltaTime, out spawnedItem);

                if (stateChange == FieldItemService.ESpawnState.Active && spawnedItem.HasValue)
                {
                    eventBus.PublishItemSpawn(spawnedItem.Value, 0);
                }
                else if (stateChange == FieldItemService.ESpawnState.Waiting)
                {
                    eventBus.PublishItemDespawn();
                }

                // 勝敗判定の実行
                if (rule != null && rule.IsMatchFinished(situation))
                {
                    Finish();
                    return;
                }

                // 基本的なステータス情報をブロードキャスト
                var status = $"Match Active - Players: {Players.Count}, Time: {situation.RemainingTimeSec}";
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

            // リザルト情報の作成
            var resultJson = new JObject();
            resultJson["MessageType"] = MessageType.MatchEndNotification;
            resultJson["RoomId"] = Id.ToString();
            
            // 勝者の判定（DeathMatch想定: 最高キル数）
            string winnerInfo = "Draw";
            if (situation is AbstractTeamMatchSituation teamSit)
            {
                if (teamSit.RedTeamKill > teamSit.BlueTeamKill) winnerInfo = "Red";
                else if (teamSit.BlueTeamKill > teamSit.RedTeamKill) winnerInfo = "Blue";
                
                resultJson["WinnerTeam"] = winnerInfo;
                resultJson["RedScore"] = teamSit.RedTeamKill;
                resultJson["BlueScore"] = teamSit.BlueTeamKill;
            }
            else
            {
                // 個人戦の勝者判定（暫定）
                resultJson["Winner"] = "Draw"; 
            }

            // マッチ終了イベントを発行
            eventBus.PublishGameEnd();
            eventBus.PublishGameEndWithResult(resultJson);

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