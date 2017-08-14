using System;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// EventArgs for a double clicked interactive text
    /// </summary>
    public class InteractiveTextDoubleClickedEventArgs : EventArgs
    {
        /// <summary>
        /// The Id of the double clicked interactive text segment
        /// </summary>
        public readonly string Id;
        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="id">The Id of the double clicked interactive text segment</param>
        public InteractiveTextDoubleClickedEventArgs(string id)
        {
            Id = id;
        }
    }
}