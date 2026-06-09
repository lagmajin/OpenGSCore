using System;

namespace OpenGSCore
{
    /// <summary>
    /// IMatchResultEvaluatorを生成するファクトリ
    /// </summary>
    public static class MatchResultEvaluatorFactory
    {
        public static IMatchResultEvaluator CreateEvaluator(EGameMode mode)
        {
            return mode switch
            {
                EGameMode.DeathMatch => new DeathMatchResultEvaluator(),
                EGameMode.OneShotKill => new DeathMatchResultEvaluator(),
                EGameMode.ArmsRace => new ArmsRaceResultEvaluator(),
                EGameMode.TeamDeathMatch => new TeamDeathMatchResultEvaluator(),
                EGameMode.Survival => new SurvivalResultEvaluator(),
                EGameMode.TeamSurvival => new TeamSurvivalResultEvaluator(),
                EGameMode.CaptureTheFlag => new CaptureTheFlagResultEvaluator(),
                EGameMode.Practice => MissionResultEvaluatorFactory.CreateEvaluator(mode),
                EGameMode.FreeStyle => new DeathMatchResultEvaluator(),
                EGameMode.Sniper => new DeathMatchResultEvaluator(),
                EGameMode.TowerMatch => new DeathMatchResultEvaluator(),
                _ => new DeathMatchResultEvaluator()
            };
        }
    }
}
