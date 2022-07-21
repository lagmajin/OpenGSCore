using System;
using System.Collections.Generic;
using System.Text;
using Des;

namespace OpenGSCore
{
    public enum eStage
    {
        Unknown,
        AuroraClassic,
        Alps,
        DryDays,
        GreenHill1,
        GreenHill2,
        CityOfDarkness1,
        CityOfDarkness2,
        BluffStructure1,
        BluffStructure2,
        Jungle1,
        Jungle2,
        Port1,
        Port2,
        Ruin1,
        Ruin2,
        Nocturn,
        Waterfall,
        Christmas
    }

    public enum eDificullcy
    {

    }

    public class StageInfo
    {
        private int boostLevel = 0;

        public eGameMode GameMode { get; set; }

        public eStage Stage { get; set; }

    }
}
