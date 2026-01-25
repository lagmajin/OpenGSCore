using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;


namespace OpenGSCore
{
    //public interface IWaitRoom { }

    //#new
    public partial class WaitRoom
    {
        public MatchRoom? MatchRoomLink { get; set; } = null;
        public string RoomName { get; set; } = "";
        public string RoomId { get; set; }
        public bool NowPlaying { get; private set; } = false;
        public int Capacity { get; set; } = 0;

        
        public List<PlayerInfo> OldPlayers { get; set; }
        
        public Dictionary<string,PlayerInfo> Players { get; set; }

        private readonly  Object lockObject = new Object();


        public AbstractMatchRule MatchRule { get; set; } = null;

        public AbstractMatchSetting setting = null;
        public WaitRoom(in string roomName, int capacity = 8)
        {
            RoomName = roomName;
            Capacity = capacity;
            RoomId = Guid.NewGuid().ToString();
            Players = new Dictionary<string, PlayerInfo>();
            OldPlayers = new List<PlayerInfo>();
        }
        public WaitRoom(in string name,in string id,int capacity=8)
        {
            RoomName = name;

            RoomId = id ?? Guid.NewGuid().ToString();

            Capacity=capacity;
            Players = new Dictionary<string, PlayerInfo>();
            OldPlayers = new List<PlayerInfo>();
        }

        public void ChangeGameMode(EGameMode mode)
        {
            lock (lockObject)
            {
                // Do not change mode while a game is in progress
                if (NowPlaying) return;

                switch (mode)
                {
                    case EGameMode.DeathMatch:
                        setting = new DeathMatchSetting();
                        break;
                    case EGameMode.TeamDeathMatch:
                        setting = new TDMMatchSetting();
                        break;
                    case EGameMode.CaptureTheFlag:
                        setting = new CaptureTheFlagMatchSetting();
                        break;
                    case EGameMode.Survival:
                        //setting = new SuvMatchSetting();
                        break;
                    case EGameMode.TeamSurvival:
                        setting = new TeamSurvivalMatchSetting();
                        break;
                    default:
                        // fallback to a generic abstract setting
                        setting = new AbstractMatchSetting(mode, Capacity);
                        break;
                }

                // Ensure capacity is in sync with setting if provided
                if (setting != null && setting.MaxPlayerCount > 0)
                {
                    Capacity = setting.MaxPlayerCount;
                }
            }
        }

        public void AddPlayer(PlayerInfo info)
        {
            lock (lockObject)
            {




            }


        }

        public void AddPlayer(in string id, in string displayName)
        {
            lock (lockObject)
            {
                //var playerInfo = new PlayerInfo(id,displayName);

                //Players.Add(id,);

                var info = new PlayerInfo(id, displayName);


                Players.Add(id,info);


            }

        }

        public void AddPlayers()
        {

        }

        // convenience overload to add multiple players
        public void AddPlayers(IEnumerable<PlayerInfo> infos)
        {
            if (infos == null) return;
            lock (lockObject)
            {
                foreach (var p in infos)
                {
                    AddPlayer(p);
                }
            }
        }

        public void AddBotPlayer()
        {

        }

        public PlayerInfo AddBotPlayer(string? displayName = null)
        {
            lock (lockObject)
            {
                if (Capacity > 0 && Players.Count >= Capacity)
                {
                    return null;
                }

                var id = $"bot_{Guid.NewGuid():N}";
                var name = displayName ?? $"Bot_{Players.Count + 1}";
                var bot = new PlayerInfo(id, name)
                {
                    IsBot = true
                };

                Players.Add(id, bot);
                OldPlayers.Add(bot);
                return bot;
            }
        }


        public void RemovePlayer(in string id)
        {
            lock (lockObject)
            {
                Players.Remove(id);



            }

        }

        public void RemoveAllPlayers()
        {
            lock(lockObject)
            {
                Players.Clear();

            }

        }

        public void RemoveAllBotPlayer()
        {
            lock (lockObject)
            {
                var removeKeys = new List<string>();
                foreach (var kv in Players)
                {
                    if (kv.Value != null && kv.Value.IsBot)
                    {
                        removeKeys.Add(kv.Key);
                    }
                }

                foreach (var k in removeKeys)
                {
                    Players.Remove(k);
                }
            }
        }


        public List<PlayerInfo> AllPlayers()
        {
            lock (lockObject)
            {

                var players = new List<PlayerInfo>();


                foreach (var info in Players)
                {
                    players.Add(info.Value);


                }


                return players;
            }
        }

        public void GameStart()
        {
            NowPlaying = true;

        }

        public void GameIsOver()
        {
            NowPlaying = false;
            MatchRoomLink = null;


        }

        public JObject ToJson()
        {
            var result = new JObject();

            var array = new JArray();
            lock (lockObject)
            {
                result["RoomName"] = RoomName;
                result["RoomId"] = RoomId;
                result["NowPlaying"] = NowPlaying;
                result["Capacity"] = Capacity;

                foreach (var p in Players.Values)
                {
                    array.Add(p.ToJson());
                }

                result["Players"] = array;
            }

            return result;
        }
    }
}
