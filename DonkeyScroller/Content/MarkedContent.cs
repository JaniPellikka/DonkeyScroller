using System.Collections.Generic;
using System.Linq;

namespace JaniPellikka.Windows.Forms.Content
{
    /// <summary>
    /// Container for marked content
    /// </summary>
    internal class MarkedContent
    {
        /// <summary>
        /// Flags whether or not the marked content has any text
        /// </summary>
        public bool HasText => !string.IsNullOrEmpty(Text);
        /// <summary>
        /// Flags whether or not the marked content has any <see cref="Char"/>s
        /// </summary>
        public bool HasChars => Chars.Count > 0;
        /// <summary>
        /// The marked content as a string
        /// </summary>
        public string Text { get; private set; } = "";
        /// <summary>
        /// List of <see cref="Char"/> that constitutes the marked content
        /// </summary>
        public readonly List<char> Chars = new List<char>();
        /// <summary>
        /// Clears the marked text
        /// </summary>
        public void ClearText()
        {
            Text = "";
        }
        /// <summary>
        /// Clears the list of <see cref="Char"/>s
        /// </summary>
        public void ClearChars()
        {
            Chars.Clear();
        }
        /// <summary>
        /// Inserts the marked chars into <see cref="Text"/>
        /// </summary>
        public void InsertChars()
        {
            if (Chars.Any())
                Text = new string(Chars.ToArray()).Trim() + "\r\n" + Text;
        }
        /// <summary>
        /// Trims the <see cref="Text"/>
        /// </summary>
        public void TrimText()
        {
            Text = Text.Trim();
        }
        /// <summary>
        /// Converts this to string using <see cref="Text"/>
        /// </summary>
        /// <returns>The marked content as a string</returns>
        public override string ToString()
        {
            return Text;
        }
    }
}