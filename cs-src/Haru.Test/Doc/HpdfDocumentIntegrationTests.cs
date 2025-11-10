using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using HpdfPageMode = Haru.Types.HpdfPageMode;


namespace Haru.Test.Doc
{
    public class HpdfDocumentIntegrationTests
    {
        [Fact]
        public void CreateDocument_WithSinglePage_GeneratesValidPDF()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNullOrEmpty();
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().StartWith("%PDF-1.2");
            pdfText.Should().Contain("%%EOF");
        }

        [Fact]
        public void CreateDocument_HasCorrectVersion()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Version = HpdfVersion.Version14;

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().StartWith("%PDF-1.4");
        }

        [Fact]
        public void CreateDocument_WithMultiplePages_TracksPageCount()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage();
            doc.AddPage();
            doc.AddPage();

            // Assert
            doc.PageCount.Should().Be(3);
            doc.Pages.Should().HaveCount(3);
        }

        [Fact]
        public void CreateDocument_ContainsCatalogObject()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("/Type /Catalog");
            pdfText.Should().Contain("/Pages");
        }

        [Fact]
        public void CreateDocument_ContainsPagesObject()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("/Type /Pages");
            pdfText.Should().Contain("/Kids");
            pdfText.Should().Contain("/Count");
        }

        [Fact]
        public void CreateDocument_ContainsPageObject()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("/Type /Page");
            pdfText.Should().Contain("/MediaBox");
            pdfText.Should().Contain("/Contents");
            pdfText.Should().Contain("/Resources");
        }

        [Fact]
        public void CreateDocument_HasValidXrefTable()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("xref");
            pdfText.Should().MatchRegex(@"\d{10} \d{5} [fn] ");
        }

        [Fact]
        public void CreateDocument_HasValidTrailer()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("trailer");
            pdfText.Should().Contain("/Root");
            pdfText.Should().Contain("startxref");
        }

        [Fact]
        public void CreateDocument_WithCustomPageSize_SetsCorrectMediaBox()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("/MediaBox");
            pdfText.Should().MatchRegex(@"/MediaBox\s*\[\s*0\s+0\s+595\.27\d*\s+841\.89");
        }

        [Fact]
        public void CreateDocument_WithLandscapePage_SwapsDimensions()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            var page = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Landscape);
            var pdfData = doc.SaveToMemory();

            // Assert
            page.Width.Should().BeApproximately(841.89f, 0.01f);
            page.Height.Should().BeApproximately(595.276f, 0.01f);
        }

        [Fact]
        public void CreateDocument_SaveToFile_CreatesFile()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.AddPage();
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                doc.SaveToFile(tempFile);

                // Assert
                File.Exists(tempFile).Should().BeTrue();
                var fileInfo = new FileInfo(tempFile);
                fileInfo.Length.Should().BeGreaterThan(0);

                var pdfData = File.ReadAllBytes(tempFile);
                var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
                pdfText.Should().StartWith("%PDF-");
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void CreateDocument_WithPageTree_GeneratesCorrectStructure()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            // Add pages directly to root
            doc.AddPage();
            doc.AddPage();

            // Create intermediate node
            var subPages = doc.InsertPagesNode();
            doc.AddPage();
            doc.AddPage();

            var pdfData = doc.SaveToMemory();

            // Assert
            doc.PageCount.Should().Be(4);
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);

            // Should have multiple Pages objects
            var pagesMatches = Regex.Matches(pdfText, @"/Type /Pages");
            pagesMatches.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public void CreateDocument_CatalogHasPageLayout()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Catalog.PageLayout = HpdfPageLayout.TwoColumnLeft;

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("/PageLayout /TwoColumnLeft");
        }

        [Fact]
        public void CreateDocument_CatalogHasPageMode()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Catalog.PageMode = HpdfPageMode.UseOutlines;

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("/PageMode /UseOutlines");
        }

        [Fact]
        public void CreateDocument_WithTenPages_AllPagesIncluded()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            for (int i = 0; i < 10; i++)
            {
                doc.AddPage();
            }
            var pdfData = doc.SaveToMemory();

            // Assert
            doc.PageCount.Should().Be(10);
            doc.RootPages.Count.Should().Be(10);

            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            var pageMatches = Regex.Matches(pdfText, @"/Type /Page\b");
            pageMatches.Count.Should().Be(10);
        }

        [Fact]
        public void CreateDocument_CurrentPage_TracksLastAddedPage()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            var page1 = doc.AddPage();
            var page2 = doc.AddPage();
            var page3 = doc.AddPage();

            // Assert
            doc.CurrentPage.Should().BeSameAs(page3);
        }

        [Fact]
        public void CreateDocument_RootPages_ContainsAllPages()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage();
            doc.AddPage();
            doc.AddPage();

            // Assert
            var allPages = doc.RootPages.GetAllPages();
            allPages.Should().HaveCount(3);
            allPages.Should().BeEquivalentTo(doc.Pages);
        }

        [Fact]
        public void CreateDocument_WithCustomSizedPage_SetsCorrectDimensions()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            var page = doc.AddPage(300, 400);
            var pdfData = doc.SaveToMemory();

            // Assert
            page.Width.Should().Be(300);
            page.Height.Should().Be(400);

            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);
            pdfText.Should().Contain("/MediaBox");
            pdfText.Should().Contain("0 0 300 400");
        }

        [Fact]
        public void CreateDocument_MinimalValid_CanBeParsed()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.AddPage();
            var pdfData = doc.SaveToMemory();

            // Assert - Check for all required PDF elements
            var pdfText = System.Text.Encoding.ASCII.GetString(pdfData);

            // Header
            pdfText.Should().MatchRegex(@"^%PDF-1\.\d");

            // Objects
            pdfText.Should().MatchRegex(@"\d+ \d+ obj");
            pdfText.Should().Contain("endobj");

            // Xref
            pdfText.Should().Contain("xref");

            // Trailer
            pdfText.Should().Contain("trailer");
            pdfText.Should().Contain("/Size");
            pdfText.Should().Contain("/Root");

            // Footer
            pdfText.Should().Contain("startxref");
            pdfText.Trim().Should().EndWith("%%EOF");
        }
    }
}
