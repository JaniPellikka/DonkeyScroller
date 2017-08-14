using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Static class for IRC color handling
    /// </summary>
    public static class IRCColorTable
    {
        /// <summary>
        /// Dictionary of indexes and colors as Mirc uses them (VGA/XP/ANSI color table)
        /// </summary>
        private static readonly Dictionary<int, Color> Table = new Dictionary<int, Color>
        {
            {0,Color.White },
            {1,ControlPaint.Light(Color.Black) },
            {2,Color.Navy },
            {3,Color.Green },
            {4,Color.Red },
            {5,Color.Brown },
            {6,Color.Purple },
            {7,Color.DarkOrange },
            {8,Color.Yellow },
            {9,Color.LimeGreen },
            {10,Color.DarkCyan },
            {11,Color.Cyan },
            {12,Color.Blue },
            {13,Color.DeepPink },
            {14,Color.Gray },
            {15,Color.LightGray },
        };
        /// <summary>
        /// Gets a color index based on color
        /// </summary>
        /// <param name="color">Color to get the index for</param>
        /// <param name="defaultValue">If <seealso cref="addNew"/> is false, this index will be used if no color was found</param>
        /// <param name="addNew">If set to true, a color is added to the table for future references</param>
        /// <returns></returns>
        public static int Get(Color color, int defaultValue = 1, bool addNew = false)
        {

            foreach (KeyValuePair<int, Color> kvp in Table.Where(kvp => kvp.Value == color))
            {
                return kvp.Key;
            }
            if (addNew)
            {
                int newIndex = Table.Select(kvp => kvp.Key).Concat(new[] { 0 }).Max() + 1;
                Table.Add(newIndex, color);
                return newIndex;
            }
            return defaultValue;
        }
        /// <summary>
        /// Gets a Color by index
        /// </summary>
        /// <param name="index">Index of color to get</param>
        /// <param name="defaultColor">Color to return if index is out of bounds. If this is null. Black is returned</param>
        /// <returns></returns>
        public static Color Get(int index, Color? defaultColor = null)
        {
            return Table.ContainsKey(index) ? Table[index] : defaultColor ?? Color.Black;
        }
        /// <summary>
        /// Gets a formatted string for color that you can add to the chat view
        /// </summary>
        /// <param name="foreColor">Color for the foreground</param>
        /// <param name="backColor">Color for the background</param>
        /// <param name="defaultForeColor">Default color index if <seealso cref="foreColor"/> was not found</param>
        /// <param name="defaultBackColor">Default color index if <seealso cref="backColor"/> was not found</param>
        /// <param name="addNew">If true, colors that are not found are added to the palette for future reference</param>
        /// <returns></returns>
        public static string GetColorString(Color foreColor, Color backColor, int defaultForeColor = 1, int defaultBackColor = 0, bool addNew = false)
        {
            int foreColorIndex = Get(foreColor, defaultForeColor, addNew);
            int backColorIndex = Get(backColor, defaultBackColor, addNew);
            return $@"{IRCControlChar.Color}{foreColorIndex},{backColorIndex}";
        }
        /// <summary>
        /// Gets a formatted string for color that you can add to the chat view
        /// </summary>
        /// <param name="foreColor">Color for the foreground</param>
        /// <param name="defaultValue">Default color index if <seealso cref="foreColor"/> was not found</param>
        /// <param name="addNew">If true, color is added to the palette for future reference</param>
        /// <returns></returns>
        public static string GetColorString(Color foreColor, int defaultValue = 1, bool addNew = false)
        {
            int foreColorIndex = Get(foreColor, defaultValue, addNew);
            return $@"{IRCControlChar.Color}{foreColorIndex}";
        }
        /// <summary>
        /// Gets a formetted string for color that you can add to the chat view
        /// </summary>
        /// <param name="foreColor">Foreground color index</param>
        /// <param name="backColor">Background color index</param>
        /// <returns></returns>
        public static string GetColorString(int foreColor, int backColor)
        {
            return $@"{IRCControlChar.Color}{foreColor},{backColor}";
        }
        /// <summary>
        /// Gets a formetted string for color that you can add to the chat view
        /// </summary>
        /// <param name="foreColor">Foreground color index</param>
        /// <returns></returns>
        public static string GetColorString(int foreColor)
        {
            return $@"{IRCControlChar.Color}{foreColor}";
        }

        /// <summary>
        /// Gets a formetted string for color that you can add to the chat view
        /// </summary>
        /// <param name="foreColor">Foreground MircColorCode</param>
        /// <param name="backColor">Background MircColorCode</param>
        /// <returns></returns>
        public static string GetColorString(MircColorCode foreColor, MircColorCode backColor)
        {
            if (foreColor == MircColorCode.None)
                return "";
            return $@"{IRCControlChar.Color}{(int)foreColor},{(int)backColor}";
        }
        /// <summary>
        /// Gets a formetted string for color that you can add to the chat view
        /// </summary>
        /// <param name="foreColor">Foreground MircColorCode</param>
        /// <returns></returns>
        public static string GetColorString(MircColorCode foreColor)
        {
            if (foreColor == MircColorCode.None)
                return "";
            return $@"{IRCControlChar.Color}{(int)foreColor}";
        }
    }
}