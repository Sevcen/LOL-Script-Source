namespace Aimtec.SDK.Damage.JSON
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     Class ChampionDamage.
    /// </summary>
    internal class ChampionDamage
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the e.
        /// </summary>
        /// <value>The e.</value>
        public List<DamageSpell> E { get; set; }

        /// <summary>
        ///     Gets or sets the q.
        /// </summary>
        /// <value>The q.</value>
        public List<DamageSpell> Q { get; set; }

        /// <summary>
        ///     Gets or sets the r.
        /// </summary>
        /// <value>The r.</value>
        public List<DamageSpell> R { get; set; }

        /// <summary>
        ///     Gets or sets the w.
        /// </summary>
        /// <value>The w.</value>
        public List<DamageSpell> W { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <returns>IEnumerable&lt;ChampionDamageSpell&gt;.</returns>
        /// <exception cref="ArgumentOutOfRangeException">slot - null</exception>
        public IEnumerable<DamageSpell> GetSlot(SpellSlot slot)
        {
            switch (slot)
            {
                case SpellSlot.Q: return this.Q;
                case SpellSlot.W: return this.W;
                case SpellSlot.E: return this.E;
                case SpellSlot.R: return this.R;
                default: throw new ArgumentOutOfRangeException(nameof(slot), slot, null);
            }
        }

        #endregion
    }
}