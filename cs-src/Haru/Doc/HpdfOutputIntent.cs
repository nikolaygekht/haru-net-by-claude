/*
 * << Haru Free PDF Library >> -- HpdfOutputIntent.cs
 *
 * C# port of Haru Free PDF Library
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 * provided that the above copyright notice appear in all copies and
 * that both that copyright notice and this permission notice appear
 * in supporting documentation.
 * It is provided "as is" without express or implied warranty.
 *
 */

using Haru.Objects;
using Haru.Xref;

namespace Haru.Doc
{
    /// <summary>
    /// Represents a PDF Output Intent for color profile specification.
    /// Required for PDF/A compliance to ensure consistent color reproduction.
    /// </summary>
    internal class HpdfOutputIntent
    {
        private readonly HpdfDict _dict;
        private readonly HpdfXref _xref;

        /// <summary>
        /// Gets the underlying dictionary object.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Creates an Output Intent with an ICC color profile.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="iccProfile">ICC color profile data (can be null for sRGB default).</param>
        /// <param name="outputCondition">Human-readable output condition (e.g., "sRGB IEC61966-2.1").</param>
        /// <param name="outputConditionIdentifier">Registry identifier (e.g., "sRGB").</param>
        public HpdfOutputIntent(HpdfXref xref, byte[]? iccProfile = null,
            string outputCondition = "sRGB IEC61966-2.1",
            string outputConditionIdentifier = "sRGB")
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            _dict = new HpdfDict();
            _xref.Add(_dict);

            _dict.Add("Type", new HpdfName("OutputIntent"));
            _dict.Add("S", new HpdfName("GTS_PDFA1"));

            if (!string.IsNullOrEmpty(outputCondition))
                _dict.Add("OutputCondition", new HpdfString(outputCondition));

            if (!string.IsNullOrEmpty(outputConditionIdentifier))
                _dict.Add("OutputConditionIdentifier", new HpdfString(outputConditionIdentifier));

            // Add ICC profile if provided
            if (iccProfile != null && iccProfile.Length > 0)
            {
                var destOutputProfile = CreateIccProfileStream(iccProfile);
                _dict.Add("DestOutputProfile", destOutputProfile);
            }
        }

        /// <summary>
        /// Creates a stream containing the ICC color profile.
        /// </summary>
        private HpdfStreamObject CreateIccProfileStream(byte[] iccProfile)
        {
            var stream = new HpdfStreamObject();
            _xref.Add(stream);

            stream.Add("N", new HpdfNumber(3)); // 3 = RGB color space

            // Write ICC profile data
            stream.WriteToStream(iccProfile, 0, iccProfile.Length);

            return stream;
        }

        /// <summary>
        /// Creates a minimal sRGB Output Intent (without embedded ICC profile).
        /// This is acceptable for PDF/A-1b when using standard sRGB.
        /// </summary>
        public static HpdfOutputIntent CreateSRgbIntent(HpdfXref xref)
        {
            return new HpdfOutputIntent(xref, (byte[]?)null,
                "sRGB IEC61966-2.1",
                "sRGB");
        }
    }
}
