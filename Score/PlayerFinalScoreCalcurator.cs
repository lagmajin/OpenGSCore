using System;

namespace OpenGSCore
{
    [Obsolete("Use PlayerFinalScore.CalcTotalPoint() directly")]
    public class PlayerFinalScoreCalculator
    {
        public static void CalcScore(PlayerFinalScore finalScore)
        {
            finalScore?.CalcTotalPoint();
        }
    }

    [Obsolete("Use PlayerFinalScoreCalculator")]
    public class PlayerFinalScoreCalcurator : PlayerFinalScoreCalculator
    {
    }
}
