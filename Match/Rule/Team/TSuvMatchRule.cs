
namespace OpenGSCore
{
    public sealed class TSuvMatchRule : AbstractMatchRule
    {

        public TSuvMatchRule(bool teamBalance = true) : base(EGameMode.TeamSurvival, 0)
        {

        }

        public TSuvMatchRule(in TeamSurvival setting)
        {

        }

        public override bool D(in AbstractMatchSituation d)
        {

            return false;
        }

        public override bool CanReSpawn()
        {

            return false;
        }

    }
}
