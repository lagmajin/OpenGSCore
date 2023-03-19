using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace OpenGSCore
{


    public class WaitRoomDatabase
    {
        private List<WaitRoom> rooms=new();

        //private Dictionary<int, WaitRoom> rooms2 = new();


        public void AddRoom(WaitRoom room)
        {

        }
        public List<WaitRoom> AllRooms()
        {
            return rooms;
        }

        

        public List<WaitRoom> SearchRoomByGameMode(eGameMode mode)
        {
            var list = new List<WaitRoom>();

            foreach (var room in rooms)
            {
                //if(room.MatchRule.)
                
            }

            return list;
        }

        public List<WaitRoom> SearchRoomByRoomName(in string roomName)
        {
            var list = new List<WaitRoom>();
            return list;
        }

        public List<WaitRoom> SearchRoomByRoomNumber(int roomNumber)
        {

            return null;
        }

        public void RemoveAllRooms()
        {
            //rooms.RemoveAll()
        }


    }


    public class MatchRoomDatabase
    {
        private Dictionary<int, WaitRoom> rooms2 = new();
        public void AddRoom()
        {

        }

        public void RemoveAllRooms()
        {

        }

    }

}
