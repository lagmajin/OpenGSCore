using OpenGSCore;
using OpenGSCore.Score;

namespace OpenGSServer
{
    public class MatchResultFactory
    {
        public static AbstractMatchResult CreateMatchResult(AbstractFinalScore score)
        {
            if (score == null)
            {
                return null;
            }

            return score.mode switch
            {
                EGameMode.DeathMatch => new DeathMatchResult(),
                EGameMode.TeamDeathMatch => new TeamDeathMatchResult(),
                EGameMode.CaptureTheFlag => new CTFMatchResult(),
                EGameMode.Survival => new SuvMatchResult(),
                EGameMode.TeamSurvival => new TSuvMatchResult(),
                _ => new DeathMatchResult()
            };
        }
    }
}
