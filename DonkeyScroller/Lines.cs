using System;
using System.Collections.Generic;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// All the lines in the chat view
    /// </summary>
    internal class Lines : List<Line>
    {
        /// <summary>
        /// Number of lines to keep in the list
        /// </summary>
        private int _maxLines = 100;
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
            Insert(0,line);
            PurgeExcessLines();
        }
    }
}