
using System;


namespace OpenGSCore
{
    public sealed class CTFMatchRule : AbstractMatchRule
    {

        public CTFMatchRule(CTFMatchSetting setting) : base()
        {

        }

        public override bool CanRespawn()
        {

            return true;
        }

        public override bool D(in MatchData data)
        {

            return true;
        }
    }
}
