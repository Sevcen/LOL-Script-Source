﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColoredTextures.cs" company="EnsoulSharp">
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

namespace EnsoulSharp.SDK.Core.UI.IMenu.Skins.Colored
{
    using System.Drawing;

    using EnsoulSharp.SDK.Properties;
    using SharpDX.Direct3D9;

    internal enum ColoredTexture
    {
        Dragging,
    }

    internal class ColoredTextures
    {

        private readonly Dictionary<ColoredTexture, ColoredTextureWrapper> textures = new Dictionary<ColoredTexture, ColoredTextureWrapper>();

        public static readonly ColoredTextures Instance = new ColoredTextures();

        private ColoredTextures()
        {
            this.textures[ColoredTexture.Dragging] = BuildTexture(Resources.cursor_drag, 16, 16);
        }

        ~ColoredTextures()
        {
            foreach (var entry in this.textures.Where(entry => !entry.Value.Texture.IsDisposed)) {
                entry.Value.Texture.Dispose();
            }
        }

        public ColoredTextureWrapper this[ColoredTexture textureType]
        {
            get
            {
                return this.textures[textureType];
            }
        }

        private ColoredTextureWrapper BuildTexture(Image bmp, int height, int width)
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
            return new ColoredTextureWrapper(texture, width, height);
        }

        public ColoredTextureWrapper AddTexture(Image bmp, int width, int height, ColoredTexture textureType)
        {
            this.textures[textureType] = BuildTexture(bmp, height, width);
            return this.textures[textureType];
        }
        
    }

    internal class ColoredTextureWrapper
    {
        public Texture Texture { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public ColoredTextureWrapper(Texture texture, int width, int height)
        {
            this.Texture = texture;
            this.Width = width;
            this.Height = height;
        }
        
    }
}
