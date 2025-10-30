using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Objects;
using Haru.Xref;
using HpdfPageMode = Haru.Types.HpdfPageMode;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Doc
{
    public class HpdfCatalogTests
    {
        [Fact]
        public void Constructor_CreatesValidCatalog()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);

            // Act
            var catalog = new HpdfCatalog(xref, rootPages);

            // Assert
            catalog.Should().NotBeNull();
            catalog.Dict.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_SetsTypeAsCatalog()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);

            // Act
            var catalog = new HpdfCatalog(xref, rootPages);

            // Assert
            catalog.Dict.TryGetValue("Type", out var typeObj);
            var type = typeObj as HpdfName;
            type.Should().NotBeNull();
            type.Value.Should().Be("Catalog");
        }

        [Fact]
        public void Constructor_SetsPagesReference()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);

            // Act
            var catalog = new HpdfCatalog(xref, rootPages);

            // Assert
            catalog.Dict.TryGetValue("Pages", out var pages);
            pages.Should().NotBeNull();
            pages.Should().BeSameAs(rootPages.Dict);
        }

        [Fact]
        public void Constructor_ThrowsWhenXrefIsNull()
        {
            // Arrange
            var rootPages = new HpdfPages(new HpdfXref(0));

            // Act
            var act = () => new HpdfCatalog(null!, rootPages);

            // Assert
            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.InvalidParameter);
        }

        [Fact]
        public void Constructor_ThrowsWhenRootPagesIsNull()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var act = () => new HpdfCatalog(xref, null!);

            // Assert
            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.InvalidParameter);
        }

        [Fact]
        public void GetRootPages_ReturnsRootPagesDict()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);
            var catalog = new HpdfCatalog(xref, rootPages);

            // Act
            var result = catalog.GetRootPages();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(rootPages.Dict);
        }

        [Fact]
        public void PageLayout_DefaultIsSinglePage()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);
            var catalog = new HpdfCatalog(xref, rootPages);

            // Act
            var layout = catalog.PageLayout;

            // Assert
            layout.Should().Be(HpdfPageLayout.SinglePage);
        }

        [Fact]
        public void PageLayout_CanBeSet()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);
            var catalog = new HpdfCatalog(xref, rootPages);

            // Act
            catalog.PageLayout = HpdfPageLayout.TwoColumnLeft;

            // Assert
            catalog.PageLayout.Should().Be(HpdfPageLayout.TwoColumnLeft);
            catalog.Dict.TryGetValue("PageLayout", out var layoutObj);
            var layoutName = layoutObj as HpdfName;
            layoutName.Should().NotBeNull();
            layoutName.Value.Should().Be("TwoColumnLeft");
        }

        [Fact]
        public void PageMode_DefaultIsUseNone()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);
            var catalog = new HpdfCatalog(xref, rootPages);

            // Act
            var mode = catalog.PageMode;

            // Assert
            mode.Should().Be(HpdfPageMode.UseNone);
        }

        [Fact]
        public void PageMode_CanBeSet()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);
            var catalog = new HpdfCatalog(xref, rootPages);

            // Act
            catalog.PageMode = HpdfPageMode.UseOutlines;

            // Assert
            catalog.PageMode.Should().Be(HpdfPageMode.UseOutlines);
            catalog.Dict.TryGetValue("PageMode", out var modeObj);
            var modeName = modeObj as HpdfName;
            modeName.Should().NotBeNull();
            modeName.Value.Should().Be("UseOutlines");
        }

        [Fact]
        public void SetNames_AddsNamesDictionary()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);
            var catalog = new HpdfCatalog(xref, rootPages);
            var namesDict = new HpdfDict();

            // Act
            catalog.SetNames(namesDict);

            // Assert
            var names = catalog.GetNames();
            names.Should().NotBeNull();
            names.Should().BeSameAs(namesDict);
        }

        [Fact]
        public void SetNames_ThrowsWhenNull()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);
            var catalog = new HpdfCatalog(xref, rootPages);

            // Act
            var act = () => catalog.SetNames(null!);

            // Assert
            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.InvalidParameter);
        }

        [Fact]
        public void GetNames_ReturnsNullWhenNotSet()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var rootPages = new HpdfPages(xref);
            var catalog = new HpdfCatalog(xref, rootPages);

            // Act
            var names = catalog.GetNames();

            // Assert
            names.Should().BeNull();
        }
    }
}
