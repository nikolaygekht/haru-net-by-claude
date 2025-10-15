/*
 * << Haru Free PDF Library >> -- EucHEncoder.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

namespace Haru.Font.CID
{
    /// <summary>
    /// EUC-H encoder for Japanese.
    /// Uses Windows Code Page 20932 (EUC-JP encoding).
    /// The PDF viewer's built-in EUC-H CMap handles byte→CID→glyph mapping.
    /// </summary>
    public class EucHEncoder : CIDEncoder
    {
        public EucHEncoder()
        {
            Name = "EUC-H";
            Registry = "Adobe";
            Ordering = "Japan1";
            Supplement = 2;
            CodePage = 20932; // Windows Code Page for EUC-JP (Japanese)

            InitializeEncoding();
        }
    }
}
