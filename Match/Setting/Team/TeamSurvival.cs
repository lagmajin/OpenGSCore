using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

//using System.Text.Json;



namespace OpenGSCore
{
    public class TeamSurvival : AbstractTeamMatchSetting
    {

        public TeamSurvival(bool randomTeam, bool teamBalance = true) : base(EGameMode.TeamSurvival)
        {

        }

        public JObject ToJson()
        {
            var result = base.ToJson();



            return result;

        }

    }
}
