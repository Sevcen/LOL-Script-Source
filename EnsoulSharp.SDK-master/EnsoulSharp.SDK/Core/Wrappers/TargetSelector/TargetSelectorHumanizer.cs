﻿// <copyright file="TargetSelectorHumanizer.cs" company="EnsoulSharp">
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
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using EnsoulSharp.SDK.Core.UI.IMenu;
    using EnsoulSharp.SDK.Core.UI.IMenu.Values;

    #endregion

    /// <summary>
    ///     Humanize the target selector process
    /// </summary>
    public class TargetSelectorHumanizer
    {
        #region Constants

        /// <summary>
        ///     The maximum delay
        /// </summary>
        private const int MaxDelay = 1500;

        /// <summary>
        ///     The minimum delay
        /// </summary>
        private const int MinDelay = 0;

        #endregion

        #region Fields

        /// <summary>
        ///     The hero visible informations
        /// </summary>
        private readonly List<HeroVisibleEntry> entries = new List<HeroVisibleEntry>();

        /// <summary>
        ///     The menu
        /// </summary>
        private readonly Menu menu;

        /// <summary>
        ///     The fow delay
        /// </summary>
        private int fowDelay = 250;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TargetSelectorHumanizer" /> class.
        /// </summary>
        /// <param name="menu">
        ///     The menu.
        /// </param>
        public TargetSelectorHumanizer(Menu menu)
        {
            this.menu = menu;
            this.menu.Add(new MenuSlider("fowDelay", "Target Acquire Delay", this.fowDelay, MinDelay, MaxDelay));

            this.menu.MenuValueChanged += (sender, args) =>
                {
                    var slider = sender as MenuSlider;
                    if (slider != null)
                    {
                        if (slider.Name.Equals("fowDelay"))
                        {
                            this.fowDelay = slider.Value;
                        }
                    }
                };

            this.fowDelay = this.menu["fowDelay"].GetValue<MenuSlider>().Value;
            this.entries.AddRange(GameObjects.EnemyHeroes.Select(e => new HeroVisibleEntry(e)));
            GameObject.OnAssign += OnAssign;
            GameObject.OnDelete += OnDelete;
            Game.OnUpdate += this.OnGameUpdate;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the fow delay.
        /// </summary>
        /// <value>
        ///     The fow delay.
        /// </value>
        public int FowDelay
        {
            get
            {
                return this.fowDelay;
            }

            set
            {
                this.fowDelay = Math.Min(MaxDelay, Math.Max(MinDelay, value));
                this.menu["fowDelay"].GetValue<MenuSlider>().Value = this.fowDelay;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Filters the targets.
        /// </summary>
        /// <param name="heroes">
        ///     The heroes.
        /// </param>
        /// <returns>
        ///     The <see cref="List{T}" /> of <see cref="AIHeroClient" />.
        /// </returns>
        public List<AIHeroClient> FilterTargets(List<AIHeroClient> heroes)
        {
            var finalHeroes = heroes;
            if (this.FowDelay > 0)
            {
                foreach (var hero in new List<AIHeroClient>(finalHeroes))
                {
                    var entry = this.entries.FirstOrDefault(e => hero.Compare(e.Hero));
                    if (entry == null || Math.Abs(Variables.TickCount - entry.LastVisibleChangeTick) < this.FowDelay)
                    {
                        finalHeroes.Remove(hero);
                    }
                }
            }

            return finalHeroes;
        }

        #endregion

        #region Methods

        private void OnAssign(GameObject obj, EventArgs args)
        {
            var hero = obj as AIHeroClient;
            if (hero == null)
            {
                return;
            }
            if (this.entries.All(e => !e.Hero.Compare(hero)))
            {
                this.entries.Add(new HeroVisibleEntry(hero));
            }
        }

        private void OnDelete(GameObject obj, EventArgs args)
        {
            var hero = obj as AIHeroClient;
            if (hero == null)
            {
                return;
            }
            for (int i = this.entries.Count - 1; i >= 0; i--)
            {
                if (this.entries[i].Hero.Compare(hero))
                {
                    this.entries.Remove(this.entries[i]);
                }
            }
        }

        /// <summary>
        ///     Raises the <see cref="E:GameUpdate" /> event.
        /// </summary>
        /// <param name="args">
        ///     The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        private void OnGameUpdate(EventArgs args)
        {
            foreach (var entry in this.entries.Where(e => e.Visible != e.Hero.IsVisible))
            {
                entry.Visible = entry.Hero.IsVisible;
                entry.LastVisibleChangeTick = Variables.TickCount;
            }
        }

        #endregion
    }
}