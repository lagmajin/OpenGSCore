using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenGSCore
{
    public class GameScene
    {
        private List<AbstractGameObject> objects = new();
        private Dictionary<string, PlayerGameObject> players = new();

        public void AddBullet(BulletGameObject bullet)
        {
            objects.Add(bullet);
        }

        public void AddPlayerCharacter(PlayerGameObject character)
        {
            if (!players.ContainsKey(character.Id))
            {
                players[character.Id] = character;
                objects.Add(character);
            }
        }

        public void AddFieldItem(AbstractFieldItem item)
        {
            objects.Add(item);
        }

        public void AddGrenade()
        {

        }

        public void AddFlag()
        {


        }

        public void RemovePlayer(string playerId)
        {
            if (players.TryGetValue(playerId, out var player))
            {
                objects.Remove(player);
                players.Remove(playerId);
            }
        }

        public void UpdateFrame()
        {
            foreach (var obj in objects)
            {
                obj.Update();
            }
        }



        public JObject ToJson()
        {
            var json = new JObject();
            var playersJson = new JArray();
            var bulletsJson = new JArray();
            var itemsJson = new JArray();

            foreach (var obj in objects)
            {
                var temp = new JObject
                {
                    ["Name"] = obj.Name,
                    ["ID"] = obj.Id,
                    ["PosX"] = obj.Posx,
                    ["PosY"] = obj.Posy
                };

                if (obj is PlayerGameObject) playersJson.Add(temp);
                else if (obj is BulletGameObject) bulletsJson.Add(temp);
                else if (obj is AbstractFieldItem) itemsJson.Add(temp);
            }

            json["Players"] = playersJson;
            json["Bullets"] = bulletsJson;
            json["Items"] = itemsJson;
            return json;
        }

        public JObject AllPlayerDataToJson()
        {
            var json = new JObject();
            foreach (var obj in objects)
            {
                if (obj is PlayerGameObject player)
                {
                    json[player.Id] = new JObject
                    {
                        ["Name"] = player.Name,
                        ["PosX"] = player.Posx,
                        ["PosY"] = player.Posy
                    };
                }
            }
            return json;
        }



    }
}
