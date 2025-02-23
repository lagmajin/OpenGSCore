using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{

    public class AbstractMatchFinalScore
    {
        public EGameMode Mode { get; private set; }

        public List<AbstractPlayerMatchFinalScore> FinalScores { get; private set; }

    }

    public class DeathMatchFinalScore : AbstractMatchFinalScore
    {

    }

    public class TeamDeathMatchFinalScore : AbstractMatchFinalScore
    {

    }

    public class CTFMatchFinalScore : AbstractMatchFinalScore
    {

    }

}
