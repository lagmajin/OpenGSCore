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
                        return new TSuvMatchRule(teamSetting.TeamBalance);
                    return new TSuvMatchRule();
                case EGameMode.CaptureTheFlag:
                    return new CaptureTheFlagMatchRule();
                default:
                    return null;
            }
        }
    }
}
