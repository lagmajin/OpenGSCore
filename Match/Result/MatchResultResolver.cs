using System;

namespace OpenGSCore
{
    /// <summary>
    /// Centralized match-result factory for both legacy and current score models.
    /// </summary>
    public static class MatchResultResolver
    {
        public static AbstractMatchResult Create(AbstractMatchFinalScore score)
        {
            if (score == null)
            {
                throw new ArgumentNullException(nameof(score));
            }

            return score switch
            {
                DeathMatchFinalScore deathMatchScore => new DeathMatchResult(deathMatchScore),
                TeamDeathMatchFinalScore teamDeathMatchScore => new TeamDeathMatchResult(teamDeathMatchScore),
                CTFMatchFinalScore ctfMatchScore => new CTFMatchResult(ctfMatchScore),
                _ => Create(score.Mode)
            };
        }

        public static AbstractMatchResult Create(AbstractFinalScore score)
        {
            if (score == null)
            {
                throw new ArgumentNullException(nameof(score));
            }

            return Create(score.mode);
        }

        public static AbstractMatchResult Create(EGameMode mode)
        {
            return mode switch
            {
                EGameMode.DeathMatch => new DeathMatchResult(),
                EGameMode.OneShotKill => new DeathMatchResult(),
                EGameMode.ArmsRace => new DeathMatchResult(),
                EGameMode.TeamDeathMatch => new TeamDeathMatchResult(),
                EGameMode.CaptureTheFlag => new CTFMatchResult(),
                EGameMode.Survival => new SuvMatchResult(),
                EGameMode.TeamSurvival => new TSuvMatchResult(),
                EGameMode.Practice => new DeathMatchResult(),
                EGameMode.FreeStyle => new DeathMatchResult(),
                EGameMode.Sniper => new DeathMatchResult(),
                EGameMode.TowerMatch => new DeathMatchResult(),
                _ => throw new NotSupportedException($"Unsupported match result mode: {mode}")
            };
        }
    }
}
