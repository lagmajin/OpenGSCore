


namespace OpenGSCore
{

    public interface IAbstractMatchSituation
    {


    }
    public class AbstractMatchSituation : IAbstractMatchSituation
    {
        public eGameMode mode;

        public int AlivePlayers { get; set; } = 0;



        public int TotalKill { get; set; } = 0;
        public int TotalDeath { get; set; } = 0;



        public int MaxPlayerKillCount { get; set; } = 0;




        public void UpdateTime(in int elapsedTime)
        {
            //elapsedTime_ += elapsedTime;
        }



    }


    public class AbstractTeamMatchSituation : AbstractMatchSituation
    {
        public int RedTeamKill { get; set; } = 0;
        public int BlueTeamKill { get; set; } = 0;
    }

}
