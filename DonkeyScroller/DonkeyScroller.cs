using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using JaniPellikka.Windows.Forms.Caches;
using JaniPellikka.Windows.Forms.Content;
using JaniPellikka.Windows.Forms.Interactive;
using JaniPellikka.Windows.Forms.Internal;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    ///     A chat view not much unlike the ones used for IRC and messenger applications
    /// </summary>
    [Description("A chat view not much unlike the ones used for IRC and messenger applications")]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(DonkeyScroller), "ToolBoxIcon")]
    [Designer(typeof(DonkeyScrollerDesigner))]
    public sealed class DonkeyScroller : Control
    {
        /// <summary>
        /// Borderstyle
        /// </summary>
        private Border3DStyle _border3DStyle = Border3DStyle.Flat;

        /// <summary>
        ///     Array of chars that are not considered as part of a word
        /// </summary>
        private static readonly char[] NonWordChars =
        {
            ' ','.',',','[',']','(',')','{','}','!',
            '?','^','_','~','½','|','<','>','*',':',
            ';','&','@','#','$','€','\\','/','+','¨',
            '"','\''
        };

        /// <summary>
        ///     Renderer for lines
        /// </summary>
        private readonly LineRenderer _lineRenderer;

        /// <summary>
        ///     The contents of the chat view
        /// </summary>
        private readonly Lines _lines;

        /// <summary>
        ///     Content marked by user with the mouse
        /// </summary>
        private readonly MarkedContent _markedContent = new MarkedContent();

        /// <summary>
        ///     Vertical scrollbar
        /// </summary>
        private readonly VScrollBar _vScrollBar;

        /// <summary>
        ///     Cache for Brushes
        /// </summary>
        internal readonly BrushCache BrushCache;

        /// <summary>
        ///     Color of the view
        /// </summary>
        private Color _backColor = Color.White;

        /// <summary>
        ///     Background image for the chatview
        /// </summary>
        private Image _backgroundImage;

        /// <summary>
        ///     Flags how to draw the background image
        /// </summary>
        private BackgroundImageMode _backgroundImageMode = BackgroundImageMode.Zoom;

        /// <summary>
        ///     Default color of the font, no color codes
        /// </summary>
        private Color _foreColor = Color.Black;

        /// <summary>
        ///     Currently hovering Interactive text
        /// </summary>
        private InteractiveText _hoveringInteractiveText;

        /// <summary>
        ///     Currently hovering <see cref="InteractiveUrl" />
        /// </summary>
        private InteractiveUrl _hoveringInteractiveUrl;

        /// <summary>
        ///     Color of line separator
        /// </summary>
        private Color _lineSeparatorColor = Color.DarkGray;

        /// <summary>
        ///     Number of pixels between lines
        /// </summary>
        private int _lineSpacing = 2;

        /// <summary>
        ///     Position where mouse was pressed down
        /// </summary>
        private RowColumn _mouseDownRowColumn;

        /// <summary>
        ///     Indicates whether or not the left mouse button is down
        /// </summary>
        private bool _mouseIsDown;

        /// <summary>
        ///     Position of mouse as Row and Column
        /// </summary>
        private RowColumn _mouseRowColumn;

        /// <summary>
        ///     Flags whether or not the lines should draw an icon in the gutter
        /// </summary>
        private bool _showLineIcons;

        /// <summary>
        ///     Flags whether or not to show line indent colors
        /// </summary>
        private bool _showLineIndentColors;

        /// <summary>
        ///     Flags whether or a line separator should be drawn
        /// </summary>
        private bool _showLineSeparator;

        /// <summary>
        ///     Timestamp format
        /// </summary>
        private string _timeStampFormat = "[HH:mm:ss]";

        /// <summary>
        ///     Selection of text with starting and ending Row and Column
        /// </summary>
        internal Selection Selection;

        /// <summary>
        ///     Constructor
        /// </summary>
        public DonkeyScroller()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Cursor = Cursors.IBeam;

            _lines = new Lines(this);

            // Cannot dock the scollbar as it would cover the controls border
            // Instead we deal with its position in RepositionVScrollBar
            //_vScrollBar = new VScrollBar {Parent = this, Dock = DockStyle.Right, LargeChange = 1, Maximum = 0, Margin = new Padding(2)};

            _vScrollBar = new VScrollBar { Parent = this, LargeChange = 1, Maximum = 0 };
            _vScrollBar.Scroll += (sender, args) =>
            {
                if (ScrollBarAtBottom)
                    _lines.UnseenCount = 0;
                Invalidate();

                RedirectFocus();
            };

            Padding = new Padding(2);
            BrushCache = new BrushCache();
            Font = new Font(FontFamily.GenericMonospace, 9.75f);

            _lineRenderer = new LineRenderer(this);
        }



        /// <summary>
        ///     If true, will show a colored line at the bottom when new lines have been added but the view is not scrolled to the
        ///     bottom
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("If true, will show a colored line at the bottom when new lines have been added but the view is not scrolled to the bottom")]
        public bool ShowNewLinesIndicator { get; set; }

        /// <summary>
        ///     Flags whether or not the lines should be prefixed with a timestamp
        /// </summary>
        /// <seealso cref="TimeStampFormat" />
        [Category("DonkeyScroller")]
        [Description("Flags whether or not lines should be prefixed with a timestamp")]
        public bool AddTimeStamp { get; set; }

        /// <summary>
        ///     Format for timestamp, see <see cref="DateTime.Now" /> and its ToString method
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Format of timestamp")]
        [DefaultValue("[HH:mm:ss]")]
        public string TimeStampFormat
        {
            get => _timeStampFormat;
            set => _timeStampFormat = value.Trim() + " "; // Make sure the format has an ending space
        }

        /// <summary>
        ///     The maximum number of chars that fit into the width of the chat view
        /// </summary>
        internal int MaxWidthInChars => (int)Math.Floor((decimal)TextAreaRectangle.Width / CharWidth);

        /// <summary>
        ///     The offset for lines rendering
        /// </summary>
        internal int ScrollOffset => _vScrollBar.Maximum - _vScrollBar.Value;

        /// <summary>
        ///     Cache for fonts
        /// </summary>
        internal FontCache FontCache { get; private set; }

        /// <summary>
        ///     Font to render the text with
        /// </summary>
        /// <remarks>Use only monospaced font</remarks>
        [DefaultValue(typeof(Font), "Courier New, 9.75")]
        [Category("DonkeyScroller")]
        [Description("Select only monospaced fonts. Selecting something else will revert to 'Courier New'")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                if (!Utils.IsMonoSpaceFont(base.Font))
                    base.Font = new Font(@"Courier New", base.Font.SizeInPoints, FontStyle.Regular, GraphicsUnit.Point);
                SizeF size = Utils.GetCharSize(base.Font, 'M');
                CharWidth = (int)Math.Ceiling(size.Width * 1f /*0.85*/) - 1 /*0*/;
                CharHeight = LineSpacing + (int)Math.Ceiling(size.Height * 1f /*0.9*/) - 1 /*0*/;
                FontCache = new FontCache(base.Font);

                _lines.SetFont(base.Font);

                Invalidate();
            }
        }

        /// <summary>
        ///     Height of the tallest char
        /// </summary>
        internal int CharHeight { get; private set; }

        /// <summary>
        ///     Width of a char (Remember, only using fixed/mono fonts)
        /// </summary>
        internal int CharWidth { get; private set; }


        /// <summary>
        ///     Width of the gutter items
        /// </summary>
        internal int GutterItemsWidth => (_showLineIndentColors ? CharWidth : 0) + (_showLineIcons ? CharHeight : 0);

        /// <summary>
        ///     The actual rectangle available for rendering. This takes padding and the vertical scrollbar into consideration
        /// </summary>
        internal Rectangle TextAreaRectangle
        {
            get
            {
                int borderWidth = BorderStyleWidth;
                return new Rectangle(
                    Padding.Left + GutterItemsWidth + borderWidth,
                    Padding.Top + borderWidth,
                    ClientRectangle.Width - (Padding.Left + Padding.Right) - (_vScrollBar.Width + GutterItemsWidth) - borderWidth*2,
                    ClientRectangle.Height - (Padding.Top + Padding.Bottom) - borderWidth*2
                );
            }
        }

        /// <summary>
        ///     Maximum number of lines to keep track of
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Defines how many lines (10 at minimum) the view keeps track of")]
        [DefaultValue(100)]
        public int MaxLines
        {
            get => _lines.MaxLines;
            set => _lines.MaxLines = value;
        }

        /// <summary>
        ///     Background image for the chat view
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Allows the user to use a background image")]
        public new Image BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                if (_backgroundImage == value) return;

                _backgroundImage = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Flags how to draw the background image
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Applies diffent ways to draw the background image")]
        [DefaultValue(typeof(BackgroundImageMode), "Zoom")]
        public BackgroundImageMode ImageMode
        {
            get => _backgroundImageMode;
            set
            {
                if (_backgroundImageMode == value)
                    return;

                _backgroundImageMode = value;
                if (_backgroundImage != null)
                    Invalidate();
            }
        }

        /// <summary>
        ///     Number of pixels to separate each line
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Number of pixels between lines")]
        [DefaultValue(2)]
        public int LineSpacing
        {
            get => _lineSpacing;
            set
            {
                if (_lineSpacing == value)
                    return;

                _lineSpacing = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Gets or sets the text in the chat view
        /// </summary>
        /// <remarks>Settings text will replace anything currently in the chat view</remarks>
        /// <seealso cref="AddText" />
        [Description("Setting text will replace anything currently  in the view. Use AddText() to append instead.")]
        public new string Text
        {
            get => _lines.Text;
            set
            {
                Utils.LockWindowUpdate(Handle);
                Clear();
                IEnumerable<string> lines = value.Split('\n').Reverse();
                foreach (string line in lines)
                    AddText(line.Trim());

                Utils.UnlockWindowUpdate(Handle);
            }
        }

        /// <summary>
        ///     Flags whether or not the vScrollBar "tab" is at the bottom of the view
        /// </summary>
        private bool ScrollBarAtBottom => ScrollOffset == 0;

        /// <summary>
        ///     Color of new lines available indicator
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Color of new lines available indicator")]
        [DefaultValue(typeof(Color), "Red")]
        public Color NewLinesIndicatorColor { get; set; } = Color.Red;

        /// <summary>
        ///     Flags whether or not to draw line indent colors
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Flags whether or not to draw line indent colors")]
        public bool ShowLineIndentColors
        {
            get => _showLineIndentColors;
            set
            {
                if (_showLineIndentColors == value)
                    return;

                _showLineIndentColors = value;

                _lines.CalculateDimensions();
                Invalidate();
            }
        }

        /// <summary>
        ///     Default color of the font, no color codes
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Default color of the font, no color codes")]
        [DefaultValue(typeof(Color), "Black")]
        public new Color ForeColor
        {
            get => _foreColor;
            set
            {
                if (_foreColor.ToArgb() == value.ToArgb())
                    return;

                _foreColor = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Color of the view
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Color of the view")]
        [DefaultValue(typeof(Color), "White")]
        public new Color BackColor
        {
            get => _backColor;
            set
            {
                if (_backColor.ToArgb() == value.ToArgb())
                    return;

                _backColor = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Color of line separator
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Color of line separator")]
        [DefaultValue(typeof(Color), "DarkGray")]
        public Color LineSeparatorColor
        {
            get => _lineSeparatorColor;
            set
            {
                if (_lineSeparatorColor == value)
                    return;
                _lineSeparatorColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Redirects focus to <see cref="FocusControl"/> if it is set
        /// </summary>
        private void RedirectFocus()
        {
            FocusControl?.Focus();
        }

        /// <summary>
        ///     Flags whether or a line separator should be drawn
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Flags whether or not a line separator should be drawn")]
        [DefaultValue(false)]
        public bool ShowLineSeparator
        {
            get => _showLineSeparator;
            set
            {
                if (_showLineSeparator == value)
                    return;
                _showLineSeparator = value;
                Invalidate();
            }
        }

        /// <summary>
        ///     Flags whether or not the lines should draw an icon in the gutter
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Flags whether or not the lines should draw an icon in the gutter")]
        public bool ShowLineIcons
        {
            get => _showLineIcons;
            set
            {
                if (_showLineIcons == value)
                    return;
                _showLineIcons = value;
                _lines.CalculateDimensions();
                Invalidate();
            }
        }

        /// <summary>
        ///     If set, focus is redirected to this control whenever mouse button up event occurs
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("If set, focus is redirected to this control whenever mouse button up event occurs")]
        [DefaultValue(null)]
        public Control FocusControl { get; set; }

        /// <summary>
        /// Borderstyle
        /// </summary>
        [Category("DonkeyScroller")]
        public Border3DStyle BorderStyle
        {
            get => _border3DStyle;
            set
            {
                if (_border3DStyle == value)
                    return;
                _border3DStyle = value;
                RepositionVScrollBar();
                _lines.CalculateDimensions();
                Invalidate();
            }
        }
        /// <summary>
        /// The width of currently selected <see cref="BorderStyle"/>
        /// </summary>
        internal int BorderStyleWidth
        {
            get
            {
                switch (_border3DStyle)
                {
                    case Border3DStyle.Adjust:
                        return 0;
                    case Border3DStyle.RaisedInner:
                    case Border3DStyle.RaisedOuter:
                    case Border3DStyle.SunkenInner:
                    case Border3DStyle.SunkenOuter:
                        return 1;
                    case Border3DStyle.Bump:
                    case Border3DStyle.Etched:
                    case Border3DStyle.Flat:
                    case Border3DStyle.Raised:
                    case Border3DStyle.Sunken:
                        return 2;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Border style (right side) of the gutter area
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Border style (right side) of the gutter area")]
        [DefaultValue(typeof(Border3DStyle), "RaisedOuter")]
        public Border3DStyle GutterBorderStyle
        {
            get => _gutterBorderStyle;
            set
            {
                if (_gutterBorderStyle == value)
                    return;
                _gutterBorderStyle = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Background color of gutter area
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Background color of the gutter area")]
        [DefaultValue(typeof(SystemColors),"Control")]
        public Color GutterBackgroundColor
        {
            get => _gutterBackgroundColor;
            set
            {
                if (_gutterBackgroundColor == value)
                    return;

                _gutterBackgroundColor = value;
                Invalidate();
            }
        }


        /// <summary>
        ///     Event when a line was added to the chat view
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Event when a line was added")]
        public event EventHandler<LineAddedEventArgs> LineAdded;

        /// <summary>
        ///     Triggers LineAdded event
        /// </summary>
        /// <param name="formattedText">The text with control codes</param>
        /// <param name="plainText">The text without control codes</param>
        private void OnLineAdded(string formattedText, string plainText)
        {
            LineAdded?.Invoke(this, new LineAddedEventArgs(formattedText, plainText));
        }

        /// <summary>
        ///     Event that is fired when lines has been added but the views scroll is not at the bottom
        /// </summary>
        [Description("Event that is fired when lines has been added but the views scroll is not at the bottom")]
        [Category("DonkeyScroller")]
        public event EventHandler InvisibleLinesAdded;

        /// <summary>
        ///     Triggers <see cref="InvisibleLinesAdded" /> event
        /// </summary>
        private void OnInvisibleLinesAdded()
        {
            InvisibleLinesAdded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///     Event for double clicking an interactive text
        /// </summary>
        [Category("DonkeyScroller")]
        [Description("Event that triggers when use doubleclicks a interactive text")]
        public event EventHandler<InteractiveTextEventArgs> InteractiveTextClick;

        /// <summary>
        ///     Handle special cases
        ///     1. Double click on URL or Interactive Text will return without propagating the message to the control,
        ///     this way we can filter out OnDoubleClick events when not desired
        /// </summary>
        /// <param name="m">The message</param>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_LBUTTONDBLCLK:
                    {
                        Point point = PointToClient(MousePosition);


                        InteractiveUrl interactiveUrl = GetInteractiveUrlAtPoint(point);
                        if (interactiveUrl != null)
                        {
                            OnMouseDoubleClickUrl(interactiveUrl);
                            // Return without propagating the message
                            return;
                        }
                        InteractiveText interactiveText = GetInteractiveTextAtPoint(point);
                        if (interactiveText != null)
                        {
                            OnInteractiveTextDoubleClick(interactiveText);
                            // Return without propagating the message
                            return;
                        }

                        break;
                    }
            }
            base.WndProc(ref m);
        }

        /// <summary>
        ///     When user moves the mouse, look for <see cref="InteractiveUrl" /> and <see cref="InteractiveText" /> and show the
        ///     Hand cursor if appropriate.
        ///     If the mouse is down while moving, track selection
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            
            _mouseRowColumn = PointToRowColumn(e.Location);
            base.OnMouseMove(e);

            InteractiveUrl interactiveUrl = GetInteractiveUrlAtPoint(e.Location);
            InteractiveText interactiveText = GetInteractiveTextAtPoint(e.Location);

            HandleInteractivePartHovering(interactiveUrl, interactiveText);

            Cursor = interactiveUrl != null || interactiveText != null ? Cursors.Hand : Cursors.IBeam;

            if (_mouseIsDown)
            {
                Selection = new Selection(_mouseDownRowColumn, _mouseRowColumn);
                Invalidate();
            }
        }

        /// <summary>
        ///     Triggers mouse leave/enter events for interactive parts
        /// </summary>
        /// <param name="interactiveUrl">Interactive URL</param>
        /// <param name="interactiveText">Interactive Text</param>
        private void HandleInteractivePartHovering(InteractiveUrl interactiveUrl, InteractiveText interactiveText)
        {
            if (interactiveUrl != null)
            {
                if (_hoveringInteractiveUrl == null)
                {
                    _hoveringInteractiveUrl = interactiveUrl;
                    MouseEnterUrl?.Invoke(this, new InteractiveUrlEventArgs(interactiveUrl));
                }
            }
            else
            {
                if (_hoveringInteractiveUrl != null)
                {
                    MouseLeaveUrl?.Invoke(this, new InteractiveUrlEventArgs(_hoveringInteractiveUrl));
                    _hoveringInteractiveUrl = null;
                }
            }
            if (interactiveText != null)
            {
                if (_hoveringInteractiveText == null)
                {
                    _hoveringInteractiveText = interactiveText;
                    MouseEnterInteractiveText?.Invoke(this, new InteractiveTextEventArgs(_hoveringInteractiveText));
                }
            }
            else
            {
                if (_hoveringInteractiveText != null)
                {
                    MouseLeaveInteractiveText?.Invoke(this, new InteractiveTextEventArgs(_hoveringInteractiveText));
                    _hoveringInteractiveText = null;
                }
            }
        }


        /// <summary>
        ///     Event for mouse entering a <see cref="InteractiveText" />
        /// </summary>
        [Description("Event for mouse entering a InteractiveText")]
        [Category("DonkeyScroller")]
        public event EventHandler<InteractiveTextEventArgs> MouseEnterInteractiveText;

        /// <summary>
        ///     Event for mouse leaving a <see cref="InteractiveText" />
        /// </summary>
        [Description("Event for mouse leaving a InteractiveText")]
        [Category("DonkeyScroller")]
        public event EventHandler<InteractiveTextEventArgs> MouseLeaveInteractiveText;

        /// <summary>
        ///     Event for mouse entering an <see cref="InteractiveUrl" />
        /// </summary>
        [Description("Event for mouse entering an InteractiveUrl")]
        [Category("DonkeyScroller")]
        public event EventHandler<InteractiveUrlEventArgs> MouseEnterUrl;

        /// <summary>
        ///     Event for mouse leaving an <see cref="InteractiveUrl" />
        /// </summary>
        [Description("Event for mouse leaving an InteractiveUrl")]
        [Category("DonkeyScroller")]
        public event EventHandler<InteractiveUrlEventArgs> MouseLeaveUrl;

        /// <summary>
        ///     Event for double clicking <see cref="InteractiveUrl" />
        /// </summary>
        [Description("Event for double clicking InteractiveUrl")]
        [Category("DonkeyScroller")]
        public event EventHandler<InteractiveUrlEventArgs> DoubleClickUrl;

        /// <summary>
        ///     Event for marking text
        /// </summary>
        [Description("Event for marking text")]
        [Category("DonkeyScroller")]
        public event EventHandler<MarkedTextEventArgs> MarkedText;

        /// <summary>
        ///     Triggers MarkedText Event
        /// </summary>
        /// <param name="text">The marked text</param>
        private void OnMarkedText(string text)
        {
            MarkedText?.Invoke(this, new MarkedTextEventArgs(text));
        }

        /// <summary>
        ///     Triggers MouseDoubleClickUrl event
        /// </summary>
        /// <param name="interactiveUrl"><see cref="InteractiveUrl" /> associated with event</param>
        private void OnMouseDoubleClickUrl(InteractiveUrl interactiveUrl)
        {
            DoubleClickUrl?.Invoke(this, new InteractiveUrlEventArgs(interactiveUrl));
        }

        /// <summary>
        ///     Triggers InteractiveTextClick event
        /// </summary>
        /// <param name="interactiveText">The <see cref="InteractiveText" /> associated with event</param>
        private void OnInteractiveTextDoubleClick(InteractiveText interactiveText)
        {
            InteractiveTextClick?.Invoke(this, new InteractiveTextEventArgs(interactiveText));
        }

        /// <summary>
        ///     Start selection marking
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (Utils.CheckMouseButton(e, MouseButtons.Left))
            {
                _mouseDownRowColumn = PointToRowColumn(e.Location);
                _mouseIsDown = true;
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        ///     End selection marking. If selection yelded in marked text, trigger MarkedText event
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_markedContent.HasText)
            {
                //Clipboard.SetText(_markedContent.ToString());
                OnMarkedText(_markedContent.ToString());
            }
            _mouseIsDown = false;
            Selection = null;
            Invalidate();

            if (Utils.CheckMouseButton(e, MouseButtons.Right))
                ContextMenuStrip?.Show(this, e.Location);

            base.OnMouseUp(e);

            RedirectFocus();
        }

        /// <summary>
        ///     When the chat views size changes we need to do some calculations about our lines and redraw everything
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnClientSizeChanged(EventArgs e)
        {
            RepositionVScrollBar();

            _lines.CalculateDimensions();
            Invalidate();
            base.OnClientSizeChanged(e);
        }

        private void RepositionVScrollBar()
        {
            int borderWidth = BorderStyleWidth;

            _vScrollBar.Location = new Point(Width - _vScrollBar.Width - borderWidth, borderWidth);
            _vScrollBar.Height = Height - borderWidth * 2;
        }

        /// <summary>
        ///     Scrolling the the mousewheel will affect the vertical scrollbar
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            int step = 1;
            if (ModifierKeys == Keys.Shift)
                step = 10;
            if (ModifierKeys == Keys.Control)
                step = 90000000; // Something ridiculously huge, it will be handled as going to top or bottom

            if (e.Delta > 0)
            {
                try
                {
                    _vScrollBar.Value -= step;
                }
                catch (ArgumentOutOfRangeException)
                {
                    _vScrollBar.Value = 0;
                }
            }
            else
            {
                try
                {
                    _vScrollBar.Value += step;
                }
                catch (ArgumentOutOfRangeException)
                {
                    _vScrollBar.Value = _vScrollBar.Maximum;
                }
            }
            if (ScrollBarAtBottom)
                _lines.UnseenCount = 0;
            Invalidate();
        }

        /// <summary>
        ///     Clears the chat view
        /// </summary>
        public void Clear()
        {
            _lines.Clear();
            Invalidate();
        }

        /// <summary>
        ///     Add text to the chat view. The text can be formatted like IRC text and some custom formats through
        ///     <see cref="ControlChar" />
        /// </summary>
        /// <seealso cref="DonkeyScrollerUtils.CreateInteractiveText" />
        /// <seealso cref="ControlChar" />
        /// <param name="text">The text to add. Can contain <see cref="ControlChar" /> codes</param>
        /// <param name="indentColor">
        ///     If you pass in a color, the lines indent will have this color. See
        ///     <see cref="ShowLineIndentColors" />.
        /// </param>
        /// <param name="foreColor">Default foreground color for the text</param>
        /// <param name="backColor">Default background color for the text</param>
        /// <param name="fillColor">Color to fill the entire area the text covers</param>
        /// <param name="lineIconImage">Icon for the text</param>
        /// <remarks>
        ///     <paramref name="lineIconImage" /> will be drawn at a size of <see cref="CharHeight" />*
        ///     <see cref="CharHeight" />
        /// </remarks>
        /// <example>
        ///     <code lang="C#" title="Usage" source="Content/HowTo/HowTo.cs" region="AddText"></code>
        /// </example>
        public void AddText(string text, Color? indentColor = null, Color? foreColor = null, Color? backColor = null, Color? fillColor = null, Image lineIconImage = null)
        {
            try
            {
                if (IsDisposed || string.IsNullOrWhiteSpace(text))
                    return;

                if (AddTimeStamp)
                    text = InjectTimeStampToText(text);

                Line line = Parsers.ParseLineText(text, this);

                if (indentColor.HasValue)
                    line.IndentColor = indentColor.Value;
                if (foreColor.HasValue)
                {
                    line.DefaultForeColor = foreColor;
                    line.DefaultBackColor = backColor;
                }
                if (fillColor.HasValue)
                    line.FillColor = fillColor;
                line.IconImage = lineIconImage;

                Utils.InvokeIfRequired(this, () =>
                {
                    _lines.Insert(line);

                    bool setScrollBarValue = ScrollBarAtBottom;
                    _vScrollBar.Maximum = _lines.Count;
                    if (setScrollBarValue)
                        _vScrollBar.Value = _vScrollBar.Maximum;
                    else
                    {
                        _lines.UnseenCount++;
                        OnInvisibleLinesAdded();
                    }

                    Invalidate();
                    OnLineAdded(text, line.Text);
                });
            }
            catch (InvalidAsynchronousStateException)
            {
                // do nothing :E
            }
        }

        /// <summary>
        ///     Load content from file
        /// </summary>
        /// <param name="logFile">The file to load</param>
        /// <param name="clearFirst">If true, will clear the view before loading the file</param>
        /// <example>
        ///     <code lang="C#" title="Usage" source="Content/HowTo/HowTo.cs" region="LoadFromFile"></code>
        /// </example>
        public void LoadFromFile(string logFile, bool clearFirst = false)
        {
            if (!File.Exists(logFile))
                throw new FileNotFoundException("Could not load log from file", logFile);
            if (clearFirst)
                Clear();

            using (TextReader textReader = new StreamReader(logFile))
            {
                string line;
                while ((line = textReader.ReadLine()) != null)
                {
                    AddText(line);
                }
            }
        }

        /// <summary>
        ///     Injects timestamp to text while keeping color or other formatting in the first char
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Text with timestamp based on <see cref="TimeStampFormat" /></returns>
        private string InjectTimeStampToText(string text)
        {
            string formattedTimeStamp = DateTime.Now.ToString(_timeStampFormat);

            switch (text[0])
            {
                case ControlChar.HexColor:
                    {
                        text = text.Substring(0, 13) + formattedTimeStamp + text.Substring(13);
                        break;
                    }
                case ControlChar.Color:
                    {
                        string colorString = Parsers.ParseColorsStringAt(text, 0);
                        text = colorString + formattedTimeStamp + text.Substring(colorString.Length);
                        break;
                    }
                case ControlChar.Underline:
                case ControlChar.Underline2:
                case ControlChar.Italic:
                case ControlChar.StrikeThrough:
                case ControlChar.Bold:
                    {
                        text = text[0] + formattedTimeStamp + text.Substring(1);
                        break;
                    }
                default:
                    {
                        text = formattedTimeStamp + text;
                        break;
                    }
            }
            return text;
        }

        /// <summary>
        ///     Paints the chat view
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_backgroundImage != null)
                DrawBackgroundImage(e);
            else
                e.Graphics.Clear(_backColor);

            if (GutterItemsWidth > 0)
                DrawGutterBackground(e.Graphics, GutterItemsWidth);

            if (!ScrollBarAtBottom && _lines.UnseenCount > 0)
                using (Pen pen = new Pen(NewLinesIndicatorColor) { Width = 2f })
                {
                    e.Graphics.DrawLine(pen, 0, ClientRectangle.Bottom - 1, ClientRectangle.Width, ClientRectangle.Bottom - 1);
                }

            int y = TextAreaRectangle.Bottom;
            int drawRow = -1;
            _markedContent.ClearText();
            _lines.Rectangles.Clear();
            foreach (Line line in _lines.Skip(ScrollOffset))
            {
                float x = TextAreaRectangle.Left;

                // Record a rectangle this line occupies (used for mouse selection, clicking etc.)
                //_lines.Rectangles.Add(line, new Rectangle(
                //    TextAreaRectangle.Left,
                //    y - CharHeight * line.RowsHigh,
                //    TextAreaRectangle.Width,
                //    CharHeight * line.RowsHigh));
                _lines.Rectangles.Add(line, new Rectangle(
                    TextAreaRectangle.Left,
                    y - line.Height,
                    TextAreaRectangle.Width,
                    line.Height));
                _markedContent.ClearChars();


                //int oldY = y;
                y = _lineRenderer.Draw(line, e.Graphics, ref x, y, ref drawRow, _markedContent.Chars);
                /*if (_showLineIndentColors)
                {
                    e.Graphics.FillRectangle(BrushCache.Get(line.IndentColor), 0, y, CharWidth, oldY - y);
                }*/
                _markedContent.InsertChars();
                drawRow++;
                if (y < 0)
                    break;
            }


            _markedContent.TrimText();

            Draw3DBorder(e.Graphics);

            base.OnPaint(e);
        }
        /// <summary>
        /// Draws the gutter background
        /// </summary>
        /// <param name="graphics">Graphics object</param>
        /// <param name="gutterWidth">Width of the gutter</param>
        private void DrawGutterBackground(Graphics graphics, int gutterWidth)
        {
            Rectangle gutterRectangle = new Rectangle(0, 0, gutterWidth, Height);
            graphics.FillRectangle(BrushCache.Get(_gutterBackgroundColor), gutterRectangle);
            ControlPaint.DrawBorder3D(graphics, gutterRectangle,_gutterBorderStyle,Border3DSide.Right);
        }

        private void Draw3DBorder(Graphics g)
        {
            if (_border3DStyle == Border3DStyle.Adjust)
                return;

            ControlPaint.DrawBorder3D(g, ClientRectangle, _border3DStyle);
        }

        /// <summary>
        ///     Gets specified data under the mouse
        /// </summary>
        /// <param name="underMouseDataRequest">The data to get</param>
        /// <returns>The requested data as a string</returns>
        public string GetDataUnderMouse(UnderMouseDataRequest underMouseDataRequest)
        {
            Point point = PointToClient(MousePosition);
            switch (underMouseDataRequest)
            {
                case UnderMouseDataRequest.Data:
                    {
                        InteractiveText interactiveText = GetInteractiveTextAtPoint(point);
                        if (interactiveText != null)
                        {
                            return interactiveText.Data;
                        }
                        break;
                    }
                case UnderMouseDataRequest.InteractiveText:
                case UnderMouseDataRequest.InteractiveTextData:
                    {
                        InteractiveText interactiveText = GetInteractiveTextAtPoint(point);
                        if (interactiveText != null)
                        {
                            return underMouseDataRequest == UnderMouseDataRequest.InteractiveTextData ? interactiveText.Data : interactiveText.Text;
                        }
                        break;
                    }
                case UnderMouseDataRequest.InteractiveUrl:
                    {
                        InteractiveUrl interactiveUrl = GetInteractiveUrlAtPoint(point);
                        if (interactiveUrl != null)
                        {
                            return interactiveUrl.Url;
                        }
                        break;
                    }
                case UnderMouseDataRequest.Line:
                    {
                        Line line = LineAtLocation(point);
                        if (line != null)
                            return line.Text;
                        break;
                    }
                case UnderMouseDataRequest.Word:
                    {
                        Line line = LineAtLocation(point);
                        if (line != null)
                        {
                            string word = GetLineWordAt(point, line);
                            if (!string.IsNullOrEmpty(word))
                                return word;
                        }
                        break;
                    }
            }
            return "";
        }


        /// <summary>
        ///     Draws the background image. Style of paint is defined by <see cref="BackgroundImageMode" />
        /// </summary>
        /// <param name="e"></param>
        private void DrawBackgroundImage(PaintEventArgs e)
        {
            switch (_backgroundImageMode)
            {
                case BackgroundImageMode.TopLeft:
                    {
                        e.Graphics.DrawImage(_backgroundImage, new Point(0, 0));
                        break;
                    }
                case BackgroundImageMode.Centered:
                    {
                        int x = ClientRectangle.Width / 2 - _backgroundImage.Width / 2;
                        int y = ClientRectangle.Height / 2 - _backgroundImage.Height / 2;
                        e.Graphics.DrawImage(_backgroundImage, new Point(x, y));
                        break;
                    }
                case BackgroundImageMode.Tiled:
                    {
                        using (TextureBrush tex = new TextureBrush(_backgroundImage, WrapMode.Tile))
                        {
                            e.Graphics.FillRectangle(tex, ClientRectangle);
                        }
                        break;
                    }
                case BackgroundImageMode.Stretch:
                    {
                        e.Graphics.DrawImage(_backgroundImage, ClientRectangle);
                        break;
                    }
                case BackgroundImageMode.Zoom:
                    {
                        /*
                        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                        e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                        */

                        Rectangle imageRectangle = new Rectangle();

                        float ratio = Math.Max(ClientRectangle.Width / (float)_backgroundImage.Size.Width, ClientRectangle.Height / (float)_backgroundImage.Size.Height);
                        imageRectangle.Width = (int)(_backgroundImage.Size.Width * ratio);
                        imageRectangle.Height = (int)(_backgroundImage.Size.Height * ratio);
                        imageRectangle.X = (ClientRectangle.Width - imageRectangle.Width) / 2;
                        imageRectangle.Y = (ClientRectangle.Height - imageRectangle.Height) / 2;

                        e.Graphics.DrawImage(_backgroundImage, imageRectangle);

                        break;
                    }
            }
        }
        /// <summary>
        /// Background color of gutter area
        /// </summary>
        private Color _gutterBackgroundColor = SystemColors.Control;
        /// <summary>
        /// Border style (right side) of the gutter area
        /// </summary>
        private Border3DStyle _gutterBorderStyle = Border3DStyle.RaisedOuter;

        /// <summary>
        ///     Converts mouse point to a Row and Column
        /// </summary>
        /// <param name="point">Position of the mouse</param>
        /// <returns><see cref="RowColumn" /> based on <paramref name="point" /></returns>
        private RowColumn PointToRowColumn(Point point)
        {
            
            RowColumn rc = new RowColumn
            {
                Column = (point.X - GutterItemsWidth + BorderStyleWidth) / CharWidth,
                Row = (TextAreaRectangle.Height - point.Y) / CharHeight
            };
            return rc;
        }

        /// <summary>
        ///     Gets URL at mouse position, if any
        /// </summary>
        /// <param name="point">Position of the mouse</param>
        /// <returns>If found, a <see cref="InteractiveUrl" /></returns>
        private InteractiveUrl GetInteractiveUrlAtPoint(Point point)
        {
            Line line = LineAtLocation(point);
            if (line == null) return null;
            if (line.InteractiveUrls.Count == 0) return null;
            if (!_lines.Rectangles.ContainsKey(line)) return null;

            int charIndex = GetLineCharIndexFromPoint(point, line);

            return line.InteractiveUrls.Get(charIndex);
        }

        /// <summary>
        ///     Gets <see cref="InteractiveText" /> at mouse position
        /// </summary>
        /// <param name="point">Mouse position</param>
        /// <returns>If found, a <see cref="InteractiveText" /> </returns>
        private InteractiveText GetInteractiveTextAtPoint(Point point)
        {
            Line line = LineAtLocation(point);
            if (line == null) return null;
            if (line.InteractiveTexts.Count == 0) return null;
            if (!_lines.Rectangles.ContainsKey(line)) return null;

            int charIndex = GetLineCharIndexFromPoint(point, line);

            return line.InteractiveTexts.Get(charIndex);
        }

        /// <summary>
        ///     Gets the index of a char on a line at mouse position
        /// </summary>
        /// <param name="point">Mouse position</param>
        /// <param name="line">The line to look for char index in</param>
        /// <returns>index of char in a <see cref="Line" /> at <paramref name="point" /> in <paramref name="line" /></returns>
        private int GetLineCharIndexFromPoint(Point point, Line line)
        {
            Rectangle r = _lines.Rectangles[line];

            // Relative X coordinate .. 0 being at the very left of the specific line
            int x = point.X - r.Left;
            // Relative Y coordinate .. 0 being at the very top of the specific line
            int y = point.Y - r.Top;

            // Convert pixel coords to row and column, keep 0 as 0
            if (y > 0)
                y = y / CharHeight;
            if (x > 0)
                x = x / CharWidth;

            if (line.WrapIndexes.Count > 0)
                return GetLineCharIndexFromWrappedLine(x, y, line);
            // We have a line that is NOT wrapped, we can just calculate the index
            return y * MaxWidthInChars + x;
        }

        /// <summary>
        ///     Gets char index of a line at relative x and y coordinates
        /// </summary>
        /// <param name="x">Relative X char coordinate .. 0 being at the very first char on a row</param>
        /// <param name="y">Relative Y char coordinate .. 0 being at the very first row of wrapped line</param>
        /// <param name="line">The line</param>
        /// <returns>
        ///     index of char in a <see cref="Line" /> at <paramref name="x" /> and <paramref name="y" /> coordinates in
        ///     <paramref name="line" />
        /// </returns>
        private static int GetLineCharIndexFromWrappedLine(int x, int y, Line line)
        {
            int row = 0;
            int column = 0;
            // Iterate through each char while keeping track of row and column
            for (int charIndex = 0; charIndex < line.Count; charIndex++)
            {
                if (line.WrapIndexes.Contains(charIndex))
                {
                    // We are at a wrapping point, increase row, reset column
                    row++;
                    column = 0;
                }
                if (row == y && column == x)
                    return charIndex;
                // Advance to next column on the row
                column++;
            }
            // Could not find a index at x and y
            return -1;
        }

        /// <summary>
        ///     Gets a word at specific row and column
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns>A word, if found at <paramref name="point" /> in <paramref name="line" /></returns>
        private string GetLineWordAt(Point point, Line line)
        {
            Rectangle r = _lines.Rectangles[line];

            // Relative X coordinate .. 0 being at the very left of the specific line
            int x = point.X - r.Left;
            // Relative Y coordinate .. 0 being at the very top of the specific line
            int y = point.Y - r.Top;

            // Convert pixel coords to row and column, keep 0 as 0
            if (y > 0)
                y = y / CharHeight;
            if (x > 0)
                x = x / CharWidth;


            int row = 0;
            int column = 0;
            // Iterate through each char while keeping track of row and column
            for (int charIndex = 0; charIndex < line.Count; charIndex++)
            {
                if (line.WrapIndexes.Contains(charIndex))
                {
                    // We are at a wrapping point, increase row, reset column
                    row++;
                    column = 0;
                }
                if (row == y && column == x)
                {
                    char c = line[charIndex].C;

                    // Is user pointing at a char that is considered being a part of a word?
                    if (!NonWordChars.Contains(c))
                    {
                        // Start with the current char
                        string result = c.ToString();

                        // Iterate backwards from char the user is pointing at
                        for (int i = charIndex - 1; i >= 0; i--)
                        {
                            char thisC = line[i].C;

                            // if this iteration char is a "nonword" char, bail out
                            if (NonWordChars.Contains(thisC))
                                break;

                            // char is considered part of word, prepend it
                            result = thisC + result;
                        }
                        // iterate forward from char the user is pointing at
                        for (int i = charIndex + 1; i < line.Count; i++)
                        {
                            char thisC = line[i].C;
                            // if this iteration char is a "nonword" char, bail out
                            if (NonWordChars.Contains(thisC))
                                break;
                            // char is considered part of word, append it
                            result += thisC;
                        }
                        // return the word
                        return result;
                    }
                }
                // Advance to next column on the row
                column++;
            }
            // No word found
            return "";
        }

        /// <summary>
        ///     Gets a line at mouse position
        /// </summary>
        /// <param name="point">Mouse position</param>
        /// <returns><see cref="Line" /> at <paramref name="point" /></returns>
        private Line LineAtLocation(Point point)
        {
            return (from kvp in _lines.Rectangles where kvp.Value.Contains(point) select kvp.Key).FirstOrDefault();
        }
    }
}