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
    /// Unit tests for compression mode functionality.
    /// </summary>
    public class HpdfCompressionModeTests
    {
        [Fact]
        public void CompressionMode_DefaultValue_ShouldBeNone()
        {
            // Arrange & Act
            var doc = new HpdfDocument();

            // Assert
            doc.CompressionMode.Should().Be(HpdfCompressionMode.None);
        }

        [Fact]
        public void SetCompressionMode_UsingExtensionMethod_ShouldUpdateProperty()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.SetCompressionMode(HpdfCompressionMode.All);

            // Assert
            doc.CompressionMode.Should().Be(HpdfCompressionMode.All);
        }

        [Fact]
        public void SetCompressionMode_UsingProperty_ShouldUpdateValue()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.CompressionMode = HpdfCompressionMode.Text;

            // Assert
            doc.CompressionMode.Should().Be(HpdfCompressionMode.Text);
        }

        [Fact]
        public void SetCompressionMode_WithTextFlag_ShouldCompressPageContent()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.Text);

            // Act
            var page = doc.AddPage();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");
            page.SetFontAndSize(font, 12);
            page.BeginText();
            page.MoveTextPos(50, 800);
            page.ShowText("Compressed text content");
            page.EndText();

            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("/Filter [/FlateDecode]");
        }

        [Fact]
        public void SetCompressionMode_WithNone_ShouldNotCompressPageContent()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.None);

            // Act
            var page = doc.AddPage();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");
            page.SetFontAndSize(font, 12);
            page.BeginText();
            page.MoveTextPos(50, 800);
            page.ShowText("Uncompressed text content");
            page.EndText();

            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = Encoding.ASCII.GetString(pdfData);

            // Should not contain any FlateDecode filters when compression is disabled
            pdfText.Should().NotContain("/Filter [/FlateDecode]");
        }

        [Fact]
        public void SetCompressionMode_WithAllFlag_ShouldSetTextCompression()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.SetCompressionMode(HpdfCompressionMode.All);

            // Assert
            doc.CompressionMode.Should().HaveFlag(HpdfCompressionMode.Text);
            doc.CompressionMode.Should().HaveFlag(HpdfCompressionMode.Image);
            doc.CompressionMode.Should().HaveFlag(HpdfCompressionMode.Metadata);
        }

        [Fact]
        public void SetCompressionMode_AppliedToMultiplePages_ShouldCompressAllPages()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetCompressionMode(HpdfCompressionMode.Text);

            // Act
            doc.AddPage();
            doc.AddPage();
            doc.AddPage();

            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = Encoding.ASCII.GetString(pdfData);

            // Count FlateDecode occurrences (should be at least 3 for page content)
            int filterCount = 0;
            int index = 0;
            while ((index = pdfText.IndexOf("/Filter [/FlateDecode]", index)) != -1)
            {
                filterCount++;
                index++;
            }

            filterCount.Should().BeGreaterOrEqualTo(3);
        }

        [Fact]
        public void SetCompressionMode_AfterPagesCreated_DoesNotAffectExistingPages()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            var page1 = doc.AddPage(); // Created without compression
            doc.SetCompressionMode(HpdfCompressionMode.Text);
            var page2 = doc.AddPage(); // Created with compression

            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Add text to both pages
            page1.SetFontAndSize(font, 12);
            page1.BeginText();
            page1.ShowText("Page 1");
            page1.EndText();

            page2.SetFontAndSize(font, 12);
            page2.BeginText();
            page2.ShowText("Page 2");
            page2.EndText();

            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = Encoding.ASCII.GetString(pdfData);

            // Should have at least one compressed stream (page2)
            pdfText.Should().Contain("/Filter [/FlateDecode]");
        }

        [Fact]
        public void SetCompressionMode_WithUIntParameter_ShouldWork()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.SetCompressionMode((uint)HpdfCompressionMode.All);

            // Assert
            doc.CompressionMode.Should().Be(HpdfCompressionMode.All);
        }

        [Fact]
        public void SetCompressionMode_WithCombinedFlags_ShouldWork()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.SetCompressionMode(HpdfCompressionMode.Text | HpdfCompressionMode.Image);

            // Assert
            doc.CompressionMode.Should().HaveFlag(HpdfCompressionMode.Text);
            doc.CompressionMode.Should().HaveFlag(HpdfCompressionMode.Image);
            doc.CompressionMode.Should().NotHaveFlag(HpdfCompressionMode.Metadata);
        }
    }
}
