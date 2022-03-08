using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    interface IPlayerScore
    {

    }

    public class MatchInGameScore
    {
        int death = 0;
        int suicide = 0;
        int totalDamage = 0;

        int flagDefence = 0;
        int flagReturn = 0;
        int salvageFrag = 0;

        public int Kill { get; } = 0;
        public int Death { get; } = 0;

        public int Suicide { get; } = 0;
        public int TotalDamage { get; } = 0;
        public int FlagReturn { get; } = 0;
        public int SalvageFrag { get; } = 0;

        public void AddKill()
        {

        }
        public void AddDeath()
        {

        }


    }
}
