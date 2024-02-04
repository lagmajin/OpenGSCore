using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public abstract class AbstractItemEffect
    {

        public AbstractItemEffect() { }

        public abstract void ApplyItemEffect(PlayerStatus status);
        public abstract void UnApplyItemEffect(PlayerStatus status);
    }
}
