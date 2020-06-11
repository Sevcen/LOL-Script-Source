﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightSeparator.cs" company="EnsoulSharp">
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
//   Implements <see cref="ADrawable{MenuSeperator}" /> as a custom skin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace EnsoulSharp.SDK.Core.UI.IMenu.Skins.Light
{
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.Core.UI.IMenu.Values;
    using EnsoulSharp.SDK.Core.Utils;

    /// <summary>
    ///     Implements <see cref="ADrawable{MenuSeperator}" /> as a default skin.
    /// </summary>
    public class LightSeparator : ADrawable<MenuSeparator>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LightSeparator" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public LightSeparator(MenuSeparator component)
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
            var centerY = LightUtilities.GetContainerRectangle(this.Component)
                .GetCenteredText(
                    null,
                    LightMenuSettings.FontCaption,
                    this.Component.DisplayName,
                    CenteredFlags.VerticalCenter | CenteredFlags.HorizontalCenter);

            LightMenuSettings.FontCaption.DrawText(
                MenuManager.Instance.Sprite,
                this.Component.DisplayName,
                (int)centerY.X,
                (int)centerY.Y,
                LightMenuSettings.TextCaptionColor);
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
            return LightUtilities.CalcWidthItem(this.Component);
        }

        #endregion
    }
}