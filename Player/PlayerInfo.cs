using Newtonsoft.Json.Linq;
using System.Collections.Generic;
//using System.Text.Json;
//using System.Text.Json.Serialization;

namespace OpenGSCore
{
    public class PlayerInfo
    {
        string id;

        string name;

        string ip;

        List<string> ipList;
        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string CurrentIp { get => ip; set => ip = value; }

        public PlayerInfo(in string id,in string name,in string currentIp)
        {
            Id = id;

            Name = name;
            CurrentIp = currentIp;

        }


        public JObject ToJson()
        {
            var result= new JObject();

            result["Id"]=id;

            result["Name"] = name;


            return result;
        }

    }
}
