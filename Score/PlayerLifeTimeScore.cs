

namespace OpenGSCore
{
    public class PlayerLifeTimeScore
    {
        public int TotalMatchCount { get; private set; } = 0;
        public int DeathMatchCount { get; private set; } = 0;
        public int DeathMatchWinCount { get; private set; } = 0;
        public int DeathMatchLoseCount { get; set; } = 0;
        public int TeamDeathMatchWinCount { get; set; } = 0;
        public int SurvivalWinCount { get; set; } = 0;
        public int SurvivalLoseCount { get; set; } = 0;
        public int TeamSurvivalWinCount { get; set; } = 0;
        public int TeamSurvivalLoseCount { get; set; } = 0;
        public int CtfFlagReturn { get; set; } = 0;
        public int CtfFlagInterrupt { get; set; } = 0;


        public PlayerLifeTimeScore()
        {

        }

        public void IncrementTotalMatchCount()
        {
            TotalMatchCount++;
        }

        public void IncrementTeamDeathMatchWinCount()
        {

            
        }

        public void IncrementSurvivalWinCount()
        {
            SurvivalWinCount++;
        }

        
        
        


    }
}
