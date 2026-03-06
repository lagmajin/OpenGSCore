using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{

    public class PlayerDeadEvent : AbstractGameEvent
    {
        private DeadReason reason_ = DeadReason.Unknown;
        private string playerName_;
        private string playerID_;
        private ETeam playerTeam_;

        public PlayerDeadEvent(DeadReason reason, string playerName, string playerID, ETeam team)
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
