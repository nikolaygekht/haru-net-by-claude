using System;

namespace Haru.Streams
{
    /// <summary>
    /// Stream filters for encoding PDF data
    /// </summary>
    [Flags]
    public enum HpdfStreamFilter
    {
        /// <summary>
        /// No filter
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// ASCII hexadecimal encoding
        /// </summary>
        AsciiHexDecode = 0x0100,

        /// <summary>
        /// ASCII base-85 encoding
        /// </summary>
        Ascii85Decode = 0x0200,

        /// <summary>
        /// Flate/Deflate compression
        /// </summary>
        FlateDecode = 0x0400,

        /// <summary>
        /// DCT (JPEG) compression
        /// </summary>
        DctDecode = 0x0800,

        /// <summary>
        /// CCITT fax compression
        /// </summary>
        CcittFaxDecode = 0x1000
    }
}
