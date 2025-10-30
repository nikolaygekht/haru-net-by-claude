using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Annotations;
using Haru.Types;
using Haru.Objects;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Annotations
{
    public class HpdfAnnotationTests
    {
        [Fact]
        public void CreateTextAnnotation_CreatesValidAnnotation()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var rect = new HpdfRect { Left = 100, Bottom = 100, Right = 200, Top = 150 };

            // Act
            var annot = page.CreateTextAnnotation(rect, "Test note");

            // Assert
            annot.Should().NotBeNull();
            annot.Type.Should().Be(HpdfAnnotationType.Text);
            annot.Dict.Should().NotBeNull();
            annot.Dict.Should().ContainKey("Type");
            annot.Dict.Should().ContainKey("Subtype");
            annot.Dict.Should().ContainKey("Rect");
            annot.Dict.Should().ContainKey("Contents");
        }

        [Fact]
        public void CreateTextAnnotation_WithIcon_SetsCorrectIcon()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var rect = new HpdfRect { Left = 100, Bottom = 100, Right = 200, Top = 150 };

            // Act
            var annot = page.CreateTextAnnotation(rect, "Test note", HpdfAnnotationIcon.Help);

            // Assert
            annot.Dict.Should().ContainKey("Name");
            var name = annot.Dict["Name"] as HpdfName;
            name.Should().NotBeNull();
            name.Value.Should().Be("Help");
        }

        [Fact]
        public void CreateURILinkAnnotation_CreatesValidLink()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var rect = new HpdfRect { Left = 100, Bottom = 100, Right = 300, Top = 120 };

            // Act
            var annot = page.CreateLinkAnnotation(rect, "https://example.com");

            // Assert
            annot.Should().NotBeNull();
            annot.Type.Should().Be(HpdfAnnotationType.Link);
            annot.Dict.Should().ContainKey("A");

            var action = annot.Dict["A"] as HpdfDict;
            action.Should().NotBeNull();
            action!.Should().ContainKey("S");
            action.Should().ContainKey("URI");

            var s = action["S"] as HpdfName;
            s!.Value.Should().Be("URI");
        }

        [Fact]
        public void SetBorderStyle_Solid_CreatesCorrectBorderDict()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var rect = new HpdfRect { Left = 100, Bottom = 100, Right = 200, Top = 150 };
            var annot = page.CreateTextAnnotation(rect, "Test");

            // Act
            annot.SetBorderStyle(HpdfBorderStyle.Solid, 2.0f);

            // Assert
            annot.Dict.Should().ContainKey("BS");
            var bs = annot.Dict["BS"] as HpdfDict;
            bs.Should().NotBeNull();
            bs!.Should().ContainKey("S");

            var style = bs["S"] as HpdfName;
            style!.Value.Should().Be("S");

            var width = bs["W"] as HpdfReal;
            width!.Value.Should().Be(2.0f);
        }

        [Fact]
        public void SetBorderStyle_Dashed_CreatesDashArray()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var rect = new HpdfRect { Left = 100, Bottom = 100, Right = 200, Top = 150 };
            var annot = page.CreateTextAnnotation(rect, "Test");

            // Act
            annot.SetBorderStyle(HpdfBorderStyle.Dashed, 1.0f, 5, 3);

            // Assert
            var bs = annot.Dict["BS"] as HpdfDict;
            bs.Should().NotBeNull();

            var style = bs!["S"] as HpdfName;
            style!.Value.Should().Be("D");

            bs.Should().ContainKey("D");
            var dashArray = bs["D"] as HpdfArray;
            dashArray.Should().NotBeNull();
            dashArray.Count.Should().Be(2);
        }

        [Fact]
        public void SetRgbColor_SetsColorArray()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var rect = new HpdfRect { Left = 100, Bottom = 100, Right = 200, Top = 150 };
            var annot = page.CreateTextAnnotation(rect, "Test");

            // Act
            annot.SetRgbColor(1.0f, 0.0f, 0.0f); // Red

            // Assert
            annot.Dict.Should().ContainKey("C");
            var colorArray = annot.Dict["C"] as HpdfArray;
            colorArray.Should().NotBeNull();
            colorArray!.Count.Should().Be(3);

            var r = colorArray[0] as HpdfReal;
            r!.Value.Should().Be(1.0f);
        }

        [Fact]
        public void SetHighlightMode_SetsCorrectMode()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var rect = new HpdfRect { Left = 100, Bottom = 100, Right = 300, Top = 120 };
            var annot = page.CreateLinkAnnotation(rect, "https://example.com");

            // Act
            annot.SetHighlightMode(HpdfHighlightMode.InvertBox);

            // Assert
            annot.Dict.Should().ContainKey("H");
            var mode = annot.Dict["H"] as HpdfName;
            mode.Should().NotBeNull();
            mode.Value.Should().Be("I");
        }

        [Fact]
        public void PageWithMultipleAnnotations_AddsAllToAnnotsArray()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();

            // Act
            page.CreateTextAnnotation(new HpdfRect { Left = 10, Bottom = 10, Right = 50, Top = 50 }, "Note 1");
            page.CreateTextAnnotation(new HpdfRect { Left = 60, Bottom = 10, Right = 100, Top = 50 }, "Note 2");
            page.CreateLinkAnnotation(new HpdfRect { Left = 110, Bottom = 10, Right = 200, Top = 30 }, "https://example.com");

            // Assert
            page.Dict.Should().ContainKey("Annots");
            var annots = page.Dict["Annots"] as HpdfArray;
            annots.Should().NotBeNull();
            annots.Count.Should().Be(3);
        }

        [Fact]
        public void TextAnnotation_SetOpened_AddsOpenFlag()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            var rect = new HpdfRect { Left = 100, Bottom = 100, Right = 200, Top = 150 };
            var annot = page.CreateTextAnnotation(rect, "Test note");

            // Act
            annot.SetOpened(true);

            // Assert
            annot.Dict.Should().ContainKey("Open");
            var open = annot.Dict["Open"] as HpdfBoolean;
            open.Should().NotBeNull();
            open.Value.Should().BeTrue();
        }

        [Fact]
        public void CreateAnnotation_WithNormalizedRect_SwapsTopAndBottom()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            // Rect with top < bottom (should be swapped)
            var rect = new HpdfRect { Left = 100, Bottom = 200, Right = 300, Top = 150 };

            // Act
            var annot = page.CreateTextAnnotation(rect, "Test");

            // Assert
            var rectArray = annot.Dict["Rect"] as HpdfArray;
            rectArray.Should().NotBeNull();

            // After normalization: [left, bottom(150), right, top(200)]
            var bottom = rectArray![1] as HpdfReal;
            var top = rectArray[3] as HpdfReal;

            bottom!.Value.Should().Be(150); // Original top becomes bottom
            top!.Value.Should().Be(200);    // Original bottom becomes top
        }

        [Fact]
        public void SaveDocument_WithAnnotations_ProducesValidPDF()
        {
            // Arrange
            var doc = new HpdfDocument();
            var page = doc.AddPage();
            page.CreateTextAnnotation(
                new HpdfRect { Left = 50, Bottom = 700, Right = 100, Top = 750 },
                "This is a test note",
                HpdfAnnotationIcon.Comment
            );
            page.CreateLinkAnnotation(
                new HpdfRect { Left = 100, Bottom = 100, Right = 300, Top = 120 },
                "https://github.com/libharu/libharu"
            );

            // Act
            byte[] pdfData = doc.SaveToMemory();

            // Assert
            pdfData.Should().NotBeNull();
            pdfData.Length.Should().BeGreaterThan(0);

            // PDF should start with %PDF-
            System.Text.Encoding.ASCII.GetString(pdfData, 0, 5).Should().Be("%PDF-");
        }
    }
}
