namespace JaniPellikka.Windows.Forms
{
    /// <summary>
    /// Defines how the background image should be drawn
    /// </summary>
    public enum BackgroundImageMode
    {
        /// <summary>
        ///  Image is centered without resizing
        /// </summary>
        Centered,
        /// <summary>
        /// Image is tiled across the view
        /// </summary>
        Tiled,
        /// <summary>
        /// Image is put in the top left corner
        /// </summary>
        TopLeft,
        /// <summary>
        /// Stretches the image over the view
        /// </summary>
        Stretch,
        /// <summary>
        /// Stretches the image over the view while keeping aspect ratio
        /// </summary>
        Zoom
    }
}