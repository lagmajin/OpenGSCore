using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class CaptureTheFlagMatchSituation : AbstractTeamMatchSituation
    {
        public int RedTeamFlagReturn { get; set; } = 0;
        public int BlueTeamFlagReturn { get; set; } = 0;
    }
}
