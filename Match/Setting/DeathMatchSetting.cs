using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace OpenGSCore
{
    public class DeathMatchSetting : AbstractMatchSetting
    {
        private int winConditionKill_ = 10;
        public DeathMatchSetting(int winConditionKill, bool teamBalance = true) : base(EGameMode.DeathMatch, 0, teamBalance)
        {
            winConditionKill_ = 20;

        }

        public override JObject ToJson()
        {
            var result = base.ToJson();

            result["WinConditionKill"] = "";


            return result;

        }
    }
}
