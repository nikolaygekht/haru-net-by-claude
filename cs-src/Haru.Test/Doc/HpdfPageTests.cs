using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Objects;
using Haru.Xref;


namespace Haru.Test.Doc
{
    public class HpdfPageTests
    {
        [Fact]
        public void Constructor_CreatesValidPage()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            using var page = new HpdfPage(xref);

            // Assert
            page.Should().NotBeNull();
            page.Dict.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_SetsTypeAsPage()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            using var page = new HpdfPage(xref);

            // Assert
            page.Dict.TryGetValue("Type", out var typeObj);
            var type = typeObj as HpdfName;
            type.Should().NotBeNull();
            type!.Value.Should().Be("Page");
        }

        [Fact]
        public void Constructor_CreatesDefaultMediaBox()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            using var page = new HpdfPage(xref);

            // Assert
            page.Dict.TryGetValue("MediaBox", out var mediaBoxObj);
            var mediaBox = mediaBoxObj as HpdfArray;
            mediaBox.Should().NotBeNull();
            mediaBox.Count.Should().Be(4);
        }

        [Fact]
        public void Constructor_CreatesContentsStream()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            using var page = new HpdfPage(xref);

            // Assert
            page.Contents.Should().NotBeNull();
            page.Dict.TryGetValue("Contents", out var contents);
            contents.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_CreatesResourcesDictionary()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            using var page = new HpdfPage(xref);

            // Assert
            var resources = page.GetResources();
            resources.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_ThrowsWhenXrefIsNull()
        {
            // Act
            var act = () => new HpdfPage(null!);

            // Assert
            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.InvalidParameter);
        }

        [Fact]
        public void Width_DefaultIsLetterSize()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            var width = page.Width;

            // Assert
            width.Should().Be(HpdfPage.DefaultWidth);
        }

        [Fact]
        public void Height_DefaultIsLetterSize()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            var height = page.Height;

            // Assert
            height.Should().Be(HpdfPage.DefaultHeight);
        }

        [Fact]
        public void Width_CanBeSet()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.Width = 500;

            // Assert
            page.Width.Should().Be(500);
        }

        [Fact]
        public void Height_CanBeSet()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.Height = 700;

            // Assert
            page.Height.Should().Be(700);
        }

        [Fact]
        public void SetMediaBox_UpdatesMediaBox()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.SetMediaBox(10, 20, 610, 820);

            // Assert
            page.Dict.TryGetValue("MediaBox", out var mediaBoxObj);
            var mediaBox = mediaBoxObj as HpdfArray;
            mediaBox.Should().NotBeNull();
            mediaBox!.Count.Should().Be(4);
            (mediaBox[0] as HpdfReal)!.Value.Should().Be(10);
            (mediaBox[1] as HpdfReal)!.Value.Should().Be(20);
            (mediaBox[2] as HpdfReal)!.Value.Should().Be(610);
            (mediaBox[3] as HpdfReal)!.Value.Should().Be(820);
        }

        [Fact]
        public void SetSize_WithWidthHeight_UpdatesMediaBox()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.SetSize(400, 600);

            // Assert
            page.Width.Should().Be(400);
            page.Height.Should().Be(600);
        }

        [Fact]
        public void SetSize_WithPageSize_UpdatesMediaBox()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.SetSize(HpdfPageSize.A4, HpdfPageDirection.Portrait);

            // Assert
            page.Width.Should().BeApproximately(595.276f, 0.01f);
            page.Height.Should().BeApproximately(841.89f, 0.01f);
        }

        [Fact]
        public void SetSize_WithLandscape_SwapsDimensions()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.SetSize(HpdfPageSize.A4, HpdfPageDirection.Landscape);

            // Assert
            page.Width.Should().BeApproximately(841.89f, 0.01f);
            page.Height.Should().BeApproximately(595.276f, 0.01f);
        }

        [Fact]
        public void GetResources_ReturnsResourcesDictionary()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            var resources = page.GetResources();

            // Assert
            resources.Should().NotBeNull();
            resources.Should().BeOfType<HpdfDict>();
        }

        [Fact]
        public void Parent_InitiallyNull()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act & Assert
            page.Parent.Should().BeNull();
        }

        [Fact]
        public void Parent_SetWhenAddedToPages()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var pages = new HpdfPages(xref);
            using var page = new HpdfPage(xref);

            // Act
            pages.AddKid(page);

            // Assert
            page.Parent.Should().BeSameAs(pages);
        }

        [Theory]
        [InlineData(HpdfPageSize.Letter, HpdfPageDirection.Portrait, 612, 792)]
        [InlineData(HpdfPageSize.Legal, HpdfPageDirection.Portrait, 612, 1008)]
        [InlineData(HpdfPageSize.A4, HpdfPageDirection.Portrait, 595.276f, 841.89f)]
        [InlineData(HpdfPageSize.A5, HpdfPageDirection.Portrait, 419.528f, 595.276f)]
        public void SetSize_WithDifferentPageSizes_SetsCorrectDimensions(
            HpdfPageSize size, HpdfPageDirection direction, float expectedWidth, float expectedHeight)
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.SetSize(size, direction);

            // Assert
            page.Width.Should().BeApproximately(expectedWidth, 0.01f);
            page.Height.Should().BeApproximately(expectedHeight, 0.01f);
        }

        // Extension method tests for API compatibility

        [Fact]
        public void SetHeight_ExtensionMethod_UpdatesHeight()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.SetHeight(800);

            // Assert
            page.Height.Should().Be(800);
        }

        [Fact]
        public void SetWidth_ExtensionMethod_UpdatesWidth()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.SetWidth(600);

            // Assert
            page.Width.Should().Be(600);
        }

        [Fact]
        public void SetWidthAndHeight_ExtensionMethods_UpdatesBoth()
        {
            // Arrange
            var xref = new HpdfXref(0);
            using var page = new HpdfPage(xref);

            // Act
            page.SetWidth(500);
            page.SetHeight(750);

            // Assert
            page.Width.Should().Be(500);
            page.Height.Should().Be(750);
        }
    }
}
