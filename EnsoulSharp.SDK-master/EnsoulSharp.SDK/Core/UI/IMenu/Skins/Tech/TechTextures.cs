﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TechTextures.cs" company="EnsoulSharp">
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
//   A custom implementation of <see cref="ADrawable{MenuTexture}" />
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;

namespace EnsoulSharp.SDK.Core.UI.IMenu.Skins.Tech
{
    using System.Drawing;

    using EnsoulSharp.SDK.Properties;
    using SharpDX.Direct3D9;

    internal enum TechTexture
    {
        Dragging,
    }

    internal class TechTextures
    {

        private readonly Dictionary<TechTexture, TechTextureWrapper> textures = new Dictionary<TechTexture, TechTextureWrapper>();

        public static readonly TechTextures Instance = new TechTextures();

        private TechTextures()
        {
            this.textures[TechTexture.Dragging] = BuildTexture(Resources.cursor_drag, 16, 16);
        }

        ~TechTextures()
        {
            foreach (var entry in this.textures.Where(entry => !entry.Value.Texture.IsDisposed)) {
                entry.Value.Texture.Dispose();
            }
        }

        public TechTextureWrapper this[TechTexture textureType]
        {
            get
            {
                return this.textures[textureType];
            }
        }

        private TechTextureWrapper BuildTexture(Image bmp, int height, int width)
        {
            var resized = new Bitmap(bmp, width, height);
            var texture =  Texture.FromMemory(
                Drawing.Direct3DDevice,
                (byte[])new ImageConverter().ConvertTo(resized, typeof(byte[])),
                resized.Width,
                resized.Height,
                0,
                Usage.None,
                Format.A1,
                Pool.Managed,
                Filter.Default,
                Filter.Default,
                0);
            resized.Dispose();
            bmp.Dispose();
            return new TechTextureWrapper(texture, width, height);
        }

        public TechTextureWrapper AddTexture(Image bmp, int width, int height, TechTexture textureType)
        {
            this.textures[textureType] = BuildTexture(bmp, height, width);
            return this.textures[textureType];
        }
        
    }

    internal class TechTextureWrapper
    {
        public Texture Texture { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TechTextureWrapper(Texture texture, int width, int height)
        {
            this.Texture = texture;
            this.Width = width;
            this.Height = height;
        }
        
    }
}
