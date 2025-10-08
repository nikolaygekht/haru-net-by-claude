using Xunit;
using FluentAssertions;
using Haru.Graphics;
using Haru.Types;

namespace Haru.Test.Graphics
{
    public class HpdfGraphicsStateTests
    {
        [Fact]
        public void Constructor_InitializesWithDefaults()
        {
            // Act
            var state = new HpdfGraphicsState();

            // Assert
            state.LineWidth.Should().Be(1.0f);
            state.LineCap.Should().Be(HpdfLineCap.ButtEnd);
            state.LineJoin.Should().Be(HpdfLineJoin.MiterJoin);
            state.MiterLimit.Should().Be(10.0f);
            state.Flatness.Should().Be(1.0f);
            state.CharSpace.Should().Be(0);
            state.WordSpace.Should().Be(0);
            state.HorizontalScaling.Should().Be(100);
            state.TextLeading.Should().Be(0);
            state.RenderingMode.Should().Be(0);
            state.TextRise.Should().Be(0);
            state.FillColorSpace.Should().Be(HpdfColorSpace.DeviceGray);
            state.StrokeColorSpace.Should().Be(HpdfColorSpace.DeviceGray);
            state.GrayFill.Should().Be(0);
            state.GrayStroke.Should().Be(0);
            state.FontSize.Should().Be(0);
            state.Previous.Should().BeNull();
            state.Depth.Should().Be(0);
        }

        [Fact]
        public void TransMatrix_InitializesToIdentity()
        {
            // Act
            var state = new HpdfGraphicsState();

            // Assert
            state.TransMatrix.Should().NotBeNull();
            state.TransMatrix.A.Should().Be(1);
            state.TransMatrix.B.Should().Be(0);
            state.TransMatrix.C.Should().Be(0);
            state.TransMatrix.D.Should().Be(1);
            state.TransMatrix.X.Should().Be(0);
            state.TransMatrix.Y.Should().Be(0);
        }

        [Fact]
        public void DashMode_InitializesToSolid()
        {
            // Act
            var state = new HpdfGraphicsState();

            // Assert
            state.DashMode.Should().NotBeNull();
            state.DashMode.NumPattern.Should().Be(0);
        }

        [Fact]
        public void RgbColors_InitializeToBlack()
        {
            // Act
            var state = new HpdfGraphicsState();

            // Assert
            state.RgbFill.R.Should().Be(0);
            state.RgbFill.G.Should().Be(0);
            state.RgbFill.B.Should().Be(0);
            state.RgbStroke.R.Should().Be(0);
            state.RgbStroke.G.Should().Be(0);
            state.RgbStroke.B.Should().Be(0);
        }

        [Fact]
        public void CmykColors_InitializeToBlack()
        {
            // Act
            var state = new HpdfGraphicsState();

            // Assert
            state.CmykFill.C.Should().Be(0);
            state.CmykFill.M.Should().Be(0);
            state.CmykFill.Y.Should().Be(0);
            state.CmykFill.K.Should().Be(1);
            state.CmykStroke.C.Should().Be(0);
            state.CmykStroke.M.Should().Be(0);
            state.CmykStroke.Y.Should().Be(0);
            state.CmykStroke.K.Should().Be(1);
        }

        [Fact]
        public void Clone_CreatesIndependentCopy()
        {
            // Arrange
            var original = new HpdfGraphicsState();
            original.LineWidth = 5.0f;
            original.LineCap = HpdfLineCap.RoundEnd;
            original.GrayFill = 0.5f;

            // Act
            var clone = original.Clone();

            // Assert
            clone.Should().NotBeSameAs(original);
            clone.LineWidth.Should().Be(5.0f);
            clone.LineCap.Should().Be(HpdfLineCap.RoundEnd);
            clone.GrayFill.Should().Be(0.5f);
        }

        [Fact]
        public void Clone_SetsPreviousReference()
        {
            // Arrange
            var original = new HpdfGraphicsState();

            // Act
            var clone = original.Clone();

            // Assert
            clone.Previous.Should().BeSameAs(original);
        }

        [Fact]
        public void Clone_IncrementsDepth()
        {
            // Arrange
            var original = new HpdfGraphicsState();
            original.Depth = 2;

            // Act
            var clone = original.Clone();

            // Assert
            clone.Depth.Should().Be(3);
        }

        [Fact]
        public void Clone_ModifyingClone_DoesNotAffectOriginal()
        {
            // Arrange
            var original = new HpdfGraphicsState();
            original.LineWidth = 2.0f;
            var clone = original.Clone();

            // Act
            clone.LineWidth = 10.0f;

            // Assert
            original.LineWidth.Should().Be(2.0f);
            clone.LineWidth.Should().Be(10.0f);
        }

        [Fact]
        public void Clone_CreatesNewTransMatrix()
        {
            // Arrange
            var original = new HpdfGraphicsState();
            original.TransMatrix = new HpdfTransMatrix(2, 0, 0, 2, 10, 20);

            // Act
            var clone = original.Clone();

            // Assert
            clone.TransMatrix.Should().NotBeSameAs(original.TransMatrix);
            clone.TransMatrix.A.Should().Be(2);
            clone.TransMatrix.X.Should().Be(10);
            clone.TransMatrix.Y.Should().Be(20);
        }

        [Fact]
        public void Clone_CreatesNewDashMode()
        {
            // Arrange
            var original = new HpdfGraphicsState();
            original.DashMode = new HpdfDashMode(new ushort[] { 3, 2 }, 0);

            // Act
            var clone = original.Clone();

            // Assert
            clone.DashMode.NumPattern.Should().Be(2);
            var pattern = clone.DashMode.GetActivePattern();
            pattern.Should().Equal(new ushort[] { 3, 2 });
        }

        [Fact]
        public void Clone_CanBeChained()
        {
            // Arrange
            var state1 = new HpdfGraphicsState();
            state1.LineWidth = 1.0f;

            // Act
            var state2 = state1.Clone();
            state2.LineWidth = 2.0f;

            var state3 = state2.Clone();
            state3.LineWidth = 3.0f;

            // Assert
            state1.Depth.Should().Be(0);
            state2.Depth.Should().Be(1);
            state3.Depth.Should().Be(2);

            state3.Previous.Should().BeSameAs(state2);
            state2.Previous.Should().BeSameAs(state1);
            state1.Previous.Should().BeNull();

            state1.LineWidth.Should().Be(1.0f);
            state2.LineWidth.Should().Be(2.0f);
            state3.LineWidth.Should().Be(3.0f);
        }
    }
}
