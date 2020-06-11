namespace Aimtec.SDK.Prediction.Skillshots
{
    public class Prediction : ISkillshotPrediction
    {
        #region Public Properties

        public static Prediction Instance { get; } = new Prediction();

        public ISkillshotPrediction Implementation { get; set; } = new PredictionImpl();

        #endregion

        #region Public Methods and Operators

        public PredictionOutput GetPrediction(PredictionInput input)
        {
            return this.Implementation.GetPrediction(input);
        }

        public PredictionOutput GetPrediction(PredictionInput input, bool ft, bool collision)
        {
            return this.Implementation.GetPrediction(input, ft, collision);
        }

        public void ResetImplementation()
        {
            this.Implementation = new PredictionImpl();
        }

        #endregion
    }
}
