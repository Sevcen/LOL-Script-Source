// <copyright file="Movement.cs" company="EnsoulSharp">
//    Copyright (c) 2019 EnsoulSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

namespace EnsoulSharp.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EnsoulSharp.SDK.Core.Utils;

    using SharpDX;

    /// <summary>
    ///     Calculates a prediction based off data values given by the source input and converts it into a output prediction
    ///     for movement, containing spell casting position and unit position in 3D-Space.
    /// </summary>
    public class Movement
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="unit">Prediction Based Unit</param>
        /// <param name="delay">Prediction Delay</param>
        /// <returns>
        ///     <see cref="PredictionOutput" />
        /// </returns>
        public static PredictionOutput GetPrediction(AIBaseClient unit, float delay)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay });
        }

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="unit">Prediction Based Unit</param>
        /// <param name="delay">Prediction Delay</param>
        /// <param name="radius">Prediction Radius</param>
        /// <returns>
        ///     <see cref="PredictionOutput" />
        /// </returns>
        public static PredictionOutput GetPrediction(AIBaseClient unit, float delay, float radius)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius });
        }

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="unit">Prediction Based Unit</param>
        /// <param name="delay">Prediction Delay</param>
        /// <param name="radius">Prediction Radius</param>
        /// <param name="speed">Prediction Speed</param>
        /// <returns>
        ///     <see cref="PredictionOutput" />
        /// </returns>
        public static PredictionOutput GetPrediction(AIBaseClient unit, float delay, float radius, float speed)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius, Speed = speed });
        }

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <returns>
        ///     <see cref="PredictionOutput" /> output
        /// </returns>
        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return GetPrediction(input, true, true);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculates the position to cast a spell according to unit movement.
        /// </summary>
        /// <param name="input">PredictionInput type</param>
        /// <param name="additionalSpeed">Additional Speed (Multiplicative)</param>
        /// <returns>The <see cref="PredictionOutput" /></returns>
        internal static PredictionOutput GetAdvancedPrediction(PredictionInput input, float additionalSpeed = 0)
        {
            var speed = Math.Abs(additionalSpeed) < float.Epsilon ? input.Speed : input.Speed * additionalSpeed;

            if (Math.Abs(speed - int.MaxValue) < float.Epsilon)
            {
                speed = 90000;
            }

            var unit = input.Unit;
            var position = PositionAfter(unit, 1, unit.MoveSpeed - 100);
            var prediction = position + (speed * (input.Delay / 1000));

            return new PredictionOutput()
                       {
                           UnitPosition = new Vector3(position.X, position.Y, unit.Position.Z),
                           CastPosition = new Vector3(prediction.X, prediction.Y, unit.Position.Z),
                           Hitchance = HitChance.High
                       };
        }

        /// <summary>
        ///     Returns Dashing Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <returns><see cref="PredictionOutput" /> output</returns>
        internal static PredictionOutput GetDashingPrediction(PredictionInput input)
        {
            var dashData = input.Unit.GetDashInfo();
            var result = new PredictionOutput { Input = input };
            input.Delay += 0.1f;

            // Normal dashes.
            if (!dashData.IsBlink)
            {
                // Mid air:
                var dashPred = GetPositionOnPath(
                    input,
                    new List<Vector2> { input.Unit.Position.ToVector2(), dashData.Path.Last() },
                    dashData.Speed);
                if (dashPred.Hitchance >= HitChance.High)
                {
                    dashPred.CastPosition = dashPred.UnitPosition;
                    dashPred.Hitchance = HitChance.Dashing;
                    return dashPred;
                }

                // At the end of the dash:
                if (dashData.Path.PathLength() > 200)
                {
                    var endP = dashData.Path.Last();
                    var timeToPoint = input.Delay + (input.From.Distance(endP) / input.Speed);
                    if (timeToPoint
                        <= (input.Unit.Distance(endP) / dashData.Speed) + (input.RealRadius / input.Unit.MoveSpeed))
                    {
                        return new PredictionOutput
                                   {
                                       CastPosition = endP.ToVector3(), UnitPosition = endP.ToVector3(),
                                       Hitchance = HitChance.Dashing
                                   };
                    }
                }

                result.CastPosition = dashData.Path.Last().ToVector3();
                result.UnitPosition = result.CastPosition;

                // Figure out where the unit is going.
            }

            return result;
        }

        /// <summary>
        ///     Returns Immobile Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <param name="remainingImmobileT">Remaining Immobile Time</param>
        /// <returns><see cref="PredictionOutput" /> output</returns>
        internal static PredictionOutput GetImmobilePrediction(PredictionInput input, double remainingImmobileT)
        {
            var timeToReachTargetPosition = input.Delay + (input.Unit.Distance(input.From) / input.Speed);

            if (timeToReachTargetPosition <= remainingImmobileT + (input.RealRadius / input.Unit.MoveSpeed))
            {
                return new PredictionOutput
                           {
                               CastPosition = input.Unit.Position, UnitPosition = input.Unit.Position,
                               Hitchance = HitChance.Immobile
                           };
            }

            return new PredictionOutput
                       {
                           Input = input, CastPosition = input.Unit.Position,
                           UnitPosition = input.Unit.Position, Hitchance = HitChance.High
                       };
        }

        /// <summary>
        ///     Get Position on Unit's Path.
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <param name="path">Path in Vector2 List</param>
        /// <param name="speed">Unit Speed</param>
        /// <returns><see cref="PredictionOutput" /> output</returns>
        internal static PredictionOutput GetPositionOnPath(PredictionInput input, List<Vector2> path, float speed = -1)
        {
            speed = (Math.Abs(speed - (-1)) < float.Epsilon) ? input.Unit.MoveSpeed : speed;

            if (path.Count <= 1)
            {
                return new PredictionOutput
                           {
                               Input = input, UnitPosition = input.Unit.Position,
                               CastPosition = input.Unit.Position, Hitchance = HitChance.VeryHigh
                           };
            }

            var pLength = path.PathLength();

            // Skillshots with only a delay
            if (pLength >= (input.Delay * speed) - input.RealRadius
                && Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
            {
                var tDistance = (input.Delay * speed) - input.RealRadius;

                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var d = a.Distance(b);

                    if (d >= tDistance)
                    {
                        var direction = (b - a).Normalized();

                        var cp = a + (direction * tDistance);
                        var p = a
                                + (direction
                                   * ((i == path.Count - 2)
                                          ? Math.Min(tDistance + input.RealRadius, d)
                                          : (tDistance + input.RealRadius)));

                        return new PredictionOutput
                                   {
                                       Input = input, CastPosition = cp.ToVector3(), UnitPosition = p.ToVector3(),
                                       Hitchance =
                                           GamePath.PathTracker.GetCurrentPath(input.Unit).Time < 0.1d
                                               ? HitChance.VeryHigh
                                               : HitChance.High
                                   };
                    }

                    tDistance -= d;
                }
            }

            // Skillshot with a delay and speed.
            if (pLength >= (input.Delay * speed) - input.RealRadius
                && Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                path = path.CutPath(Math.Max(0, (input.Delay * speed) - input.RealRadius));
                var tT = 0f;
                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var tB = a.Distance(b) / speed;
                    var direction = (b - a).Normalized();
                    a = a - (speed * tT * direction);
                    var sol = a.VectorMovementCollision(b, speed, input.From.ToVector2(), input.Speed, tT);
                    var t = (float)sol[0];
                    var pos = (Vector2)sol[1];

                    if (pos.IsValid() && t >= tT && t <= tT + tB)
                    {
                        var p = pos + (input.RealRadius * direction);

                        if (input.Type == SkillshotType.SkillshotLine)
                        {
                            var alpha = (input.From.ToVector2() - p).AngleBetween(a - b);
                            if (alpha > 30 && alpha < 180 - 30)
                            {
                                var beta = (float)Math.Asin(input.RealRadius / p.Distance(input.From));
                                var cp1 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(beta);
                                var cp2 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(-beta);

                                pos = cp1.DistanceSquared(pos) < cp2.DistanceSquared(pos) ? cp1 : cp2;
                            }
                        }

                        return new PredictionOutput
                                   {
                                       Input = input, CastPosition = pos.ToVector3(), UnitPosition = p.ToVector3(),
                                       Hitchance =
                                           GamePath.PathTracker.GetCurrentPath(input.Unit).Time < 0.1d
                                               ? HitChance.VeryHigh
                                               : HitChance.High
                                   };
                    }

                    tT += tB;
                }
            }

            var position = path.Last();
            return new PredictionOutput
                       {
                           Input = input, CastPosition = position.ToVector3(), UnitPosition = position.ToVector3(),
                           Hitchance = HitChance.Medium
                       };
        }

        /// <summary>
        ///     Returns Calculated Prediction based off given data values.
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <param name="ft">Add Delay</param>
        /// <param name="checkCollision">Check Collision</param>
        /// <returns>
        ///     <see cref="PredictionOutput" /> output
        /// </returns>
        internal static PredictionOutput GetPrediction(PredictionInput input, bool ft, bool checkCollision)
        {
            PredictionOutput result = null;

            if (!input.Unit.IsValidTarget(float.MaxValue, false))
            {
                return new PredictionOutput();
            }

            if (ft)
            {
                // Increase the delay due to the latency and server tick:
                input.Delay += (Game.Ping / 2000f) + 0.06f;

                if (input.AoE)
                {
                    return Cluster.GetAoEPrediction(input);
                }
            }

            // Target too far away.
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon
                && input.Unit.DistanceSquared(input.RangeCheckFrom) > Math.Pow(input.Range * 1.5, 2))
            {
                return new PredictionOutput { Input = input };
            }

            // Unit is dashing.
            if (input.Unit.IsDashing())
            {
                result = GetDashingPrediction(input);
            }
            else
            {
                // Unit is immobile.
                var remainingImmobileT = UnitIsImmobileUntil(input.Unit);
                if (remainingImmobileT >= 0d)
                {
                    result = GetImmobilePrediction(input, remainingImmobileT);
                }
            }

            // Normal prediction
            if (result == null)
            {
                result = GetAdvancedPrediction(input);
            }

            // Check if the unit position is in range
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
            {
                if (result.Hitchance >= HitChance.High
                    && input.RangeCheckFrom.DistanceSquared(input.Unit.Position)
                    > Math.Pow(input.Range + (input.RealRadius * 3 / 4), 2))
                {
                    result.Hitchance = HitChance.Medium;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.UnitPosition)
                    > Math.Pow(input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.RealRadius : 0), 2))
                {
                    result.Hitchance = HitChance.OutOfRange;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.CastPosition) > Math.Pow(input.Range, 2))
                {
                    if (result.Hitchance != HitChance.OutOfRange)
                    {
                        result.CastPosition = input.RangeCheckFrom
                                              + (input.Range
                                                 * (result.UnitPosition - input.RangeCheckFrom).Normalized().SetZ());
                    }
                    else
                    {
                        result.Hitchance = HitChance.OutOfRange;
                    }
                }
            }

            // Check for collision
            if (checkCollision && input.Collision)
            {
                var positions = new List<Vector3> { result.UnitPosition, result.CastPosition, input.Unit.Position };
                var originalUnit = input.Unit;
                result.CollisionObjects = Collision.GetCollision(positions, input);
                result.CollisionObjects.RemoveAll(x => x.NetworkId == originalUnit.NetworkId);
                result.Hitchance = result.CollisionObjects.Count > 0 ? HitChance.Collision : result.Hitchance;
            }

            return result;
        }

        /// <summary>
        ///     Returns Standard Prediction
        /// </summary>
        /// <param name="input">
        ///     <see cref="PredictionInput" /> input
        /// </param>
        /// <returns><see cref="PredictionOutput" /> output</returns>
        internal static PredictionOutput GetStandardPrediction(PredictionInput input)
        {
            var speed = input.Unit.MoveSpeed;

            if (input.Unit.DistanceSquared(input.From) < 200 * 200)
            {
                // input.Delay /= 2;
                speed /= 1.5f;
            }

            var result = GetPositionOnPath(input, input.Unit.GetWaypoints(), speed);

            if (result.Hitchance >= HitChance.High && input.Unit is AIHeroClient)
            {
            }

            return result;
        }

        /// <summary>
        ///     Calculates the unit position after "t"
        /// </summary>
        /// <param name="unit">Unit to track</param>
        /// <param name="t">Track time</param>
        /// <param name="speed">Speed of unit</param>
        /// <returns>The <see cref="Vector2" /> of the after position</returns>
        internal static Vector2 PositionAfter(AIBaseClient unit, float t, float speed = float.MaxValue)
        {
            var distance = t * speed;
            var path = unit.GetWaypoints();

            for (var i = 0; i < path.Count - 1; i++)
            {
                var a = path[i];
                var b = path[i + 1];
                var d = a.Distance(b);

                if (d < distance)
                {
                    distance -= d;
                }
                else
                {
                    return a + (distance * (b - a).Normalized());
                }
            }

            return path[path.Count - 1];
        }

        /// <summary>
        ///     Returns if the unit is immobile and immobile time.
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <returns>Immobile Time left</returns>
        internal static double UnitIsImmobileUntil(AIBaseClient unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                    buff.IsActive && Game.Time <= buff.EndTime
                    && (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun
                        || buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare))
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return result - Game.Time;
        }

        #endregion
    }

    #region Prediction Input/Output

    /// <summary>
    ///     Prediction Input, collect data values from the input source of the requested prediction to execute a movement
    ///     prediction for both a unit position and a skill-shot casting area which is then returned as a
    ///     <see cref="PredictionOutput" />
    /// </summary>
    public class PredictionInput
    {
        #region Fields

        /// <summary>
        ///     From source location on a 3D-Space.
        /// </summary>
        private Vector3 @from;

        /// <summary>
        ///     Range check from location on a 3D-Space.
        /// </summary>
        private Vector3 rangeCheckFrom;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets a value indicating whether prediction should include Area of Effect calculations to hit as many as
        ///     targets possible.
        /// </summary>
        public bool AoE { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether input source has a collision flag to collide with other objects before
        ///     reaching the target.
        /// </summary>
        public bool Collision { get; set; }

        /// <summary>
        ///     Gets or sets the flags that contains the unit types that the skill-shot can collide with.
        /// </summary>
        public CollisionableObjects CollisionObjects { get; set; } = CollisionableObjects.Minions
                                                                     | CollisionableObjects.YasuoWall;

        /// <summary>
        ///     Gets or sets the skill-shot delay in seconds.
        /// </summary>
        public float Delay { get; set; }

        /// <summary>
        ///     Gets or sets the position from where the skill-shot missile gets fired.
        /// </summary>
        public Vector3 From
        {
            get
            {
                return this.@from.IsValid() ? this.@from : ObjectManager.Player.Position;
            }

            set
            {
                this.@from = value;
            }
        }

        /// <summary>
        ///     Gets or sets the skill-shot width's radius or the angle in case of the cone skill shots.
        /// </summary>
        public float Radius { get; set; } = 1f;

        /// <summary>
        ///     Gets or sets the skill-shot range in units.
        /// </summary>
        public float Range { get; set; } = float.MaxValue;

        /// <summary>
        ///     Gets or sets the position from where the range is checked.
        /// </summary>
        public Vector3 RangeCheckFrom
        {
            get
            {
                return this.rangeCheckFrom.IsValid() ? this.rangeCheckFrom : this.From;
            }

            set
            {
                this.rangeCheckFrom = value;
            }
        }

        /// <summary>
        ///     Gets or sets the skill-shot speed in units per second.
        /// </summary>
        public float Speed { get; set; } = float.MaxValue;

        /// <summary>
        ///     Gets or sets the skill-shot type.
        /// </summary>
        public SkillshotType Type { get; set; } = SkillshotType.SkillshotLine;

        /// <summary>
        ///     Gets or sets the unit that the prediction will made for.
        /// </summary>
        public AIBaseClient Unit { get; set; } = ObjectManager.Player;

        /// <summary>
        ///     Gets or sets a value indicating whether use bounding radius.
        /// </summary>
        public bool UseBoundingRadius { get; set; } = true;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the the calculated real radius of the unit.
        /// </summary>
        internal float RealRadius => this.UseBoundingRadius ? this.Radius + this.Unit.BoundingRadius : this.Radius;

        #endregion
    }

    /// <summary>
    ///     Prediction Output, contains the calculated data from the source prediction input.
    /// </summary>
    public class PredictionOutput
    {
        #region Fields

        /// <summary>
        ///     Cast Predicted Position data in a 3D-Space given value.
        /// </summary>
        private Vector3 castPosition;

        /// <summary>
        ///     Unit Predicted Position data ina a 3D-Space given value.
        /// </summary>
        private Vector3 unitPosition;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the data value which is declared for output data after calculation of how many Area-of-Effect
        ///     targets will get hit by the prediction input source.
        /// </summary>
        public int AoeHitCount { get; set; }

        /// <summary>
        ///     Gets or sets the list of the targets that the spell will hit (only if Area of Effect was enabled).
        /// </summary>
        public List<AIHeroClient> AoeTargetsHit { get; set; } = new List<AIHeroClient>();

        /// <summary>
        ///     Gets the number of targets the skill-shot will hit (only if Area of Effect was enabled).
        /// </summary>
        public int AoeTargetsHitCount => Math.Max(this.AoeHitCount, this.AoeTargetsHit.Count);

        /// <summary>
        ///     Gets or sets the position where the skill-shot should be casted to increase the accuracy.
        /// </summary>
        public Vector3 CastPosition
        {
            get
            {
                return this.castPosition.IsValid() ? this.castPosition.SetZ() : this.Input.Unit.Position;
            }

            set
            {
                this.castPosition = value;
            }
        }

        /// <summary>
        ///     Gets or sets the collision objects list which the input source would collide with.
        /// </summary>
        public List<AIBaseClient> CollisionObjects { get; set; } = new List<AIBaseClient>();

        /// <summary>
        ///     Gets or sets the hit chance.
        /// </summary>
        public HitChance Hitchance { get; set; } = HitChance.Impossible;

        /// <summary>
        ///     Gets or sets where the unit is going to be when the skill-shot reaches his position.
        /// </summary>
        public Vector3 UnitPosition
        {
            get
            {
                return this.unitPosition.IsValid() ? this.unitPosition.SetZ() : this.Input.Unit.Position;
            }

            set
            {
                this.unitPosition = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the input.
        /// </summary>
        internal PredictionInput Input { get; set; }

        #endregion
    }

    #endregion
}