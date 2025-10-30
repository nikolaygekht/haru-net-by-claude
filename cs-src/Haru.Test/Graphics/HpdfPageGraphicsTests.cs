using System.Text;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Types;
using Haru.Xref;


namespace Haru.Test.Graphics
{
    public class HpdfPageGraphicsTests
    {
        private HpdfPage CreateTestPage()
        {
            var xref = new HpdfXref(0);
            return new HpdfPage(xref);
        }

        private string GetStreamContent(HpdfPage page)
        {
            var data = page.Contents.Stream.ToArray();
            return Encoding.ASCII.GetString(data);
        }

        // Graphics State Tests

        [Fact]
        public void GSave_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.GSave();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("q\n");
        }

        [Fact]
        public void GSave_PushesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();
            var originalState = page.GraphicsState;

            // Act
            page.GSave();

            // Assert
            page.GraphicsState.Should().NotBeSameAs(originalState);
            page.GraphicsState.Previous.Should().BeSameAs(originalState);
        }

        [Fact]
        public void GRestore_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();
            page.GSave();

            // Act
            page.GRestore();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("Q\n");
        }

        [Fact]
        public void GRestore_PopsGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();
            var originalState = page.GraphicsState;
            page.GSave();

            // Act
            page.GRestore();

            // Assert
            page.GraphicsState.Should().BeSameAs(originalState);
        }

        // Line Attribute Tests

        [Fact]
        public void SetLineWidth_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetLineWidth(2.5f);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("2.5 w\n");
        }

        [Fact]
        public void SetLineWidth_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetLineWidth(3.0f);

            // Assert
            page.GraphicsState.LineWidth.Should().Be(3.0f);
        }

        [Fact]
        public void SetLineWidth_ThrowsOnNegativeValue()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            var act = () => page.SetLineWidth(-1.0f);

            // Assert
            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.PageOutOfRange);
        }

        [Fact]
        public void SetLineCap_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetLineCap(HpdfLineCap.RoundEnd);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("1 J\n");
        }

        [Fact]
        public void SetLineCap_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetLineCap(HpdfLineCap.ProjectingSquareEnd);

            // Assert
            page.GraphicsState.LineCap.Should().Be(HpdfLineCap.ProjectingSquareEnd);
        }

        [Fact]
        public void SetLineJoin_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetLineJoin(HpdfLineJoin.RoundJoin);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("1 j\n");
        }

        [Fact]
        public void SetLineJoin_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetLineJoin(HpdfLineJoin.BevelJoin);

            // Assert
            page.GraphicsState.LineJoin.Should().Be(HpdfLineJoin.BevelJoin);
        }

        [Fact]
        public void SetMiterLimit_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetMiterLimit(5.0f);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("5 M\n");
        }

        [Fact]
        public void SetMiterLimit_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetMiterLimit(8.0f);

            // Assert
            page.GraphicsState.MiterLimit.Should().Be(8.0f);
        }

        [Fact]
        public void SetMiterLimit_ThrowsOnValueLessThanOne()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            var act = () => page.SetMiterLimit(0.5f);

            // Assert
            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.PageOutOfRange);
        }

        [Fact]
        public void SetDash_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetDash(new ushort[] { 3, 2 }, 0);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("[3 2] 0 d\n");
        }

        [Fact]
        public void SetDash_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetDash(new ushort[] { 5, 3, 1, 3 }, 2);

            // Assert
            page.GraphicsState.DashMode.NumPattern.Should().Be(4);
            page.GraphicsState.DashMode.Phase.Should().Be(2);
        }

        // Path Construction Tests

        [Fact]
        public void MoveTo_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.MoveTo(100, 200);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("100 200 m\n");
        }

        [Fact]
        public void MoveTo_UpdatesCurrentPosition()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.MoveTo(150, 250);

            // Assert
            page.CurrentPos.X.Should().Be(150);
            page.CurrentPos.Y.Should().Be(250);
        }

        [Fact]
        public void LineTo_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.LineTo(300, 400);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("300 400 l\n");
        }

        [Fact]
        public void LineTo_UpdatesCurrentPosition()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.LineTo(350, 450);

            // Assert
            page.CurrentPos.X.Should().Be(350);
            page.CurrentPos.Y.Should().Be(450);
        }

        [Fact]
        public void Rectangle_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Rectangle(10, 20, 100, 50);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("10 20 100 50 re\n");
        }

        [Fact]
        public void ClosePath_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.ClosePath();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("h\n");
        }

        // Path Painting Tests

        [Fact]
        public void Stroke_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Stroke();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("S\n");
        }

        [Fact]
        public void ClosePathStroke_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.ClosePathStroke();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("s\n");
        }

        [Fact]
        public void Fill_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.Fill();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("f\n");
        }

        [Fact]
        public void EoFill_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.EoFill();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("f*\n");
        }

        [Fact]
        public void FillStroke_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.FillStroke();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("B\n");
        }

        [Fact]
        public void ClosePathFillStroke_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.ClosePathFillStroke();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("b\n");
        }

        [Fact]
        public void EndPath_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.EndPath();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("n\n");
        }

        // Color Operation Tests

        [Fact]
        public void SetGrayFill_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetGrayFill(0.5f);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("0.5 g\n");
        }

        [Fact]
        public void SetGrayFill_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetGrayFill(0.75f);

            // Assert
            page.GraphicsState.GrayFill.Should().Be(0.75f);
            page.GraphicsState.FillColorSpace.Should().Be(HpdfColorSpace.DeviceGray);
        }

        [Fact]
        public void SetGrayFill_ThrowsOnInvalidValue()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act & Assert
            var act1 = () => page.SetGrayFill(-0.1f);
            act1.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.PageOutOfRange);

            var act2 = () => page.SetGrayFill(1.1f);
            act2.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.PageOutOfRange);
        }

        [Fact]
        public void SetGrayStroke_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetGrayStroke(0.25f);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("0.25 G\n");
        }

        [Fact]
        public void SetGrayStroke_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetGrayStroke(0.8f);

            // Assert
            page.GraphicsState.GrayStroke.Should().Be(0.8f);
            page.GraphicsState.StrokeColorSpace.Should().Be(HpdfColorSpace.DeviceGray);
        }

        [Fact]
        public void SetRgbFill_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetRgbFill(1.0f, 0.5f, 0.0f);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("1 0.5 0 rg\n");
        }

        [Fact]
        public void SetRgbFill_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetRgbFill(0.2f, 0.4f, 0.6f);

            // Assert
            page.GraphicsState.RgbFill.R.Should().Be(0.2f);
            page.GraphicsState.RgbFill.G.Should().Be(0.4f);
            page.GraphicsState.RgbFill.B.Should().Be(0.6f);
            page.GraphicsState.FillColorSpace.Should().Be(HpdfColorSpace.DeviceRgb);
        }

        [Fact]
        public void SetRgbStroke_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetRgbStroke(0.0f, 1.0f, 0.5f);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("0 1 0.5 RG\n");
        }

        [Fact]
        public void SetRgbStroke_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetRgbStroke(0.7f, 0.3f, 0.9f);

            // Assert
            page.GraphicsState.RgbStroke.R.Should().Be(0.7f);
            page.GraphicsState.RgbStroke.G.Should().Be(0.3f);
            page.GraphicsState.RgbStroke.B.Should().Be(0.9f);
            page.GraphicsState.StrokeColorSpace.Should().Be(HpdfColorSpace.DeviceRgb);
        }

        [Fact]
        public void SetCmykFill_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetCmykFill(0.1f, 0.2f, 0.3f, 0.4f);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("0.1 0.2 0.3 0.4 k\n");
        }

        [Fact]
        public void SetCmykFill_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetCmykFill(0.5f, 0.6f, 0.7f, 0.8f);

            // Assert
            page.GraphicsState.CmykFill.C.Should().Be(0.5f);
            page.GraphicsState.CmykFill.M.Should().Be(0.6f);
            page.GraphicsState.CmykFill.Y.Should().Be(0.7f);
            page.GraphicsState.CmykFill.K.Should().Be(0.8f);
            page.GraphicsState.FillColorSpace.Should().Be(HpdfColorSpace.DeviceCmyk);
        }

        [Fact]
        public void SetCmykStroke_WritesCorrectOperator()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetCmykStroke(0.9f, 0.8f, 0.7f, 0.6f);

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("0.9 0.8 0.7 0.6 K\n");
        }

        [Fact]
        public void SetCmykStroke_UpdatesGraphicsState()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetCmykStroke(0.15f, 0.25f, 0.35f, 0.45f);

            // Assert
            page.GraphicsState.CmykStroke.C.Should().Be(0.15f);
            page.GraphicsState.CmykStroke.M.Should().Be(0.25f);
            page.GraphicsState.CmykStroke.Y.Should().Be(0.35f);
            page.GraphicsState.CmykStroke.K.Should().Be(0.45f);
            page.GraphicsState.StrokeColorSpace.Should().Be(HpdfColorSpace.DeviceCmyk);
        }

        // Integration Tests

        [Fact]
        public void DrawRectangle_ProducesCorrectSequence()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetLineWidth(2);
            page.SetRgbStroke(1, 0, 0);
            page.Rectangle(50, 50, 100, 100);
            page.Stroke();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("2 w\n");
            content.Should().Contain("1 0 0 RG\n");
            content.Should().Contain("50 50 100 100 re\n");
            content.Should().Contain("S\n");
        }

        [Fact]
        public void DrawPath_ProducesCorrectSequence()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.MoveTo(10, 10);
            page.LineTo(100, 10);
            page.LineTo(100, 100);
            page.LineTo(10, 100);
            page.ClosePath();
            page.Stroke();

            // Assert
            var content = GetStreamContent(page);
            content.Should().Contain("10 10 m\n");
            content.Should().Contain("100 10 l\n");
            content.Should().Contain("100 100 l\n");
            content.Should().Contain("10 100 l\n");
            content.Should().Contain("h\n");
            content.Should().Contain("S\n");
        }

        [Fact]
        public void GraphicsStateStack_WorksCorrectly()
        {
            // Arrange
            using var page = CreateTestPage();

            // Act
            page.SetLineWidth(1);
            page.GSave();
            page.SetLineWidth(5);
            page.GRestore();

            // Assert
            page.GraphicsState.LineWidth.Should().Be(1);
            var content = GetStreamContent(page);
            content.Should().Contain("1 w\n");
            content.Should().Contain("q\n");
            content.Should().Contain("5 w\n");
            content.Should().Contain("Q\n");
        }
    }
}
