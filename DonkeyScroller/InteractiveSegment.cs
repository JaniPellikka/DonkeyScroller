namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Segment of an interactive text
    /// </summary>
    internal class InteractiveSegment
    {
        /// <summary>
        /// Index of the start
        /// </summary>
        public int CharStart { get; private set; }
        /// <summary>
        /// Index of the end
        /// </summary>
        public int CharEnd { get; private set; }

        /// <summary>
        /// Id of the interactive text
        /// </summary>
        public readonly string Id;
        /// <summary>
        /// Text to display for the interactive text
        /// </summary>
        public readonly string Text;
        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="charStart">Start index of the interactive text</param>
        /// <param name="charEnd">End index of the interactive text</param>
        /// <param name="id">Id of the interactive text</param>
        /// <param name="text">The text to display for the interavtive text</param>
        public InteractiveSegment(int charStart, int charEnd, string id, string text)
        {
            CharStart = charStart;
            CharEnd = charEnd;
            Id = id;
            Text = text;
        }
    }
}