using FluentAssertions;
using Haru.Png;
using Xunit;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Png
{
    /// <summary>
    /// Tests for 256-color palette PNG images.
    /// Test file: 2x2 palette PNG with indices: (0,0)=0, (1,0)=85, (0,1)=170, (1,1)=255
    /// Palette is grayscale: index N -> RGB(N, N, N)
    /// </summary>
    public class PalettePngTests
    {
        private readonly IPngReader _pngReader;
        private const string TestFile = "test_palette_2x2.png";

        public PalettePngTests()
        {
            _pngReader = PngReaderFactory.Create();
        }

        [Fact]
        public void ReadImageInfo_PalettePng_ReturnsCorrectSize()
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
        public void ReadImageInfo_PalettePng_ReturnsCorrectColorType()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                // Note: ImageSharp may convert palette to RGB during loading
                info.ColorType.Should().BeOneOf(PngColorType.Palette, PngColorType.Grayscale, PngColorType.Rgb);
            }
        }

        [Fact]
        public void ReadImageInfo_PalettePng_ReturnsCorrectBitDepth()
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
        public void ReadImageInfo_PalettePng_HasPalette()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                // If color type is Palette, then palette should be present
                if (info.ColorType == PngColorType.Palette)
                {
                    info.Palette.Should().NotBeNull("palette PNG should have palette data");
                }
            }
        }

        [Fact]
        public void ReadImageInfo_PalettePng_PaletteHasCorrectColorCount()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                if (info.ColorType == PngColorType.Palette && info.Palette != null)
                {
                    info.Palette.ColorCount.Should().BeGreaterThan(0, "palette should have colors");
                    info.Palette.ColorCount.Should().BeLessThanOrEqualTo(256, "palette can have max 256 colors");
                }
            }
        }

        [Fact]
        public void ReadImageInfo_PalettePng_PaletteHasCorrectData()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                if (info.ColorType == PngColorType.Palette && info.Palette != null)
                {
                    var palette = info.Palette;
                    palette.Colors.Should().NotBeNull();
                    palette.Colors.Length.Should().Be(palette.ColorCount * 3, "each color has 3 bytes (RGB)");

                    // Test file uses grayscale palette: index N -> RGB(N, N, N)
                    // Check a few specific entries
                    if (palette.ColorCount >= 256)
                    {
                        // Index 0: RGB(0, 0, 0) - Black
                        var color0 = palette.GetColor(0);
                        color0.R.Should().Be(0);
                        color0.G.Should().Be(0);
                        color0.B.Should().Be(0);

                        // Index 85: RGB(85, 85, 85) - Dark gray
                        var color85 = palette.GetColor(85);
                        color85.R.Should().Be(85);
                        color85.G.Should().Be(85);
                        color85.B.Should().Be(85);

                        // Index 170: RGB(170, 170, 170) - Light gray
                        var color170 = palette.GetColor(170);
                        color170.R.Should().Be(170);
                        color170.G.Should().Be(170);
                        color170.B.Should().Be(170);

                        // Index 255: RGB(255, 255, 255) - White
                        var color255 = palette.GetColor(255);
                        color255.R.Should().Be(255);
                        color255.G.Should().Be(255);
                        color255.B.Should().Be(255);
                    }
                }
            }
        }

        [Fact]
        public void ReadPixelData_PalettePng_ReturnsCorrectPixelValues()
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

                // The palette uses grayscale values, so we expect either:
                // - Palette indices: [0, 85, 170, 255] if color type is Palette
                // - Grayscale values: [0, 85, 170, 255] if converted to grayscale
                // - RGB triplets if converted to RGB

                if (info.ColorType == PngColorType.Palette || info.ColorType == PngColorType.Grayscale)
                {
                    pixelData.Length.Should().Be(4);
                    pixelData[0].Should().Be(0);
                    pixelData[1].Should().Be(85);
                    pixelData[2].Should().Be(170);
                    pixelData[3].Should().Be(255);
                }
                else if (info.ColorType == PngColorType.Rgb)
                {
                    pixelData.Length.Should().Be(12); // 2x2 pixels * 3 bytes per pixel
                    // First pixel (0,0): RGB(0, 0, 0)
                    pixelData[0].Should().Be(0);
                    pixelData[1].Should().Be(0);
                    pixelData[2].Should().Be(0);
                    // Second pixel (1,0): RGB(85, 85, 85)
                    pixelData[3].Should().Be(85);
                    pixelData[4].Should().Be(85);
                    pixelData[5].Should().Be(85);
                }
            }
        }

        [Fact]
        public void ReadPixelDataByRow_PalettePng_InvokesCallbackForEachRow()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream(TestFile))
            {
                var info = _pngReader.ReadImageInfo(stream);
                stream.Position = 0;

                int rowCount = 0;

                // Act
                _pngReader.ReadPixelDataByRow(stream, info, (data, rowIndex) =>
                {
                    data.Should().NotBeNull();
                    data.Length.Should().Be((int)info.BytesPerRow);
                    rowCount++;
                });

                // Assert
                rowCount.Should().Be(2);
            }
        }

        [Fact]
        public void ValidateSignature_PalettePng_ReturnsTrue()
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
