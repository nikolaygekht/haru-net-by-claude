/*
 * << Haru Free PDF Library >> -- HpdfTrueTypeFont.cs
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
using Haru.Objects;
using Haru.Xref;
using Haru.Types;
using Haru.Font.TrueType;
using Haru.Streams;

namespace Haru.Font
{
    /// <summary>
    /// Represents a TrueType font that can be embedded in a PDF.
    /// </summary>
    public class HpdfTrueTypeFont : IHpdfFontImplementation, IDisposable
    {
        private readonly HpdfDict _dict;
        private string _baseFont = null!;
        private readonly string _localName;
        private readonly HpdfXref _xref;

        // TrueType font data - initialized by LoadFromStream factory method
        private TrueTypeOffsetTable _offsetTable = null!;
        private TrueTypeHead _head = null!;
        private TrueTypeMaxp _maxp = null!;
        private TrueTypeHhea _hhea = null!;
        private TrueTypeLongHorMetric[] _hMetrics = null!;
        private TrueTypeNameTable _nameTable = null!;
        private TrueTypeCmapFormat4 _cmap = null!;
        private TrueTypeOS2? _os2;
        private TrueTypeGlyphOffsets? _glyphOffsets;
        private TrueTypePost? _post;
        private uint[]? _locaOffsets;

        private byte[]? _fontData;
        private bool _embedding;
        private HpdfDict _descriptor = null!;
        private HpdfStreamObject? _fontFileStream;
        private HpdfStreamObject? _toUnicodeStream;
        private int _codePage;
        private bool _disposed = false;

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
        /// Gets the font ascent in 1000-unit glyph space (scaled from font units).
        /// </summary>
        public int Ascent
        {
            get
            {
                if (_hhea != null && _head != null)
                    return (int)Math.Round(_hhea.Ascender * 1000.0 / _head.UnitsPerEm);
                return 750;
            }
        }

        /// <summary>
        /// Gets the font descent in 1000-unit glyph space (scaled from font units).
        /// </summary>
        public int Descent
        {
            get
            {
                if (_hhea != null && _head != null)
                    return (int)Math.Round(_hhea.Descender * 1000.0 / _head.UnitsPerEm);
                return -250;
            }
        }

        /// <summary>
        /// Gets the x-height in 1000-unit glyph space (scaled from font units).
        /// </summary>
        public int XHeight
        {
            get
            {
                if (_os2 != null && _head != null)
                    return (int)Math.Round(_os2.STypoAscender * 1000.0 / _head.UnitsPerEm);
                if (_head != null)
                    return (int)Math.Round(_head.YMax * 1000.0 / _head.UnitsPerEm);
                return 500;
            }
        }

        /// <summary>
        /// Gets the font bounding box in 1000-unit glyph space (scaled from font units).
        /// </summary>
        public HpdfBox FontBBox
        {
            get
            {
                if (_head != null)
                {
                    float scale = 1000.0f / _head.UnitsPerEm;
                    return new HpdfBox(
                        _head.XMin * scale,
                        _head.YMin * scale,
                        _head.XMax * scale,
                        _head.YMax * scale);
                }
                return new HpdfBox(0, -250, 1000, 750);
            }
        }

        /// <summary>
        /// Gets an HpdfFont wrapper for this TrueType font that can be used with page operations.
        /// </summary>
        public HpdfFont AsFont()
        {
            return new HpdfFont(this);
        }

        private HpdfTrueTypeFont(HpdfXref xref, string localName, bool embedding, int codePage)
        {
            _xref = xref ?? throw new ArgumentNullException(nameof(xref));
            _localName = localName ?? throw new ArgumentNullException(nameof(localName));
            _embedding = embedding;
            _codePage = codePage;
            _dict = new HpdfDict();
        }

        /// <summary>
        /// Loads a TrueType font from a file with default code page (437 - DOS).
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        /// <param name="filePath">Path to the TTF file.</param>
        /// <param name="embedding">Whether to embed the font data.</param>
        /// <returns>The loaded TrueType font.</returns>
        public static HpdfTrueTypeFont LoadFromFile(HpdfXref xref, string localName, string filePath, bool embedding)
        {
            return LoadFromFile(xref, localName, filePath, embedding, 437);
        }

        /// <summary>
        /// Loads a TrueType font from a file with specified code page.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        /// <param name="filePath">Path to the TTF file.</param>
        /// <param name="embedding">Whether to embed the font data.</param>
        /// <param name="codePage">The code page to use for encoding (e.g., 437 for DOS, 1251 for Cyrillic).</param>
        /// <returns>The loaded TrueType font.</returns>
        public static HpdfTrueTypeFont LoadFromFile(HpdfXref xref, string localName, string filePath, bool embedding, int codePage)
        {
            if (!File.Exists(filePath))
                throw new HpdfException(HpdfErrorCode.FileNotFound, $"Font file not found: {filePath}");

            byte[] fontData = File.ReadAllBytes(filePath);

            using (var stream = new MemoryStream(fontData))
            {
                return LoadFromStream(xref, localName, stream, fontData, embedding, codePage);
            }
        }

        /// <summary>
        /// Loads a TrueType font from a stream.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        /// <param name="stream">Stream containing TTF data.</param>
        /// <param name="fontData">Font data bytes (for embedding).</param>
        /// <param name="embedding">Whether to embed the font data.</param>
        /// <param name="codePage">The code page to use for encoding (e.g., 437 for DOS, 1251 for Cyrillic).</param>
        /// <returns>The loaded TrueType font.</returns>
        public static HpdfTrueTypeFont LoadFromStream(HpdfXref xref, string localName, Stream stream, byte[] fontData, bool embedding, int codePage = 437)
        {
            var font = new HpdfTrueTypeFont(xref, localName, embedding, codePage);

            if (embedding)
                font._fontData = fontData;

            using (var parser = new TrueTypeParser(stream))
            {
                // Parse required tables
                font._offsetTable = parser.ParseOffsetTable();

                var headTable = parser.FindTable(font._offsetTable, "head");
                font._head = parser.ParseHead(headTable!);

                var maxpTable = parser.FindTable(font._offsetTable, "maxp");
                font._maxp = parser.ParseMaxp(maxpTable!);

                var hheaTable = parser.FindTable(font._offsetTable, "hhea");
                font._hhea = parser.ParseHhea(hheaTable!);

                var hmtxTable = parser.FindTable(font._offsetTable, "hmtx");
                font._hMetrics = parser.ParseHmtx(hmtxTable!, font._hhea.NumberOfHMetrics, font._maxp.NumGlyphs);

                var nameTable = parser.FindTable(font._offsetTable, "name");
                if (nameTable != null)
                {
                    font._nameTable = parser.ParseName(nameTable);
                }

                // Parse cmap table
                font.ParseCmapTable(parser);

                // Parse OS/2 table if present
                var os2Table = parser.FindTable(font._offsetTable, "OS/2");
                if (os2Table != null)
                {
                    font.ParseOS2Table(parser, os2Table);
                }

                // Parse post table if present (for italic angle)
                var postTable = parser.FindTable(font._offsetTable, "post");
                if (postTable != null)
                {
                    font._post = parser.ParsePost(postTable);
                }

                // Initialize glyph tracking for subsetting
                if (embedding)
                {
                    font._glyphOffsets = new TrueTypeGlyphOffsets
                    {
                        Offsets = new uint[font._maxp.NumGlyphs],
                        Flags = new byte[font._maxp.NumGlyphs]
                    };
                    // Mark glyph 0 (.notdef) as used
                    font._glyphOffsets.Flags[0] = 1;

                    // Parse loca table for subsetting
                    var locaTable = parser.FindTable(font._offsetTable, "loca");
                    if (locaTable != null)
                    {
                        font._locaOffsets = parser.ParseLoca(locaTable, font._head.IndexToLocFormat, font._maxp.NumGlyphs);
                    }
                }
            }

            // Extract font name
            font._baseFont = font.ExtractFontName();

            // Create font dictionary
            font.CreateFontDictionary();

            // Create font descriptor
            font.CreateFontDescriptor();

            return font;
        }

        private void ParseCmapTable(TrueTypeParser parser)
        {
            var cmapTable = parser.FindTable(_offsetTable, "cmap");
            if (cmapTable is null)
                throw new HpdfException(HpdfErrorCode.TtInvalidCmapTable, "cmap table not found");

            parser.Seek(cmapTable.Offset);
            ushort version = parser.ReadUInt16();
            ushort numTables = parser.ReadUInt16();

            // Find Unicode cmap (platform 3, encoding 1 or 10)
            uint cmapOffset = 0;
            for (int i = 0; i < numTables; i++)
            {
                ushort platformId = parser.ReadUInt16();
                ushort encodingId = parser.ReadUInt16();
                uint offset = parser.ReadUInt32();

                // Platform 3 (Microsoft), encoding 1 (Unicode BMP) or 10 (Unicode full)
                if (platformId == 3 && (encodingId == 1 || encodingId == 10))
                {
                    cmapOffset = offset;
                    break;
                }
                // Platform 0 (Unicode) as fallback
                else if (platformId == 0)
                {
                    cmapOffset = offset;
                }
            }

            if (cmapOffset == 0)
                throw new HpdfException(HpdfErrorCode.TtInvalidCmapTable, "No suitable cmap subtable found");

            // Parse cmap format 4
            parser.Seek(cmapTable.Offset + cmapOffset);
            ushort format = parser.ReadUInt16();

            if (format == 4)
            {
                ParseCmapFormat4(parser);
            }
            else
            {
                throw new HpdfException(HpdfErrorCode.UnsupportedFunc, $"Unsupported cmap format: {format}");
            }
        }

        private void ParseCmapFormat4(TrueTypeParser parser)
        {
            _cmap = new TrueTypeCmapFormat4
            {
                Format = 4,
                Length = parser.ReadUInt16(),
                Language = parser.ReadUInt16(),
                SegCountX2 = parser.ReadUInt16()
            };

            int segCount = _cmap.SegCountX2 / 2;

            _cmap.SearchRange = parser.ReadUInt16();
            _cmap.EntrySelector = parser.ReadUInt16();
            _cmap.RangeShift = parser.ReadUInt16();

            // Read endCount array
            _cmap.EndCount = new ushort[segCount];
            for (int i = 0; i < segCount; i++)
            {
                _cmap.EndCount[i] = parser.ReadUInt16();
            }

            _cmap.ReservedPad = parser.ReadUInt16();

            // Read startCount array
            _cmap.StartCount = new ushort[segCount];
            for (int i = 0; i < segCount; i++)
            {
                _cmap.StartCount[i] = parser.ReadUInt16();
            }

            // Read idDelta array
            _cmap.IdDelta = new short[segCount];
            for (int i = 0; i < segCount; i++)
            {
                _cmap.IdDelta[i] = parser.ReadInt16();
            }

            // Read idRangeOffset array
            _cmap.IdRangeOffset = new ushort[segCount];
            for (int i = 0; i < segCount; i++)
            {
                _cmap.IdRangeOffset[i] = parser.ReadUInt16();
            }

            // Read glyphIdArray
            int glyphIdArraySize = (_cmap.Length - (16 + segCount * 8)) / 2;
            _cmap.GlyphIdArray = new ushort[glyphIdArraySize];
            for (int i = 0; i < glyphIdArraySize; i++)
            {
                _cmap.GlyphIdArray[i] = parser.ReadUInt16();
            }
        }

        private void ParseOS2Table(TrueTypeParser parser, TrueTypeTable table)
        {
            parser.Seek(table.Offset);

            _os2 = new TrueTypeOS2
            {
                Version = parser.ReadUInt16(),
                XAvgCharWidth = parser.ReadInt16(),
                WeightClass = parser.ReadUInt16(),
                WidthClass = parser.ReadUInt16(),
                FsType = parser.ReadUInt16(),
                YSubscriptXSize = parser.ReadInt16(),
                YSubscriptYSize = parser.ReadInt16(),
                YSubscriptXOffset = parser.ReadInt16(),
                YSubscriptYOffset = parser.ReadInt16(),
                YSuperscriptXSize = parser.ReadInt16(),
                YSuperscriptYSize = parser.ReadInt16(),
                YSuperscriptXOffset = parser.ReadInt16(),
                YSuperscriptYOffset = parser.ReadInt16(),
                YStrikeoutSize = parser.ReadInt16(),
                YStrikeoutPosition = parser.ReadInt16(),
                SFamilyClass = parser.ReadInt16(),
                Panose = parser.ReadBytes(10)
            };

            _os2.UnicodeRange1 = parser.ReadUInt32();
            _os2.UnicodeRange2 = parser.ReadUInt32();
            _os2.UnicodeRange3 = parser.ReadUInt32();
            _os2.UnicodeRange4 = parser.ReadUInt32();
            _os2.AchVendID = parser.ReadBytes(4);
            _os2.FsSelection = parser.ReadUInt16();
            _os2.FirstCharIndex = parser.ReadUInt16();
            _os2.LastCharIndex = parser.ReadUInt16();
            _os2.STypoAscender = parser.ReadInt16();
            _os2.STypoDescender = parser.ReadInt16();
            _os2.STypoLineGap = parser.ReadInt16();
            _os2.UsWinAscent = parser.ReadUInt16();
            _os2.UsWinDescent = parser.ReadUInt16();

            if (_os2.Version >= 1)
            {
                _os2.CodePageRange1 = parser.ReadUInt32();
                _os2.CodePageRange2 = parser.ReadUInt32();
            }
        }

        private string ExtractFontName()
        {
            // Try to get PostScript name (name ID 6) or full font name (name ID 4)
            if (_nameTable?.NameRecords != null)
            {
                foreach (var record in _nameTable.NameRecords)
                {
                    if (record.NameId == 6 && record.PlatformId == 3) // PostScript name
                    {
                        // For now, use a simplified name
                        return "CustomTTFont";
                    }
                }
            }

            return "CustomTTFont";
        }

        private void CreateFontDictionary()
        {
            _dict.Add("Type", new HpdfName("Font"));
            _dict.Add("Subtype", new HpdfName("TrueType"));
            _dict.Add("BaseFont", new HpdfName(_baseFont));

            // Add FirstChar and LastChar
            byte firstChar = 32;
            byte lastChar = 255;

            if (_os2 != null)
            {
                firstChar = (byte)Math.Max(32, (int)_os2.FirstCharIndex);
                lastChar = (byte)Math.Min(255, (int)_os2.LastCharIndex);
            }

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
            // This tells the PDF reader which glyph to render for each byte value
            CreateEncodingDictionary(encoding, firstChar, lastChar);

            // Add Widths array (scaled to 1000-unit em square)
            // For each byte value in the encoding, we need to:
            // 1. Convert byte to Unicode using the code page
            // 2. Look up the glyph ID for that Unicode value
            // 3. Get the width for that glyph
            var widths = new HpdfArray();
            for (int i = firstChar; i <= lastChar; i++)
            {
                // Convert byte value to Unicode character using the code page
                byte[] byteArray = new byte[] { (byte)i };
                string str = encoding.GetString(byteArray);
                ushort unicode = (str.Length > 0) ? (ushort)str[0] : (ushort)0;

                // Get glyph ID for this Unicode character
                ushort glyphId = GetGlyphId(unicode);
                int width = GetGlyphWidth(glyphId);

                // Scale from font units to 1000-unit em square (PDF standard)
                int scaledWidth = (int)Math.Round(width * 1000.0 / _head.UnitsPerEm);
                widths.Add(new HpdfNumber(scaledWidth));
            }
            _dict.Add("Widths", widths);

            // Add to xref
            _xref.Add(_dict);
        }

        /// <summary>
        /// Creates a custom Encoding dictionary with Differences array for the specified code page.
        /// This maps byte codes to Unicode characters for proper glyph rendering.
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
                // Use uni[XXXX] format where XXXX is the Unicode hex value
                string glyphName = $"uni{unicode:X4}";

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

        private void CreateFontDescriptor()
        {
            _descriptor = new HpdfDict();
            _descriptor.Add("Type", new HpdfName("FontDescriptor"));
            _descriptor.Add("FontName", new HpdfName(_baseFont));

            // Calculate flags based on font properties
            int flags = CalculateFlags();
            _descriptor.Add("Flags", new HpdfNumber(flags));

            // Add font bounding box
            var bbox = new HpdfArray();
            bbox.Add(new HpdfNumber(_head.XMin));
            bbox.Add(new HpdfNumber(_head.YMin));
            bbox.Add(new HpdfNumber(_head.XMax));
            bbox.Add(new HpdfNumber(_head.YMax));
            _descriptor.Add("FontBBox", bbox);

            // Calculate and add italic angle
            double italicAngle = CalculateItalicAngle();
            _descriptor.Add("ItalicAngle", new HpdfReal((float)italicAngle));

            // Add metrics
            _descriptor.Add("Ascent", new HpdfNumber(_hhea.Ascender));
            _descriptor.Add("Descent", new HpdfNumber(_hhea.Descender));

            // Calculate CapHeight (use OS/2 or estimate from bbox)
            int capHeight = _os2 != null ? _os2.STypoAscender : _head.YMax;
            _descriptor.Add("CapHeight", new HpdfNumber(capHeight));

            // Calculate StemV (estimate based on font weight)
            int stemV = CalculateStemV();
            _descriptor.Add("StemV", new HpdfNumber(stemV));

            // Embed font data if requested
            if (_embedding && _fontData != null)
            {
                EmbedFontData();
            }

            // Add ToUnicode CMap for text extraction
            CreateToUnicodeCMap();

            // Link descriptor to font
            _dict.Add("FontDescriptor", _descriptor);

            // Add to xref
            _xref.Add(_descriptor);
        }

        /// <summary>
        /// Calculates font descriptor flags based on font properties.
        /// </summary>
        private int CalculateFlags()
        {
            int flags = 0;

            if (_os2 != null)
            {
                // Bit 1: FixedPitch
                if (_os2.Panose.Length > 3 && _os2.Panose[3] == 9)
                    flags |= 1;

                // Bit 2: Serif (from PANOSE)
                if (_os2.Panose.Length > 0 && _os2.Panose[0] == 2)
                    flags |= 2;

                // Bit 4: Script (from PANOSE)
                if (_os2.Panose.Length > 0 && _os2.Panose[0] == 3)
                    flags |= 8;

                // Bit 6: Italic
                if ((_os2.FsSelection & 0x01) != 0 || (_head.MacStyle & 0x02) != 0)
                    flags |= 64;
            }

            // Bit 6: Nonsymbolic (always set for TrueType text fonts)
            flags |= 32;

            return flags;
        }

        /// <summary>
        /// Calculates the italic angle from the post table.
        /// </summary>
        private double CalculateItalicAngle()
        {
            if (_post != null)
            {
                // ItalicAngle is in Fixed 16.16 format
                return _post.ItalicAngle / 65536.0;
            }

            // Check OS/2 and head tables for italic flag
            if (_os2 != null && (_os2.FsSelection & 0x01) != 0)
                return -12.0; // Default italic angle

            if ((_head.MacStyle & 0x02) != 0)
                return -12.0; // Default italic angle

            return 0.0;
        }

        /// <summary>
        /// Calculates StemV based on font weight.
        /// </summary>
        private int CalculateStemV()
        {
            if (_os2 != null)
            {
                // Estimate based on weight class (100-900)
                // Formula: StemV ≈ 50 + (weight - 400) / 5
                int weight = _os2.WeightClass;
                return Math.Max(50, 50 + (weight - 400) / 5);
            }

            return 80; // Default value
        }

        /// <summary>
        /// Embeds the font data in the PDF.
        /// </summary>
        private void EmbedFontData()
        {
            // Create font file stream
            _fontFileStream = new HpdfStreamObject();

            // For TrueType fonts, use FontFile2
            _fontFileStream.Add("Length1", new HpdfNumber(_fontData!.Length));

            // Add font data to stream
            _fontFileStream.WriteToStream(_fontData!);

            // Apply compression
            _fontFileStream.Filter = HpdfStreamFilter.FlateDecode;

            // Link to font descriptor
            _descriptor.Add("FontFile2", _fontFileStream);

            // Add to xref
            _xref.Add(_fontFileStream);
        }

        /// <summary>
        /// Creates the ToUnicode CMap for text extraction.
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
        /// Gets the glyph ID for a Unicode character.
        /// </summary>
        public ushort GetGlyphId(ushort unicode)
        {
            if (_cmap is null)
                return 0;

            int segCount = _cmap.SegCountX2 / 2;

            for (int i = 0; i < segCount; i++)
            {
                if (unicode <= _cmap.EndCount[i] && unicode >= _cmap.StartCount[i])
                {
                    if (_cmap.IdRangeOffset[i] == 0)
                    {
                        return (ushort)((unicode + _cmap.IdDelta[i]) & 0xFFFF);
                    }
                    else
                    {
                        int offset = _cmap.IdRangeOffset[i] / 2 + (unicode - _cmap.StartCount[i]) - (segCount - i);
                        if (offset >= 0 && offset < _cmap.GlyphIdArray.Length)
                        {
                            ushort glyphId = _cmap.GlyphIdArray[offset];
                            if (glyphId != 0)
                            {
                                return (ushort)((glyphId + _cmap.IdDelta[i]) & 0xFFFF);
                            }
                        }
                    }
                    break;
                }
            }

            return 0; // .notdef
        }

        /// <summary>
        /// Gets the width of a glyph in font units.
        /// </summary>
        public int GetGlyphWidth(ushort glyphId)
        {
            if (_hMetrics is null || glyphId >= _hMetrics.Length)
                return 0;

            return _hMetrics[glyphId].AdvanceWidth;
        }

        /// <summary>
        /// Gets the width of a character in font units.
        /// </summary>
        public float GetCharWidth(byte charCode)
        {
            ushort glyphId = GetGlyphId(charCode);
            return GetGlyphWidth(glyphId);
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

            // Convert from font units to user space
            float scale = fontSize / _head.UnitsPerEm;
            return totalWidth * scale;
        }

        /// <summary>
        /// Converts text to glyph IDs. Not applicable for TrueType fonts.
        /// </summary>
        public byte[] ConvertTextToGlyphIDs(string text)
        {
            throw new InvalidOperationException("The method is available for CID fonts only");
        }

        /// <summary>
        /// Releases all resources used by this TrueType font.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by this TrueType font and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _fontFileStream?.Dispose();
                    _toUnicodeStream?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
