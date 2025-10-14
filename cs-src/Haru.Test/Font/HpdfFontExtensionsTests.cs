/*
 * << Haru Free PDF Library >> -- HpdfFontExtensionsTests.cs
 *
 * Unit tests for HpdfFontExtensions
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 */

using Xunit;
using FluentAssertions;
using Haru.Font;
using Haru.Doc;

namespace Haru.Test.Font
{
    public class HpdfFontExtensionsTests
    {
        [Fact]
        public void MeasureText_WithWordWrap_BreaksAtWhitespace()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "Hello World Test";
            float fontSize = 12f;
            float width = 50f;  // Small width to force wrapping

            // Act
            int charCount = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: true, out float realWidth);

            // Assert
            charCount.Should().BeGreaterThan(0);
            charCount.Should().BeLessThan(text.Length);
            realWidth.Should().BeLessOrEqualTo(width);

            // Should break at a space character
            if (charCount > 0 && charCount < text.Length)
            {
                char breakChar = text[charCount - 1];
                (breakChar == ' ' || breakChar == '\t' || breakChar == '\n').Should().BeTrue();
            }
        }

        [Fact]
        public void MeasureText_WithoutWordWrap_BreaksAnywhere()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "HelloWorld";  // No spaces
            float fontSize = 12f;
            float width = 40f;

            // Act
            int charCount = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: false, out float realWidth);

            // Assert
            charCount.Should().BeGreaterThan(0);
            charCount.Should().BeLessThan(text.Length);
            realWidth.Should().BeLessOrEqualTo(width);
        }

        [Fact]
        public void MeasureText_AllTextFits_ReturnsFullLength()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "Short";
            float fontSize = 12f;
            float width = 1000f;  // Large width

            // Act
            int charCount = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: true, out float realWidth);

            // Assert
            charCount.Should().Be(text.Length);
            realWidth.Should().BeLessOrEqualTo(width);
        }

        [Fact]
        public void MeasureText_EmptyString_ReturnsZero()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "";
            float fontSize = 12f;
            float width = 100f;

            // Act
            int charCount = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: true, out float realWidth);

            // Assert
            charCount.Should().Be(0);
            realWidth.Should().Be(0);
        }

        [Fact]
        public void MeasureText_NullString_ReturnsZero()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = null;
            float fontSize = 12f;
            float width = 100f;

            // Act
            int charCount = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: true, out float realWidth);

            // Assert
            charCount.Should().Be(0);
            realWidth.Should().Be(0);
        }

        [Fact]
        public void MeasureText_WithLineFeed_BreaksAtLineFeed()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "Line1\nLine2";
            float fontSize = 12f;
            float width = 1000f;  // Large width

            // Act
            int charCount = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: true, out float realWidth);

            // Assert
            // Should stop at the line feed (position 6 = after "Line1\n")
            charCount.Should().Be(6);
            text.Substring(0, charCount).Should().EndWith("\n");
        }

        [Fact]
        public void MeasureText_WithCharSpacing_IncludesCharSpacing()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "Test";
            float fontSize = 12f;
            float width = 1000f;
            float charSpace = 5f;

            // Act
            int charCount1 = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: false, out float realWidth1);
            int charCount2 = font.MeasureText(text, fontSize, width, charSpace, 0f, wordWrap: false, out float realWidth2);

            // Assert
            charCount1.Should().Be(text.Length);
            charCount2.Should().Be(text.Length);
            // Width with character spacing should be larger
            realWidth2.Should().BeGreaterThan(realWidth1);
        }

        [Fact]
        public void MeasureText_WithWordSpacing_IncludesWordSpacing()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "Hello World";
            float fontSize = 12f;
            float width = 1000f;
            float wordSpace = 10f;

            // Act
            int charCount1 = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: true, out float realWidth1);
            int charCount2 = font.MeasureText(text, fontSize, width, 0f, wordSpace, wordWrap: true, out float realWidth2);

            // Assert
            charCount1.Should().Be(text.Length);
            charCount2.Should().Be(text.Length);
            // Width with word spacing should be larger
            realWidth2.Should().BeGreaterThan(realWidth1);
        }

        [Fact]
        public void MeasureText_MultipleSpaces_HandlesCorrectly()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "Hello  World";  // Two spaces
            float fontSize = 12f;
            float width = 1000f;

            // Act
            int charCount = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: true, out float realWidth);

            // Assert
            charCount.Should().Be(text.Length);
            realWidth.Should().BeGreaterThan(0);
        }

        [Fact]
        public void MeasureText_VerySmallWidth_ReturnsZeroOrMinimal()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "Test";
            float fontSize = 12f;
            float width = 1f;  // Very small width

            // Act
            int charCount = font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: false, out float realWidth);

            // Assert
            // Should fit at least 0 characters, maybe 1 if it's very narrow
            charCount.Should().BeInRange(0, 1);
            realWidth.Should().BeLessOrEqualTo(width);
        }

        [Fact]
        public void MeasureText_DifferentFonts_ProducesDifferentResults()
        {
            // Arrange
            var document = new HpdfDocument();
            var helvetica = document.GetFont("Helvetica");
            var courier = document.GetFont("Courier");
            string text = "Test Text";
            float fontSize = 12f;
            float width = 1000f;

            // Act
            helvetica.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: false, out float helveticaWidth);
            courier.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: false, out float courierWidth);

            // Assert
            // Courier is monospaced, Helvetica is proportional - widths should differ
            helveticaWidth.Should().NotBe(courierWidth);
        }

        [Fact]
        public void MeasureText_RealWidth_MatchesBasicMeasureText()
        {
            // Arrange
            var document = new HpdfDocument();
            var font = document.GetFont("Helvetica");
            string text = "Test";
            float fontSize = 12f;
            float width = 1000f;

            // Act
            font.MeasureText(text, fontSize, width, 0f, 0f, wordWrap: false, out float realWidthFromExtension);
            float basicWidth = font.MeasureText(text, fontSize);

            // Assert
            // When all text fits, realWidth should match basic MeasureText
            realWidthFromExtension.Should().BeApproximately(basicWidth, 0.1f);
        }
    }
}
