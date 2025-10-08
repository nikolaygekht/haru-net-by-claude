using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Formats.Png;

namespace Haru.Png
{
    /// <summary>
    /// Implementation of IPngReader using SixLabors.ImageSharp library.
    /// </summary>
    public class ImageSharpPngReader : IPngReader
    {
        private const int PngSignatureLength = 8;
        private static readonly byte[] PngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

        /// <summary>
        /// Validates if the stream contains a valid PNG signature.
        /// </summary>
        public bool ValidateSignature(Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            long originalPosition = stream.Position;
            try
            {
                byte[] header = new byte[PngSignatureLength];
                int bytesRead = stream.Read(header, 0, PngSignatureLength);

                if (bytesRead != PngSignatureLength)
                    return false;

                for (int i = 0; i < PngSignatureLength; i++)
                {
                    if (header[i] != PngSignature[i])
                        return false;
                }

                return true;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// Reads PNG image information from the stream.
        /// </summary>
        public PngImageInfo ReadImageInfo(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            long originalPosition = stream.Position;
            try
            {
                using (var image = Image.Load<Rgba32>(stream))
                {
                    var info = new PngImageInfo
                    {
                        Width = (uint)image.Width,
                        Height = (uint)image.Height,
                        BitDepth = 8, // ImageSharp normalizes to 8 bits
                        IsInterlaced = false
                    };

                    // Determine color type based on pixel format
                    info.ColorType = DetermineColorType(image);

                    // Calculate bytes per row
                    int channels = info.Channels;
                    info.BytesPerRow = (uint)(image.Width * channels);

                    // Extract palette if present
                    if (image.Metadata.TryGetPngMetadata(out PngMetadata pngMetadata))
                    {
                        info.IsInterlaced = pngMetadata.InterlaceMethod == PngInterlaceMode.Adam7;
                    }

                    return info;
                }
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// Reads the raw pixel data from the PNG image.
        /// </summary>
        public byte[] ReadPixelData(Stream stream, PngImageInfo info)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (info == null)
                throw new ArgumentNullException(nameof(info));

            long originalPosition = stream.Position;
            try
            {
                using (var image = Image.Load<Rgba32>(stream))
                {
                    int dataSize = (int)(info.BytesPerRow * info.Height);
                    byte[] pixelData = new byte[dataSize];
                    int offset = 0;

                    image.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            var rowSpan = accessor.GetRowSpan(y);
                            for (int x = 0; x < accessor.Width; x++)
                            {
                                var pixel = rowSpan[x];
                            switch (info.ColorType)
                            {
                                case PngColorType.Rgb:
                                    pixelData[offset++] = pixel.R;
                                    pixelData[offset++] = pixel.G;
                                    pixelData[offset++] = pixel.B;
                                    break;

                                case PngColorType.RgbAlpha:
                                    pixelData[offset++] = pixel.R;
                                    pixelData[offset++] = pixel.G;
                                    pixelData[offset++] = pixel.B;
                                    pixelData[offset++] = pixel.A;
                                    break;

                                case PngColorType.Grayscale:
                                    // Convert to grayscale using standard formula
                                    byte gray = (byte)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                                    pixelData[offset++] = gray;
                                    break;

                                case PngColorType.GrayscaleAlpha:
                                    byte gray2 = (byte)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                                    pixelData[offset++] = gray2;
                                    pixelData[offset++] = pixel.A;
                                    break;
                            }
                        }
                        }
                    });

                    return pixelData;
                }
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// Reads pixel data row by row for non-interlaced images.
        /// </summary>
        public void ReadPixelDataByRow(Stream stream, PngImageInfo info, Action<byte[], int> rowCallback)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            if (rowCallback == null)
                throw new ArgumentNullException(nameof(rowCallback));

            long originalPosition = stream.Position;
            try
            {
                using (var image = Image.Load<Rgba32>(stream))
                {
                    byte[] rowBuffer = new byte[info.BytesPerRow];

                    image.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            var rowSpan = accessor.GetRowSpan(y);
                            int offset = 0;

                            for (int x = 0; x < accessor.Width; x++)
                            {
                                var pixel = rowSpan[x];
                                switch (info.ColorType)
                                {
                                    case PngColorType.Rgb:
                                        rowBuffer[offset++] = pixel.R;
                                        rowBuffer[offset++] = pixel.G;
                                        rowBuffer[offset++] = pixel.B;
                                        break;

                                    case PngColorType.RgbAlpha:
                                        rowBuffer[offset++] = pixel.R;
                                        rowBuffer[offset++] = pixel.G;
                                        rowBuffer[offset++] = pixel.B;
                                        rowBuffer[offset++] = pixel.A;
                                        break;

                                    case PngColorType.Grayscale:
                                        byte gray = (byte)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                                        rowBuffer[offset++] = gray;
                                        break;

                                    case PngColorType.GrayscaleAlpha:
                                        byte gray2 = (byte)(0.299 * pixel.R + 0.587 * pixel.G + 0.114 * pixel.B);
                                        rowBuffer[offset++] = gray2;
                                        rowBuffer[offset++] = pixel.A;
                                        break;
                                }
                            }

                            rowCallback(rowBuffer, y);
                        }
                    });
                }
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        private PngColorType DetermineColorType(Image<Rgba32> image)
        {
            // Check if image has alpha channel by sampling pixels
            bool hasAlpha = false;
            bool hasColor = false;

            // Sample some pixels to determine color type
            int sampleStep = Math.Max(1, Math.Min(image.Width, image.Height) / 10);

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height && (!hasAlpha || !hasColor); y += sampleStep)
                {
                    var rowSpan = accessor.GetRowSpan(y);
                    for (int x = 0; x < accessor.Width && (!hasAlpha || !hasColor); x += sampleStep)
                    {
                        var pixel = rowSpan[x];

                        if (pixel.A < 255)
                            hasAlpha = true;

                        if (pixel.R != pixel.G || pixel.G != pixel.B)
                            hasColor = true;
                    }
                }
            });

            if (hasColor)
                return hasAlpha ? PngColorType.RgbAlpha : PngColorType.Rgb;
            else
                return hasAlpha ? PngColorType.GrayscaleAlpha : PngColorType.Grayscale;
        }

        public void Dispose()
        {
            // No resources to dispose in this implementation
        }
    }
}
