


namespace OpenGSCore
{


    public sealed class DeathMatchRule : AbstractMatchRule
    {
        public DeathMatchRule(int killCondition = 20, int matchTimeMsec = 1000) : base(eGameMode.DeathMatch, matchTimeMsec)
        {

        }

        public DeathMatchRule(in DMMatchSetting setting) : base()
        {

        }

        public override bool CanReSpawn()
        {
            return true;
        }

        public override bool D(in MatchData data)
        {



            return false;
        }
    }
}
