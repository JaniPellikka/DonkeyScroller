using JaniPellikka.Windows.Forms.Interactive;

namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Enum type of fetching data under the mouse
    /// </summary>
    public enum UnderMouseDataRequest
    {
        /// <summary>
        /// Any kind of URI, file://, http:// etc
        /// </summary>
        InteractiveUrl,
        /// <summary>
        /// Text of an <see cref="Interactive.InteractiveText"/>
        /// </summary>
        InteractiveText,
        /// <summary>
        /// Text of an <see cref="Interactive.InteractiveText.Data"/>
        /// </summary>
        InteractiveTextData,
        /// <summary>
        /// Data of an <see cref="Interactive.InteractiveText"/>
        /// </summary>
        Data,
        /// <summary>
        /// Word under the mouse
        /// </summary>
        Word,
        /// <summary>
        /// The line text under the mouse
        /// </summary>
        Line
    }
}