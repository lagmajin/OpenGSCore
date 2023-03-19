


namespace OpenGSCore
{
    public sealed class SuvMatchRule : AbstractMatchRule
    {
        public SuvMatchRule(in SuvMatchSetting setting) : base()
        {

        }

        public override bool CanReSpawn()
        {
            return false;
        }

        public override bool D(in AbstractMatchSituation d)
        {
            return false;
        }
    }
}
