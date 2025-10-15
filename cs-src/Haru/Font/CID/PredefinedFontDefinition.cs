/*
 * << Haru Free PDF Library >> -- PredefinedFontDefinition.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Haru.Font.CID
{
    /// <summary>
    /// Represents a predefined CJK font with built-in metrics and width arrays.
    /// Font data is loaded from embedded JSON resources.
    /// </summary>
    public class PredefinedFontDefinition
    {
        /// <summary>
        /// Gets or sets the font name (e.g., "SimSun", "SimHei", "MingLiU").
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Gets or sets the font metrics (ascent, descent, bbox, flags, etc.).
        /// </summary>
        public FontMetrics Metrics { get; set; }

        /// <summary>
        /// Gets or sets the CID system info (registry, ordering, supplement).
        /// </summary>
        public CIDSystemInfo SystemInfo { get; set; }

        /// <summary>
        /// Gets or sets the array of CID width mappings (sorted by CID for binary search).
        /// </summary>
        public CIDWidth[] Widths { get; set; }

        /// <summary>
        /// Gets the width of a CID in 1000-unit glyph space.
        /// Uses binary search for efficient lookup.
        /// </summary>
        /// <param name="cid">The Character ID to look up.</param>
        /// <returns>The width in 1000-unit glyph space, or DefaultWidth if not found.</returns>
        public short GetCIDWidth(ushort cid)
        {
            if (Widths == null || Widths.Length == 0)
                return (short)Metrics.DefaultWidth;

            // Binary search in sorted width array
            int left = 0, right = Widths.Length - 1;
            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (Widths[mid].CID == cid)
                    return Widths[mid].Width;
                else if (Widths[mid].CID < cid)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            // Return default width if not found
            return (short)Metrics.DefaultWidth;
        }

        /// <summary>
        /// Loads a predefined font definition from embedded resources.
        /// </summary>
        /// <param name="fontName">The font name (e.g., "SimSun", "SimHei").</param>
        /// <returns>The loaded font definition.</returns>
        /// <exception cref="InvalidOperationException">If the font definition cannot be loaded.</exception>
        public static PredefinedFontDefinition Load(string fontName)
        {
            var assembly = typeof(PredefinedFontDefinition).Assembly;
            var resourceName = $"Haru.Font.CID.Data.{fontName}.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new InvalidOperationException($"Font definition resource not found: {resourceName}");

                using (var reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    var fontDef = JsonSerializer.Deserialize<PredefinedFontDefinition>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (fontDef == null)
                        throw new InvalidOperationException($"Failed to deserialize font definition: {fontName}");

                    // Ensure widths are sorted by CID for binary search
                    if (fontDef.Widths != null && fontDef.Widths.Length > 0)
                    {
                        fontDef.Widths = fontDef.Widths.OrderBy(w => w.CID).ToArray();
                    }

                    return fontDef;
                }
            }
        }

        /// <summary>
        /// Creates a font definition from hardcoded data (for POC/testing).
        /// </summary>
        /// <param name="fontName">The font name.</param>
        /// <param name="metrics">The font metrics.</param>
        /// <param name="systemInfo">The CID system info.</param>
        /// <param name="widths">The width array.</param>
        /// <returns>A new PredefinedFontDefinition.</returns>
        public static PredefinedFontDefinition Create(
            string fontName,
            FontMetrics metrics,
            CIDSystemInfo systemInfo,
            Dictionary<ushort, short> widths)
        {
            var fontDef = new PredefinedFontDefinition
            {
                FontName = fontName,
                Metrics = metrics,
                SystemInfo = systemInfo,
                Widths = widths.Select(kvp => new CIDWidth { CID = kvp.Key, Width = kvp.Value })
                               .OrderBy(w => w.CID)
                               .ToArray()
            };

            return fontDef;
        }
    }
}
