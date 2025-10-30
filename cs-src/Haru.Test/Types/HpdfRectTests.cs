using FluentAssertions;
using Haru.Types;
using Xunit;


namespace Haru.Test.Types
{
    public class HpdfRectTests
    {
        [Fact]
        public void Constructor_SetsAllCoordinates()
        {
            var rect = new HpdfRect(10f, 20f, 100f, 80f);

            rect.Left.Should().Be(10f);
            rect.Bottom.Should().Be(20f);
            rect.Right.Should().Be(100f);
            rect.Top.Should().Be(80f);
        }

        [Fact]
        public void Width_CalculatesCorrectly()
        {
            var rect = new HpdfRect(10f, 20f, 100f, 80f);

            rect.Width.Should().Be(90f); // 100 - 10
        }

        [Fact]
        public void Height_CalculatesCorrectly()
        {
            var rect = new HpdfRect(10f, 20f, 100f, 80f);

            rect.Height.Should().Be(60f); // 80 - 20
        }

        [Fact]
        public void Equals_WithSameValues_ReturnsTrue()
        {
            var rect1 = new HpdfRect(10f, 20f, 100f, 80f);
            var rect2 = new HpdfRect(10f, 20f, 100f, 80f);

            rect1.Equals(rect2).Should().BeTrue();
            (rect1 == rect2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentValues_ReturnsFalse()
        {
            var rect1 = new HpdfRect(10f, 20f, 100f, 80f);
            var rect2 = new HpdfRect(10f, 20f, 100f, 81f);

            rect1.Equals(rect2).Should().BeFalse();
            (rect1 != rect2).Should().BeTrue();
        }
    }
}
