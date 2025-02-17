using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public class Chat
    {
        private string id = Guid.NewGuid().ToString("N");
        private string playerName = "";
        private string message = "";

        public string Id { get => id; set => id = value; }
        public Chat(in string playerName, in string message)
        {
            //playerName = playerName;

        }



    }
}
