namespace Haru.Png
{
    /// <summary>
    /// Represents PNG transparency information from the tRNS chunk.
    /// </summary>
    public class PngTransparency
    {
        /// <summary>
        /// Alpha values for palette entries (for indexed color images).
        /// </summary>
        public byte[] PaletteAlpha { get; set; }

        /// <summary>
        /// Number of transparent palette entries.
        /// </summary>
        public int TransparentColorCount { get; set; }

        /// <summary>
        /// Gets whether transparency information is present.
        /// </summary>
        public bool HasTransparency => PaletteAlpha != null && TransparentColorCount > 0;
    }
}
