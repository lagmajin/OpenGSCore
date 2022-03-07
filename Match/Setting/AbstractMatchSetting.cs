using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;




namespace OpenGSCore
{
    public interface IAbstractMatchSetting
    {

    }
    public class AbstractMatchSetting:IAbstractMatchSetting
    {
        private eGameMode gameMode_=eGameMode.Unknown;

        private bool teamBalance_ = true;

        private int maxPlayerCount_;

        public bool hasTimeLimit_=false;

        private int matchTimeMSec_ = 1000;
        public bool TeamBalance { get => teamBalance_; }
        public int MaxPlayerCount { get => maxPlayerCount_; }
        public eGameMode Mode { get => gameMode_; }
        public int MatchTimeMSec { get => matchTimeMSec_;}

        public AbstractMatchSetting(eGameMode gamemode, int maxplayerCount, bool teamBalance = true, bool hasTimeLimit_ = false)
        {
            
        }

        public AbstractMatchSetting(in eGameMode gamemode, int maxplayerCount, bool teamBalance = true, bool hasTimeLimit_ = false,int timeLimit=1000)
        {

            gameMode_ = gamemode;
            maxPlayerCount_ = maxplayerCount;

            matchTimeMSec_ = 600000;



        }

        public virtual JObject ToJson()
        {
            var result = new JObject();

            IDictionary<string, JToken> dictionary = result;


            return result;
        }

    }
}
