﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechTheme.cs" company="EnsoulSharp">
//   Copyright (C) 2019 EnsoulSharp
//   
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//   
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details.
//   
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// <summary>
//   Implements a custom ITheme.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace EnsoulSharp.SDK.Core.UI.IMenu.Skins.Tech
{
    using System.Linq;

    using EnsoulSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Implements a default ITheme.
    /// </summary>
    public class TechTheme : ITheme
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true, Width = 1 };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="TechMenuSettings" /> class.
        ///     Use to preload <see cref="TechTheme" /> visual settings.
        /// </summary>
        static TechTheme()
        {
            TechMenuSettings.LoadSettings();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Builds a new handler for the given <see cref="MenuBool" />.
        /// </summary>
        /// <param name="component">The <see cref="MenuBool" /> where this handler is responsible for.</param>
        /// <returns>The handler</returns>
        public ADrawable<MenuBool> BuildBoolHandler(MenuBool component)
        {
            return new TechBool(component);
        }

        /// <summary>
        ///     Builds a new handler for the given <see cref="MenuButton" />.
        /// </summary>
        /// <param name="component">The <see cref="MenuButton" /> where this handler is responsible for.</param>
        /// <returns>The handler</returns>
        public ADrawable<MenuButton> BuildButtonHandler(MenuButton component)
        {
            return new TechButton(component);
        }

        /// <summary>
        ///     Builds a new handler for the given <see cref="MenuColor" />.
        /// </summary>
        /// <param name="component">The <see cref="MenuColor" /> where this handler is responsible for.</param>
        /// <returns>The handler</returns>
        public ADrawable<MenuColor> BuildColorHandler(MenuColor component)
        {
            return new TechColorPicker(component);
        }

        /// <summary>
        ///     Builds a new handler for the given <see cref="MenuKeyBind" />.
        /// </summary>
        /// <param name="component">The <see cref="MenuKeyBind" /> where this handler is responsible for.</param>
        /// <returns>The handler</returns>
        public ADrawable<MenuKeyBind> BuildKeyBindHandler(MenuKeyBind component)
        {
            return new TechKeyBind(component);
        }

        /// <summary>
        ///     Builds a new handler for the given <see cref="MenuList" />.
        /// </summary>
        /// <param name="component">The <see cref="MenuList" /> where this handler is responsible for.</param>
        /// <returns>The handler</returns>
        public ADrawable<MenuList> BuildListHandler(MenuList component)
        {
            return new TechList(component);
        }

        /// <summary>
        ///     Builds a new handler for the given <see cref="Menu" />.
        /// </summary>
        /// <param name="menu">The <see cref="Menu" /> where this handler is responsible for.</param>
        /// <returns>The handler</returns>
        public ADrawable<Menu> BuildMenuHandler(Menu menu)
        {
            return new TechMenu(menu);
        }

        /// <summary>
        ///     Builds a new handler for the given <see cref="MenuSeparator" />.
        /// </summary>
        /// <param name="component">The <see cref="MenuSeparator" /> where this handler is responsible for.</param>
        /// <returns>The handler</returns>
        public ADrawable<MenuSeparator> BuildSeparatorHandler(MenuSeparator component)
        {
            return new TechSeparator(component);
        }

        /// <summary>
        ///     Builds a new handler for the given <see cref="MenuSlider" />.
        /// </summary>
        /// <param name="component">The <see cref="MenuSlider" /> where this handler is responsible for.</param>
        /// <returns>The handler</returns>
        public ADrawable<MenuSlider> BuildSliderHandler(MenuSlider component)
        {
            return new TechSlider(component);
        }

        /// <summary>
        ///     Builds a new handler for the given <see cref="MenuSliderButton" />.
        /// </summary>
        /// <param name="component">The <see cref="MenuSliderButton" /> where this handler is responsible for.</param>
        /// <returns>The handler</returns>
        public ADrawable<MenuSliderButton> BuildSliderButtonHandler(MenuSliderButton component)
        {
            return new TechSliderButton(component);
        }

        /// <summary>
        ///     OnDraw event.
        /// </summary>
        public void Draw()
        {
            var position = MenuSettings.Position;
            var menuManager = MenuManager.Instance;
            var height = MenuSettings.ContainerHeight * menuManager.Menus.Count;
            var width = MenuSettings.ContainerWidth;
            if (menuManager.Menus.Count > 0)
            {
                width = menuManager.Menus.First().MenuWidth;
            }

            Utils.DrawBoxRounded(position.X - 1, position.Y - 1, width + 2, height + 3, 4, true,
                    MenuSettings.RootContainerColor, new ColorBGRA(13, 37, 37, 255), new ColorBGRA(30, 126, 125, 255));

            for (var i = 0; i < menuManager.Menus.Count; ++i)
            {
                var childPos = new Vector2(position.X, position.Y + i * MenuSettings.ContainerHeight);

                menuManager.Menus[i].OnDraw(childPos);
            }
        }

        #endregion
    }
}