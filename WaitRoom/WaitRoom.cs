using System;
using System.Collections.Generic;
using System.Text;




namespace OpenGSCore
{
    //public interface IWaitRoom { }


    public class WaitRoom
    {
        public string RoomName { get; set; } = "";
        public string ID { get; set; }

        public int Capacity { get; set; } = 0;

        
        public List<PlayerInfo> OldPlayers { get; set; }
        
        public Dictionary<string,PlayerInfo> Players { get; set; }

        private readonly  Object lockObject = new Object();


        public AbstractMatchRule MatchRule { get; set; } = null;

        public AbstractMatchSetting setting = null;

        public WaitRoom(in string name,in string id,int capacity=8)
        {
            RoomName = name;

            ID = id;

            Capacity=capacity;

        }

        public void ChangeGameMode(eGameMode mode)
        {

        }

        public void AddPlayer(PlayerInfo info)
        {
            lock (lockObject)
            {




            }


        }

        public void AddPlayer(in string id, in string displayName)
        {
            lock (lockObject)
            {
                //var playerInfo = new PlayerInfo(id,displayName);

                //Players.Add(id,);

                var info = new PlayerInfo(id, displayName);


                Players.Add(id,info);


            }

        }

        public void AddPlayers()
        {

        }

        public void AddBotPlayer()
        {

        }


        public void RemovePlayer(in string id)
        {
            lock (lockObject)
            {
                Players.Remove(id);



            }

        }

        public void RemoveAllPlayers()
        {
            lock(lockObject)
            {
                Players.Clear();

            }

        }

        public void RemoveAllBotPlayer()
        {

        }


        public List<PlayerInfo> AllPlayers()
        {
            lock (lockObject)
            {

                var players = new List<PlayerInfo>();


                foreach (var info in Players)
                {
                    players.Add(info.Value);


                }


                return players;
            }
        }


    }
}
