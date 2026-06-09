namespace OpenGSCore
{
    public class PlayerLifeTimeScore
    {
        public int TotalMatchCount { get; private set; }
        public int DeathMatchCount { get; private set; }
        public int DeathMatchWinCount { get; private set; }
        public int DeathMatchLoseCount { get; private set; }
        public int TeamDeathMatchWinCount { get; private set; }
        public int TeamDeathMatchLoseCount { get; private set; }
        public int SurvivalWinCount { get; private set; }
        public int SurvivalLoseCount { get; private set; }
        public int TeamSurvivalWinCount { get; private set; }
        public int TeamSurvivalLoseCount { get; private set; }
        public int CtfFlagReturn { get; private set; }
        public int CtfFlagInterrupt { get; private set; }

        public PlayerLifeTimeScore()
        {
        }

        public void IncrementTotalMatchCount() => TotalMatchCount++;
        public void IncrementTeamDeathMatchWinCount() => TeamDeathMatchWinCount++;
        public void IncrementSurvivalWinCount() => SurvivalWinCount++;
        public void IncrementTeamSurvivalWinCount() => TeamSurvivalWinCount++;

        public void RecordDeathMatchResult(bool won)
        {
            IncrementTotalMatchCount();
            DeathMatchCount++;
            if (won) DeathMatchWinCount++;
            else DeathMatchLoseCount++;
        }

        public void RecordTeamDeathMatchResult(bool won)
        {
            IncrementTotalMatchCount();
            if (won) TeamDeathMatchWinCount++;
            else TeamDeathMatchLoseCount++;
        }

        public void RecordSurvivalResult(bool won, bool isTeamMatch)
        {
            IncrementTotalMatchCount();
            if (isTeamMatch)
            {
                if (won) TeamSurvivalWinCount++;
                else TeamSurvivalLoseCount++;
            }
            else
            {
                if (won) SurvivalWinCount++;
                else SurvivalLoseCount++;
            }
        }

        public void RecordCtfFlagReturn() => CtfFlagReturn++;
        public void RecordCtfFlagInterrupt() => CtfFlagInterrupt++;
    }
}
