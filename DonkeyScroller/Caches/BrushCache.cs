﻿using System.Collections.Generic;
using System.Drawing;

namespace JaniPellikka.Windows.Forms.Caches
{
    /// <summary>
    /// Cache for colored Brushes
    /// </summary>
    internal class BrushCache
    {
        /// <summary>
        /// Dictionary of colors and brushes
        /// </summary>
        private readonly Dictionary<Color, Brush> _brushDictionary = new Dictionary<Color, Brush>();
        /// <summary>
        /// Gets a Brush based on color. If this color has been fetched before, we get one from the cache
        /// </summary>
        /// <param name="color">The color for the brush</param>
        /// <returns>Brush based on <paramref name="color"/></returns>
        public Brush Get(Color color)
        {
            if (!_brushDictionary.ContainsKey(color))
                _brushDictionary.Add(color, new SolidBrush(color));
            return _brushDictionary[color];
        }
    }
}