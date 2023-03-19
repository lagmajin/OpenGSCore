


namespace OpenGSCore
{


    public sealed class DeathMatchRule : AbstractMatchRule
    {
        public DeathMatchRule(int killCondition = 20, int matchTimeMsec = 1000) : base(EGameMode.DeathMatch, matchTimeMsec)
        {

        }

        public DeathMatchRule(in DeathMatchSetting setting) : base()
        {

        }

        public override bool CanReSpawn()
        {
            return true;
        }

        public override bool D(in AbstractMatchSituation data)
        {



            return false;
        }
    }
}
