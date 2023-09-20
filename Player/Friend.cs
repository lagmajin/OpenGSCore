using System;
using System.Collections.Generic;
using System.Text;



namespace OpenGSCore
{
    public interface IFriend
    {

    }


    public class Friend
    {
        public string lv="0";
        public string accountID="";

        public string displayName="";

        public Friend()
        {

        }

        public Friend(string lv, string accountID, string displayName)
        {
            
            this.lv = lv;
                
            this.accountID = accountID;
            this.displayName = displayName;


        }

    }

    public class FriendList
    {

    }

}
