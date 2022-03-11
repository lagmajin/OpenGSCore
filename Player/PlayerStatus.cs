using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public enum ePlayerType
    {
        Unknown,
        MyPlayer,
        OtherPlayer,
        AI
    }


    public sealed class PlayerStatus
    {
        ePlayerType playerType = ePlayerType.Unknown;


        private eTeam? team;



        private float hp = 500;

        private float booster = 100;
        private float maxBooster = 100;

        private float boosterPower = 3.0f;

        public float MaxHp { get; private set; } = 500;
        public float Hp { get => hp; set => hp = value; }
        public float MaxBooster { get => booster; }
        public float Booster { get => booster; set => booster = value; }

        public float BoosterPower { get => boosterPower; set => boosterPower = value; }
        public eTeam? Team { get => team; set => team = value; }
    }


}
