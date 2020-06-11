namespace Aimtec.SDK.Prediction.Skillshots.AoE
{
    using System.Collections.Generic;
    using System.Linq;

    using Aimtec.SDK.Extensions;

    public static class Line
    {
        #region Public Methods and Operators

        public static PredictionOutput GetLinePrediction(PredictionInput input)
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

            foreach (var targetCandidates in posibleTargets.Select(
                target => GetCandidates((Vector2) input.From, target.Position, input.Radius, input.Range)))
            {
                candidates.AddRange(targetCandidates);
            }

            var bestCandidateHits = -1;
            var bestCandidate = default(Vector2);
            var bestCandidateHitPoints = new List<Vector2>();
            var positionsList = posibleTargets.Select(t => t.Position).ToList();

            foreach (var candidate in candidates)
            {
                if (GetHits(
                    (Vector2) input.From,
                    candidate,
                    input.Radius + input.Unit.BoundingRadius / 3 - 10,
                    new List<Vector2> { posibleTargets[0].Position }).Count() != 1)
                {
                    continue;
                }

                var hits = GetHits((Vector2) input.From, candidate, input.Radius, positionsList).ToList();
                var hitsCount = hits.Count;

                if (hitsCount < bestCandidateHits)
                {
                    continue;
                }

                bestCandidateHits = hitsCount;
                bestCandidate = candidate;
                bestCandidateHitPoints = hits.ToList();
            }

            if (bestCandidateHits <= 1)
            {
                return mainTargetPrediction;
            }

            float maxDistance = -1;
            Vector2 p1 = default(Vector2), p2 = default(Vector2);

            for (var i = 0; i < bestCandidateHitPoints.Count; i++)
            {
                for (var j = 0; j < bestCandidateHitPoints.Count; j++)
                {
                    var startP = (Vector2) input.From;
                    var endP = bestCandidate;
                    var proj1 = positionsList[i].ProjectOn(startP, endP);
                    var proj2 = positionsList[j].ProjectOn(startP, endP);
                    var dist = bestCandidateHitPoints[i].DistanceSquared(proj1.LinePoint)
                        + bestCandidateHitPoints[j].DistanceSquared(proj2.LinePoint);

                    if (!(dist >= maxDistance)
                        || !((proj1.LinePoint - positionsList[i]).AngleBetween(proj2.LinePoint - positionsList[j])
                            > 90))
                    {
                        continue;
                    }

                    maxDistance = dist;
                    p1 = positionsList[i];
                    p2 = positionsList[j];
                }
            }

            return new PredictionOutput
            {
                HitChance = mainTargetPrediction.HitChance,
                AoeHitCount = bestCandidateHits,
                UnitPosition = mainTargetPrediction.UnitPosition,
                CastPosition = (Vector3) ((p1 + p2) * 0.5f),
                Input = input
            };
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Returns a list of Vector2 point candidates for the linear prediction.
        /// </summary>
        /// <param name="from">Vector2 from position</param>
        /// <param name="to">Vector2 to position</param>
        /// <param name="radius">The Radius</param>
        /// <param name="range">The Range</param>
        /// <returns>Vector2 list</returns>
        internal static Vector2[] GetCandidates(Vector2 from, Vector2 to, float radius, float range)
        {
            var middlePoint = (from + to) / 2;
            var intersections = from.CircleCircleIntersection(middlePoint, radius, from.Distance(middlePoint));

            if (intersections.Length <= 1)
            {
                return new Vector2[] { };
            }

            var c1 = intersections[0];
            var c2 = intersections[1];

            c1 = from + range * (to - c1).Normalized();
            c2 = from + range * (to - c2).Normalized();

            return new[] { c1, c2 };
        }

        /// <summary>
        ///     Returns the number of hits
        /// </summary>
        /// <param name="start">Vector2 starting point</param>
        /// <param name="end">Vector2 ending point</param>
        /// <param name="radius">Line radius</param>
        /// <param name="points">Vector2 points</param>
        /// <returns>Number of Hits</returns>
        internal static IEnumerable<Vector2> GetHits(Vector2 start, Vector2 end, double radius, List<Vector2> points)
        {
            return points.Where(p => p.DistanceSquared(start, end, true) <= radius * radius);
        }

        #endregion
    }
}