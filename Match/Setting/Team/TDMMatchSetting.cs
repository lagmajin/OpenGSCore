using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class TDMMatchSetting:AbstractTeamMatchSetting
    {


        public TDMMatchSetting(int maxPlayerCapacity=8,bool teamBalance=true):base(eGameMode.TDM)
        {

        }
    }
}
