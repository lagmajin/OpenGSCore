using System;

namespace OpenGSCore
{
    /// <summary>
    /// 試合結果のファクトリ。旧3層 (Resolver/Factory/Service) を一本化。
    /// </summary>
    public static class MatchResultResolver
    {
        public static MatchResult<MatchFinalScore> Create(EGameMode mode)
        {
            return new MatchResult<MatchFinalScore>(new MatchFinalScore(mode));
        }

        public static MatchResult<MatchFinalScore> Create(MatchFinalScore score)
        {
            if (score == null) throw new ArgumentNullException(nameof(score));
            return new MatchResult<MatchFinalScore>(score);
        }
    }
}
