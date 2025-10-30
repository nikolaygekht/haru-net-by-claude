using FluentAssertions;
using Haru.Png;
using Xunit;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Png
{
    /// <summary>
    /// Tests for PNG images with transparency.
    /// </summary>
    public class TransparencyPngTests
    {
        private readonly IPngReader _pngReader;

        public TransparencyPngTests()
        {
            _pngReader = PngReaderFactory.Create();
        }

        #region RGBA Tests
        // Test file: 2x2 RGBA PNG
        // (0,0) = Red, opaque (255,0,0,255)
        // (1,0) = Green, semi-transparent (0,255,0,170)
        // (0,1) = Blue, more transparent (0,0,255,85)
        // (1,1) = White, fully transparent (255,255,255,0)

        [Fact]
        public void ReadImageInfo_RgbaPng_ReturnsCorrectSize()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_rgba_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.Width.Should().Be(2);
                info.Height.Should().Be(2);
            }
        }

        [Fact]
        public void ReadImageInfo_RgbaPng_ReturnsCorrectColorType()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_rgba_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.ColorType.Should().Be(PngColorType.RgbAlpha);
            }
        }

        [Fact]
        public void ReadImageInfo_RgbaPng_HasAlpha()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_rgba_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.HasAlpha.Should().BeTrue();
            }
        }

        [Fact]
        public void ReadImageInfo_RgbaPng_ReturnsCorrectChannelCount()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_rgba_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.Channels.Should().Be(4);
            }
        }

        [Fact]
        public void ReadPixelData_RgbaPng_ReturnsCorrectPixelValues()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_rgba_2x2.png"))
            {
                var info = _pngReader.ReadImageInfo(stream);
                stream.Position = 0;

                // Act
                var pixelData = _pngReader.ReadPixelData(stream, info);

                // Assert
                pixelData.Should().NotBeNull();
                pixelData.Length.Should().Be(16); // 2x2 pixels * 4 bytes per pixel

                // (0,0) = Red, opaque (255,0,0,255)
                pixelData[0].Should().Be(255);
                pixelData[1].Should().Be(0);
                pixelData[2].Should().Be(0);
                pixelData[3].Should().Be(255);

                // (1,0) = Green, semi-transparent (0,255,0,170)
                pixelData[4].Should().Be(0);
                pixelData[5].Should().Be(255);
                pixelData[6].Should().Be(0);
                pixelData[7].Should().Be(170);

                // (0,1) = Blue, more transparent (0,0,255,85)
                pixelData[8].Should().Be(0);
                pixelData[9].Should().Be(0);
                pixelData[10].Should().Be(255);
                pixelData[11].Should().Be(85);

                // (1,1) = White, fully transparent (255,255,255,0)
                pixelData[12].Should().Be(255);
                pixelData[13].Should().Be(255);
                pixelData[14].Should().Be(255);
                pixelData[15].Should().Be(0);
            }
        }

        #endregion

        #region Grayscale Alpha Tests
        // Test file: 2x2 Grayscale+Alpha PNG
        // (0,0) = Black, opaque (0,255)
        // (1,0) = Dark gray, semi-transparent (85,170)
        // (0,1) = Light gray, more transparent (170,85)
        // (1,1) = White, fully transparent (255,0)

        [Fact]
        public void ReadImageInfo_GrayscaleAlphaPng_ReturnsCorrectSize()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_grayscale_alpha_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.Width.Should().Be(2);
                info.Height.Should().Be(2);
            }
        }

        [Fact]
        public void ReadImageInfo_GrayscaleAlphaPng_ReturnsCorrectColorType()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_grayscale_alpha_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.ColorType.Should().Be(PngColorType.GrayscaleAlpha);
            }
        }

        [Fact]
        public void ReadImageInfo_GrayscaleAlphaPng_HasAlpha()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_grayscale_alpha_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.HasAlpha.Should().BeTrue();
            }
        }

        [Fact]
        public void ReadImageInfo_GrayscaleAlphaPng_ReturnsCorrectChannelCount()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_grayscale_alpha_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.Channels.Should().Be(2);
            }
        }

        [Fact]
        public void ReadPixelData_GrayscaleAlphaPng_ReturnsCorrectPixelValues()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_grayscale_alpha_2x2.png"))
            {
                var info = _pngReader.ReadImageInfo(stream);
                stream.Position = 0;

                // Act
                var pixelData = _pngReader.ReadPixelData(stream, info);

                // Assert
                pixelData.Should().NotBeNull();
                pixelData.Length.Should().Be(8); // 2x2 pixels * 2 bytes per pixel

                // (0,0) = Black, opaque (0,255)
                pixelData[0].Should().Be(0);
                pixelData[1].Should().Be(255);

                // (1,0) = Dark gray, semi-transparent (85,170)
                pixelData[2].Should().Be(85);
                pixelData[3].Should().Be(170);

                // (0,1) = Light gray, more transparent (170,85)
                pixelData[4].Should().Be(170);
                pixelData[5].Should().Be(85);

                // (1,1) = White, fully transparent (255,0)
                pixelData[6].Should().Be(255);
                pixelData[7].Should().Be(0);
            }
        }

        #endregion

        #region Palette with Transparency Tests
        // Test file: 2x2 Palette PNG with tRNS chunk
        // (0,0) = Index 0: Red with alpha 255 (opaque)
        // (1,0) = Index 1: Green with alpha 170 (semi-transparent)
        // (0,1) = Index 2: Blue with alpha 85 (more transparent)
        // (1,1) = Index 3: White with alpha 0 (fully transparent)

        [Fact]
        public void ReadImageInfo_PaletteTransPng_ReturnsCorrectSize()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_palette_trans_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.Width.Should().Be(2);
                info.Height.Should().Be(2);
            }
        }

        [Fact]
        public void ReadImageInfo_PaletteTransPng_HasTransparency()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_palette_trans_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                info.HasAlpha.Should().BeTrue("palette with tRNS chunk has transparency");
            }
        }

        [Fact]
        public void ReadImageInfo_PaletteTransPng_TransparencyDataIsPresent()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_palette_trans_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                if (info.ColorType == PngColorType.Palette)
                {
                    info.Transparency.Should().NotBeNull("palette PNG with tRNS should have transparency data");
                    if (info.Transparency != null)
                    {
                        info.Transparency.HasTransparency.Should().BeTrue();
                        info.Transparency.PaletteAlpha.Should().NotBeNull();
                        info.Transparency.TransparentColorCount.Should().BeGreaterThan(0);
                    }
                }
            }
        }

        [Fact]
        public void ReadImageInfo_PaletteTransPng_TransparencyAlphaValues()
        {
            // Arrange
            using (var stream = TestHelper.GetResourceStream("test_palette_trans_2x2.png"))
            {
                // Act
                var info = _pngReader.ReadImageInfo(stream);

                // Assert
                if (info.ColorType == PngColorType.Palette && info.Transparency != null)
                {
                    var trans = info.Transparency;
                    trans!.TransparentColorCount.Should().BeGreaterThanOrEqualTo(4);

                    // Check alpha values for palette entries 0-3
                    trans.PaletteAlpha![0].Should().Be(255, "Index 0 is opaque");
                    trans.PaletteAlpha![1].Should().Be(170, "Index 1 is semi-transparent");
                    trans.PaletteAlpha![2].Should().Be(85, "Index 2 is more transparent");
                    trans.PaletteAlpha![3].Should().Be(0, "Index 3 is fully transparent");
                }
            }
        }

        [Fact]
        public void ValidateSignature_TransparentPngs_ReturnsTrue()
        {
            // Arrange & Act & Assert
            using (var stream1 = TestHelper.GetResourceStream("test_rgba_2x2.png"))
            {
                _pngReader.ValidateSignature(stream1).Should().BeTrue();
            }

            using (var stream2 = TestHelper.GetResourceStream("test_grayscale_alpha_2x2.png"))
            {
                _pngReader.ValidateSignature(stream2).Should().BeTrue();
            }

            using (var stream3 = TestHelper.GetResourceStream("test_palette_trans_2x2.png"))
            {
                _pngReader.ValidateSignature(stream3).Should().BeTrue();
            }
        }

        #endregion
    }
}
