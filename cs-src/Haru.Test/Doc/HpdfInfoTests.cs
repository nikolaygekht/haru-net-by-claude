using System;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Xref;


namespace Haru.Test.Doc
{
    public class HpdfInfoTests
    {
        [Fact]
        public void Constructor_CreatesValidInfoDict()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var info = new HpdfInfo(xref);

            // Assert
            info.Should().NotBeNull();
            info.Dict.Should().NotBeNull();
            info.Producer.Should().Contain("Haru");
        }

        [Fact]
        public void Title_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);

            // Act
            info.Title = "Test Document";

            // Assert
            info.Title.Should().Be("Test Document");
        }

        [Fact]
        public void Author_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);

            // Act
            info.Author = "John Doe";

            // Assert
            info.Author.Should().Be("John Doe");
        }

        [Fact]
        public void Subject_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);

            // Act
            info.Subject = "Test Subject";

            // Assert
            info.Subject.Should().Be("Test Subject");
        }

        [Fact]
        public void Keywords_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);

            // Act
            info.Keywords = "test, pdf, document";

            // Assert
            info.Keywords.Should().Be("test, pdf, document");
        }

        [Fact]
        public void Creator_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);

            // Act
            info.Creator = "Test Application";

            // Assert
            info.Creator.Should().Be("Test Application");
        }

        [Fact]
        public void Producer_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);

            // Act
            info.Producer = "Custom Producer";

            // Assert
            info.Producer.Should().Be("Custom Producer");
        }

        [Fact]
        public void SetCreationDate_SetsDateInPdfFormat()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);
            var date = new DateTime(2023, 12, 15, 14, 30, 45);

            // Act
            info.SetCreationDate(date);

            // Assert
            var dateStr = info.GetCreationDate();
            dateStr.Should().StartWith("D:20231215143045");
        }

        [Fact]
        public void SetModificationDate_SetsDateInPdfFormat()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);
            var date = new DateTime(2023, 12, 16, 10, 15, 30);

            // Act
            info.SetModificationDate(date);

            // Assert
            var dateStr = info.GetModificationDate();
            dateStr.Should().StartWith("D:20231216101530");
        }

        [Fact]
        public void Trapped_SetValid_ReturnsCorrectValue()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);

            // Act
            info.Trapped = "True";

            // Assert
            info.Trapped.Should().Be("True");
        }

        [Fact]
        public void Trapped_SetInvalid_ThrowsException()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);

            // Act
            Action act = () => info.Trapped = "Invalid";

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Trapped*");
        }

        [Fact]
        public void CustomMetadata_SetAndGet_ReturnsCorrectValue()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);

            // Act
            info.SetCustomMetadata("CustomKey", "CustomValue");

            // Assert
            info.GetCustomMetadata("CustomKey").Should().Be("CustomValue");
        }

        [Fact]
        public void SetNull_RemovesEntry()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var info = new HpdfInfo(xref);
            info.Title = "Test";

            // Act
            info.Title = null;

            // Assert
            info.Title.Should().BeNull();
        }

        [Fact]
        public void HpdfDocument_HasInfoDictionary()
        {
            // Arrange & Act
            var doc = new HpdfDocument();

            // Assert
            doc.Info.Should().NotBeNull();
            doc.Info.Producer.Should().Contain("Haru");
        }

        [Fact]
        public void HpdfDocument_InfoInTrailer()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.Info.Title = "Test Document";

            // Assert
            doc.Xref.Trailer.Should().ContainKey("Info");
            doc.Xref.Trailer["Info"].Should().Be(doc.Info.Dict);
        }

        [Fact]
        public void CompleteMetadata_AllFieldsSet()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.Info.Title = "Complete Test Document";
            doc.Info.Author = "Test Author";
            doc.Info.Subject = "Testing PDF Metadata";
            doc.Info.Keywords = "test, pdf, metadata, haru";
            doc.Info.Creator = "Test Application v1.0";
            doc.Info.Producer = "Haru PDF Library";
            doc.Info.SetCreationDate(DateTime.Now);
            doc.Info.SetModificationDate(DateTime.Now);
            doc.Info.Trapped = "False";

            // Assert
            doc.Info.Title.Should().Be("Complete Test Document");
            doc.Info.Author.Should().Be("Test Author");
            doc.Info.Subject.Should().Be("Testing PDF Metadata");
            doc.Info.Keywords.Should().Be("test, pdf, metadata, haru");
            doc.Info.Creator.Should().Be("Test Application v1.0");
            doc.Info.Producer.Should().Be("Haru PDF Library");
            doc.Info.Trapped.Should().Be("False");
            doc.Info.GetCreationDate().Should().StartWith("D:");
            doc.Info.GetModificationDate().Should().StartWith("D:");
        }
    }
}
