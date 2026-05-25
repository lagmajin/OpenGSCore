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
}
