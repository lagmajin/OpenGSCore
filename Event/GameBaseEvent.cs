using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public enum eDeadReason
    {
        Unknown,
        Burst,
        KilldByOtherPeople,
        FireSuicide

    }

    public class PlayerDeadEvent : AbstractGameEvent
    {
        private eDeadReason reason_ = eDeadReason.Unknown;
        private string playerName_;
        private string playerID_;
        private eTeam playerTeam_;

        public PlayerDeadEvent(eDeadReason reason, string playerName, string playerID, eTeam team)
        {
            reason_ = reason;


        }
        public string PlayerName()
        {
            return playerName_;
        }

        public eTeam PlayerTeam()
        {
            return playerTeam_;
        }

    }

}
