using System;
using System.Text;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Xref;
using Haru.Types;
using Haru.Objects;

namespace Haru.Test.Doc
{
    public class HpdfExtGStateTests
    {
        [Fact]
        public void Constructor_CreatesValidExtGState()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var extGState = new HpdfExtGState(xref, "GS1");

            // Assert
            extGState.Should().NotBeNull();
            extGState.Dict.Should().NotBeNull();
            extGState.LocalName.Should().Be("GS1");
        }

        [Fact]
        public void Constructor_SetsTypeEntry()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var extGState = new HpdfExtGState(xref, "GS1");

            // Assert
            extGState.Dict.Should().ContainKey("Type");
            (extGState.Dict["Type"] as HpdfName)?.Value.Should().Be("ExtGState");
        }

        [Fact]
        public void Constructor_ThrowsWhenXrefIsNull()
        {
            // Act
            Action act = () => new HpdfExtGState(null, "GS1");

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
            Action act = () => new HpdfExtGState(xref, null);

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
            Action act = () => new HpdfExtGState(xref, "");

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Local name cannot be null or empty*");
        }

        // Alpha Stroke Tests

        [Fact]
        public void SetAlphaStroke_SetsCAEntry()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            extGState.SetAlphaStroke(0.5f);

            // Assert
            extGState.Dict.Should().ContainKey("CA");
            (extGState.Dict["CA"] as HpdfReal)?.Value.Should().BeApproximately(0.5f, 0.001f);
        }

        [Fact]
        public void SetAlphaStroke_ThrowsWhenAlphaIsNegative()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            Action act = () => extGState.SetAlphaStroke(-0.1f);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Alpha must be 0-1*");
        }

        [Fact]
        public void SetAlphaStroke_ThrowsWhenAlphaIsGreaterThanOne()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            Action act = () => extGState.SetAlphaStroke(1.5f);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Alpha must be 0-1*");
        }

        [Fact]
        public void SetAlphaStroke_AcceptsZero()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            extGState.SetAlphaStroke(0f);

            // Assert
            (extGState.Dict["CA"] as HpdfReal)?.Value.Should().Be(0f);
        }

        [Fact]
        public void SetAlphaStroke_AcceptsOne()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            extGState.SetAlphaStroke(1f);

            // Assert
            (extGState.Dict["CA"] as HpdfReal)?.Value.Should().Be(1f);
        }

        // Alpha Fill Tests

        [Fact]
        public void SetAlphaFill_SetsCaEntry()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            extGState.SetAlphaFill(0.7f);

            // Assert
            extGState.Dict.Should().ContainKey("ca");
            (extGState.Dict["ca"] as HpdfReal)?.Value.Should().BeApproximately(0.7f, 0.001f);
        }

        [Fact]
        public void SetAlphaFill_ThrowsWhenAlphaIsNegative()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            Action act = () => extGState.SetAlphaFill(-0.1f);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Alpha must be 0-1*");
        }

        [Fact]
        public void SetAlphaFill_ThrowsWhenAlphaIsGreaterThanOne()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            Action act = () => extGState.SetAlphaFill(1.1f);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Alpha must be 0-1*");
        }

        // Blend Mode Tests

        [Fact]
        public void SetBlendMode_SetsBMEntry()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            extGState.SetBlendMode(HpdfBlendMode.Multiply);

            // Assert
            extGState.Dict.Should().ContainKey("BM");
            (extGState.Dict["BM"] as HpdfName)?.Value.Should().Be("Multiply");
        }

        [Theory]
        [InlineData(HpdfBlendMode.Normal, "Normal")]
        [InlineData(HpdfBlendMode.Multiply, "Multiply")]
        [InlineData(HpdfBlendMode.Screen, "Screen")]
        [InlineData(HpdfBlendMode.Overlay, "Overlay")]
        [InlineData(HpdfBlendMode.Darken, "Darken")]
        [InlineData(HpdfBlendMode.Lighten, "Lighten")]
        public void SetBlendMode_AllModes_SetCorrectly(HpdfBlendMode mode, string expectedName)
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            extGState.SetBlendMode(mode);

            // Assert
            (extGState.Dict["BM"] as HpdfName)?.Value.Should().Be(expectedName);
        }

        [Fact]
        public void SetBlendMode_ThrowsWhenModeIsEof()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            Action act = () => extGState.SetBlendMode(HpdfBlendMode.Eof);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Invalid blend mode*");
        }

        // Integration Tests

        [Fact]
        public void ExtGState_WithMultipleSettings_CreatesValidDict()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var extGState = new HpdfExtGState(xref, "GS1");

            // Act
            extGState.SetAlphaStroke(0.8f);
            extGState.SetAlphaFill(0.6f);
            extGState.SetBlendMode(HpdfBlendMode.Multiply);

            // Assert
            extGState.Dict.Should().ContainKey("Type");
            extGState.Dict.Should().ContainKey("CA");
            extGState.Dict.Should().ContainKey("ca");
            extGState.Dict.Should().ContainKey("BM");
        }

        [Fact]
        public void ExtGState_AddedToXref()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var initialCount = xref.Entries.Count;

            // Act
            var extGState = new HpdfExtGState(xref, "GS1");

            // Assert
            xref.Entries.Count.Should().BeGreaterThan(initialCount);
            extGState.Dict.IsIndirect.Should().BeTrue();
            extGState.Dict.ObjectId.Should().BeGreaterThan(0);
        }
    }
}
