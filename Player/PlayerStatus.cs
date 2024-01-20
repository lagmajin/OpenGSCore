using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public enum EPlayerType
    {
        Unknown,
        MyPlayer,
        OtherPlayer,
        AI
    }


    public sealed class PlayerStatus
    {
        private readonly object lockObject = new();

        private ETeam Team { get; set; }=ETeam.NoTeam;

     
        public float MaxHp { get; private set; } = 500;
        public float Hp { get; set; } = 500;
        public float MaxBooster { get; private set; } = 100;
        public float Booster { get; set; } = 100;

        public float BoosterPowerGround { get; set; } = 3.0f;
        public float BoosterPower { get; set; } = 1.0f;

        public PlayerStatus()
        {




        }

        public PlayerStatus(int maxHP=500, float maxBooster=100.0f, float boosterPower=1.0f)
        {
            MaxHp = maxHP;
            MaxBooster = maxBooster;
            BoosterPower = boosterPower;

        }

        public PlayerStatus(ETeam team = ETeam.NoTeam,EPlayerType type=EPlayerType.Unknown, int maxHP = 550,float maxBooster=100.0f)
        {
            Team = team;


        }

        //public PlayerStatus()


    }


}
