﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColoredSliderButton.cs" company="EnsoulSharp">
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
namespace EnsoulSharp.SDK.Core.UI.IMenu.Skins.Colored
{
    using System;
    using System.Globalization;

    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.Core.UI.IMenu.Values;
    using EnsoulSharp.SDK.Core.Utils;

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     A default implementation of an <see cref="ADrawable{MenuSliderButton}" />
    /// </summary>
    public class ColoredSliderButton : ADrawable<MenuSliderButton>
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
        ///     Initializes a new instance of the <see cref="ColoredSliderButton" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public ColoredSliderButton(MenuSliderButton component)
            : base(component)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Returns the Rectangle that defines the Slider
        /// </summary>
        /// <param name="component">The <see cref="MenuSliderButton" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle SliderBoundaries(MenuSliderButton component)
        {
            return new Rectangle(
                (int)component.Position.X + Offset,
                (int)component.Position.Y,
                component.MenuWidth - MenuSettings.ContainerHeight - Offset,
                MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Returns the Rectangle that defines the on/off Button
        /// </summary>
        /// <param name="component">The <see cref="MenuSliderButton" /></param>
        /// <returns>The <see cref="Rectangle" /></returns>
        public Rectangle ButtonBoundaries(MenuSliderButton component)
        {
            return new Rectangle(
                (int)(component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight * 1.2),
                (int)component.Position.Y,
                MenuSettings.ContainerHeight,
                MenuSettings.ContainerHeight);
        }

        /// <summary>
        ///     Disposes any resources used in this handler.
        /// </summary>
        public override void Dispose()
        {
            // Do nothing.
        }

        /// <summary>
        ///     Draws a <see cref="MenuSlider" />
        /// </summary>
        public override void Draw()
        {
            //Slider

            var position = this.Component.Position;
            var centeredY =
                (int)
                ColoredUtilities.GetContainerRectangle(this.Component)
                    .GetCenteredText(null, MenuSettings.Font, this.Component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;
            var percent = (this.Component.SValue - this.Component.MinValue)
                          / (float)(this.Component.MaxValue - this.Component.MinValue);
            var x = position.X + Offset + (percent * (this.Component.MenuWidth - Offset * 2 - MenuSettings.ContainerHeight));
            var maxX = position.X + Offset + ((this.Component.MenuWidth - Offset * 2 - MenuSettings.ContainerHeight));

            MenuManager.Instance.DrawDelayed(
                delegate
                    {
                        Utils.DrawCircleFilled(
                            x,
                            position.Y + MenuSettings.ContainerHeight / 1.5f + 3,
                            4,
                            0,
                            Utils.CircleType.Full,
                            true,
                            32,
                            MenuSettings.ContainerSelectedColor);
                    });

            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                this.Component.DisplayName, 
                (int)(position.X + MenuSettings.ContainerTextOffset),
                (int)(position.Y + (centeredY - position.Y) / 2),
                MenuSettings.TextColor);

            var measureText = MenuSettings.Font.MeasureText(
                null, 
                this.Component.SValue.ToString(CultureInfo.InvariantCulture), 
                0);
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite, 
                this.Component.SValue.ToString(CultureInfo.InvariantCulture), 
                (int)(position.X +   this.Component.MenuWidth - measureText.Width - Offset - MenuSettings.ContainerHeight),
                (int)(position.Y + (centeredY - position.Y) / 2),
                MenuSettings.TextColor);

            Line.Width = 2;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(position.X + Offset, position.Y + MenuSettings.ContainerHeight / 1.5f + 3),
                        new Vector2(maxX, position.Y + MenuSettings.ContainerHeight / 1.5f + 3)
                    },
                new ColorBGRA(193, 189, 188, 255));
            Line.End();

            Line.Width = 2;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(position.X + Offset, position.Y + MenuSettings.ContainerHeight / 1.5f + 3),
                        new Vector2(x, position.Y + MenuSettings.ContainerHeight / 1.5f + 3)
                    },
                MenuSettings.ContainerSelectedColor);
            Line.End();

            //On / Off Button

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
                        CenteredFlags.HorizontalCenter).X - 5;

            //Left
            Utils.DrawCircle(centerX, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f, 7, 270, Utils.CircleType.Half, true, 32, MenuSettings.TextColor);

            //Right
            Utils.DrawCircle(centerX + 15, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f, 7, 90, Utils.CircleType.Half, true, 32, MenuSettings.TextColor);

            //Top
            Line.Width = 1;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(centerX, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f - 8),
                        new Vector2(centerX + 15, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f - 8)
                    },
                MenuSettings.TextColor);
            Line.End();

            //Bot
            Line.Width = 1;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(centerX, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f + 7),
                        new Vector2(centerX + 15, this.Component.Position.Y + MenuSettings.ContainerHeight / 2f + 7)
                    },
                MenuSettings.TextColor);
            Line.End();

            //FullCircle
            Utils.DrawCircleFilled(this.Component.BValue ? centerX + 14 : centerX + 1,
                this.Component.Position.Y + MenuSettings.ContainerHeight / 2f, 6, 0, Utils.CircleType.Full, true, 32,
                this.Component.BValue ? MenuSettings.ContainerSelectedColor : MenuSettings.TextColor);
        }

        /// <summary>
        ///     Processes windows messages
        /// </summary>
        /// <param name="args">event data</param>
        public override void OnWndProc(WindowsKeys args)
        {
            if (!this.Component.Visible)
            {
                return;
            }

            if (args.Msg == WindowsMessages.MOUSEMOVE && this.Component.Interacting)
            {
                this.CalculateNewValue(this.Component, args);
            }
            else if (args.Msg == WindowsMessages.LBUTTONDOWN)
            {
                var rect = this.ButtonBoundaries(this.Component);

                if (args.Cursor.IsUnderRectangle(rect.X, rect.Y, rect.Width, rect.Height))
                {
                    this.Component.BValue = !this.Component.BValue;
                    this.Component.FireEvent();
                }

                if (!this.Component.Interacting)
                {
                    var container = this.SliderBoundaries(this.Component);

                    if (args.Cursor.IsUnderRectangle(container.X, container.Y, container.Width, container.Height))
                    {
                        this.Component.Interacting = true;
                        this.CalculateNewValue(this.Component, args);
                    }
                }
            }
            else if (args.Msg == WindowsMessages.LBUTTONUP)
            {
                this.Component.Interacting = false;
            }
        }

        /// <summary>
        ///     Calculates the width of this component
        /// </summary>
        /// <returns>
        ///     The width.
        /// </returns>
        public override int Width()
        {
            return ColoredUtilities.CalcWidthItem(this.Component) + 100;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Calculate the new value based onto the cursor position.
        /// </summary>
        /// <param name="component">menu component</param>
        /// <param name="args">
        ///     <see cref="WindowsKeys" /> data
        /// </param>
        private void CalculateNewValue(MenuSliderButton component, WindowsKeys args)
        {
            var newValue =
                (int)
                Math.Round(
                    component.MinValue
                    + ((args.Cursor.X - component.Position.X - Offset) * (component.MaxValue - component.MinValue))
                    / (component.MenuWidth - Offset * 2 - MenuSettings.ContainerHeight));
            if (newValue < component.MinValue)
            {
                newValue = component.MinValue;
            }
            else if (newValue > component.MaxValue)
            {
                newValue = component.MaxValue;
            }

            if (newValue != component.Value)
            {
                component.SValue = newValue;
                component.FireEvent();
            }
        }

        #endregion
    }
}