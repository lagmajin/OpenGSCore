#nullable enable
using System;
using System.Linq;

namespace OpenGSCore
{
    public partial class MatchRoom
    {
        /// <summary>
        /// Applies server-authoritative damage and updates the shared match
        /// counters when the hit eliminates its target.
        /// </summary>
        public bool TryApplyPlayerDamage(
            string attackerId,
            string targetId,
            int baseDamage,
            out int damageApplied,
            out int remainingHealth,
            out bool wasKilled)
        {
            return TryApplyPlayerDamage(attackerId, targetId, baseDamage, false,
                out damageApplied, out remainingHealth, out wasKilled);
        }

        /// <summary>
        /// Applies damage, optionally allowing an eliminated attacker to finish
        /// a projectile that was already in flight.
        /// </summary>
        public bool TryApplyPlayerDamage(
            string attackerId,
            string targetId,
            int baseDamage,
            bool allowDeadAttacker,
            out int damageApplied,
            out int remainingHealth,
            out bool wasKilled)
        {
            damageApplied = 0;
            remainingHealth = 0;
            wasKilled = false;

            if (string.IsNullOrWhiteSpace(attackerId) || string.IsNullOrWhiteSpace(targetId) ||
                string.Equals(attackerId, targetId, StringComparison.OrdinalIgnoreCase) || baseDamage <= 0)
                return false;

            lock (playerSyncLock)
            {
                if (!Playing || Finished || _finishing)
                    return false;

                var attacker = Players.FirstOrDefault(player =>
                    string.Equals(player.Id, attackerId, StringComparison.OrdinalIgnoreCase));
                var target = Players.FirstOrDefault(player =>
                    string.Equals(player.Id, targetId, StringComparison.OrdinalIgnoreCase));
                if (attacker == null || target == null ||
                    (!allowDeadAttacker && attacker.Health <= 0) || target.Health <= 0)
                    return false;

                if (BlocksFriendlyDamage(attacker, target))
                    return false;

                var configuredRate = Setting?.DamageRate ?? 1f;
                var damageRate = !float.IsNaN(configuredRate) && !float.IsInfinity(configuredRate) && configuredRate > 0f
                    ? configuredRate
                    : 1f;
                var scaledDamage = (int)Math.Min(1000d,
                    Math.Max(1d, Math.Ceiling(baseDamage * (double)damageRate)));
                damageApplied = Math.Min(target.Health, scaledDamage);
                target.Health = Math.Max(0, target.Health - damageApplied);
                remainingHealth = target.Health;
                wasKilled = target.Health == 0;

                if (wasKilled)
                    RecordPlayerElimination(target.Id, attacker.Id, 0);

                situation.AlivePlayers = Players.Count(player => player.Health > 0);
                return true;
            }
        }

        private bool BlocksFriendlyDamage(PlayerInfo attacker, PlayerInfo target)
        {
            if (attacker.Team == ETeam.NoTeam || attacker.Team != target.Team)
                return false;

            return Setting?.Mode is EGameMode.TeamDeathMatch or
                EGameMode.TeamSurvival or EGameMode.CaptureTheFlag;
        }

        /// <summary>
        /// Reports whether two current room members are teammates in a mode
        /// that protects teammates from damage.
        /// </summary>
        public bool IsFriendlyDamageBlocked(string attackerId, string targetId)
        {
            if (string.IsNullOrWhiteSpace(attackerId) || string.IsNullOrWhiteSpace(targetId))
                return false;

            lock (playerSyncLock)
            {
                var attacker = Players.FirstOrDefault(player =>
                    string.Equals(player.Id, attackerId, StringComparison.OrdinalIgnoreCase));
                var target = Players.FirstOrDefault(player =>
                    string.Equals(player.Id, targetId, StringComparison.OrdinalIgnoreCase));
                return attacker != null && target != null && BlocksFriendlyDamage(attacker, target);
            }
        }

        /// <summary>
        /// Restores a defeated player's health when the active match rule allows
        /// respawning.
        /// </summary>
        public bool TryRespawnPlayer(string playerId, out int currentHealth, out int maxHealth)
        {
            currentHealth = 0;
            maxHealth = 0;
            if (string.IsNullOrWhiteSpace(playerId)) return false;

            lock (playerSyncLock)
            {
                if (!Playing || Finished || _finishing || rule?.CanReSpawn() != true)
                    return false;

                var player = Players.FirstOrDefault(candidate =>
                    string.Equals(candidate.Id, playerId, StringComparison.OrdinalIgnoreCase));
                if (player == null || player.Health > 0)
                    return false;

                maxHealth = Math.Max(player.MaxHealth, 1);
                player.MaxHealth = maxHealth;
                player.Health = maxHealth;
                currentHealth = maxHealth;
                situation.AlivePlayers = Players.Count(candidate => candidate.Health > 0);
                RefreshAliveCounts();
                return true;
            }
        }

        /// <summary>
        /// Restores up to the requested amount without exceeding maximum health.
        /// </summary>
        public bool TryHealPlayer(
            string playerId,
            int requestedHealing,
            out int restoredHealth,
            out int currentHealth,
            out int maxHealth)
        {
            restoredHealth = 0;
            currentHealth = 0;
            maxHealth = 0;
            if (string.IsNullOrWhiteSpace(playerId) || requestedHealing <= 0)
                return false;

            lock (playerSyncLock)
            {
                if (!Playing || Finished || _finishing)
                    return false;

                var player = Players.FirstOrDefault(candidate =>
                    string.Equals(candidate.Id, playerId, StringComparison.OrdinalIgnoreCase));
                if (player == null || player.Health <= 0)
                    return false;

                maxHealth = Math.Max(player.MaxHealth, 1);
                currentHealth = Math.Clamp(player.Health, 0, maxHealth);
                restoredHealth = Math.Min(requestedHealing, maxHealth - currentHealth);
                if (restoredHealth <= 0)
                    return false;

                player.MaxHealth = maxHealth;
                player.Health = currentHealth + restoredHealth;
                currentHealth = player.Health;
                return true;
            }
        }
    }
}
