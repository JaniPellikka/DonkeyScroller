namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Segment for URL
    /// </summary>
    internal class UrlSegment
    {
        /// <summary>
        /// Start index of line
        /// </summary>
        public readonly int CharStart;
        /// <summary>
        /// End index of line
        /// </summary>
        public readonly int CharEnd;
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
        public UrlSegment(int charStart, int charEnd, string url)
        {
            CharStart = charStart;
            CharEnd = charEnd;
            Url = url;
        }
    }
}