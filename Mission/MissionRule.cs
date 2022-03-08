using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class MissionRule
    {
        public bool CanRespawn { get; } = true;

        public int MaxLife { get; } = 5;

        public MissionRule(in MissionSetting setting)
        {

        }

    }
}
