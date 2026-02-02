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
            if (string.IsNullOrWhiteSpace(objectId)) return null;
            lock (_syncRoot)
            {
                return _objectIndex.TryGetValue(objectId, out var obj) ? obj : null;
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
