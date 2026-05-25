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

     
        public float MaxHp { get; set; } = 500;
        public float Hp { get; set; } = 500;
        public float MaxBooster { get;set; } = 100;
        public float Booster { get; set; } = 100;

        public float BoosterPowerGround { get; set; } = 3.0f;
        public float BoosterPower { get; set; } = 1.0f;
        public int AttackPower { get; set; } = 10;
        public int DefensePower { get; set; } = 5;
        public int GrenadeCount { get; set; } = 3;
        public int MaxGrenadeCount { get; set; } = 3;

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

        public void AddHp(float amount)
        {
            if (amount <= 0) return;
            Hp = Math.Min(MaxHp, Hp + amount);
        }

        public void Damage(float amount)
        {
            if (amount <= 0) return;
            Hp = Math.Max(0, Hp - amount);
        }

        public void AddBooster(float amount)
        {
            if (amount <= 0) return;
            Booster = Math.Min(MaxBooster, Booster + amount);
        }

        public void UseBooster(float amount)
        {
            if (amount <= 0) return;
            Booster = Math.Max(0, Booster - amount);
        }

        public void RefillBooster()
        {
            Booster = MaxBooster;
        }

        public void AddAttackPower(int amount)
        {
            AttackPower = Math.Max(0, AttackPower + amount);
        }

        public void AddDefensePower(int amount)
        {
            DefensePower = Math.Max(0, DefensePower + amount);
        }

        public void RefillGrenade()
        {
            GrenadeCount = MaxGrenadeCount;
        }

        public bool UseGrenade()
        {
            if (GrenadeCount <= 0)
            {
                return false;
            }

            GrenadeCount--;
            return true;
        }


    }


}
