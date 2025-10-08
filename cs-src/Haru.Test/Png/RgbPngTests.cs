using System.IO;
using FluentAssertions;
using Haru.Png;
using Xunit;

namespace Haru.Test.Png
{
    /// <summary>
    /// Tests for truecolor RGB PNG images.
    /// Test file: 2x2 RGB PNG with pixels: (0,0)=Red, (1,0)=Green, (0,1)=Blue, (1,1)=White
    /// </summary>
    public class RgbPngTests
    {
        private readonly IPngReader _pngReader;
        private const string TestFile = "test_rgb_2x2.png";

        public RgbPngTests()
        {
            _pngReader = PngReaderFactory.Create();
        }

        [Fact]
        public void ReadImageInfo_RgbPng_ReturnsCorrectSize()
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
        public void ReadImageInfo_RgbPng_ReturnsCorrectColorType()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.ColorType.Should().Be(PngColorType.Rgb);
            }
        }

        [Fact]
        public void ReadImageInfo_RgbPng_ReturnsCorrectBitDepth()
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
        public void ReadImageInfo_RgbPng_ReturnsCorrectChannelCount()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.Channels.Should().Be(3);
            }
        }

        [Fact]
        public void ReadImageInfo_RgbPng_HasNoAlpha()
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
        public void ReadImageInfo_RgbPng_HasNoPalette()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.Palette.Should().BeNull("RGB images do not have palettes");
            }
        }

        [Fact]
        public void ReadPixelData_RgbPng_ReturnsCorrectPixelValues()
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
                pixelData.Length.Should().Be(12); // 2x2 pixels * 3 bytes per pixel

                // Expected pixels in row-major order:
                // (0,0) = Red (255, 0, 0)
                pixelData[0].Should().Be(255);
                pixelData[1].Should().Be(0);
                pixelData[2].Should().Be(0);

                // (1,0) = Green (0, 255, 0)
                pixelData[3].Should().Be(0);
                pixelData[4].Should().Be(255);
                pixelData[5].Should().Be(0);

                // (0,1) = Blue (0, 0, 255)
                pixelData[6].Should().Be(0);
                pixelData[7].Should().Be(0);
                pixelData[8].Should().Be(255);

                // (1,1) = White (255, 255, 255)
                pixelData[9].Should().Be(255);
                pixelData[10].Should().Be(255);
                pixelData[11].Should().Be(255);
            }
        }

        [Fact]
        public void ReadPixelDataByRow_RgbPng_InvokesCallbackForEachRow()
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

                // Row 0: Red (255,0,0), Green (0,255,0)
                rows[0].Length.Should().Be(6); // 2 pixels * 3 bytes
                rows[0][0].Should().Be(255); // Red R
                rows[0][1].Should().Be(0);   // Red G
                rows[0][2].Should().Be(0);   // Red B
                rows[0][3].Should().Be(0);   // Green R
                rows[0][4].Should().Be(255); // Green G
                rows[0][5].Should().Be(0);   // Green B

                // Row 1: Blue (0,0,255), White (255,255,255)
                rows[1].Length.Should().Be(6); // 2 pixels * 3 bytes
                rows[1][0].Should().Be(0);   // Blue R
                rows[1][1].Should().Be(0);   // Blue G
                rows[1][2].Should().Be(255); // Blue B
                rows[1][3].Should().Be(255); // White R
                rows[1][4].Should().Be(255); // White G
                rows[1][5].Should().Be(255); // White B
            }
        }

        [Fact]
        public void ValidateSignature_RgbPng_ReturnsTrue()
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
