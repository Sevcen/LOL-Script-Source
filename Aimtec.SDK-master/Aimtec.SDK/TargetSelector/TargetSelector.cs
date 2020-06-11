namespace Aimtec.SDK.TargetSelector
{
    using System.Collections.Generic;

    using Aimtec.SDK.Menu;
    using Aimtec.SDK.Menu.Config;

    using NLog;
    using NLog.Fluent;

    /// <summary>
    ///     Target Selector Class
    /// </summary>
    public class TargetSelector
    {
        #region Static Fields

        private static ITargetSelector impl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetSelector" /> class.
        /// </summary>
        /// <param name="impl">The implementation.</param>
        public TargetSelector(ITargetSelector impl)
        {
            Implementation = impl;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the implementation of the target selector.
        /// </summary>
        /// <value>
        ///     The implementation of the target selector.
        /// </value>
        public static ITargetSelector Implementation
        {
            get => impl;
            set
            {
                if (impl == null)
                {
                    Logger.Info("Target Selector Implementation is being initialized.");
                }

                else
                {
                    Logger.Info("Target Selector Implementation is being changed.");
                    Dispose();
                }

                impl = value;

                LoadMenu();
            }
        }

        #endregion

        #region Properties

        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        private static Menu TargetSelectorMenu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disposes the current instance
        /// </summary>
        public static void Dispose()
        {
            Logger.Info("Disposing the old Implementation");

            if (TargetSelectorMenu != null)
            {
                TargetSelectorMenu.Dispose();
            }

            if (Implementation != null)
            {
                Implementation.Dispose();
            }
        }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="autoAttackTarget">If true then range is ignored and max auto attack range is used</param>
        /// <returns></returns>
        public static Obj_AI_Hero GetTarget(float range, bool autoAttackTarget = false)
        {
            return Implementation.GetTarget(range, autoAttackTarget);
        }

        /// <summary>
        ///     Gets an ordered enumerable of the available targets within the range based on their priority ranking by the Target
        ///     Selector
        /// </summary>
        public static IEnumerable<Obj_AI_Hero> GetOrderedTargets(float range, bool autoAttackTarget = false)
        {
            return Implementation.GetOrderedTargets(range, autoAttackTarget);
        }

        /// <summary>
        ///     Gets the selected target
        /// </summary>
        public static Obj_AI_Hero GetSelectedTarget()
        {
            return Implementation.GetSelectedTarget();
        }

        #endregion

        #region Methods

        internal static void Load()
        {
            Log.Info().Message("Loading Default Target Selector").Write();
            Implementation = new TargetSelectorImpl();
        }

        internal static void LoadMenu()
        {
            Logger.Info("Loading the Target Selector Menu");

            TargetSelectorMenu = impl.Config;

            AimtecMenu.Instance.Add(TargetSelectorMenu);
        }

        #endregion
    }
}