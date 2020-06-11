namespace Aimtec.SDK.Prediction.Skillshots
{
    using System;
    using System.Collections.Generic;

    using Aimtec.SDK.Extensions;

    /// <summary>
    ///     Prediction Output, contains the calculated data from the source prediction input.
    /// </summary>
    public class PredictionOutput
    {
        #region Fields

        /// <summary>
        ///     Cast Predicted Position data in a 3D-Space given value.
        /// </summary>
        private Vector3 castPosition;

        /// <summary>
        ///     Unit Predicted Position data ina a 3D-Space given value.
        /// </summary>
        private Vector3 unitPosition;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the data value which is declared for output data after calculation of how many Area-of-Effect
        ///     targets will get hit by the prediction input source.
        /// </summary>
        public int AoeHitCount { get; set; }

        /// <summary>
        ///     Gets or sets the list of the targets that the spell will hit (only if Area of Effect was enabled).
        /// </summary>
        public List<Obj_AI_Hero> AoeTargetsHit { get; set; } = new List<Obj_AI_Hero>();

        /// <summary>
        ///     Gets the number of targets the skill-shot will hit (only if Area of Effect was enabled).
        /// </summary>
        public int AoeTargetsHitCount => Math.Max(this.AoeHitCount, this.AoeTargetsHit.Count);

        /// <summary>
        ///     Gets or sets the position where the skill-shot should be casted to increase the accuracy.
        /// </summary>
        public Vector3 CastPosition
        {
            get => this.castPosition.IsZero ? this.Input.Unit.ServerPosition : this.castPosition.FixHeight();
            set => this.castPosition = value;
        }

        /// <summary>
        ///     Gets or sets the collision objects list which the input source would collide with.
        /// </summary>
        public List<Obj_AI_Base> CollisionObjects { get; set; } = new List<Obj_AI_Base>();

        /// <summary>
        ///     Gets or sets the hit chance.
        /// </summary>
        public HitChance HitChance { get; set; } = HitChance.Impossible;

        /// <summary>
        ///     Gets or sets where the unit is going to be when the skill-shot reaches his position.
        /// </summary>
        public Vector3 UnitPosition
        {
            get => this.unitPosition.IsZero ? this.Input.Unit.ServerPosition : this.unitPosition.FixHeight();
            set => this.unitPosition = value;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the input.
        /// </summary>
        internal PredictionInput Input { get; set; }

        #endregion
    }
}