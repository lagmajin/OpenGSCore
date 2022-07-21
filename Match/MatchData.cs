using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenGSCore
{

    public interface IMatchData
    {

    }
    public class MatchData : IMatchData
    {
        //Dictionary<string, PlayerStatus> otherPlayers = new Dictionary<string, PlayerStatus>();

        private int elapsedTime_ = 0;
        private int reminingTime = 0;



        private int redTeamAlivePlayers = 0;
        private int blueTeamAlivePlayers = 0;


        private List<string> mostKillPlayerID;

        private List<string> mostReturnPlayerID;

        public int AlivePlayer { get; set; }

        public int TotalKill { get; set; } = 0;
        public int TotalDeath { get; set; } = 0;
        public int RedTeamKill { get; set; } = 0;
        public int BlueTeamKill { get; set; } = 0;
        public int RedTeamFlagReturn { get; set; } = 0;
        public int BlueTeamFlagReturn { get; set; } = 0;
        public int MaxPlayerKillCount { get; set; } = 0;

        public void UpdateTime(in int elapsedTime)
        {
            elapsedTime_ += elapsedTime;
        }



    }

}
