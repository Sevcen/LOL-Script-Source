﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlueColorPicker2.cs" company="EnsoulSharp">
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
//   A custom implementation of <see cref="ADrawable{MenuColorPicker}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace EnsoulSharp.SDK.Core.UI.IMenu.Skins.Blue2
{
    using EnsoulSharp.SDK.Core.UI.IMenu.Skins.Blue;
    using EnsoulSharp.SDK.Core.UI.IMenu.Values;

    internal class BlueColorPicker2 : BlueColorPicker
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlueColorPicker2" /> class.
        /// </summary>
        /// <param name="component">
        ///     The component.
        /// </param>
        public BlueColorPicker2(MenuColor component)
            : base(component)
        {

        }

        #endregion
    }
}