using System;
using System.Collections.Generic;
using System.Text;
using Des;

namespace OpenGSCore
{


    public enum eMap
    {
        Unknown,
        AuroraClassic,
        IceValley,
        DryDays,
        GreenHillSide1,
        GreenHillSide2,
        CityOfDarkness1,
        CityOfDarkness2,
        BluffStructure1,
        BluffStructure2,
        DesertedJungleSide1,
        DesertedJungleSide2,
        BattlePort1,
        BattlePortCTF,
        FullHouse,
        FactoryInGaol,
        RobotFactory,
        RedStorm1,
        RedStorm2,
        ThePark,
        TheParkCTF,
        RuinOfWarSide1,
        RuinOfWarSide2,
        Nocturn,
        Waterfall,
        SkyHigh,
        SkyHighCTF,
        GhostHouse,
        OnStudio,
        Christmas,
        SeaSideBase,
        RandomMap,
    }

    public enum eDificullcy
    {

    }

    public class MapInfo
    {
        private int boostLevel = 0;

        public eGameMode GameMode { get; set; }

        //public eStage Stage { get; set; }


        public eMap Map { get; set; }
    }
}
