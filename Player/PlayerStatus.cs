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

        //private float boosterPowerOnGround = 4.0f;

        private float boosterPower = 1.0f;



        public float MaxHp { get; private set; } = 500;
        public float Hp { get => hp; set => hp = value; }
        public float MaxBooster { get => booster; }
        public float Booster { get => booster; set => booster = value; }

        public float BoosterPowerGround { get; set; } = 4.0f;
        public float BoosterPower { get => boosterPower; set => boosterPower = value; }
        public eTeam? Team { get => team; set => team = value; }
    }


}
