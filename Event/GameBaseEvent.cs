using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public enum EDeadReason
    {
        Unknown,
        Burst,
        KilledByOtherPeople,
        FireSuicide

    }

    public class PlayerDeadEvent : AbstractGameEvent
    {
        private EDeadReason reason_ = EDeadReason.Unknown;
        private string playerName_;
        private string playerID_;
        private eTeam playerTeam_;

        public PlayerDeadEvent(EDeadReason reason, string playerName, string playerID, eTeam team)
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
