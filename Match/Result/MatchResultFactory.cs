using OpenGSCore;
using OpenGSCore.Score;
using System;

namespace OpenGSServer
{
    public class MatchResultFactory
    {
        public static AbstractMatchResult CreateMatchResult(AbstractFinalScore score)
        {
            if (score == null)
            {
                throw new ArgumentNullException(nameof(score));
            }

            return score.mode switch
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
                _ => throw new NotSupportedException($"Unsupported game mode: {score.mode}")
            };
        }
    }
}
