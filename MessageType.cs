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
        public const string RoomCreated = "RoomCreated";
        public const string RoomDeleted = "RoomDeleted";
        public const string RoomFull = "RoomFull";
        public const string RoomNotFound = "RoomNotFound";
        public const string RoomSettingChanged = "RoomSettingChanged";
        public const string LobbyChatRequest = "LobbyChatRequest";
        public const string LobbyChatNotification = "LobbyChatNotification";
        public const string AddLobbyChat = LobbyChatRequest;
        public const string LobbyEnter = JoinRoomRequest;
        public const string LobbyLeave = LeaveRoomRequest;
        public const string LobbyPlayerList = "LobbyPlayerList";
        public const string LobbyChat = LobbyChatRequest;
        public const string InvalidRoomId = "InvalidRoomId";
        public const string CreateNewWaitRoomSuccess = "CreateNewWaitRoomSuccess";
        public const string UpdateRoomResult = "UpdateRoomResult";
        public const string LobbyInfo = "LobbyInfo";
        public const string LobbyInfoResponse = "LobbyInfoResponse";

        // --- マッチメイキング・準備関連 (TCP) ---
        public const string MatchServerInfoRequest = "MatchServerInfoRequest";
        public const string MatchServerInfoResponse = "MatchServerInfoResponse";
        public const string PlayerReadyRequest = "PlayerReadyRequest";
        public const string PlayerReadyNotification = "PlayerReadyNotification";
        public const string PlayerUnready = "PlayerUnready";
        public const string GameStartRequest = "GameStartRequest";
        public const string PlayerReady = PlayerReadyRequest;
        public const string GameStartNotification = "GameStartNotification";
        public const string ItemSpawnNotification = "ItemSpawnNotification";
        public const string ItemDespawnNotification = "ItemDespawnNotification";
        public const string ClientLoadingSceneEntered = "ClientLoadingSceneEntered";
        public const string LoadingStarted = "LoadingStarted";
        public const string LoadingProgress = "LoadingProgress";
        public const string LoadingCompleted = "LoadingCompleted";
        public const string LoadingStartedNotification = "LoadingStartedNotification";
        public const string LoadingProgressNotification = "LoadingProgressNotification";
        public const string LoadingCompletedNotification = "LoadingCompletedNotification";
        public const string LoadingFailed = "LoadingFailed";
        public const string LoadingMessage = "LoadingMessage";
        public const string AllowEnterMap = "AllowEnterMap";
        public const string WaitRoomEnter = JoinRoomRequest;
        public const string WaitRoomLeave = LeaveRoomRequest;
        public const string WaitRoomPlayerList = "WaitRoomPlayerList";
        public const string WaitRoomChat = LobbyChatRequest;
        public const string WaitRoomPlayerReady = PlayerReadyRequest;
        public const string WaitRoomPlayerUnready = PlayerUnready;
        public const string WaitRoomSettingsChange = "WaitRoomSettingsChange";
        public const string WaitRoomKickPlayer = "WaitRoomKickPlayer";
        public const string WaitRoomOwnerChange = "WaitRoomOwnerChange";
        public const string WaitRoomStartCountdown = "WaitRoomStartCountdown";
        public const string WaitRoomCancelCountdown = "WaitRoomCancelCountdown";
        public const string WaitRoomUpdateNotification = UpdateRoomResponse;

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

        // ギルド関連 (TCP)
        public const string GuildListRequest = "GuildListRequest";
        public const string GuildListResponse = "GuildListResponse";
        public const string GuildInfoRequest = "GuildInfoRequest";
        public const string GuildInfoResponse = "GuildInfoResponse";
        public const string GuildCreateRequest = "GuildCreateRequest";
        public const string GuildCreateResponse = "GuildCreateResponse";
        public const string GuildJoinRequest = "GuildJoinRequest";
        public const string GuildJoinResponse = "GuildJoinResponse";
        public const string GuildLeaveRequest = "GuildLeaveRequest";
        public const string GuildLeaveResponse = "GuildLeaveResponse";
        public const string GuildInviteRequest = "GuildInviteRequest";
        public const string GuildInviteResponse = "GuildInviteResponse";
        public const string GuildInviteNotification = "GuildInviteNotification";
        public const string GuildKickRequest = "GuildKickRequest";
        public const string GuildKickResponse = "GuildKickResponse";
        public const string GuildKickNotification = "GuildKickNotification";
        public const string GuildChatRequest = "GuildChatRequest";
        public const string GuildChatNotification = "GuildChatNotification";
        
        // 旧互換用 (移行期間)
        public const string Notification = "Notification";

        public static string Normalize(string messageType)
        {
            if (string.IsNullOrWhiteSpace(messageType))
            {
                return messageType;
            }

            return messageType switch
            {
                CreateNewWaitRoomRequest => CreateRoomRequest,
                CreateNewWaitRoomResponse => CreateRoomResponse,
                UpdateRoomRequest => RoomListUpdateRequest,
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
                "LobbyEnterRequest" => LobbyEnter,
                "LobbyLeaveRequest" => LobbyLeave,
                "WaitRoomEnterRequest" => WaitRoomEnter,
                "WaitRoomLeaveRequest" => WaitRoomLeave,
                "WaitRoomChatRequest" => WaitRoomChat,
                "WaitRoomPlayerReadyRequest" => WaitRoomPlayerReady,
                "WaitRoomPlayerUnreadyRequest" => WaitRoomPlayerUnready,
                "LoadingStartedNotification" => LoadingStartedNotification,
                "LoadingProgressNotification" => LoadingProgressNotification,
                "LoadingCompletedNotification" => LoadingCompletedNotification,
                _ => messageType
            };
        }
    }
}
