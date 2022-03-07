using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class DMMatchSetting:AbstractMatchSetting
    {
        private int winConditionKill_;
        public DMMatchSetting(int winConditionKill,bool teamBalance=true):base(eGameMode.DeathMatch,0,teamBalance)
        {
            winConditionKill_ = 20;

        }

    }
}
