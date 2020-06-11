namespace Aimtec.SDK.Prediction.Skillshots.AoE
{
    internal class PossibleTarget
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the unit position.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        ///     Gets or sets the unit.
        /// </summary>
        public Obj_AI_Base Unit { get; set; }

        #endregion
    }
}