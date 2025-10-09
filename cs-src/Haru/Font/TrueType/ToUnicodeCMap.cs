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

namespace Haru.Font.TrueType
{
    /// <summary>
    /// Generates ToUnicode CMap streams for TrueType fonts.
    /// This enables text extraction and searching in PDF files.
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
        /// Creates a ToUnicode CMap for WinAnsiEncoding (code page 1252).
        /// </summary>
        public static ToUnicodeCMap CreateWinAnsiCMap()
        {
            var cmap = new ToUnicodeCMap();

            // Standard ASCII range (0x20 - 0x7E)
            for (byte i = 0x20; i <= 0x7E; i++)
            {
                cmap.AddMapping(i, i);
            }

            // Windows-1252 specific mappings (0x80 - 0xFF)
            // Characters 0x80-0x9F have special mappings
            cmap.AddMapping(0x80, 0x20AC); // Euro sign
            cmap.AddMapping(0x82, 0x201A); // Single low-9 quotation mark
            cmap.AddMapping(0x83, 0x0192); // Latin small letter f with hook
            cmap.AddMapping(0x84, 0x201E); // Double low-9 quotation mark
            cmap.AddMapping(0x85, 0x2026); // Horizontal ellipsis
            cmap.AddMapping(0x86, 0x2020); // Dagger
            cmap.AddMapping(0x87, 0x2021); // Double dagger
            cmap.AddMapping(0x88, 0x02C6); // Modifier letter circumflex accent
            cmap.AddMapping(0x89, 0x2030); // Per mille sign
            cmap.AddMapping(0x8A, 0x0160); // Latin capital letter S with caron
            cmap.AddMapping(0x8B, 0x2039); // Single left-pointing angle quotation mark
            cmap.AddMapping(0x8C, 0x0152); // Latin capital ligature OE
            cmap.AddMapping(0x8E, 0x017D); // Latin capital letter Z with caron
            cmap.AddMapping(0x91, 0x2018); // Left single quotation mark
            cmap.AddMapping(0x92, 0x2019); // Right single quotation mark
            cmap.AddMapping(0x93, 0x201C); // Left double quotation mark
            cmap.AddMapping(0x94, 0x201D); // Right double quotation mark
            cmap.AddMapping(0x95, 0x2022); // Bullet
            cmap.AddMapping(0x96, 0x2013); // En dash
            cmap.AddMapping(0x97, 0x2014); // Em dash
            cmap.AddMapping(0x98, 0x02DC); // Small tilde
            cmap.AddMapping(0x99, 0x2122); // Trade mark sign
            cmap.AddMapping(0x9A, 0x0161); // Latin small letter s with caron
            cmap.AddMapping(0x9B, 0x203A); // Single right-pointing angle quotation mark
            cmap.AddMapping(0x9C, 0x0153); // Latin small ligature oe
            cmap.AddMapping(0x9E, 0x017E); // Latin small letter z with caron
            cmap.AddMapping(0x9F, 0x0178); // Latin capital letter Y with diaeresis

            // ISO-8859-1 range (0xA0 - 0xFF) maps directly to Unicode
            for (ushort i = 0xA0; i <= 0xFF; i++)
            {
                cmap.AddMapping((byte)i, i);
            }

            return cmap;
        }
    }
}
