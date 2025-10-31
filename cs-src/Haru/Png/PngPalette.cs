using System;

namespace Haru.Png
{
    /// <summary>
    /// Represents a PNG palette for indexed color images.
    /// </summary>
    public class PngPalette
    {
        /// <summary>
        /// Array of RGB color entries. Each entry contains 3 bytes (R, G, B).
        /// Length is ColorCount * 3.
        /// </summary>
        public byte[] Colors { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Number of colors in the palette.
        /// </summary>
        public int ColorCount { get; set; }

        /// <summary>
        /// Gets the RGB color at the specified index.
        /// </summary>
        public (byte R, byte G, byte B) GetColor(int index)
        {
            if (index < 0 || index >= ColorCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            int offset = index * 3;
            return (Colors[offset], Colors[offset + 1], Colors[offset + 2]);
        }
    }
}
