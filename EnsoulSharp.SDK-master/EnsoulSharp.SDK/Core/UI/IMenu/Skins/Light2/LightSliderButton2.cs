﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightSliderButton2.cs" company="EnsoulSharp">
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
//   A custom implementation of an <see cref="ADrawable{MenuSliderButton}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace EnsoulSharp.SDK.Core.UI.IMenu.Skins.Light2
{
    using System.Globalization;

    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.Core.UI.IMenu.Skins.Light;
    using EnsoulSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of an <see cref="ADrawable{MenuSliderButton}" />
    /// </summary>
    public class LightSliderButton2 : LightSliderButton
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        /// <summary>
        ///     Offset.
        /// </summary>
        private static readonly int Offset = 15;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LightSliderButton2" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public LightSliderButton2(MenuSliderButton component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Draws a <see cref="MenuSliderButton" />
        /// </summary>
        public override void Draw()
        {
            //Slider

            var position = this.Component.Position;
            var centeredY =
                (int)
                LightUtilities.GetContainerRectangle(this.Component)
                    .GetCenteredText(null, MenuSettings.Font, this.Component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;
            var percent = (this.Component.SValue - this.Component.MinValue)
                          / (float)(this.Component.MaxValue - this.Component.MinValue);
            var x = position.X + Offset + (percent * (this.Component.MenuWidth - Offset * 2 - MenuSettings.ContainerHeight / 2));

            Line.Width = 3;
            Line.Begin();
            Line.Draw(
                new[] { new Vector2(x, position.Y + 1), new Vector2(x, position.Y + MenuSettings.ContainerHeight) },
                this.Component.Interacting ? new ColorBGRA(210, 210, 210, 255) : new ColorBGRA(170, 170, 170, 255));
            Line.End();

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                this.Component.DisplayName, 
                (int)(position.X + MenuSettings.ContainerTextOffset), 
                centeredY,
                MenuSettings.TextColor);

            var measureText = MenuSettings.Font.MeasureText(
                null, 
                this.Component.SValue.ToString(CultureInfo.InvariantCulture), 
                0);
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                this.Component.SValue.ToString(CultureInfo.InvariantCulture), 
                (int)(position.X + this.Component.MenuWidth - 5 - measureText.Width - MenuSettings.ContainerHeight), 
                centeredY,
                MenuSettings.TextColor);

            Line.Width = MenuSettings.ContainerHeight;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(position.X + Offset, position.Y + MenuSettings.ContainerHeight / 2f),
                        new Vector2(x, position.Y + MenuSettings.ContainerHeight / 2f)
                    },
                new ColorBGRA(229, 229, 229, 255));
            Line.End();

            //On / Off Button

            Line.Width = MenuSettings.ContainerHeight - 7;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(
                            (this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight - 1)
                            + MenuSettings.ContainerHeight / 2f,
                            this.Component.Position.Y + 1 + 3),
                        new Vector2(
                            (this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight - 1)
                            + MenuSettings.ContainerHeight / 2f,
                            this.Component.Position.Y + MenuSettings.ContainerHeight - 3)
                    },
                this.Component.BValue ? new ColorBGRA(68, 160, 255, 255) : new ColorBGRA(151, 151, 151, 255));
            Line.End();

            var centerX =
                (int)
                new Rectangle(
                    (int)(this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight),
                    (int)this.Component.Position.Y,
                    MenuSettings.ContainerHeight,
                    MenuSettings.ContainerHeight).GetCenteredText(
                        null,
                        MenuSettings.Font,
                        this.Component.BValue ? "On" : "Off",
                        CenteredFlags.HorizontalCenter).X;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                this.Component.BValue ? "On" : "Off",
                centerX,
                centeredY,
                new ColorBGRA(221, 233, 255, 255));
        }

        #endregion
    }
}