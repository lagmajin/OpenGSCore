


namespace OpenGSCore
{
    public sealed class SuvMatchRule : AbstractMatchRule
    {
        public SuvMatchRule() : base(EGameMode.Survival)
        {
        }

        public SuvMatchRule(in SuvMatchSetting setting) : base(EGameMode.Survival, setting.MatchTimeMSec)
        {

        }

        public override bool CanReSpawn()
        {
            return false;
        }

        public override bool IsMatchFinished(AbstractMatchSituation situation)
        {
            return false;
        }
    }
}
