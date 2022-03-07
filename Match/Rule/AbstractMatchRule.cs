




namespace OpenGSCore
{
    public interface IMatchRule
    {

        bool D(in MatchData d);
        bool CanRespawn();

        public int MatchTimeMsec();
        //bool HasTimeLimit();
    }

    abstract public class AbstractMatchRule:IMatchRule
    {
        private eGameMode mode = new eGameMode();
        abstract public bool D(in MatchData d);
        abstract public bool CanRespawn();

        private bool hasTimeLimit = false;
        private int matchTimeMsec = 3600;

        public AbstractMatchRule()
        {

        }

        public AbstractMatchRule(eGameMode mode,int matchTimeMsec)
        {

        }

        public int MatchTimeMsec()
        {
            return matchTimeMsec;
        }



    }



}
