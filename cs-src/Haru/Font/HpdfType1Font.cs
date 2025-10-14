/*
 * << Haru Free PDF Library >> -- HpdfType1Font.cs
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
using System.Linq;
using Haru.Objects;
using Haru.Xref;
using Haru.Font.Type1;
using Haru.Streams;
using Haru.Types;

namespace Haru.Font
{
    /// <summary>
    /// Represents a Type 1 font that can be embedded in a PDF.
    /// Supports custom code pages for multi-language rendering (similar to TrueType fonts).
    /// </summary>
    public class HpdfType1Font : IHpdfFontImplementation
    {
        private readonly HpdfDict _dict;
        private string _baseFont;
        private readonly string _localName;
        private readonly HpdfXref _xref;

        // Type 1 font data
        private AfmData _afmData;
        private byte[] _pfbData;
        private bool _embedding;
        private HpdfDict _descriptor;
        private HpdfStreamObject _fontFileStream;
        private HpdfStreamObject _toUnicodeStream;
        private int _codePage;

        /// <summary>
        /// Gets the underlying dictionary object for this font.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets the BaseFont name (PostScript name).
        /// </summary>
        public string BaseFont => _baseFont;

        /// <summary>
        /// Gets the local name used to reference this font in page resources.
        /// </summary>
        public string LocalName => _localName;

        /// <summary>
        /// Gets the code page used for encoding this font.
        /// </summary>
        public int? CodePage => _codePage;

        /// <summary>
        /// Gets the font descriptor dictionary.
        /// </summary>
        public HpdfDict Descriptor => _descriptor;

        /// <summary>
        /// Gets the font ascent in 1000-unit glyph space.
        /// </summary>
        public int Ascent => _afmData?.Ascender ?? 750;

        /// <summary>
        /// Gets the font descent in 1000-unit glyph space.
        /// </summary>
        public int Descent => _afmData?.Descender ?? -250;

        /// <summary>
        /// Gets the x-height in 1000-unit glyph space.
        /// </summary>
        public int XHeight => _afmData?.XHeight ?? 500;

        /// <summary>
        /// Gets the font bounding box.
        /// </summary>
        public HpdfBox FontBBox
        {
            get
            {
                if (_afmData?.FontBBox != null)
                {
                    var r = _afmData.FontBBox;
                    return new HpdfBox(r.Left, r.Bottom, r.Right, r.Top);
                }
                return new HpdfBox(0, -250, 1000, 750);
            }
        }

        /// <summary>
        /// Gets an HpdfFont wrapper for this Type 1 font that can be used with page operations.
        /// </summary>
        public HpdfFont AsFont()
        {
            return new HpdfFont(this);
        }

        private HpdfType1Font(HpdfXref xref, string localName, bool embedding, int codePage)
        {
            _xref = xref ?? throw new ArgumentNullException(nameof(xref));
            _localName = localName ?? throw new ArgumentNullException(nameof(localName));
            _embedding = embedding;
            _codePage = codePage;
            _dict = new HpdfDict();
        }

        /// <summary>
        /// Loads a Type 1 font from AFM and optional PFB files with default code page (1252 - Windows Latin).
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        /// <param name="afmPath">Path to the AFM file.</param>
        /// <param name="pfbPath">Path to the PFB file (optional, null for no embedding).</param>
        /// <returns>The loaded Type 1 font.</returns>
        public static HpdfType1Font LoadFromFile(HpdfXref xref, string localName, string afmPath, string pfbPath = null)
        {
            return LoadFromFile(xref, localName, afmPath, pfbPath, 1252);
        }

        /// <summary>
        /// Loads a Type 1 font from AFM and optional PFB files with specified code page.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        /// <param name="afmPath">Path to the AFM file.</param>
        /// <param name="pfbPath">Path to the PFB file (null for no embedding).</param>
        /// <param name="codePage">The code page to use for encoding (e.g., 1252 for Windows Latin, 1251 for Cyrillic).</param>
        /// <returns>The loaded Type 1 font.</returns>
        public static HpdfType1Font LoadFromFile(HpdfXref xref, string localName, string afmPath, string pfbPath, int codePage)
        {
            if (!File.Exists(afmPath))
                throw new HpdfException(HpdfErrorCode.FileNotFound, $"AFM file not found: {afmPath}");

            bool embedding = !string.IsNullOrEmpty(pfbPath);
            if (embedding && !File.Exists(pfbPath))
                throw new HpdfException(HpdfErrorCode.FileNotFound, $"PFB file not found: {pfbPath}");

            var font = new HpdfType1Font(xref, localName, embedding, codePage);

            // Parse AFM file
            font._afmData = AfmParser.ParseFile(afmPath);

            // Parse PFB file if embedding
            if (embedding)
            {
                font._pfbData = PfbParser.ParseFile(pfbPath);
            }

            // Extract font name
            font._baseFont = font._afmData.FontName ?? "CustomType1Font";

            // Create font dictionary
            font.CreateFontDictionary();

            // Create font descriptor
            font.CreateFontDescriptor(pfbPath);

            return font;
        }

        private void CreateFontDictionary()
        {
            _dict.Add("Type", new HpdfName("Font"));
            _dict.Add("Subtype", new HpdfName("Type1"));
            _dict.Add("BaseFont", new HpdfName(_baseFont));

            // Add FirstChar and LastChar
            byte firstChar = 32;
            byte lastChar = 255;

            _dict.Add("FirstChar", new HpdfNumber(firstChar));
            _dict.Add("LastChar", new HpdfNumber(lastChar));

            // Register code page provider if needed
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            }
            catch
            {
                // Already registered
            }

            var encoding = System.Text.Encoding.GetEncoding(_codePage);

            // Create custom Encoding dictionary with Differences array
            // (Same approach as TrueType fonts)
            CreateEncodingDictionary(encoding, firstChar, lastChar);

            // Add Widths array
            var widths = new HpdfArray();
            for (int i = firstChar; i <= lastChar; i++)
            {
                // Convert byte value to Unicode character using the code page
                byte[] byteArray = new byte[] { (byte)i };
                string str = encoding.GetString(byteArray);
                ushort unicode = (str.Length > 0) ? (ushort)str[0] : (ushort)0;

                // Get width for this Unicode character from AFM data
                int width = GetWidthForUnicode(unicode);
                widths.Add(new HpdfNumber(width));
            }
            _dict.Add("Widths", widths);

            // Add to xref
            _xref.Add(_dict);
        }

        /// <summary>
        /// Creates a custom Encoding dictionary with Differences array for the specified code page.
        /// This maps byte codes to Unicode characters for proper glyph rendering.
        /// Same approach as TrueType fonts.
        /// </summary>
        private void CreateEncodingDictionary(System.Text.Encoding encoding, byte firstChar, byte lastChar)
        {
            // For standard encodings, just use the built-in name
            if (_codePage == 1252)
            {
                _dict.Add("Encoding", new HpdfName("WinAnsiEncoding"));
                return;
            }
            else if (_codePage == 10000)
            {
                _dict.Add("Encoding", new HpdfName("MacRomanEncoding"));
                return;
            }

            // For custom code pages, create an Encoding dictionary with Differences array
            var encodingDict = new HpdfDict();
            encodingDict.Add("Type", new HpdfName("Encoding"));

            // Use WinAnsiEncoding as base for Western characters (0x20-0x7E are the same)
            encodingDict.Add("BaseEncoding", new HpdfName("WinAnsiEncoding"));

            // Build Differences array
            // Format: [code1 /glyphname1 /glyphname2 ... code2 /glyphname3 ...]
            var differences = new HpdfArray();

            // Track if we need to add a range
            int rangeStart = -1;
            var rangeGlyphs = new List<string>();

            for (int i = firstChar; i <= lastChar; i++)
            {
                // Convert byte value to Unicode character using the code page
                byte[] byteArray = new byte[] { (byte)i };
                string str = encoding.GetString(byteArray);
                ushort unicode = (str.Length > 0) ? (ushort)str[0] : (ushort)0;

                // Skip if it's ASCII range (0x20-0x7E) and matches WinAnsi
                // WinAnsi and most code pages share ASCII characters
                if (i >= 0x20 && i <= 0x7E)
                {
                    // Flush current range if any
                    if (rangeStart >= 0)
                    {
                        differences.Add(new HpdfNumber(rangeStart));
                        foreach (var glyph in rangeGlyphs)
                        {
                            differences.Add(new HpdfName(glyph));
                        }
                        rangeStart = -1;
                        rangeGlyphs.Clear();
                    }
                    continue;
                }

                // For non-ASCII, we need to specify the glyph name
                // Find the glyph name from AFM data for this Unicode value
                string glyphName = GetGlyphNameForUnicode(unicode);

                // Start a new range or continue current one
                if (rangeStart < 0)
                {
                    rangeStart = i;
                }
                rangeGlyphs.Add(glyphName);
            }

            // Flush final range if any
            if (rangeStart >= 0)
            {
                differences.Add(new HpdfNumber(rangeStart));
                foreach (var glyph in rangeGlyphs)
                {
                    differences.Add(new HpdfName(glyph));
                }
            }

            // Only add Differences if we have custom mappings
            if (differences.Count > 0)
            {
                encodingDict.Add("Differences", differences);
            }

            // Add encoding dictionary to xref and link to font
            _xref.Add(encodingDict);
            _dict.Add("Encoding", encodingDict);
        }

        private void CreateFontDescriptor(string pfbPath)
        {
            _descriptor = new HpdfDict();
            _descriptor.Add("Type", new HpdfName("FontDescriptor"));
            _descriptor.Add("FontName", new HpdfName(_baseFont));

            // Add flags from AFM data
            _descriptor.Add("Flags", new HpdfNumber(_afmData.Flags));

            // Add font bounding box
            var bbox = new HpdfArray();
            bbox.Add(new HpdfNumber((int)_afmData.FontBBox.Left));
            bbox.Add(new HpdfNumber((int)_afmData.FontBBox.Bottom));
            bbox.Add(new HpdfNumber((int)_afmData.FontBBox.Right));
            bbox.Add(new HpdfNumber((int)_afmData.FontBBox.Top));
            _descriptor.Add("FontBBox", bbox);

            // Add italic angle
            _descriptor.Add("ItalicAngle", new HpdfReal(_afmData.ItalicAngle));

            // Add metrics
            _descriptor.Add("Ascent", new HpdfNumber(_afmData.Ascender));
            _descriptor.Add("Descent", new HpdfNumber(_afmData.Descender));

            // Add CapHeight
            _descriptor.Add("CapHeight", new HpdfNumber(_afmData.CapHeight > 0 ? _afmData.CapHeight : _afmData.Ascender));

            // Calculate StemV (use StdVW from AFM, or estimate)
            int stemV = _afmData.StdVW > 0 ? _afmData.StdVW : 80;
            _descriptor.Add("StemV", new HpdfNumber(stemV));

            // Embed font data if requested
            if (_embedding && _pfbData != null && !string.IsNullOrEmpty(pfbPath))
            {
                EmbedFontData(pfbPath);
            }

            // Add ToUnicode CMap for text extraction
            CreateToUnicodeCMap();

            // Link descriptor to font
            _dict.Add("FontDescriptor", _descriptor);

            // Add to xref
            _xref.Add(_descriptor);
        }

        /// <summary>
        /// Embeds the font data (PFB) in the PDF.
        /// </summary>
        private void EmbedFontData(string pfbPath)
        {
            // Create font file stream
            _fontFileStream = new HpdfStreamObject();

            // For Type 1 fonts, we need to specify Length1, Length2, Length3
            var (length1, length2, length3) = PfbParser.GetSectionLengths(pfbPath);
            _fontFileStream.Add("Length1", new HpdfNumber(length1));
            _fontFileStream.Add("Length2", new HpdfNumber(length2));
            _fontFileStream.Add("Length3", new HpdfNumber(length3));

            // Add font data to stream
            _fontFileStream.WriteToStream(_pfbData);

            // Apply compression
            _fontFileStream.Filter = HpdfStreamFilter.FlateDecode;

            // Link to font descriptor (FontFile for Type 1, not FontFile2)
            _descriptor.Add("FontFile", _fontFileStream);

            // Add to xref
            _xref.Add(_fontFileStream);
        }

        /// <summary>
        /// Creates the ToUnicode CMap for text extraction.
        /// Uses the same ToUnicodeCMap class as TrueType fonts.
        /// </summary>
        private void CreateToUnicodeCMap()
        {
            // Create ToUnicode CMap for the specified code page
            var cmap = ToUnicodeCMap.CreateFromCodePage(_codePage);
            string cmapContent = cmap.Generate();

            // Create stream object
            _toUnicodeStream = new HpdfStreamObject();
            byte[] cmapBytes = System.Text.Encoding.UTF8.GetBytes(cmapContent);
            _toUnicodeStream.WriteToStream(cmapBytes);
            _toUnicodeStream.Filter = HpdfStreamFilter.FlateDecode;

            // Link to font dictionary
            _dict.Add("ToUnicode", _toUnicodeStream);

            // Add to xref
            _xref.Add(_toUnicodeStream);
        }

        /// <summary>
        /// Gets the glyph name for a Unicode character from AFM data.
        /// </summary>
        private string GetGlyphNameForUnicode(ushort unicode)
        {
            if (_afmData.CharMetrics == null)
                return $"uni{unicode:X4}"; // Fallback to uni format

            // Find the character in AFM metrics
            var metric = _afmData.CharMetrics.FirstOrDefault(m => m.Unicode == unicode);
            if (metric != null && !string.IsNullOrEmpty(metric.Name))
                return metric.Name;

            // Not found, use uni format as fallback
            return $"uni{unicode:X4}";
        }

        /// <summary>
        /// Gets the width for a Unicode character from AFM data.
        /// </summary>
        private int GetWidthForUnicode(ushort unicode)
        {
            if (_afmData.CharMetrics == null)
                return 500; // Default width

            // Find the character in AFM metrics
            var metric = _afmData.CharMetrics.FirstOrDefault(m => m.Unicode == unicode);
            if (metric != null)
                return metric.Width;

            // Not found, return default
            return 500;
        }

        /// <summary>
        /// Gets the width of a character in font units.
        /// </summary>
        public float GetCharWidth(byte charCode)
        {
            // Convert charCode to Unicode using the code page
            try
            {
                var encoding = System.Text.Encoding.GetEncoding(_codePage);
                byte[] byteArray = new byte[] { charCode };
                string str = encoding.GetString(byteArray);
                ushort unicode = (str.Length > 0) ? (ushort)str[0] : (ushort)0;

                return GetWidthForUnicode(unicode);
            }
            catch
            {
                return 500;
            }
        }

        /// <summary>
        /// Measures text width in user space units.
        /// </summary>
        public float MeasureText(string text, float fontSize)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            float totalWidth = 0;
            foreach (char c in text)
            {
                totalWidth += GetCharWidth((byte)c);
            }

            // Type 1 fonts are in 1000-unit space by default
            return totalWidth * fontSize / 1000.0f;
        }
    }
}
