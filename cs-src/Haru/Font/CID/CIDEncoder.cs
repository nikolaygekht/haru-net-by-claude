/*
 * << Haru Free PDF Library >> -- CIDEncoder.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using System.Text;

namespace Haru.Font.CID
{
    /// <summary>
    /// Base class for CID font encoders.
    /// For CIDFontType0 with predefined Adobe CMaps, encoders delegate to .NET's code page support.
    /// The PDF viewer's built-in CMap handles the byte→CID→glyph mapping.
    /// </summary>
    public abstract class CIDEncoder
    {
        /// <summary>
        /// Gets the encoder name (e.g., "GBK-EUC-H", "ETen-B5-H").
        /// This is the predefined Adobe CMap name.
        /// </summary>
        public string Name { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets the CID system info registry (typically "Adobe").
        /// </summary>
        public string Registry { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets the CID system info ordering (e.g., "GB1", "CNS1", "Japan1", "Korea1").
        /// </summary>
        public string Ordering { get; protected set; } = string.Empty;

        /// <summary>
        /// Gets the CID system info supplement number.
        /// </summary>
        public int Supplement { get; protected set; }

        /// <summary>
        /// Gets the Windows code page number (e.g., 936 for GBK, 950 for Big5).
        /// </summary>
        public int CodePage { get; protected set; }

        /// <summary>
        /// Gets the .NET encoding for this code page.
        /// </summary>
        protected Encoding Encoding { get; private set; } = System.Text.Encoding.UTF8;

        protected CIDEncoder()
        {
            // Ensure code page provider is registered
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            }
            catch
            {
                // Already registered
            }
        }

        /// <summary>
        /// Initializes the encoding after CodePage is set.
        /// Call this from derived class constructors after setting CodePage.
        /// </summary>
        protected void InitializeEncoding()
        {
            if (CodePage <= 0)
                throw new InvalidOperationException("CodePage must be set before initializing encoding");

            Encoding = Encoding.GetEncoding(CodePage);
        }

        /// <summary>
        /// Encodes Unicode text to bytes using the code page.
        /// For CIDFontType0, these bytes are interpreted by the PDF viewer's predefined CMap.
        /// </summary>
        /// <param name="text">The Unicode text to encode.</param>
        /// <returns>Encoded bytes for use in PDF content stream.</returns>
        public virtual byte[] EncodeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<byte>();

            return Encoding.GetBytes(text);
        }

        /// <summary>
        /// Gets the CID system info for this encoder.
        /// </summary>
        public CIDSystemInfo GetSystemInfo()
        {
            return new CIDSystemInfo
            {
                Registry = Registry,
                Ordering = Ordering,
                Supplement = Supplement
            };
        }
    }
}
