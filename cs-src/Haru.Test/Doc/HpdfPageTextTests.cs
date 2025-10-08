using System;
using System.Text;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Font;
using Haru.Xref;
using Haru.Types;

namespace Haru.Test.Doc
{
    public class HpdfPageTextTests
    {
        private HpdfPage CreateTestPage()
        {
            var xref = new HpdfXref(0);
            return new HpdfPage(xref);
        }

        private string GetPageContent(HpdfPage page)
        {
            return Encoding.ASCII.GetString(page.Contents.Stream.ToArray());
        }

        // Text Object Operations

        [Fact]
        public void BeginText_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.BeginText();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("BT\n");
        }

        [Fact]
        public void EndText_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.EndText();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("ET\n");
        }

        [Fact]
        public void BeginText_InitializesTextMatrix()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.BeginText();

            // Assert
            page.TextMatrix.Should().NotBeNull();
            page.TextMatrix.A.Should().Be(1);
            page.TextMatrix.D.Should().Be(1);
            page.TextLineMatrix.Should().NotBeNull();
        }

        // Text State Operations

        [Fact]
        public void SetCharSpace_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.SetCharSpace(2.5f);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("2.5 Tc\n");
            page.GraphicsState.CharSpace.Should().Be(2.5f);
        }

        [Fact]
        public void SetWordSpace_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.SetWordSpace(3.0f);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("3 Tw\n");
            page.GraphicsState.WordSpace.Should().Be(3.0f);
        }

        [Fact]
        public void SetHorizontalScaling_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.SetHorizontalScaling(110f);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("110 Tz\n");
            page.GraphicsState.HorizontalScaling.Should().Be(110f);
        }

        [Fact]
        public void SetHorizontalScaling_ThrowsWhenNegative()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            Action act = () => page.SetHorizontalScaling(-50f);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Horizontal scaling must be positive*");
        }

        [Fact]
        public void SetTextLeading_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.SetTextLeading(14f);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("14 TL\n");
            page.GraphicsState.TextLeading.Should().Be(14f);
        }

        [Fact]
        public void SetFontAndSize_WritesCorrectOperator()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            page.SetFontAndSize(font, 12f);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("/F1 12 Tf\n");
            page.CurrentFont.Should().BeSameAs(font);
            page.GraphicsState.FontSize.Should().Be(12f);
        }

        [Fact]
        public void SetFontAndSize_AddsFontToResources()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            page.SetFontAndSize(font, 12f);

            // Assert
            var resources = page.GetResources();
            resources.Should().ContainKey("Font");
        }

        [Fact]
        public void SetFontAndSize_ThrowsWhenFontIsNull()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            Action act = () => page.SetFontAndSize(null, 12f);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Font cannot be null*");
        }

        [Fact]
        public void SetFontAndSize_ThrowsWhenSizeIsZero()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            Action act = () => page.SetFontAndSize(font, 0f);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Font size must be positive*");
        }

        [Fact]
        public void SetTextRenderingMode_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.SetTextRenderingMode(HpdfTextRenderingMode.Stroke);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("1 Tr\n");
            page.GraphicsState.RenderingMode.Should().Be(1);
        }

        [Theory]
        [InlineData(HpdfTextRenderingMode.Fill, "0 Tr\n")]
        [InlineData(HpdfTextRenderingMode.Stroke, "1 Tr\n")]
        [InlineData(HpdfTextRenderingMode.FillThenStroke, "2 Tr\n")]
        [InlineData(HpdfTextRenderingMode.Invisible, "3 Tr\n")]
        public void SetTextRenderingMode_AllModes_WriteCorrectly(HpdfTextRenderingMode mode, string expected)
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.SetTextRenderingMode(mode);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain(expected);
        }

        [Fact]
        public void SetTextRise_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.SetTextRise(5f);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("5 Ts\n");
            page.GraphicsState.TextRise.Should().Be(5f);
        }

        // Text Positioning Operations

        [Fact]
        public void MoveTextPos_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.MoveTextPos(100f, 200f);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("100 200 Td\n");
        }

        [Fact]
        public void MoveTextPosAndSetLeading_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.MoveTextPosAndSetLeading(50f, -14f);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("50 -14 TD\n");
            page.GraphicsState.TextLeading.Should().Be(14f);
        }

        [Fact]
        public void SetTextMatrix_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();
            var matrix = new HpdfTransMatrix(1, 0, 0, 1, 100, 700);

            // Act
            page.SetTextMatrix(matrix);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("1 0 0 1 100 700 Tm\n");
            page.TextMatrix.X.Should().Be(100);
            page.TextMatrix.Y.Should().Be(700);
        }

        [Fact]
        public void MoveToNextLine_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();
            page.SetTextLeading(14f);

            // Act
            page.MoveToNextLine();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("T*\n");
        }

        // Text Showing Operations

        [Fact]
        public void ShowText_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.ShowText("Hello");

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("(Hello) Tj\n");
        }

        [Fact]
        public void ShowText_EmptyString_DoesNotWrite()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.ShowText("");

            // Assert
            var content = GetPageContent(page);
            content.Should().BeEmpty();
        }

        [Fact]
        public void ShowText_EscapesSpecialCharacters()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.ShowText("Test\\");

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("Tj\n");
        }

        [Fact]
        public void ShowTextNextLine_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.ShowTextNextLine("World");

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("(World) '\n");
        }

        [Fact]
        public void SetSpacingAndShowText_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.SetSpacingAndShowText(1.5f, 0.5f, "Test");

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("1.5 0.5 (Test) \"\n");
            page.GraphicsState.WordSpace.Should().Be(1.5f);
            page.GraphicsState.CharSpace.Should().Be(0.5f);
        }

        // Integration Tests

        [Fact]
        public void CompleteTextSequence_ProducesValidPDF()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            page.BeginText();
            page.SetFontAndSize(font, 12f);
            page.MoveTextPos(100f, 700f);
            page.ShowText("Hello, World!");
            page.EndText();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("BT\n");
            content.Should().Contain("/F1 12 Tf\n");
            content.Should().Contain("100 700 Td\n");
            content.Should().Contain("(Hello, World!) Tj\n");
            content.Should().Contain("ET\n");
        }

        [Fact]
        public void MultipleLines_WithLeading_WorksCorrectly()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            var font = new HpdfFont(xref, HpdfStandardFont.TimesRoman, "F1");

            // Act
            page.BeginText();
            page.SetFontAndSize(font, 10f);
            page.SetTextLeading(14f);
            page.MoveTextPos(50f, 750f);
            page.ShowText("Line 1");
            page.MoveToNextLine();
            page.ShowText("Line 2");
            page.MoveToNextLine();
            page.ShowText("Line 3");
            page.EndText();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("(Line 1) Tj\n");
            content.Should().Contain("T*\n");
            content.Should().Contain("(Line 2) Tj\n");
            content.Should().Contain("(Line 3) Tj\n");
        }
    }
}
