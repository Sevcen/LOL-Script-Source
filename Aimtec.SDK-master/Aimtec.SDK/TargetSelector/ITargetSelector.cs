namespace Aimtec.SDK.TargetSelector
{
    using System;
    using System.Collections.Generic;

    using Aimtec.SDK.Menu;

    /// <summary>
    ///     The base class for target selector.
    /// </summary>
    public interface ITargetSelector : IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the Target Selector Menu
        /// </summary>
        Menu Config { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="autoAttackTarget">If true then range is ignored and max auto attack range is used</param>
        List<Obj_AI_Hero> GetOrderedTargets(float range, bool autoAttackTarget = false);

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="autoAttackTarget">If true then range is ignored and max auto attack range is used</param>
        Obj_AI_Hero GetTarget(float range, bool autoAttackTarget = false);

        /// <summary>
        ///     Gets the selected target if any.
        /// </summary>
        Obj_AI_Hero GetSelectedTarget();

        #endregion
    }
}