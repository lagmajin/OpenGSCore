using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenGSCore
{
    [Obsolete("Use AbstractMatchRule + IMatchResultEvaluator instead")]
    public interface IMatchLogic
    {
        void StartMatch();
        void EndMatch();
        void AddPlayerToMatch(int matchId, string playerName);
        void RemovePlayerFromMatch(int matchId, string playerName);
        string GetMatchStatus(int matchId);
        void UpdateScore(int matchId, string playerName, int score);
    }

    [Obsolete("Use SuvMatchRule + SurvivalResultEvaluator instead")]
    public class SurvivalMode : IMatchLogic
    {
        public void StartMatch() { /* サバイバルモードのロジック */ }
        public void EndMatch() { /* サバイバルモードのロジック */ }
        public void AddPlayerToMatch(int matchId, string playerName) { /* サバイバルモードのロジック */ }
        public void RemovePlayerFromMatch(int matchId, string playerName) { /* サバイバルモードのロジック */ }
        public string GetMatchStatus(int matchId) { return "Survival Mode"; }
        public void UpdateScore(int matchId, string playerName, int score) { /* サバイバルモードのスコア更新 */ }
    }

    [Obsolete("Use DeathMatchRule + DeathMatchResultEvaluator instead")]
    public class DeathMatchMode : IMatchLogic
    {
        public void StartMatch() { /* デスマッチモードのロジック */ }
        public void EndMatch() { /* デスマッチモードのロジック */ }
        public void AddPlayerToMatch(int matchId, string playerName) { /* デスマッチモードのロジック */ }
        public void RemovePlayerFromMatch(int matchId, string playerName) { /* デスマッチモードのロジック */ }
        public string GetMatchStatus(int matchId) { return "Death Match Mode"; }
        public void UpdateScore(int matchId, string playerName, int score) { /* デスマッチモードのスコア更新 */ }
    }

    [Obsolete("Use TDMMatchRule + TeamDeathMatchResultEvaluator instead")]
    public class TeamDeathMatchMode : IMatchLogic
    {
        public void StartMatch() { /* チームデスマッチモードのロジック */ }
        public void EndMatch() { /* チームデスマッチモードのロジック */ }
        public void AddPlayerToMatch(int matchId, string playerName) { /* チームデスマッチモードのロジック */ }
        public void RemovePlayerFromMatch(int matchId, string playerName) { /* チームデスマッチモードのロジック */ }
        public string GetMatchStatus(int matchId) { return "Team Death Match Mode"; }
        public void UpdateScore(int matchId, string playerName, int score) { /* チームデスマッチモードのスコア更新 */ }
    }

    [Obsolete("Use CaptureTheFlagMatchRule + CaptureTheFlagResultEvaluator instead")]
    public class CaptureTheFlagMode : IMatchLogic
    {
        public void StartMatch() { /* キャプチャー・ザ・フラッグモードのロジック */ }
        public void EndMatch() { /* キャプチャー・ザ・フラッグモードのロジック */ }
        public void AddPlayerToMatch(int matchId, string playerName) { /* キャプチャー・ザ・フラッグモードのロジック */ }
        public void RemovePlayerFromMatch(int matchId, string playerName) { /* キャプチャー・ザ・フラッグモードのロジック */ }
        public string GetMatchStatus(int matchId) { return "Capture The Flag Mode"; }
        public void UpdateScore(int matchId, string playerName, int score) { /* キャプチャー・ザ・フラッグモードのスコア更新 */ }
    }
}
