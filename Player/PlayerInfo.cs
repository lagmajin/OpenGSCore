using Newtonsoft.Json.Linq;
using System.Collections.Generic;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//using NodaTime;

namespace OpenGSCore
{



    public class PlayerInfo
    {
        string id;

        string name;

        string ip;

        List<string> ipList;
        public string Id { get; set; }
        public string Name { get; set; }
        public string? CurrentIp { get; set; } = null;

        public int Ping { get; set; } = 0;

        //public ZonedDateTime LastAckTime { get; set; }
        public PlayerInfo(in string? id = null, in string name = null, in string? currentIp = null)
        {
            Id = id;

            Name = name;


            CurrentIp = currentIp;

        }


        public JObject ToJson()
        {
            var result = new JObject();

            result["Id"] = id;

            result["Name"] = name;
            result["CurrentIP"] = "";


            return result;
        }

    }




}
