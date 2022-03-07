


namespace OpenGSCore
{
    public class TDMMatchRule:AbstractMatchRule
    {
        public TDMMatchRule()
        {

        }

        public TDMMatchRule(in TDMMatchSetting setting)
        {

        }

        override public bool D(in MatchData d)
        {

            return false;
        }

        override public bool CanRespawn()
        {
            return true;
        }
    }
}
