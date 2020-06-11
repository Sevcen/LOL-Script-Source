namespace Aimtec.SDK.Damage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Aimtec.SDK.Damage.JSON;
    using Aimtec.SDK.Extensions;

    using Newtonsoft.Json;

    using NLog;

    /// <summary>
    ///     Class DamageLibrary.
    /// </summary>
    internal static class DamageLibrary
    {
        #region Public Properties

        /// <summary>
        ///     Gets the damages.
        /// </summary>
        /// <value>The damages.</value>
        public static IReadOnlyDictionary<string, ChampionDamage> Damages { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the bonus spell damage.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="spellBonus">The spell bonus.</param>
        /// <param name="index">The index.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        internal static double GetBonusSpellDamage(
            this Obj_AI_Base source,
            Obj_AI_Base target,
            DamageSpellBonus spellBonus,
            int index)
        {
            var sourceScale = spellBonus.ScalingTarget == DamageScalingTarget.Source ? source : target;
            var percent = spellBonus.DamagePercentages?.Count > 0
                ? spellBonus.DamagePercentages[Math.Min(index, spellBonus.DamagePercentages.Count - 1)]
                : 0d;
            float origin;

            switch (spellBonus.ScalingType)
            {
                case DamageScalingType.BonusAbilityPoints:
                    origin = sourceScale.FlatMagicDamageMod;
                    break;
                case DamageScalingType.AbilityPoints:
                    origin = sourceScale.TotalAbilityDamage;
                    break;
                case DamageScalingType.BonusAttackPoints:
                    origin = sourceScale.FlatPhysicalDamageMod;
                    break;
                case DamageScalingType.AttackPoints:
                    origin = sourceScale.TotalAttackDamage;
                    break;
                case DamageScalingType.MaxHealth:
                    origin = sourceScale.MaxHealth;
                    break;
                case DamageScalingType.CurrentHealth:
                    origin = sourceScale.Health;
                    break;
                case DamageScalingType.MissingHealth:
                    origin = sourceScale.MaxHealth - sourceScale.Health;
                    break;
                case DamageScalingType.BonusHealth: // TODO: Implement sourceScale.BaseHealth, since Total-Base = Bonus
                    origin = ((Obj_AI_Hero)sourceScale).MaxHealth;
                    break;
                case DamageScalingType.BonusArmor:
                    origin = sourceScale.BonusArmor;
                    break;
                case DamageScalingType.Armor:
                    origin = sourceScale.Armor;
                    break;
                case DamageScalingType.MaxMana:
                    origin = sourceScale.MaxMana;
                    break;
                case DamageScalingType.BonusCriticalDamage:
                    origin = sourceScale.HasItem(ItemId.InfinityEdge) ? 50 : 0; // TODO: Implement sourceScale.BonusCritDamage or sourceScale.CritDamageMod
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            var dmg = origin * (percent > 0 || percent < 0
                ? (percent > 0 ? percent : 0)
                + (spellBonus.ScalePer100BonusAp > 0
                    ? Math.Abs(source.FlatMagicDamageMod / 100) * spellBonus.ScalePer100BonusAp
                    : 0)
                + (spellBonus.ScalePer100Ap > 0
                    ? Math.Abs(source.TotalAbilityDamage / 100) * spellBonus.ScalePer100Ap
                    : 0)
                + (spellBonus.ScalePer35BonusAd > 0
                    ? Math.Abs(source.FlatPhysicalDamageMod / 35) * spellBonus.ScalePer35BonusAd
                    : 0)
                + (spellBonus.ScalePer100BonusAd > 0
                    ? Math.Abs(source.FlatPhysicalDamageMod / 100) * spellBonus.ScalePer100BonusAd
                    : 0)
                + (spellBonus.ScalePer100Ad > 0
                    ? Math.Abs(source.TotalAttackDamage / 100) * spellBonus.ScalePer100Ad
                    : 0)
                + (spellBonus.ScalePerCritPercent > 0
                    ? Math.Abs(source.Crit * 100) * spellBonus.ScalePerCritPercent
                    : 0)
                : 0);

            if (target is Obj_AI_Minion && spellBonus.BonusDamageOnMinion?.Count > 0)
            {
                dmg += spellBonus.BonusDamageOnMinion[Math.Min(index, spellBonus.BonusDamageOnMinion.Count - 1)];
            }

            if (!string.IsNullOrEmpty(spellBonus.ScalingBuff))
            {
                var buffCount = (spellBonus.ScalingBuffTarget == DamageScalingTarget.Source ? source : target)
                    .GetRealBuffCount(spellBonus.ScalingBuff);
                dmg = buffCount > 0 ? dmg * (buffCount + spellBonus.ScalingBuffOffset) : 0d;
            }

            if (dmg <= 0)
            {
                return dmg;
            }

            if (spellBonus.MinDamage?.Count > 0)
            {
                dmg = Math.Max(dmg, spellBonus.MinDamage[Math.Min(index, spellBonus.MinDamage.Count - 1)]);
            }

            if (target is Obj_AI_Minion && spellBonus.MaxDamageOnMinion?.Count > 0)
            {
                dmg = Math.Min(
                    dmg,
                    spellBonus.MaxDamageOnMinion[Math.Min(index, spellBonus.MaxDamageOnMinion.Count - 1)]);
            }

            return dmg;
        }

        /// <summary>
        ///     Loads the damages.
        /// </summary>
        internal static void LoadDamages()
        {
            Logger.Debug("Embedded Resources: " + string.Join(" | ", Assembly.GetExecutingAssembly().GetManifestResourceNames()));

            try
            {
                // TODO: make this load dynamically based on current running game version.
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Aimtec.SDK.Damage.Data.7.16.json"))
                {
                    if (stream == null)
                    {
                        Logger.Error($"Could not load the damage library. {nameof(stream)} was null.");
                        return;
                    }

                    using (var streamReader = new StreamReader(stream))
                    {
                        Damages =
                            JsonConvert.DeserializeObject<Dictionary<string, ChampionDamage>>(streamReader.ReadToEnd());

                        Logger.Info("Damage Library Loaded");
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Fatal(e, "Could not load damages. Subsequent Damage API calls will return 0.");

                // Create empty damages to suppress errors
                Damages = new Dictionary<string, ChampionDamage>();
            }
        }

        #endregion
    }
}
