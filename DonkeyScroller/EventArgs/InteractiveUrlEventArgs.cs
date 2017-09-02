using System;
using JaniPellikka.Windows.Forms.Interactive;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Argument for Url related events
    /// </summary>
    public sealed class InteractiveUrlEventArgs : EventArgs
    {
        /// <summary>
        /// The <see cref="InteractiveUrl"/> associated with the event
        /// </summary>
        public readonly InteractiveUrl InteractiveUrl;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interactiveUrl"><see cref="InteractiveUrl"/> associated with the event</param>
        internal InteractiveUrlEventArgs(InteractiveUrl interactiveUrl)
        {
            InteractiveUrl = interactiveUrl;
        }
    }
}