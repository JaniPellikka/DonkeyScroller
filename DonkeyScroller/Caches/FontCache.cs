using System.Collections.Generic;
using System.Drawing;

namespace JaniPellikka.Windows.Forms.Caches
{
    /// <summary>
    /// Font cache for different FontStyles
    /// </summary>
    internal class FontCache
    {
        /// <summary>
        /// Base font for this cache
        /// </summary>
        private readonly Font _baseFont;

        /// <summary>
        /// Dictionary of fonts based on FontStyle
        /// </summary>
        private readonly Dictionary<FontStyle, Font> _fontDictionary = new Dictionary<FontStyle, Font>();

        /// <summary>
        /// Instantiates new instance
        /// </summary>
        /// <param name="baseFont">The font to be used</param>
        public FontCache(Font baseFont)
        {
            _baseFont = baseFont;
        }

        /// <summary>
        /// Gets a font based on FontStyle. If fetched earlier, a local cached font is returned
        /// </summary>
        /// <param name="fontStyle"></param>
        /// <returns>Font based on <paramref name="fontStyle"/> and the <see cref="_baseFont"/></returns>
        public Font Get(FontStyle fontStyle)
        {
            if (!_fontDictionary.ContainsKey(fontStyle))
            {
                _fontDictionary.Add(fontStyle, new Font(_baseFont, fontStyle));
            }
            return _fontDictionary[fontStyle];
        }
    }
}