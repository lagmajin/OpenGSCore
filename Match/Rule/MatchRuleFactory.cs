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
                    return setting is DeathMatchSetting deathMatchSetting
                        ? new DeathMatchRule(deathMatchSetting)
                        : new DeathMatchRule();
                case EGameMode.TeamDeathMatch:
                    return new TDMMatchRule();
                case EGameMode.Survival:
                    return setting is SuvMatchSetting suvSetting
                        ? new SuvMatchRule(suvSetting)
                        : new SuvMatchRule();
                case EGameMode.TeamSurvival:
                    if (setting is TeamSurvivalMatchSetting teamSetting)
                        return new TSuvMatchRule(teamSetting);
                    return new TSuvMatchRule();
                case EGameMode.CaptureTheFlag:
                    return setting is CaptureTheFlagMatchSetting ctfSetting
                        ? new CaptureTheFlagMatchRule(ctfSetting.WinConditionPoint)
                        : new CaptureTheFlagMatchRule();
                default:
                    return null;
            }
        }
    }
}
