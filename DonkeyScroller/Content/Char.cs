namespace JaniPellikka.Windows.Forms.Content
{
    /// <summary>
    /// Individual char of a line
    /// </summary>
    internal class Char
    {
        /// <summary>
        /// The actual char to display
        /// </summary>
        public char C;
        /// <summary>
        /// Associated data with this <see cref="Char"/> and every <see cref="Char"/> that follows without their own <see cref="Data"/>
        /// </summary>
        public CharData Data;
    }
}