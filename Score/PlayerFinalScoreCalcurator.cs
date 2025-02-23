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

    
        public static MatchResultScore calcScore(AbstractPlayerMatchFinalScore finalScore)
        {

            switch (finalScore.Mode)
            {
                case EGameMode.DeathMatch:
                    if (finalScore is PlayerDeathMatchFinalScore deathMatchScore)
                    {


                        

                    }

                        break;

                case EGameMode.TeamDeathMatch:

                    if (finalScore is PlayerTeamDeathMatchFinalScore teamDeathMatchScore)
                    {


                    }

                    break;

            }


            return null;
        }



    }


}
