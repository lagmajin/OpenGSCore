using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class MatchRuleFactory
    {
        AbstractMatchRule CreateMatchRule(AbstractMatchSetting setting)
        {
            switch (setting.Mode)
            {
                case EGameMode.DeathMatch:

                    var dmSetting = setting as DeathMatchSetting;

                    //dmSetting.

                    break;
                case EGameMode.Practice:
                    break;
                case EGameMode.FreeStyle:
                    break;
                case EGameMode.TowerMatch:
                    break;
                case EGameMode.CaptureTheFlag:
                    break;
                case EGameMode.Unknown:
                    break;
                default:
                    break;

            }

            //setting.Mode

            return null;
        }

    }
}
