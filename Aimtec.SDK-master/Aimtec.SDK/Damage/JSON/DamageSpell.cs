namespace Aimtec.SDK.Damage.JSON
{
    /// <summary>
    ///     Class ChampionDamageSpell.
    /// </summary>
    internal class DamageSpell
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the spell data.
        /// </summary>
        /// <value>The spell data.</value>
        public DamageSpellData SpellData { get; set; }

        /// <summary>
        ///     Gets or sets the stage.
        /// </summary>
        /// <value>The stage.</value>
        public DamageStage Stage { get; set; }

        #endregion
    }
}