using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public sealed class MatchLimitations
    {
        public bool CanUseFieldItem { get; private set; } = true;

        public bool CanEquipBooster { get; private set; } = true;

        public bool CanEquipInstantItem { get; private set; } = true;


        public MatchLimitations()
        {

        }


    }


}
