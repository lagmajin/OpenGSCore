using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class AbstractTeamMatchSetting:AbstractMatchSetting
    {
        private bool randomTeam = true;

        private bool teamBalance = false;

        public AbstractTeamMatchSetting(eGameMode mode,bool randomTeam=false,bool teamBalance=true):base(mode,8,true,false)
        {

        }


    }
}
