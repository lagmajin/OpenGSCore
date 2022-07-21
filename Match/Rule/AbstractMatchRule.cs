




namespace OpenGSCore
{
    public interface IMatchRule
    {

        bool D(in MatchData d);
        bool CanReSpawn();

        public int MatchTimeMSec();
        //bool HasTimeLimit();
    }

    public abstract class AbstractMatchRule : IMatchRule
    {
        private eGameMode mode = new eGameMode();
        public abstract bool D(in MatchData d);
        public abstract bool CanReSpawn();

        private bool hasTimeLimit = false;
        private int matchTimeMsec = 3600;

        public AbstractMatchRule()
        {

        }

        public AbstractMatchRule(eGameMode mode, int matchTimeMsec)
        {

        }

        public int MatchTimeMSec()
        {
            return matchTimeMsec;
        }



    }



}
