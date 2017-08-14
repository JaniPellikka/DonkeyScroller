using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// A single line in the chat view
    /// </summary>
    internal class Line : List<Char>
    {
        /// <summary>
        /// Reference to the DonkeyScroller that owns this Line
        /// </summary>
        private readonly DonkeyScroller _donkeyScroller;

        /// <summary>
        /// Char indexes where the text should wrap
        /// </summary>
        private readonly List<int> _wrapIndexes = new List<int>();
        /// <summary>
        /// Width of this Line in pixels (including wrapping)
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Height of this Line in pixels (including wrapping)
        /// </summary>
        public int Height => _donkeyScroller.CharHeight * RowsHigh;
        /// <summary>
        /// Number of rows this Line occupies
        /// </summary>
        public int RowsHigh { get; private set; }
        /// <summary>
        /// URL Segments for this line
        /// </summary>
        public UrlSegmentCollection UrlSegments = new UrlSegmentCollection();
        /// <summary>
        /// Interactive text segments for this line
        /// </summary>
        public InteractiveSegmentCollection InteractiveSegments = new InteractiveSegmentCollection();
        /// <summary>
        /// The text in this line with all the control codes stripped
        /// TODO Make sure InteractiveSegment is stripped correctly
        /// </summary>
        public string Text
        {
            get
            {
                string t = "";
                return this.Aggregate(t, (current, c) => current + c.C);
            }
        }
        /// <summary>
        /// Instantiates new instance
        /// </summary>
        /// <param name="donkeyScroller">Parent DonkeyScroller</param>
        private Line(DonkeyScroller donkeyScroller)
        {
            _donkeyScroller = donkeyScroller;
        }
        /// <summary>
        /// Calculates the height and width of this line.
        /// Also creates wrap indexes for wrapping the line
        /// </summary>
        public void CalculateDimensions()
        {
            _wrapIndexes.Clear();

            int segmentLength = 0;

            int wrapIndex = 0;
            RowsHigh = 1;

            for (int i = 0; i < Count; i++)
            {
                Char c = this[i];
                if (!char.IsLetterOrDigit(c.C) && c.C != '_' && c.C != '\'' && c.C != '\xa0')
                    wrapIndex = Math.Min(i + 1, Count - 1);
                segmentLength++;

                if (segmentLength >= _donkeyScroller.MaxWidthInChars)
                {

                    if (wrapIndex == 0 || (_wrapIndexes.Count > 0 && wrapIndex == _wrapIndexes[_wrapIndexes.Count - 1]))
                        wrapIndex = i + 1;
                    _wrapIndexes.Add(wrapIndex);
                    Width = Math.Max(Width, segmentLength);
                    segmentLength = 1 + i - wrapIndex;
                    RowsHigh++;
                }
            }

        }
        /// <summary>
        /// Draws the line
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="x">X pixel position</param>
        /// <param name="y">Y pixel position</param>
        /// <param name="drawRow">Row index</param>
        /// <param name="markedChars">List of marked chars</param>
        /// <returns></returns>
        public int Draw(Graphics graphics, float x, int y, ref int drawRow, List<char> markedChars)
        {

            y -= Height;
#if DEBUG
            //   graphics.DrawLine(Pens.Gray,0,y,_donkeyScroller.TextAreaRectangle.Width,y);
#endif
            int yReturn = y;
            float x2 = x;
            int y2 = y;
            int charIndex = 0;
            int rowIndex = drawRow + RowsHigh;
            int rowCharIndex = 0;
            foreach (Char c in this)
            {
                Brush brush = null;

                if (IsInSelection(rowCharIndex, rowIndex))
                    brush = _donkeyScroller.BrushCache.Get(SystemColors.Highlight);
                else if (c.BackColor != (int)MircColorCode.None)
                    brush = _donkeyScroller.BrushCache.Get(IRCColorTable.Get(c.BackColor));

                if (brush != null)
                {
                    graphics.SmoothingMode = SmoothingMode.None;
                    //graphics.FillRectangle(brush, x2 + _donkeyScroller.CharWidth/3f, y2, _donkeyScroller.CharWidth + 2, _donkeyScroller.CharHeight);
                    graphics.FillRectangle(brush, x2, y2, _donkeyScroller.CharWidth, _donkeyScroller.CharHeight);
                }

                AdvanceCoords(ref x2, ref y2, ref charIndex, ref rowIndex, ref rowCharIndex);
            }
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            rowIndex = drawRow + RowsHigh;
            charIndex = 0;
            rowCharIndex = 0;
            foreach (Char c in this)
            {
                FontStyle fontStyle = Utils.GetFontStyle(c);
                Font font = _donkeyScroller.FontCache.Get(fontStyle);
                Brush brush;

                if (IsInSelection(rowCharIndex, rowIndex))
                {
                    brush = _donkeyScroller.BrushCache.Get(SystemColors.HighlightText);
                    markedChars.Add(c.C);
                }
                else
                {
                    // If the char is bold, make it darker to enhance the visual
                    brush = _donkeyScroller.BrushCache.Get(
                        c.Bold ?
                            ControlPaint.Dark(IRCColorTable.Get(c.ForeColor), 0.05f) :
                            IRCColorTable.Get(c.ForeColor)
                    );
                }
                graphics.DrawString(c.C.ToString(), font, brush, new Rectangle((int)x - 50 + _donkeyScroller.CharWidth / 2, y, 100, _donkeyScroller.CharHeight), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                AdvanceCoords(ref x, ref y, ref charIndex, ref rowIndex, ref rowCharIndex, markedChars);
            }
            drawRow += RowsHigh - 1;
            return yReturn;
        }
        /// <summary>
        /// Checks whether or not char and row index are in selection
        /// </summary>
        /// <param name="charIndex">Char index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns></returns>
        private bool IsInSelection(int charIndex, int rowIndex)
        {
            Selection selection = _donkeyScroller.Selection;
            if (selection == null)
                return false;

            bool inside = selection.Inside(rowIndex, charIndex);

            return inside;
        }
        /// <summary>
        /// Advances the drawing coordinates
        /// </summary>
        /// <param name="x">Pixel x position</param>
        /// <param name="y">Pixel y position</param>
        /// <param name="charIndex">Char index</param>
        /// <param name="rowIndex">Row index</param>
        /// <param name="rowCharIndex">RowChar Index</param>
        /// <param name="markedChars">List of marked chars</param>
        private void AdvanceCoords(ref float x, ref int y, ref int charIndex, ref int rowIndex, ref int rowCharIndex, List<char> markedChars = null)
        {
            x += _donkeyScroller.CharWidth;
            rowCharIndex++;
            if (_wrapIndexes.Contains(++charIndex))
            {
                if (markedChars != null)
                {
                    markedChars.Add('\r');
                    markedChars.Add('\n');
                }

                rowCharIndex = 0;
                rowIndex--;
                y += _donkeyScroller.CharHeight;
                x = _donkeyScroller.TextAreaRectangle.Left;
            }
        }
        
        /// <summary>
        /// Parses a line
        /// </summary>
        /// <param name="text">The text to parse</param>
        /// <param name="donkeyScroller">Parent DonkeyScroller</param>
        /// <returns></returns>
        public static Line Parse(string text, DonkeyScroller donkeyScroller)
        {
            Line line = new Line(donkeyScroller);

            int foreColor = (int)MircColorCode.Black;
            int backColor = (int)MircColorCode.None;
            bool bold = false;
            bool underline = false;
            bool markingUrl = false;
            int urlStart = -1;


            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                switch (c)
                {
                    case IRCControlChar.InteractiveEnd:
                    {
                        // do nothing, just eat it as it was handled in the case below
                        break;
                    }
                    case IRCControlChar.InteractiveStart:
                    {
                        string interactiveID = "";
                        int interactiveStart = line.Count;
                        bool doContinue = true;
                        for (++i; i < text.Length && doContinue; i++)
                        {
                            char thisC = text[i];
                            if (thisC == IRCControlChar.InteractiveSplit)
                            {
                                int textStart = i + 1;
                                for (++i; i < text.Length && doContinue; i++)
                                {
                                    thisC = text[i];
                                    if (thisC == IRCControlChar.InteractiveEnd)
                                    {
                                        string interactiveText = text.Substring(textStart, i - textStart);
                                        line.InteractiveSegments.Add(interactiveStart, interactiveStart + interactiveText.Length - 1, interactiveID, interactiveText);
                                        i = textStart - 3;
                                        doContinue = false;
                                    }
                                }
                            }
                            else
                            {
                                interactiveID += thisC;
                            }
                        }
                        break;
                    }
                    case IRCControlChar.Url:
                    {
                        if (!markingUrl)
                        {
                            urlStart = line.Count;
                        }
                        else
                        {
                            int urlEnd = line.Count;
                            string url = line.GetRange(urlStart, urlEnd - urlStart).Aggregate("", (current, thisChar) => current + thisChar.C);
                            line.UrlSegments.Add(urlStart, urlEnd - 1, url);

                        }
                        markingUrl = !markingUrl;
                        break;
                    }
                    case IRCControlChar.HexColor:
                    {
                        Utils.ParseHexColors(text, ref i, out foreColor, out backColor);
                        break;
                    }
                    case IRCControlChar.Color:
                    {
                        Utils.ParseIRCColors(text, ref i, out foreColor, out backColor);
                        break;
                    }
                    case IRCControlChar.Bold:
                    {
                        bold = !bold;
                        break;
                    }
                    case IRCControlChar.Underline:
                    case IRCControlChar.Underline2:
                    {
                        underline = !underline;
                        break;
                    }
                    default:
                    {
                        if (markingUrl && foreColor == (int)MircColorCode.Black && backColor == (int)MircColorCode.None)
                        {
                            line.Add(new Char { BackColor = backColor, ForeColor = (int)MircColorCode.Blue, C = c, Bold = bold, Underline = true });
                        }
                        else
                            line.Add(new Char { BackColor = backColor, ForeColor = foreColor, C = c, Bold = bold, Underline = underline });
                        break;
                    }
                }
            }
            line.CalculateDimensions();
            return line;
        }
    }
}