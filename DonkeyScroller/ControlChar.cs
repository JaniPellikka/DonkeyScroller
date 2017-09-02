namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// char codes that control the rendering of lines of text
    /// </summary>

    public static class ControlChar
    {
        /// <summary>
        /// Toggle bold style
        /// </summary>
        /// <example><code>
        /// string str = $@"This is {ControlChar.Bold}bold";
        /// </code></example>
        public const char Bold = (char)0x02;
        /// <summary>
        /// Colored text
        /// </summary>
        /// <example><code>
        /// string str1 = $@"This is {ControlChar.Color}4Red text";
        /// string str2 = $@"This is {ControlChar.Color}4,1Red text with black background";
        /// </code></example>
        public const char Color = (char)0x03;
        /// <summary>
        /// URL start and end indicator
        /// </summary>
        /// <remarks>Do NOT add this manually as it is injected when parsing a text</remarks>
        internal const char Url = (char)0x04;
        /// <summary>
        /// Indicator for interactive text start
        /// </summary>
        /// <example><code>
        ///  string str = $@"{ControlCode.InteractiveTextStart}4{ControlCode.InteractiveTextSplit}Four{ControlChar.InteractiveTextEnd}";
        ///  </code>
        /// <remarks>The str now contains a coded InteractiveText with 4 as Data and "Four" as its display text</remarks>
        /// </example>
        /// <seealso cref="DonkeyScrollerUtils.CreateInteractiveText"/>
        public const char InteractiveTextStart = (char)0x05;
        /// <summary>
        /// Indicator for interactive text separator. The text before is the Id, text after is the text to display
        /// </summary>

        public const char InteractiveTextSplit = (char)0x06;
        /// <summary>
        /// Indicator for interactive text end
        /// </summary>
        public const char InteractiveTextEnd = (char)0x07;
        /// <summary>
        /// Toggle italis style
        /// </summary>
        /// <example><code>
        /// string str = $@"This is {ControlChar.Italic}italic";
        /// </code></example>
        public const char Italic = (char)0x09;
        /// <summary>
        /// Custom hexcolor
        /// </summary>
        /// <remarks>Must define back and fore color like RRGGBBRRGGBB
        /// If backcolor is not be used; use RRGGBBXXXXXX; XX is not a valid hex and will be treated as no color</remarks>
        /// <see cref="DonkeyScrollerUtils.CreateHexColorString"/>
        /// <example><code>
        /// string str = $@"{ControlChar.HexColor}FF0000FFFF00Red text on yellow background";
        /// </code></example>
        public const char HexColor = (char)0x10;
        /// <summary>
        /// Toggle strike through style
        /// </summary>
        /// <example><code>
        /// string str = $@"This is {ControlChar.StrikeThrough}strikethrough";
        /// </code></example>
        public const char StrikeThrough = (char)0x13;
        /// <summary>
        /// Reset styles
        /// </summary>
        /// <example><code>
        /// string str = $@"This is {ControlChar.Bold}bold{ControlChar.Reset} but this is not";
        /// </code>
        /// <remarks>The word bold is in bold style, but the rest are in default style. This applies to colors as well</remarks>
        /// </example>
        public const char Reset = (char)0x0f;
        /// <summary>
        /// Toggle underline style
        /// </summary>
        /// <example><code>
        /// string str = $@"This is {ControlChar.Underline}underlined";
        /// </code></example>
        public const char Underline = (char)0x15;
        /// <summary>
        /// Toggle underline2 style 
        /// </summary>
        /// <remarks>To be hones, I cannot remember where I got this and why underline would need two different codes??</remarks>
        public const char Underline2 = (char)0x1f;
        /// <summary>
        /// Swap foreground and background colors around
        /// </summary>
        /// <example><code>
        /// string str = $@"This is {ControlChar.Color}4,1 RED ON BLACK {ControlChar.Reverse} BLACK ON RED";
        /// </code></example>
        public const char Reverse = (char)0x16;
    }
}