using System;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace OpenGSCore
{
    /// <summary>
    /// ArmsRaceの勝敗判定ロジック
    /// キルで武器レベルが昇格し、最高レベル到達または最多キルで勝利。
    /// Score を武器レベルとして扱う。
    /// </summary>
    public class ArmsRaceResultEvaluator : IMatchResultEvaluator
    {
        private const int MaxWeaponLevel = 10;

        public JObject Evaluate(AbstractMatchSituation situation, List<PlayerInfo> players)
        {
            var resultJson = new JObject();
            resultJson["MessageType"] = MessageType.MatchEndNotification;
            resultJson["WinningTeam"] = "None";

            var safePlayers = players ?? new List<PlayerInfo>();
            if (!safePlayers.Any())
            {
                resultJson["Winner"] = "NoPlayers";
                resultJson["Players"] = new JArray();
                return resultJson;
            }

            int topLevel = safePlayers.Max(p => Math.Min(p?.Score ?? 0, MaxWeaponLevel));
            var topPlayers = safePlayers.Where(p => Math.Min(p?.Score ?? 0, MaxWeaponLevel) == topLevel).ToList();

            string winnerId;
            string winnerName;
            if (topPlayers.Count == 1)
            {
                winnerId = topPlayers[0].Id;
                winnerName = topPlayers[0].Name;
            }
            else
            {
                var tieBreaker = topPlayers.OrderByDescending(p => p.Kills).ThenBy(p => p.Deaths).First();
                winnerId = tieBreaker.Id;
                winnerName = tieBreaker.Name;
            }

            resultJson["Winner"] = winnerId;
            resultJson["WinningPlayerId"] = winnerId;
            resultJson["WinnerName"] = winnerName;
            resultJson["TopWeaponLevel"] = topLevel;
            resultJson["MaxWeaponLevel"] = MaxWeaponLevel;

            var playersArray = new JArray();
            foreach (var p in safePlayers.OrderByDescending(p => p.Score).ThenByDescending(p => p.Kills))
            {
                var entry = p.ToJson();
                entry["WeaponLevel"] = Math.Min(p.Score, MaxWeaponLevel);
                playersArray.Add(entry);
            }
            resultJson["Players"] = playersArray;

            return resultJson;
        }
    }
}
