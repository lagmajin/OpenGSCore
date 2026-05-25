namespace OpenGSCore
{
    public class MatchResultService
    {
        public AbstractMatchResult createMatchResult(AbstractMatchFinalScore score)
        {
            if (score == null)
            {
                return null;
            }

            return score switch
            {
                DeathMatchFinalScore deathMatchScore => new DeathMatchResult(deathMatchScore),
                TeamDeathMatchFinalScore teamDeathMatchScore => new TeamDeathMatchResult(teamDeathMatchScore),
                CTFMatchFinalScore ctfMatchScore => new CTFMatchResult(ctfMatchScore),
                _ => score.Mode switch
                {
                    EGameMode.Survival => new SuvMatchResult(),
                    EGameMode.TeamSurvival => new TSuvMatchResult(),
                    _ => null
                },
            };
        }
    }
}
