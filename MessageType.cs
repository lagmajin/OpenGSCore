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
        public const string LoginSuccessful = LoginResponse;
        public const string CreateAccountRequest = "CreateAccountRequest";
        public const string CreateAccountResponse = "CreateAccountResponse";
        public const string LogoutRequest = "LogoutRequest";
        public const string LogoutSuccessful = "LogoutSuccessful";
        public const string Heartbeat = "Heartbeat";
        public const string ErrorNotification = "ErrorNotification";

        // --- ロビー・ルーム管理関連 (TCP) ---
        public const string CreateRoomRequest = "CreateRoomRequest";
        public const string CreateRoomResponse = "CreateRoomResponse";
        public const string CreateNewWaitRoomRequest = CreateRoomRequest;
        public const string CreateNewWaitRoomResponse = CreateRoomResponse;
        public const string JoinRoomRequest = "JoinRoomRequest";
        public const string JoinRoomResponse = "JoinRoomResponse";
        public const string EnterWaitRoomRequest = JoinRoomRequest;
        public const string EnterWaitRoomResponse = JoinRoomResponse;
        public const string LeaveRoomRequest = "LeaveRoomRequest";
        public const string LeaveRoomResponse = "LeaveRoomResponse";
        public const string LeaveWaitRoomRequest = LeaveRoomRequest;
        public const string LeaveWaitRoomResponse = LeaveRoomResponse;
        public const string RoomListUpdateRequest = "RoomListUpdateRequest";
        public const string RoomListUpdateNotification = "RoomListUpdateNotification";
        public const string UpdateRoomRequest = RoomListUpdateRequest;
        public const string UpdateRoomResponse = RoomListUpdateNotification;
        public const string LobbyChatRequest = "LobbyChatRequest";
        public const string LobbyChatNotification = "LobbyChatNotification";
        public const string AddLobbyChat = LobbyChatRequest;

        // --- マッチメイキング・準備関連 (TCP) ---
        public const string MatchServerInfoRequest = "MatchServerInfoRequest";
        public const string MatchServerInfoResponse = "MatchServerInfoResponse";
        public const string PlayerReadyRequest = "PlayerReadyRequest";
        public const string PlayerReadyNotification = "PlayerReadyNotification";
        public const string PlayerReady = PlayerReadyRequest;
        public const string PlayerUnready = "PlayerUnready";
        public const string GameStartNotification = "GameStartNotification";
        public const string GameStartRequest = "GameStartRequest";
        public const string ItemSpawnNotification = "ItemSpawnNotification";
        public const string ItemDespawnNotification = "ItemDespawnNotification";

        // --- リアルタイムゲームプレイ関連 (UDP/RUDP) ---
        public const string WelcomeMessage = "WelcomeMessage";
        public const string PlayerSpawned = "PlayerSpawned";
        public const string PlayerPositionUpdate = "PlayerPositionUpdate";
        public const string PlayerShot = "PlayerShot";
        public const string PlayerDamage = "PlayerDamage";
        public const string PlayerDeath = "PlayerDeath";
        public const string GameStateSync = "GameStateSync";
        public const string MatchEndNotification = "MatchEndNotification";
        public const string MatchResult = MatchEndNotification;
        
        // プレイヤー情報取得関連
        public const string PlayerInfoRequest = "PlayerInfoRequest";
        public const string PlayerInfoResponse = "PlayerInfoResponse";
        public const string PlayerInfo = PlayerInfoRequest;

        // ショップ関連
        public const string ShopStateRequest = "ShopStateRequest";
        public const string ShopStateResponse = "ShopStateResponse";
        public const string ShopPurchaseRequest = "ShopPurchaseRequest";
        public const string ShopPurchaseResponse = "ShopPurchaseResponse";
        public const string ShopEquipRequest = "ShopEquipRequest";
        public const string ShopEquipResponse = "ShopEquipResponse";
        public const string ShopUnequipRequest = "ShopUnequipRequest";

        // フレンド関連 (TCP)
        public const string FriendRequest = "FriendRequest";
        public const string FriendRequestResponse = "FriendRequestResponse";
        public const string FriendRequestNotification = "FriendRequestNotification";
        public const string FriendApproveRequest = "FriendApproveRequest";
        public const string FriendApproveResponse = "FriendApproveResponse";
        public const string FriendListRequest = "FriendListRequest";
        public const string FriendListResponse = "FriendListResponse";
        
        // 旧互換用 (移行期間)
        public const string Notification = "Notification";

        public static string Normalize(string messageType)
        {
            return messageType switch
            {
                CreateNewWaitRoomRequest => CreateRoomRequest,
                CreateNewWaitRoomResponse => CreateRoomResponse,
                UpdateRoomRequest => RoomListUpdateRequest,
                UpdateRoomResponse => RoomListUpdateNotification,
                EnterWaitRoomRequest => JoinRoomRequest,
                EnterWaitRoomResponse => JoinRoomResponse,
                LeaveWaitRoomRequest => LeaveRoomRequest,
                LeaveWaitRoomResponse => LeaveRoomResponse,
                AddLobbyChat => LobbyChatRequest,
                LoginSuccessful => LoginResponse,
                "LogoutSuccess" => LogoutSuccessful,
                "SendEnterRoom" => JoinRoomRequest,
                "CreateNewWaitRoomRequest" => CreateRoomRequest,
                "CreateNewWaitRoomResponse" => CreateRoomResponse,
                "UpdateRoomResponse" => RoomListUpdateNotification,
                "EnterWaitRoomResponse" => JoinRoomResponse,
                "LeaveWaitRoomResponse" => LeaveRoomResponse,
                "AddLobbyChat" => LobbyChatRequest,
                _ => messageType
            };
        }
    }
}
