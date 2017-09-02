using System.Collections;
using System.Linq;

namespace JaniPellikka.Windows.Forms.Interactive
{
    /// <summary>
    /// Collection of interactive texts
    /// </summary>
    internal class InteractiveTextCollection : CollectionBase
    {
        /// <summary>
        /// Adds a record for an interactive text
        /// </summary>
        /// <param name="charStart">Start index for the interactive text</param>
        /// <param name="charEnd">End index for the interactive text</param>
        /// <param name="id">Id for the interactive text</param>
        /// <param name="text">Text to display for the interactive text</param>
        public void Add(int charStart, int charEnd, string id, string text)
        {
            Add(new InteractiveText(charStart, charEnd, id, text));
        }
        /// <summary>
        /// Adds a <see cref="InteractiveText"/> 
        /// </summary>
        /// <param name="interactiveText">The interactive text to add</param>
        public void Add(InteractiveText interactiveText)
        {
            List.Add(interactiveText);
        }
        /// <summary>
        /// Gets a <see cref="InteractiveText"/> at a char index, if any
        /// </summary>
        /// <param name="charPos">Index of char</param>
        /// <returns><see cref="InteractiveText"/> based on <paramref name="charPos"/></returns>
        public InteractiveText Get(int charPos)
        {
            return List.Cast<InteractiveText>().FirstOrDefault(interactiveText => charPos >= interactiveText.CharStart && charPos <= interactiveText.CharEnd);
        }
    }
}