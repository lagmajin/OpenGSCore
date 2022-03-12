using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public sealed class MatchPlayerInfo
    {
        private string name_;
        private string guid_;
        private eTeam? team_;

        public MatchPlayerInfo(in string name, in string guid, in eTeam? team)
        {
            name_ = name;
            guid_ = guid;

        }

        private bool HasTeam()
        {
            if (team_ == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool NoTeam()
        {
            return !HasTeam();
        }

    }
}
