#nullable enable
using System;
using System.Collections.Generic;

namespace OpenGSCore
{
    public interface IMatchResult
    {
        IReadOnlyList<PlayerID> Winners { get; }
        IReadOnlyList<PlayerID> Losers { get; }
    }

    /// <summary>
    /// 単一のジェネリック試合結果クラス。
    /// 従来の5つのサブクラス (DeathMatchResult, TeamDeathMatchResult, SuvMatchResult, TSuvMatchResult, CTFMatchResult) を統合。
    /// </summary>
    public sealed class MatchResult<TFinalScore> : IMatchResult where TFinalScore : class
    {
        private readonly List<PlayerID> _winners = new();
        private readonly List<PlayerID> _losers = new();
        private readonly TFinalScore _finalScore;

        public TFinalScore FinalScore => _finalScore;
        public IReadOnlyList<PlayerID> Winners => _winners.AsReadOnly();
        public IReadOnlyList<PlayerID> Losers => _losers.AsReadOnly();

        public MatchResult(TFinalScore? finalScore)
        {
            _finalScore = finalScore ?? throw new ArgumentNullException(nameof(finalScore));
        }

        public void SetWinners(IEnumerable<PlayerID>? playerIds)
        {
            _winners.Clear();
            if (playerIds != null)
            {
                foreach (var id in playerIds)
                {
                    if (id != null) _winners.Add(id);
                }
            }
        }

        public void SetLosers(IEnumerable<PlayerID>? playerIds)
        {
            _losers.Clear();
            if (playerIds != null)
            {
                foreach (var id in playerIds)
                {
                    if (id != null) _losers.Add(id);
                }
            }
        }
    }

    public static class MatchResultFactory2
    {
        public static MatchResult<TFinalScore> Create<TFinalScore>(TFinalScore? finalScore)
            where TFinalScore : class
        {
            return new MatchResult<TFinalScore>(finalScore);
        }
    }
}
