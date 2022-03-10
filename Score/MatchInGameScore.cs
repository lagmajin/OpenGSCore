﻿using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    interface IPlayerScore
    {

    }

    public sealed class MatchInGameScore
    {

        public int Kill { get; private set; } = 0;
        public int Death { get; private set; } = 0;

        public int Suicide { get; private set; } = 0;

        public int Killed { get; private set; } = 0;
        public int TotalDamage { get; private set; } = 0;

        public int FlagCarrierKill { get; private set; } = 0;
        public int RecoverFlag { get; private set; } = 0;
        public int FlagReturn { get; private set; } = 0;



        public void AddKill(int kill)
        {
            Kill += kill;

        }

        public void MinusKill()
        {
            if (Kill > 0)
            {
                Kill--;
            }


        }

        public void AddDeath(int death = 1)
        {



        }

        public void AddSuicide()
        {
            MinusKill();
            AddDeath();



        }

        public void AddFlagCarrierKill()
        {
            FlagCarrierKill++;

        }


        public void AddTotalDamage(int damage)
        {
            if (damage < 0)
            {
                damage = 0;
            }

            TotalDamage += damage;

        }

        public void MinusTotalDamage(int damage)
        {

        }


    }
}
