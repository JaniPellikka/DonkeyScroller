using System.Collections;
using System.Linq;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Collection of URL segments in a line
    /// </summary>
    internal class UrlSegmentCollection : CollectionBase
    {
        /// <summary>
        /// Adds a url segment to the collection
        /// </summary>
        /// <param name="charStart">Char start index in line</param>
        /// <param name="charEnd">Char end index in line</param>
        /// <param name="url">The URL</param>
        public void Add(int charStart, int charEnd, string url)
        {
            Add(new UrlSegment(charStart, charEnd, url));
        }
        /// <summary>
        /// Adds a segment to the collection
        /// </summary>
        /// <param name="segment">The URL Segment</param>
        public void Add(UrlSegment segment)
        {
            List.Add(segment);
        }
        /// <summary>
        /// Gets URL Segment at char index, if any
        /// </summary>
        /// <param name="charPos">Char index in the line</param>
        /// <returns></returns>
        public UrlSegment Get(int charPos)
        {
            return List.Cast<UrlSegment>().FirstOrDefault(segment => charPos >= segment.CharStart && charPos <= segment.CharEnd);
        }
    }
}