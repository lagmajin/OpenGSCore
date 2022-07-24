using Newtonsoft.Json.Linq;
using System.Collections.Generic;
//using System.Text.Json;
//using System.Text.Json.Serialization;

namespace OpenGSCore
{
    public class PlayerNetworkStatus
    {

    }


    public class PlayerInfo
    {
        string id;

        string name;

        string ip;

        List<string> ipList;
        public string Id { get; set; }
        public string Name { get; set; }
        public string? CurrentIp { get; set; } = null;

        public PlayerInfo(in string id,in string name,in string? currentIp)
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
