using System;
using System.Text;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Xref;


namespace Haru.Test.Doc
{
    public class HpdfPageShapesTests
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

        // Circle Tests

        [Fact]
        public void Circle_CreatesValidPath()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Circle(100, 100, 50);

            // Assert
            var content = GetPageContent(page);
            // Should start with MoveTo to leftmost point
            content.Should().Contain("50 100 m\n");
            // Should contain multiple CurveTo operators (4 for a complete circle)
            content.Should().Contain(" c\n");
        }

        [Fact]
        public void Circle_ThrowsWhenRadiusIsZero()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            Action act = () => page.Circle(100, 100, 0);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Radius must be positive*");
        }

        [Fact]
        public void Circle_ThrowsWhenRadiusIsNegative()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            Action act = () => page.Circle(100, 100, -10);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Radius must be positive*");
        }

        [Fact]
        public void Circle_CanBeStroked()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Circle(200, 200, 75);
            page.Stroke();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("125 200 m\n");  // 200 - 75 = 125
            content.Should().Contain("S\n");
        }

        [Fact]
        public void Circle_CanBeFilled()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Circle(200, 200, 75);
            page.Fill();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("125 200 m\n");  // 200 - 75 = 125
            content.Should().Contain("f\n");
        }

        // Ellipse Tests

        [Fact]
        public void Ellipse_CreatesValidPath()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Ellipse(100, 100, 80, 50);

            // Assert
            var content = GetPageContent(page);
            // Should start with MoveTo to leftmost point
            content.Should().Contain("20 100 m\n");
            // Should contain multiple CurveTo operators
            content.Should().Contain(" c\n");
        }

        [Fact]
        public void Ellipse_ThrowsWhenXRadiusIsZero()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            Action act = () => page.Ellipse(100, 100, 0, 50);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Radii must be positive*");
        }

        [Fact]
        public void Ellipse_ThrowsWhenYRadiusIsNegative()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            Action act = () => page.Ellipse(100, 100, 50, -10);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Radii must be positive*");
        }

        [Fact]
        public void Ellipse_WithEqualRadii_CreatesCircle()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Ellipse(100, 100, 50, 50);

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("50 100 m\n");
        }

        // Arc Tests

        [Fact]
        public void Arc_CreatesValidPath()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Arc(100, 100, 50, 0, 90);

            // Assert
            var content = GetPageContent(page);
            // Should contain MoveTo and CurveTo
            content.Should().Contain(" m\n");
            content.Should().Contain(" c\n");
        }

        [Fact]
        public void Arc_ThrowsWhenRadiusIsZero()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            Action act = () => page.Arc(100, 100, 0, 0, 90);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Radius must be positive*");
        }

        [Fact]
        public void Arc_ThrowsWhenAngleRangeIs360OrMore()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            Action act = () => page.Arc(100, 100, 50, 0, 360);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Arc angle range must be < 360 degrees*");
        }

        [Fact]
        public void Arc_HandlesNegativeAngles()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act - Should normalize negative angles
            page.Arc(100, 100, 50, -90, 0);

            // Assert
            var content = GetPageContent(page);
            content.Should().NotBeEmpty();
        }

        [Fact]
        public void Arc_LargeArc_BreaksIntoSegments()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act - 270 degree arc should be broken into multiple segments
            page.Arc(100, 100, 50, 0, 270);

            // Assert
            var content = GetPageContent(page);
            // Should contain multiple CurveTo operators (at least 3 for 270 degrees)
            var curveCount = CountOccurrences(content, " c\n");
            curveCount.Should().BeGreaterOrEqualTo(3);
        }

        // Integration Tests

        [Fact]
        public void MultipleShapes_ProduceValidPDF()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Circle(100, 100, 30);
            page.Stroke();

            page.Ellipse(200, 100, 40, 20);
            page.Fill();

            page.Arc(300, 100, 30, 0, 180);
            page.Stroke();

            // Assert
            var content = GetPageContent(page);
            content.Should().Contain("70 100 m\n");   // Circle start
            content.Should().Contain("160 100 m\n");  // Ellipse start
            // Arc operations
            content.Should().Contain("S\n");          // Stroke operations
            content.Should().Contain("f\n");          // Fill operation
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
