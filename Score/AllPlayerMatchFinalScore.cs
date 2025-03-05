using OpenGSCore;
using OpenGSCore.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OpenGSCore.Score
{
    public interface IAllPlayerMatchFinalScore
    {

    }

    public class AbstractAllPlayerMatchFinalScore
    {
        public List<AbstractPlayerMatchFinalScore> AllPlayerFinalScore() { return null; }
    }

    public class AllPlayerDeathMatchPlayerMatchFinalScore:AbstractAllPlayerMatchFinalScore
    {
        public List<PlayerDeathMatchFinalScore> scores = new();

        public List<AbstractPlayerMatchFinalScore> AllPlayerFinalScore() { return scores.Cast<AbstractPlayerMatchFinalScore>().ToList(); }
    }

    public class AllPlayerTeamDeathMatchPlayerMatchFinalScore:AbstractAllPlayerMatchFinalScore
    {
        public List<PlayerTeamDeathMatchFinalScore> scores = new();

        public List<AbstractPlayerMatchFinalScore> AllPlayerFinalScore() { return scores.Cast<AbstractPlayerMatchFinalScore>().ToList(); }
    }

    public class AllCaptureTheFlagMatchPlayerFinalScore : AbstractAllPlayerMatchFinalScore
    {
        public List<PlayerCTFMatchFinalScore> scores = new();

        public List<AbstractPlayerMatchFinalScore> AllPlayerFinalScore() { return scores.Cast<AbstractPlayerMatchFinalScore>().ToList(); }
    }

}
