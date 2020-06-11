namespace Aimtec.SDK.Prediction.Skillshots
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Stored Path Container, contains a stored path
    /// </summary>
    public class StoredPath
    {
        #region Public Properties

        /// <summary>
        ///     Gets the end point.
        /// </summary>
        public Vector2 EndPoint => this.Path.LastOrDefault();

        /// <summary>
        ///     Gets or sets the path.
        /// </summary>
        public List<Vector2> Path { get; set; }

        /// <summary>
        ///     Gets the start point.
        /// </summary>
        public Vector2 StartPoint => this.Path.FirstOrDefault();

        /// <summary>
        ///     Gets or sets the tick.
        /// </summary>
        public int Tick { get; set; }

        /// <summary>
        ///     Gets the current tick of the path.
        /// </summary>
        public double Time => (Game.TickCount - this.Tick) / 1000d;

        /// <summary>
        ///     Gets the number of waypoints within the path.
        /// </summary>
        public int WaypointCount => this.Path.Count;

        #endregion
    }
}