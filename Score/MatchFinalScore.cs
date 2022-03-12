using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class MatchFinalScore
    {
        private int _kill;

        private int _flagDefence = 0;

        private int _suicide = 0;


        public int Rank { get; private set; } = 0;

        public int TotalPoint { get; private set; } = 0;

        public MatchFinalScore()
        {

        }

    }
}
