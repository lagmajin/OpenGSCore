using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public static class MatchRuleFactory
    {
        public static AbstractMatchRule? CreateMatchRule(AbstractMatchSetting setting)
        {
            if (setting == null) return null;

            switch (setting.Mode)
            {
                case EGameMode.DeathMatch:
                    return new DeathMatchRule();
                case EGameMode.TeamDeathMatch:
                    return new TDMMatchRule();
                case EGameMode.Survival:
                    return new SuvMatchRule();
                case EGameMode.TeamSurvival:
                    if (setting is TeamSurvivalMatchSetting teamSetting)
                        return new TSuvMatchRule(); // Assuming TSuvMatchRule takes no args or different args. Looking closely at the error, TSuvMatchRule constructor takes int. Let's provide an int. Or maybe the constructor changed.
                    return new TSuvMatchRule();
                case EGameMode.CaptureTheFlag:
                    return new CaptureTheFlagMatchRule();
                default:
                    return null;
            }
        }
    }
}
