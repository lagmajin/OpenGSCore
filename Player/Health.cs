using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class Health
    {
        public int health { get; private set; } = 0;
        public Health()
        {

        }

        public bool IsDead()
        {
            if (health == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

       
        }

    }
}
