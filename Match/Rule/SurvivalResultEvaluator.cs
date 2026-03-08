using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace OpenGSCore
{
    /// <summary>
    /// サバイバル（個人戦）の勝敗判定ロジック
    /// </summary>
    public class SurvivalResultEvaluator : IMatchResultEvaluator
    {
        public JObject Evaluate(AbstractMatchSituation situation, List<PlayerInfo> players)
        {
            var resultJson = new JObject();
            resultJson["MessageType"] = "MatchResult";

            // サポートされている場合、生存者を勝者とする
            // ここでは簡易的に、キル数が最も多い（またはデスしていない）プレイヤーを上位に表示
            var sortedPlayers = players
                .OrderByDescending(p => p.Health > 0) // 生存優先
                .ThenByDescending(p => p.Kills)     // 次にキル数
                .ThenBy(p => p.Deaths)            // 次にデス数の少なさ
                .ToList();

            string winnerId = sortedPlayers.Count > 0 ? sortedPlayers[0].Id : "None";
            string winnerName = sortedPlayers.Count > 0 ? sortedPlayers[0].Name : "None";

            resultJson["WinningPlayerId"] = winnerId;
            resultJson["WinnerName"] = winnerName;
            resultJson["WinningTeam"] = "None"; // 個人戦なので

            var playersArray = new JArray();
            foreach (var p in sortedPlayers)
            {
                playersArray.Add(p.ToJson());
            }
            resultJson["Players"] = playersArray;

            return resultJson;
        }
    }
}
