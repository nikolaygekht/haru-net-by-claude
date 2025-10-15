/*
 * << Haru Free PDF Library >> -- ETenB5HEncoder.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

namespace Haru.Font.CID
{
    /// <summary>
    /// ETen-B5-H encoder for Traditional Chinese.
    /// Uses Windows Code Page 950 (Big5 encoding).
    /// The PDF viewer's built-in ETen-B5-H CMap handles byte→CID→glyph mapping.
    /// </summary>
    public class ETenB5HEncoder : CIDEncoder
    {
        public ETenB5HEncoder()
        {
            Name = "ETen-B5-H";
            Registry = "Adobe";
            Ordering = "CNS1";
            Supplement = 0;
            CodePage = 950; // Windows Code Page for Big5 (Chinese Traditional)

            InitializeEncoding();
        }
    }
}
