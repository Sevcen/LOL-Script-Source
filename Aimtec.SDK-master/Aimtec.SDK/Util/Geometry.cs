namespace Aimtec.SDK.Util
{
    using System;

    /// <summary>
    ///     Class Geometry.
    /// </summary>
    public class Geometry
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Calculates the collision point between two vectors with different velocities after a fixed delay.
        /// </summary>
        /// <param name="startPoint1">The start point.</param>
        /// <param name="endPoint1">The ending point</param>
        /// <param name="v1">The velocity of the first vector.</param>
        /// <param name="startPoint2">The observerer standing point.</param>
        /// <param name="v2">The velocity of the second vector.</param>
        /// <param name="delay">The delay.</param>
        /// <returns>VectorMovementCollisionResult.</returns>
        public static VectorMovementCollisionResult VectorMovementCollision(
            Vector2 startPoint1,
            Vector2 endPoint1,
            float v1,
            Vector2 startPoint2,
            float v2,
            float delay = 0f)
        {
            float sP1x = startPoint1.X,
                  sP1y = startPoint1.Y,
                  eP1x = endPoint1.X,
                  eP1y = endPoint1.Y,
                  sP2x = startPoint2.X,
                  sP2y = startPoint2.Y;

            float d = eP1x - sP1x, e = eP1y - sP1y;
            float dist = (float) Math.Sqrt(d * d + e * e), t1 = float.NaN;
            float S = Math.Abs(dist) > float.Epsilon ? v1 * d / dist : 0,
                  K = Math.Abs(dist) > float.Epsilon ? v1 * e / dist : 0f;

            float r = sP2x - sP1x, j = sP2y - sP1y;
            var c = r * r + j * j;

            if (dist > 0f)
            {
                if (Math.Abs(v1 - float.MaxValue) < float.Epsilon)
                {
                    var t = dist / v1;
                    t1 = v2 * t >= 0f ? t : float.NaN;
                }
                else if (Math.Abs(v2 - float.MaxValue) < float.Epsilon)
                {
                    t1 = 0f;
                }
                else
                {
                    float a = S * S + K * K - v2 * v2, b = -r * S - j * K;

                    if (Math.Abs(a) < float.Epsilon)
                    {
                        if (Math.Abs(b) < float.Epsilon)
                        {
                            t1 = Math.Abs(c) < float.Epsilon ? 0f : float.NaN;
                        }
                        else
                        {
                            var t = -c / (2 * b);
                            t1 = v2 * t >= 0f ? t : float.NaN;
                        }
                    }
                    else
                    {
                        var sqr = b * b - a * c;
                        if (sqr >= 0)
                        {
                            var nom = (float) Math.Sqrt(sqr);
                            var t = (-nom - b) / a;
                            t1 = v2 * t >= 0f ? t : float.NaN;
                            t = (nom - b) / a;
                            var t2 = v2 * t >= 0f ? t : float.NaN;

                            if (!float.IsNaN(t2) && !float.IsNaN(t1))
                            {
                                if (t1 >= delay && t2 >= delay)
                                {
                                    t1 = Math.Min(t1, t2);
                                }
                                else if (t2 >= delay)
                                {
                                    t1 = t2;
                                }
                            }
                        }
                    }
                }
            }
            else if (Math.Abs(dist) < float.Epsilon)
            {
                t1 = 0f;
            }

            return new VectorMovementCollisionResult()
            {
                Time = t1,
                Position = !float.IsNaN(t1) ? new Vector2(sP1x + S * t1, sP1y + K * t1) : new Vector2()
            };
        }

        #endregion

        /// <summary>
        ///     Struct VectorMovementCollisionResult
        /// </summary>
        public struct VectorMovementCollisionResult
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the position.
            /// </summary>
            /// <value>The position.</value>
            public Vector2 Position { get; set; }

            /// <summary>
            ///     Gets or sets the time.
            /// </summary>
            /// <value>The time.</value>
            public float Time { get; set; }

            #endregion
        }
    }
}