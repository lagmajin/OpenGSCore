using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class CaptureTheFlagMatchSituation : AbstractTeamMatchSituation
    {
        public int RedTeamFlagReturn { get; set; } = 0;
        public int BlueTeamFlagReturn { get; set; } = 0;

        public void AddFlagReturn(ETeam team)
        {
            switch (team)
            {
                case ETeam.Red:
                    RedTeamFlagReturn++;
                    break;
                case ETeam.Blue:
                    BlueTeamFlagReturn++;
                    break;
            }
        }

        public void AddTeamKill(ETeam team)
        {
            switch (team)
            {
                case ETeam.Red:
                    RedTeamKill++;
                    break;
                case ETeam.Blue:
                    BlueTeamKill++;
                    break;
            }
        }
    }
}
