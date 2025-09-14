using Newtonsoft.Json.Linq;
using System.Collections.Generic;
//using System.Text.Json;
//using System.Text.Json.Serialization;

//using NodaTime;

namespace OpenGSCore
{
    public class PlayerInfoLite
    {
        public string Id { get; }
        public string Name { get; }
        public string? CurrentIp { get; }



        public PlayerInfoLite(string id, string name, string? currentIp = null)
        {
            Id = id;
            Name = name;
            CurrentIp = currentIp;
        }

        public JObject ToJson()
        {

            return null;          
        }

    }

    //#PlayerInfo
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

        public EPlayerCharacter playerCharacter { get; set; }
        public PlayerInfo(string? id = null,string name = null, in string? currentIp = null)
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
            result["CurrentIP"] = CurrentIp;


            return result;
        }

    }




}
