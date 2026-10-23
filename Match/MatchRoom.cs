#nullable enable
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace OpenGSCore
{
    public interface IMatchRoom
    {
        string RoomName { get; set; }
    }

    /// <summary>
    /// OpenGS MatchRoom - ゲームマッチの状態管理クラス
    /// 同時実行性（Simultaneous Processing）をサポートするように設計されています。
    /// </summary>
    public partial class MatchRoom : AbstractGameRoom, IMatchRoom, ISyncable
    {
        private readonly MatchRoomEventBus eventBus;

        AbstractMatchRule? rule;

        public AbstractMatchSetting Setting { get; set; } = null!;

        private AbstractMatchSituation? Situation { get; set; } = null;
        private readonly object playerSyncLock = new();
        private readonly object finishSyncLock = new();

        public GameScene GameScene { get; set; } = new();

        public WaitRoom? WaitRoomLink { get; set; } = null;
        public string RoomName { get; set; }

        public bool MatchEnd { get; private set; } = false;
        public bool IsSuddenDeathModeNow { get; private set; } = false;

        // 同時処理（Simultaneous Tick）のための入力バッファ
        private readonly ConcurrentQueue<JObject> _inputBuffer = new();

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

        public void AddFlagCapture(ETeam team)
        {
            lock (playerSyncLock)
            {
                if (situation is not AbstractTeamMatchSituation teamSituation) return;
                if (team == ETeam.Red) teamSituation.RedTeamFlagCaptures++;
                else if (team == ETeam.Blue) teamSituation.BlueTeamFlagCaptures++;
            }
        }

        public int GetFlagScore(ETeam team)
        {
            lock (playerSyncLock)
            {
                if (situation is not AbstractTeamMatchSituation teamSituation) return 0;
                return team switch
                {
                    ETeam.Red => teamSituation.RedTeamFlagCaptures,
                    ETeam.Blue => teamSituation.BlueTeamFlagCaptures,
                    _ => 0
                };
            }
        }

        public int Capacity { get; } = 20;

        public bool Playing { get; private set; } = false;
        public bool Finished { get; private set; } = false;
        public bool CanRespawn => rule?.CanReSpawn() ?? false;

        private Stopwatch sw = new();

        private HighPrecisionGameTimer? timer;

        private FieldItemService itemServiceA = new FieldItemService();
        private AbstractMatchSituation situation;

        /// <summary>
        /// 外部から入力をバッファに追加する（マルチスレッドセーフ）
        /// </summary>
        public void PushInput(JObject input)
        {
            _inputBuffer.Enqueue(input);
        }

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
            Id = Guid.NewGuid().ToString("N");
            RoomName = roomName;

            // ルールと状況の初期化
            rule = MatchRuleFactory.CreateMatchRule(setting);
            
            // モードに応じて適切な Situation を作成
            situation = setting.Mode switch
            {
                EGameMode.TeamDeathMatch => new AbstractTeamMatchSituation(),
                EGameMode.CaptureTheFlag => new CaptureTheFlagMatchSituation(),
                EGameMode.TeamSurvival => new TeamSurvivalMatchSituation(),
                EGameMode.DeathMatch => new AbstractMatchSituation(),
                _ => new AbstractMatchSituation()
            };
            
            situation.mode = setting.Mode;
            situation.RemainingTimeSec = (rule != null) ? rule.MatchTimeMSec() / 1000f : 300f;
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
                // 途中参加の処理は要検討
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

        public bool TryGetPlayer(string playerId, out PlayerInfo? player)
        {
            lock (playerSyncLock)
            {
                player = null;
                if (string.IsNullOrWhiteSpace(playerId))
                {
                    return false;
                }

                player = Players.FirstOrDefault(p => p.Id == playerId);
                return player != null;
            }
        }

        public bool ContainsPlayer(string playerId)
        {
            lock (playerSyncLock)
            {
                if (string.IsNullOrWhiteSpace(playerId))
                {
                    return false;
                }

                return Players.Any(p => p.Id == playerId);
            }
        }

        public bool TryApplyPlayerDamage(
            string attackerId, string targetId, int baseDamage,
            out int damageApplied, out int remainingHealth, out bool wasKilled)
        {
            return TryApplyPlayerDamage(attackerId, targetId, baseDamage, false,
                out damageApplied, out remainingHealth, out wasKilled);
        }

        public bool TryApplyPlayerDamage(
            string attackerId, string targetId, int baseDamage, bool allowDeadAttacker,
            out int damageApplied, out int remainingHealth, out bool wasKilled)
        {
            damageApplied = 0;
            remainingHealth = 0;
            wasKilled = false;
            if (string.IsNullOrWhiteSpace(attackerId) || string.IsNullOrWhiteSpace(targetId) ||
                attackerId == targetId || baseDamage <= 0 || !Playing || Finished)
                return false;

            lock (playerSyncLock)
            {
                var attacker = Players.FirstOrDefault(player => player.Id == attackerId);
                var target = Players.FirstOrDefault(player => player.Id == targetId);
                if (attacker == null || target == null ||
                    (!allowDeadAttacker && attacker.Health <= 0) || target.Health <= 0)
                    return false;

                var damageRate = !float.IsNaN(Setting.DamageRate) && !float.IsInfinity(Setting.DamageRate) &&
                    Setting.DamageRate > 0f
                    ? Setting.DamageRate : 1f;
                var scaledDamage = (int)Math.Min(1000d, Math.Max(1d, Math.Ceiling(baseDamage * (double)damageRate)));
                damageApplied = Math.Min(target.Health, scaledDamage);
                target.Health = Math.Max(0, target.Health - damageApplied);
                remainingHealth = target.Health;
                wasKilled = target.Health == 0;

                if (wasKilled)
                {
                    target.Deaths++;
                    attacker.Kills++;
                    situation.TotalKill++;
                    situation.TotalDeath++;
                    situation.MaxPlayerKillCount = Math.Max(situation.MaxPlayerKillCount, attacker.Kills);
                    if (situation is AbstractTeamMatchSituation teamSituation)
                    {
                        if (attacker.Team == ETeam.Red) teamSituation.RedTeamKill++;
                        else if (attacker.Team == ETeam.Blue) teamSituation.BlueTeamKill++;
                    }
                    if (situation is TeamSurvivalMatchSituation teamSurvival &&
                        (target.Team == ETeam.Red || target.Team == ETeam.Blue))
                    {
                        int aliveCount = Players.Count(player => player.Team == target.Team && player.Health > 0);
                        teamSurvival.SetAliveCount(target.Team, aliveCount);
                    }
                }

                situation.AlivePlayers = Players.Count(player => player.Health > 0);
                return true;
            }
        }

        public bool TryRespawnPlayer(string playerId, out int currentHealth, out int maxHealth)
        {
            currentHealth = 0;
            maxHealth = 0;
            if (!CanRespawn || !Playing || Finished || string.IsNullOrWhiteSpace(playerId))
                return false;

            lock (playerSyncLock)
            {
                var player = Players.FirstOrDefault(candidate => candidate.Id == playerId);
                if (player == null || player.Health > 0)
                    return false;

                maxHealth = Math.Max(player.MaxHealth, 1);
                player.MaxHealth = maxHealth;
                player.Health = maxHealth;
                currentHealth = player.Health;
                situation.AlivePlayers = Players.Count(candidate => candidate.Health > 0);
                return true;
            }
        }

        public bool TryHealPlayer(
            string playerId, int requestedHealing,
            out int restoredHealth, out int currentHealth, out int maxHealth)
        {
            restoredHealth = 0;
            currentHealth = 0;
            maxHealth = 0;
            if (!Playing || Finished || string.IsNullOrWhiteSpace(playerId) || requestedHealing <= 0)
                return false;

            lock (playerSyncLock)
            {
                var player = Players.FirstOrDefault(candidate => candidate.Id == playerId);
                if (player == null || player.Health <= 0) return false;
                maxHealth = Math.Max(player.MaxHealth, 1);
                player.MaxHealth = maxHealth;
                currentHealth = Math.Clamp(player.Health, 0, maxHealth);
                restoredHealth = Math.Min(requestedHealing, maxHealth - currentHealth);
                if (restoredHealth <= 0) return false;
                player.Health = currentHealth + restoredHealth;
                currentHealth = player.Health;
                return true;
            }
        }

        public void AddNewPlayers(List<PlayerInfo> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            foreach (var info in list)
            {
                AddNewPlayer(info);
            }
        }

        /// <summary>
        /// 同時更新処理 (Simultaneous Tick Update)
        /// バッファに蓄積された全入力を一括処理してからシーンを更新する
        /// </summary>
        public override void GameUpdate()
        {
            if (Finished) return;

            // 1. バッファに溜まった全入力を処理
            while (_inputBuffer.TryDequeue(out var input))
            {
                ProcessBufferedInput(input);
            }

            // 2. ゲームシーンのフレーム更新（物理・ロジック）
            GameScene.UpdateFrame();

            // 3. ルールチェック（終了判定など）
            if (Playing && rule != null && rule.IsMatchFinished(situation))
            {
                Finish();
            }
        }

        private void ProcessBufferedInput(JObject input)
        {
            var messageType = input.GetStringOrNull("MessageType");
            var playerId = input.GetStringOrNull("PlayerID");

            if (string.IsNullOrEmpty(messageType) || string.IsNullOrEmpty(playerId)) return;

            switch (messageType)
            {
                case "PlayerMove":
                    var posX = input.Value<float>("PosX");
                    var posY = input.Value<float>("PosY");
                    // GameSceneの状態を更新
                    GameScene.UpdatePlayerPosition(playerId!, posX, posY);
                    break;

                case "PlayerAction":
                    var action = input.GetStringOrNull("ActionType");
                    if (action == "Shoot")
                    {
                        // 射撃イベントの処理
                    }
                    break;
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
            }
        }

        public void GameStart()
        {
            sw.Start();

            lock (playerSyncLock)
            {
                if (situation is TeamSurvivalMatchSituation teamSurvival)
                {
                    teamSurvival.SetAliveCount(ETeam.Red,
                        Players.Count(player => player.Team == ETeam.Red && player.Health > 0));
                    teamSurvival.SetAliveCount(ETeam.Blue,
                        Players.Count(player => player.Team == ETeam.Blue && player.Health > 0));
                }
            }

            // ステータス更新を開始
            StartStatusUpdates();

            Playing = true;
            eventBus.PublishGameStart();
        }

        public void Finish()
        {
            lock (finishSyncLock)
            {
                if (Finished) return;
                var evaluator = MatchResultEvaluatorFactory.CreateEvaluator(Setting.Mode);
                CompleteFinish(evaluator.Evaluate(situation, Players));
            }
        }

        public bool TryFinishMission(bool succeeded, string? reason = null)
        {
            lock (finishSyncLock)
            {
                if (Setting is not MissionMatchSetting || !Playing || Finished)
                    return false;

                var evaluator = MatchResultEvaluatorFactory.CreateEvaluator(Setting.Mode);
                var result = evaluator.Evaluate(situation, Players);
                result["ResultType"] = "Mission";
                result["Success"] = succeeded;
                if (!string.IsNullOrWhiteSpace(reason))
                    result["Reason"] = reason;
                CompleteFinish(result);
                return true;
            }
        }

        private void CompleteFinish(JObject resultJson)
        {
            StopStatusUpdates();
            resultJson["RoomId"] = Id.ToString();
            Playing = false;
            Finished = true;
            eventBus.PublishGameEnd();
            eventBus.PublishGameEndWithResult(resultJson);
            if (WaitRoomLink != null)
            {
                WaitRoomLink.GameIsOver();
                WaitRoomLink = null;
            }
        }

        #region ISyncable Implementation

        private JObject _lastSyncState = new();

        public JObject ToJSon()
        {
            var json = new JObject();
            json["RoomID"] = Id;
            json["RoomName"] = RoomName;
            json["PlayerCount"] = PlayerCount;
            json["IsPlaying"] = Playing;
            json["IsFinished"] = Finished;
            
            // GameSceneのスナップショットを追加
            json["Snapshot"] = GameScene.GetSnapshot();
            
            return json;
        }

        public bool HasChanged()
        {
            var currentState = ToJSon();
            return !JToken.DeepEquals(currentState, _lastSyncState);
        }

        public void SaveSyncState()
        {
            _lastSyncState = ToJSon();
        }

        #endregion

        public void Dispose()
        {
            StopStatusUpdates();
        }
    }
}

