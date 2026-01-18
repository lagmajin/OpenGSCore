using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OpenGSCore
{
    public class TeamSurvivalMatchSetting : AbstractTeamMatchSetting
    {
        public int SurvivalTimeMinutes { get; set; } = 10;
        public bool LastTeamStanding { get; set; } = true;

        public TeamSurvivalMatchSetting(int maxPlayerCapacity = 8, bool teamBalance = true)
            : base(EGameMode.TeamSurvival, true, teamBalance)
        {
            SurvivalTimeMinutes = 10;
            LastTeamStanding = true;
        }

        public new JObject ToJson()
        {
            var result = base.ToJson();

            result["MatchType"] = "TeamSurvival";
            result["SurvivalTimeMinutes"] = SurvivalTimeMinutes;
            result["LastTeamStanding"] = LastTeamStanding;

            return result;
        }
    }
}