using System;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Objects;

namespace Haru.Test.Doc
{
    public class HpdfOutlineTests
    {
        [Fact]
        public void CreateOutline_CreatesTopLevelBookmark()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            var outline = doc.CreateOutline("Chapter 1");

            // Assert
            outline.Should().NotBeNull();
            outline.Dict.Should().ContainKey("Title");
            outline.Dict.Should().ContainKey("Parent");
        }

        [Fact]
        public void CreateOutline_AddsToDocumentCatalog()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.CreateOutline("Chapter 1");

            // Assert
            doc.Catalog.Dict.Should().ContainKey("Outlines");
        }

        [Fact]
        public void CreateChildOutline_CreatesNestedStructure()
        {
            // Arrange
            var doc = new HpdfDocument();
            var chapter = doc.CreateOutline("Chapter 1");

            // Act
            var section = chapter.CreateChild("Section 1.1");

            // Assert
            section.Should().NotBeNull();
            section.Dict.Should().ContainKey("Parent");

            chapter.Dict.Should().ContainKey("First");
            chapter.Dict.Should().ContainKey("Last");
        }

        [Fact]
        public void CreateMultipleChildren_LinksCorrectly()
        {
            // Arrange
            var doc = new HpdfDocument();
            var chapter = doc.CreateOutline("Chapter 1");

            // Act
            var section1 = chapter.CreateChild("Section 1.1");
            var section2 = chapter.CreateChild("Section 1.2");

            // Assert
            chapter.Dict["First"].Should().Be(section1.Dict);
            chapter.Dict["Last"].Should().Be(section2.Dict);
            section1.Dict.Should().ContainKey("Next");
            section2.Dict.Should().ContainKey("Prev");
        }

        [Fact]
        public void SaveDocument_WithOutlines_ProducesValidPDF()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page1 = doc.AddPage();
            var page2 = doc.AddPage();

            var chapter1 = doc.CreateOutline("Chapter 1");
            chapter1.CreateChild("Section 1.1");
            chapter1.CreateChild("Section 1.2");

            var chapter2 = doc.CreateOutline("Chapter 2");

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);
            System.Text.Encoding.ASCII.GetString(pdfData, 0, 5).Should().Be("%PDF-");
        }
    }
}
