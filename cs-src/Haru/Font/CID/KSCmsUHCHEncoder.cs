/*
 * << Haru Free PDF Library >> -- KSCmsUHCHEncoder.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

namespace Haru.Font.CID
{
    /// <summary>
    /// KSCms-UHC-H encoder for Korean.
    /// Uses Windows Code Page 949 (EUC-KR/UHC encoding).
    /// The PDF viewer's built-in KSCms-UHC-H CMap handles byte→CID→glyph mapping.
    /// </summary>
    public class KSCmsUHCHEncoder : CIDEncoder
    {
        public KSCmsUHCHEncoder()
        {
            Name = "KSCms-UHC-H";
            Registry = "Adobe";
            Ordering = "Korea1";
            Supplement = 1;
            CodePage = 949; // Windows Code Page for EUC-KR/UHC (Korean)

            InitializeEncoding();
        }
    }
}
