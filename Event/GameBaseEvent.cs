using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{

    public class PlayerDeadEvent : AbstractGameEvent
    {
        private EDeadReason reason_ = EDeadReason.Unknown;
        private string playerName_;
        private string playerID_;
        private ETeam playerTeam_;

        public PlayerDeadEvent(EDeadReason reason, string playerName, string playerID, ETeam team)
        {
            reason_ = reason;


        }
        public string PlayerName()
        {
            return playerName_;
        }

        public ETeam PlayerTeam()
        {
            return playerTeam_;
        }

    }

}
