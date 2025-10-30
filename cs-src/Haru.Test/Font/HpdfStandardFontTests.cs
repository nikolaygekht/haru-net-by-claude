using Xunit;
using FluentAssertions;
using Haru.Font;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Font
{
    public class HpdfStandardFontTests
    {
        [Theory]
        [InlineData(HpdfStandardFont.Courier, "Courier")]
        [InlineData(HpdfStandardFont.CourierBold, "Courier-Bold")]
        [InlineData(HpdfStandardFont.CourierOblique, "Courier-Oblique")]
        [InlineData(HpdfStandardFont.CourierBoldOblique, "Courier-BoldOblique")]
        [InlineData(HpdfStandardFont.Helvetica, "Helvetica")]
        [InlineData(HpdfStandardFont.HelveticaBold, "Helvetica-Bold")]
        [InlineData(HpdfStandardFont.HelveticaOblique, "Helvetica-Oblique")]
        [InlineData(HpdfStandardFont.HelveticaBoldOblique, "Helvetica-BoldOblique")]
        [InlineData(HpdfStandardFont.TimesRoman, "Times-Roman")]
        [InlineData(HpdfStandardFont.TimesBold, "Times-Bold")]
        [InlineData(HpdfStandardFont.TimesItalic, "Times-Italic")]
        [InlineData(HpdfStandardFont.TimesBoldItalic, "Times-BoldItalic")]
        [InlineData(HpdfStandardFont.Symbol, "Symbol")]
        [InlineData(HpdfStandardFont.ZapfDingbats, "ZapfDingbats")]
        public void GetPostScriptName_ReturnsCorrectName(HpdfStandardFont font, string expected)
        {
            // Act
            var name = font.GetPostScriptName();

            // Assert
            name.Should().Be(expected);
        }

        [Fact]
        public void StandardFontEnum_HasExactly14Values()
        {
            // Arrange & Act
            var values = System.Enum.GetValues(typeof(HpdfStandardFont));

            // Assert
            values.Length.Should().Be(14, "PDF spec defines exactly 14 standard fonts");
        }
    }
}
