using OpenGSCore.Score;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{

    public abstract class AbstractMatchFinalScore
    {
        public EGameMode Mode { get; private set; }

        public abstract AbstractAllPlayerMatchFinalScore AllPlayerFinalScores();

    }

    public class DeathMatchFinalScore : AbstractMatchFinalScore
    {
        public AllPlayerDeathMatchPlayerMatchFinalScore allPlayerFinalScores;

        public override AbstractAllPlayerMatchFinalScore AllPlayerFinalScores()
        {
            return allPlayerFinalScores;
        }
    }

    public class TeamDeathMatchFinalScore : AbstractMatchFinalScore
    {
        public AllPlayerTeamDeathMatchPlayerMatchFinalScore allPlayerFinalScores;


        public override AbstractAllPlayerMatchFinalScore AllPlayerFinalScores()
        {
            return null;
        }
    }

    public class CTFMatchFinalScore : AbstractMatchFinalScore
    {



        public override AbstractAllPlayerMatchFinalScore AllPlayerFinalScores()
        {

            return null;
        }
    }

}
