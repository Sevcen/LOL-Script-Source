namespace LeagueSharp.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Automatically levels skills.
    /// </summary>
    public class AutoLevel
    {
        #region Static Fields

        /// <summary>
        ///     The player
        /// </summary>
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        /// <summary>
        ///     The enabled
        /// </summary>
        private static bool enabled;

        /// <summary>
        ///     The initialize
        /// </summary>
        private static bool init;

        /// <summary>
        ///     The last leveled
        /// </summary>
        private static float LastLeveled;

        /// <summary>
        ///     The next delay
        /// </summary>
        private static float NextDelay;

        /// <summary>
        ///     The order
        /// </summary>
        private static List<SpellSlot> order = new List<SpellSlot>();

        /// <summary>
        ///     The random number
        /// </summary>
        private static Random RandomNumber;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoLevel" /> class.
        /// </summary>
        /// <param name="levels">The levels.</param>
        public AutoLevel(IEnumerable<int> levels)
        {
            UpdateSequence(levels);
            Init();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoLevel" /> class.
        /// </summary>
        /// <param name="levels">The levels.</param>
        public AutoLevel(List<SpellSlot> levels)
        {
            UpdateSequence(levels);
            Init();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disables this instance.
        /// </summary>
        public static void Disable()
        {
            enabled = false;
        }

        /// <summary>
        ///     Enables this instance.
        /// </summary>
        public static void Enable()
        {
            enabled = true;
        }

        /// <summary>
        ///     Sets if this instance is enabled or not according to the <paramref name="b" />.
        /// </summary>
        /// <param name="b">if set to <c>true</c> [b].</param>
        public static void Enabled(bool b)
        {
            enabled = b;
        }

        /// <summary>
        ///     Gets the sequence.
        /// </summary>
        /// <returns></returns>
        public static int[] GetSequence()
        {
            return order.Select(spell => (int)spell).ToArray();
        }

        /// <summary>
        ///     Gets the sequence list.
        /// </summary>
        /// <returns></returns>
        public static List<SpellSlot> GetSequenceList()
        {
            return order;
        }

        /// <summary>
        ///     Updates the sequence.
        /// </summary>
        /// <param name="levels">The levels.</param>
        public static void UpdateSequence(IEnumerable<int> levels)
        {
            Init();
            order.Clear();
            foreach (var level in levels)
            {
                order.Add((SpellSlot)level);
            }
        }

        /// <summary>
        ///     Updates the sequence.
        /// </summary>
        /// <param name="levels">The levels.</param>
        public static void UpdateSequence(List<SpellSlot> levels)
        {
            Init();
            order.Clear();
            order = levels;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when the game updates.
        /// </summary>
        /// <param name="args">The <see cref="EventArgs" /> instance containing the event data.</param>
        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (!enabled || Player.SpellTrainingPoints < 1 || Utils.TickCount - LastLeveled < NextDelay
                || MenuGUI.IsShopOpen)
            {
                return;
            }

            NextDelay = RandomNumber.Next(300, 1200);
            LastLeveled = Utils.TickCount;
            var spell = order[GetTotalPoints()];
            Player.Spellbook.LevelSpell(spell);
        }

        /// <summary>
        ///     Gets the total points.
        /// </summary>
        /// <returns></returns>
        private static int GetTotalPoints()
        {
            var spell = Player.Spellbook;
            var q = spell.GetSpell(SpellSlot.Q).Level;
            var w = spell.GetSpell(SpellSlot.W).Level;
            var e = spell.GetSpell(SpellSlot.E).Level;
            var r = spell.GetSpell(SpellSlot.R).Level;

            return q + w + e + r;
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        private static void Init()
        {
            if (init)
            {
                return;
            }

            init = true;
            RandomNumber = new Random(Utils.TickCount);
            Game.OnUpdate += Game_OnGameUpdate;
        }

        #endregion
    }
}