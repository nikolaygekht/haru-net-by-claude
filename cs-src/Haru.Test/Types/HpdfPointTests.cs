using FluentAssertions;
using Haru.Types;
using Xunit;


namespace Haru.Test.Types
{
    public class HpdfPointTests
    {
        [Fact]
        public void Constructor_SetsXAndY()
        {
            var point = new HpdfPoint(10.5f, 20.3f);

            point.X.Should().Be(10.5f);
            point.Y.Should().Be(20.3f);
        }

        [Fact]
        public void Equals_WithSameValues_ReturnsTrue()
        {
            var point1 = new HpdfPoint(5f, 10f);
            var point2 = new HpdfPoint(5f, 10f);

            point1.Equals(point2).Should().BeTrue();
            (point1 == point2).Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentValues_ReturnsFalse()
        {
            var point1 = new HpdfPoint(5f, 10f);
            var point2 = new HpdfPoint(5f, 11f);

            point1.Equals(point2).Should().BeFalse();
            (point1 != point2).Should().BeTrue();
        }

        [Fact]
        public void GetHashCode_WithSameValues_ReturnsSameHash()
        {
            var point1 = new HpdfPoint(5f, 10f);
            var point2 = new HpdfPoint(5f, 10f);

            point1.GetHashCode().Should().Be(point2.GetHashCode());
        }

        [Fact]
        public void ToString_ReturnsFormattedString()
        {
            var point = new HpdfPoint(1.5f, 2.5f);

            point.ToString().Should().Be("(1.5, 2.5)");
        }
    }
}
