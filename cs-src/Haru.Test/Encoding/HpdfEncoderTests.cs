/*
 * << Haru Free PDF Library >> -- HpdfEncoderTests.cs
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

using Xunit;
using Haru.Encoding;
using Haru.Objects;

namespace Haru.Test.Encoding
{
    /// <summary>
    /// Tests for the PDF encoder system.
    /// </summary>
    public class HpdfEncoderTests
    {
        [Fact]
        public void WinAnsiEncoding_Should_Be_Available()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("WinAnsiEncoding");

            Assert.NotNull(encoder);
            Assert.Equal("WinAnsiEncoding", encoder.Name);
            Assert.Equal("WinAnsiEncoding", encoder.BaseEncodingName);
            Assert.False(encoder.HasDifferences);
        }

        [Fact]
        public void WinAnsiEncoding_Should_Map_ASCII_Correctly()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("WinAnsiEncoding")!;

            // Test basic ASCII characters
            Assert.Equal((ushort)0x0041, encoder.GetUnicode(0x41));  // 'A'
            Assert.Equal((ushort)0x0061, encoder.GetUnicode(0x61));  // 'a'
            Assert.Equal((ushort)0x0030, encoder.GetUnicode(0x30));  // '0'
            Assert.Equal((ushort)0x0020, encoder.GetUnicode(0x20));  // space

            // Test reverse mapping
            Assert.Equal((byte)0x41, encoder.GetByteValue('A')!.Value);
            Assert.Equal((byte)0x61, encoder.GetByteValue('a')!.Value);
            Assert.Equal((byte)0x30, encoder.GetByteValue('0')!.Value);
            Assert.Equal((byte)0x20, encoder.GetByteValue(' ')!.Value);
        }

        [Fact]
        public void WinAnsiEncoding_Should_Not_Create_Differences_Array()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("WinAnsiEncoding")!;

            var differences = encoder.CreateDifferencesArray();
            Assert.Null(differences);
        }

        [Theory]
        [InlineData("CP1250")]  // Central European
        [InlineData("CP1251")]  // Cyrillic
        [InlineData("CP1252")]  // Western European (same as WinAnsi)
        [InlineData("CP1253")]  // Greek
        [InlineData("CP1254")]  // Turkish
        [InlineData("CP1255")]  // Hebrew
        [InlineData("CP1256")]  // Arabic
        [InlineData("CP1257")]  // Baltic
        [InlineData("CP1258")]  // Vietnamese
        public void All_CP125x_Encoders_Should_Be_Available(string encodingName)
        {
            var encoder = HpdfEncoderFactory.GetEncoder(encodingName);

            Assert.NotNull(encoder);
            Assert.Equal(encodingName, encoder.Name);
        }

        [Theory]
        [InlineData("ISO8859-2")]   // Latin-2 (Central European)
        [InlineData("ISO8859-3")]   // Latin-3 (South European)
        [InlineData("ISO8859-4")]   // Latin-4 (North European)
        [InlineData("ISO8859-5")]   // Cyrillic
        [InlineData("ISO8859-6")]   // Arabic
        [InlineData("ISO8859-7")]   // Greek
        [InlineData("ISO8859-8")]   // Hebrew
        [InlineData("ISO8859-9")]   // Turkish
        [InlineData("ISO8859-10")]  // Nordic
        [InlineData("ISO8859-11")]  // Thai
        [InlineData("ISO8859-13")]  // Baltic Rim
        [InlineData("ISO8859-14")]  // Celtic
        [InlineData("ISO8859-15")]  // Western European with Euro
        [InlineData("ISO8859-16")]  // South-Eastern European
        public void All_ISO8859_Encoders_Should_Be_Available(string encodingName)
        {
            var encoder = HpdfEncoderFactory.GetEncoder(encodingName);

            Assert.NotNull(encoder);
            Assert.Equal(encodingName, encoder.Name);
        }

        [Fact]
        public void KOI8R_Encoder_Should_Be_Available()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("KOI8-R");

            Assert.NotNull(encoder);
            Assert.Equal("KOI8-R", encoder.Name);
        }

        [Fact]
        public void StandardEncoding_Should_Be_Available()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("StandardEncoding");

            Assert.NotNull(encoder);
            Assert.Equal("StandardEncoding", encoder.Name);
            Assert.Equal("StandardEncoding", encoder.BaseEncodingName);
        }

        [Fact]
        public void MacRomanEncoding_Should_Be_Available()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("MacRomanEncoding");

            Assert.NotNull(encoder);
            Assert.Equal("MacRomanEncoding", encoder.Name);
            Assert.Equal("MacRomanEncoding", encoder.BaseEncodingName);
        }

        [Fact]
        public void CP1251_Should_Map_Cyrillic_Characters_Correctly()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("CP1251")!;

            // Test Cyrillic capital А (U+0410) should be at byte 0xC0 (192)
            Assert.Equal((ushort)0x0410, encoder.GetUnicode(0xC0));
            Assert.Equal((byte)0xC0, encoder.GetByteValue('А')!.Value);

            // Test Cyrillic lowercase а (U+0430) should be at byte 0xE0 (224)
            Assert.Equal((ushort)0x0430, encoder.GetUnicode(0xE0));
            Assert.Equal((byte)0xE0, encoder.GetByteValue('а')!.Value);

            // Test some more Cyrillic characters
            Assert.Equal((ushort)0x0411, encoder.GetUnicode(0xC1));  // Б
            Assert.Equal((ushort)0x0412, encoder.GetUnicode(0xC2));  // В
            Assert.Equal((ushort)0x0413, encoder.GetUnicode(0xC3));  // Г
        }

        [Fact]
        public void CP1251_Should_Have_Differences_From_WinAnsi()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("CP1251")!;

            Assert.True(encoder.HasDifferences);
            Assert.Equal("WinAnsiEncoding", encoder.BaseEncodingName);
        }

        [Fact]
        public void CP1251_Should_Create_Valid_Differences_Array()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("CP1251")!;

            var differences = encoder.CreateDifferencesArray();
            Assert.NotNull(differences);
            Assert.True(differences!.Count > 0);

            // The differences array should contain HpdfNumber and HpdfName objects
            bool hasNumbers = false;
            bool hasNames = false;

            foreach (var item in differences)
            {
                if (item is HpdfNumber) hasNumbers = true;
                if (item is HpdfName) hasNames = true;
            }

            Assert.True(hasNumbers, "Differences array should contain HpdfNumber entries for byte positions");
            Assert.True(hasNames, "Differences array should contain HpdfName entries for glyph names");
        }

        [Fact]
        public void Encoder_Should_Encode_Text_Correctly()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("WinAnsiEncoding")!;

            byte[] result = encoder.EncodeText("Hello");

            Assert.Equal(5, result.Length);
            Assert.Equal((byte)'H', result[0]);
            Assert.Equal((byte)'e', result[1]);
            Assert.Equal((byte)'l', result[2]);
            Assert.Equal((byte)'l', result[3]);
            Assert.Equal((byte)'o', result[4]);
        }

        [Fact]
        public void Encoder_Should_Handle_Empty_Text()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("WinAnsiEncoding")!;

            byte[] result1 = encoder.EncodeText("");
            byte[] result2 = encoder.EncodeText(null!);

            Assert.NotNull(result1);
            Assert.Empty(result1);
            Assert.NotNull(result2);
            Assert.Empty(result2);
        }

        [Fact]
        public void Encoder_Should_Handle_Unmappable_Characters()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("WinAnsiEncoding")!;

            // Try to encode Cyrillic characters with WinAnsi encoder
            // Should return '?' (0x3F) for unmappable characters
            byte[] result = encoder.EncodeText("Привет");  // Russian "Hello"

            Assert.Equal(6, result.Length);
            // All characters should be replaced with '?' since WinAnsi doesn't support Cyrillic
            Assert.All(result, b => Assert.Equal((byte)'?', b));
        }

        [Fact]
        public void CP1251_Should_Encode_Cyrillic_Text_Correctly()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("CP1251")!;

            // Encode "Привет" (Russian "Hello")
            byte[] result = encoder.EncodeText("Привет");

            Assert.Equal(6, result.Length);
            // П=0xCF, р=0xF0, и=0xE8, в=0xE2, е=0xE5, т=0xF2
            Assert.Equal((byte)0xCF, result[0]);
            Assert.Equal((byte)0xF0, result[1]);
            Assert.Equal((byte)0xE8, result[2]);
            Assert.Equal((byte)0xE2, result[3]);
            Assert.Equal((byte)0xE5, result[4]);
            Assert.Equal((byte)0xF2, result[5]);
        }

        [Fact]
        public void CP1255_Should_Map_Hebrew_Characters()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("CP1255")!;

            // Test Hebrew Alef (U+05D0) should be at byte 0xE0 (224)
            Assert.Equal((ushort)0x05D0, encoder.GetUnicode(0xE0));

            // Test Hebrew Bet (U+05D1) should be at byte 0xE1 (225)
            Assert.Equal((ushort)0x05D1, encoder.GetUnicode(0xE1));
        }

        [Fact]
        public void CP1256_Should_Map_Arabic_Characters()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("CP1256")!;

            // Test Arabic Letter Hamza (U+0621) should be at byte 0xC1 (193)
            Assert.Equal((ushort)0x0621, encoder.GetUnicode(0xC1));
        }

        [Fact]
        public void CP1253_Should_Map_Greek_Characters()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("CP1253")!;

            // Test Greek Capital Alpha (U+0391) should be at byte 0xC1 (193)
            Assert.Equal((ushort)0x0391, encoder.GetUnicode(0xC1));

            // Test Greek Capital Beta (U+0392) should be at byte 0xC2 (194)
            Assert.Equal((ushort)0x0392, encoder.GetUnicode(0xC2));
        }

        [Fact]
        public void ISO8859_5_Should_Map_Cyrillic_Characters()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("ISO8859-5")!;

            // ISO8859-5 has Cyrillic at different byte positions than CP1251
            // Cyrillic capital А (U+0410) should be at byte 0xB0 (176)
            Assert.Equal((ushort)0x0410, encoder.GetUnicode(0xB0));
        }

        [Fact]
        public void KOI8R_Should_Map_Cyrillic_Characters()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("KOI8-R")!;

            // KOI8-R uses a different mapping for Cyrillic
            // Test that Cyrillic characters are mapped (exact positions vary)
            bool foundCyrillic = false;
            for (int i = 0x80; i < 0x100; i++)
            {
                ushort unicode = encoder.GetUnicode((byte)i);
                if (unicode >= 0x0410 && unicode <= 0x044F)
                {
                    foundCyrillic = true;
                    break;
                }
            }
            Assert.True(foundCyrillic, "KOI8-R should map Cyrillic characters in the upper byte range");
        }

        [Fact]
        public void GetEncoder_Should_Return_Default_For_Unknown_Encoding()
        {
            var encoder = HpdfEncoderFactory.GetEncoder("UnknownEncoding");

            Assert.NotNull(encoder);
            // Should default to WinAnsiEncoding
            Assert.Equal("WinAnsiEncoding", encoder.Name);
        }

        [Fact]
        public void GetEncoder_Should_Be_Case_Insensitive()
        {
            var encoder1 = HpdfEncoderFactory.GetEncoder("CP1251");
            var encoder2 = HpdfEncoderFactory.GetEncoder("cp1251");
            var encoder3 = HpdfEncoderFactory.GetEncoder("cP1251");

            Assert.NotNull(encoder1);
            Assert.NotNull(encoder2);
            Assert.NotNull(encoder3);
            Assert.Equal("CP1251", encoder1.Name);
            Assert.Equal("CP1251", encoder2.Name);
            Assert.Equal("CP1251", encoder3.Name);
        }
    }
}
