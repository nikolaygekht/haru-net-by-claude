using System;
using System.IO;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Font;
using Haru.Font.CID;

namespace Haru.Test.Font
{
    public class HpdfCIDFontTests
    {
        // Note: These tests require actual font files to be present
        // For CI/CD, use test font files or skip if not available

        private const string TestFontPath = "demo/ttfont/noto-jp.ttf";
        private const int JapaneseCodePage = 932;  // Shift-JIS

        private bool TestFontExists()
        {
            return File.Exists(TestFontPath);
        }

        [Fact]
        public void LoadFromTrueTypeFile_ValidFont_LoadsSuccessfully()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();

            // Act
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.LocalName.Should().Be("TestFont");
            cidFont.CodePage.Should().Be(JapaneseCodePage);
            cidFont.BaseFont.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void LoadFromTrueTypeFile_AutoUpgradesPdfVersion()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            doc.Version.Should().Be(HpdfVersion.Version12); // Default is 1.2

            // Act
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Assert
            doc.Version.Should().Be(HpdfVersion.Version14); // Should upgrade to 1.4
        }

        [Fact]
        public void LoadFromTrueTypeFile_DoesNotDowngradePdfVersion()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            doc.Version = HpdfVersion.Version16; // Set to 1.6

            // Act
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Assert
            doc.Version.Should().Be(HpdfVersion.Version16); // Should stay at 1.6
        }

        [Fact]
        public void LoadFromTrueTypeFile_NullDocument_ThrowsException()
        {
            // Act
            Action act = () => HpdfCIDFont.LoadFromTrueTypeFile(
                null,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("document");
        }

        [Fact]
        public void LoadFromTrueTypeFile_InvalidFilePath_ThrowsException()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            Action act = () => HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                "nonexistent.ttf",
                JapaneseCodePage);

            // Assert
            act.Should().Throw<HpdfException>()
                .Where(e => e.ErrorCode == HpdfErrorCode.FileNotFound);
        }

        [Theory]
        [InlineData(932)]  // Japanese (Shift-JIS)
        [InlineData(936)]  // Simplified Chinese (GBK)
        [InlineData(949)]  // Korean (EUC-KR)
        [InlineData(950)]  // Traditional Chinese (Big5)
        public void LoadFromTrueTypeFile_ValidCodePages_Succeeds(int codePage)
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();

            // Act
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                codePage);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.CodePage.Should().Be(codePage);
        }

        [Theory]
        [InlineData(1252)]  // Windows Latin
        [InlineData(1251)]  // Cyrillic
        [InlineData(850)]   // DOS Latin
        public void LoadFromTrueTypeFile_InvalidCodePages_ThrowsException(int invalidCodePage)
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            Action act = () => HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                invalidCodePage);

            // Assert
            act.Should().Throw<HpdfException>()
                .Where(e => e.ErrorCode == HpdfErrorCode.InvalidParameter);
        }

        [Fact]
        public void AsFont_ReturnsHpdfFontWrapper()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Act
            var font = cidFont.AsFont();

            // Assert
            font.Should().NotBeNull();
            font.Should().BeOfType<HpdfFont>();
            font.IsCIDFont.Should().BeTrue();
            font.LocalName.Should().Be("TestFont");
            font.BaseFont.Should().Be(cidFont.BaseFont);
            font.EncodingCodePage.Should().Be(JapaneseCodePage);
        }

        [Fact]
        public void ConvertTextToGlyphIDs_ValidText_ReturnsGlyphBytes()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Act
            var glyphBytes = cidFont.ConvertTextToGlyphIDs("Hello");

            // Assert
            glyphBytes.Should().NotBeNull();
            glyphBytes.Length.Should().Be(10); // 5 chars × 2 bytes per glyph ID
        }

        [Fact]
        public void ConvertTextToGlyphIDs_EmptyString_ReturnsEmptyArray()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Act
            var glyphBytes = cidFont.ConvertTextToGlyphIDs("");

            // Assert
            glyphBytes.Should().NotBeNull();
            glyphBytes.Length.Should().Be(0);
        }

        [Fact]
        public void ConvertTextToGlyphIDs_NullString_ReturnsEmptyArray()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Act
            var glyphBytes = cidFont.ConvertTextToGlyphIDs(null);

            // Assert
            glyphBytes.Should().NotBeNull();
            glyphBytes.Length.Should().Be(0);
        }

        [Fact]
        public void HpdfFont_IsCIDFont_ReturnsTrue()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Act
            var font = cidFont.AsFont();

            // Assert
            font.IsCIDFont.Should().BeTrue();
        }

        [Fact]
        public void HpdfFont_ConvertTextToGlyphIDs_DelegatesToCIDFont()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);
            var font = cidFont.AsFont();

            // Act
            var glyphBytes = font.ConvertTextToGlyphIDs("Test");

            // Assert
            glyphBytes.Should().NotBeNull();
            glyphBytes.Length.Should().Be(8); // 4 chars × 2 bytes
        }

        [Fact]
        public void GetGlyphId_BasicAscii_ReturnsNonZero()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Act - Get glyph ID for 'A' (U+0041)
            var glyphId = cidFont.GetGlyphId(0x0041);

            // Assert
            glyphId.Should().BeGreaterThan((ushort)0); // Should not be .notdef
        }

        [Fact]
        public void MultipleLoadsSamePage_AllFontsWork()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();

            // Act - Load multiple CID fonts
            var font1 = HpdfCIDFont.LoadFromTrueTypeFile(
                doc, "Font1", TestFontPath, 932);
            var font2 = HpdfCIDFont.LoadFromTrueTypeFile(
                doc, "Font2", TestFontPath, 936);

            // Assert
            font1.Should().NotBeNull();
            font2.Should().NotBeNull();
            font1.LocalName.Should().NotBe(font2.LocalName);
            doc.Version.Should().Be(HpdfVersion.Version14); // Upgraded once
        }

        [Fact]
        public void CIDFont_HasCorrectDictStructure()
        {
            // Skip if font file not available
            if (!TestFontExists())
            {
                return; // Skip test
            }

            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFont.LoadFromTrueTypeFile(
                doc,
                "TestFont",
                TestFontPath,
                JapaneseCodePage);

            // Act
            var dict = cidFont.Dict;

            // Assert
            dict.Should().NotBeNull();
            dict.Should().ContainKey("Type");
            dict.Should().ContainKey("Subtype");
            dict.Should().ContainKey("BaseFont");
            dict.Should().ContainKey("Encoding");
            dict.Should().ContainKey("DescendantFonts");
            dict.Should().ContainKey("ToUnicode");
        }
    }
}
