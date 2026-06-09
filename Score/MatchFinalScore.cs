using System.Collections.Generic;

namespace OpenGSCore
{
    /// <summary>
    /// 試合結果のスコアデータ。全プレイヤーの最終成績を保持。
    /// AbstractFinalScore と AbstractMatchFinalScore を統合。
    /// </summary>
    public class MatchFinalScore
    {
        public EGameMode Mode { get; }
        public List<PlayerFinalScore> PlayerScores { get; } = new();

        public MatchFinalScore(EGameMode mode)
        {
            Mode = mode;
        }

        public void AddPlayerScore(PlayerFinalScore score)
        {
            if (score != null) PlayerScores.Add(score);
        }
    }

    /// <summary>
    /// 1プレイヤー分の最終成績。
    /// AbstractPlayerMatchFinalScore + 空サブクラス3つを統合。
    /// </summary>
    public class PlayerFinalScore
    {
        public string PlayerName { get; set; } = string.Empty;
        public string PlayerId { get; set; } = string.Empty;
        public int Kill { get; set; }
        public int Death { get; set; }
        public int Suicide { get; set; }
        public int? Rank { get; set; }
        public float TotalPoint { get; set; }
        public EGameMode Mode { get; set; } = EGameMode.Unknown;

        public float CalcTotalPoint()
        {
            TotalPoint = Kill * 100f - Death * 50f - Suicide * 100f;
            return TotalPoint;
        }
    }
}
