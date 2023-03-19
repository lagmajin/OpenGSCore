using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class PlayerScore
    {
        public int MatchCount { get; set; } = 0;

        public int DeathMatchCount { get; set; } = 0;
        public int DeathMatchWinCount { get; set; } = 0;
        public int DeathMatchLoseCount { get; set; } = 0;

        public int TeamDeathMatchWinCount { get; set; } = 0;


        public int SurvivalWinCount { get; set; } = 0;
        public int SurvivalLoseCount { get; set; } = 0;

        public int TeamSurvivalWinCount { get; set; } = 0;
        public int TeamSurvivalLoseCount { get; set; } = 0;

        public int CtfFlagReturn { get; set; } = 0;

        //public int CtfFlagDefence { get; set; } = 0;
        public int CtfFlagInterrupt { get; set; } = 0;


        public PlayerScore()
        {

        }

    }
}
