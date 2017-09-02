using System;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Arguments for marked text event
    /// </summary>
    public sealed class MarkedTextEventArgs : EventArgs
    {
        /// <summary>
        /// The marked text
        /// </summary>
        public readonly string Text;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text">The marked text</param>
        public MarkedTextEventArgs(string text)
        {
            Text = text;
        }
    }
}