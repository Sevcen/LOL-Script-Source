namespace Aimtec.SDK.Prediction.Health
{
    /// <summary>
    ///     Class HealthPrediction.
    /// </summary>
    /// <seealso cref="Aimtec.SDK.Prediction.Health.IHealthPrediction" />
    public class HealthPrediction : IHealthPrediction
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the implementation.
        /// </summary>
        /// <value>The implementation.</value>
        public static IHealthPrediction Implementation { get; set; } = new HealthPredictionImplB();

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static HealthPrediction Instance { get; } = new HealthPrediction();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the predicted health of the target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="time">The time.</param>
        /// <returns>System.Single.</returns>
        public float GetPrediction(Obj_AI_Base target, int time)
        {
            return Implementation.GetPrediction(target, time);
        }

        #endregion
    }
}