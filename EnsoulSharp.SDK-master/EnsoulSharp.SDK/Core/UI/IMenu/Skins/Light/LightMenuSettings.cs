﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LightMenuSettings.cs" company="EnsoulSharp">
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
//   Light Skin Settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

//Concept by User Vasconcellos

namespace EnsoulSharp.SDK.Core.UI.IMenu.Skins.Light
{

    using SharpDX;
    using SharpDX.Direct3D9;

    /// <summary>
    ///     Default Skin Settings.
    /// </summary>
    public class LightMenuSettings : MenuSettings
    {
        #region Static Fields

        /// <summary>
        ///     Local Caption Font.
        /// </summary>
        private static Font fontCaption;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="MenuSettings" /> class.
        ///     Default Settings Static Constructor.
        /// </summary>
        static LightMenuSettings()
        {
            RootContainerColor = new ColorBGRA(231, 231, 231, 255);
            ContainerSeparatorColor = new ColorBGRA(210, 210, 210, 255);
            ContainerSelectedColor = new ColorBGRA(67, 160, 255, 255);

            FontCaption = new Font(
                Drawing.Direct3DDevice,
                14,
                0,
                FontWeight.DoNotCare,
                0,
                true,
                FontCharacterSet.Default,
                FontPrecision.TrueType,
                FontQuality.ClearType,
                FontPitchAndFamily.DontCare | FontPitchAndFamily.Decorative | FontPitchAndFamily.Modern,
                "Tahoma");
            
            TextColor = new ColorBGRA(17, 17, 17, 255);
            TextCaptionColor = new ColorBGRA(72, 157, 248, 255);
            KeyBindColor = new ColorBGRA(67, 159, 255, 255);
            SliderColor = new ColorBGRA(163, 202, 241, 255);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the Global Caption Font.
        /// </summary>
        public static Font FontCaption
        {
            get
            {
                return fontCaption;
            }

            set
            {
                fontCaption = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Global Text Caption Color.
        /// </summary>
        public static ColorBGRA TextCaptionColor { get; set; }

        /// <summary>
        ///     Gets or sets the Global KeyBind Color.
        /// </summary>
        public static ColorBGRA KeyBindColor { get; set; }

        /// <summary>
        ///     Gets or sets the Global Slider Color.
        /// </summary>
        public static ColorBGRA SliderColor { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Used to load the menu settings.
        /// </summary>
        public static void LoadSettings()
        {
            
        }

        #endregion
    }
}