
using System;


namespace OpenGSCore
{
    public sealed class CaptureTheFlagMatchRule : AbstractMatchRule
    {
        private CaptureTheFlagMatchRule rule = null;

        public CaptureTheFlagMatchRule()
        {
        }

        public CaptureTheFlagMatchRule(CaptureTheFlagMatchSetting setting) : base()
        {

        }

        public override bool CanReSpawn()
        {

            return true;
        }

        public override bool D(in AbstractMatchSituation data)
        {
            var ctfSituation = data as CaptureTheFlagMatchSituation;

            var redTeamFlag = false;
            var blueTeamFlag = false;

            if (ctfSituation.RedTeamFlagReturn > 3)
            {

            }


            //ctfSituation.BlueTeamFlagReturn




            return false;
        }
    }
}
