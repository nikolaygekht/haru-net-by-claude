using System;
using System.Text;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Xref;
using Haru.Types;


namespace Haru.Test.Doc
{
    public class HpdfPageExtGStateTests
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

        [Fact]
        public void SetExtGState_WritesCorrectOperator()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            page.SetExtGState(extGState);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("/GS1 gs\n");
        }

        [Fact]
        public void SetExtGState_AddsToResources()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            page.SetExtGState(extGState);

            // Assert
            var resources = page.GetResources();
            resources.Should().ContainKey("ExtGState");
        }

        [Fact]
        public void SetExtGState_ThrowsWhenExtGStateIsNull()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            Action act = () => page.SetExtGState(null!);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Extended graphics state cannot be null*");
        }

        [Fact]
        public void SetExtGState_MultipleStates_AllAdded()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);
            var extGState1 = new HpdfExtGState(xref, "GS1");
            var extGState2 = new HpdfExtGState(xref, "GS2");

            // Act
            page.SetExtGState(extGState1);
            page.SetExtGState(extGState2);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("/GS1 gs\n");
            content.Should().Contain("/GS2 gs\n");
        }

        [Fact]
        public void SetExtGState_SameStateTwice_OnlyAddedOnce()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            page.SetExtGState(extGState);
            page.SetExtGState(extGState);

            // Assert
            var resources = page.GetResources();
            var extGStateDict = resources["ExtGState"] as Haru.Objects.HpdfDict;
            extGStateDict.Should().NotBeNull();
            extGStateDict.Count.Should().Be(1);
        }

        // Integration Tests

        [Fact]
        public void TransparentDrawing_ProducesValidSequence()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);
            var extGState = new HpdfExtGState(xref, "GS1");
            extGState.SetAlphaFill(0.5f);

            // Act
            page.GSave();
            page.SetExtGState(extGState);
            page.Circle(100, 100, 50);
            page.Fill();
            page.GRestore();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("q\n");
            content.Should().Contain("/GS1 gs\n");
            content.Should().Contain(" m\n");
            content.Should().Contain(" c\n");
            content.Should().Contain("f\n");
            content.Should().Contain("Q\n");
        }

        [Fact]
        public void BlendModeDrawing_ProducesValidSequence()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);
            var extGState = new HpdfExtGState(xref, "GS1");
            extGState.SetBlendMode(HpdfBlendMode.Multiply);

            // Act
            page.GSave();
            page.SetExtGState(extGState);
            page.Rectangle(50, 50, 100, 100);
            page.FillStroke();
            page.GRestore();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("q\n");
            content.Should().Contain("/GS1 gs\n");
            content.Should().Contain("50 50 100 100 re\n");
            content.Should().Contain("B\n");
            content.Should().Contain("Q\n");
        }

        [Fact]
        public void ComplexTransparencyScene_ProducesValidPDF()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);
            var gs1 = new HpdfExtGState(xref, "GS1");
            gs1.SetAlphaFill(0.7f);
            var gs2 = new HpdfExtGState(xref, "GS2");
            gs2.SetAlphaFill(0.3f);

            // Act - Draw two overlapping transparent circles
            page.SetRgbFill(1, 0, 0);
            page.GSave();
            page.SetExtGState(gs1);
            page.Circle(100, 100, 50);
            page.Fill();
            page.GRestore();

            page.SetRgbFill(0, 0, 1);
            page.GSave();
            page.SetExtGState(gs2);
            page.Circle(150, 100, 50);
            page.Fill();
            page.GRestore();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("1 0 0 rg\n");
            content.Should().Contain("/GS1 gs\n");
            content.Should().Contain("0 0 1 rg\n");
            content.Should().Contain("/GS2 gs\n");
        }
    }
}
