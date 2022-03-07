using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{




    public enum eGameMode : byte
    {
        DeathMatch=0,
        Practice,
        FreeStyle,
        TDM,
        OneShotKill,
        Sniper,
        TowerMatch,
        Survival,
        TeamSurvival,
        CTF,
        Unknown,
        ArmsRace

    }

    public class GameMode
    {
       private  eGameMode mode=eGameMode.Unknown;

        private string str = "";

        public static List<eGameMode> AllGameMode()
        {
            var result = new List<eGameMode>();

            result.Add(eGameMode.DeathMatch);
            result.Add(eGameMode.TDM);
            result.Add(eGameMode.Survival);
            result.Add(eGameMode.TeamSurvival);
            result.Add(eGameMode.CTF);
            result.Add(eGameMode.OneShotKill);
            result.Add(eGameMode.ArmsRace);

            return result;
        }
        public GameMode(eGameMode mode)
        {

        }

        public GameMode(in string mode)
        {
            var temp = mode.ToLower();
            str = "unkonown";

            

            if(temp=="DeathMatch" || temp=="DM")
            {
                str = "dm";
            }

            if (temp == "TeamDeathMatch" || temp == "TDM")
            {
                str = "tdm";
            }

            if (temp=="Survival"|| temp=="SUV")
            {
                str = "suv";
            }
            if (temp == "TeamSurvival" || temp == "TSUV")
            {
                str = "tsuv";
            }

            if(temp=="CaptureTheFlag" || temp=="CTF")
            {
                str = "ctf";
            }

            

        }

        public string Name()
        {
            return str;
        }

        public bool Valid()
        {
            return false;
        }

        internal eGameMode Mode { get => mode; set => mode = value; }
    }


}
