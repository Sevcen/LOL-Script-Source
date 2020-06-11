namespace Aimtec.SDK.Prediction.Skillshots.AoE
{
    using System.Collections.Generic;
    using System.Linq;

    using Aimtec.SDK.Extensions;
    using Aimtec.SDK.Util;

    public static class Circle
    {
        #region Public Methods and Operators

        public static PredictionOutput GetCirclePrediction(PredictionInput input)
        {
            var mainTargetPrediction = Prediction.Instance.GetPrediction(input, false, true);
            var posibleTargets = new List<PossibleTarget>
            {
                new PossibleTarget { Position = (Vector2) mainTargetPrediction.UnitPosition, Unit = input.Unit }
            };

            if (mainTargetPrediction.HitChance >= HitChance.Medium)
            {
                posibleTargets.AddRange(AoePrediction.GetPossibleTargets(input));
            }

            while (posibleTargets.Count > 1)
            {
                var mecCircle = Mec.GetMec(posibleTargets.Select(h => h.Position).ToList());

                if (mecCircle.Radius <= input.RealRadius - 10 && mecCircle.Center.DistanceSquared(input.RangeCheckFrom)
                    < input.Range * input.Range)
                {
                    return new PredictionOutput
                    {
                        AoeTargetsHit = posibleTargets.Select(h => (Obj_AI_Hero) h.Unit).ToList(),
                        CastPosition = (Vector3) mecCircle.Center,
                        UnitPosition = mainTargetPrediction.UnitPosition,
                        HitChance = mainTargetPrediction.HitChance,
                        Input = input,
                        AoeHitCount = posibleTargets.Count
                    };
                }

                float maxdist = -1;
                var maxdistindex = 1;

                for (var i = 1; i < posibleTargets.Count; i++)
                {
                    var distance = posibleTargets[i].Position.DistanceSquared(posibleTargets[0].Position);

                    if (!(distance > maxdist) && maxdist.CompareTo(-1) != 0)
                    {
                        continue;
                    }

                    maxdistindex = i;
                    maxdist = distance;
                }

                posibleTargets.RemoveAt(maxdistindex);
            }

            return mainTargetPrediction;
        }

        #endregion
    }
}