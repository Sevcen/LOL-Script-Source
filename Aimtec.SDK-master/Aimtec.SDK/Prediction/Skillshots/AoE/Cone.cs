namespace Aimtec.SDK.Prediction.Skillshots.AoE
{
    using System.Collections.Generic;
    using System.Linq;

    using Aimtec.SDK.Extensions;

    public static class Cone
    {
        #region Public Methods and Operators

        public static PredictionOutput GetConePrediction(PredictionInput input)
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

            if (posibleTargets.Count <= 1)
            {
                return mainTargetPrediction;
            }
            var candidates = new List<Vector2>();

            foreach (var target in posibleTargets)
            {
                target.Position = target.Position - (Vector2) input.From;
            }

            for (var i = 0; i < posibleTargets.Count; i++)
            {
                for (var j = 0; j < posibleTargets.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }

                    var p = (posibleTargets[i].Position + posibleTargets[j].Position) * 0.5f;

                    if (!candidates.Contains(p))
                    {
                        candidates.Add(p);
                    }
                }
            }

            var bestCandidateHits = -1;
            var bestCandidate = default(Vector2);
            var positionsList = posibleTargets.Select(t => t.Position).ToList();

            foreach (var candidate in candidates)
            {
                var hits = GetHits(candidate, input.Range, input.Radius, positionsList);

                if (hits <= bestCandidateHits)
                {
                    continue;
                }

                bestCandidate = candidate;
                bestCandidateHits = hits;
            }

            if (bestCandidateHits > 1 && input.From.DistanceSquared(bestCandidate) > 50 * 50)
            {
                return new PredictionOutput
                {
                    HitChance = mainTargetPrediction.HitChance,
                    AoeHitCount = bestCandidateHits,
                    UnitPosition = mainTargetPrediction.UnitPosition,
                    CastPosition = (Vector3) bestCandidate,
                    Input = input
                };
            }

            return mainTargetPrediction;
        }

        #endregion

        #region Methods

        internal static int GetHits(Vector2 end, double range, float angle, List<Vector2> points)
        {
            return points.Select(x => new { point = x, edge1 = end.Rotated(-angle / 2) })
                         .Select(x => new { t = x, edge2 = x.edge1.Rotated(angle) })
                         .Where(
                             x => x.t.point.DistanceSquared(default(Vector2)) < range * range
                                 && x.t.edge1.CrossProduct(x.t.point) > 0 && x.t.point.CrossProduct(x.edge2) > 0)
                         .Select(x => x.t.point).Count();
        }

        #endregion
    }
}