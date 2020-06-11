namespace Aimtec.SDK.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aimtec.SDK.Extensions;

    public static class Dash
    {
        #region Static Fields


        private static readonly Dictionary<int, DashArgs> DetectedDashes = new Dictionary<int, DashArgs>();

        #endregion

        #region Constructors and Destructors

        static Dash()
        {
            Obj_AI_Base.OnNewPath += OnObjAiHeroDash;
        }

        #endregion

        #region Public Events

        public static event EventHandler<DashArgs> HeroDashed;

        #endregion

        #region Public Methods and Operators


        public static DashArgs GetDashInfo(this Obj_AI_Base unit)
        {
            return DetectedDashes.TryGetValue(unit.NetworkId, out DashArgs value) ? value : new DashArgs();
        }

        public static bool IsDashing(this Obj_AI_Base unit)
        {
            if (DetectedDashes.TryGetValue(unit.NetworkId, out DashArgs value) && unit.Path.Length != 0)
            {
                return value.EndTick != 0;
            }

            return false;
        }

        #endregion

        #region Methods

        private static void OnObjAiHeroDash(Obj_AI_Base sender, Obj_AI_BaseNewPathEventArgs args)
        {
            var hero = sender as Obj_AI_Hero;

            if (hero == null || !hero.IsValid)
            {
                return;
            }

            if (!DetectedDashes.ContainsKey(hero.NetworkId))
            {
                DetectedDashes.Add(hero.NetworkId, new DashArgs());
            }

            if (args.IsDash)
            {
                var path = new List<Vector2> { (Vector2) hero.ServerPosition };

                path.AddRange(args.Path.ToList().Select(x => (Vector2) x));

                DetectedDashes[hero.NetworkId].Unit = sender;
                DetectedDashes[hero.NetworkId].Path = path;
                DetectedDashes[hero.NetworkId].Speed = args.Speed;
                DetectedDashes[hero.NetworkId].StartPos = (Vector2) hero.ServerPosition;
                DetectedDashes[hero.NetworkId].StartTick = Game.TickCount - Game.Ping / 2;
                DetectedDashes[hero.NetworkId].EndPos = path.Last();
                DetectedDashes[hero.NetworkId].EndTick = DetectedDashes[hero.NetworkId].StartTick
                    + (int) (1000
                        * (DetectedDashes[hero.NetworkId].EndPos.Distance(DetectedDashes[hero.NetworkId].StartPos)
                            / DetectedDashes[hero.NetworkId].Speed));
                DetectedDashes[hero.NetworkId].Duration = DetectedDashes[hero.NetworkId].EndTick
                    - DetectedDashes[hero.NetworkId].StartTick;

                HeroDashed?.Invoke(null, DetectedDashes[hero.NetworkId]);
            }
            else
            {
                DetectedDashes[hero.NetworkId].EndTick = 0;
            }
        }

        #endregion

        public class DashArgs : EventArgs
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the dash duration.
            /// </summary>
            public int Duration { get; set; }

            /// <summary>
            ///     Gets or sets the end position.
            /// </summary>
            public Vector2 EndPos { get; set; }

            /// <summary>
            ///     Gets or sets the end tick.
            /// </summary>
            public int EndTick { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether is blink.
            /// </summary>
            public bool IsBlink { get; set; }

            /// <summary>
            ///     Gets or sets the path.
            /// </summary>
            public List<Vector2> Path { get; set; }

            /// <summary>
            ///     Gets or sets the speed.
            /// </summary>
            public float Speed { get; set; }

            /// <summary>
            ///     Gets or sets the start position.
            /// </summary>
            public Vector2 StartPos { get; set; }

            /// <summary>
            ///     Gets or sets the start tick.
            /// </summary>
            public int StartTick { get; set; }

            /// <summary>
            ///     Gets or sets the unit.
            /// </summary>
            public Obj_AI_Base Unit { get; set; }

            #endregion
        }
    }
}