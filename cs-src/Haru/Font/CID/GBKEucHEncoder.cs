/*
 * << Haru Free PDF Library >> -- GBKEucHEncoder.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

namespace Haru.Font.CID
{
    /// <summary>
    /// GBK-EUC-H encoder for Simplified Chinese.
    /// Uses Windows Code Page 936 (GBK encoding).
    /// The PDF viewer's built-in GBK-EUC-H CMap handles byte→CID→glyph mapping.
    /// </summary>
    public class GBKEucHEncoder : CIDEncoder
    {
        public GBKEucHEncoder()
        {
            Name = "GBK-EUC-H";
            Registry = "Adobe";
            Ordering = "GB1";
            Supplement = 2;
            CodePage = 936; // Windows Code Page for GBK (Chinese Simplified)

            InitializeEncoding();
        }
    }
}
