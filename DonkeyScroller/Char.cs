namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Represents a single char in the view
    /// </summary>
    internal class Char
    {
        /// <summary>
        /// Index for foreground color
        /// </summary>
        public int ForeColor;
        /// <summary>
        /// Index for background color
        /// </summary>
        public int BackColor;
        /// <summary>
        /// Flags whether or not this char is bold
        /// </summary>
        public bool Bold;
        /// <summary>
        /// Flags whether or not this char is underline
        /// </summary>
        public bool Underline;
        /// <summary>
        /// The char itself
        /// </summary>
        public char C;
    }
}