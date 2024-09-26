



using System;
using System.Collections.Generic;

namespace OpenGSCore
{
    public interface IMultipleKeyDictionary
    {

    }

    public class SearchOption
    {

    }


    public class SearchInfo
    {
        public MultipleKey Key { get; set; }=null;

        
    }

    public class PlayerData
    {
        public PlayerInfo info;
        public PlayerStatus status;
        public PlayerData(PlayerInfo info,PlayerStatus status)
        {

        }

    }


    public class PlayerDatabase
    {
        //private List<Tuple<MultipleKey, PlayerStatus>> player = new();

        private List<string> allIDCache=new();

        //private List<Tuple<MultipleKey, PlayerInfo>> data=new ();

        private List<PlayerData> data = new();


        public PlayerDatabase()
        {

        }

        public void Player(SearchInfo info)
        {



        }

        public PlayerData Player(string id)
        {
            foreach (PlayerData temp in data)
            {
                if(id==temp.info.Id)
                {
                    return temp;
                }

            }


            return null;
        }

        public List<string> AllID()
        {

            return allIDCache;
        }

        public List<PlayerData> AllPlayer()
        {
            return data;
        }

        public void RemoveAll()
        {
            data.Clear ();

        }
    }
}
