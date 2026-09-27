using System;
using System.Collections.Generic;

namespace OpenGSCore
{
    /// <summary>
    /// The single place where a field item type becomes a wire name and back.
    /// <para>
    /// The server used to carry item types as free form strings, so a typo in
    /// a spawn rule silently produced an item nothing could pick up, and the
    /// client carried its own enum with the same values under different names.
    /// Both sides now go through EFieldItemType and convert only at the JSON
    /// boundary, so the wire name stays stable while the code stays typed.
    /// </para>
    /// <para>
    /// Two names are accepted on the way in because the client and the server
    /// did not agree historically: "PowerUp" and "PowerUpItem" both mean
    /// PowerUpItem, and likewise for the other timed items. New writes always
    /// use the canonical EFieldItemType name.
    /// </para>
    /// </summary>
    public static class FieldItemTypeNames
    {
        private static readonly Dictionary<string, EFieldItemType> Aliases =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["PowerUp"] = EFieldItemType.PowerUpItem,
                ["DefenceUp"] = EFieldItemType.DefenceUpItem,
                ["SpeedUp"] = EFieldItemType.SpeedUpItem,
                ["Stealth"] = EFieldItemType.StealthItem,
                ["NormalGrenadePack"] = EFieldItemType.GrenadePack,
                ["RocketLauncher"] = EFieldItemType.GranadeLauncher
            };

        /// <summary>
        /// Every item type the game knows about, in enum order.
        /// </summary>
        public static IReadOnlyList<EFieldItemType> All { get; } =
            (EFieldItemType[])Enum.GetValues(typeof(EFieldItemType));

        /// <summary>
        /// The canonical name written to the wire.
        /// </summary>
        public static string ToWireName(EFieldItemType type)
        {
            return type.ToString();
        }

        /// <summary>
        /// Parses a wire name. Returns false for anything unknown so a caller
        /// can reject the item instead of spawning a type nothing can use.
        /// </summary>
        public static bool TryParse(string? name, out EFieldItemType type)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                type = default;
                return false;
            }

            if (Enum.TryParse(name, ignoreCase: true, out type) &&
                Enum.IsDefined(typeof(EFieldItemType), type))
            {
                return true;
            }

            return Aliases.TryGetValue(name.Trim(), out type);
        }

        /// <summary>
        /// True when the type grants a timed buff rather than a one shot effect.
        /// </summary>
        public static bool IsTimedBuff(EFieldItemType type)
        {
            return type
                is EFieldItemType.PowerUpItem
                    or EFieldItemType.DefenceUpItem
                    or EFieldItemType.SpeedUpItem
                    or EFieldItemType.StealthItem;
        }
    }
}