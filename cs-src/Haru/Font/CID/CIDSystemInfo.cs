/*
 * << Haru Free PDF Library >> -- CIDSystemInfo.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using Haru.Objects;

namespace Haru.Font.CID
{
    /// <summary>
    /// Represents the CIDSystemInfo dictionary in a CID font
    /// Identifies the character collection used by the CID font
    /// </summary>
    public class CIDSystemInfo
    {
        /// <summary>
        /// Registry: Identifies the issuer of the character collection
        /// Typically "Adobe" for standard CID fonts
        /// </summary>
        public string Registry { get; set; }

        /// <summary>
        /// Ordering: Identifies the character collection within the registry
        /// Examples:
        /// - "Identity" for Identity-H/V encoding
        /// - "Japan1" for Japanese
        /// - "GB1" for Simplified Chinese
        /// - "CNS1" for Traditional Chinese
        /// - "Korea1" for Korean
        /// </summary>
        public string Ordering { get; set; }

        /// <summary>
        /// Supplement: Version/supplement number of the character collection
        /// Typically 0 for Identity, or higher numbers for specific versions
        /// </summary>
        public int Supplement { get; set; }

        /// <summary>
        /// Creates a CIDSystemInfo for Adobe-Identity-0
        /// This is the most common configuration for Unicode-based CID fonts
        /// </summary>
        public static CIDSystemInfo CreateIdentity()
        {
            return new CIDSystemInfo
            {
                Registry = "Adobe",
                Ordering = "Identity",
                Supplement = 0
            };
        }

        /// <summary>
        /// Converts this CIDSystemInfo to a PDF dictionary
        /// </summary>
        /// <returns>HpdfDict containing Registry, Ordering, and Supplement</returns>
        public HpdfDict ToDict()
        {
            var dict = new HpdfDict();
            dict.Add("Registry", new HpdfString(Registry));
            dict.Add("Ordering", new HpdfString(Ordering));
            dict.Add("Supplement", new HpdfNumber(Supplement));
            return dict;
        }

        public CIDSystemInfo()
        {
            Registry = "Adobe";
            Ordering = "Identity";
            Supplement = 0;
        }
    }
}
