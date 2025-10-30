using FluentAssertions;
using Haru.Png;
using Xunit;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Png
{
    /// <summary>
    /// Tests for grayscale PNG images.
    /// Test file: 2x2 grayscale PNG with pixels: (0,0)=0, (1,0)=85, (0,1)=170, (1,1)=255
    /// </summary>
    public class GrayscalePngTests
    {
        private readonly IPngReader _pngReader;
        private const string TestFile = "test_grayscale_2x2.png";

        public GrayscalePngTests()
        {
            _pngReader = PngReaderFactory.Create();
        }

        [Fact]
        public void ReadImageInfo_GrayscalePng_ReturnsCorrectSize()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.Width.Should().Be(2);
                info.Height.Should().Be(2);
            }
        }

        [Fact]
        public void ReadImageInfo_GrayscalePng_ReturnsCorrectColorType()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.ColorType.Should().Be(PngColorType.Grayscale);
            }
        }

        [Fact]
        public void ReadImageInfo_GrayscalePng_ReturnsCorrectBitDepth()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.BitDepth.Should().Be(8);
            }
        }

        [Fact]
        public void ReadImageInfo_GrayscalePng_ReturnsCorrectChannelCount()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.Channels.Should().Be(1);
            }
        }

        [Fact]
        public void ReadImageInfo_GrayscalePng_HasNoAlpha()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.HasAlpha.Should().BeFalse();
            }
        }

        [Fact]
        public void ReadPixelData_GrayscalePng_ReturnsCorrectPixelValues()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                var info = _pngReader.ReadImageInfo(stream);
                stream.Position = 0;

                // Act
                var pixelData = _pngReader.ReadPixelData(stream, info);

                // Assert
                pixelData.Should().NotBeNull();
                pixelData.Length.Should().Be(4); // 2x2 pixels, 1 byte per pixel

                // Expected values: (0,0)=0, (1,0)=85, (0,1)=170, (1,1)=255
                // Row-major order: [0, 85, 170, 255]
                pixelData[0].Should().Be(0);   // Top-left: Black
                pixelData[1].Should().Be(85);  // Top-right: Dark gray
                pixelData[2].Should().Be(170); // Bottom-left: Light gray
                pixelData[3].Should().Be(255); // Bottom-right: White
            }
        }

        [Fact]
        public void ReadPixelDataByRow_GrayscalePng_InvokesCallbackForEachRow()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                var info = _pngReader.ReadImageInfo(stream);
                stream.Position = 0;

                int rowCount = 0;
                byte[][] rows = new byte[2][];

                // Act
                _pngReader.ReadPixelDataByRow(stream, info, (data, rowIndex) =>
                {
                    rows[rowIndex] = new byte[data.Length];
                    data.CopyTo(rows[rowIndex], 0);
                    rowCount++;
                });

                // Assert
                rowCount.Should().Be(2);

                // Row 0: [0, 85]
                rows[0][0].Should().Be(0);
                rows[0][1].Should().Be(85);

                // Row 1: [170, 255]
                rows[1][0].Should().Be(170);
                rows[1][1].Should().Be(255);
            }
        }

        [Fact]
        public void ValidateSignature_GrayscalePng_ReturnsTrue()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                bool result = _pngReader.ValidateSignature(stream);

                // Assert
                result.Should().BeTrue();
            }
        }
    }
}
