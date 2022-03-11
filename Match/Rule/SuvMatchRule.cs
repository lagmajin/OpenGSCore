


namespace OpenGSCore
{
    public sealed class SuvMatchRule : AbstractMatchRule
    {
        public SuvMatchRule(in SuvMatchSetting setting) : base()
        {

        }

        public override bool CanRespawn()
        {
            return false;
        }

        public override bool D(in MatchData d)
        {
            return false;
        }
    }
}
