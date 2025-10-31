/*
 * << Haru Free PDF Library >> -- PfbParser.cs
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

namespace Haru.Font.Type1
{
    /// <summary>
    /// Parser for Printer Font Binary (PFB) files.
    /// PFB files contain the actual Type 1 font program in binary format.
    ///
    /// PFB format:
    /// - Segment header: 0x80 followed by segment type (1=ASCII, 2=binary, 3=EOF)
    /// - Length: 4 bytes (little-endian) for the segment length
    /// - Data: segment data
    /// </summary>
    public class PfbParser
    {
        private const byte PFB_MARKER = 0x80;
        private const byte PFB_ASCII = 0x01;
        private const byte PFB_BINARY = 0x02;
        private const byte PFB_EOF = 0x03;

        /// <summary>
        /// Parses a PFB file and extracts the font data suitable for PDF embedding.
        /// The returned data is the raw Type 1 font program (ASCII + binary sections).
        /// </summary>
        public static byte[] ParseFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new HpdfException(HpdfErrorCode.FileNotFound, $"PFB file not found: {filePath}");

            using (var stream = File.OpenRead(filePath))
            {
                return ParseStream(stream);
            }
        }

        /// <summary>
        /// Parses a PFB stream and extracts the font data.
        /// </summary>
        public static byte[] ParseStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream, System.Text.Encoding.ASCII, leaveOpen: true))
            using (var output = new MemoryStream())
            {
                while (true)
                {
                    // Read segment marker
                    byte marker = reader.ReadByte();
                    if (marker != PFB_MARKER)
                        throw new HpdfException(HpdfErrorCode.InvalidFontDefData, "Invalid PFB file: Missing segment marker");

                    // Read segment type
                    byte segmentType = reader.ReadByte();

                    if (segmentType == PFB_EOF)
                        break;

                    if (segmentType != PFB_ASCII && segmentType != PFB_BINARY)
                        throw new HpdfException(HpdfErrorCode.InvalidFontDefData, $"Invalid PFB segment type: {segmentType}");

                    // Read segment length (little-endian)
                    byte[] lengthBytes = reader.ReadBytes(4);
                    int length = BitConverter.ToInt32(lengthBytes, 0);

                    // Read segment data
                    byte[] data = reader.ReadBytes(length);
                    output.Write(data, 0, data.Length);
                }

                return output.ToArray();
            }
        }

        /// <summary>
        /// Extracts the lengths of the ASCII and binary sections from a PFB file.
        /// Returns (length1, length2, length3) where:
        /// - length1: length of ASCII section (cleartext)
        /// - length2: length of binary section (encrypted)
        /// - length3: length of ASCII trailer section
        /// These are needed for the PDF FontFile dictionary.
        /// </summary>
        public static (int length1, int length2, int length3) GetSectionLengths(string filePath)
        {
            if (!File.Exists(filePath))
                throw new HpdfException(HpdfErrorCode.FileNotFound, $"PFB file not found: {filePath}");

            using (var stream = File.OpenRead(filePath))
            {
                return GetSectionLengths(stream);
            }
        }

        /// <summary>
        /// Extracts the lengths of the ASCII and binary sections from a PFB stream.
        /// </summary>
        public static (int length1, int length2, int length3) GetSectionLengths(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            int length1 = 0;
            int length2 = 0;
            int length3 = 0;
            int sectionIndex = 0;

            using (var reader = new BinaryReader(stream, System.Text.Encoding.ASCII, leaveOpen: true))
            {
                while (true)
                {
                    // Read segment marker
                    byte marker = reader.ReadByte();
                    if (marker != PFB_MARKER)
                        throw new HpdfException(HpdfErrorCode.InvalidFontDefData, "Invalid PFB file: Missing segment marker");

                    // Read segment type
                    byte segmentType = reader.ReadByte();

                    if (segmentType == PFB_EOF)
                        break;

                    if (segmentType != PFB_ASCII && segmentType != PFB_BINARY)
                        throw new HpdfException(HpdfErrorCode.InvalidFontDefData, $"Invalid PFB segment type: {segmentType}");

                    // Read segment length (little-endian)
                    byte[] lengthBytes = reader.ReadBytes(4);
                    int length = BitConverter.ToInt32(lengthBytes, 0);

                    // Assign to appropriate section
                    if (segmentType == PFB_ASCII)
                    {
                        if (sectionIndex == 0)
                            length1 = length;
                        else
                            length3 = length;
                    }
                    else if (segmentType == PFB_BINARY)
                    {
                        length2 = length;
                    }

                    sectionIndex++;

                    // Skip the data
                    stream.Seek(length, SeekOrigin.Current);
                }
            }

            return (length1, length2, length3);
        }
    }
}
