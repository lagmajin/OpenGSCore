using System;
using System.Collections.Generic;
using System.Text;

namespace OpenGSCore
{
    public partial class MatchRoom
    {
        private readonly HashSet<string> _readyPlayers = new();

        public void StartLoading()
        {
            lock (playerSyncLock)
            {
                _readyPlayers.Clear();
            }
            eventBus.PublishLoadingStart();
        }

        public void SetPlayerReady(string playerId)
        {
            bool shouldStart = false;

            lock (playerSyncLock)
            {
                if (!Players.Exists(p => p.Id == playerId))
                {
                    return;
                }

                _readyPlayers.Add(playerId);
                shouldStart = _readyPlayers.Count >= Players.Count && Players.Count > 0 && !Playing;
            }

            if (shouldStart)
            {
                GameStart();
            }
        }
    }


}
