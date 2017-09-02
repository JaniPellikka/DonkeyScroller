using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace JaniPellikka.Windows.Forms.Content
{
    /// <summary>
    /// Renderer for a line
    /// </summary>
    internal class LineRenderer
    {
        /// <summary>
        /// Reference to the <see cref="DonkeyScroller"/>
        /// </summary>
        private readonly DonkeyScroller _donkeyScroller;
        /// <summary>
        /// Instance
        /// </summary>
        /// <param name="donkeyScroller">The owner</param>
        public LineRenderer(DonkeyScroller donkeyScroller)
        {
            _donkeyScroller = donkeyScroller;
        }
        /// <summary>
        /// Draws a line
        /// </summary>
        /// <param name="line">The line to draw</param>
        /// <param name="graphics">Graphics object</param>
        /// <param name="x">The X coordinate where to draw the line</param>
        /// <param name="y">The Y coordinate where to draw the line</param>
        /// <param name="drawRow">Reference to the row being drawn</param>
        /// <param name="markedChars">List of marked chars</param>
        /// <returns>Updated Y coordinate</returns>
        public int Draw(Line line, Graphics graphics, ref float x, int y, ref int drawRow, List<char> markedChars)
        {
            y -= line.Height;
            

            int yReturn = y;
            int charIndex = 0;
            int rowIndex = drawRow + line.RowsHigh;
            int rowCharIndex = 0;
            DrawBackground(line, graphics, x, y, rowCharIndex, rowIndex, charIndex);
            DrawIndent(line, graphics, y);
            DrawIcon(line, graphics, y);
            DrawSeparator(graphics, y);
            DrawText(line, graphics, ref x, y, drawRow, markedChars);
            drawRow += line.RowsHigh - 1;

            return yReturn;
        }
        /// <summary>
        /// Draws separator line if is enabled in <see cref="DonkeyScroller"/>
        /// </summary>
        /// <param name="graphics">Graphics Object</param>
        /// <param name="y">Y Coordinate where to draw the line</param>
        private void DrawSeparator(Graphics graphics, int y)
        {
            if (!_donkeyScroller.ShowLineSeparator)
                return;

            using (Pen pen = new Pen(_donkeyScroller.LineSeparatorColor))
            {
                graphics.DrawLine(pen, 0, y, _donkeyScroller.Width, y);
            }
        }
        /// <summary>
        /// Draw icon for the line if enabled in <see cref="DonkeyScroller"/>
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="graphics">Graphics object</param>
        /// <param name="y">Y Coordinate</param>
        private void DrawIcon(Line line, Graphics graphics, int y)
        {
            if (!_donkeyScroller.ShowLineIcons || line.IconImage==null)
                return;
            int xOffset = _donkeyScroller.ShowLineIndentColors ? _donkeyScroller.CharWidth : 0;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            
            graphics.DrawImage(line.IconImage,new Rectangle(xOffset,y,_donkeyScroller.CharHeight,_donkeyScroller.CharHeight));
        }
        /// <summary>
        /// Draw line indent color if enabled in <see cref="DonkeyScroller"/>
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="graphics">Graphics object</param>
        /// <param name="y">Y Coordinate</param>
        private void DrawIndent(Line line, Graphics graphics, int y)
        {
            if (!_donkeyScroller.ShowLineIndentColors)
                return;

            graphics.SmoothingMode = SmoothingMode.None;
            graphics.FillRectangle(_donkeyScroller.BrushCache.Get(line.IndentColor), 0, y, _donkeyScroller.CharWidth, line.Height);
        }
        /// <summary>
        /// Draws the text of the line
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="graphics">Graphics object</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="drawRow">Row to draw</param>
        /// <param name="markedChars">List of marked chars</param>
        private void DrawText(Line line, Graphics graphics, ref float x, int y, int drawRow, List<char> markedChars)
        {
            CharData charData = new CharData(_donkeyScroller.Font, _donkeyScroller.ForeColor, Color.Transparent);
            
            int rowIndex = drawRow + line.RowsHigh;
            int charIndex = 0;
            int rowCharIndex = 0;

            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            foreach (Char c in line)
            {
                charData = c.Data ?? charData;
                SetDefaultColorsIfAny(line, c);
                
                Font font = charData.Font;
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
                        charData.Font.Style.HasFlag(FontStyle.Bold) ? ControlPaint.Dark(charData.ForeColor ?? _donkeyScroller.ForeColor, 0.05f) : charData.ForeColor ?? _donkeyScroller.ForeColor
                    );
                }
                graphics.DrawString(c.C.ToString(), font, brush, new Rectangle((int) x - 50 + _donkeyScroller.CharWidth / 2, y, 100, _donkeyScroller.CharHeight), new StringFormat() {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.MeasureTrailingSpaces});
                AdvanceCoords(line, ref x, ref y, ref charIndex, ref rowIndex, ref rowCharIndex, markedChars);
            }
        }
        /// <summary>
        /// Sets the default colors of <see cref="Char"/> if they are defined
        /// </summary>
        /// <param name="line">The Line</param>
        /// <param name="c">The Char</param>
        private static void SetDefaultColorsIfAny(Line line, Char c)
        {
            if (c.Data != null && line.IndexOf(c) == 0)
            {
                if (line.DefaultBackColor.HasValue)
                {
                    c.Data.ForeColor = line.DefaultForeColor;
                    c.Data.BackColor = line.DefaultBackColor;
                }
            }
        }
        /// <summary>
        /// Draws text background colors
        /// </summary>
        /// <param name="line">The line</param>
        /// <param name="graphics">Graphics object</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="rowCharIndex">Row Char Index</param>
        /// <param name="rowIndex">Row Index</param>
        /// <param name="charIndex">Char Index</param>
        private void DrawBackground(Line line, Graphics graphics, float x, int y, int rowCharIndex, int rowIndex, int charIndex)
        {
            CharData charData = new CharData(_donkeyScroller.Font, _donkeyScroller.ForeColor, Color.Transparent);
            float x2 = x;
            int y2 = y;
            if (line.FillColor.HasValue)
            {
                Brush brush = _donkeyScroller.BrushCache.Get(line.FillColor.Value);
                graphics.FillRectangle(brush,_donkeyScroller.GutterItemsWidth-1,y,_donkeyScroller.Width,line.Height);
            }
            foreach (Char c in line)
            {
                charData = c.Data ?? charData;
                SetDefaultColorsIfAny(line, c);

                Brush brush = null;

                if (IsInSelection(rowCharIndex, rowIndex))
                    brush = _donkeyScroller.BrushCache.Get(SystemColors.Highlight);
                else if (charData.BackColor != Color.Transparent)
                    brush = _donkeyScroller.BrushCache.Get(charData.BackColor ?? _donkeyScroller.BackColor);

                if (brush != null)
                {
                    graphics.SmoothingMode = SmoothingMode.None;
                    graphics.FillRectangle(brush, x2, y2, _donkeyScroller.CharWidth, _donkeyScroller.CharHeight);
                }

                AdvanceCoords(line, ref x2, ref y2, ref charIndex, ref rowIndex, ref rowCharIndex);
            }
        }

        /// <summary>
        /// Checks whether or not char and row index are in selection
        /// </summary>
        /// <param name="charIndex">Char index</param>
        /// <param name="rowIndex">Row index</param>
        /// <returns>True if inside selection</returns>
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
        /// <param name="line">The line</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="charIndex">Char index</param>
        /// <param name="rowIndex">Row index</param>
        /// <param name="rowCharIndex">RowChar Index</param>
        /// <param name="markedChars">List of marked chars</param>
        private void AdvanceCoords(Line line,ref float x, ref int y, ref int charIndex, ref int rowIndex, ref int rowCharIndex, List<char> markedChars = null)
        {
            x += _donkeyScroller.CharWidth;
            rowCharIndex++;
            if (line.WrapIndexes.Contains(++charIndex))
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
    }
}