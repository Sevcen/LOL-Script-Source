﻿// <copyright file="DefaultSliderButton.cs" company="EnsoulSharp">
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
    using System;
    using System.Globalization;
    using Core.Utils;
    using EnsoulSharp.SDK;
    using SharpDX;
    using SharpDX.Direct3D9;
    using Values;

    /// <summary>
    ///     A default implementation of an <see cref="ADrawable{MenuSliderButton}" />
    /// </summary>
    public class DefaultSliderButton : ADrawable<MenuSliderButton>
    {
        #region Static Fields

        /// <summary>
        ///     The line.
        /// </summary>
        private static readonly Line Line = new Line(Drawing.Direct3DDevice) { GLLines = true };

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultSliderButton" /> class.
        /// </summary>
        /// <param name="component">
        ///     The menu component
        /// </param>
        public DefaultSliderButton(MenuSliderButton component)
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
                (int)component.Position.X,
                (int)component.Position.Y,
                component.MenuWidth - MenuSettings.ContainerHeight,
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
                (int)(component.Position.X + component.MenuWidth - MenuSettings.ContainerHeight),
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
        ///     Draws a <see cref="MenuSliderButton" />
        /// </summary>
        public override void Draw()
        {
            //Slider

            var position = this.Component.Position;
            var centeredY =
                (int)
                DefaultUtilities.GetContainerRectangle(this.Component)
                    .GetCenteredText(null, MenuSettings.Font, this.Component.DisplayName, CenteredFlags.VerticalCenter)
                    .Y;
            var percent = (this.Component.SValue - this.Component.MinValue)
                          / (float)(this.Component.MaxValue - this.Component.MinValue);
            var x = position.X + (percent * (this.Component.MenuWidth - MenuSettings.ContainerHeight));

            Line.Width = 2;
            Line.Begin();
            Line.Draw(
                new[] { new Vector2(x, position.Y + 1), new Vector2(x, position.Y + MenuSettings.ContainerHeight) }, 
                this.Component.Interacting ? new ColorBGRA(255, 0, 0, 255) : new ColorBGRA(50, 154, 205, 255));
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
                        new Vector2(position.X, position.Y + (MenuSettings.ContainerHeight / 2f)), 
                        new Vector2(x, position.Y + (MenuSettings.ContainerHeight / 2f))
                    }, 
                MenuSettings.HoverColor);
            Line.End();

            // On / Off Button

            Line.Width = MenuSettings.ContainerHeight;
            Line.Begin();
            Line.Draw(
                new[]
                    {
                        new Vector2(
                            (this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight)
                            + (MenuSettings.ContainerHeight / 2f),
                            this.Component.Position.Y + 1),
                        new Vector2(
                            (this.Component.Position.X + this.Component.MenuWidth - MenuSettings.ContainerHeight)
                            + (MenuSettings.ContainerHeight / 2f),
                            this.Component.Position.Y + MenuSettings.ContainerHeight)
                    },
                this.Component.BValue ? new ColorBGRA(0, 100, 0, 255) : new ColorBGRA(255, 0, 0, 255));
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
                        this.Component.BValue ? "ON" : "OFF",
                        CenteredFlags.HorizontalCenter).X;
            MenuSettings.Font.DrawText(
                MenuManager.Instance.Sprite,
                this.Component.BValue ? "ON" : "OFF",
                centerX,
                centeredY,
                MenuSettings.TextColor);
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
            return DefaultUtilities.CalcWidthItem(this.Component) + 100;
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
                    + (((args.Cursor.X - component.Position.X) * (component.MaxValue - component.MinValue))
                    / (component.MenuWidth - MenuSettings.ContainerHeight)));
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