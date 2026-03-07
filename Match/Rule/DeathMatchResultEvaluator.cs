using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace OpenGSCore
{
    /// <summary>
    /// デスマッチの勝敗判定ロジック
    /// (最高キル数のプレイヤーが勝者)
    /// </summary>
    public class DeathMatchResultEvaluator : IMatchResultEvaluator
    {
        public JObject Evaluate(AbstractMatchSituation situation, List<PlayerInfo> players)
        {
            var resultJson = new JObject();
            resultJson["MessageType"] = MessageType.MatchEndNotification;

            // 各プレイヤーのキル数を集計（ここではPlayerInfoにキル数プロパティがないため仮に0）
            // 実際にはPlayerInfoを拡張するか、マッチ状況から取得する必要があります。
            // 現状では仮のロジックとして、プレイヤーが一人ならそのプレイヤーを勝者とします。
            
            if (!players.Any())
            {
                resultJson["Winner"] = "NoPlayers";
                return resultJson;
            }
            
            // TODO: 実際のキル数に基づく勝者判定ロジックを実装
            // 現状はダミーとして、キル数ではなく参加プレイヤーから適当に選ぶか、Drawとする
            resultJson["Winner"] = "Draw (Kill Count Logic TBD)";

            // 例: 最もキル数が多いプレイヤーを勝者とする場合
            // var topKiller = players.OrderByDescending(p => p.Kills).FirstOrDefault();
            // if (topKiller != null) { resultJson["Winner"] = topKiller.Id; }

            return resultJson;
        }
    }
}
