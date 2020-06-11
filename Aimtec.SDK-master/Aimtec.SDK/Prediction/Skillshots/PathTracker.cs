namespace Aimtec.SDK.Prediction.Skillshots
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aimtec.SDK.Extensions;

    public static class PathTracker
    {
        #region Constants

        private const double MaxTime = 1.5d;

        #endregion

        #region Static Fields

        private static readonly Dictionary<int, List<StoredPath>> StoredPaths = new Dictionary<int, List<StoredPath>>();

        #endregion

        #region Constructors and Destructors

        static PathTracker()
        {
            Obj_AI_Base.OnNewPath += Obj_AI_Hero_OnNewPath;
        }

        #endregion

        #region Public Methods and Operators

        public static StoredPath GetCurrentPath(Obj_AI_Base unit)
        {
            return StoredPaths.TryGetValue(unit.NetworkId, out List<StoredPath> value)
                ? value.LastOrDefault()
                : new StoredPath();
        }

        public static double GetMeanSpeed(Obj_AI_Base unit, double maxT)
        {
            var paths = GetStoredPaths(unit, MaxTime);
            var distance = 0d;
            if (paths.Count > 0)
            {
                // Assume that the unit was moving for the first path:
                distance += (maxT - paths[0].Time) * unit.MoveSpeed;

                for (var i = 0; i < paths.Count - 1; i++)
                {
                    var currentPath = paths[i];
                    var nextPath = paths[i + 1];

                    if (currentPath.WaypointCount > 0)
                    {
                        distance += Math.Min(
                            (currentPath.Time - nextPath.Time) * unit.MoveSpeed,
                            currentPath.Path.GetPathLength());
                    }
                }

                // Take into account the last path:
                var lastPath = paths.Last();
                if (lastPath.WaypointCount > 0)
                {
                    distance += Math.Min(lastPath.Time * unit.MoveSpeed, lastPath.Path.GetPathLength());
                }
            }
            else
            {
                return unit.MoveSpeed;
            }

            return distance / maxT;
        }

        public static List<StoredPath> GetStoredPaths(Obj_AI_Base unit, double maxT)
        {
            return StoredPaths.TryGetValue(unit.NetworkId, out List<StoredPath> value)
                ? value.Where(p => p.Time < maxT).ToList()
                : new List<StoredPath>();
        }

        #endregion

        #region Methods

        private static void Obj_AI_Hero_OnNewPath(Obj_AI_Base sender, Obj_AI_BaseNewPathEventArgs args)
        {
            if (!(sender is Obj_AI_Hero))
            {
                return;
            }

            if (!StoredPaths.ContainsKey(sender.NetworkId))
            {
                StoredPaths.Add(sender.NetworkId, new List<StoredPath>());
            }

            var newPath = new StoredPath { Tick = Game.TickCount, Path = args.Path.Select(x => (Vector2) x).ToList() };
            StoredPaths[sender.NetworkId].Add(newPath);

            if (StoredPaths[sender.NetworkId].Count > 50)
            {
                StoredPaths[sender.NetworkId].RemoveRange(0, 40);
            }
        }

        #endregion
    }
}