using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

//using System.Text.Json;



namespace OpenGSCore
{
    public class TSuvMatchSetting:AbstractTeamMatchSetting
    {
        private bool randomTeam=false;
        public TSuvMatchSetting(bool randomTeam,bool teamBalance=true):base(eGameMode.TeamSurvival)
        {

        }

        public override JObject ToJson()
        {
            var result = new JObject();

            return result;

        }

    }
}
