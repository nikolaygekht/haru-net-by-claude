/*
 * << Haru Free PDF Library >> -- TrueTypeParser.cs
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
using System.Text;

namespace Haru.Font.TrueType
{
    /// <summary>
    /// Parses TrueType font files and extracts table data.
    /// </summary>
    internal class TrueTypeParser : IDisposable
    {
        private readonly Stream _stream;
        private readonly BinaryReader _reader;
        private bool _disposed = false;

        public TrueTypeParser(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
            _reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
        }

        /// <summary>
        /// Reads a big-endian UInt16.
        /// </summary>
        public ushort ReadUInt16()
        {
            byte b1 = _reader.ReadByte();
            byte b2 = _reader.ReadByte();
            return (ushort)((b1 << 8) | b2);
        }

        /// <summary>
        /// Reads a big-endian Int16.
        /// </summary>
        public short ReadInt16()
        {
            byte b1 = _reader.ReadByte();
            byte b2 = _reader.ReadByte();
            return (short)((b1 << 8) | b2);
        }

        /// <summary>
        /// Reads a big-endian UInt32.
        /// </summary>
        public uint ReadUInt32()
        {
            byte b1 = _reader.ReadByte();
            byte b2 = _reader.ReadByte();
            byte b3 = _reader.ReadByte();
            byte b4 = _reader.ReadByte();
            return (uint)((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
        }

        /// <summary>
        /// Reads a big-endian Int32.
        /// </summary>
        public int ReadInt32()
        {
            byte b1 = _reader.ReadByte();
            byte b2 = _reader.ReadByte();
            byte b3 = _reader.ReadByte();
            byte b4 = _reader.ReadByte();
            return (int)((b1 << 24) | (b2 << 16) | (b3 << 8) | b4);
        }

        /// <summary>
        /// Reads a 4-byte tag as a string.
        /// </summary>
        public string ReadTag()
        {
            byte[] bytes = _reader.ReadBytes(4);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Seeks to a specific position in the stream.
        /// </summary>
        public void Seek(long offset)
        {
            _stream.Position = offset;
        }

        /// <summary>
        /// Gets the current position in the stream.
        /// </summary>
        public long Position => _stream.Position;

        /// <summary>
        /// Reads a byte array.
        /// </summary>
        public byte[] ReadBytes(int count)
        {
            return _reader.ReadBytes(count);
        }

        /// <summary>
        /// Parses the offset table (table directory) from the font file.
        /// </summary>
        public TrueTypeOffsetTable ParseOffsetTable()
        {
            _stream.Position = 0;

            var offsetTable = new TrueTypeOffsetTable
            {
                SfntVersion = ReadUInt32(),
                NumTables = ReadUInt16(),
                SearchRange = ReadUInt16(),
                EntrySelector = ReadUInt16(),
                RangeShift = ReadUInt16()
            };

            offsetTable.Tables = new TrueTypeTable[offsetTable.NumTables];

            for (int i = 0; i < offsetTable.NumTables; i++)
            {
                offsetTable.Tables[i] = new TrueTypeTable
                {
                    Tag = ReadTag(),
                    CheckSum = ReadUInt32(),
                    Offset = ReadUInt32(),
                    Length = ReadUInt32()
                };
            }

            return offsetTable;
        }

        /// <summary>
        /// Finds a table by its tag.
        /// </summary>
        public TrueTypeTable? FindTable(TrueTypeOffsetTable offsetTable, string tag)
        {
            if (offsetTable?.Tables is null)
                return null;

            foreach (var table in offsetTable.Tables)
            {
                if (table.Tag == tag)
                    return table;
            }

            return null;
        }

        /// <summary>
        /// Parses the 'head' table.
        /// </summary>
        public TrueTypeHead ParseHead(TrueTypeTable table)
        {
            if (table is null)
                throw new HpdfException(HpdfErrorCode.TtInvalidFormat, "head table not found");

            Seek(table.Offset);

            var head = new TrueTypeHead
            {
                VersionNumber = ReadBytes(4),
                FontRevision = ReadUInt32(),
                CheckSumAdjustment = ReadUInt32(),
                MagicNumber = ReadUInt32(),
                Flags = ReadUInt16(),
                UnitsPerEm = ReadUInt16(),
                Created = ReadBytes(8),
                Modified = ReadBytes(8),
                XMin = ReadInt16(),
                YMin = ReadInt16(),
                XMax = ReadInt16(),
                YMax = ReadInt16(),
                MacStyle = ReadUInt16(),
                LowestRecPPEM = ReadUInt16(),
                FontDirectionHint = ReadInt16(),
                IndexToLocFormat = ReadInt16(),
                GlyphDataFormat = ReadInt16()
            };

            // Validate magic number
            if (head.MagicNumber != 0x5F0F3CF5)
                throw new HpdfException(HpdfErrorCode.TtInvalidFormat, "Invalid head table magic number");

            return head;
        }

        /// <summary>
        /// Parses the 'maxp' table.
        /// </summary>
        public TrueTypeMaxp ParseMaxp(TrueTypeTable table)
        {
            if (table is null)
                throw new HpdfException(HpdfErrorCode.TtInvalidFormat, "maxp table not found");

            Seek(table.Offset);

            var maxp = new TrueTypeMaxp
            {
                Version = ReadUInt32(),
                NumGlyphs = ReadUInt16()
            };

            // Version 1.0 has additional fields
            if (maxp.Version == 0x00010000)
            {
                maxp.MaxPoints = ReadUInt16();
                maxp.MaxContours = ReadUInt16();
                maxp.MaxCompositePoints = ReadUInt16();
                maxp.MaxCompositeContours = ReadUInt16();
                maxp.MaxZones = ReadUInt16();
                maxp.MaxTwilightPoints = ReadUInt16();
                maxp.MaxStorage = ReadUInt16();
                maxp.MaxFunctionDefs = ReadUInt16();
                maxp.MaxInstructionDefs = ReadUInt16();
                maxp.MaxStackElements = ReadUInt16();
                maxp.MaxSizeOfInstructions = ReadUInt16();
                maxp.MaxComponentElements = ReadUInt16();
                maxp.MaxComponentDepth = ReadUInt16();
            }

            return maxp;
        }

        /// <summary>
        /// Parses the 'hhea' table.
        /// </summary>
        public TrueTypeHhea ParseHhea(TrueTypeTable table)
        {
            if (table is null)
                throw new HpdfException(HpdfErrorCode.TtInvalidFormat, "hhea table not found");

            Seek(table.Offset);

            var hhea = new TrueTypeHhea
            {
                Version = ReadUInt32(),
                Ascender = ReadInt16(),
                Descender = ReadInt16(),
                LineGap = ReadInt16(),
                AdvanceWidthMax = ReadUInt16(),
                MinLeftSideBearing = ReadInt16(),
                MinRightSideBearing = ReadInt16(),
                XMaxExtent = ReadInt16(),
                CaretSlopeRise = ReadInt16(),
                CaretSlopeRun = ReadInt16(),
                CaretOffset = ReadInt16(),
                Reserved1 = ReadInt16(),
                Reserved2 = ReadInt16(),
                Reserved3 = ReadInt16(),
                Reserved4 = ReadInt16(),
                MetricDataFormat = ReadInt16(),
                NumberOfHMetrics = ReadUInt16()
            };

            return hhea;
        }

        /// <summary>
        /// Parses the 'hmtx' table.
        /// </summary>
        public TrueTypeLongHorMetric[] ParseHmtx(TrueTypeTable table, ushort numberOfHMetrics, ushort numGlyphs)
        {
            if (table is null)
                throw new HpdfException(HpdfErrorCode.TtInvalidFormat, "hmtx table not found");

            Seek(table.Offset);

            var metrics = new TrueTypeLongHorMetric[numGlyphs];

            // Read long horizontal metrics
            for (int i = 0; i < numberOfHMetrics; i++)
            {
                metrics[i] = new TrueTypeLongHorMetric
                {
                    AdvanceWidth = ReadUInt16(),
                    LeftSideBearing = ReadInt16()
                };
            }

            // Remaining glyphs have the same advance width as the last metric
            if (numberOfHMetrics > 0)
            {
                ushort lastAdvanceWidth = metrics[numberOfHMetrics - 1].AdvanceWidth;
                for (int i = numberOfHMetrics; i < numGlyphs; i++)
                {
                    metrics[i] = new TrueTypeLongHorMetric
                    {
                        AdvanceWidth = lastAdvanceWidth,
                        LeftSideBearing = ReadInt16()
                    };
                }
            }

            return metrics;
        }

        /// <summary>
        /// Parses the 'name' table.
        /// </summary>
        public TrueTypeNameTable ParseName(TrueTypeTable table)
        {
            if (table is null)
                throw new HpdfException(HpdfErrorCode.TtInvalidFormat, "name table not found");

            Seek(table.Offset);
            long tableStart = table.Offset;

            var nameTable = new TrueTypeNameTable
            {
                Format = ReadUInt16(),
                Count = ReadUInt16(),
                StringOffset = ReadUInt16()
            };

            nameTable.NameRecords = new TrueTypeNameRecord[nameTable.Count];

            for (int i = 0; i < nameTable.Count; i++)
            {
                nameTable.NameRecords[i] = new TrueTypeNameRecord
                {
                    PlatformId = ReadUInt16(),
                    EncodingId = ReadUInt16(),
                    LanguageId = ReadUInt16(),
                    NameId = ReadUInt16(),
                    Length = ReadUInt16(),
                    Offset = ReadUInt16()
                };
            }

            return nameTable;
        }

        /// <summary>
        /// Reads a string from the 'name' table.
        /// </summary>
        public string? ReadNameString(TrueTypeNameTable nameTable, TrueTypeTable table, TrueTypeNameRecord record)
        {
            if (nameTable is null || table is null || record is null)
                return null;

            Seek(table.Offset + nameTable.StringOffset + record.Offset);
            byte[] bytes = ReadBytes(record.Length);

            // Unicode strings (platform 0 or 3)
            if (record.PlatformId == 0 || record.PlatformId == 3)
            {
                return Encoding.BigEndianUnicode.GetString(bytes);
            }
            else
            {
                return Encoding.ASCII.GetString(bytes);
            }
        }

        /// <summary>
        /// Parses the 'post' table.
        /// </summary>
        public TrueTypePost? ParsePost(TrueTypeTable table)
        {
            if (table is null)
                return null;

            Seek(table.Offset);

            var post = new TrueTypePost
            {
                Version = ReadUInt32(),
                ItalicAngle = ReadInt32(),
                UnderlinePosition = ReadInt16(),
                UnderlineThickness = ReadInt16(),
                IsFixedPitch = ReadUInt32(),
                MinMemType42 = ReadUInt32(),
                MaxMemType42 = ReadUInt32(),
                MinMemType1 = ReadUInt32(),
                MaxMemType1 = ReadUInt32()
            };

            return post;
        }

        /// <summary>
        /// Parses the 'loca' table (glyph location table).
        /// </summary>
        /// <param name="table">The loca table.</param>
        /// <param name="indexToLocFormat">Format from head table (0=short, 1=long).</param>
        /// <param name="numGlyphs">Number of glyphs from maxp table.</param>
        /// <returns>Array of glyph offsets.</returns>
        public uint[] ParseLoca(TrueTypeTable table, short indexToLocFormat, ushort numGlyphs)
        {
            if (table is null)
                throw new HpdfException(HpdfErrorCode.TtInvalidFormat, "loca table not found");

            Seek(table.Offset);

            uint[] offsets = new uint[numGlyphs + 1];

            if (indexToLocFormat == 0)
            {
                // Short format: offsets are stored as ushort / 2
                for (int i = 0; i <= numGlyphs; i++)
                {
                    offsets[i] = (uint)(ReadUInt16() * 2);
                }
            }
            else
            {
                // Long format: offsets are stored as uint32
                for (int i = 0; i <= numGlyphs; i++)
                {
                    offsets[i] = ReadUInt32();
                }
            }

            return offsets;
        }

        /// <summary>
        /// Reads glyph data from the 'glyf' table.
        /// </summary>
        /// <param name="glyfTable">The glyf table.</param>
        /// <param name="glyphOffset">Offset to the glyph within the glyf table.</param>
        /// <param name="nextGlyphOffset">Offset to the next glyph (to determine size).</param>
        /// <returns>The glyph data.</returns>
        public TrueTypeGlyphData ReadGlyphData(TrueTypeTable glyfTable, uint glyphOffset, uint nextGlyphOffset)
        {
            if (glyfTable is null)
                throw new HpdfException(HpdfErrorCode.TtInvalidFormat, "glyf table not found");

            uint glyphLength = nextGlyphOffset - glyphOffset;

            // Empty glyph (e.g., space character)
            if (glyphLength == 0)
            {
                return new TrueTypeGlyphData
                {
                    Data = new byte[0],
                    IsComposite = false,
                    ComponentGlyphs = new ushort[0]
                };
            }

            Seek(glyfTable.Offset + glyphOffset);

            // Read glyph header
            var header = new TrueTypeGlyphHeader
            {
                NumberOfContours = ReadInt16(),
                XMin = ReadInt16(),
                YMin = ReadInt16(),
                XMax = ReadInt16(),
                YMax = ReadInt16()
            };

            bool isComposite = header.NumberOfContours < 0;
            var componentGlyphs = new System.Collections.Generic.List<ushort>();

            // If composite glyph, parse component references
            if (isComposite)
            {
                ushort flags;
                do
                {
                    flags = ReadUInt16();
                    ushort glyphIndex = ReadUInt16();
                    componentGlyphs.Add(glyphIndex);

                    // Skip arguments and transformation data based on flags
                    bool arg1And2AreWords = (flags & 0x0001) != 0;
                    bool weHaveAScale = (flags & 0x0008) != 0;
                    bool weHaveAnXAndYScale = (flags & 0x0040) != 0;
                    bool weHaveATwoByTwo = (flags & 0x0080) != 0;

                    // Skip arguments (1 or 2 bytes each)
                    if (arg1And2AreWords)
                    {
                        ReadInt16(); // arg1
                        ReadInt16(); // arg2
                    }
                    else
                    {
                        _reader.ReadByte(); // arg1
                        _reader.ReadByte(); // arg2
                    }

                    // Skip transformation matrix
                    if (weHaveATwoByTwo)
                    {
                        ReadInt16(); // xscale
                        ReadInt16(); // scale01
                        ReadInt16(); // scale10
                        ReadInt16(); // yscale
                    }
                    else if (weHaveAnXAndYScale)
                    {
                        ReadInt16(); // xscale
                        ReadInt16(); // yscale
                    }
                    else if (weHaveAScale)
                    {
                        ReadInt16(); // scale
                    }

                } while ((flags & 0x0020) != 0); // MORE_COMPONENTS flag
            }

            // Read entire glyph data
            Seek(glyfTable.Offset + glyphOffset);
            byte[] glyphData = ReadBytes((int)glyphLength);

            return new TrueTypeGlyphData
            {
                Data = glyphData,
                IsComposite = isComposite,
                ComponentGlyphs = componentGlyphs.ToArray()
            };
        }

        /// <summary>
        /// Reads a Fixed-point number (16.16 format).
        /// </summary>
        public int ReadFixed()
        {
            return ReadInt32();
        }

        /// <summary>
        /// Disposes the parser and releases resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the parser
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose the BinaryReader (which wraps the stream)
                    // Stream is left open due to leaveOpen: true
                    _reader?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
