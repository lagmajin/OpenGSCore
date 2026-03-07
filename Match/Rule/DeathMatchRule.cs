


namespace OpenGSCore
{


    public sealed class DeathMatchRule : AbstractMatchRule
    {
        private int killLimit = 20;

        public DeathMatchRule(int killCondition = 20, int matchTimeMsec = 300000) 
            : base(EGameMode.DeathMatch, matchTimeMsec)
        {
            killLimit = killCondition;
        }

        public DeathMatchRule(in DeathMatchSetting setting) 
            : base(EGameMode.DeathMatch, 300000) // デフォルト5分
        {
            // 設定からキル制限を読み取る（将来的には Setting クラスを拡張）
            killLimit = 20;
        }

        public override bool IsMatchFinished(AbstractMatchSituation situation)
        {
            // 時間切れ判定
            if (situation.RemainingTimeSec <= 0) return true;

            // 誰かが規定キル数に到達したか
            if (situation.MaxPlayerKillCount >= killLimit) return true;

            return false;
        }
    }
}
