namespace Aimtec.SDK.Damage.JSON
{
    using System.Collections.Generic;

    /// <summary>
    ///     The Champion Damage Spell Data class container.
    /// </summary>
    internal class DamageSpellData
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the bonus damage on minion.
        /// </summary>
        /// <value>The bonus damage on minion.</value>
        public List<double> BonusDamageOnMinion { get; set; }

        /// <summary>
        ///     Gets the Bonus Damages.
        /// </summary>
        public List<DamageSpellBonus> BonusDamages { get; set; }

        /// <summary>
        ///     Gets the Main Damages.
        /// </summary>
        public List<double> Damages { get; set; }

        /// <summary>
        ///     Gets or sets the damages per level.
        /// </summary>
        /// <value>The damages per level.</value>
        public List<double> DamagesPerLvl { get; set; }

        /// <summary>
        ///     Gets the Damage Type.
        /// </summary>
        public DamageType DamageType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is apply on hit.
        /// </summary>
        /// <value><c>true</c> if this instance is apply on hit; otherwise, <c>false</c>.</value>
        public bool IsApplyOnHit { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is modified damage.
        /// </summary>
        /// <value><c>true</c> if this instance is modified damage; otherwise, <c>false</c>.</value>
        public bool IsModifiedDamage { get; set; }

        /// <summary>
        ///     Gets or sets the maximum damage on minion.
        /// </summary>
        /// <value>The maximum damage on minion.</value>
        public List<int> MaxDamageOnMinion { get; set; }

        /// <summary>
        ///     Gets or sets the scale per crit percent.
        /// </summary>
        /// <value>The scale per crit percent.</value>
        public double ScalePerCritPercent { get; set; }

        /// <summary>
        ///     Gets or sets the scale per target missing health.
        /// </summary>
        /// <value>The scale per target missing health.</value>
        public double ScalePerTargetMissHealth { get; set; }

        /// <summary>
        ///     Gets the Scaling Slot.
        /// </summary>
        public SpellSlot ScaleSlot { get; set; } = SpellSlot.Unknown;

        /// <summary>
        ///     Gets the Scaling Buff.
        /// </summary>
        public string ScalingBuff { get; set; }

        /// <summary>
        ///     Gets the Scaling Buff Offset.
        /// </summary>
        public int ScalingBuffOffset { get; set; }

        /// <summary>
        ///     Gets the Scaling Buff Target.
        /// </summary>
        public DamageScalingTarget ScalingBuffTarget { get; set; }

        /// <summary>
        ///     Gets or sets the type of the spell effect.
        /// </summary>
        /// <value>The type of the spell effect.</value>
        public SpellEffectType SpellEffectType { get; set; }

        #endregion
    }
}