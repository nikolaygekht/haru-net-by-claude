using System;
using Xunit;
using FluentAssertions;
using Haru.Font;
using Haru.Xref;
using Haru.Objects;

namespace Haru.Test.Font
{
    public class HpdfFontTests
    {
        [Fact]
        public void Constructor_CreatesValidFont()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Assert
            font.Should().NotBeNull();
            font.Dict.Should().NotBeNull();
            font.BaseFont.Should().Be("Helvetica");
            font.LocalName.Should().Be("F1");
        }

        [Fact]
        public void Constructor_SetsCorrectDictionaryEntries()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var font = new HpdfFont(xref, HpdfStandardFont.TimesRoman, "F2");

            // Assert
            font.Dict.Should().ContainKey("Type");
            font.Dict.Should().ContainKey("Subtype");
            font.Dict.Should().ContainKey("BaseFont");

            (font.Dict["Type"] as HpdfName)?.Value.Should().Be("Font");
            (font.Dict["Subtype"] as HpdfName)?.Value.Should().Be("Type1");
            (font.Dict["BaseFont"] as HpdfName)?.Value.Should().Be("Times-Roman");
        }

        [Fact]
        public void Constructor_AddsWinAnsiEncodingForStandardFonts()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var font = new HpdfFont(xref, HpdfStandardFont.Courier, "F1");

            // Assert
            font.Dict.Should().ContainKey("Encoding");
            (font.Dict["Encoding"] as HpdfName)?.Value.Should().Be("WinAnsiEncoding");
        }

        [Fact]
        public void Constructor_DoesNotAddEncodingForSymbol()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var font = new HpdfFont(xref, HpdfStandardFont.Symbol, "F1");

            // Assert
            font.Dict.Should().NotContainKey("Encoding", "Symbol uses built-in encoding");
        }

        [Fact]
        public void Constructor_DoesNotAddEncodingForZapfDingbats()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var font = new HpdfFont(xref, HpdfStandardFont.ZapfDingbats, "F1");

            // Assert
            font.Dict.Should().NotContainKey("Encoding", "ZapfDingbats uses built-in encoding");
        }

        [Fact]
        public void Constructor_ThrowsWhenXrefIsNull()
        {
            // Act
            Action act = () => new HpdfFont(null, HpdfStandardFont.Helvetica, "F1");

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Xref cannot be null*");
        }

        [Fact]
        public void Constructor_ThrowsWhenLocalNameIsNull()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            Action act = () => new HpdfFont(xref, HpdfStandardFont.Helvetica, null);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Local name cannot be null or empty*");
        }

        [Fact]
        public void Constructor_ThrowsWhenLocalNameIsEmpty()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            Action act = () => new HpdfFont(xref, HpdfStandardFont.Helvetica, "");

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Local name cannot be null or empty*");
        }

        [Fact]
        public void GetCharWidth_ReturnsPositiveValue()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var width = font.GetCharWidth((byte)'A');

            // Assert
            width.Should().BeGreaterThan(0);
        }

        [Fact]
        public void MeasureText_EmptyString_ReturnsZero()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var width = font.MeasureText("", 12);

            // Assert
            width.Should().Be(0);
        }

        [Fact]
        public void MeasureText_NullString_ReturnsZero()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var width = font.MeasureText(null, 12);

            // Assert
            width.Should().Be(0);
        }

        [Fact]
        public void MeasureText_ReturnsPositiveWidth()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var width = font.MeasureText("Hello", 12);

            // Assert
            width.Should().BeGreaterThan(0);
        }

        [Fact]
        public void MeasureText_LargerFontSize_ReturnsLargerWidth()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Act
            var width10 = font.MeasureText("Test", 10);
            var width20 = font.MeasureText("Test", 20);

            // Assert
            width20.Should().BeGreaterThan(width10);
        }

        [Fact]
        public void Constructor_AddsFontToXref()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var initialCount = xref.Entries.Count;

            // Act
            var font = new HpdfFont(xref, HpdfStandardFont.Helvetica, "F1");

            // Assert
            xref.Entries.Count.Should().BeGreaterThan(initialCount);
            font.Dict.IsIndirect.Should().BeTrue();
            font.Dict.ObjectId.Should().BeGreaterThan(0);
        }
    }
}
