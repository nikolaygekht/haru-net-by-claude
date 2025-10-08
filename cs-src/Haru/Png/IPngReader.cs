using System;
using System.IO;

namespace Haru.Png
{
    /// <summary>
    /// Facade interface for PNG image reading operations.
    /// Abstracts the underlying PNG library implementation.
    /// </summary>
    public interface IPngReader : IDisposable
    {
        /// <summary>
        /// Validates if the stream contains a valid PNG signature.
        /// </summary>
        bool ValidateSignature(Stream stream);

        /// <summary>
        /// Reads PNG image information from the stream.
        /// </summary>
        PngImageInfo ReadImageInfo(Stream stream);

        /// <summary>
        /// Reads the raw pixel data from the PNG image.
        /// </summary>
        /// <param name="stream">The stream containing PNG data</param>
        /// <param name="info">Previously read image information</param>
        /// <returns>Raw pixel data</returns>
        byte[] ReadPixelData(Stream stream, PngImageInfo info);

        /// <summary>
        /// Reads pixel data row by row for non-interlaced images.
        /// </summary>
        /// <param name="stream">The stream containing PNG data</param>
        /// <param name="info">Previously read image information</param>
        /// <param name="rowCallback">Callback invoked for each row</param>
        void ReadPixelDataByRow(Stream stream, PngImageInfo info, Action<byte[], int> rowCallback);
    }
}
