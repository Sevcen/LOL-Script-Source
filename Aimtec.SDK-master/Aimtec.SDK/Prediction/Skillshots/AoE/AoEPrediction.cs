namespace Aimtec.SDK.Prediction.Skillshots.AoE
{
    using System.Collections.Generic;
    using System.Linq;

    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util.Cache;

    public static class AoePrediction
    {
        #region Public Methods and Operators

        public static PredictionOutput GetAoEPrediction(PredictionInput input)
        {
            switch (input.Type)
            {
                case SkillshotType.Circle: return Circle.GetCirclePrediction(input);
                case SkillshotType.Cone: return Cone.GetConePrediction(input);
                case SkillshotType.Line: return Line.GetLinePrediction(input);
                default: return new PredictionOutput();
            }
        }

        #endregion

        #region Methods

        internal static List<PossibleTarget> GetPossibleTargets(PredictionInput input)
        {
            var result = new List<PossibleTarget>();
            var originalUnit = input.Unit;

            foreach (var enemy in GameObjects.EnemyHeroes.Where(
                x => x.NetworkId != originalUnit.NetworkId && x.IsValidTarget(
                    input.Range + 200 + input.RealRadius,
                    checkRangeFrom: input.RangeCheckFrom)))
            {
                input.Unit = enemy;

                var prediction = Prediction.Instance.GetPrediction(input, false, false);
                if (prediction.HitChance >= HitChance.High)
                {
                    result.Add(new PossibleTarget { Position = (Vector2) prediction.UnitPosition, Unit = enemy });
                }
            }

            return result;
        }

        #endregion
    }
}