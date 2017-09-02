using System;
using JaniPellikka.Windows.Forms.Interactive;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Event arguments for Interactive Text
    /// </summary>
    public sealed class InteractiveTextEventArgs : EventArgs
    {
        /// <summary>
        /// <see cref="InteractiveUrl"/> associated with the event
        /// </summary>
        public readonly InteractiveText InteractiveText;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interactiveText"><see cref="InteractiveText"/> associated with the event</param>
        internal InteractiveTextEventArgs(InteractiveText interactiveText)
        {
            InteractiveText = interactiveText;
        }
    }
}