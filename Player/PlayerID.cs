using System;
using System.Collections.Generic;
using System.Text;


namespace OpenGSCore
{
    enum EGenerateType
    {
    }

    public sealed class PlayerID
    {
        private string playerID;

        public PlayerID()
        {
            DateTime.Now.ToString("");
        }

        public override string ToString()
        {
            return playerID;
        }

    }
}
