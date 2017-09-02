namespace JaniPellikka.Windows.Forms.Interactive
{
    /// <summary>
    /// Interactive text
    /// </summary>
    public sealed class InteractiveText
    {
        /// <summary>
        /// Index of the start
        /// </summary>
        internal readonly int CharStart;

        /// <summary>
        /// Index of the end
        /// </summary>
        internal readonly int CharEnd;

        /// <summary>
        /// Data of the interactive text
        /// </summary>
        public readonly string Data;
        /// <summary>
        /// Text to display for the interactive text
        /// </summary>
        public readonly string Text;
        /// <summary>
        /// Instantiates a new instance
        /// </summary>
        /// <param name="charStart">Start index of the interactive text</param>
        /// <param name="charEnd">End index of the interactive text</param>
        /// <param name="data">Data of the interactive text</param>
        /// <param name="text">The text to display for the interavtive text</param>
        internal InteractiveText(int charStart, int charEnd, string data, string text)
        {
            CharStart = charStart;
            CharEnd = charEnd;
            Data = data;
            Text = text;
        }

        private InteractiveText()
        {
            // Prevent creation outside of this assembly
        }
    }
}