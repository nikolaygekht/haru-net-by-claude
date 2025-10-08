using FluentAssertions;
using Haru.Types;
using Xunit;

namespace Haru.Test.Types
{
    public class HpdfColorTests
    {
        [Fact]
        public void RgbColor_Constructor_SetsValues()
        {
            var color = new HpdfRgbColor(0.5f, 0.6f, 0.7f);

            color.R.Should().Be(0.5f);
            color.G.Should().Be(0.6f);
            color.B.Should().Be(0.7f);
        }

        [Fact]
        public void RgbColor_Constructor_ClampsValues()
        {
            var color = new HpdfRgbColor(1.5f, -0.5f, 0.5f);

            color.R.Should().Be(1.0f); // Clamped to 1.0
            color.G.Should().Be(0.0f); // Clamped to 0.0
            color.B.Should().Be(0.5f);
        }

        [Fact]
        public void RgbColor_FromBytes_ConvertsCorrectly()
        {
            var color = HpdfRgbColor.FromBytes(255, 128, 0);

            color.R.Should().BeApproximately(1.0f, 0.01f);
            color.G.Should().BeApproximately(0.502f, 0.01f);
            color.B.Should().Be(0.0f);
        }

        [Fact]
        public void RgbColor_CommonColors_HaveCorrectValues()
        {
            HpdfRgbColor.Black.Should().Be(new HpdfRgbColor(0, 0, 0));
            HpdfRgbColor.White.Should().Be(new HpdfRgbColor(1, 1, 1));
            HpdfRgbColor.Red.Should().Be(new HpdfRgbColor(1, 0, 0));
            HpdfRgbColor.Green.Should().Be(new HpdfRgbColor(0, 1, 0));
            HpdfRgbColor.Blue.Should().Be(new HpdfRgbColor(0, 0, 1));
        }

        [Fact]
        public void CmykColor_Constructor_SetsValues()
        {
            var color = new HpdfCmykColor(0.1f, 0.2f, 0.3f, 0.4f);

            color.C.Should().Be(0.1f);
            color.M.Should().Be(0.2f);
            color.Y.Should().Be(0.3f);
            color.K.Should().Be(0.4f);
        }

        [Fact]
        public void CmykColor_Constructor_ClampsValues()
        {
            var color = new HpdfCmykColor(1.5f, -0.5f, 0.5f, 2.0f);

            color.C.Should().Be(1.0f); // Clamped to 1.0
            color.M.Should().Be(0.0f); // Clamped to 0.0
            color.Y.Should().Be(0.5f);
            color.K.Should().Be(1.0f); // Clamped to 1.0
        }

        [Fact]
        public void CmykColor_CommonColors_HaveCorrectValues()
        {
            HpdfCmykColor.Black.Should().Be(new HpdfCmykColor(0, 0, 0, 1));
            HpdfCmykColor.White.Should().Be(new HpdfCmykColor(0, 0, 0, 0));
        }
    }
}
