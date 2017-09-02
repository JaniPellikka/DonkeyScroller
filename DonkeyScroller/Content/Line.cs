using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JaniPellikka.Windows.Forms.Interactive;

namespace JaniPellikka.Windows.Forms.Content
{
    /// <summary>
    /// Represents a single line entry of text
    /// </summary>
    internal class Line : List<Char>
    {
        /// <summary>
        /// Icon to display for the line
        /// </summary>
        public Image IconImage;
        /// <summary>
        /// Color to fill the whole lines area with
        /// </summary>
        public Color? FillColor;
        /// <summary>
        /// Default background color for this line
        /// </summary>
        public Color? DefaultBackColor;
        /// <summary>
        /// Default foreground color for this line
        /// </summary>
        public Color? DefaultForeColor;

        /// <summary>
        /// Color of indent for the line
        /// </summary>
        public Color IndentColor = Color.Transparent;
        /// <summary>
        /// <see cref="InteractiveUrl"/>s for this line
        /// </summary>
        public InteractiveUrlCollection InteractiveUrls = new InteractiveUrlCollection();
        /// <summary>
        /// <see cref="InteractiveText"/>s for this line
        /// </summary>
        public InteractiveTextCollection InteractiveTexts = new InteractiveTextCollection();
        /// <summary>
        /// Reference to the DonkeyScroller that owns this Line
        /// </summary>
        private readonly DonkeyScroller _donkeyScroller;
        /// <summary>
        /// Char indexes where the text should wrap
        /// </summary>
        public readonly List<int> WrapIndexes = new List<int>();
        /// <summary>
        /// Width of this Line in pixels (including wrapping)
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Height of this Line in pixels (including wrapping)
        /// </summary>
        public int Height
        {
            get
            {
                if (_donkeyScroller.ShowLineIcons && IconImage!=null)
                    return Math.Max(_donkeyScroller.CharHeight * RowsHigh, _donkeyScroller.CharHeight);

                return _donkeyScroller.CharHeight * RowsHigh;
            }
        }
        /// <summary>
        /// Number of rows this Line occupies
        /// </summary>
        public int RowsHigh { get; private set; }
        /// <summary>
        /// The text in this line with all the control codes stripped
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
        /// Calculates the height and width of this line.
        /// Also creates wrap indexes for wrapping the line
        /// </summary>
        public void CalculateDimensions()
        {
            WrapIndexes.Clear();

            int textLength = 0;

            int wrapIndex = 0;
            RowsHigh = 1;

            for (int i = 0; i < Count; i++)
            {
                Char c = this[i];
                if (!char.IsLetterOrDigit(c.C) && c.C != '_' && c.C != '\'' && c.C != '\xa0')
                    wrapIndex = Math.Min(i + 1, Count - 1);
                textLength++;

                if (textLength >= _donkeyScroller.MaxWidthInChars)
                {

                    if (wrapIndex == 0 || (WrapIndexes.Count > 0 && wrapIndex == WrapIndexes[WrapIndexes.Count - 1]))
                        wrapIndex = i + 1;
                    WrapIndexes.Add(wrapIndex);
                    Width = Math.Max(Width, textLength);
                    textLength = 1 + i - wrapIndex;
                    RowsHigh++;
                }
            }

        }
        /// <summary>
        /// Instantiates new instance
        /// </summary>
        /// <param name="donkeyScroller">Parent DonkeyScroller</param>
        public Line(DonkeyScroller donkeyScroller)
        {
            _donkeyScroller = donkeyScroller;
        }


    }
}