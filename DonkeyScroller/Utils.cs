using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Internal utils
    /// </summary>
    internal static class Utils
    {
        /// <summary>
        /// Gets the Font Style for Char
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static FontStyle GetFontStyle(Char c)
        {
            FontStyle fontStyle = FontStyle.Regular;
            if (c.Bold)
                fontStyle |= FontStyle.Bold;
            if (c.Underline)
                fontStyle |= FontStyle.Underline;
            return fontStyle;
        }
        /// <summary>
        /// Parses colors from a string
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <param name="i">Index in text where the color is</param>
        /// <param name="colorForeground">Foreground color index</param>
        /// <param name="colorBackground">Background color index</param>
        public static void ParseIRCColors(string text, ref int i, out int colorForeground, out int colorBackground)
        {
            text = text.Substring(i + 1);

            if (text.Length > 0 && System.Char.IsDigit(text[0])) // are we even setting a color?
            {
                int bg = (int)MircColorCode.None;
                int fg;
                if (text.Length > 1 && System.Char.IsDigit(text[1]))
                {
                    fg = Int32.Parse(text.Substring(0, 2));
                    text = text.Substring(2);
                    i += 2;
                }
                else
                {
                    fg = Int32.Parse(text.Substring(0, 1));
                    text = text.Substring(1);
                    i += 1;
                }
                if (text.Length > 0 && text[0] == ',')
                {
                    i += 1; // inc 1 for the comma ... note the else clause at the end
                    text = text.Substring(1);
                    if (text.Length > 1 && System.Char.IsDigit(text[1]))
                    {
                        bg = Int32.Parse(text.Substring(0, 2));
                        i += 2;
                    }
                    else if (System.Char.IsDigit(text[0]))
                    {
                        bg = Int32.Parse(text.Substring(0, 1));
                        i += 1;
                    }
                    else // no background digits found, dec 1 for the comma
                        i -= 1;

                }

                colorForeground = fg;
                colorBackground = bg;

            }
            else
            {
                colorForeground = (int)MircColorCode.Black;
                colorBackground = (int)MircColorCode.None;
            }

        }

        /// <summary>
        /// Parses colors from a string containing hex values
        /// </summary>
        /// <param name="text">The text to parse from</param>
        /// <param name="i">Index where the color is in the text</param>
        /// <param name="colorForeground">The foreground color index</param>
        /// <param name="colorBackground">The background color index</param>
        public static void ParseHexColors(string text, ref int i, out int colorForeground, out int colorBackground)
        {
            text = text.Substring(i + 1);

            string hexForeground = text.Substring(0, 6);
            string hexBackground = text.Substring(6, 6);

            Color clrForeGround = ColorTranslator.FromHtml("#" + hexForeground);

            if (!hexBackground.ToUpper().Contains("X"))
            {
                Color clrBackGround = ColorTranslator.FromHtml("#" + hexBackground);
                colorBackground = IRCColorTable.Get(clrBackGround, 0, true);
            }
            else
            {
                colorBackground = (int)MircColorCode.None;
            }
            colorForeground = IRCColorTable.Get(clrForeGround, 1, true);
            i += 12;
        }

        /// <summary>
        /// Executes the specified delegate on the thread that owns the control's underlying window handle.
        /// </summary>
        /// <param name="control">The control whose window handle the delegate should be invoked on.</param>
        /// <param name="method">A delegate method to be called in the control's thread context.</param>
        public static void InvokeIfRequired(Control control, Action method)
        {
            if (control.IsDisposed)
                return;
            try
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(method);
                }
                else
                {
                    method();
                }
            }
            catch (InvalidAsynchronousStateException)
            {
                // HACK Do nothing??
            }
        }

        /// <summary>
        /// Executes the specified delegate on the thread that owns the control's underlying window handle, returning a
        /// value.
        /// </summary>
        /// <param name="control">The control whose window handle the delegate should be invoked on.</param>
        /// <param name="method">A delegate method to be called in the control's thread context and that returns a value.</param>
        /// <returns>The return value from the delegate being invoked.</returns>
        public static TResult InvokeIfRequired<TResult>(Control control, Func<TResult> method)
        {
            if (control.InvokeRequired)
            {
                return (TResult)control.Invoke(method);
            }
            return method();
        }
        /// <summary>
        /// Checks whether or not a font is monospaced
        /// </summary>
        /// <param name="font">The font to check</param>
        /// <returns></returns>
        public static bool IsMonoSpaceFont(Font font)
        {
            return GetCharSize(font, 'M') == GetCharSize(font, '.');
        }
        /// <summary>
        /// Gets the size of a char using specified Font
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="c">The char to get the size for</param>
        /// <returns></returns>
        internal static SizeF GetCharSize(Font font, char c)
        {
            Size sz2 = TextRenderer.MeasureText("<" + c + ">", font);
            Size sz3 = TextRenderer.MeasureText("<>", font);

            return new SizeF(sz2.Width - sz3.Width + 1, /*sz2.Height*/font.Height);
        }
    }
}