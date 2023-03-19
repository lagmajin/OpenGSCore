




namespace OpenGSCore
{
    public interface IMatchRule
    {

        bool D(in AbstractMatchSituation situation);
        bool CanReSpawn();

        public int MatchTimeMSec();
        //bool HasTimeLimit();
    }

    public abstract class AbstractMatchRule : IMatchRule
    {
        private EGameMode mode = new EGameMode();
        
        
        
        public abstract bool D(in AbstractMatchSituation d);

        private bool canRespawn = true;
        public virtual bool CanReSpawn()
        {
            return canRespawn;
        }

        

        private bool hasTimeLimit = false;
        private int matchTimeMsec = 3600;

        public AbstractMatchRule()
        {

        }

        public AbstractMatchRule(EGameMode mode, int matchTimeMsec)
        {

        }

        public int MatchTimeMSec()
        {
            return matchTimeMsec;
        }



    }



}
