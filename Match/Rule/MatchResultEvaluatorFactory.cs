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
                EGameMode.TeamDeathMatch => new TeamDeathMatchResultEvaluator(),
                EGameMode.Survival => new SurvivalResultEvaluator(),
                EGameMode.TeamSurvival => new TeamSurvivalResultEvaluator(),
                EGameMode.CaptureTheFlag => new CaptureTheFlagResultEvaluator(),
                // 他のゲームモードを追加
                _ => new DeathMatchResultEvaluator() // デフォルトでDeathMatchを返す（例外を避ける）
            };
        }
    }
}
