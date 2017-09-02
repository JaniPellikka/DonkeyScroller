namespace JaniPellikka.Windows.Forms.Content
{
    /// <summary>
    /// Holds information about selection in the chat view
    /// </summary>
    internal class Selection
    {
        /// <summary>
        /// Start row and column
        /// </summary>
        public RowColumn Start;
        /// <summary>
        /// End row and column
        /// </summary>
        public RowColumn End;
        /// <summary>
        /// Instantiates new instance
        /// </summary>
        /// <param name="start">Start of the selection</param>
        /// <param name="end">End of the selection</param>
        public Selection(RowColumn start, RowColumn end)
        {
            if (start.Row > end.Row || (start.Row == end.Row && start.Column > end.Column))
            {
                Start = end;
                End = start;
            }
            else
            {
                Start = start;
                End = end;
            }
        }
        /// <summary>
        /// Checks whether or not row and column is within the selection
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <returns>True if inside the selection</returns>
        public bool Inside(int row, int column)
        {
            return Inside(new RowColumn { Row = row, Column = column });
        }
        /// <summary>
        /// Checks whether or not row and column is within the selection
        /// </summary>
        /// <param name="rowColumn">The row and column</param>
        /// <returns>True if inside the selection</returns>
        public bool Inside(RowColumn rowColumn)
        {
            if (rowColumn.Row == Start.Row && rowColumn.Row == End.Row) // same row
                return rowColumn.Column >= Start.Column && rowColumn.Column <= End.Column; // between columns
            if (rowColumn.Row > Start.Row && rowColumn.Row < End.Row) // between rows
                return true;
            if (rowColumn.Row == Start.Row) // start row
                return rowColumn.Column <= Start.Column; // before column
            if (rowColumn.Row == End.Row) // end row
                return rowColumn.Column >= End.Column; // after column
            return false;

        }

    }
}