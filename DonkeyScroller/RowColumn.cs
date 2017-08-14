namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Holds the row and column indexes based on a point on the chat view
    /// </summary>
    internal class RowColumn
    {
        /// <summary>
        /// Row index
        /// </summary>
        public int Row;
        /// <summary>
        /// Column index
        /// </summary>
        public int Column;
        /// <summary>
        /// Checks whether or not this is equal to another RowColumn
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RowColumn other)
        {
            return other != null && Row == other.Row && Column == other.Column;
        }
        /// <summary>
        /// Converts this to a human readable string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $@"{Row}:{Column}";
        }
    }
}