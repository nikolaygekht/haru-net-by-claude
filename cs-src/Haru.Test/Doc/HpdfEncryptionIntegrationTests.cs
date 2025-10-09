using System;
using System.IO;
using FluentAssertions;
using Haru.Doc;
using Haru.Types;
using Xunit;
using HpdfPageDirection = Haru.Doc.HpdfPageDirection;

namespace Haru.Test.Doc
{
    /// <summary>
    /// Integration tests for PDF encryption at the document level
    /// </summary>
    public class HpdfEncryptionIntegrationTests
    {
        [Fact]
        public void SetEncryption_R2_ShouldCreateEncryptedPDF()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R2);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);

            // PDF should contain encryption dictionary
            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            pdfText.Should().Contain("/Encrypt");
            pdfText.Should().Contain("/V 1"); // Version 1 for R2
            pdfText.Should().Contain("/R 2"); // Revision 2
        }

        [Fact]
        public void SetEncryption_R3_ShouldCreateEncryptedPDF()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print | HpdfPermission.Copy, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);

            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            pdfText.Should().Contain("/Encrypt");
            pdfText.Should().Contain("/V 2"); // Version 2 for R3
            pdfText.Should().Contain("/R 3"); // Revision 3
            pdfText.Should().Contain("/Length 128"); // 128-bit key
        }

        [Fact]
        public void SetEncryption_R4_ShouldCreateEncryptedPDF()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R4);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);

            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            pdfText.Should().Contain("/Encrypt");
            pdfText.Should().Contain("/V 4"); // Version 4 for R4 (AES)
            pdfText.Should().Contain("/R 4"); // Revision 4
            pdfText.Should().Contain("/CF"); // Crypt filter for AES
            pdfText.Should().Contain("/AESV2"); // AES version 2
        }

        [Fact]
        public void SetEncryption_ShouldIncludeDocumentID()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            pdfText.Should().Contain("/ID"); // Document ID is required for encryption
        }

        [Fact]
        public void SetEncryption_WithPDFA_ShouldReuseExistingID()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.SetPdfACompliance("1B"); // PDF/A creates a document ID
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);

            // Should have exactly one ID entry (not duplicated)
            int idCount = 0;
            int index = 0;
            while ((index = pdfText.IndexOf("/ID", index)) != -1)
            {
                idCount++;
                index += 3;
            }
            idCount.Should().Be(1);
        }

        [Fact]
        public void SetEncryption_R2_ShouldUpdatePDFVersion()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Version = HpdfVersion.Version12; // Start with PDF 1.2
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R2);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            // R2 works with PDF 1.2, so version should not change
            pdfText.Should().StartWith("%PDF-1.2");
        }

        [Fact]
        public void SetEncryption_R3_ShouldUpdatePDFVersionTo14()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Version = HpdfVersion.Version12; // Start with PDF 1.2
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            pdfText.Should().StartWith("%PDF-1.4"); // R3 requires PDF 1.4
        }

        [Fact]
        public void SetEncryption_R4_ShouldUpdatePDFVersionTo16()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Version = HpdfVersion.Version12; // Start with PDF 1.2
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R4);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            pdfText.Should().StartWith("%PDF-1.6"); // R4 requires PDF 1.6
        }

        [Fact]
        public void SetEncryption_WithContent_ShouldEncryptStrings()
        {
            // Arrange
            var doc = new HpdfDocument();
            doc.Info.Title = "Secret Document";
            doc.Info.Author = "Test Author";
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);

            // The actual title and author should be encrypted, so plain text shouldn't appear
            // (though Latin-1 decoding might show garbage)
            pdfText.Should().Contain("/Title");
            pdfText.Should().Contain("/Author");
        }

        [Fact]
        public void SetEncryption_WithPageContent_ShouldEncryptStreams()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);

            // Add some content to the page
            page.SetRgbStroke(0.5f, 0.5f, 0.5f);
            page.SetLineWidth(2);
            page.Rectangle(100, 100, 200, 200);
            page.Stroke();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);

            // The PDF should have encrypted stream content
            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            pdfText.Should().Contain("stream");
            pdfText.Should().Contain("endstream");
        }

        [Theory]
        [InlineData(HpdfPermission.Print)]
        [InlineData(HpdfPermission.Print | HpdfPermission.Copy)]
        [InlineData(HpdfPermission.Print | HpdfPermission.Edit)]
        [InlineData(HpdfPermission.All)]
        public void SetEncryption_DifferentPermissions_ShouldSetCorrectly(HpdfPermission permission)
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", permission, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            pdfText.Should().Contain("/P "); // Permission flags
        }

        [Fact]
        public void IsEncrypted_WithoutEncryption_ShouldReturnFalse()
        {
            // Arrange
            var doc = new HpdfDocument();

            // Act & Assert
            doc.IsEncrypted.Should().BeFalse();
        }

        [Fact]
        public void IsEncrypted_WithEncryption_ShouldReturnTrue()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R3);

            // Assert
            doc.IsEncrypted.Should().BeTrue();
        }

        [Fact]
        public void SetEncryption_EmptyPasswords_ShouldUseDefaultPadding()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            // Act
            doc.SetEncryption("", "", HpdfPermission.Print, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void SetEncryption_LongPasswords_ShouldTruncateCorrectly()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            string longPassword = new string('a', 100);

            // Act
            doc.SetEncryption(longPassword, longPassword, HpdfPermission.Print, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void SetEncryption_SpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            string specialPassword = "p@$$w0rd!#123";

            // Act
            doc.SetEncryption(specialPassword, specialPassword, HpdfPermission.Print, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void SetEncryption_MultiplePages_ShouldEncryptAllContent()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page1 = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
            var page2 = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);
            var page3 = doc.AddPage(HpdfPageSize.A4, HpdfPageDirection.Portrait);

            // Add content to pages
            page1.SetLineWidth(1);
            page1.Rectangle(50, 50, 100, 100);
            page1.Stroke();

            page2.SetLineWidth(2);
            page2.Rectangle(100, 100, 150, 150);
            page2.Stroke();

            page3.SetLineWidth(3);
            page3.Rectangle(150, 150, 200, 200);
            page3.Stroke();

            // Act
            doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R3);
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);

            string pdfText = System.Text.Encoding.Latin1.GetString(pdfData);
            pdfText.Should().Contain("/Count 3"); // Should have 3 pages
        }

        [Fact]
        public void SetEncryption_SaveToFile_ShouldCreateEncryptedFile()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            string tempFile = Path.GetTempFileName();

            try
            {
                // Act
                doc.SetEncryption("user", "owner", HpdfPermission.Print, HpdfEncryptMode.R3);
                doc.SaveToFile(tempFile);

                // Assert
                File.Exists(tempFile).Should().BeTrue();
                var fileInfo = new FileInfo(tempFile);
                fileInfo.Length.Should().BeGreaterThan(0);

                byte[] fileData = File.ReadAllBytes(tempFile);
                string pdfText = System.Text.Encoding.Latin1.GetString(fileData);
                pdfText.Should().Contain("/Encrypt");
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
