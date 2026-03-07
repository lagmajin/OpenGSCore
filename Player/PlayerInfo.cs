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
        public ETeam Team { get; set; } = ETeam.None;

        // New properties for player stats
        public int Level { get; set; } = 1;
        public long Exp { get; set; } = 0;
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public int AttackPower { get; set; } = 10;
        public int DefensePower { get; set; } = 5;

        public int Kills { get; set; } = 0;
        public int Deaths { get; set; } = 0;

        // Indicates whether this entry represents a bot
        public bool IsBot { get; set; } = false;

        public PlayerInfo(string? id = null, string name = "", string? currentIp = null, int level = 1, long exp = 0, int health = 100, int attack = 10, int defense = 5)
        {
            Id = id ?? string.Empty;
            Name = name;
            CurrentIp = currentIp;
            Level = level;
            Exp = exp;
            Health = health;
            MaxHealth = health; // MaxHealth defaults to initial Health
            AttackPower = attack;
            DefensePower = defense;
        }

        public JObject ToJson()
        {
            var result = new JObject();
            result["Id"] = Id;
            result["Name"] = Name;
            if (CurrentIp != null) result["CurrentIP"] = CurrentIp;
            result["Ping"] = Ping;
            result["IsBot"] = IsBot;
            result["Level"] = Level;
            result["Exp"] = Exp;
            result["Health"] = Health;
            result["MaxHealth"] = MaxHealth;
            result["AttackPower"] = AttackPower;
            result["DefensePower"] = DefensePower;
            result["Team"] = Team.ToString();
            result["Kills"] = Kills;
            result["Deaths"] = Deaths;
            return result;
        }

    }




}
