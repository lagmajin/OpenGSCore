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
    public class MatchData:IMatchData
    {
        //Dictionary<string, PlayerStatus> otherPlayers = new Dictionary<string, PlayerStatus>();

        private int elapsedTime_ = 0;
        private int reminingTime = 0;

        private int alivePlayer = 0;

        private int totalDeath = 0;
        private int totalKill = 0;

        private int redTeamTotalKill = 0;
        private int blueTeamTotalKill = 0;

        private int redTeamAlivePlayers = 0;
        private int blueTeamAlivePlayers = 0;


        private int redTeamFlagReturn = 0;
        private int blueTeamFlagReturn = 0;

        private List<string> mostKillPlayerID;

        private List<string> mostReturnPlayerID;

        public int AlivePlayer { get => alivePlayer; set => alivePlayer = value; }
        public int AllDeath { get => totalDeath; set => totalDeath = value; }
        public int AllKill { get => totalKill; set => totalKill = value; }
        public int RedTeamKill { get => redTeamTotalKill; set => redTeamTotalKill = value; }
        public int BlueTeamKill { get => blueTeamTotalKill; set => blueTeamTotalKill = value; }
        public int RedTeamFlagReturn { get => redTeamFlagReturn; set => redTeamFlagReturn = value; }
        public int BlueTeamFlagReturn { get => blueTeamFlagReturn; set => blueTeamFlagReturn = value; }
        public int MaxPlayerKillCount { get; set; }

        public void UpdateTime(in int elapsedTime)
        {
            elapsedTime_ += elapsedTime;
        }



    }

}
