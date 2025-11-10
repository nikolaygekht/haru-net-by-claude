/*
 * << Haru Free PDF Library >> -- ToUnicodeCMap.cs
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Haru.Font
{
    /// <summary>
    /// Generates ToUnicode CMap streams for fonts.
    /// This enables text extraction and searching in PDF files.
    /// Used by both TrueType and Type 1 fonts.
    /// </summary>
    internal class ToUnicodeCMap
    {
        private readonly Dictionary<byte, ushort> _charToUnicode;

        public ToUnicodeCMap()
        {
            _charToUnicode = new Dictionary<byte, ushort>();
        }

        /// <summary>
        /// Adds a character code to Unicode mapping.
        /// </summary>
        /// <param name="charCode">The character code (0-255).</param>
        /// <param name="unicode">The Unicode code point.</param>
        public void AddMapping(byte charCode, ushort unicode)
        {
            _charToUnicode[charCode] = unicode;
        }

        /// <summary>
        /// Generates the ToUnicode CMap stream content.
        /// </summary>
        /// <returns>CMap stream as a string.</returns>
        public string Generate()
        {
            var sb = new StringBuilder();

            // CMap header
            sb.AppendLine("/CIDInit /ProcSet findresource begin");
            sb.AppendLine("12 dict begin");
            sb.AppendLine("begincmap");
            sb.AppendLine("/CIDSystemInfo");
            sb.AppendLine("<< /Registry (Adobe)");
            sb.AppendLine("/Ordering (UCS)");
            sb.AppendLine("/Supplement 0");
            sb.AppendLine(">> def");
            sb.AppendLine("/CMapName /Adobe-Identity-UCS def");
            sb.AppendLine("/CMapType 2 def");

            // Write character code space range
            sb.AppendLine("1 begincodespacerange");
            sb.AppendLine("<00> <FF>");
            sb.AppendLine("endcodespacerange");

            // Group mappings into ranges for efficiency
            if (_charToUnicode.Count > 0)
            {
                var ranges = BuildBfRanges();

                if (ranges.Count > 0)
                {
                    sb.AppendLine($"{ranges.Count} beginbfrange");
                    foreach (var range in ranges)
                    {
                        sb.AppendLine(range);
                    }
                    sb.AppendLine("endbfrange");
                }
            }

            // CMap footer
            sb.AppendLine("endcmap");
            sb.AppendLine("CMapName currentdict /CMap defineresource pop");
            sb.AppendLine("end");
            sb.AppendLine("end");

            return sb.ToString();
        }

        /// <summary>
        /// Builds bfrange entries for the CMap.
        /// Groups consecutive character codes with consecutive Unicode values into ranges.
        /// </summary>
        private List<string> BuildBfRanges()
        {
            var ranges = new List<string>();

            // Sort by character code
            var sortedMappings = new List<KeyValuePair<byte, ushort>>(_charToUnicode);
            sortedMappings.Sort((a, b) => a.Key.CompareTo(b.Key));

            if (sortedMappings.Count == 0)
                return ranges;

            // Build ranges
            int rangeStart = 0;
            for (int i = 1; i < sortedMappings.Count; i++)
            {
                byte currentCode = sortedMappings[i].Key;
                byte prevCode = sortedMappings[i - 1].Key;
                ushort currentUnicode = sortedMappings[i].Value;
                ushort prevUnicode = sortedMappings[i - 1].Value;

                // Check if this continues the range
                bool continuesRange = (currentCode == prevCode + 1) && (currentUnicode == prevUnicode + 1);

                if (!continuesRange)
                {
                    // End current range
                    ranges.Add(FormatBfRange(sortedMappings, rangeStart, i - 1));
                    rangeStart = i;
                }
            }

            // Add final range
            ranges.Add(FormatBfRange(sortedMappings, rangeStart, sortedMappings.Count - 1));

            return ranges;
        }

        /// <summary>
        /// Formats a bfrange entry.
        /// </summary>
        private string FormatBfRange(List<KeyValuePair<byte, ushort>> mappings, int startIndex, int endIndex)
        {
            byte startCode = mappings[startIndex].Key;
            byte endCode = mappings[endIndex].Key;
            ushort startUnicode = mappings[startIndex].Value;

            // Format: <srcStart> <srcEnd> <dstStart>
            return $"<{startCode:X2}> <{endCode:X2}> <{startUnicode:X4}>";
        }

        /// <summary>
        /// Creates a ToUnicode CMap for a specified code page using System.Text.Encoding.
        /// Note: In .NET Core/.NET 5+, code pages other than UTF-8, UTF-16, UTF-32, ASCII and Latin-1
        /// require registering System.Text.Encoding.CodePagesEncodingProvider.
        /// </summary>
        /// <param name="codePage">The code page identifier (e.g., 437 for DOS, 1251 for Cyrillic, 1252 for Western).</param>
        /// <returns>A ToUnicode CMap for the specified code page.</returns>
        public static ToUnicodeCMap CreateFromCodePage(int codePage)
        {
            var cmap = new ToUnicodeCMap();
            System.Text.Encoding encoding;

            try
            {
                // Try to register code page provider if not already registered
                try
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                }
                catch
                {
                    // Already registered or not available
                }

                encoding = System.Text.Encoding.GetEncoding(codePage);
            }
            catch (Exception)
            {
                // Fallback to UTF-8 if code page not supported
                encoding = System.Text.Encoding.UTF8;
            }

            // Map all byte values (0x00 - 0xFF) to their Unicode equivalents
            for (int i = 0; i <= 0xFF; i++)
            {
                byte[] byteArray = new byte[] { (byte)i };
                string str = encoding.GetString(byteArray);

                if (!string.IsNullOrEmpty(str) && str.Length > 0)
                {
                    ushort unicode = (ushort)str[0];
                    cmap.AddMapping((byte)i, unicode);
                }
            }

            return cmap;
        }
    }
}
