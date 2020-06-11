namespace Aimtec.SDK.Prediction.Health
{
    /// <summary>
    ///     Interface IHealthPrediction
    /// </summary>
    public interface IHealthPrediction
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the predicted health of the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="time">The time.</param>
        /// <returns>System.Single.</returns>
        float GetPrediction(Obj_AI_Base target, int time);

        #endregion
    }
}