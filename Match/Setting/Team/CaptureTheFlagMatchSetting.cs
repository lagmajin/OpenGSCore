using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class CaptureTheFlagMatchSetting : AbstractMatchSetting
    {
        private int winConditionPoint = 3;




        public CaptureTheFlagMatchSetting(int winCondition = 3, bool teamBalance = true) : base(EGameMode.CaptureTheFlag, 0, true)
        {
            winConditionPoint = winCondition;
        }
    }
}
