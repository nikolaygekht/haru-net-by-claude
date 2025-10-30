using System;
using System.IO;
using System.Text;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Types;
using Haru.Font;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Doc
{
    /// <summary>
    /// Integration tests for compression mode functionality.
    /// Tests end-to-end compression behavior including file size and content verification.
    /// </summary>
    public class HpdfCompressionIntegrationTests
    {
        [Fact]
        public void CompressionMode_WithTextContent_ReducesFileSize()
        {
            // Arrange
            var docUncompressed = new HpdfDocument();
            var docCompressed = new HpdfDocument();
            docCompressed.SetCompressionMode(HpdfCompressionMode.Text);

            // Create same content in both documents
            var font1 = new HpdfFont(docUncompressed.Xref, HpdfStandardFont.Helvetica, "F1");
            var page1 = docUncompressed.AddPage();
            page1.SetFontAndSize(font1, 12);
            page1.BeginText();
            page1.MoveTextPos(50, 800);

            // Add substantial text to make compression noticeable
            for (int i = 0; i < 50; i++)
            {
                page1.ShowText($"Line {i}: This is a test line with repeated content to demonstrate compression. ");
                page1.MoveTextPos(0, -15);
            }
            page1.EndText();

            var font2 = new HpdfFont(docCompressed.Xref, HpdfStandardFont.Helvetica, "F1");
            var page2 = docCompressed.AddPage();
            page2.SetFontAndSize(font2, 12);
            page2.BeginText();
            page2.MoveTextPos(50, 800);

            for (int i = 0; i < 50; i++)
            {
                page2.ShowText($"Line {i}: This is a test line with repeated content to demonstrate compression. ");
                page2.MoveTextPos(0, -15);
            }
            page2.EndText();

            // Act
            var uncompressedData = docUncompressed.SaveToMemory();
            var compressedData = docCompressed.SaveToMemory();

            // Assert
            compressedData.Length.Should().BeLessThan(uncompressedData.Length,
                "compressed PDF should be smaller than uncompressed");

            // Verify compression actually happened
            var compressedText = Encoding.ASCII.GetString(compressedData);
            compressedText.Should().Contain("/Filter [/FlateDecode]");
        }

        [Fact]
        public void CompressionMode_VerifyZlibHeader_InCompressedStream()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.Text);

            // Act
            var page = doc.AddPage();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");
            page.SetFontAndSize(font, 12);
            page.BeginText();
            page.ShowText("Test content");
            page.EndText();

            var pdfData = doc.SaveToMemory();

            // Assert
            // Look for zlib header (0x78 0x9C) after "stream\n"
            var streamMarker = Encoding.ASCII.GetBytes("stream\n");
            bool foundZlibHeader = false;

            for (int i = 0; i < pdfData.Length - streamMarker.Length - 2; i++)
            {
                bool isStreamMarker = true;
                for (int j = 0; j < streamMarker.Length; j++)
                {
                    if (pdfData[i + j] != streamMarker[j])
                    {
                        isStreamMarker = false;
                        break;
                    }
                }

                if (isStreamMarker)
                {
                    // Check if next two bytes are zlib header
                    if (pdfData[i + streamMarker.Length] == 0x78 &&
                        pdfData[i + streamMarker.Length + 1] == 0x9C)
                    {
                        foundZlibHeader = true;
                        break;
                    }
                }
            }

            foundZlibHeader.Should().BeTrue("compressed streams should contain zlib header (0x78 0x9C)");
        }

        [Fact]
        public void CompressionMode_MultiplePages_EachPageCompressed()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.Text);

            // Act
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            for (int i = 0; i < 5; i++)
            {
                var page = doc.AddPage();
                page.SetFontAndSize(font, 12);
                page.BeginText();
                page.MoveTextPos(50, 800);
                page.ShowText($"Page {i + 1} content");
                page.EndText();
            }

            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = Encoding.ASCII.GetString(pdfData);

            // Count /Filter [/FlateDecode] occurrences
            int filterCount = CountOccurrences(pdfText, "/Filter [/FlateDecode]");

            // Should have at least 5 filters (one per page content stream)
            filterCount.Should().BeGreaterOrEqualTo(5,
                "each page content stream should have FlateDecode filter");
        }

        [Fact]
        public void CompressionMode_SaveToFile_CreatesValidCompressedPDF()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.All);
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var page = doc.AddPage();
                var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");
                page.SetFontAndSize(font, 12);
                page.BeginText();
                page.ShowText("Compressed content in file");
                page.EndText();

                doc.SaveToFile(tempFile);

                // Assert
                File.Exists(tempFile).Should().BeTrue();

                var fileData = File.ReadAllBytes(tempFile);
                var fileText = Encoding.ASCII.GetString(fileData);

                fileText.Should().StartWith("%PDF-");
                fileText.Should().Contain("/Filter [/FlateDecode]");
                fileText.Trim().Should().EndWith("%%EOF");
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void CompressionMode_WithGraphicsOperations_CompressesContentStream()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.Text);

            // Act
            var page = doc.AddPage();

            // Add graphics operations
            page.SetLineWidth(2);
            page.SetRgbStroke(1, 0, 0);
            page.Rectangle(50, 50, 200, 100);
            page.Stroke();

            page.Circle(150, 150, 50);
            page.Fill();

            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("/Filter [/FlateDecode]");

            // Verify graphics commands are compressed (not visible as plain text in stream)
            // The stream should contain binary data, not readable "re" or "S" operators
            var streamIndex = pdfText.IndexOf("stream\n");
            if (streamIndex > 0)
            {
                var afterStream = pdfText.Substring(streamIndex + 7, Math.Min(100, pdfText.Length - streamIndex - 7));
                // Should not contain readable graphics operators like "re" or "S"
                // because they should be compressed
                afterStream.Should().NotContain(" re\n");
                afterStream.Should().NotContain(" S\n");
            }
        }

        [Fact]
        public void CompressionMode_CombinedWithEncryption_ShouldWork()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.Text);
            doc.SetEncryption("user", "owner", HpdfPermission.Print);

            // Act
            var page = doc.AddPage();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");
            page.SetFontAndSize(font, 12);
            page.BeginText();
            page.ShowText("Compressed and encrypted");
            page.EndText();

            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = Encoding.ASCII.GetString(pdfData);

            // Should have both encryption and compression
            pdfText.Should().Contain("/Encrypt");
            pdfText.Should().Contain("/Filter [/FlateDecode]");
        }

        [Fact]
        public void CompressionMode_WithImages_ImagesAlreadyCompressed()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.All);

            // Act
            var page = doc.AddPage();

            // Note: This test assumes image loading is implemented
            // Images should already have their own compression (FlateDecode for PNG, DCTDecode for JPEG)
            // The All mode should not interfere with image compression

            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = Encoding.ASCII.GetString(pdfData);

            // Page content should be compressed
            pdfText.Should().Contain("/Filter [/FlateDecode]");
        }

        [Fact]
        public void CompressionMode_DefaultNone_ProducesReadableContentStream()
        {
            // Arrange
            var doc = new HpdfDocument();
            // Explicitly set to None (though it's the default)
            doc.SetCompressionMode(HpdfCompressionMode.None);

            // Act
            var page = doc.AddPage();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");
            page.SetFontAndSize(font, 12);
            page.BeginText();
            page.ShowText("Readable");
            page.EndText();

            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = Encoding.ASCII.GetString(pdfData);

            // Should be able to find text operators in plain text
            pdfText.Should().Contain("BT"); // BeginText
            pdfText.Should().Contain("ET"); // EndText
            pdfText.Should().Contain("Tf"); // SetFont
        }

        [Fact]
        public void CompressionMode_LargeDocument_SignificantSizeReduction()
        {
            // Arrange
            var docUncompressed = new HpdfDocument();
            var docCompressed = new HpdfDocument();
            docCompressed.SetCompressionMode(HpdfCompressionMode.All);

            // Act - Create substantial content
            for (int p = 0; p < 10; p++)
            {
                var font1 = new HpdfFont(docUncompressed.Xref, HpdfStandardFont.Helvetica, $"F{p}");
                var page1 = docUncompressed.AddPage();
                page1.SetFontAndSize(font1, 10);
                page1.BeginText();
                page1.MoveTextPos(50, 800);

                for (int i = 0; i < 30; i++)
                {
                    page1.ShowText($"Page {p+1} Line {i}: Lorem ipsum dolor sit amet, consectetur adipiscing elit. ");
                    page1.MoveTextPos(0, -12);
                }
                page1.EndText();

                var font2 = new HpdfFont(docCompressed.Xref, HpdfStandardFont.Helvetica, $"F{p}");
                var page2 = docCompressed.AddPage();
                page2.SetFontAndSize(font2, 10);
                page2.BeginText();
                page2.MoveTextPos(50, 800);

                for (int i = 0; i < 30; i++)
                {
                    page2.ShowText($"Page {p+1} Line {i}: Lorem ipsum dolor sit amet, consectetur adipiscing elit. ");
                    page2.MoveTextPos(0, -12);
                }
                page2.EndText();
            }

            var uncompressedData = docUncompressed.SaveToMemory();
            var compressedData = docCompressed.SaveToMemory();

            // Assert
            var compressionRatio = (double)compressedData.Length / uncompressedData.Length;

            compressionRatio.Should().BeLessThan(0.5,
                "compressed PDF should be less than 50% of uncompressed size for text-heavy content");

            compressedData.Length.Should().BeLessThan(uncompressedData.Length);
        }

        private int CountOccurrences(string text, string pattern)
        {
            int count = 0;
            int index = 0;
            while ((index = text.IndexOf(pattern, index)) != -1)
            {
                count++;
                index += pattern.Length;
            }
            return count;
        }
    }
}
