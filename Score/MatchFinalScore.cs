using OpenGSCore.Score;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{

    public abstract class AbstractMatchFinalScore
    {
        public EGameMode Mode { get; private set; }

        protected AbstractMatchFinalScore(EGameMode mode)
        {
            Mode = mode;
        }

        public abstract AbstractAllPlayerMatchFinalScore AllPlayerFinalScores();

    }

    public class DeathMatchFinalScore : AbstractMatchFinalScore
    {
        public AllPlayerDeathMatchPlayerMatchFinalScore allPlayerFinalScores;

        public DeathMatchFinalScore() : base(EGameMode.DeathMatch)
        {
        }

        public override AbstractAllPlayerMatchFinalScore AllPlayerFinalScores()
        {
            allPlayerFinalScores ??= new AllPlayerDeathMatchPlayerMatchFinalScore();
            return allPlayerFinalScores;
        }
    }

    public class TeamDeathMatchFinalScore : AbstractMatchFinalScore
    {
        public AllPlayerTeamDeathMatchPlayerMatchFinalScore allPlayerFinalScores;

        public TeamDeathMatchFinalScore() : base(EGameMode.TeamDeathMatch)
        {
        }


        public override AbstractAllPlayerMatchFinalScore AllPlayerFinalScores()
        {
            allPlayerFinalScores ??= new AllPlayerTeamDeathMatchPlayerMatchFinalScore();
            return allPlayerFinalScores;
        }
    }

    public class CTFMatchFinalScore : AbstractMatchFinalScore
    {
        public CTFMatchFinalScore() : base(EGameMode.CaptureTheFlag)
        {
        }



        public override AbstractAllPlayerMatchFinalScore AllPlayerFinalScores()
        {

            return new AllCaptureTheFlagMatchPlayerFinalScore();
        }
    }

}
