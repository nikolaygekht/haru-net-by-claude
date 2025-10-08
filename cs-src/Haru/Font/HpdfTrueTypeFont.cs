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

namespace Haru.Font
{
    /// <summary>
    /// Represents a TrueType font that can be embedded in a PDF.
    /// </summary>
    public class HpdfTrueTypeFont
    {
        private readonly HpdfDict _dict;
        private string _baseFont;
        private readonly string _localName;
        private readonly HpdfXref _xref;

        // TrueType font data
        private TrueTypeOffsetTable _offsetTable;
        private TrueTypeHead _head;
        private TrueTypeMaxp _maxp;
        private TrueTypeHhea _hhea;
        private TrueTypeLongHorMetric[] _hMetrics;
        private TrueTypeNameTable _nameTable;
        private TrueTypeCmapFormat4 _cmap;
        private TrueTypeOS2 _os2;
        private TrueTypeGlyphOffsets _glyphOffsets;

        private byte[] _fontData;
        private bool _embedding;
        private HpdfDict _descriptor;

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
        /// Gets the font descriptor dictionary.
        /// </summary>
        public HpdfDict Descriptor => _descriptor;

        private HpdfTrueTypeFont(HpdfXref xref, string localName, bool embedding)
        {
            _xref = xref ?? throw new ArgumentNullException(nameof(xref));
            _localName = localName ?? throw new ArgumentNullException(nameof(localName));
            _embedding = embedding;
            _dict = new HpdfDict();
        }

        /// <summary>
        /// Loads a TrueType font from a file.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="localName">Local resource name (e.g., "F1").</param>
        /// <param name="filePath">Path to the TTF file.</param>
        /// <param name="embedding">Whether to embed the font data.</param>
        /// <returns>The loaded TrueType font.</returns>
        public static HpdfTrueTypeFont LoadFromFile(HpdfXref xref, string localName, string filePath, bool embedding)
        {
            if (!File.Exists(filePath))
                throw new HpdfException(HpdfErrorCode.FileNotFound, $"Font file not found: {filePath}");

            byte[] fontData = File.ReadAllBytes(filePath);

            using (var stream = new MemoryStream(fontData))
            {
                return LoadFromStream(xref, localName, stream, fontData, embedding);
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
        /// <returns>The loaded TrueType font.</returns>
        public static HpdfTrueTypeFont LoadFromStream(HpdfXref xref, string localName, Stream stream, byte[] fontData, bool embedding)
        {
            var font = new HpdfTrueTypeFont(xref, localName, embedding);

            if (embedding)
                font._fontData = fontData;

            var parser = new TrueTypeParser(stream);

            // Parse required tables
            font._offsetTable = parser.ParseOffsetTable();

            var headTable = parser.FindTable(font._offsetTable, "head");
            font._head = parser.ParseHead(headTable);

            var maxpTable = parser.FindTable(font._offsetTable, "maxp");
            font._maxp = parser.ParseMaxp(maxpTable);

            var hheaTable = parser.FindTable(font._offsetTable, "hhea");
            font._hhea = parser.ParseHhea(hheaTable);

            var hmtxTable = parser.FindTable(font._offsetTable, "hmtx");
            font._hMetrics = parser.ParseHmtx(hmtxTable, font._hhea.NumberOfHMetrics, font._maxp.NumGlyphs);

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
            if (cmapTable == null)
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

            // For now, use WinAnsiEncoding
            _dict.Add("Encoding", new HpdfName("WinAnsiEncoding"));

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

            // Add Widths array
            var widths = new HpdfArray();
            for (int i = firstChar; i <= lastChar; i++)
            {
                ushort glyphId = GetGlyphId((ushort)i);
                int width = GetGlyphWidth(glyphId);
                widths.Add(new HpdfNumber(width));
            }
            _dict.Add("Widths", widths);

            // Add to xref
            _xref.Add(_dict);
        }

        private void CreateFontDescriptor()
        {
            _descriptor = new HpdfDict();
            _descriptor.Add("Type", new HpdfName("FontDescriptor"));
            _descriptor.Add("FontName", new HpdfName(_baseFont));

            // Calculate flags
            int flags = 0;
            if (_os2 != null)
            {
                if ((_os2.Panose[3] == 9)) // Monospaced
                    flags |= 1; // FixedPitch
                flags |= 32; // Nonsymbolic
            }
            else
            {
                flags = 32; // Nonsymbolic
            }

            _descriptor.Add("Flags", new HpdfNumber(flags));

            // Add font bounding box
            var bbox = new HpdfArray();
            bbox.Add(new HpdfNumber(_head.XMin));
            bbox.Add(new HpdfNumber(_head.YMin));
            bbox.Add(new HpdfNumber(_head.XMax));
            bbox.Add(new HpdfNumber(_head.YMax));
            _descriptor.Add("FontBBox", bbox);

            // Add metrics
            _descriptor.Add("ItalicAngle", new HpdfNumber(0));
            _descriptor.Add("Ascent", new HpdfNumber(_hhea.Ascender));
            _descriptor.Add("Descent", new HpdfNumber(_hhea.Descender));
            _descriptor.Add("CapHeight", new HpdfNumber(_head.YMax));
            _descriptor.Add("StemV", new HpdfNumber(80)); // Default value

            // Link descriptor to font
            _dict.Add("FontDescriptor", _descriptor);

            // Add to xref
            _xref.Add(_descriptor);
        }

        /// <summary>
        /// Gets the glyph ID for a Unicode character.
        /// </summary>
        public ushort GetGlyphId(ushort unicode)
        {
            if (_cmap == null)
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
            if (_hMetrics == null || glyphId >= _hMetrics.Length)
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
    }
}
