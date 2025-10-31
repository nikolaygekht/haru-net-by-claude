using System;
using System.IO;
using StbImageSharp;

namespace Haru.Png
{
    /// <summary>
    /// Implementation of IPngReader using StbImageSharp library.
    /// Supports both PNG and JPEG image formats.
    /// </summary>
    public class StbImageReader : IPngReader
    {
        private const int PngSignatureLength = 8;
        private static readonly byte[] PngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

        private const int JpegSignatureLength = 2;
        private static readonly byte[] JpegSignature = new byte[] { 0xFF, 0xD8 };

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
        /// Validates if the stream contains a valid JPEG signature.
        /// </summary>
        public bool ValidateJpegSignature(Stream stream)
        {
            if (stream == null || !stream.CanRead)
                return false;

            long originalPosition = stream.Position;
            try
            {
                byte[] header = new byte[JpegSignatureLength];
                int bytesRead = stream.Read(header, 0, JpegSignatureLength);

                if (bytesRead != JpegSignatureLength)
                    return false;

                for (int i = 0; i < JpegSignatureLength; i++)
                {
                    if (header[i] != JpegSignature[i])
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
        /// Reads PNG or JPEG image information from the stream.
        /// </summary>
        public PngImageInfo ReadImageInfo(Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            long originalPosition = stream.Position;
            try
            {
                // Load image using StbImageSharp
                stream.Position = 0;
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                var info = new PngImageInfo
                {
                    Width = (uint)image.Width,
                    Height = (uint)image.Height,
                    BitDepth = 8, // StbImageSharp normalizes to 8 bits
                    IsInterlaced = false
                };

                // Determine color type based on original components
                info.ColorType = DetermineColorType(image);

                // Calculate bytes per row
                int channels = info.Channels;
                info.BytesPerRow = (uint)(image.Width * channels);

                return info;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        /// <summary>
        /// Reads the raw pixel data from the PNG or JPEG image.
        /// </summary>
        public byte[] ReadPixelData(Stream stream, PngImageInfo info)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            long originalPosition = stream.Position;
            try
            {
                stream.Position = 0;
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                int dataSize = (int)(info.BytesPerRow * info.Height);
                byte[] pixelData = new byte[dataSize];
                int offset = 0;

                // StbImageSharp returns data in RGBA format
                // We need to convert it to the requested color type
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int srcIndex = (y * image.Width + x) * 4; // RGBA = 4 bytes
                        byte r = image.Data[srcIndex];
                        byte g = image.Data[srcIndex + 1];
                        byte b = image.Data[srcIndex + 2];
                        byte a = image.Data[srcIndex + 3];

                        switch (info.ColorType)
                        {
                            case PngColorType.Rgb:
                                pixelData[offset++] = r;
                                pixelData[offset++] = g;
                                pixelData[offset++] = b;
                                break;

                            case PngColorType.RgbAlpha:
                                pixelData[offset++] = r;
                                pixelData[offset++] = g;
                                pixelData[offset++] = b;
                                pixelData[offset++] = a;
                                break;

                            case PngColorType.Grayscale:
                                // Convert to grayscale using standard formula
                                byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                                pixelData[offset++] = gray;
                                break;

                            case PngColorType.GrayscaleAlpha:
                                byte gray2 = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                                pixelData[offset++] = gray2;
                                pixelData[offset++] = a;
                                break;
                        }
                    }
                }

                return pixelData;
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
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (info is null)
                throw new ArgumentNullException(nameof(info));
            if (rowCallback is null)
                throw new ArgumentNullException(nameof(rowCallback));

            long originalPosition = stream.Position;
            try
            {
                stream.Position = 0;
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                byte[] rowBuffer = new byte[info.BytesPerRow];

                for (int y = 0; y < image.Height; y++)
                {
                    int offset = 0;

                    for (int x = 0; x < image.Width; x++)
                    {
                        int srcIndex = (y * image.Width + x) * 4; // RGBA = 4 bytes
                        byte r = image.Data[srcIndex];
                        byte g = image.Data[srcIndex + 1];
                        byte b = image.Data[srcIndex + 2];
                        byte a = image.Data[srcIndex + 3];

                        switch (info.ColorType)
                        {
                            case PngColorType.Rgb:
                                rowBuffer[offset++] = r;
                                rowBuffer[offset++] = g;
                                rowBuffer[offset++] = b;
                                break;

                            case PngColorType.RgbAlpha:
                                rowBuffer[offset++] = r;
                                rowBuffer[offset++] = g;
                                rowBuffer[offset++] = b;
                                rowBuffer[offset++] = a;
                                break;

                            case PngColorType.Grayscale:
                                byte gray = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                                rowBuffer[offset++] = gray;
                                break;

                            case PngColorType.GrayscaleAlpha:
                                byte gray2 = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
                                rowBuffer[offset++] = gray2;
                                rowBuffer[offset++] = a;
                                break;
                        }
                    }

                    rowCallback(rowBuffer, y);
                }
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }

        private PngColorType DetermineColorType(ImageResult image)
        {
            // Sample some pixels to determine if image has alpha and color
            bool hasAlpha = false;
            bool hasColor = false;

            int sampleStep = Math.Max(1, Math.Min(image.Width, image.Height) / 10);

            for (int y = 0; y < image.Height && (!hasAlpha || !hasColor); y += sampleStep)
            {
                for (int x = 0; x < image.Width && (!hasAlpha || !hasColor); x += sampleStep)
                {
                    int index = (y * image.Width + x) * 4; // RGBA
                    byte r = image.Data[index];
                    byte g = image.Data[index + 1];
                    byte b = image.Data[index + 2];
                    byte a = image.Data[index + 3];

                    if (a < 255)
                        hasAlpha = true;

                    if (r != g || g != b)
                        hasColor = true;
                }
            }

            if (hasColor)
                return hasAlpha ? PngColorType.RgbAlpha : PngColorType.Rgb;
            else
                return hasAlpha ? PngColorType.GrayscaleAlpha : PngColorType.Grayscale;
        }

        public void Dispose()
        {
            // No resources to dispose in this implementation
            // Call SuppressFinalize to prevent derived types with finalizers
            // from needing to re-implement IDisposable
            GC.SuppressFinalize(this);
        }
    }
}
