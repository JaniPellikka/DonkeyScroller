using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Color translator
    /// </summary>
    public static class IRCColorTranslator
    {
        /// <summary>
        /// List of standard IRC colors
        /// </summary>
        private static readonly List<Color> ColorsTable = new List<Color>
        {
            Color.FromArgb(255,255,255),
            Color.FromArgb(0,0,0),
            Color.FromArgb(0,0,127),
            Color.FromArgb(0,147,0),
            Color.FromArgb(255,0,0),
            Color.FromArgb(127,0,0),
            Color.FromArgb(156,0,156),
            Color.FromArgb(252,127,0),
            Color.FromArgb(255,255,0),
            Color.FromArgb(0,252,0),
            Color.FromArgb(0,147,147),
            Color.FromArgb(0,255,255),
            Color.FromArgb(0,0,252),
            Color.FromArgb(255,0,255),
            Color.FromArgb(127,127,127),
            Color.FromArgb(210,210,210)
        };
        /// <summary>
        /// Array of colors in the IRC index based palette
        /// </summary>
        public static Color[] Colors => ColorsTable.ToArray();
        /// <summary>
        /// Converts IRC index based color to <see cref="Color"/>
        /// </summary>
        /// <param name="number"></param>
        /// <param name="defaultColor"></param>
        /// <returns>Color based on the <paramref name="number"/> or <paramref name="defaultColor"/> if index is out of bounds</returns>
        public static Color NumberToColor(int number, Color? defaultColor = null)
        {
            if (number >= 0 && number < ColorsTable.Count)
                return ColorsTable[number];

            return defaultColor ?? Color.Black;
        }
        /// <summary>
        /// Converts <see cref="Color"/> or IRC color index
        /// </summary>
        /// <param name="color">Color to convert</param>
        /// <param name="defaultIndex">Default index to return of color is not a part of the IRC color table</param>
        /// <returns>Index of the color or <paramref name="defaultIndex"/> if the color is not indexed</returns>
        public static int ColorToNumber(Color color, int defaultIndex = -1)
        {
            int i = ColorsTable.IndexOf(Color.FromArgb(color.A, color.R,color.G,color.B));

            return i == -1 ? defaultIndex : i;
        }
    }
}