using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class TimeLimit
    {
        private long timeLimit = 0;

        private bool hasLimit = true;

        public TimeLimit()
        {

        }

        public void SetTimeLimit(long limit = 0)
        {

        }

        public void NoTimeLimit()
        {
            hasLimit = false;
        }


    }
}
