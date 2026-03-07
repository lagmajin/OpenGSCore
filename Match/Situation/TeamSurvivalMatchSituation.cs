using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class TeamSurvivalMatchSituation : AbstractTeamMatchSituation
    {
        public int RedTeamAliveCount { get; set; } = 0;
        public int BlueTeamAliveCount { get; set; } = 0;
        
        // 残機合計（オプション）
        public int RedTeamTotalLives { get; set; } = 0;
        public int BlueTeamTotalLives { get; set; } = 0;
    }
}
