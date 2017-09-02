namespace JaniPellikka.Windows.Forms.Interactive
{
    /// <summary>
    /// Interactive URL
    /// </summary>
    public sealed class InteractiveUrl
    {
        /// <summary>
        /// Start index of line
        /// </summary>
        internal readonly int CharStart;
        /// <summary>
        /// End index of line
        /// </summary>
        internal readonly int CharEnd;
        /// <summary>
        /// The URL
        /// </summary>
        public readonly string Url;
        /// <summary>
        /// Instantiates new instance
        /// </summary>
        /// <param name="charStart">Start index of line</param>
        /// <param name="charEnd">End index of line</param>
        /// <param name="url">The URL</param>
        internal InteractiveUrl(int charStart, int charEnd, string url)
        {
            CharStart = charStart;
            CharEnd = charEnd;
            Url = url;
        }

        private InteractiveUrl()
        {
            // Prevent creation outside of this assembly
        }
    }
}