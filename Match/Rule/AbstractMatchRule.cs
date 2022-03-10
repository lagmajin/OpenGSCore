




namespace OpenGSCore
{
    public interface IMatchRule
    {

        bool D(in MatchData d);
        bool CanRespawn();

        public int MatchTimeMsec();
        //bool HasTimeLimit();
    }

    public abstract class AbstractMatchRule : IMatchRule
    {
        private eGameMode mode = new eGameMode();
        public abstract bool D(in MatchData d);
        public abstract bool CanRespawn();

        private bool hasTimeLimit = false;
        private int matchTimeMsec = 3600;

        public AbstractMatchRule()
        {

        }

        public AbstractMatchRule(eGameMode mode, int matchTimeMsec)
        {

        }

        public int MatchTimeMsec()
        {
            return matchTimeMsec;
        }



    }



}
