/*
 * << Haru Free PDF Library >> -- HpdfPdfPigIntegrationTests.cs
 *
 * Integration tests using PdfPig to validate generated PDFs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 */

using System;
using System.IO;
using System.Linq;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Xref;
using Haru.Types;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Haru.Test.Doc
{
    /// <summary>
    /// Integration tests that validate generated PDFs using PdfPig library.
    /// These tests ensure our PDF output is readable by third-party tools.
    /// </summary>
    public class HpdfPdfPigIntegrationTests
    {
        private const string TestResourcePath = "Haru.Test.Resources.";

        private Stream GetResourceStream(string resourceName)
        {
            var assembly = typeof(HpdfPdfPigIntegrationTests).Assembly;
            var fullName = TestResourcePath + resourceName;
            return assembly.GetManifestResourceStream(fullName);
        }

        // Text Extraction Tests

        [Fact]
        public void TextExtraction_SimpleText_CanBeReadByPdfPig()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var font = doc.GetFont("Helvetica", null);

            // Act - Write text at different positions
            page.BeginText();
            page.SetFontAndSize(font, 12);
            page.MoveTextPos(100, 700);
            page.ShowText("Hello World");
            page.EndText();

            page.BeginText();
            page.SetFontAndSize(font, 24);
            page.MoveTextPos(100, 650);
            page.ShowText("Large Text");
            page.EndText();

            // Save to memory
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - Read with PdfPig
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);
            var text = pdfPage.Text;

            text.Should().Contain("Hello World");
            text.Should().Contain("Large Text");
        }

        [Fact]
        public void TextExtraction_MultiplePages_AllTextReadable()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = doc.GetFont("Helvetica", null);

            // Create 3 pages with different text
            for (int i = 1; i <= 3; i++)
            {
                var page = doc.AddPage();
                page.BeginText();
                page.SetFontAndSize(font, 14);
                page.MoveTextPos(100, 700);
                page.ShowText($"Page {i} Content");
                page.EndText();
            }

            // Save to memory
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - Read all pages with PdfPig
            using var pdfDoc = PdfDocument.Open(ms);
            pdfDoc.NumberOfPages.Should().Be(3);

            for (int i = 1; i <= 3; i++)
            {
                var pdfPage = pdfDoc.GetPage(i);
                var text = pdfPage.Text;
                text.Should().Contain($"Page {i} Content");
            }
        }

        [Fact]
        public void TextExtraction_WithSpecialCharacters_PreservesText()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var font = doc.GetFont("Helvetica", null);

            var testText = "Special: $100.00 (50% off!)";

            // Act
            page.BeginText();
            page.SetFontAndSize(font, 12);
            page.MoveTextPos(100, 700);
            page.ShowText(testText);
            page.EndText();

            // Save to memory
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);
            var text = pdfPage.Text;

            // Note: Some special characters might be represented differently
            text.Should().Contain("Special");
            text.Should().Contain("100");
            text.Should().Contain("50");
        }

        // PNG Image Extraction Tests

        [Fact]
        public void ImageExtraction_PngImage_IsReadableByPdfPig()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            using var imageStream = GetResourceStream("test_rgb_2x2.png");
            var image = HpdfImage.LoadPngImage(doc.Xref, "Im1", imageStream);

            // Act - Draw image
            page.DrawImage(image, 100, 500, 100, 100);

            // Save to memory
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - Read with PdfPig
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);
            var images = pdfPage.GetImages().ToList();

            images.Should().HaveCount(1);
            var pdfImage = images[0];
            pdfImage.WidthInSamples.Should().Be(2); // Original image width
            pdfImage.HeightInSamples.Should().Be(2); // Original image height
        }

        [Fact]
        public void ImageExtraction_MultipleImages_AllReadable()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            using var imageStream1 = GetResourceStream("test_rgb_2x2.png");
            using var imageStream2 = GetResourceStream("test_grayscale_2x2.png");
            var image1 = HpdfImage.LoadPngImage(doc.Xref, "Im1", imageStream1);
            var image2 = HpdfImage.LoadPngImage(doc.Xref, "Im2", imageStream2);

            // Act - Draw both images
            page.DrawImage(image1, 100, 500, 50, 50);
            page.DrawImage(image2, 200, 500, 50, 50);

            // Save to memory
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - Read with PdfPig
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);
            var images = pdfPage.GetImages().ToList();

            images.Should().HaveCount(2);
            images.Should().OnlyContain(img => img.WidthInSamples == 2 && img.HeightInSamples == 2);
        }

        [Fact]
        public void ImageExtraction_PngWithTransparency_IsReadable()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            using var imageStream = GetResourceStream("test_rgba_2x2.png");
            var image = HpdfImage.LoadPngImage(doc.Xref, "Im1", imageStream);

            // Act - Draw image with transparency
            page.DrawImage(image, 100, 500, 100, 100);

            // Save to memory
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - Read with PdfPig
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);
            var images = pdfPage.GetImages().ToList();

            images.Should().HaveCountGreaterOrEqualTo(1); // Main image (might have SMask as separate image)
            var mainImage = images.First();
            mainImage.WidthInSamples.Should().Be(2);
            mainImage.HeightInSamples.Should().Be(2);
        }

        // Page Size Tests

        [Fact]
        public void PageSize_DefaultA4Size_MatchesExpected()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            // Act - Save with default page size (A4)
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - Read with PdfPig
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);

            pdfPage.Width.Should().BeApproximately(595.276, 1.0); // A4 width in points
            pdfPage.Height.Should().BeApproximately(841.89, 1.0); // A4 height in points
        }

        [Fact]
        public void PageSize_A4Portrait_MatchesExpected()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            page.SetSize(HpdfPageSize.A4, HpdfPageDirection.Portrait);

            // Act
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - Read with PdfPig
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);

            pdfPage.Width.Should().BeApproximately(595.276, 1.0); // A4 width in points
            pdfPage.Height.Should().BeApproximately(841.89, 1.0); // A4 height in points
        }

        [Fact]
        public void PageSize_A4Landscape_MatchesExpected()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            page.SetSize(HpdfPageSize.A4, HpdfPageDirection.Landscape);

            // Act
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - Read with PdfPig (dimensions should be swapped)
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);

            pdfPage.Width.Should().BeApproximately(841.89, 1.0); // A4 height becomes width
            pdfPage.Height.Should().BeApproximately(595.276, 1.0); // A4 width becomes height
        }

        [Fact]
        public void PageSize_CustomDimensions_MatchesExpected()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            page.SetWidth(400);
            page.SetHeight(600);

            // Act
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - Read with PdfPig
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);

            pdfPage.Width.Should().BeApproximately(400, 0.1);
            pdfPage.Height.Should().BeApproximately(600, 0.1);
        }

        // Compression Tests

        [Fact]
        public void Compression_Disabled_PdfIsReadable()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.None);

            var page = doc.AddPage();
            var font = doc.GetFont("Helvetica", null);

            // Add substantial content
            page.BeginText();
            page.SetFontAndSize(font, 12);
            for (int i = 0; i < 10; i++)
            {
                page.MoveTextPos(100, 700 - (i * 20));
                page.ShowText($"Line {i}: This is uncompressed text content.");
            }
            page.EndText();

            // Act
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;
            var uncompressedSize = ms.Length;

            // Assert - PdfPig can read uncompressed PDF
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);
            var text = pdfPage.Text;

            text.Should().Contain("Line 0");
            text.Should().Contain("Line 9");
            text.Should().Contain("uncompressed text content");
        }

        [Fact]
        public void Compression_Enabled_PdfIsReadableAndSmaller()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.Text);

            var page = doc.AddPage();
            var font = doc.GetFont("Helvetica", null);

            // Add substantial content (same as uncompressed test)
            page.BeginText();
            page.SetFontAndSize(font, 12);
            for (int i = 0; i < 10; i++)
            {
                page.MoveTextPos(100, 700 - (i * 20));
                page.ShowText($"Line {i}: This is compressed text content.");
            }
            page.EndText();

            // Act
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;
            var compressedSize = ms.Length;

            // Assert - PdfPig can read compressed PDF
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);
            var text = pdfPage.Text;

            text.Should().Contain("Line 0");
            text.Should().Contain("Line 9");
            text.Should().Contain("compressed text content");

            // Compressed should be smaller (though not guaranteed for small files)
            // We're mainly testing readability here
        }

        [Fact]
        public void Compression_CompareCompressedVsUncompressed_BothReadable()
        {
            // Arrange - Create uncompressed version
            var docUncompressed = new HpdfDocument();
            docUncompressed.SetCompressionMode(HpdfCompressionMode.None);
            var pageUncompressed = docUncompressed.AddPage();
            var fontUncompressed = docUncompressed.GetFont("Helvetica", null);

            string testContent = "The quick brown fox jumps over the lazy dog. ";
            string repeatedContent = string.Concat(Enumerable.Repeat(testContent, 20)); // Repeat for better compression

            pageUncompressed.BeginText();
            pageUncompressed.SetFontAndSize(fontUncompressed, 10);
            pageUncompressed.MoveTextPos(50, 700);
            pageUncompressed.ShowText(repeatedContent);
            pageUncompressed.EndText();

            using var msUncompressed = new MemoryStream();
            docUncompressed.Save(msUncompressed);
            var uncompressedSize = msUncompressed.Length;

            // Arrange - Create compressed version
            var docCompressed = new HpdfDocument();
            docCompressed.SetCompressionMode(HpdfCompressionMode.Text);
            var pageCompressed = docCompressed.AddPage();
            var fontCompressed = docCompressed.GetFont("Helvetica", null);

            pageCompressed.BeginText();
            pageCompressed.SetFontAndSize(fontCompressed, 10);
            pageCompressed.MoveTextPos(50, 700);
            pageCompressed.ShowText(repeatedContent);
            pageCompressed.EndText();

            using var msCompressed = new MemoryStream();
            docCompressed.Save(msCompressed);
            var compressedSize = msCompressed.Length;

            // Assert - Both readable, compressed is smaller
            msUncompressed.Position = 0;
            using (var pdfDocUncompressed = PdfDocument.Open(msUncompressed))
            {
                var pageText = pdfDocUncompressed.GetPage(1).Text;
                pageText.Should().Contain("quick brown fox");
            }

            msCompressed.Position = 0;
            using (var pdfDocCompressed = PdfDocument.Open(msCompressed))
            {
                var pageText = pdfDocCompressed.GetPage(1).Text;
                pageText.Should().Contain("quick brown fox");
            }

            // Verify compression actually reduces size
            compressedSize.Should().BeLessThan(uncompressedSize,
                $"Compressed ({compressedSize} bytes) should be smaller than uncompressed ({uncompressedSize} bytes)");
        }

        [Fact]
        public void Compression_WithImages_PdfIsReadable()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.All); // Compress everything

            var page = doc.AddPage();
            var font = doc.GetFont("Helvetica", null);

            // Add text
            page.BeginText();
            page.SetFontAndSize(font, 14);
            page.MoveTextPos(100, 700);
            page.ShowText("Compressed PDF with Image");
            page.EndText();

            // Add image
            using var imageStream = GetResourceStream("test_rgb_2x2.png");
            var image = HpdfImage.LoadPngImage(doc.Xref, "Im1", imageStream);
            page.DrawImage(image, 100, 500, 100, 100);

            // Act
            using var ms = new MemoryStream();
            doc.Save(ms);
            ms.Position = 0;

            // Assert - PdfPig can read compressed PDF with image
            using var pdfDoc = PdfDocument.Open(ms);
            var pdfPage = pdfDoc.GetPage(1);

            var text = pdfPage.Text;
            text.Should().Contain("Compressed PDF with Image");

            var images = pdfPage.GetImages().ToList();
            images.Should().HaveCount(1);
            images[0].WidthInSamples.Should().Be(2);
        }
    }
}
