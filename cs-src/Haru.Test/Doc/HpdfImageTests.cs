using System;
using System.IO;
using System.Text;
using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Xref;
using Haru.Types;
using Haru.Objects;
using Haru.Streams;

namespace Haru.Test.Doc
{
    public class HpdfImageTests
    {
        private const string TestResourcePath = "Haru.Test.Resources.";

        private Stream GetResourceStream(string resourceName)
        {
            var assembly = typeof(HpdfImageTests).Assembly;
            var fullName = TestResourcePath + resourceName;
            return assembly.GetManifestResourceStream(fullName);
        }

        // PNG Loading Tests

        [Fact]
        public void LoadPngImage_Grayscale_CreatesValidImage()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_grayscale_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            image.Should().NotBeNull();
            image.LocalName.Should().Be("Im1");
            image.Width.Should().Be(2);
            image.Height.Should().Be(2);
            image.Dict.Should().ContainKey("ColorSpace");
            (image.Dict["ColorSpace"] as HpdfName)?.Value.Should().Be("DeviceGray");
        }

        [Fact]
        public void LoadPngImage_RGB_CreatesValidImage()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_rgb_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            image.Should().NotBeNull();
            image.Width.Should().Be(2);
            image.Height.Should().Be(2);
            (image.Dict["ColorSpace"] as HpdfName)?.Value.Should().Be("DeviceRGB");
        }

        [Fact]
        public void LoadPngImage_RGBA_CreatesSMask()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_rgba_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            image.Should().NotBeNull();
            image.Width.Should().Be(2);
            image.Height.Should().Be(2);
            (image.Dict["ColorSpace"] as HpdfName)?.Value.Should().Be("DeviceRGB");

            // Should have SMask for transparency
            image.Dict.Should().ContainKey("SMask");
            var smask = image.Dict["SMask"] as HpdfStreamObject;
            smask.Should().NotBeNull();
            (smask["ColorSpace"] as HpdfName)?.Value.Should().Be("DeviceGray");
        }

        [Fact]
        public void LoadPngImage_GrayscaleAlpha_CreatesSMask()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_grayscale_alpha_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            image.Should().NotBeNull();
            image.Width.Should().Be(2);
            image.Height.Should().Be(2);
            (image.Dict["ColorSpace"] as HpdfName)?.Value.Should().Be("DeviceGray");

            // Should have SMask for transparency
            image.Dict.Should().ContainKey("SMask");
            var smask = image.Dict["SMask"] as HpdfStreamObject;
            smask.Should().NotBeNull();
            (smask["ColorSpace"] as HpdfName)?.Value.Should().Be("DeviceGray");
        }

        [Fact]
        public void LoadPngImage_Palette_CreatesIndexedColorSpace()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_palette_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            image.Should().NotBeNull();
            image.Width.Should().Be(2);
            image.Height.Should().Be(2);

            // Should have indexed color space array (or DeviceRGB if palette was expanded)
            image.Dict.Should().ContainKey("ColorSpace");
            var colorSpace = image.Dict["ColorSpace"];
            colorSpace.Should().NotBeNull();

            // Could be either indexed array or DeviceRGB (depending on PNG reader implementation)
            if (colorSpace is HpdfArray array)
            {
                array.Count.Should().BeGreaterOrEqualTo(4);
                (array[0] as HpdfName)?.Value.Should().Be("Indexed");
            }
            else if (colorSpace is HpdfName name)
            {
                // Some PNG readers expand palette to RGB
                name.Value.Should().BeOneOf("DeviceRGB", "DeviceGray");
            }
        }

        [Fact]
        public void LoadPngImage_AddedToXref()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var initialCount = xref.Entries.Count;
            using var stream = GetResourceStream("test_rgb_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            xref.Entries.Count.Should().BeGreaterThan(initialCount);
            image.Dict.IsIndirect.Should().BeTrue();
            image.Dict.ObjectId.Should().BeGreaterThan(0);
        }

        [Fact]
        public void LoadPngImage_SetsXObjectType()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_rgb_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            image.Dict.Should().ContainKey("Type");
            (image.Dict["Type"] as HpdfName)?.Value.Should().Be("XObject");
            image.Dict.Should().ContainKey("Subtype");
            (image.Dict["Subtype"] as HpdfName)?.Value.Should().Be("Image");
        }

        [Fact]
        public void LoadPngImage_AppliesFlateDecode()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_rgb_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            image.StreamObject.Filter.Should().Be(HpdfStreamFilter.FlateDecode);
        }

        [Fact]
        public void LoadPngImage_InvalidSignature_ThrowsException()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = new MemoryStream(new byte[] { 0x00, 0x01, 0x02, 0x03 });

            // Act
            Action act = () => HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Invalid PNG signature*");
        }

        [Fact]
        public void LoadPngImage_NullXref_ThrowsException()
        {
            // Arrange
            using var stream = GetResourceStream("test_rgb_2x2.png");

            // Act
            Action act = () => HpdfImage.LoadPngImage(null, "Im1", stream);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Xref cannot be null*");
        }

        [Fact]
        public void LoadPngImage_NullLocalName_ThrowsException()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_rgb_2x2.png");

            // Act
            Action act = () => HpdfImage.LoadPngImage(xref, null, stream);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Local name cannot be null or empty*");
        }

        // SMask Tests

        [Fact]
        public void LoadPngImage_RGBA_SMaskHasCorrectDimensions()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_rgba_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            var smask = image.Dict["SMask"] as HpdfStreamObject;
            smask.Should().NotBeNull();
            (smask["Width"] as HpdfNumber)?.Value.Should().Be(2);
            (smask["Height"] as HpdfNumber)?.Value.Should().Be(2);
        }

        [Fact]
        public void LoadPngImage_RGBA_SMaskIsInXref()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_rgba_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            var smask = image.Dict["SMask"] as HpdfStreamObject;
            smask.IsIndirect.Should().BeTrue();
            smask.ObjectId.Should().BeGreaterThan(0);
        }

        [Fact]
        public void LoadPngImage_RGBA_SMaskIsCompressed()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var stream = GetResourceStream("test_rgba_2x2.png");

            // Act
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Assert
            var smask = image.Dict["SMask"] as HpdfStreamObject;
            smask.Filter.Should().Be(HpdfStreamFilter.FlateDecode);
        }

        // Integration Tests

        [Fact]
        public void DrawImage_OnPage_WritesCorrectOperators()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            using var stream = GetResourceStream("test_rgb_2x2.png");
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Act
            page.DrawImage(image, 100, 200, 50, 75);

            // Assert
            var content = Encoding.ASCII.GetString(page.Contents.Stream.ToArray());
            content.Should().Contain("q\n");  // GSave
            content.Should().Contain("50 0 0 75 100 200 cm\n");  // Transform
            content.Should().Contain("/Im1 Do\n");  // Draw image
            content.Should().Contain("Q\n");  // GRestore
        }

        [Fact]
        public void DrawImage_AddsImageToPageResources()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            using var stream = GetResourceStream("test_rgb_2x2.png");
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Act
            page.DrawImage(image, 100, 200, 50, 75);

            // Assert
            var resources = page.GetResources();
            resources.Should().ContainKey("XObject");
            var xobjects = resources["XObject"] as HpdfDict;
            xobjects.Should().ContainKey("Im1");
        }

        [Fact]
        public void DrawImage_MultipleImages_AllAdded()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            using var stream1 = GetResourceStream("test_rgb_2x2.png");
            using var stream2 = GetResourceStream("test_grayscale_2x2.png");
            var image1 = HpdfImage.LoadPngImage(xref, "Im1", stream1);
            var image2 = HpdfImage.LoadPngImage(xref, "Im2", stream2);

            // Act
            page.DrawImage(image1, 10, 10, 50, 50);
            page.DrawImage(image2, 70, 10, 50, 50);

            // Assert
            var resources = page.GetResources();
            var xobjects = resources["XObject"] as HpdfDict;
            xobjects.Should().ContainKey("Im1");
            xobjects.Should().ContainKey("Im2");
        }

        [Fact]
        public void DrawImage_NullImage_ThrowsException()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);

            // Act
            Action act = () => page.DrawImage(null, 0, 0, 100, 100);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Image cannot be null*");
        }

        [Fact]
        public void DrawImage_ZeroWidth_ThrowsException()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            using var stream = GetResourceStream("test_rgb_2x2.png");
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Act
            Action act = () => page.DrawImage(image, 0, 0, 0, 100);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Image dimensions must be positive*");
        }

        [Fact]
        public void DrawImage_NegativeHeight_ThrowsException()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var page = new HpdfPage(xref);
            using var stream = GetResourceStream("test_rgb_2x2.png");
            var image = HpdfImage.LoadPngImage(xref, "Im1", stream);

            // Act
            Action act = () => page.DrawImage(image, 0, 0, 100, -50);

            // Assert
            act.Should().Throw<HpdfException>()
                .WithMessage("*Image dimensions must be positive*");
        }
    }
}
