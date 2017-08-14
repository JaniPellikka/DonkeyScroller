using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Chat view control
    /// </summary>
    public sealed class DonkeyScroller : Control
    {
        // TODO Make this configurable from the designer
        /// <summary>
        /// Number of pixels to separate each line
        /// </summary>
        private const int LineSpacing = 2;
        /// <summary>
        /// Regex to parse URLs
        /// </summary>
        private static readonly Regex UrlRegex = new Regex(@"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])", RegexOptions.IgnoreCase);
        /// <summary>
        /// Lines for the chat view
        /// </summary>
        private readonly Lines _lines = new Lines();
        /// <summary>
        /// Literal chars of marked text
        /// </summary>
        private readonly List<char> _markedChars = new List<char>();
        /// <summary>
        /// Dictionary of lines and their bounding rectangles
        /// </summary>
        private readonly Dictionary<Line, Rectangle> _lineRectangles = new Dictionary<Line, Rectangle>();
        /// <summary>
        /// Vertical scrollbar
        /// </summary>
        private readonly VScrollBar _vScrollBar;
        /// <summary>
        /// Cache for Brushes
        /// </summary>
        internal readonly BrushCache BrushCache;

        private RowColumn _mouseDownRowColumn;
        /// <summary>
        /// Indicates whether or not the left mouse button is down
        /// </summary>
        private bool _mouseIsDown;
        /// <summary>
        /// Position of mouse as Row and Column
        /// </summary>
        private RowColumn _mouseRowColumn;
        /// <summary>
        /// Currently marked text
        /// </summary>
        private string _markedText;
        /// <summary>
        /// Selection of text with starting and ending Row and Column
        /// </summary>
        internal Selection Selection;
        /// <summary>
        /// Cache for fonts
        /// </summary>
        internal FontCache FontCache { get; private set; }
        
        /// <summary>
        /// Height of the tallest char 
        /// </summary>
        public int CharHeight { get; private set; }
        /// <summary>
        /// Width of a char (Remember, only using fixed/mono fonts)
        /// </summary>
        public int CharWidth { get; private set; }
        /// <summary>
        /// The maximum number of chars that fit into the width of the chat view
        /// </summary>
        public int MaxWidthInChars => (int)Math.Floor((decimal)TextAreaRectangle.Width / CharWidth);
        /// <summary>
        /// The offset for lines rendering
        /// </summary>
        internal int ScrollOffset => _vScrollBar.Maximum - _vScrollBar.Value;
        /// <summary>
        /// Maximum number of lines to keep track of
        /// </summary>
        public int MaxLines
        {
            get => _lines.MaxLines;
            set => _lines.MaxLines = value;
        }
        /// <summary>
        ///     Font to render the text with
        /// </summary>
        /// <remarks>Use only monospaced font</remarks>
        [DefaultValue(typeof(Font), "Courier New, 9.75")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                if (!Utils.IsMonoSpaceFont(base.Font))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    // ReSharper disable once LocalizableElement
                    base.Font = new Font("Courier New", base.Font.SizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
                }
                SizeF size = Utils.GetCharSize(base.Font, 'M');
                CharWidth = (int)Math.Ceiling(size.Width * 1f /*0.85*/) - 1 /*0*/;
                CharHeight = LineSpacing + (int)Math.Ceiling(size.Height * 1f /*0.9*/) - 1 /*0*/;
                FontCache = new FontCache(base.Font);
                Invalidate();
            }
        }
        /// <summary>
        /// Event for double clicking an interactive text
        /// </summary>
        public event EventHandler<InteractiveTextDoubleClickedEventArgs> InteractiveTextClick;
        /// <summary>
        /// Triggers the double clicking and interactive text event
        /// </summary>
        /// <param name="id">The interactive text segments Id</param>
        private void OnInteractiveTextDoubleClick(string id)
        {
            InteractiveTextClick?.Invoke(this, new InteractiveTextDoubleClickedEventArgs(id));
        }
        /// <summary>
        /// The actual rectangle available for rendering. This takes padding and the vertical scrollbar into consideration
        /// </summary>
        internal Rectangle TextAreaRectangle => new Rectangle(
            Padding.Left,
            Padding.Top,
            ClientRectangle.Width - (Padding.Left + Padding.Right) - _vScrollBar.Width,
            ClientRectangle.Height - (Padding.Top + Padding.Bottom)
        );

        /// <summary>
        /// Instantiates a new instance for the chat view
        /// </summary>
        public DonkeyScroller()
        {
            // Make sure we have ALL the control over painting the control
            SetStyle(ControlStyles.AllPaintingInWmPaint|
                ControlStyles.UserPaint|
                ControlStyles.ResizeRedraw|
                ControlStyles.OptimizedDoubleBuffer, 
                true
                );

            Cursor = Cursors.IBeam;

            _vScrollBar = new VScrollBar { Parent = this, Dock = DockStyle.Right, LargeChange = 1 };
            _vScrollBar.Scroll += (sender, args) => { Invalidate(); };

            Padding = new Padding(2);
            BrushCache = new BrushCache();
            Font = new Font(FontFamily.GenericMonospace, 9.75f);


        }
        /// <summary>
        /// Scrolling the the mousewheel will affect the vertical scrollbar
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta > 0)
            {
                if (_vScrollBar.Value > 0)
                    _vScrollBar.Value--;
            }
            else
            {
                if (_vScrollBar.Value < _vScrollBar.Maximum)
                    _vScrollBar.Value++;
            }
            Invalidate();
        }
        /// <summary>
        /// Clears the chat view
        /// </summary>
        public void Clear()
        {
            _lines.Clear();
            Invalidate();
        }
        /// <summary>
        /// Creates an interactive text that you can add to the chat view with specified color
        /// </summary>
        /// <param name="id">Id for the interactive text. This will be available when double clicking the text</param>
        /// <param name="text">The text for the interactive part</param>
        /// <param name="color">Color to use</param>
        /// <returns>Formatted string ready to be inserted into the chat view</returns>
        public string CreateInteractiveString(string id, string text, Color color)
        {
            return IRCControlChar.Color + IRCColorTable.Get(color) + CreateInteractiveString(id, text) + IRCControlChar.Color;
        }
        /// <summary>
        /// Creates an interactive text that you can add to the chat view 
        /// </summary>
        /// <param name="id">Id for the interactive text. This will be available when double clicking the text</param>
        /// <param name="text">The text for the interactive part</param>
        /// <returns>Formatted string ready to be inserted into the chat view</returns>
        public string CreateInteractiveString(string id, string text)
        {
            return IRCControlChar.InteractiveStart + id + IRCControlChar.InteractiveSplit + text + IRCControlChar.InteractiveEnd;
        }
        /// <summary>
        /// Adds text to the chat view
        /// </summary>
        /// <param name="text">Text to add</param>
        /// <param name="color">Optional color for the text</param>
        /// <seealso cref="CreateInteractiveString(string,string,System.Drawing.Color)"/>
        public void AddText(string text, MircColorCode color = MircColorCode.None)
        {

            try
            {
                if (!IsDisposed)
                {
                    if (string.IsNullOrWhiteSpace(text))
                        return;
                    text = UrlRegex.Replace(text, IRCControlChar.Url + "$0" + IRCControlChar.Url);
                    text = IRCColorTable.GetColorString(color) + text;

                    Line line = Line.Parse(text, this);
                    Utils.InvokeIfRequired(this, () =>
                    {
                        _lines.Insert(line);
                        _vScrollBar.Maximum = _lines.Count;
                        _vScrollBar.Value = _vScrollBar.Maximum;
                        Invalidate();
                    });
                }
            }
            catch (InvalidAsynchronousStateException)
            {
                // do nothing :E
            }
        }

        /// <summary>
        /// Converts mouse point to a Row and Column
        /// </summary>
        /// <param name="point">Position of the mouse</param>
        /// <returns></returns>
        internal RowColumn PointToRowColumn(Point point)
        {
            RowColumn rc = new RowColumn
            {
                Column = point.X / CharWidth,
                Row = (TextAreaRectangle.Height - point.Y) / CharHeight
            };
            return rc;
        }

        /// <summary>
        /// Gets URL at mouse position, if any
        /// </summary>
        /// <param name="point">Position of the mouse</param>
        /// <returns></returns>
        private UrlSegment GetUrlSegmentAtPoint(Point point)
        {
            Line line = LineAtLocation(point);
            if (line == null) return null;
            if (line.UrlSegments.Count <= 0) return null;
            if (!_lineRectangles.ContainsKey(line)) return null;

            int charIndex = GetLineCharIndexFromPoint(point, line);

            return line.UrlSegments.Get(charIndex);
        }
        /// <summary>
        /// Gets interactive segment at mouse position
        /// </summary>
        /// <param name="point">Mouse position</param>
        /// <returns></returns>
        private InteractiveSegment GetInteractiveSegmentAtPoint(Point point)
        {
            Line line = LineAtLocation(point);
            if (line == null) return null;
            if (line.InteractiveSegments.Count <= 0) return null;
            if (!_lineRectangles.ContainsKey(line)) return null;

            int charIndex = GetLineCharIndexFromPoint(point, line);

            return line.InteractiveSegments.Get(charIndex);
        }
        /// <summary>
        /// Gets the index of a char on a line at mouse position
        /// </summary>
        /// <param name="point">Mouse position</param>
        /// <param name="line">The line to look for char index in</param>
        /// <returns></returns>
        private int GetLineCharIndexFromPoint(Point point, Line line)
        {
            Rectangle r = _lineRectangles[line];
            int y = point.Y - r.Top;
            int x = point.X - r.Left;

            if (y > 0)
                y = y / CharHeight;
            if (x > 0)
                x = x / CharWidth;

            return y * MaxWidthInChars + x;
        }
        /// <summary>
        /// When user double clicks we look for URLs and interactive texts and act accordingly
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            UrlSegment urlSegment = GetUrlSegmentAtPoint(e.Location);
            if (urlSegment != null)
            {
                Process.Start(urlSegment.Url);
                return;
            }
            InteractiveSegment interactiveSegment = GetInteractiveSegmentAtPoint(e.Location);
            if (interactiveSegment != null)
            {
                OnInteractiveTextDoubleClick(interactiveSegment.Id);
                return;
            }
            base.OnMouseDoubleClick(e);
        }
        /// <summary>
        /// When user moves the mouse, look for URLs and interactive segments and show the Hand cursor if appropriate.
        /// If the mouse is down while moving, track selection
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _mouseRowColumn = PointToRowColumn(e.Location);
            base.OnMouseMove(e);

            UrlSegment urlSegment = GetUrlSegmentAtPoint(e.Location);
            InteractiveSegment interactiveSegment = GetInteractiveSegmentAtPoint(e.Location);

            Cursor = urlSegment != null || interactiveSegment != null ? Cursors.Hand : Cursors.IBeam;

            if (_mouseIsDown)
            {
                Selection = new Selection(_mouseDownRowColumn, _mouseRowColumn);
                Invalidate();
            }
        }
        /// <summary>
        /// Gets a line at mouse position
        /// </summary>
        /// <param name="point">Mouse position</param>
        /// <returns></returns>
        private Line LineAtLocation(Point point)
        {
            return (from kvp in _lineRectangles where kvp.Value.Contains(point) select kvp.Key).FirstOrDefault();
        }
        // TODO Uhm... should we not check for left/right mouse button?
        /// <summary>
        /// Start selection marking
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _mouseDownRowColumn = PointToRowColumn(e.Location);
            _mouseIsDown = true;
            base.OnMouseDown(e);
        }
        /// <summary>
        /// End selection marking. If selection yelded in marked text, copy it to the clipboard
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(_markedText))
            {
                Clipboard.SetText(_markedText);
            }
            _mouseIsDown = false;
            Selection = null;
            Invalidate();
            base.OnMouseUp(e);
        }
        /// <summary>
        /// Paints the chat view
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            int x = TextAreaRectangle.Left;
            int y = TextAreaRectangle.Bottom;
            int drawRow = -1;
            _markedText = "";
            _lineRectangles.Clear();
            foreach (Line line in _lines.Skip(ScrollOffset))
            {
                _lineRectangles.Add(line, new Rectangle(
                    TextAreaRectangle.Left,
                    y - CharHeight * line.RowsHigh,
                    TextAreaRectangle.Width,
                    CharHeight * line.RowsHigh));
                _markedChars.Clear();
                y = line.Draw(e.Graphics, x, y, ref drawRow, _markedChars);
                if (_markedChars.Count > 0)
                {
                    _markedText = new string(_markedChars.ToArray()).Trim() + "\r\n" + _markedText;
                }
                drawRow++;
                if (y < 0)
                    break;
            }

            _markedText = _markedText.Trim();


            base.OnPaint(e);
        }
        /// <summary>
        /// When the chat views size changes we need to do some calculations about our lines and redraw everything
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            CalculateDimensions();
            Invalidate();
            base.OnClientSizeChanged(e);
        }
        /// <summary>
        /// Calculates all the lines dimensions
        /// </summary>
        private void CalculateDimensions()
        {
            _lines.ForEach(x => x.CalculateDimensions());
        }
    }
}
