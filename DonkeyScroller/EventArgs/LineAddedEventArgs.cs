using System;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Arguments for line added event
    /// </summary>
    public class LineAddedEventArgs : EventArgs
    {
        /// <summary>
        /// Original string, with control codes
        /// </summary>
        public readonly string FormattedText;
        /// <summary>
        /// Stripped text, with no control codes
        /// </summary>
        public readonly string PlainText;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="formattedText">Original string, with control codes</param>
        /// <param name="plainText">Stripped text, with no control codes</param>
        public LineAddedEventArgs(string formattedText, string plainText)
        {
            FormattedText = formattedText;
            PlainText = plainText;
        }
    }
}