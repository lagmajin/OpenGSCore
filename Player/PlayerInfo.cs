using Newtonsoft.Json.Linq;
using System.Collections.Generic;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//using NodaTime;

namespace OpenGSCore
{

    public class PlayerInfoLite
    {
        public string Id { get; }
        public string Name { get; }
        public string? CurrentIp { get; }

        public PlayerInfoLite(string id, string name, string? currentIp = null)
        {
            Id = id;
            Name = name;
            CurrentIp = currentIp;
        }

        public JObject ToJson()
        {
            var result = new JObject();
            result["Id"] = Id;
            result["Name"] = Name;
            if (CurrentIp != null) result["CurrentIP"] = CurrentIp;
            return result;
        }

    }

    //#PlayerInfo
    public class PlayerInfo
    {
        // public-facing properties
        public string Id { get; set; }
        public string Name { get; set; }
        public string? CurrentIp { get; set; } = null;
        public int Ping { get; set; } = 0;
        public EPlayerCharacter playerCharacter { get; set; }

        // Indicates whether this entry represents a bot
        public bool IsBot { get; set; } = false;

        public PlayerInfo(string? id = null, string name = null, in string? currentIp = null)
        {
            Id = id;
            Name = name;
            CurrentIp = currentIp;
        }

        public JObject ToJson()
        {
            var result = new JObject();
            result["Id"] = Id;
            result["Name"] = Name;
            if (CurrentIp != null) result["CurrentIP"] = CurrentIp;
            result["Ping"] = Ping;
            result["IsBot"] = IsBot;
            return result;
        }

    }




}
