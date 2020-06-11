﻿// <copyright file="Gapcloser.cs" company="EnsoulSharp">
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

    using SharpDX;

    /// <summary>
    ///     Detection of Gap-closers and fires the OnGapCloser event.
    /// </summary>
    public static partial class Events
    {
        #region Static Fields

        /// <summary>
        ///     Gets or sets the active spells.
        /// </summary>
        private static readonly List<GapCloserEventArgs> ActiveSpellsList = new List<GapCloserEventArgs>();

        /// <summary>
        ///     Gets or sets the spells.
        /// </summary>
        [ResourceImport("Data.Gapclosers.json")]
        private static readonly Dictionary<string, List<GapCloser>> SpellsList = new Dictionary<string, List<GapCloser>>();

        #endregion

        #region Public Events

        /// <summary>
        ///     OnGapCloser Event.
        /// </summary>
        public static event EventHandler<GapCloserEventArgs> OnGapCloser;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the active spells.
        /// </summary>
        public static IEnumerable<GapCloserEventArgs> ActiveSpells => ActiveSpellsList;

        /// <summary>
        ///     Gets the spells.
        /// </summary>
        public static IEnumerable<IEnumerable<GapCloser>> GapCloserSpells => SpellsList.Values;

        #endregion

        #region Methods

        /// <summary>
        ///     On Do Cast subscribed event function
        /// </summary>
        /// <param name="sender"><see cref="AIBaseClient" /> sender</param>
        /// <param name="args">Process Spell Cast Data</param>
        private static void EventGapcloser(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (SpellsList.All(spell => spell.Value.All(data => data.SpellName != args.SData.Name.ToLower())))
            {
                return;
            }

            var gapcloser = SpellsList.Select(s => s.Value.Where(g => g.SpellName == args.SData.Name.ToLower()).FirstOrDefault()).FirstOrDefault();
            var hero = sender as AIHeroClient;
            var player = GameObjects.Player;
            if (hero != null)
            {
                ActiveSpellsList.Add(
                    new GapCloserEventArgs
                        {
                            Start = args.Start, End = args.To, Sender = hero, TickCount = Variables.TickCount,
                            SkillType =
                                (args.Target != null && args.Target.IsValid)
                                    ? GapcloserType.Targeted
                                    : GapcloserType.Skillshot,
                            Slot = args.Slot,
                            IsDirectedToPlayer =
                                (args.Target != null && args.Target.IsValid && args.Target.IsMe)
                                || (!gapcloser.Invert && player.Distance(args.To) < player.Distance(args.Start))
                                || (gapcloser.Invert && player.Distance(args.To) > player.Distance(args.Start))
                                || sender.IsFacing(player),
                            SpellName = args.SData.Name
                        });
            }
        }

        /// <summary>
        ///     On game tick update subscribed event function.
        /// </summary>
        private static void EventGapcloser()
        {
            ActiveSpellsList.RemoveAll(entry => Variables.TickCount > entry.TickCount + 900);
            if (OnGapCloser == null)
            {
                return;
            }

            foreach (var gapcloser in
                ActiveSpellsList.Where(gapcloser => gapcloser.Sender.IsValidTarget())
                    .Where(
                        gapcloser =>
                        gapcloser.SkillType == GapcloserType.Targeted
                        || (gapcloser.SkillType == GapcloserType.Skillshot
                            && GameObjects.Player.DistanceSquared(gapcloser.Sender) < 250000)))
            {
                OnGapCloser(MethodBase.GetCurrentMethod().DeclaringType, gapcloser);
            }
        }

        #endregion

        /// <summary>
        ///     GapCloser Data Container
        /// </summary>
        public struct GapCloser
        {
            #region Fields

            /// <summary>
            ///     Spell Type
            /// </summary>
            public GapcloserType SkillType { get; set; }

            /// <summary>
            ///     Spell Slot
            /// </summary>
            public SpellSlot Slot { get; set; }

            /// <summary>
            ///     Spell Name
            /// </summary>
            public string SpellName { get; set; }

            /// <summary>
            ///     Invert
            /// </summary>
            public bool Invert { get; set; }

            #endregion
        }

        /// <summary>
        ///     GapCloser Data Container
        /// </summary>
        public class GapCloserEventArgs : EventArgs
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the end.
            /// </summary>
            public Vector3 End { get; set; }

            /// <summary>
            ///     Gets or sets a value indicating whether is directed to player.
            /// </summary>
            public bool IsDirectedToPlayer { get; set; }

            /// <summary>
            ///     Gets or sets the sender.
            /// </summary>
            public AIHeroClient Sender { get; set; }

            /// <summary>
            ///     Gets or sets the skill type.
            /// </summary>
            public GapcloserType SkillType { get; set; }

            /// <summary>
            ///     Gets or sets the slot.
            /// </summary>
            public SpellSlot Slot { get; set; }

            /// <summary>
            ///     Gets or sets the spell name.
            /// </summary>
            public string SpellName { get; set; }

            /// <summary>
            ///     Gets or sets the start.
            /// </summary>
            public Vector3 Start { get; set; }

            /// <summary>
            ///     Gets or sets the tick count.
            /// </summary>
            public int TickCount { get; set; }

            #endregion
        }
    }
}