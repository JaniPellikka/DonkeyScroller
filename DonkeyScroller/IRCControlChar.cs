namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Chars used by IRC to format text and some DonkeyScroller specific codes
    /// </summary>
    public static class IRCControlChar
    {
        /// <summary>
        /// Toggle bold style
        /// </summary>
        public const char Bold = (char)0x02;
        /// <summary>
        /// Toggle colored text
        /// </summary>
        public const char Color = (char)0x03;
        /// <summary>
        /// URL start and end indicator
        /// </summary>
        public const char Url = (char)0x04;
        /// <summary>
        /// Indicator for interactive text start
        /// </summary>
        public const char InteractiveStart = (char)0x05;
        /// <summary>
        /// Indicator for interactive text separator. The text before is the Id, text after is the text to display
        /// </summary>

        public const char InteractiveSplit = (char)0x06;
        /// <summary>
        /// Indicator for interactive text end
        /// </summary>
        public const char InteractiveEnd = (char)0x07;
        /// <summary>
        /// Toggle italis style
        /// </summary>
        public const char Italic = (char)0x09;
        /** 
         * Custom hexcolor
         * Must define back and fore color like RRGGBBRRGGBB
         * If backcolor is not be used; use RRGGBBXXXXXX; XX is not a valid hex and will be treated as no color
         */
        public const char HexColor = (char)0x10;
        /// <summary>
        /// Toggle strike through style
        /// </summary>
        public const char StrikeThrough = (char)0x13;
        /// <summary>
        /// Reset styles
        /// </summary>
        public const char Reset = (char)0x0f;
        /// <summary>
        /// Toggle underline style
        /// </summary>
        public const char Underline = (char)0x15;
        /// <summary>
        /// Toggle underline2 style 
        /// </summary>
        /// <remarks>To be hones, I cannot remember where I got this and why underline would need two different codes??</remarks>
        public const char Underline2 = (char)0x1f;
        /// <summary>
        /// Swap foreground and background colors around
        /// </summary>
        public const char Reverse = (char)0x16;
        /// <summary>
        /// CTCP command
        /// </summary>
        public const char Ctcp = (char)0x1;
    }
}