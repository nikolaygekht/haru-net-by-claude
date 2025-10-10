/*
 * << Haru Free PDF Library >> -- CMapGenerator.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Haru.Font.CID
{
    /// <summary>
    /// Generates CMap streams for CID fonts.
    /// CMaps map character codes to CIDs (Character IDs) or Unicode values.
    /// </summary>
    internal static class CMapGenerator
    {
        /// <summary>
        /// Generates a ToUnicode CMap for CID fonts.
        /// This enables text extraction and searching in PDF viewers.
        ///
        /// For Identity-H encoding:
        /// - Character code = Unicode code point (UTF-16BE)
        /// - Maps 2-byte character codes to Unicode values
        /// </summary>
        /// <param name="systemInfo">CID system info</param>
        /// <param name="cidToUnicode">Mapping from CID (character code) to Unicode</param>
        /// <returns>ToUnicode CMap content as string</returns>
        public static string GenerateToUnicodeCMap(
            CIDSystemInfo systemInfo,
            Dictionary<ushort, ushort> cidToUnicode)
        {
            if (cidToUnicode == null || cidToUnicode.Count == 0)
            {
                throw new ArgumentException("CID to Unicode mapping cannot be empty", nameof(cidToUnicode));
            }

            var sb = new StringBuilder();

            // CMap header
            sb.AppendLine("/CIDInit /ProcSet findresource begin");
            sb.AppendLine("12 dict begin");
            sb.AppendLine("begincmap");

            // CIDSystemInfo dictionary
            sb.AppendLine("/CIDSystemInfo");
            sb.AppendLine("<< /Registry (Adobe)");
            sb.AppendLine("/Ordering (UCS)");  // UCS for ToUnicode
            sb.AppendLine("/Supplement 0");
            sb.AppendLine(">> def");

            sb.AppendLine("/CMapName /Adobe-Identity-UCS def");
            sb.AppendLine("/CMapType 2 def");  // Type 2 for ToUnicode

            // Code space range (2-byte for CID fonts)
            sb.AppendLine("1 begincodespacerange");
            sb.AppendLine("<0000> <FFFF>");  // Full 2-byte range
            sb.AppendLine("endcodespacerange");

            // Generate bfrange entries
            var ranges = BuildBfRanges(cidToUnicode);
            if (ranges.Count > 0)
            {
                // Split into chunks of 100 (PDF spec recommendation)
                var chunks = ranges.Select((range, index) => new { range, index })
                                  .GroupBy(x => x.index / 100)
                                  .Select(g => g.Select(x => x.range).ToList())
                                  .ToList();

                foreach (var chunk in chunks)
                {
                    sb.AppendLine($"{chunk.Count} beginbfrange");
                    foreach (var range in chunk)
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
        /// Builds bfrange entries for the ToUnicode CMap.
        /// Groups consecutive CIDs with consecutive Unicode values into ranges for efficiency.
        /// </summary>
        /// <param name="cidToUnicode">CID to Unicode mapping</param>
        /// <returns>List of bfrange entries</returns>
        private static List<string> BuildBfRanges(Dictionary<ushort, ushort> cidToUnicode)
        {
            var ranges = new List<string>();

            // Sort by CID (character code)
            var sortedMappings = cidToUnicode.OrderBy(kvp => kvp.Key).ToList();

            if (sortedMappings.Count == 0)
                return ranges;

            // Build ranges
            int rangeStart = 0;
            for (int i = 1; i < sortedMappings.Count; i++)
            {
                ushort currentCid = sortedMappings[i].Key;
                ushort prevCid = sortedMappings[i - 1].Key;
                ushort currentUnicode = sortedMappings[i].Value;
                ushort prevUnicode = sortedMappings[i - 1].Value;

                // Check if this continues the range
                bool continuesRange = (currentCid == prevCid + 1) && (currentUnicode == prevUnicode + 1);

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
        /// Formats a bfrange entry for 2-byte character codes.
        /// Format: srcStart> <srcEnd> <dstStart>
        /// </summary>
        private static string FormatBfRange(
            List<KeyValuePair<ushort, ushort>> mappings,
            int startIndex,
            int endIndex)
        {
            ushort startCid = mappings[startIndex].Key;
            ushort endCid = mappings[endIndex].Key;
            ushort startUnicode = mappings[startIndex].Value;

            // Format: <srcStart> <srcEnd> <dstStart>
            // Use 4-digit hex for 2-byte values
            return $"<{startCid:X4}> <{endCid:X4}> <{startUnicode:X4}>";
        }

        /// <summary>
        /// Generates an Identity-H CMap (if needed).
        /// For most cases, we can just use the predefined "/Identity-H" name in PDF.
        /// This method is provided for completeness but may not be used in the implementation.
        /// </summary>
        /// <param name="systemInfo">CID system info</param>
        /// <returns>Identity-H CMap content (usually not needed, use "/Identity-H" name instead)</returns>
        public static string GenerateIdentityHCMap(CIDSystemInfo systemInfo)
        {
            var sb = new StringBuilder();

            // CMap header
            sb.AppendLine("/CIDInit /ProcSet findresource begin");
            sb.AppendLine("12 dict begin");
            sb.AppendLine("begincmap");

            // CIDSystemInfo dictionary
            sb.AppendLine("/CIDSystemInfo");
            sb.AppendLine($"<< /Registry ({systemInfo.Registry})");
            sb.AppendLine($"/Ordering ({systemInfo.Ordering})");
            sb.AppendLine($"/Supplement {systemInfo.Supplement}");
            sb.AppendLine(">> def");

            sb.AppendLine("/CMapName /Identity-H def");
            sb.AppendLine("/CMapType 1 def");  // Type 1 for encoding CMap
            sb.AppendLine("/WMode 0 def");     // Horizontal writing

            // Code space range
            sb.AppendLine("1 begincodespacerange");
            sb.AppendLine("<0000> <FFFF>");
            sb.AppendLine("endcodespacerange");

            // Identity mapping: all codes map to themselves
            // For Identity-H, we typically use the predefined name instead
            sb.AppendLine("1 begincidrange");
            sb.AppendLine("<0000> <FFFF> 0");
            sb.AppendLine("endcidrange");

            // CMap footer
            sb.AppendLine("endcmap");
            sb.AppendLine("CMapName currentdict /CMap defineresource pop");
            sb.AppendLine("end");
            sb.AppendLine("end");

            return sb.ToString();
        }
    }
}
