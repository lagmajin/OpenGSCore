using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;




namespace OpenGSCore
{
    public class AllMatchPlayer
    {
        //public SortedDictionary<string, PlayerStatus> AllPlayers { get;set; } = new();

        private List<PlayerStatus> players = new();


        public AllMatchPlayer()
        {

        }


        public PlayerStatus? SearchPlayer()
        {
            var player = new PlayerStatus();

            


            return player;

        }

        public List<PlayerStatus>? SearchPlayers()
        {
            var list = new List<PlayerStatus>();


            return null;
        }

        

        public List<PlayerStatus>? SearchTeamPlayers(eTeam team)
        {

            var teamPlayers = new List<PlayerStatus>();


            return null;
        }

        public void AddPlayer(string id,string displayName)
        {

        }




    }
}
