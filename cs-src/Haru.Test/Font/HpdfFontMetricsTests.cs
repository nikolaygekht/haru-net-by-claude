using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Font;


namespace Haru.Test.Font
{
    /// <summary>
    /// Unit tests for font metrics methods (GetAscent, GetDescent, GetXHeight, GetBBox).
    /// </summary>
    public class HpdfFontMetricsTests
    {
        [Theory]
        [InlineData(HpdfStandardFont.Helvetica, 718, -207, 523)]
        [InlineData(HpdfStandardFont.HelveticaBold, 718, -207, 532)]
        [InlineData(HpdfStandardFont.TimesRoman, 683, -217, 450)]
        [InlineData(HpdfStandardFont.TimesBold, 683, -217, 461)]
        [InlineData(HpdfStandardFont.Courier, 629, -157, 426)]
        [InlineData(HpdfStandardFont.CourierBold, 626, -142, 439)]
        public void StandardFont_ShouldReturnCorrectMetrics(
            HpdfStandardFont fontType,
            int expectedAscent,
            int expectedDescent,
            int expectedXHeight)
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, fontType, "F1");

            // Act
            var ascent = font.GetAscent();
            var descent = font.GetDescent();
            var xHeight = font.GetXHeight();

            // Assert
            ascent.Should().Be(expectedAscent, "ascent should match standard font metrics");
            descent.Should().Be(expectedDescent, "descent should match standard font metrics");
            xHeight.Should().Be(expectedXHeight, "x-height should match standard font metrics");
        }

        [Fact]
        public void StandardFont_Helvetica_ShouldReturnCorrectBBox()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var bbox = font.GetBBox();

            // Assert
            bbox.Should().NotBeNull();
            bbox.Left.Should().Be(-166);
            bbox.Bottom.Should().Be(-225);
            bbox.Right.Should().Be(1000);
            bbox.Top.Should().Be(931);
        }

        [Fact]
        public void StandardFont_TimesRoman_ShouldReturnCorrectBBox()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.TimesRoman, "F1");

            // Act
            var bbox = font.GetBBox();

            // Assert
            bbox.Should().NotBeNull();
            bbox.Left.Should().Be(-168);
            bbox.Bottom.Should().Be(-218);
            bbox.Right.Should().Be(1000);
            bbox.Top.Should().Be(898);
        }

        [Fact]
        public void StandardFont_Courier_ShouldReturnCorrectBBox()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Courier, "F1");

            // Act
            var bbox = font.GetBBox();

            // Assert
            bbox.Should().NotBeNull();
            bbox.Left.Should().Be(-23);
            bbox.Bottom.Should().Be(-250);
            bbox.Right.Should().Be(715);
            bbox.Top.Should().Be(805);
        }

        [Theory]
        [InlineData(HpdfStandardFont.Symbol, 1010, -293, 478)]
        [InlineData(HpdfStandardFont.ZapfDingbats, 820, -143, 0)]
        public void SpecialFont_ShouldReturnCorrectMetrics(
            HpdfStandardFont fontType,
            int expectedAscent,
            int expectedDescent,
            int expectedXHeight)
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, fontType, "F1");

            // Act
            var ascent = font.GetAscent();
            var descent = font.GetDescent();
            var xHeight = font.GetXHeight();

            // Assert
            ascent.Should().Be(expectedAscent);
            descent.Should().Be(expectedDescent);
            xHeight.Should().Be(expectedXHeight);
        }

        [Fact]
        public void GetAscent_ShouldReturnPositiveValue()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var ascent = font.GetAscent();

            // Assert
            ascent.Should().BePositive("ascent is the height above baseline");
        }

        [Fact]
        public void GetDescent_ShouldReturnNegativeValue()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var descent = font.GetDescent();

            // Assert
            descent.Should().BeNegative("descent is the depth below baseline");
        }

        [Fact]
        public void GetXHeight_ShouldReturnPositiveValue()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var xHeight = font.GetXHeight();

            // Assert
            xHeight.Should().BePositive("x-height is the height of lowercase 'x'");
        }

        [Fact]
        public void GetBBox_ShouldReturnValidBox()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var bbox = font.GetBBox();

            // Assert
            bbox.Should().NotBeNull();
            bbox.Right.Should().BeGreaterThan(bbox.Left, "right should be greater than left");
            bbox.Top.Should().BeGreaterThan(bbox.Bottom, "top should be greater than bottom");
        }

        [Fact]
        public void FontMetrics_ShouldBeConsistentWithBBox()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var ascent = font.GetAscent();
            var descent = font.GetDescent();
            var bbox = font.GetBBox();

            // Assert - ascent should be <= bbox top
            ascent.Should().BeLessOrEqualTo((int)bbox.Top,
                "ascent should not exceed bounding box top");

            // descent should be >= bbox bottom (both negative, so descent closer to 0)
            descent.Should().BeGreaterOrEqualTo((int)bbox.Bottom,
                "descent should not go below bounding box bottom");
        }

        [Fact]
        public void MeasureText_WithEmptyString_ShouldReturnZero()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var width = font.MeasureText("", 12);

            // Assert
            width.Should().Be(0);
        }

        [Fact]
        public void MeasureText_WithNullString_ShouldReturnZero()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var width = font.MeasureText(null!, 12);

            // Assert
            width.Should().Be(0);
        }

        [Fact]
        public void MeasureText_WithValidText_ShouldReturnPositiveWidth()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var width = font.MeasureText("Hello World", 12);

            // Assert
            width.Should().BeGreaterThan(0, "text should have positive width");
        }

        [Fact]
        public void MeasureText_LargerFontSize_ShouldReturnLargerWidth()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");
            var text = "Hello World";

            // Act
            var width12 = font.MeasureText(text, 12);
            var width24 = font.MeasureText(text, 24);

            // Assert
            width24.Should().BeGreaterThan(width12, "larger font size should produce wider text");

            // Should be approximately 2x (allowing for rounding)
            var ratio = width24 / width12;
            ratio.Should().BeApproximately(2.0f, 0.1f, "width should scale linearly with font size");
        }

        [Fact]
        public void MeasureText_LongerText_ShouldReturnLargerWidth()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var widthShort = font.MeasureText("Hello", 12);
            var widthLong = font.MeasureText("Hello World", 12);

            // Assert
            widthLong.Should().BeGreaterThan(widthShort, "longer text should have greater width");
        }

        [Theory]
        [InlineData(HpdfStandardFont.Helvetica)]
        [InlineData(HpdfStandardFont.TimesRoman)]
        [InlineData(HpdfStandardFont.Courier)]
        public void AllStandardFonts_ShouldSupportMetricsMethods(HpdfStandardFont fontType)
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, fontType, "F1");

            // Act & Assert - should not throw
            var ascent = font.GetAscent();
            var descent = font.GetDescent();
            var xHeight = font.GetXHeight();
            var bbox = font.GetBBox();
            var width = font.MeasureText("Test", 12);

            // Verify reasonable values
            ascent.Should().BePositive();
            descent.Should().BeNegative();
            xHeight.Should().BePositive();
            bbox.Should().NotBeNull();
            width.Should().BePositive();
        }

        [Fact]
        public void FontMetrics_ShouldBe_InThousandUnitGlyphSpace()
        {
            // Arrange
            var doc = new HpdfDocument();
            var font = new HpdfFont(doc.Xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var ascent = font.GetAscent();
            var descent = font.GetDescent();

            // Assert - values should be in typical 1000-unit range
            // Most Type 1 fonts have ascent around 700-900
            ascent.Should().BeInRange(500, 1200, "ascent should be in typical 1000-unit range");

            // Most Type 1 fonts have descent around -150 to -300
            descent.Should().BeInRange(-400, -50, "descent should be in typical 1000-unit range");
        }

        [Fact]
        public void GetBBox_AllStandardFonts_ShouldHaveValidDimensions()
        {
            // Test that all standard fonts return valid bounding boxes
            var doc = new HpdfDocument();
            var standardFonts = new[]
            {
                HpdfStandardFont.Helvetica,
                HpdfStandardFont.HelveticaBold,
                HpdfStandardFont.HelveticaOblique,
                HpdfStandardFont.HelveticaBoldOblique,
                HpdfStandardFont.TimesRoman,
                HpdfStandardFont.TimesBold,
                HpdfStandardFont.TimesItalic,
                HpdfStandardFont.TimesBoldItalic,
                HpdfStandardFont.Courier,
                HpdfStandardFont.CourierBold,
                HpdfStandardFont.CourierOblique,
                HpdfStandardFont.CourierBoldOblique,
                HpdfStandardFont.Symbol,
                HpdfStandardFont.ZapfDingbats
            };

            foreach (var standardFont in standardFonts)
            {
                // Arrange
                var font = new HpdfFont(doc.Xref, standardFont, $"F{(int)standardFont}");

                // Act
                var bbox = font.GetBBox();

                // Assert
                bbox.Should().NotBeNull($"{standardFont} should have a bounding box");
                bbox.Right.Should().BeGreaterThan(bbox.Left, $"{standardFont} bbox right > left");
                bbox.Top.Should().BeGreaterThan(bbox.Bottom, $"{standardFont} bbox top > bottom");
            }
        }
    }
}
