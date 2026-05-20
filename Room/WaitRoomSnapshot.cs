using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace OpenGSCore
{
    /// <summary>
    /// 待機室の共通スナップショット DTO。
    /// サーバーの RoomInfo とクライアントの表示状態を共通化するための軽量モデル。
    /// </summary>
    public class WaitRoomSnapshot
    {
        public string RoomId { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; } = 8;
        public bool NowPlaying { get; set; }
        public string GameMode { get; set; } = EGameMode.DeathMatch.ToString();
        public bool TeamBalance { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public List<PlayerInfo> Players { get; set; } = new();

        public JObject ToJson()
        {
            var result = new JObject
            {
                ["RoomId"] = RoomId,
                ["RoomName"] = RoomName,
                ["Capacity"] = Capacity,
                ["NowPlaying"] = NowPlaying,
                ["GameMode"] = GameMode,
                ["TeamBalance"] = TeamBalance,
                ["OwnerId"] = OwnerId
            };

            var players = new JArray();
            foreach (var player in Players)
            {
                if (player != null)
                {
                    players.Add(player.ToJson());
                }
            }

            result["Players"] = players;
            return result;
        }

        public static WaitRoomSnapshot FromJson(JObject json)
        {
            var snapshot = new WaitRoomSnapshot
            {
                RoomId = json["RoomId"]?.ToString() ?? json["RoomID"]?.ToString() ?? string.Empty,
                RoomName = json["RoomName"]?.ToString() ?? string.Empty,
                Capacity = json["Capacity"]?.ToObject<int>() ?? 8,
                NowPlaying = json["NowPlaying"]?.ToObject<bool>() ?? false,
                GameMode = json["GameMode"]?.ToString() ?? EGameMode.DeathMatch.ToString(),
                TeamBalance = json["TeamBalance"]?.ToObject<bool>() ?? false,
                OwnerId = json["OwnerId"]?.ToString() ?? json["OwnerID"]?.ToString() ?? string.Empty
            };

            if (json["Players"] is JArray players)
            {
                foreach (var token in players)
                {
                    if (token is not JObject playerJson)
                    {
                        continue;
                    }

                    var player = new PlayerInfo(
                        playerJson["Id"]?.ToString() ?? playerJson["PlayerId"]?.ToString() ?? string.Empty,
                        playerJson["Name"]?.ToString() ?? playerJson["PlayerName"]?.ToString() ?? string.Empty);
                    player.IsReady = playerJson["IsReady"]?.ToObject<bool>() ?? false;
                    player.IsBot = playerJson["IsBot"]?.ToObject<bool>() ?? false;
                    player.Team = Enum.TryParse(playerJson["Team"]?.ToString(), true, out ETeam team) ? team : ETeam.NoTeam;
                    snapshot.Players.Add(player);
                }
            }

            return snapshot;
        }
    }
}
