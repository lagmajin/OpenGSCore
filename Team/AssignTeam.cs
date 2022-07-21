using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class AssignTeamResult
    {
        public List<string> RedTeam { get; private set; }
        public List<string> BlueTeam { get; private set; }

        public AssignTeamResult(List<string> redTeam, List<string> blueTeam)
        {

        }
    }

    public class AssignTeam
    {
        public static AssignTeamResult? Random(List<string> idList)
        {
            if (idList.Count == 0)
            {

                return null;
            }


            var redTeam = new List<string>();

            var blueTeam = new List<string>();

            var result = new AssignTeamResult(redTeam,blueTeam);


            return result;

        }

    }
}
