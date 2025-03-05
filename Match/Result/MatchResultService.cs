using OpenGSServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class MatchResultService
    {


        public AbstractMatchResult createMatchResult(AbstractMatchFinalScore score)
        {
            //var playerFinalScore=score.FinalScores;

            switch(score)
            {
                case DeathMatchFinalScore deathMatchScore:
                    
                    var deathMatchResult=new DeathMatchResult();



                    return deathMatchResult;

                    break;

                case TeamDeathMatchFinalScore teamDeathMatchScore:
                    var teamDeathMatchResult=new TeamDeathMatchResult();


                    return null;
                    break;



                case CTFMatchFinalScore cTFMatchFinalScore:

                    return null;

                    break;


            }

        

            return null;
        }

    }
}
