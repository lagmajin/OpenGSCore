using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;




namespace OpenGSCore
{
    public interface IAbstractMatchSetting
    {

    }
    public class AbstractMatchSetting : IAbstractMatchSetting
    {

        //public bool TeamBalance { get; set; } = true;
        public int MaxPlayerCount { get; set; } = 0;
        public EGameMode Mode { get; set; } = EGameMode.Unknown;
        public int MatchTimeMSec { get; set; } = 0;

        public float DamageRate { get; set; } = 1.0f;
        public float BoosterRate { get; set; } = 1.0f;
        public bool TimeLimit { get; set; } = false;

        public bool AllowOvertime { get; set; } = false;


        private bool isLocked = false;

        public void LockSettings() => isLocked = true;

        public AbstractMatchSetting(EGameMode gamemode, int maxplayerCount, bool teamBalance = true, bool hasTimeLimit_ = false)
        {

        }

        public AbstractMatchSetting(in EGameMode gamemode, int maxplayerCount, bool teamBalance = true, bool hasTimeLimit_ = false, int timeLimit = 1000)
        {

            //gameMode_ = gamemode;
            //maxPlayerCount_ = maxplayerCount;

            //matchTimeMSec_ = 600000;



        }

        public AbstractMatchSetting()
        {

        }

        public AbstractMatchSetting(EGameMode gamemode, int maxplayerCount)
        {
            Mode = gamemode;
            MaxPlayerCount = maxplayerCount;
        }

        public virtual JObject ToJson()
        {
            var result = new JObject();

            //result["TeamBalance"] = TeamBalance.ToString();
            result["GameMode"] = Mode.ToString();
            result["MaxPlayerCount"] = MaxPlayerCount.ToString();
            result["AllowOvertime"] = AllowOvertime;


            //Dictionary<string, JToken> dictionary = result;





            return result;
        }




    }


}
