using OpenGSCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGSServer
{

    public class DeathMatchFinalScore
    {


    }
    public class DeathMatchResult:AbstractMatchResult
    {

        public DeathMatchFinalScore FinalScore()
        {
            var result = new DeathMatchFinalScore();

            return result;
        }


    }
}
