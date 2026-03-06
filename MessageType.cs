using System;

namespace OpenGSCore
{
    /// <summary>
    /// サーバー・クライアント間の通信で使用するメッセージタイプの共通定義
    /// </summary>
    public static class MessageType
    {
        // --- システム・認証関連 (TCP) ---
        public const string LoginRequest = "LoginRequest";
        public const string LoginResponse = "LoginResponse";
        public const string LogoutRequest = "LogoutRequest";
        public const string Heartbeat = "Heartbeat";
        public const string ErrorNotification = "ErrorNotification";

        // --- ロビー・ルーム管理関連 (TCP) ---
        public const string CreateRoomRequest = "CreateRoomRequest";
        public const string CreateRoomResponse = "CreateRoomResponse";
        public const string JoinRoomRequest = "JoinRoomRequest";
        public const string JoinRoomResponse = "JoinRoomResponse";
        public const string LeaveRoomRequest = "LeaveRoomRequest";
        public const string LeaveRoomResponse = "LeaveRoomResponse";
        public const string RoomListUpdateRequest = "RoomListUpdateRequest";
        public const string RoomListUpdateNotification = "RoomListUpdateNotification";
        public const string LobbyChatRequest = "LobbyChatRequest";
        public const string LobbyChatNotification = "LobbyChatNotification";

        // --- マッチメイキング・準備関連 (TCP) ---
        public const string MatchServerInfoRequest = "MatchServerInfoRequest";
        public const string MatchServerInfoResponse = "MatchServerInfoResponse";
        public const string PlayerReadyRequest = "PlayerReadyRequest";
        public const string PlayerReadyNotification = "PlayerReadyNotification";
        public const string GameStartNotification = "GameStartNotification";

        // --- リアルタイムゲームプレイ関連 (UDP/RUDP) ---
        public const string WelcomeMessage = "WelcomeMessage";
        public const string PlayerSpawned = "PlayerSpawned";
        public const string PlayerPositionUpdate = "PlayerPositionUpdate";
        public const string PlayerShot = "PlayerShot";
        public const string PlayerDamage = "PlayerDamage";
        public const string PlayerDeath = "PlayerDeath";
        public const string GameStateSync = "GameStateSync";
        public const string MatchEndNotification = "MatchEndNotification";
        
        // 旧互換用 (移行期間)
        public const string Notification = "Notification";
    }
}
