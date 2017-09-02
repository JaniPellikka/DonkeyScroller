using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JaniPellikka.Windows.Forms.Internal;

namespace JaniPellikka.Windows.Forms.Content
{
    /// <summary>
    /// List of lines in the view
    /// </summary>
    internal class Lines : List<Line>
    {
        /// <summary>
        /// Dictionary of lines and their bounding rectangles
        /// </summary>
        public readonly Dictionary<Line, Rectangle> Rectangles = new Dictionary<Line, Rectangle>();
        /// <summary>
        /// The <see cref="DonkeyScroller"/> that own these lines
        /// </summary>
        private readonly DonkeyScroller _donkeyScroller;
        /// <summary>
        /// Number of lines to keep in the list
        /// </summary>
        private int _maxLines = 100;
        /// <summary>
        /// Number of lines that have been added but not shown due to VScrollbar not being at the bottom
        /// </summary>
        internal int UnseenCount = 0;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="donkeyScroller">The owner</param>
        public Lines(DonkeyScroller donkeyScroller)
        {
            _donkeyScroller = donkeyScroller;
        }

        /// <summary>
        /// Number of lines to keep in the list
        /// </summary>
        public int MaxLines
        {
            get => _maxLines;
            set
            {
                if (_maxLines == value)
                    return;
                // Make sure we cannot select a ridiculous small amount of lines to display
                value = Math.Max(value, 10);

                _maxLines = value;
                PurgeExcessLines();
            }
        }
        /// <summary>
        /// Gets all the lines as a string
        /// </summary>
        public string Text
        {
            get
            {
                return this.Aggregate("", (current, line) => line.Text + "\r\n" + current);
            }
        }

        /// <summary>
        /// Removes excess lines
        /// </summary>
        private void PurgeExcessLines()
        {
            while (Count > _maxLines)
                RemoveAt(Count - 1);
        }
        /// <summary>
        /// Inserts a new line to the list
        /// </summary>
        /// <remarks>Will also purge excess lines according to <see cref="MaxLines"/></remarks>
        /// <param name="line"></param>
        public void Insert(Line line)
        {
            Insert(0, line);
            PurgeExcessLines();
        }
        /// <summary>
        /// Adds text as a new <see cref="Line"/>
        /// </summary>
        /// <param name="text"></param>
        public void Add(string text)
        {
            Add(Parsers.ParseLineText(text, _donkeyScroller));
        }
        // TODO: Calculate only for visible lines
        /// <summary>
        /// Calculates all the lines dimensions
        /// </summary>
        public void CalculateDimensions()
        {
            ForEach(x => x.CalculateDimensions());
        }

        /// <summary>
        /// Sets the font on every <see cref="Char"/> that has a <see cref="CharData"/>
        /// </summary>
        /// <param name="font">The font</param>
        public void SetFont(Font font)
        {
            foreach (Line line in this)
            {
                foreach (Char c in line)
                {
                    if (c.Data != null)
                        c.Data.Font = new Font(font, c.Data.Font.Style);
                }
            }
        }
    }
}