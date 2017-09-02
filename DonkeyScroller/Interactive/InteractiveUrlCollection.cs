using System.Collections;
using System.Linq;

namespace JaniPellikka.Windows.Forms.Interactive
{
    /// <summary>
    /// Collection of InteractiveURLs in a line
    /// </summary>
    internal class InteractiveUrlCollection : CollectionBase
    {
        /// <summary>
        /// Adds a interactive url to the collection
        /// </summary>
        /// <seealso cref="InteractiveUrl"/>
        /// <param name="charStart">Char start index in line</param>
        /// <param name="charEnd">Char end index in line</param>
        /// <param name="url">The URL</param>
        public void Add(int charStart, int charEnd, string url)
        {
            Add(new InteractiveUrl(charStart, charEnd, url));
        }
        /// <summary>
        /// Adds a <see cref="InteractiveUrl"/> to the collection
        /// </summary>
        /// <param name="urlText">The <see cref="InteractiveUrl"/> to add</param>
        public void Add(InteractiveUrl urlText)
        {
            List.Add(urlText);
        }
        /// <summary>
        /// Gets <see cref="InteractiveUrl"/> at char index, if any
        /// </summary>
        /// <param name="charPos">Char index in the line</param>
        /// <returns><see cref="InteractiveUrl"/> based on <paramref name="charPos"/></returns>
        public InteractiveUrl Get(int charPos)
        {
            return List.Cast<InteractiveUrl>().FirstOrDefault(interactiveUrl => charPos >= interactiveUrl.CharStart && charPos <= interactiveUrl.CharEnd);
        }
    }
}