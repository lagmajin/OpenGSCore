using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public sealed class MissionSetting
    {
        public bool LifeLimit { get; } = true;

        public int LifeCount { get; } = 3;

        public int MaxPlayer { get; } = 3;

        public MissionSetting(int life = 3, int maxPlayer = 3)
        {

        }
    }
}
