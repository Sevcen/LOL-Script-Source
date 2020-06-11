﻿// <copyright file="InterruptableSpell.cs" company="EnsoulSharp">
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
    using System.Reflection;

    using EnsoulSharp.SDK.Core.Utils;

    /// <summary>
    ///     Provides events for interrupting spells.
    /// </summary>
    public static partial class Events
    {
        #region Public Events

        /// <summary>
        ///     Gets fired when an enemy is casting a spellData that should be interrupted.
        /// </summary>
        public static event EventHandler<InterruptableTargetEventArgs> OnInterruptableTarget;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the casting interruptible spell dictionary.
        /// </summary>
        public static IReadOnlyDictionary<uint, InterruptableSpellData> CastingInterruptableSpell
            => CastingInterruptableSpellDictionary;

        /// <summary>
        ///     Gets the interruptible spells dictionary.
        /// </summary>
        public static IReadOnlyList<InterruptableSpellData> GlobalInterruptableSpells => GlobalInterruptableSpellsList;

        /// <summary>
        ///     Gets the interruptible spells dictionary.
        /// </summary>
        public static IReadOnlyDictionary<string, List<InterruptableSpellData>> InterruptableSpells
            => InterruptableSpellsDictionary;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the casting interrupt-able spell.
        /// </summary>
        private static Dictionary<uint, InterruptableSpellData> CastingInterruptableSpellDictionary { get; set; } =
            new Dictionary<uint, InterruptableSpellData>();

        /// <summary>
        ///     Gets or sets the interrupt-able spells.
        /// </summary>
        [ResourceImport("Data.InterruptableSpells.json")]
        private static Dictionary<string, List<InterruptableSpellData>> InterruptableSpellsDictionary { get; set; } =
            new Dictionary<string, List<InterruptableSpellData>>();

        /// <summary>
        ///     Gets the global interruptable spells list.
        /// </summary>
        [ResourceImport("Data.GlobalInterruptableSpellsList.json")]
        private static List<InterruptableSpellData> GlobalInterruptableSpellsList { get; set; } =
            new List<InterruptableSpellData>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the <see cref="InterruptableTargetEventArgs" /> for the unit.
        /// </summary>
        /// <param name="target">
        ///     The unit
        /// </param>
        /// <returns>
        ///     <see cref="InterruptableTargetEventArgs" />
        /// </returns>
        public static InterruptableTargetEventArgs GetInterruptableTargetData(AIHeroClient target)
        {
            if (target == null || !target.IsValid)
            {
                return null;
            }

            InterruptableSpellData value;
            if (CastingInterruptableSpellDictionary.TryGetValue(target.NetworkId, out value))
            {
                return new InterruptableTargetEventArgs(
                    target,
                    value.DangerLevel,
                    target.Spellbook.SpellEndTime,
                    value.MovementInterrupts);
            }

            return null;
        }

        /// <summary>
        ///     Checks if the unit is casting a spell that can be interrupted.
        /// </summary>
        /// <param name="target">
        ///     The unit
        /// </param>
        /// <param name="checkMovementInterruption">
        ///     Checks if moving cancels the spellData.
        /// </param>
        /// <returns>
        ///     If the unit is casting an interrupt-able spellData.
        /// </returns>
        public static bool IsCastingInterruptableSpell(this AIHeroClient target, bool checkMovementInterruption = false)
        {
            var data = GetInterruptableTargetData(target);
            return data != null && (!checkMovementInterruption || data.MovementInterrupts);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Function called by update tick event.
        /// </summary>
        private static void EventInterruptableSpell()
        {
            foreach (var hero in
                GameObjects.Heroes.Where(
                    hero =>
                    CastingInterruptableSpellDictionary.ContainsKey(hero.NetworkId) && !hero.Spellbook.IsCastingSpell
                    && !hero.Spellbook.IsChanneling && !hero.Spellbook.IsCharging))
            {
                CastingInterruptableSpellDictionary.Remove(hero.NetworkId);
            }

            if (OnInterruptableTarget == null)
            {
                return;
            }

            foreach (var newArgs in
                GameObjects.EnemyHeroes.Select(GetInterruptableTargetData).Where(newArgs => newArgs != null))
            {
                OnInterruptableTarget(MethodBase.GetCurrentMethod().DeclaringType, newArgs);
            }
        }

        /// <summary>
        ///     Function called by OnDoCast event
        /// </summary>
        /// <param name="sender">
        ///     The sender
        /// </param>
        /// <param name="args">
        ///     Processed Spell Cast Data
        /// </param>
        private static void EventInterruptableSpell(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var target = sender as AIHeroClient;
            if (target == null || CastingInterruptableSpellDictionary.ContainsKey(target.NetworkId))
            {
                return;
            }

            var globalInterruptSpell = GlobalInterruptableSpells.FirstOrDefault(s => s.Name.Equals(args.SData.Name.ToLower()));
            if (globalInterruptSpell != null)
            {
                CastingInterruptableSpellDictionary.Add(target.NetworkId, globalInterruptSpell);
                return;
            }

            if (!InterruptableSpellsDictionary.ContainsKey(target.CharacterName))
            {
                return;
            }

            var spell = InterruptableSpellsDictionary[target.CharacterName].Find(
                s =>
                    {
                        var firstOrDefault = target.Spellbook.Spells.FirstOrDefault(
                            x => x.SData.Name == args.SData.Name);
                        return firstOrDefault != null && s.Slot == firstOrDefault.Slot;
                    });

            if (spell != null)
            {
                CastingInterruptableSpellDictionary.Add(target.NetworkId, spell);
            }
        }

        /// <summary>
        ///     Function called by the stop cast event.
        /// </summary>
        /// <param name="sender">
        ///     Sender Spell-book
        /// </param>
        private static void EventInterruptableSpell(Spellbook sender)
        {
            var target = sender.Owner as AIHeroClient;

            if (target == null)
            {
                return;
            }

            if (!target.Spellbook.IsCastingSpell && !target.Spellbook.IsChanneling && !target.Spellbook.IsCharging)
            {
                CastingInterruptableSpellDictionary.Remove(target.NetworkId);
            }
        }

        #endregion

        /// <summary>
        ///     Interrupt-able Spell Data
        /// </summary>
        public class InterruptableSpellData
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="InterruptableSpellData" /> class.
            /// </summary>
            public InterruptableSpellData()
            {
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="InterruptableSpellData" /> class.
            /// </summary>
            /// <param name="slot">
            ///     Spell Slot
            /// </param>
            /// <param name="dangerLevel">
            ///     Danger Level
            /// </param>
            /// <param name="movementInterrupts">
            ///     Does movement interrupt the spell
            /// </param>
            public InterruptableSpellData(SpellSlot slot, DangerLevel dangerLevel, bool movementInterrupts = true)
            {
                this.Slot = slot;
                this.DangerLevel = dangerLevel;
                this.MovementInterrupts = movementInterrupts;
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="InterruptableSpellData" /> class.
            /// </summary>
            /// <param name="name">
            ///     Spell Name
            /// </param>
            /// <param name="dangerLevel">
            ///     Danger Level
            /// </param>
            /// <param name="slot">
            ///     The slot.
            /// </param>
            /// <param name="movementInterrupts">
            ///     Does movement interrupt the spell
            /// </param>
            public InterruptableSpellData(
                string name,
                DangerLevel dangerLevel,
                SpellSlot slot = SpellSlot.Unknown,
                bool movementInterrupts = true)
            {
                this.Name = name;
                this.DangerLevel = dangerLevel;
                this.Slot = slot;
                this.MovementInterrupts = movementInterrupts;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the danger level.
            /// </summary>
            public DangerLevel DangerLevel { get; set; }

            /// <summary>
            ///     Gets a value indicating whether movement interrupts.
            /// </summary>
            public bool MovementInterrupts { get; set; }

            /// <summary>
            ///     Gets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            ///     Gets the slot.
            /// </summary>
            public SpellSlot Slot { get; set; }

            #endregion
        }

        /// <summary>
        ///     Class that represents the event arguments for <see cref="OnInterruptableTarget" />
        /// </summary>
        public class InterruptableTargetEventArgs : EventArgs
        {
            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="InterruptableTargetEventArgs" /> class.
            /// </summary>
            /// <param name="sender">
            ///     Sender or classified Target
            /// </param>
            /// <param name="dangerLevel">
            ///     Danger Level
            /// </param>
            /// <param name="endTime">
            ///     Ending time of the spell
            /// </param>
            /// <param name="movementInterrupts">
            ///     Does Movement Interrupts the spell
            /// </param>
            internal InterruptableTargetEventArgs(
                AIHeroClient sender,
                DangerLevel dangerLevel,
                float endTime,
                bool movementInterrupts)
            {
                this.Sender = sender;
                this.DangerLevel = dangerLevel;
                this.EndTime = endTime;
                this.MovementInterrupts = movementInterrupts;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the <see cref="DangerLevel" /> of the spellData.
            /// </summary>
            public DangerLevel DangerLevel { get; private set; }

            /// <summary>
            ///     Gets the time the spellData ends.
            /// </summary>
            public float EndTime { get; private set; }

            /// <summary>
            ///     Gets a value indicating whether movement interrupts.
            /// </summary>
            public bool MovementInterrupts { get; }

            /// <summary>
            ///     Gets the sender.
            /// </summary>
            public AIHeroClient Sender { get; }

            #endregion
        }
    }
}