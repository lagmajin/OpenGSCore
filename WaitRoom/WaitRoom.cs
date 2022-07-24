using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public  class WaitRoom
    {
        public string RoomName { get; set; } = "";
        public string ID { get; set; }

        public int Capacity { get; set; } = 0;

        
        public List<PlayerInfo> OldPlayers { get; set; }
        
        public Dictionary<string,PlayerInfo> Players { get; set; }

        private Object lockObject = new Object();
        public WaitRoom(in string name,in string id,int capacity)
        {
            RoomName = name;

            ID = id;

            Capacity=capacity;

        }

        public void AddPlayer(PlayerInfo info)
        {
            lock (lockObject)
            {




            }


        }

        public void AddPlayers()
        {

        }

        public void RemovePlayer(in string id)
        {
            lock (lockObject)
            {

            }

        }

        public void RemoveAllPlayers()
        {

        }



    }
}
