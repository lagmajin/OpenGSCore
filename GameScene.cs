#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace OpenGSCore
{
    public class GameScene
    {
        private JObject? _lastSnapshot = null;
        private readonly List<AbstractGameObject> _objects = new();
        private readonly Dictionary<string, AbstractGameObject> _objectIndex = new(StringComparer.Ordinal);
        private readonly object _syncRoot = new();

        public int ObjectCount
        {
            get
            {
                lock (_syncRoot)
                {
                    return _objects.Count;
                }
            }
        }

        public bool AddBullet(BulletGameObject bullet) => AddObject(bullet);

        public bool AddPlayerCharacter(PlayerGameObject character) => AddObject(character);

        public bool AddFieldItem(AbstractFieldItem item) => AddObject(item);

        public bool AddGrenade(AbstractGameObject grenade) => AddObject(grenade);

        public bool AddFlag(AbstractGameObject flag) => AddObject(flag);

        public bool RemoveObject(string objectId)
        {
            if (string.IsNullOrWhiteSpace(objectId)) return false;

            lock (_syncRoot)
            {
                if (!_objectIndex.TryGetValue(objectId, out var obj)) return false;

                _objects.Remove(obj);
                _objectIndex.Remove(objectId);
                return true;
            }
        }

        public AbstractGameObject? FindObject(string objectId)
        {
            if (string.IsNullOrWhiteSpace(objectId))
            {
                Console.WriteLine("[GameScene] FindObject called with empty objectId.");
                return null;
            }
            lock (_syncRoot)
            {
                return _objectIndex.TryGetValue(objectId, out var obj) ? obj : null;
            }
        }

        public void UpdatePlayerPosition(string playerId, float posX, float posY)
        {
            var player = FindObject(playerId) as PlayerGameObject ?? FindPlayerObject(playerId);
            if (player != null)
            {
                player.Posx = posX;
                player.Posy = posY;
            }
        }

        public void UpdateFrame()
        {
            foreach (var obj in Snapshot())
            {
                obj.Update();
            }

            // save sync state after updates
            foreach (var obj in Snapshot())
            {
                obj.SaveSyncState();
            }
        }

        // Deterministic tick entry point
        public void FixedUpdate() => UpdateFrame();

        public JObject ToJson()
        {
            var array = new JArray();
            foreach (var item in Snapshot())
            {
                array.Add(CreateObjectJson(item));
            }

            return new JObject
            {
                ["Objects"] = array
            };
        }

        public JObject GetSnapshot()
        {
            var snap = ToJson();
            _lastSnapshot = snap.DeepClone() as JObject;
            return snap;
        }

        public JObject? GetSnapshotDelta()
        {
            var current = ToJson();
            if (_lastSnapshot == null)
            {
                _lastSnapshot = current.DeepClone() as JObject;
                return current;
            }

            var lastCount = (_lastSnapshot["Objects"] as JArray)?.Count ?? 0;
            var curCount = (current["Objects"] as JArray)?.Count ?? 0;
            if (lastCount != curCount)
            {
                _lastSnapshot = current.DeepClone() as JObject;
                return current;
            }

            Console.WriteLine("[GameScene] No snapshot delta available.");
            return null;
        }

        public JObject AllPlayerDataToJson()
        {
            var players = new JArray();
            foreach (var player in Snapshot().OfType<PlayerGameObject>())
            {
                players.Add(CreateObjectJson(player));
            }

            return new JObject
            {
                ["Players"] = players
            };
        }

        public void ApplySnapshot(JObject snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            var objectTokens = snapshot["Objects"] as JArray ?? snapshot["Players"] as JArray ?? new JArray();
            var rebuiltObjects = new List<AbstractGameObject>();

            foreach (var token in objectTokens.OfType<JObject>())
            {
                var obj = CreateObjectFromJson(token);
                if (obj != null)
                {
                    rebuiltObjects.Add(obj);
                }
            }

            lock (_syncRoot)
            {
                _objects.Clear();
                _objectIndex.Clear();

                foreach (var obj in rebuiltObjects)
                {
                    if (string.IsNullOrWhiteSpace(obj.Id))
                    {
                        obj.Id = Guid.NewGuid().ToString("N");
                    }

                    _objects.Add(obj);
                    _objectIndex[obj.Id] = obj;
                    obj.SaveSyncState();
                }
            }

            _lastSnapshot = snapshot.DeepClone() as JObject;
        }

        private bool AddObject(AbstractGameObject gameObject)
        {
            if (gameObject == null) return false;

            lock (_syncRoot)
            {
                if (string.IsNullOrWhiteSpace(gameObject.Id)) gameObject.Id = Guid.NewGuid().ToString("N");
                if (_objectIndex.ContainsKey(gameObject.Id)) return false;
                _objects.Add(gameObject);
                _objectIndex.Add(gameObject.Id, gameObject);
            }

            // initialize sync state
            gameObject.SaveSyncState();
            return true;
        }

        private List<AbstractGameObject> Snapshot()
        {
            lock (_syncRoot)
            {
                return _objects.ToList();
            }
        }

        private PlayerGameObject? FindPlayerObject(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
            {
                return null;
            }

            lock (_syncRoot)
            {
                return _objects.OfType<PlayerGameObject>()
                    .FirstOrDefault(player => string.Equals(player.PlayerId, playerId, StringComparison.OrdinalIgnoreCase));
            }
        }

        private static AbstractGameObject? CreateObjectFromJson(JObject json)
        {
            if (json == null)
            {
                return null;
            }

            var typeText = json["type"]?.ToString() ?? json["Type"]?.ToString() ?? string.Empty;
            Enum.TryParse(typeText, true, out eGameObjectType objectType);

            var id = json["id"]?.ToString() ?? json["ID"]?.ToString() ?? json["objectId"]?.ToString() ?? string.Empty;
            var name = json["name"]?.ToString() ?? json["Name"]?.ToString() ?? string.Empty;
            var posX = json["posx"]?.ToObject<float?>() ?? json["PosX"]?.ToObject<float?>() ?? 0f;
            var posY = json["posy"]?.ToObject<float?>() ?? json["PosY"]?.ToObject<float?>() ?? 0f;

            AbstractGameObject obj = objectType switch
            {
                eGameObjectType.Bullet => new BulletGameObject(posX, posY, json["damage"]?.ToObject<float?>() ?? json["Damage"]?.ToObject<float?>() ?? 0f),
                eGameObjectType.FieldItem => new FieldItem(posX, posY),
                eGameObjectType.Grenade => new NormalGranade(posX, posY, json["velocityX"]?.ToObject<float?>() ?? json["VelocityX"]?.ToObject<float?>() ?? 0f, json["velocityY"]?.ToObject<float?>() ?? json["VelocityY"]?.ToObject<float?>() ?? 0f),
                _ => json["playerId"] != null || json["PlayerId"] != null
                    ? new PlayerGameObject(json["playerId"]?.ToString() ?? json["PlayerId"]?.ToString() ?? id, name, posX, posY)
                    : new Character(name, posX, posY)
            };

            obj.Id = string.IsNullOrWhiteSpace(id) ? obj.Id : id;
            obj.Name = string.IsNullOrWhiteSpace(name) ? obj.Name : name;
            obj.Posx = posX;
            obj.Posy = posY;
            ApplyObjectState(obj, json);
            return obj;
        }

        private static void ApplyObjectState(AbstractGameObject obj, JObject json)
        {
            if (obj == null || json == null)
            {
                return;
            }

            if (obj is PlayerGameObject player)
            {
                player.PlayerId = json["playerId"]?.ToString() ?? json["PlayerId"]?.ToString() ?? player.PlayerId;

                var teamText = json["team"]?.ToString() ?? json["Team"]?.ToString();
                Enum.TryParse(teamText, true, out ETeam team);
                var maxHp = json["maxHp"]?.ToObject<float?>() ?? json["MaxHp"]?.ToObject<float?>() ?? 500f;
                var maxBooster = json["maxBooster"]?.ToObject<float?>() ?? json["MaxBooster"]?.ToObject<float?>() ?? 100f;
                var status = new PlayerStatus(team, EPlayerType.OtherPlayer, Math.Max(1, (int)maxHp), maxBooster)
                {
                    Hp = json["hp"]?.ToObject<float?>() ?? json["Hp"]?.ToObject<float?>() ?? maxHp,
                    Booster = json["booster"]?.ToObject<float?>() ?? json["Booster"]?.ToObject<float?>() ?? maxBooster,
                    AttackPower = json["attackPower"]?.ToObject<int?>() ?? json["AttackPower"]?.ToObject<int?>() ?? 10,
                    DefensePower = json["defensePower"]?.ToObject<int?>() ?? json["DefensePower"]?.ToObject<int?>() ?? 5
                };
                player.Status = status;
            }
            else if (obj is NormalGranade grenade)
            {
                grenade.VelocityX = json["velocityX"]?.ToObject<float?>() ?? json["VelocityX"]?.ToObject<float?>() ?? grenade.VelocityX;
                grenade.VelocityY = json["velocityY"]?.ToObject<float?>() ?? json["VelocityY"]?.ToObject<float?>() ?? grenade.VelocityY;
                grenade.LifeTime = json["lifeTime"]?.ToObject<float?>() ?? json["LifeTime"]?.ToObject<float?>() ?? grenade.LifeTime;
            }
        }

        private static JObject CreateObjectJson(AbstractGameObject item)
        {
            if (item == null) return new JObject();
            return item.ToJSon() ?? new JObject
            {
                ["Name"] = item.Name ?? string.Empty,
                ["ID"] = item.Id ?? string.Empty,
                ["PosX"] = item.Posx,
                ["PosY"] = item.Posy
            };
        }
    }
}

