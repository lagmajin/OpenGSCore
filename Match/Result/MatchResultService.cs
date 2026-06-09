using System;

namespace OpenGSCore
{
    [Obsolete("Use MatchResultResolver directly")]
    public class MatchResultService
    {
        public MatchResult<MatchFinalScore> CreateMatchResult(MatchFinalScore score)
        {
            return MatchResultResolver.Create(score);
        }
    }
}
