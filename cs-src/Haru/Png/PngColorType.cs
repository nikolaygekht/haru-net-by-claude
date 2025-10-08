namespace Haru.Png
{
    /// <summary>
    /// PNG color types as defined in PNG specification.
    /// </summary>
    public enum PngColorType
    {
        /// <summary>
        /// Grayscale: 1, 2, 4, 8, or 16 bits per pixel.
        /// </summary>
        Grayscale = 0,

        /// <summary>
        /// RGB: 8 or 16 bits per channel (24 or 48 bits per pixel).
        /// </summary>
        Rgb = 2,

        /// <summary>
        /// Palette (indexed color): 1, 2, 4, or 8 bits per pixel.
        /// </summary>
        Palette = 3,

        /// <summary>
        /// Grayscale with alpha: 8 or 16 bits per channel (16 or 32 bits per pixel).
        /// </summary>
        GrayscaleAlpha = 4,

        /// <summary>
        /// RGB with alpha: 8 or 16 bits per channel (32 or 64 bits per pixel).
        /// </summary>
        RgbAlpha = 6
    }
}
