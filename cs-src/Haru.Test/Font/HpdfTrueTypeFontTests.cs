/*
 * << Haru Free PDF Library >> -- HpdfTrueTypeFontTests.cs
 *
 * Test suite for TrueType font implementation
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

using System.IO;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Font;

namespace Haru.Test.Font
{
    public class HpdfTrueTypeFontTests
    {
        private const string TestFontPath = "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf";

        private bool IsFontAvailable()
        {
            return File.Exists(TestFontPath);
        }

        [Fact]
        public void TrueTypeFont_LoadFromFile_ShouldSucceed()
        {
            if (!IsFontAvailable())
            {
                // Skip test if font not available
                return;
            }

            // Arrange
            var doc = new HpdfDocument();

            // Act
            var font = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "F1",
                TestFontPath,
                embedding: false);

            // Assert
            font.Should().NotBeNull();
            font.LocalName.Should().Be("F1");
            font.Dict.Should().NotBeNull();
            font.Descriptor.Should().NotBeNull();
        }

        [Fact]
        public void TrueTypeFont_WithEmbedding_ShouldCreateFontFile2Entry()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();

            // Act
            var font = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "F1",
                TestFontPath,
                embedding: true);

            // Assert
            font.Descriptor.Should().ContainKey("FontFile2");
        }

        [Fact]
        public void TrueTypeFont_ShouldHaveToUnicodeCMap()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();

            // Act
            var font = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "F1",
                TestFontPath,
                embedding: false);

            // Assert
            font.Dict.Should().ContainKey("ToUnicode");
        }

        [Fact]
        public void TrueTypeFont_ShouldHaveCorrectType()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();

            // Act
            var font = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "F1",
                TestFontPath,
                embedding: false);

            // Assert
            var typeObj = font.Dict["Type"];
            typeObj.Should().NotBeNull();
        }

        [Fact]
        public void TrueTypeFont_ShouldHaveWidthsArray()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();

            // Act
            var font = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "F1",
                TestFontPath,
                embedding: false);

            // Assert
            font.Dict.Should().ContainKey("Widths");
            font.Dict.Should().ContainKey("FirstChar");
            font.Dict.Should().ContainKey("LastChar");
        }

        [Fact]
        public void TrueTypeFont_FontDescriptor_ShouldHaveRequiredEntries()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();

            // Act
            var font = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "F1",
                TestFontPath,
                embedding: false);

            // Assert
            var descriptor = font.Descriptor;
            descriptor.Should().ContainKey("Type");
            descriptor.Should().ContainKey("FontName");
            descriptor.Should().ContainKey("Flags");
            descriptor.Should().ContainKey("FontBBox");
            descriptor.Should().ContainKey("ItalicAngle");
            descriptor.Should().ContainKey("Ascent");
            descriptor.Should().ContainKey("Descent");
            descriptor.Should().ContainKey("CapHeight");
            descriptor.Should().ContainKey("StemV");
        }

        [Fact]
        public void TrueTypeFont_MeasureText_ShouldReturnNonZeroWidth()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();
            var font = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "F1",
                TestFontPath,
                embedding: false);

            // Act
            float width = font.MeasureText("Hello", 12);

            // Assert
            width.Should().BeGreaterThan(0);
        }

        [Fact]
        public void TrueTypeFont_GetCharWidth_ShouldReturnNonZeroForPrintableChars()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();
            var font = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "F1",
                TestFontPath,
                embedding: false);

            // Act
            float width = font.GetCharWidth((byte)'A');

            // Assert
            width.Should().BeGreaterThan(0);
        }

        [Fact]
        public void TrueTypeFont_InDocument_ShouldRenderText()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);

            var font = HpdfTrueTypeFont.LoadFromFile(
                doc.Xref,
                "F1",
                TestFontPath,
                embedding: false);

            // Act
            page.BeginText();
            page.MoveTextPos(100, 700);
            page.ShowText("TrueType Font Test");
            page.EndText();

            // Assert
            var outputPath = Path.GetTempFileName() + ".pdf";
            try
            {
                doc.SaveToFile(outputPath);
                File.Exists(outputPath).Should().BeTrue();
                var fileInfo = new FileInfo(outputPath);
                fileInfo.Length.Should().BeGreaterThan(0);
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void TrueTypeFont_WithEmbedding_ShouldProduceLargerPDF()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Create two PDFs: one with embedding, one without
            var doc1 = new HpdfDocument();
            var page1 = doc1.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
            var font1 = HpdfTrueTypeFont.LoadFromFile(doc1.Xref, "F1", TestFontPath, embedding: false);
            page1.BeginText();
            page1.MoveTextPos(100, 700);
            page1.ShowText("Test");
            page1.EndText();

            var doc2 = new HpdfDocument();
            var page2 = doc2.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
            var font2 = HpdfTrueTypeFont.LoadFromFile(doc2.Xref, "F1", TestFontPath, embedding: true);
            page2.BeginText();
            page2.MoveTextPos(100, 700);
            page2.ShowText("Test");
            page2.EndText();

            var path1 = Path.GetTempFileName() + ".pdf";
            var path2 = Path.GetTempFileName() + ".pdf";

            try
            {
                doc1.SaveToFile(path1);
                doc2.SaveToFile(path2);

                var size1 = new FileInfo(path1).Length;
                var size2 = new FileInfo(path2).Length;

                // PDF with embedded font should be larger
                size2.Should().BeGreaterThan(size1);
            }
            finally
            {
                if (File.Exists(path1)) File.Delete(path1);
                if (File.Exists(path2)) File.Delete(path2);
            }
        }

        [Fact]
        public void TrueTypeFont_MultipleTexts_ShouldRenderCorrectly()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
            var font = HpdfTrueTypeFont.LoadFromFile(doc.Xref, "F1", TestFontPath, embedding: true);

            // Act
            page.BeginText();
            page.MoveTextPos(100, 700);
            page.ShowText("Line 1: TrueType Font");
            page.MoveTextPos(0, -20);
            page.ShowText("Line 2: With Embedding");
            page.MoveTextPos(0, -20);
            page.ShowText("Line 3: Unicode Support");
            page.EndText();

            // Assert
            var outputPath = Path.GetTempFileName() + ".pdf";
            try
            {
                doc.SaveToFile(outputPath);
                File.Exists(outputPath).Should().BeTrue();
            }
            finally
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Fact]
        public void TrueTypeFont_InvalidPath_ShouldThrowException()
        {
            // Arrange
            var doc = new HpdfDocument();
            var invalidPath = "/nonexistent/font.ttf";

            // Act & Assert
            Assert.Throws<HpdfException>(() =>
            {
                HpdfTrueTypeFont.LoadFromFile(doc.Xref, "F1", invalidPath, embedding: false);
            });
        }

        [Fact]
        public void TrueTypeFont_GetGlyphId_ShouldReturnValidId()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();
            var font = HpdfTrueTypeFont.LoadFromFile(doc.Xref, "F1", TestFontPath, embedding: false);

            // Act
            ushort glyphId = font.GetGlyphId((ushort)'A');

            // Assert
            glyphId.Should().BeGreaterThan(0); // 'A' should not map to .notdef (0)
        }

        [Fact]
        public void TrueTypeFont_GetGlyphWidth_ShouldReturnPositiveWidth()
        {
            if (!IsFontAvailable())
            {
                return;
            }

            // Arrange
            var doc = new HpdfDocument();
            var font = HpdfTrueTypeFont.LoadFromFile(doc.Xref, "F1", TestFontPath, embedding: false);
            ushort glyphId = font.GetGlyphId((ushort)'M');

            // Act
            int width = font.GetGlyphWidth(glyphId);

            // Assert
            width.Should().BeGreaterThan(0);
        }
    }
}
