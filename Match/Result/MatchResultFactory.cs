using System;

namespace OpenGSServer
{
    [Obsolete("Use MatchResultResolver directly")]
    public class MatchResultFactory
    {
        public static OpenGSCore.MatchResult<OpenGSCore.MatchFinalScore> CreateMatchResult(OpenGSCore.MatchFinalScore score)
        {
            return OpenGSCore.MatchResultResolver.Create(score);
        }
    }
}
