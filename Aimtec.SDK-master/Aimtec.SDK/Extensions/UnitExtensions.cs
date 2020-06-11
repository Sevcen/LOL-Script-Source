namespace Aimtec.SDK.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Aimtec.SDK.Damage;

    /// <summary>
    ///     Class UnitExtensions.
    /// </summary>
    public static class UnitExtensions
    {
        #region Static Fields

        private static Obj_AI_Hero Player => ObjectManager.GetLocalPlayer();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the SpellSlot of the spell
        /// </summary>
        public static SpellSlot GetSpellSlot(this Obj_AI_Base unit, string spellName)
        {
            var spell = unit.SpellBook.Spells.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && x.Name.Equals(spellName, StringComparison.OrdinalIgnoreCase));

            return spell?.Slot ?? SpellSlot.Unknown;
        }

        /// <summary>
        ///     Gets the spell that this slot belongs to
        /// </summary>
        public static Aimtec.Spell GetSpell(this Obj_AI_Base unit, SpellSlot slot)
        {
            return unit.SpellBook.GetSpell(slot);
        }


        /// <summary>
        ///     Counts the ally heroes in range.
        /// </summary>
        /// <param name="unit">the unit.</param>
        /// <param name="range">The range.</param>
        /// <returns>How many ally heroes are inside a 'float' range from the starting 'unit' GameObject.</returns>
        public static int CountAllyHeroesInRange(this GameObject unit, float range)
        {
            return unit.ServerPosition.CountAllyHeroesInRange(range);
        }

        /// <summary>
        ///     Counts the enemy heroes in range.
        /// </summary>
        /// <param name="unit">the unit.</param>
        /// <param name="range">The range.</param>
        /// <param name="dontIncludeStartingUnit">The original unit.</param>
        /// <returns>How many enemy heroes are inside a 'float' range from the starting 'unit' GameObject.</returns>
        public static int CountEnemyHeroesInRange(
            this GameObject unit,
            float range,
            Obj_AI_Base dontIncludeStartingUnit = null)
        {
            return unit.ServerPosition.CountEnemyHeroesInRange(range, dontIncludeStartingUnit);
        }

        /// <summary>
        ///     Returns the 3D distance between two vectors.
        /// </summary>
        /// <param name="v1">The start vector.</param>
        /// <param name="v2">The end vector.</param>
        public static float Distance(this Vector3 v1, Vector3 v2)
        {
            return Vector3.Distance(v1, v2);
        }

        /// <summary>
        ///     Returns the 3D distance between a gameobject and a vector.
        /// </summary>
        /// <param name="gameObject">The GameObject.</param>
        /// <param name="v1">The vector.</param>
        public static float Distance(this GameObject gameObject, Vector3 v1)
        {
            return Vector3.Distance(gameObject.ServerPosition, v1);
        }

        /// <summary>
        ///     Returns the 2D distance between a gameobject and a vector.
        /// </summary>
        /// <param name="gameObject">The GameObject.</param>
        /// <param name="v1">The vector.</param>
        /// <returns></returns>
        public static float Distance(this GameObject gameObject, Vector2 v1)
        {
            return Vector2.Distance((Vector2) gameObject.ServerPosition, v1);
        }

        /// <summary>
        ///     Returns the 3D distance between two gameobjects.
        /// </summary>
        /// <param name="gameObj">The start GameObject.</param>
        /// <param name="gameObj1">The target GameObject.</param>
        public static float Distance(this GameObject gameObj, GameObject gameObj1)
        {
            return Vector3.Distance(gameObj.ServerPosition, gameObj1.ServerPosition);
        }

        /// <summary>
        ///     Returns the 3D distance squared between two gameobjects.
        /// </summary>
        /// <param name="gameObj">The start GameObject.</param>
        /// <param name="gameObj1">The target GameObject.</param>
        public static float DistanceSqr(this GameObject gameObj, GameObject gameObj1)
        {
            return Vector3.DistanceSquared(gameObj.ServerPosition, gameObj1.ServerPosition);
        }

        /// <summary>
        ///     Returns the 3D distance squared between two gameobjects.
        /// </summary>
        /// <param name="gameObj">The start GameObject.</param>
        /// <param name="pos">The target position.</param>
        public static float DistanceSqr(this GameObject gameObj, Vector3 pos)
        {
            return Vector3.DistanceSquared(gameObj.ServerPosition, pos);
        }

        /// <summary>
        ///     Returns if a source unit is facing a target unit.
        /// </summary>
        public static bool IsFacingUnit(this Obj_AI_Base source, Obj_AI_Base target)
        {
            return source.Orientation.To2D().Perpendicular().AngleBetween((target.ServerPosition - source.ServerPosition).To2D()) < 90;
        }

        /// <summary>
        ///     Returns if both source and target are facing eachother.
        /// </summary>
        public static bool AreBothFacingEachother(Obj_AI_Base source, Obj_AI_Base target)
        {
            return source.IsFacingUnit(target) && target.IsFacingUnit(source);
        }

        /// <returns>
        ///     true if an unit is a Building; otherwise, false.
        /// </returns>
        public static bool IsBuilding(this AttackableUnit unit)
        {
            switch (unit.Type)
            {
                case GameObjectType.obj_AI_Turret:
                case GameObjectType.obj_BarracksDampener:
                case GameObjectType.obj_HQ:
                    return true;
            }

            return false;
        }

        /// <summary>
        ///     Returns a determined buff a determined unit has.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="buffName">The buff's stringname</param>
        public static Buff GetBuff(this Obj_AI_Base unit, string buffName)
        {
            return unit.BuffManager.GetBuff(buffName);
        }

        /// <summary>
        ///     Returns how many stacks of the 'buffname' buff the target possesses.
        /// </summary>
        /// <param name="from">The target.</param>
        /// <param name="buffname">The buffname.</param>
        public static int GetBuffCount(this Obj_AI_Base from, string buffname)
        {
            return from.BuffManager.GetBuffCount(buffname, true);
        }

        /// <summary>
        ///     Returns the real number of the stacks of the 'buffname' buff the target possesses.
        /// </summary>
        /// <param name="from">The target.</param>
        /// <param name="buffname">The buffname.</param>
        public static int GetRealBuffCount(this Obj_AI_Base from, string buffname)
        {
            var getBuffCount = from.BuffManager.GetBuffCount(buffname, true);
            switch (getBuffCount)
            {
                case -1:
                    return 0;
                case 0:
                    return 1;
            }

            return getBuffCount;
        }

        /// <summary>
        ///     Gets the full attack range.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>System.Single.</returns>
        public static float GetFullAttackRange(this Obj_AI_Base source, AttackableUnit target)
        {
            var baseRange = source.AttackRange + source.BoundingRadius;

            if (!target.IsValidTarget())
            {
                return baseRange;
            }

            if (!Player.ChampionName.Equals("Caitlyn"))
            {
                return baseRange + target.BoundingRadius;
            }

            var unit = target as Obj_AI_Base;

            if (unit != null && unit.HasBuff("caitlynyordletrapinternal"))
            {
                baseRange = 1300 + Player.BoundingRadius;
            }

            return baseRange + target.BoundingRadius;
        }

        public static List<Vector2> GetWaypoints(this Obj_AI_Base unit)
        {
            var result = new List<Vector2>();

            if (!unit.IsVisible)
            {
                return result;
            }

            result.Add(unit.ServerPosition.To2D());
            var path = unit.Path;

            if (path.Length <= 0)
            {
                return result;
            }

            for (var i = 1; i < path.Length; i++)
            {
                result.Add(path[i].To2D());
            }

            return result;
        }

        /// <summary>
        ///     Determines whether the specified target has a determined buff.
        /// </summary>
        /// <param name="from">The target.</param>
        /// <param name="buffname">The buffname.</param>
        /// <returns>
        ///     <c>true</c> if the specified target has the 'buffname' buff; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasBuff(this Obj_AI_Base from, string buffname)
        {
            return from.BuffManager.HasBuff(buffname, true);
        }

        /// <summary>
        ///     Determines whether the specified unit is affected by a determined bufftype.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="buffType">The buff type.</param>
        /// <returns>
        ///     <c>true</c> if the specified unit is affected by the 'buffType' BuffType; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasBuffOfType(this Obj_AI_Base unit, BuffType buffType)
        {
            return unit.BuffManager.HasBuffOfType(buffType);
        }

        /// <summary>
        ///     Determines whether the specified hero target has a determined item.
        /// </summary>
        /// <param name="from">The target.</param>
        /// <param name="itemId">The item's ID.</param>
        /// <returns>
        ///     <c>true</c> if the specified hero target has the 'itemId' item; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasItem(this Obj_AI_Hero from, uint itemId)
        {
            return from.Inventory.HasItem(itemId);
        }

        /// <summary>
        ///     Determines whether the specified target has a determined item.
        /// </summary>
        /// <param name="from">The target.</param>
        /// <param name="itemId">The item's ID.</param>
        /// <returns>
        ///     <c>true</c> if the specified target has the 'itemId' item; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasItem(this Obj_AI_Base from, uint itemId)
        {
            return from.Inventory.HasItem(itemId);
        }

        /// <summary>
        ///     Determines whether the specified target has a spell shield.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <returns>
        ///     <c>true</c> if the specified target has a spell shield; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasSpellShield(this Obj_AI_Base unit)
        {
            // probably broken 
            return unit.HasBuffOfType(BuffType.SpellShield) || unit.HasBuffOfType(BuffType.SpellImmunity);
        }

        /// <summary>
        ///     Returns the current health a determined unit has, in percentual.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public static float HealthPercent(this AttackableUnit unit)
        {
            return unit.Health / unit.MaxHealth * 100;
        }
        
        /// <summary>
        ///      Returns if the unit is facing the position
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <param name="position">The position</param>
        /// <returns></returns>
        public static bool IsFacing(this Obj_AI_Base unit, Vector3 position)
        {
            return unit != null && unit.IsValid &&
                   unit.Orientation.To2D().AngleBetween((position - unit.ServerPosition).To2D()) < 90;
        }

        /// <summary>
        ///       Returns if the unit is facing the target
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <param name="target">The target</param>
        /// <returns></returns>
        public static bool IsFacing(this Obj_AI_Base unit, Obj_AI_Base target)
        {
            return unit != null && target != null && unit.IsValid && target.IsValid &&
                   unit.Orientation.To2D().Perpendicular().AngleBetween((target.ServerPosition - unit.ServerPosition).To2D())
                   < 90;
        }

        /// <summary>
        ///     Determines whether the specified target is inside a determined range.
        /// </summary>
        /// <param name="unit">The target.</param>
        /// <param name="range">The range.</param>
        /// <returns>
        ///     <c>true</c> if the specified target is inside the specified range; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInRange(this Obj_AI_Base unit, float range)
        {
            if (unit != null)
            {
                return Vector3.Distance(unit.ServerPosition, Player.ServerPosition) <= range;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether or not the specified unit is recalling.
        /// </summary>
        /// <param name="unit">The unit</param>
        /// <returns>
        ///     <c>true</c> if the specified unit is recalling; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRecalling(this Obj_AI_Hero unit)
        {
            return unit.HasBuff("recall");
        }

        /// <summary>
        ///     Determines whether the target is a valid target in the auto attack range from the specified check range from
        ///     vector.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="allyIsValidTarget">if set to <c>true</c> allies will be set as valid targets.</param>
        /// <param name="checkRangeFrom">The check range from.</param>
        /// <returns><c>true</c> if the target is a valid target in the auto attack range; otherwise, <c>false</c>.</returns>
        public static bool IsValidAutoRange(
            this AttackableUnit target,
            bool allyIsValidTarget = false,
            Vector3 checkRangeFrom = default(Vector3))
        {
            if (target == null || !target.IsValid || target.IsDead || target.IsInvulnerable || !target.IsVisible
                || !target.IsTargetable)
            {
                return false;
            }

            if (!allyIsValidTarget && target.Team == Player.Team)
            {
                return false;
            }

            return target.Distance(checkRangeFrom != Vector3.Zero ? checkRangeFrom : Player.ServerPosition)
                < Player.GetFullAttackRange(target);
        }

        /// <summary>
        ///     Determines whether the specified target is a valid target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="range">The range.</param>
        /// <param name="allyIsValidTarget">if set to <c>true</c> allies will be set as valid targets.</param>
        /// <param name="includeBoundingRadius"></param>
        /// <param name="checkRangeFrom">The check range from position.</param>
        /// <returns>
        ///     <c>true</c> if the specified target is a valid target; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidTarget(
            this AttackableUnit target,
            float range = float.MaxValue,
            bool allyIsValidTarget = false,
            bool includeBoundingRadius = false,
            Vector3 checkRangeFrom = default(Vector3))
        {
            if (target == null || !target.IsValid || target.IsDead || target.IsInvulnerable || !target.IsVisible
                || !target.IsTargetable)
            {
                return false;
            }

            if (!allyIsValidTarget && target.Team == Player.Team)
            {
                return false;
            }

            return target.Distance(checkRangeFrom != Vector3.Zero ? checkRangeFrom : Player.ServerPosition) < range
                + (includeBoundingRadius ? Player.BoundingRadius + target.BoundingRadius : 0);
        }

        /// <summary>
        ///     Returns true if this unit is able to be targetted by spells 
        /// </summary>
        /// <param name="unit">The unit.</param
        /// <param name="range">The range.</param>
        public static bool IsValidSpellTarget(this AttackableUnit unit, float range = float.MaxValue)
        {
            if (!unit.IsValidTarget(range))
            {
                return false;
            }

            if (unit is Obj_AI_Hero)
            {
                return true;
            }

            var mUnit = unit as Obj_AI_Minion;
            if (mUnit == null)
            {
                return false;
            }

            var name = mUnit.UnitSkinName.ToLower();
            if (name.Contains("ward") || name.Contains("sru_plant_") || name.Contains("barrel"))
            {
                return false;
            }

            return true;
        }

  
        /// <summary>
        ///     Returns the current mana a determined unit has, in percentual.
        /// </summary>
        /// <param name="unit">The unit.</param>
        public static float ManaPercent(this Obj_AI_Base unit)
        {
            return unit.Mana / unit.MaxMana * 100;
        }

        /// <summary>
        ///     Gets the buffs of the unit which are valid and active
        /// </summary>
        /// <param name="unit">The unit.</param>
        public static Buff[] ValidActiveBuffs(this Obj_AI_Base unit)
        {
            return unit.Buffs.Where(buff => buff.IsValid && buff.IsActive).ToArray();
        }

        /// <summary>
        ///     Gets the crit multiplier
        /// </summary>
        /// <param name="hero">The hero.</param>
        /// <param name="checkCrit">The crit check.</param>
        public static float GetCritMultiplier(this Obj_AI_Hero hero, bool checkCrit = false)
        {
            var crit = hero.HasItem(ItemId.InfinityEdge) ? 1.5f : 1;
            return !checkCrit
                ? crit
                : (Math.Abs(hero.Crit - 1) < float.Epsilon
                    ? 1 + crit
                    : 1);
        }

        #endregion

        /// <summary>
        ///     Waypoint Tracker data container.
        /// </summary>
        internal static class WaypointTracker
        {
            #region Static Fields

            /// <summary>
            ///     Stored Paths.
            /// </summary>
            public static readonly Dictionary<int, List<Vector2>> StoredPaths = new Dictionary<int, List<Vector2>>();

            /// <summary>
            ///     Stored Ticks.
            /// </summary>
            public static readonly Dictionary<int, int> StoredTick = new Dictionary<int, int>();

            #endregion
        }
    }
}
