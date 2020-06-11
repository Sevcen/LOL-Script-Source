namespace Aimtec.SDK.Extensions
{
    using System.Linq;

    using Aimtec.SDK.Util.Cache;

    public static class Vector3Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Counts the ally heroes in range.
        /// </summary>
        /// <param name="vector3">The vector3.</param>
        /// <param name="range">The range.</param>
        /// <returns>How many ally heroes are inside a 'float' range from the starting 'vector3' point.</returns>
        public static int CountAllyHeroesInRange(this Vector3 vector3, float range)
        {
            return GameObjects.AllyHeroes.Count(h => !h.IsMe && h.IsValidTarget(range, true, false, vector3));
        }

        /// <summary>
        ///     Counts the enemy heroes in range.
        /// </summary>
        /// <param name="vector3">The vector3.</param>
        /// <param name="range">The range.</param>
        /// <param name="dontIncludeStartingUnit">The starting unit which should not be included in the counting.</param>
        /// <returns>How many enemy heroes are inside a 'float' range from the starting 'vector3' point.</returns>
        public static int CountEnemyHeroesInRange(
            this Vector3 vector3,
            float range,
            Obj_AI_Base dontIncludeStartingUnit = null)
        {
            return GameObjects.EnemyHeroes.Count(
                h => h.NetworkId != dontIncludeStartingUnit?.NetworkId
                    && h.IsValidTarget(range, false, false, vector3));
        }

        public static float Distance(this Vector3 v1, Vector2 v2)
        {
            return Vector2.Distance((Vector2) v1, v2);
        }

        public static float Distance(this Vector3 v1, GameObject g)
        {
            return Vector3.Distance(v1, g.ServerPosition);
        }

        public static float DistanceSqr(this Vector3 v, Vector3 v2)
        {
            return Vector3.DistanceSquared(v, v2);
        }

        public static float DistanceSquared(this Vector3 v1, Vector2 v2)
        {
            return Vector2.DistanceSquared((Vector2) v1, v2);
        }

        public static float DistanceSquared(this Vector3 v1, Vector3 v2, bool calc3D = false)
        {
            return calc3D ? Vector3.DistanceSquared(v1, v2) : Vector2.DistanceSquared((Vector2) v1, (Vector2) v2);
        }

        /// <summary>
        ///     Extends a Vector3 to another Vector3.
        /// </summary>
        /// <param name="vector3">the starting position.</param>
        /// <param name="toVector3">the target direction.</param>
        /// <param name="distance">the amount of distance.</param>
        /// <returns>'vector3' extended to 'toVector3' by 'distance'.</returns>
        public static Vector3 Extend(this Vector3 vector3, Vector3 toVector3, float distance)
        {
            return vector3 + distance * (toVector3 - vector3).Normalized();
        }

        /// <summary>
        ///     Extends a Vector3 to a Vector2.
        /// </summary>
        /// <param name="vector3">the starting position.</param>
        /// <param name="toVector2">the target direction.</param>
        /// <param name="distance">the amount of distance.</param>
        /// <returns>'vector3' extended to 'toVector2' by 'distance'.</returns>
        public static Vector3 Extend(this Vector3 vector3, Vector2 toVector2, float distance)
        {
            return vector3 + distance * ((Vector3) toVector2 - vector3).Normalized();
        }

        public static Vector3 FixHeight(this Vector3 v1)
        {
            var v = v1;
            v.Y = NavMesh.GetHeightForWorld(v1.X, v1.Z);

            return v;
        }

        /// <summary>
        ///     Normalizeds the specified vector.
        /// </summary>
        /// <param name="v">The vector.</param>
        /// <returns>Vector3.</returns>
        public static Vector3 Normalized(this Vector3 v)
        {
            return Vector3.Normalize(v);
        }

        public static bool PointUnderAllyTurret(this Vector3 point)
        {
            var allyTurrets = ObjectManager
                .Get<Obj_AI_Turret>().Any(
                    t => t.IsAlly && point.Distance(t.ServerPosition)
                        < 950f + ObjectManager.GetLocalPlayer().BoundingRadius + t.BoundingRadius);
            return allyTurrets;
        }

        public static bool PointUnderEnemyTurret(this Vector3 point)
        {
            var enemyTurrets = ObjectManager
                .Get<Obj_AI_Turret>().Any(
                    t => t.IsEnemy && point.Distance(t.ServerPosition)
                        < 950f + ObjectManager.GetLocalPlayer().BoundingRadius + t.BoundingRadius);
            return enemyTurrets;
        }

        public static Vector2 To2D(this Vector3 v)
        {
            return (Vector2) v;
        }

        /// <summary>
        ///     Converts a Vector3 World Position to a Vector2 Minimap Position
        /// </summary>
        /// <param name="worldPos">The World Position.</param>
        public static Vector2 ToMiniMapPosition(this Vector3 worldPos)
        {
            Render.WorldToMinimap(worldPos, out Vector2 miniMapPosition);
            return miniMapPosition;
        }

        /// <summary>
        ///     Converts a Vector3 World Position to a Vector2 Screen Position
        /// </summary>
        /// <param name="worldPos">The World Position.</param>
        public static Vector2 ToScreenPosition(this Vector3 worldPos)
        {
            Render.WorldToScreen(worldPos, out Vector2 screenPosition);
            return screenPosition;
        }

        #endregion
    }
}