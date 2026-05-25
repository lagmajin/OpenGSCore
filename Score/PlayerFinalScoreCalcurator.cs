using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;



namespace OpenGSCore
{
    public interface IPlayerFinalScoreCalcurator
    {

    }
    /// <summary>
    /// スコアの計算を担当するクラス。  
    /// This class handles score calculations.
    /// </summary>
    /// #Score
    public class PlayerFinalScoreCalcurator:IPlayerFinalScoreCalcurator
    {


        public PlayerFinalScoreCalcurator() { }

    
        public static void calcScore(AbstractPlayerMatchFinalScore finalScore)
        {
            if (finalScore == null)
            {
                return;
            }

            switch (finalScore.Mode)
            {
                case EGameMode.DeathMatch:
                    if (finalScore is PlayerDeathMatchFinalScore deathMatchScore)
                    {
                        deathMatchScore.CalcTotalPoint();
                    }

                    break;

                case EGameMode.TeamDeathMatch:
                    if (finalScore is PlayerTeamDeathMatchFinalScore teamDeathMatchScore)
                    {
                        teamDeathMatchScore.CalcTotalPoint();
                    }

                    break;

                case EGameMode.CaptureTheFlag:
                    if (finalScore is PlayerCTFMatchFinalScore ctfMatchScore)
                    {
                        ctfMatchScore.CalcTotalPoint();
                    }

                    break;

                default:
                    finalScore.CalcTotalPoint();
                    break;
            }
        }



    }


}
