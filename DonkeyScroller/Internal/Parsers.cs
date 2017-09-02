using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using JaniPellikka.Windows.Forms.Content;
using Char = JaniPellikka.Windows.Forms.Content.Char;

namespace JaniPellikka.Windows.Forms.Internal
{
    /// <summary>
    /// Internal parsers
    /// </summary>
    internal static class Parsers
    {
        /// <summary>
        /// Regex to parse URLs
        /// </summary>
        private static readonly Regex UrlRegex = new Regex(@"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=\S+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])", RegexOptions.IgnoreCase);
        /// <summary>
        /// Removes trailing control codes from text.
        /// 
        /// If the text ends with, say, bold, there is no reason to keep it as there is nothing to display, thus, we trim it.
        /// </summary>
        /// 
        /// <param name="text">The text to trim</param>
        /// <returns>Trimmed text</returns>
        private static string TrimLineTextRightSide(string text)
        {
            if (text.Length == 0)
                return "";

            text = TrimLineTextRightSideSingleControlChars(text);

            text = TrimLineTextRightSideColorCode(text);

            return text;
        }
        /// <summary>
        /// Trims away color code chars and color numbers from the end of the string
        /// </summary>
        /// <param name="text">The text to trim</param>
        /// <returns>Trimmed text</returns>
        private static string TrimLineTextRightSideColorCode(string text)
        {
            if (char.IsDigit(text.LastOrDefault()))
            {
                // Get index of last color control char
                int colorControlCharIndex = text.LastIndexOf(ControlChar.Color.ToString(), StringComparison.InvariantCultureIgnoreCase);

                // There is no color control char, return text as it is
                if (colorControlCharIndex == -1)
                    return text;

                // Iterate the chars from color code index to the end
                for (int j = colorControlCharIndex; j < text.Length; j++)
                {
                    char c = text[j];

                    // Current char is NOT a number AND NOT a comma AND NOT a color control char
                    // Return the text as it is
                    if (!char.IsDigit(c) && c != ',' && c != ControlChar.Color)
                        return text;
                }
                // All chars after the color control char were part of a color code, strip it 
                return text.Substring(0, colorControlCharIndex);
            }



            return text;
        }
        /// <summary>
        /// Trims away single control chars from the end of the string
        /// </summary>
        /// <param name="text">The text to trim</param>
        /// <returns>Trimmed text</returns>
        private static string TrimLineTextRightSideSingleControlChars(string text)
        {
            // Remember length of text
            int lenBeforeTrim = text.Length;

            // Remove any single control chars
            text = text.TrimEnd(
                ControlChar.Bold,
                ControlChar.StrikeThrough,
                ControlChar.Italic,
                ControlChar.Underline2,
                ControlChar.Underline
            );
            // Compare text length with our reminder, if length missmatches, we removed something and need to trim again
            if (lenBeforeTrim != text.Length)
                return text;
            return text;
        }
        /// <summary>
        /// Parses text to a <see cref="Line"/>
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <param name="donkeyScroller">The owner for the new <see cref="Line"/></param>
        /// <returns>New <see cref="Line"/></returns>
        public static Line ParseLineText(string text, DonkeyScroller donkeyScroller)
        {
            text = TrimLineTextRightSide(text);

            text = UrlRegex.Replace(text, ControlChar.Url + "$0" + ControlChar.Url);

            Line line = new Line(donkeyScroller);
            bool bold = false;
            bool underline = false;
            bool strikethrough = false;
            bool italic = false;
            bool markingUrl = false;
            int urlStartIndex = -1;
            Color foreColor = Color.Black;
            Color backColor = Color.Transparent;
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            CharData currentCharFormat = new CharData(donkeyScroller.Font, foreColor, backColor);
            // ReSharper enable ConditionIsAlwaysTrueOrFalse

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                switch (c)
                {
                    case ControlChar.Bold:
                        bold = !bold;
                        break;
                    case ControlChar.StrikeThrough:
                        strikethrough = !strikethrough;
                        break;
                    case ControlChar.Italic:
                        italic = !italic;
                        break;
                    case ControlChar.Underline:
                    case ControlChar.Underline2:
                        underline = !underline;
                        break;
                    case ControlChar.HexColor:
                        {
                            ParseHexColors(text, ref i, out foreColor, out backColor);
                            break;
                        }
                    case ControlChar.Color:
                        {
                            // Remember current background color
                            Color previousBackColor = backColor;

                            ParseIRCColors(text, ref i, out foreColor, out backColor);

                            // If the parsed color has NO backgroundcolor but the previous did, restore it
                            // This way we can change foreground color but keep the current background color
                            if (backColor == Color.Transparent && previousBackColor != Color.Transparent)
                                backColor = previousBackColor;

                            break;
                        }
                    case ControlChar.Url:
                        {
                            if (!markingUrl)
                            {
                                urlStartIndex = line.Count;
                            }
                            else
                            {
                                int urlEndIndex = line.Count;
                                string url = line.GetRange(urlStartIndex, urlEndIndex - urlStartIndex).Aggregate("", (current, thisChar) => current + thisChar.C);
                                line.InteractiveUrls.Add(urlStartIndex, urlEndIndex - 1, url);

                            }
                            markingUrl = !markingUrl;
                            break;
                        }
                    case ControlChar.InteractiveTextEnd:
                        {
                            // do nothing, just eat it as it was handled in the case below
                            break;
                        }
                    case ControlChar.InteractiveTextStart:
                        {
                            string interactiveData = "";
                            int interactiveStart = line.Count;
                            bool doContinue = true;
                            for (++i; i < text.Length && doContinue; i++)
                            {
                                char thisC = text[i];
                                if (thisC == ControlChar.InteractiveTextSplit)
                                {
                                    int textStart = i + 1;
                                    for (++i; i < text.Length && doContinue; i++)
                                    {
                                        thisC = text[i];
                                        if (thisC == ControlChar.InteractiveTextEnd)
                                        {
                                            string interactiveText = text.Substring(textStart, i - textStart);
                                            line.InteractiveTexts.Add(interactiveStart, interactiveStart + interactiveText.Length - 1, interactiveData, interactiveText);
                                            i = textStart - 3;
                                            doContinue = false;
                                        }
                                    }
                                }
                                else
                                {
                                    interactiveData += thisC;
                                }
                            }
                            break;
                        }

                    default:
                        FontStyle fontStyle = Utils.GetFontStyle(bold, underline, italic, strikethrough);
                        CharData newCharFormat = new CharData(new Font(donkeyScroller.Font, fontStyle), foreColor, backColor);
                        line.Add(new Char
                        {
                            C = c,
                            Data = !newCharFormat.Equals(currentCharFormat) ? newCharFormat : null
                        });
                        currentCharFormat = newCharFormat;
                        break;
                }
            }
            line.CalculateDimensions();
            return line;
        }


        /// <summary>
        /// Parses colors from a string
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <param name="i">Index in text where the color is</param>
        /// <param name="colorForeground">Foreground color index</param>
        /// <param name="colorBackground">Background color index</param>
        public static void ParseIRCColors(string text, ref int i, out Color colorForeground, out Color colorBackground)
        {

            ParseIRCColors(text, ref i, out int iForeground, out int iBackground, out int foregroundLength, out int backgroundLength);

            colorForeground = IRCColorTranslator.NumberToColor(iForeground, Color.Black);
            colorBackground = IRCColorTranslator.NumberToColor(iBackground, Color.Transparent);

            
        }
        /// <summary>
        /// Parses IRC Colors from a string
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <param name="i">The index where the color code starts</param>
        /// <param name="colorForeground">Foreground color</param>
        /// <param name="colorBackground">Background color</param>
        /// <param name="colorForegroundLength">Length of the color integer in string format</param>
        /// <param name="colorBackgroundLength">Length of the color integer in string format</param>
        public static void ParseIRCColors(string text, ref int i, out int colorForeground, out int colorBackground, out int colorForegroundLength, out int colorBackgroundLength)
        {
            colorBackgroundLength = 1;
            colorForegroundLength = 1;
            text = text.Substring(i + 1);
            colorForeground = -1;
            colorBackground = -1;
            if (text.Length > 0 && char.IsDigit(text[0])) // are we even setting a color?
            {
                int bg = -1;
                int fg;
                if (text.Length > 1 && char.IsDigit(text[1]))
                {
                    fg = int.Parse(text.Substring(0, 2));
                    text = text.Substring(2);
                    i += 2;
                    colorForegroundLength = 2;
                }
                else
                {
                    fg = int.Parse(text.Substring(0, 1));
                    text = text.Substring(1);
                    i += 1;
                }
                if (text.Length > 0 && text[0] == ',')
                {
                    i += 1; // inc 1 for the comma ... note the else clause at the end
                    text = text.Substring(1);
                    if (text.Length > 1 && char.IsDigit(text[1]))
                    {
                        bg = int.Parse(text.Substring(0, 2));
                        i += 2;
                        colorBackgroundLength = 2;
                    }
                    else if (char.IsDigit(text[0]))
                    {
                        bg = int.Parse(text.Substring(0, 1));
                        i += 1;
                    }
                    else // no background digits found, dec 1 for the comma
                        i -= 1;

                }

                colorForeground = fg;
                colorBackground = bg;

            }
        }
        /// <summary>
        /// Parses colors from a string containing hex values
        /// </summary>
        /// <param name="text">The text to parse from</param>
        /// <param name="i">Index where the color is in the text</param>
        /// <param name="colorForeground">The foreground color index</param>
        /// <param name="colorBackground">The background color index</param>
        public static void ParseHexColors(string text, ref int i, out Color colorForeground, out Color colorBackground)
        {
            text = text.Substring(i + 1);

            string hexForeground = text.Substring(0, 6);
            string hexBackground = text.Substring(6, 6);

            colorForeground = ColorTranslator.FromHtml("#" + hexForeground);

            colorBackground = !hexBackground.ToUpper().Contains("X") ?
                ColorTranslator.FromHtml("#" + hexBackground) :
                Color.Transparent;

            i += 12;
        }
        /// <summary>
        /// Parses color string at a specific index
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <param name="index">The index where the color code is</param>
        /// <returns>The color code a string</returns>
        public static string ParseColorsStringAt(string text, int index)
        {
            if (string.IsNullOrEmpty(text.Trim()))
                return "";
            if (text[0] != ControlChar.Color)
                return "";

            int i = index;
            ParseIRCColors(text, ref i, out int iForeground, out int iBackground, out int foregroundLength, out int backgroundLength);

            string colorString = ControlChar.Color.ToString();

            if (iForeground != -1)
            {


                colorString += iForeground.ToString().PadLeft(foregroundLength, '0');
                if (iBackground != -1)
                {
                    colorString += "," + iBackground.ToString().PadLeft(backgroundLength, '0');
                }
            }

            return colorString;

        }
    }
}