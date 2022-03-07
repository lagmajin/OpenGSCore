
namespace OpenGSCore
{
    public class TSuvMatchRule:AbstractMatchRule
    {
        public TSuvMatchRule():base(eGameMode.TeamSurvival,0)
        {

        }

        public TSuvMatchRule(in TSuvMatchSetting setting)
        {

        }

        override public bool D(in MatchData d)
        {

            return false;
        }

        override public bool CanRespawn()
        {

            return false;
        }

    }
}
