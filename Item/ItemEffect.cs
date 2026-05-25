using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class PowerUpItemEffect:AbstractItemEffect
    {

        public PowerUpItemEffect() { }

        public override void ApplyItemEffect(PlayerStatus status)
        {
            if (status == null)
            {
                return;
            }

            status.AddHp(100f);
            status.AddBooster(25f);
            status.AddAttackPower(5);
        }

        public override void UnApplyItemEffect(PlayerStatus status)
        {
            if (status == null)
            {
                return;
            }

            status.Damage(100f);
            status.UseBooster(25f);
            status.AddAttackPower(-5);
        }

    }

    public class DefenceUpItemEffect:AbstractItemEffect
    {


        public override void ApplyItemEffect(PlayerStatus status)
        {
            if (status == null)
            {
                return;
            }

            status.AddHp(50f);
            status.AddBooster(50f);
            status.AddDefensePower(5);
        }

        public override void UnApplyItemEffect(PlayerStatus status)
        {
            if (status == null)
            {
                return;
            }

            status.Damage(50f);
            status.UseBooster(50f);
            status.AddDefensePower(-5);
        }
    }

    public class NormalGranadePackItemEffect : AbstractItemEffect
    {
        public override void ApplyItemEffect(PlayerStatus status)
        {
            status?.RefillGrenade();
        }

        public override void UnApplyItemEffect(PlayerStatus status)
        {
            // 消費型なので解除処理は不要
        }
    }

    public class SpeedUpItemEffect : AbstractItemEffect
    {
        public override void ApplyItemEffect(PlayerStatus status)
        {
            if (status == null)
            {
                return;
            }

            status.AddBooster(20f);
        }

        public override void UnApplyItemEffect(PlayerStatus status)
        {
            if (status == null)
            {
                return;
            }

            status.UseBooster(20f);
        }
    }

    public class StealthItemEffect : AbstractItemEffect
    {
        public override void ApplyItemEffect(PlayerStatus status)
        {
            if (status == null)
            {
                return;
            }

            status.AddDefensePower(1);
        }

        public override void UnApplyItemEffect(PlayerStatus status)
        {
            if (status == null)
            {
                return;
            }

            status.AddDefensePower(-1);
        }
    }

    public class HealItemEffect : AbstractItemEffect
    {
        public override void ApplyItemEffect(PlayerStatus status)
        {
            status?.AddHp(150f);
        }

        public override void UnApplyItemEffect(PlayerStatus status)
        {
            if (status == null)
            {
                return;
            }

            status.Damage(150f);
        }
    }
}
