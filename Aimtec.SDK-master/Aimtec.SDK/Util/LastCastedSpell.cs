namespace Aimtec.SDK.Util
{
    using System.Collections.Generic;

    /// <summary>
    ///     Class LastCastedSpell.
    /// </summary>
    public static class LastCastedSpell
    {
        #region Constructors and Destructors

        static LastCastedSpell()
        {
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
        }

        #endregion

        #region Properties

        private static Dictionary<int, SpellData> UnitLastCastedSpell { get; } = new Dictionary<int, SpellData>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the name of the <paramref name="hero" /> last casted spell.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns>The name of the <paramref name="hero" /> last casted spell.</returns>
        public static string GetLastCastedSpell(this Obj_AI_Hero hero)
        {
            return UnitLastCastedSpell.TryGetValue(hero.NetworkId, out var data) ? data.Name : string.Empty;
        }

        /// <summary>
        ///     Gets the tick count, accounting for ping, of the <paramref name="hero" /> last casted spell.
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <returns>The tick count, accounting for ping, of the <paramref name="hero" /> last casted spell.</returns>
        public static int GetLastCastedSpellTime(this Obj_AI_Hero hero)
        {
            return UnitLastCastedSpell.TryGetValue(hero.NetworkId, out var data) ? data.Time : 0;
        }

        #endregion

        #region Methods

        private static void OnProcessSpellCast(Obj_AI_Base sender, Obj_AI_BaseMissileClientDataEventArgs e)
        {
            if (sender.Type != GameObjectType.obj_AI_Hero)
            {
                return;
            }

            if (!UnitLastCastedSpell.TryGetValue(sender.NetworkId, out var data))
            {
                data = new SpellData();
            }

            data.Name = e.SpellData.Name;
            data.Time = Game.TickCount - Game.Ping / 2;

            UnitLastCastedSpell[sender.NetworkId] = data;
        }

        #endregion

        class SpellData
        {
            #region Public Properties

            public string Name { get; set; }

            public int Time { get; set; }

            #endregion
        }
    }
}