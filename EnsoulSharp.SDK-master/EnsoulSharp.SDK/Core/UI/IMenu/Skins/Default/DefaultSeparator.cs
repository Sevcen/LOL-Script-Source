﻿// <copyright file="DefaultSeparator.cs" company="EnsoulSharp">
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

namespace EnsoulSharp.SDK.Core.UI.IMenu.Skins.Default
{
    using Core.Utils;
    using EnsoulSharp.SDK;
    using Values;

    /// <summary>
    ///     Implements <see cref="ADrawable{MenuSeperator}" /> as a default skin.
    /// </summary>
    public class DefaultSeparator : ADrawable<MenuSeparator>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultSeparator" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public DefaultSeparator(MenuSeparator component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            // Do nothing.
        }

        /// <summary>
        ///     Draw a <see cref="MenuSeparator" />
        /// </summary>
        public override void Draw()
        {
            var centerY = DefaultUtilities.GetContainerRectangle(this.Component)
                .GetCenteredText(
                    null, 
                    MenuSettings.Font, 
                    this.Component.DisplayName, 
                    CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                this.Component.DisplayName, 
                (int)centerY.X, 
                (int)centerY.Y, 
                MenuSettings.TextColor);
        }

        /// <summary>
        ///     Processes windows messages
        /// </summary>
        /// <param name="args">
        ///     The event data
        /// </param>
        public override void OnWndProc(WindowsKeys args)
        {
            // Do nothing.
        }

        /// <summary>
        ///     Calculates the Width of an AMenuComponent
        /// </summary>
        /// <returns>
        ///     The width.
        /// </returns>
        public override int Width()
        {
            return DefaultUtilities.CalcWidthItem(this.Component);
        }

        #endregion
    }
}