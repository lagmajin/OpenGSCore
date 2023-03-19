


namespace OpenGSCore
{
    public sealed class TDMMatchRule : AbstractMatchRule
    {
        public TDMMatchRule()
        {

        }

        public TDMMatchRule(in TDMMatchSetting setting)
        {

        }

        public override bool D(in AbstractMatchSituation d)
        {

            return false;
        }

        public override bool CanReSpawn()
        {
            return true;
        }
    }
}
