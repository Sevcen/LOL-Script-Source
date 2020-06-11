namespace Aimtec.SDK.Damage.JSON
{
    using System.Collections.Generic;

    /// <summary>
    ///     Class ChampionDamageSpellBonus.
    /// </summary>
    internal class DamageSpellBonus
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the bonus damage on minion.
        /// </summary>
        /// <value>The bonus damage on minion.</value>
        public List<double> BonusDamageOnMinion { get; set; }

        /// <summary>
        ///     Gets or sets the damage percentages.
        /// </summary>
        /// <value>The damage percentages.</value>
        public List<double> DamagePercentages { get; set; }

        /// <summary>
        ///     Gets or sets the type of the damage.
        /// </summary>
        /// <value>The type of the damage.</value>
        public DamageType DamageType { get; set; }

        /// <summary>
        ///     Gets or sets the maximum damage on minion.
        /// </summary>
        /// <value>The maximum damage on minion.</value>
        public List<int> MaxDamageOnMinion { get; set; }

        /// <summary>
        ///     Gets or sets the minimum damage.
        /// </summary>
        /// <value>The minimum damage.</value>
        public List<double> MinDamage { get; set; }

        /// <summary>
        ///     Gets or sets the scale per100 ad.
        /// </summary>
        /// <value>The scale per100 ad.</value>
        public double ScalePer100Ad { get; set; }

        /// <summary>
        ///     Gets or sets the scale per100 ap.
        /// </summary>
        /// <value>The scale per100 ap.</value>
        public double ScalePer100Ap { get; set; }

        /// <summary>
        ///     Gets or sets the scale per100 bonus ad.
        /// </summary>
        /// <value>The scale per100 bonus ad.</value>
        public double ScalePer100BonusAd { get; set; }

        /// <summary>
        ///     Gets or sets the scale per100 bonus ap.
        /// </summary>
        /// <value>The scale per100 bonus ap.</value>
        public double ScalePer100BonusAp { get; set; }

        /// <summary>
        ///     Gets or sets the scale per35 bonus ad.
        /// </summary>
        /// <value>The scale per35 bonus ad.</value>
        public double ScalePer35BonusAd { get; set; }

        /// <summary>
        ///     Gets or sets the scale per crit percent.
        /// </summary>
        /// <value>The scale per crit percent.</value>
        public double ScalePerCritPercent { get; set; }

        /// <summary>
        ///     Gets or sets the scaling buff.
        /// </summary>
        /// <value>The scaling buff.</value>
        public string ScalingBuff { get; set; }

        /// <summary>
        ///     Gets or sets the scaling buff offset.
        /// </summary>
        /// <value>The scaling buff offset.</value>
        public int ScalingBuffOffset { get; set; }

        /// <summary>
        ///     Gets or sets the scaling buff target.
        /// </summary>
        /// <value>The scaling buff target.</value>
        public DamageScalingTarget ScalingBuffTarget { get; set; }

        /// <summary>
        ///     Gets or sets the scaling target.
        /// </summary>
        /// <value>The scaling target.</value>
        public DamageScalingTarget ScalingTarget { get; set; }

        /// <summary>
        ///     Gets or sets the type of the scaling.
        /// </summary>
        /// <value>The type of the scaling.</value>
        public DamageScalingType ScalingType { get; set; }

        #endregion
    }
}