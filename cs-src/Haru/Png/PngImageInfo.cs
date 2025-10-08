using System;

namespace Haru.Png
{
    /// <summary>
    /// Represents PNG image information extracted from the image header.
    /// </summary>
    public class PngImageInfo
    {
        /// <summary>
        /// Image width in pixels.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Image height in pixels.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Bits per channel (1, 2, 4, 8, or 16).
        /// </summary>
        public int BitDepth { get; set; }

        /// <summary>
        /// Color type of the image.
        /// </summary>
        public PngColorType ColorType { get; set; }

        /// <summary>
        /// Whether the image is interlaced.
        /// </summary>
        public bool IsInterlaced { get; set; }

        /// <summary>
        /// Number of bytes per row.
        /// </summary>
        public uint BytesPerRow { get; set; }

        /// <summary>
        /// Palette data for indexed color images.
        /// </summary>
        public PngPalette Palette { get; set; }

        /// <summary>
        /// Transparency information.
        /// </summary>
        public PngTransparency Transparency { get; set; }

        /// <summary>
        /// Gets the number of channels based on color type.
        /// </summary>
        public int Channels
        {
            get
            {
                switch (ColorType)
                {
                    case PngColorType.Grayscale:
                        return 1;
                    case PngColorType.GrayscaleAlpha:
                        return 2;
                    case PngColorType.Rgb:
                        return 3;
                    case PngColorType.RgbAlpha:
                        return 4;
                    case PngColorType.Palette:
                        return 1;
                    default:
                        throw new InvalidOperationException($"Unknown color type: {ColorType}");
                }
            }
        }

        /// <summary>
        /// Gets whether this image has an alpha channel.
        /// </summary>
        public bool HasAlpha => ColorType == PngColorType.GrayscaleAlpha ||
                                ColorType == PngColorType.RgbAlpha ||
                                (ColorType == PngColorType.Palette && Transparency?.HasTransparency == true);
    }
}
