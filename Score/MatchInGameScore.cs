namespace OpenGSCore
{
    /// <summary>
    /// 試合中のライブスコア追跡。
    /// </summary>
    public sealed class MatchInGameScore
    {
        public int Kill { get; private set; }
        public int Death { get; private set; }
        public int Suicide { get; private set; }
        public int TotalDamage { get; private set; }
        public int FlagCarrierKill { get; private set; }
        public int RecoverFlag { get; private set; }

        public void AddKill(int value = 1)
        {
            if (value > 0) Kill += value;
        }

        public void AddDeath(int value = 1)
        {
            if (value > 0) Death += value;
        }

        public void AddSuicide()
        {
            Suicide++;
            if (Kill > 0) Kill--;
        }

        public void AddFlagCarrierKill()
        {
            FlagCarrierKill++;
        }

        public void AddTotalDamage(int damage)
        {
            if (damage > 0) TotalDamage += damage;
        }
    }
}
