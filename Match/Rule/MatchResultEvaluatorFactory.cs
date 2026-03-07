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
                // 他のゲームモードを追加
                _ => throw new ArgumentException($"No result evaluator for game mode: {mode}")
            };
        }
    }
}
