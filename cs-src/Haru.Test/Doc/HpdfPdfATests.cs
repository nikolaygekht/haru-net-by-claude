using System;
using System.Text;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Objects;


namespace Haru.Test.Doc
{
    public class HpdfPdfATests
    {
        [Fact]
        public void SetPdfACompliance_EnablesCompliance()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.SetPdfACompliance("1B");

            // Assert
            doc.IsPdfACompliant.Should().BeTrue();
        }

        [Fact]
        public void SetPdfACompliance_SetsVersionTo14()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act
            doc.SetPdfACompliance("1B");

            // Assert
            doc.Version.Should().Be(HpdfVersion.Version14);
        }

        [Fact]
        public void SaveDocument_WithPdfA_AddsMetadataToCatalog()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Info.Title = "Test PDF/A Document";
            doc.Info.Author = "Test Author";
            doc.SetPdfACompliance("1B");
            doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            doc.Catalog.Dict.Should().ContainKey("Metadata");
            var metadata = doc.Catalog.Dict["Metadata"];
            metadata.Should().BeOfType<HpdfStreamObject>();
        }

        [Fact]
        public void SaveDocument_WithPdfA_AddsOutputIntentsToCatalog()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetPdfACompliance("1B");
            doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            doc.Catalog.Dict.Should().ContainKey("OutputIntents");
            var outputIntents = doc.Catalog.Dict["OutputIntents"];
            outputIntents.Should().BeOfType<HpdfArray>();

            var array = outputIntents as HpdfArray;
            array!.Count.Should().Be(1);
            array[0].Should().BeOfType<HpdfDict>();
        }

        [Fact]
        public void SaveDocument_WithPdfA_AddsDocumentIdToTrailer()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetPdfACompliance("1B");
            doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            doc.Xref.Trailer.Should().ContainKey("ID");
            var id = doc.Xref.Trailer["ID"];
            id.Should().BeOfType<HpdfArray>();

            var idArray = id as HpdfArray;
            idArray!.Count.Should().Be(2);
            idArray![0].Should().BeOfType<HpdfBinary>();
            idArray![1].Should().BeOfType<HpdfBinary>();
        }

        [Fact]
        public void XmpMetadata_ContainsPdfAIdentification()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Info.Title = "Test Title";
            doc.SetPdfACompliance("1B");
            doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            doc.Catalog.Dict.Should().ContainKey("Metadata");
            var metadataStream = doc.Catalog.Dict["Metadata"] as HpdfStreamObject;
            metadataStream!.Should().NotBeNull();

            var xmpBytes = metadataStream!.Stream.ToArray();
            var xmpContent = System.Text.Encoding.UTF8.GetString(xmpBytes);

            xmpContent.Should().Contain("pdfaid:part");
            xmpContent.Should().Contain("pdfaid:conformance");
            xmpContent.Should().Contain("<pdfaid:part>1</pdfaid:part>");
            xmpContent.Should().Contain("<pdfaid:conformance>1B</pdfaid:conformance>");
        }

        [Fact]
        public void XmpMetadata_ContainsDocumentInfo()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Info.Title = "My PDF/A Document";
            doc.Info.Author = "John Doe";
            doc.Info.Subject = "Testing PDF/A";
            doc.Info.Keywords = "PDF, PDF/A, Archive";
            doc.SetPdfACompliance("1B");
            doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            var metadataStream = doc.Catalog.Dict["Metadata"] as HpdfStreamObject;
            var xmpBytes = metadataStream!.Stream.ToArray();
            var xmpContent = System.Text.Encoding.UTF8.GetString(xmpBytes);

            xmpContent.Should().Contain("My PDF/A Document");
            xmpContent.Should().Contain("John Doe");
            xmpContent.Should().Contain("Testing PDF/A");
            xmpContent.Should().Contain("PDF, PDF/A, Archive");
        }

        [Fact]
        public void OutputIntent_HasCorrectStructure()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetPdfACompliance("1B");
            doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            var outputIntents = doc.Catalog.Dict["OutputIntents"] as HpdfArray;
            var outputIntent = outputIntents![0] as HpdfDict;

            outputIntent!.Should().ContainKey("Type");
            outputIntent!.Should().ContainKey("S");
            outputIntent!.Should().ContainKey("OutputCondition");
            outputIntent!.Should().ContainKey("OutputConditionIdentifier");

            var type = outputIntent!["Type"] as HpdfName;
            type!.Value.Should().Be("OutputIntent");

            var s = outputIntent!["S"] as HpdfName;
            s!.Value.Should().Be("GTS_PDFA1");
        }

        [Fact]
        public void DocumentId_IsConsistent()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Info.Title = "Consistent ID Test";
            doc.SetPdfACompliance("1B");
            doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            var idArray = doc.Xref.Trailer["ID"] as HpdfArray;
            var id1 = idArray![0] as HpdfBinary;
            var id2 = idArray![1] as HpdfBinary;

            // Both IDs should be identical for PDF/A
            id1!.Value.Should().BeEquivalentTo(id2!.Value);

            // ID should be 16 bytes (MD5 hash)
            id1!.Length.Should().Be(16);
        }

        [Fact]
        public void SaveDocument_WithPdfA_ProducesValidPDF()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Info.Title = "Complete PDF/A Test";
            doc.Info.Author = "Haru Library";
            doc.Info.Creator = "Haru C# Port";
            doc.SetPdfACompliance("1B");

            var page = doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);

            // Check PDF header
            string header = System.Text.Encoding.ASCII.GetString(pdfData, 0, 8);
            header.Should().StartWith("%PDF-1.4"); // PDF/A-1 requires 1.4
        }

        [Fact]
        public void XmpMetadata_IncludesCreationDate()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Info.Title = "Date Test";
            doc.Info.SetCreationDate(new DateTime(2025, 1, 15, 10, 30, 45, DateTimeKind.Utc));
            doc.SetPdfACompliance("1B");
            doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            var metadataStream = doc.Catalog.Dict["Metadata"] as HpdfStreamObject;
            var xmpBytes = metadataStream!.Stream.ToArray();
            var xmpContent = System.Text.Encoding.UTF8.GetString(xmpBytes);

            xmpContent.Should().Contain("xmp:CreateDate");
            xmpContent.Should().Contain("2025-01-15");
        }

        [Fact]
        public void PdfACompliance_CanBeCheckedBeforeSave()
        {
            // Arrange
            var doc1 = new HpdfDocument();
            var doc2 = new HpdfDocument();
            doc2.SetPdfACompliance("1B");

            // Assert
            doc1.IsPdfACompliant.Should().BeFalse();
            doc2.IsPdfACompliant.Should().BeTrue();
        }

        [Fact]
        public void XmpMetadata_EscapesSpecialXmlCharacters()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Info.Title = "Title with <special> & \"characters\"";
            doc.SetPdfACompliance("1B");
            doc.AddPage();

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            var metadataStream = doc.Catalog.Dict["Metadata"] as HpdfStreamObject;
            var xmpBytes = metadataStream!.Stream.ToArray();
            var xmpContent = System.Text.Encoding.UTF8.GetString(xmpBytes);

            xmpContent.Should().Contain("&lt;special&gt;");
            xmpContent.Should().Contain("&amp;");
            xmpContent.Should().Contain("&quot;");
        }
    }
}
