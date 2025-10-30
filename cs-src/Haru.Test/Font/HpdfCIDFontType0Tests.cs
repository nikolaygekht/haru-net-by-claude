using System;
using System.IO;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Font;
using Haru.Font.CID;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Font
{
    /// <summary>
    /// Unit tests for CIDFontType0 (predefined CJK fonts without embedding).
    /// Tests all 11 fonts: Chinese (SimSun, SimHei, MingLiU), Japanese (MS-Gothic, MS-Mincho, MS-PGothic, MS-PMincho),
    /// Korean (DotumChe, BatangChe, Dotum, Batang).
    /// </summary>
    public class HpdfCIDFontType0Tests
    {
        #region Chinese Simplified Fonts Tests

        [Fact]
        public void CreateSimSun_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("SimSun");
            cidFont.LocalName.Should().Be("F1");
            cidFont.CodePage.Should().Be(936);
            cidFont.Ascent.Should().Be(859);
            cidFont.Descent.Should().Be(-140);
        }

        [Fact]
        public void CreateSimSun_WithEncoder_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new GBKEucHEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("SimSun");
            encoder.Name.Should().Be("GBK-EUC-H");
            encoder.CodePage.Should().Be(936);
            encoder.Registry.Should().Be("Adobe");
            encoder.Ordering.Should().Be("GB1");
        }

        [Fact]
        public void CreateSimHei_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "SimHei", "F2", 936, "GBK-EUC-H");

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("SimHei");
            cidFont.XHeight.Should().Be(769);
        }

        [Fact]
        public void SimSun_ConvertChineseText_ReturnsGBKBytes()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new GBKEucHEncoder();
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H", encoder);

            // Act
            var bytes = cidFont.ConvertTextToBytes("你好世界");

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().BeGreaterThan(0);
        }

        #endregion

        #region Chinese Traditional Fonts Tests

        [Fact]
        public void CreateMingLiU_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new ETenB5HEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "MingLiU", "F3", 950, "ETen-B5-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("MingLiU");
            cidFont.CodePage.Should().Be(950);
            encoder.Name.Should().Be("ETen-B5-H");
            encoder.Ordering.Should().Be("CNS1");
        }

        [Fact]
        public void MingLiU_ConvertChineseText_ReturnsBig5Bytes()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new ETenB5HEncoder();
            var cidFont = HpdfCIDFontType0.Create(doc, "MingLiU", "F3", 950, "ETen-B5-H", encoder);

            // Act
            var bytes = cidFont.ConvertTextToBytes("你好世界");

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().BeGreaterThan(0);
        }

        #endregion

        #region Japanese Fixed-Width Fonts Tests

        [Fact]
        public void CreateMSGothic_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new Ms90RKSJHEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "MS-Gothic", "F4", 932, "90ms-RKSJ-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("MS-Gothic");
            cidFont.CodePage.Should().Be(932);
            cidFont.Ascent.Should().Be(859);
            encoder.Name.Should().Be("90ms-RKSJ-H");
            encoder.Ordering.Should().Be("Japan1");
        }

        [Fact]
        public void CreateMSMincho_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new Ms90RKSJHEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "MS-Mincho", "F5", 932, "90ms-RKSJ-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("MS-Mincho");
            cidFont.XHeight.Should().Be(769);
        }

        [Fact]
        public void MSGothic_ConvertJapaneseText_ReturnsShiftJISBytes()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new Ms90RKSJHEncoder();
            var cidFont = HpdfCIDFontType0.Create(doc, "MS-Gothic", "F4", 932, "90ms-RKSJ-H", encoder);

            // Act
            var bytes = cidFont.ConvertTextToBytes("こんにちは世界");

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().BeGreaterThan(0);
        }

        #endregion

        #region Japanese Proportional Fonts Tests

        [Fact]
        public void CreateMSPGothic_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new Ms90RKSJHEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "MS-PGothic", "F6", 932, "90ms-RKSJ-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("MS-PGothic");
            cidFont.XHeight.Should().Be(679);
        }

        [Fact]
        public void CreateMSPMincho_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new Ms90RKSJHEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "MS-PMincho", "F7", 932, "90ms-RKSJ-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("MS-PMincho");
            cidFont.XHeight.Should().Be(679);
        }

        [Fact]
        public void MSPGothic_HasVariableWidths()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new Ms90RKSJHEncoder();
            var cidFont = HpdfCIDFontType0.Create(doc, "MS-PGothic", "F6", 932, "90ms-RKSJ-H", encoder);

            // Act - Get widths for different CIDs
            var width1 = cidFont.GetCIDWidth(1);
            var width2 = cidFont.GetCIDWidth(65);

            // Assert - Proportional font should have different widths
            width1.Should().NotBe(width2);
        }

        #endregion

        #region Korean Fixed-Width Fonts Tests

        [Fact]
        public void CreateDotumChe_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new KSCmsUHCHEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "DotumChe", "F8", 949, "KSCms-UHC-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("DotumChe");
            cidFont.CodePage.Should().Be(949);
            cidFont.Ascent.Should().Be(858);
            encoder.Name.Should().Be("KSCms-UHC-H");
            encoder.Ordering.Should().Be("Korea1");
        }

        [Fact]
        public void CreateBatangChe_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new KSCmsUHCHEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "BatangChe", "F9", 949, "KSCms-UHC-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("BatangChe");
            cidFont.XHeight.Should().Be(769);
        }

        [Fact]
        public void DotumChe_ConvertKoreanText_ReturnsUHCBytes()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new KSCmsUHCHEncoder();
            var cidFont = HpdfCIDFontType0.Create(doc, "DotumChe", "F8", 949, "KSCms-UHC-H", encoder);

            // Act
            var bytes = cidFont.ConvertTextToBytes("안녕하세요 세계");

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().BeGreaterThan(0);
        }

        #endregion

        #region Korean Proportional Fonts Tests

        [Fact]
        public void CreateDotum_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new KSCmsUHCHEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "Dotum", "F10", 949, "KSCms-UHC-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("Dotum");
            cidFont.XHeight.Should().Be(679);
        }

        [Fact]
        public void CreateBatang_ValidDocument_LoadsSuccessfully()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new KSCmsUHCHEncoder();

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "Batang", "F11", 949, "KSCms-UHC-H", encoder);

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("Batang");
            cidFont.XHeight.Should().Be(679);
        }

        [Fact]
        public void Dotum_HasVariableWidths()
        {
            // Arrange
            var doc = new HpdfDocument();
            var encoder = new KSCmsUHCHEncoder();
            var cidFont = HpdfCIDFontType0.Create(doc, "Dotum", "F10", 949, "KSCms-UHC-H", encoder);

            // Act - Get widths for different CIDs
            var width1 = cidFont.GetCIDWidth(1);
            var width34 = cidFont.GetCIDWidth(34);

            // Assert - Proportional font should have different widths
            width1.Should().NotBe(width34);
        }

        #endregion

        #region Common Tests

        [Fact]
        public void CIDFontType0_AutoUpgradesPdfVersion()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Version.Should().Be(HpdfVersion.Version12); // Default is 1.2

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");

            // Assert
            doc.Version.Should().Be(HpdfVersion.Version14); // CIDFontType0 requires 1.4+
        }

        [Fact]
        public void CIDFontType0_DoesNotDowngradePdfVersion()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Version = HpdfVersion.Version16;

            // Act
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");

            // Assert
            doc.Version.Should().Be(HpdfVersion.Version16); // Should stay at 1.6
        }

        [Fact]
        public void CIDFontType0_NullDocument_ThrowsException()
        {
            // Act
            Action act = () => HpdfCIDFontType0.Create(null!, "SimSun", "F1", 936, "GBK-EUC-H");

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("document");
        }

        [Fact]
        public void CIDFontType0_InvalidFontName_ThrowsException()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            Action act = () => HpdfCIDFontType0.Create(doc, "NonExistentFont", "F1", 936, "GBK-EUC-H");

            // Assert
            act.Should().Throw<Exception>(); // PredefinedFontRegistry will throw
        }

        [Fact]
        public void AsFont_ReturnsHpdfFontWrapper()
        {
            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");

            // Act
            var font = cidFont.AsFont();

            // Assert
            font.Should().NotBeNull();
            font.Should().BeOfType<HpdfFont>();
            font.LocalName.Should().Be("F1");
            font.BaseFont.Should().Be("SimSun");
        }

        [Fact]
        public void ConvertTextToBytes_EmptyString_ReturnsEmptyArray()
        {
            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");

            // Act
            var bytes = cidFont.ConvertTextToBytes("");

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().Be(0);
        }

        [Fact]
        public void ConvertTextToBytes_NullString_ReturnsEmptyArray()
        {
            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");

            // Act
            var bytes = cidFont.ConvertTextToBytes(null!);

            // Assert
            bytes.Should().NotBeNull();
            bytes.Length.Should().Be(0);
        }

        [Fact]
        public void MeasureText_ValidText_ReturnsPositiveWidth()
        {
            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");

            // Act
            var width = cidFont.MeasureText("Hello", 12f);

            // Assert
            width.Should().BeGreaterThan(0);
        }

        [Fact]
        public void MeasureText_EmptyString_ReturnsZero()
        {
            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");

            // Act
            var width = cidFont.MeasureText("", 12f);

            // Assert
            width.Should().Be(0);
        }

        [Fact]
        public void CIDFontType0_HasCorrectDictStructure()
        {
            // Arrange
            var doc = new HpdfDocument();
            var cidFont = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");

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

        [Fact]
        public void MultipleCIDFonts_SamePage_AllWork()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act - Load multiple CID fonts
            var simSun = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H");
            var mingLiU = HpdfCIDFontType0.Create(doc, "MingLiU", "F2", 950, "ETen-B5-H");
            var msGothic = HpdfCIDFontType0.Create(doc, "MS-Gothic", "F3", 932, "90ms-RKSJ-H");
            var dotumChe = HpdfCIDFontType0.Create(doc, "DotumChe", "F4", 949, "KSCms-UHC-H");

            // Assert
            simSun.Should().NotBeNull();
            mingLiU.Should().NotBeNull();
            msGothic.Should().NotBeNull();
            dotumChe.Should().NotBeNull();
            simSun.LocalName.Should().NotBe(mingLiU.LocalName);
            doc.Version.Should().Be(HpdfVersion.Version14); // Upgraded once
        }

        [Fact]
        public void CreateSimSun_UsingCreateSimSunMethod_Works()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            var cidFont = HpdfCIDFontType0.CreateSimSun(doc, "F1");

            // Assert
            cidFont.Should().NotBeNull();
            cidFont.BaseFont.Should().Be("SimSun");
            cidFont.LocalName.Should().Be("F1");
            cidFont.CodePage.Should().Be(936);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void CreatePDFWithAllCJKFonts_GeneratesValidPDF()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            page.SetSize(HpdfPageSize.A4, HpdfPageDirection.Portrait);

            // Act - Create all 11 fonts
            var simSun = HpdfCIDFontType0.Create(doc, "SimSun", "F1", 936, "GBK-EUC-H", new GBKEucHEncoder()).AsFont();
            var simHei = HpdfCIDFontType0.Create(doc, "SimHei", "F2", 936, "GBK-EUC-H", new GBKEucHEncoder()).AsFont();
            var mingLiU = HpdfCIDFontType0.Create(doc, "MingLiU", "F3", 950, "ETen-B5-H", new ETenB5HEncoder()).AsFont();
            var msGothic = HpdfCIDFontType0.Create(doc, "MS-Gothic", "F4", 932, "90ms-RKSJ-H", new Ms90RKSJHEncoder()).AsFont();
            var msMincho = HpdfCIDFontType0.Create(doc, "MS-Mincho", "F5", 932, "90ms-RKSJ-H", new Ms90RKSJHEncoder()).AsFont();
            var msPGothic = HpdfCIDFontType0.Create(doc, "MS-PGothic", "F6", 932, "90ms-RKSJ-H", new Ms90RKSJHEncoder()).AsFont();
            var msPMincho = HpdfCIDFontType0.Create(doc, "MS-PMincho", "F7", 932, "90ms-RKSJ-H", new Ms90RKSJHEncoder()).AsFont();
            var dotumChe = HpdfCIDFontType0.Create(doc, "DotumChe", "F8", 949, "KSCms-UHC-H", new KSCmsUHCHEncoder()).AsFont();
            var batangChe = HpdfCIDFontType0.Create(doc, "BatangChe", "F9", 949, "KSCms-UHC-H", new KSCmsUHCHEncoder()).AsFont();
            var dotum = HpdfCIDFontType0.Create(doc, "Dotum", "F10", 949, "KSCms-UHC-H", new KSCmsUHCHEncoder()).AsFont();
            var batang = HpdfCIDFontType0.Create(doc, "Batang", "F11", 949, "KSCms-UHC-H", new KSCmsUHCHEncoder()).AsFont();

            // Render text with each font
            page.BeginText();
            page.SetFontAndSize(simSun, 12);
            page.MoveTextPos(50, 700);
            page.ShowText("你好世界");
            page.EndText();

            // Save to memory stream
            using var stream = new MemoryStream();
            doc.Save(stream);

            // Assert
            stream.Length.Should().BeGreaterThan(0);
            doc.Version.Should().Be(HpdfVersion.Version14);
        }

        #endregion
    }
}
