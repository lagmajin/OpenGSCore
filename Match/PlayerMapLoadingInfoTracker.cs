using System;
using System.Collections.Generic;
using System.Linq;
//using System.Reactive;
//using System.Reactive.Linq;
//using System.Reactive.Subjects;
using System.Text;
using UniRx;
using Unit = UniRx.Unit;

namespace OpenGSCore{

    public sealed class MatchLoadingTracker
    {
        private readonly Dictionary<PlayerID, PlayerLoadingInfo> _statuses = new();
        private readonly UniRx.Subject<PlayerLoadingInfo> _progressChanged = new();
        public UniRx.IObservable<PlayerLoadingInfo> ProgressChanged => _progressChanged.AsObservable();

        // 全員完了を流すストリーム
        private readonly Subject<Unit> _allCompleted = new();
        public UniRx.IObservable<Unit> AllCompleted => _allCompleted.AsObservable();

        public void AddPlayer(PlayerID playerId)
        {
            _statuses[playerId] = new PlayerLoadingInfo(playerId);
        }

        public void UpdateProgress(PlayerID playerId, int percent)
        {
            if (_statuses.TryGetValue(playerId, out var status))
            {
                var clamped = Math.Clamp(percent, 0, 100);
                status.LoadingProgress = clamped;

                // 進捗イベント発火
                _progressChanged.OnNext(status);

                // 全員完了したら一度だけ通知
                if (_statuses.Values.All(s => s.IsCompleted))
                {
                    _allCompleted.OnNext(Unit.Default);
                }
            }
        }

        //public bool AllCompleted => _statuses.Values.All(s => s.IsCompleted);

        public IReadOnlyCollection<PlayerLoadingInfo> GetAllStatuses() => _statuses.Values;
    }
}
