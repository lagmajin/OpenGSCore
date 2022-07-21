
namespace OpenGSCore
{
    public sealed class TSuvMatchRule : AbstractMatchRule
    {

        public TSuvMatchRule(bool teamBalance = true) : base(eGameMode.TeamSurvival, 0)
        {

        }

        public TSuvMatchRule(in TSuvMatchSetting setting)
        {

        }

        override public bool D(in MatchData d)
        {

            return false;
        }

        public override bool CanRespawn()
        {

            return false;
        }

    }
}
