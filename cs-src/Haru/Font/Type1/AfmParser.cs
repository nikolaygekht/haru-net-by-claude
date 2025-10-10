/*
 * << Haru Free PDF Library >> -- AfmParser.cs
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
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Haru.Types;

namespace Haru.Font.Type1
{
    /// <summary>
    /// Parser for Adobe Font Metrics (AFM) files.
    /// </summary>
    public class AfmParser
    {
        /// <summary>
        /// Parses an AFM file from a file path.
        /// </summary>
        public static AfmData ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new HpdfException(HpdfErrorCode.FileNotFound, $"AFM file not found: {filePath}");

            using (var stream = File.OpenRead(filePath))
            using (var reader = new StreamReader(stream))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses an AFM file from a stream.
        /// </summary>
        public static AfmData ParseStream(Stream stream)
        {
            using (var reader = new StreamReader(stream, leaveOpen: true))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses AFM data from a TextReader.
        /// </summary>
        private static AfmData Parse(TextReader reader)
        {
            var data = new AfmData();
            string line;

            // Check header
            line = reader.ReadLine();
            if (line == null || !line.StartsWith("StartFontMetrics"))
            {
                throw new HpdfException(HpdfErrorCode.InvalidAfmHeader, "Invalid AFM file: Missing StartFontMetrics header");
            }

            // Parse global font information
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("StartCharMetrics"))
                {
                    // Parse character metrics count
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 && int.TryParse(parts[1], out int count))
                    {
                        data.CharMetrics = ParseCharMetrics(reader, count);
                    }
                    break;
                }

                ParseGlobalMetric(line, data);
            }

            // Calculate flags for PDF descriptor
            CalculateFlags(data);

            return data;
        }

        private static void ParseGlobalMetric(string line, AfmData data)
        {
            var parts = SplitLine(line, 2);
            if (parts.Length < 2)
                return;

            string keyword = parts[0];
            string value = parts[1];

            switch (keyword)
            {
                case "FontName":
                    data.FontName = value.Trim();
                    break;

                case "FullName":
                    data.FullName = value.Trim();
                    break;

                case "FamilyName":
                    data.FamilyName = value.Trim();
                    break;

                case "Weight":
                    data.Weight = value.Trim();
                    break;

                case "ItalicAngle":
                    if (float.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out float angle))
                        data.ItalicAngle = angle;
                    break;

                case "IsFixedPitch":
                    data.IsFixedPitch = value.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                    break;

                case "UnderlinePosition":
                    if (int.TryParse(value.Trim(), out int underlinePos))
                        data.UnderlinePosition = underlinePos;
                    break;

                case "UnderlineThickness":
                    if (int.TryParse(value.Trim(), out int underlineThick))
                        data.UnderlineThickness = underlineThick;
                    break;

                case "Version":
                    data.Version = value.Trim();
                    break;

                case "EncodingScheme":
                    data.EncodingScheme = value.Trim();
                    break;

                case "FontBBox":
                    ParseFontBBox(value, data);
                    break;

                case "CapHeight":
                    if (int.TryParse(value.Trim(), out int capHeight))
                        data.CapHeight = capHeight;
                    break;

                case "XHeight":
                    if (int.TryParse(value.Trim(), out int xHeight))
                        data.XHeight = xHeight;
                    break;

                case "Ascender":
                    if (int.TryParse(value.Trim(), out int ascender))
                        data.Ascender = ascender;
                    break;

                case "Descender":
                    if (int.TryParse(value.Trim(), out int descender))
                        data.Descender = descender;
                    break;

                case "StdHW":
                    if (int.TryParse(value.Trim(), out int stdHW))
                        data.StdHW = stdHW;
                    break;

                case "StdVW":
                    if (int.TryParse(value.Trim(), out int stdVW))
                        data.StdVW = stdVW;
                    break;

                case "CharacterSet":
                    data.CharacterSet = value.Trim();
                    break;
            }
        }

        private static void ParseFontBBox(string value, AfmData data)
        {
            var parts = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 4)
            {
                if (float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float left) &&
                    float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float bottom) &&
                    float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float right) &&
                    float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float top))
                {
                    data.FontBBox = new HpdfRect(left, bottom, right, top);
                }
            }
        }

        private static AfmCharMetric[] ParseCharMetrics(TextReader reader, int count)
        {
            var metrics = new List<AfmCharMetric>();
            string line;

            while ((line = reader.ReadLine()) != null && metrics.Count < count)
            {
                line = line.Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                if (line.StartsWith("EndCharMetrics"))
                    break;

                var metric = ParseCharMetric(line);
                if (metric != null)
                    metrics.Add(metric);
            }

            return metrics.ToArray();
        }

        private static AfmCharMetric ParseCharMetric(string line)
        {
            var metric = new AfmCharMetric();
            metric.CharCode = -1;

            // Parse the line: C code ; WX width ; N name ; B bbox ;
            var parts = line.Split(';');

            foreach (var part in parts)
            {
                var trimmed = part.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;

                var tokens = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length < 2)
                    continue;

                string key = tokens[0];
                string value = tokens[1];

                switch (key)
                {
                    case "C":
                        if (int.TryParse(value, out int charCode))
                            metric.CharCode = charCode;
                        break;

                    case "WX":
                        if (int.TryParse(value, out int width))
                            metric.Width = width;
                        break;

                    case "N":
                        metric.Name = value;
                        // Convert glyph name to Unicode
                        metric.Unicode = GlyphNameToUnicode(value);
                        break;
                }
            }

            return metric;
        }

        private static string[] SplitLine(string line, int maxParts)
        {
            var index = line.IndexOf(' ');
            if (index < 0)
                return new[] { line };

            if (maxParts == 2)
            {
                return new[] { line.Substring(0, index), line.Substring(index + 1) };
            }

            return line.Split(new[] { ' ' }, maxParts, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void CalculateFlags(AfmData data)
        {
            int flags = 0;

            // Bit 1: FixedPitch
            if (data.IsFixedPitch)
                flags |= 1;

            // Bit 2: Serif (default for most Type 1 fonts)
            flags |= 2;

            // Bit 6: Italic
            if (data.ItalicAngle != 0)
                flags |= 64;

            // Bit 7: Nonsymbolic (for text fonts)
            flags |= 32;

            data.Flags = flags;
        }

        /// <summary>
        /// Converts a PostScript glyph name to Unicode.
        /// This is a simplified implementation for common glyph names.
        /// </summary>
        private static ushort GlyphNameToUnicode(string glyphName)
        {
            if (string.IsNullOrEmpty(glyphName))
                return 0;

            // Handle uni[XXXX] format
            if (glyphName.StartsWith("uni") && glyphName.Length == 7)
            {
                if (ushort.TryParse(glyphName.Substring(3), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort unicode))
                    return unicode;
            }

            // Use static glyph name to Unicode mapping
            return GlyphNames.GetUnicode(glyphName);
        }
    }
}
