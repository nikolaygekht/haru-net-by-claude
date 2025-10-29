using System.Text;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Xref;
using Haru.Types;

namespace Haru.Test.Doc
{
    public class HpdfPageAdvancedGraphicsTests
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

        // Bezier Curve Operations

        [Fact]
        public void CurveTo_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.CurveTo(100, 200, 150, 250, 200, 200);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("100 200 150 250 200 200 c\n");
            page.CurrentPos.X.Should().Be(200);
            page.CurrentPos.Y.Should().Be(200);
        }

        [Fact]
        public void CurveTo2_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.CurveTo2(150, 250, 200, 200);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("150 250 200 200 v\n");
            page.CurrentPos.X.Should().Be(200);
            page.CurrentPos.Y.Should().Be(200);
        }

        [Fact]
        public void CurveTo3_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.CurveTo3(100, 200, 200, 200);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("100 200 200 200 y\n");
            page.CurrentPos.X.Should().Be(200);
            page.CurrentPos.Y.Should().Be(200);
        }

        // Transformation Matrix Operations

        [Fact]
        public void Concat_WithFloats_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.Concat(1, 0, 0, 1, 100, 200);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("1 0 0 1 100 200 cm\n");
        }

        [Fact]
        public void Concat_WithMatrix_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();
            var matrix = new HpdfTransMatrix(2, 0, 0, 2, 50, 75);

            // Act
            page.Concat(matrix);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("2 0 0 2 50 75 cm\n");
        }

        [Fact]
        public void Concat_Rotation_ProducesRotationMatrix()
        {
            // Arrange
            var page = CreateTestPage();
            // Rotate 90 degrees: cos(90)=0, sin(90)=1
            float cos90 = 0f;
            float sin90 = 1f;

            // Act
            page.Concat(cos90, sin90, -sin90, cos90, 0, 0);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("0 1 -1 0 0 0 cm\n");
        }

        // Clipping Path Operations

        [Fact]
        public void Clip_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.Clip();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("W\n");
        }

        [Fact]
        public void EoClip_WritesCorrectOperator()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.EoClip();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("W*\n");
        }

        [Fact]
        public void ClipWithPath_ProducesValidSequence()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.Rectangle(50, 50, 100, 100);
            page.Clip();
            page.EndPath();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("50 50 100 100 re\n");
            content.Should().Contain("W\n");
            content.Should().Contain("n\n");
        }

        // Integration Tests

        [Fact]
        public void BezierCurveSequence_ProducesValidPath()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.MoveTo(100, 100);
            page.CurveTo(150, 150, 200, 150, 250, 100);
            page.Stroke();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("100 100 m\n");
            content.Should().Contain("150 150 200 150 250 100 c\n");
            content.Should().Contain("S\n");
        }

        [Fact]
        public void TransformAndDraw_ProducesValidSequence()
        {
            // Arrange
            var page = CreateTestPage();

            // Act
            page.GSave();
            page.Concat(2, 0, 0, 2, 0, 0);  // Scale by 2
            page.Rectangle(0, 0, 50, 50);
            page.Stroke();
            page.GRestore();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("q\n");
            content.Should().Contain("2 0 0 2 0 0 cm\n");
            content.Should().Contain("0 0 50 50 re\n");
            content.Should().Contain("S\n");
            content.Should().Contain("Q\n");
        }
    }
}
