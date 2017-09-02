using System.Drawing;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Utilities for DonkeyScroller
    /// </summary>
    public static class DonkeyScrollerUtils
    {
        /// <summary>
        /// Creates a string that is encoded as a doubleclickable text
        /// </summary>
        /// <param name="data">Anything you want, json or custom formatted data, as long as it is a string</param>
        /// <param name="text">The text to display in the chat view</param>
        /// <returns>A text formatted as a interactive text based on <paramref name="data"/> and <paramref name="text"/></returns>
        public static string CreateInteractiveText(string data, string text)
        {
            return $@"{ControlChar.InteractiveTextStart}{data}{ControlChar.InteractiveTextSplit}{text}{ControlChar.InteractiveTextEnd}";
        }
        /// <summary>
        /// Creates a string that contains color information in hex format
        /// </summary>
        /// <param name="foreground">Foreground color</param>
        /// <param name="background">Background color</param>
        /// <returns>A hex color string based on <paramref name="foreground"/> and <paramref name="background"/></returns>
        public static string CreateHexColorString(Color foreground, Color? background = null)
        {
            string str = ColorTranslator.ToHtml(Color.FromArgb(foreground.ToArgb())).Substring(1);
            if (background.HasValue)
                str += ColorTranslator.ToHtml(Color.FromArgb(background.Value.ToArgb())).Substring(1);
            else
                str += "XXXXXX";
            return $@"{ControlChar.HexColor}{str}";
        }
    }
}