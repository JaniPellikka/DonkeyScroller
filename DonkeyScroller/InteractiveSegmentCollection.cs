using System.Collections;
using System.Linq;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Collection of interavtive text segments
    /// </summary>
    internal class InteractiveSegmentCollection : CollectionBase
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
            Add(new InteractiveSegment(charStart, charEnd, id, text));
        }
        /// <summary>
        /// Adds an interactive text segment
        /// </summary>
        /// <param name="segment">The segment to add</param>
        public void Add(InteractiveSegment segment)
        {
            List.Add(segment);
        }
        /// <summary>
        /// Gets an interactive segment at a char index, if any
        /// </summary>
        /// <param name="charPos">Index of char</param>
        /// <returns></returns>
        public InteractiveSegment Get(int charPos)
        {
            return List.Cast<InteractiveSegment>().FirstOrDefault(segment => charPos >= segment.CharStart && charPos <= segment.CharEnd);
        }
    }
}