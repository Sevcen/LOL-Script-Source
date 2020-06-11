namespace Aimtec.SDK.Prediction.Skillshots
{
    public interface ISkillshotPrediction
    {
        PredictionOutput GetPrediction(PredictionInput input);
        PredictionOutput GetPrediction(PredictionInput input, bool ft, bool collision);
    }
}
