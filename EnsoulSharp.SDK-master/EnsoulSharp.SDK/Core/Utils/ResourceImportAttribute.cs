// <copyright file="ResourceImportAttribute.cs" company="EnsoulSharp">
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

namespace EnsoulSharp.SDK.Core.Utils
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    internal class ResourceImportAttribute : Attribute
    {
        public ResourceImportAttribute()
        {
        }

        public ResourceImportAttribute(string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            this.File = file;
        }

        public string File { get; set; }

        public Type Filter { get; set; }
    }
}