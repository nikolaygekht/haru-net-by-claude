using Xunit;
using FluentAssertions;
using Haru.Doc;
using Haru.Objects;
using Haru.Xref;

namespace Haru.Test.Doc
{
    public class HpdfPagesTests
    {
        [Fact]
        public void Constructor_CreatesValidPages()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var pages = new HpdfPages(xref);

            // Assert
            pages.Should().NotBeNull();
            pages.Dict.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_SetsTypeAsPages()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var pages = new HpdfPages(xref);

            // Assert
            pages.Dict.TryGetValue("Type", out var typeObj);
            var type = typeObj as HpdfName;
            type.Should().NotBeNull();
            type.Value.Should().Be("Pages");
        }

        [Fact]
        public void Constructor_InitializesKidsArray()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var pages = new HpdfPages(xref);

            // Assert
            pages.Dict.TryGetValue("Kids", out var kidsObj);
            var kids = kidsObj as HpdfArray;
            kids.Should().NotBeNull();
            kids.Count.Should().Be(0);
        }

        [Fact]
        public void Constructor_InitializesCountToZero()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var pages = new HpdfPages(xref);

            // Assert
            pages.Count.Should().Be(0);
        }

        [Fact]
        public void Constructor_ThrowsWhenXrefIsNull()
        {
            // Act
            var act = () => new HpdfPages(null);

            // Assert
            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.InvalidParameter);
        }

        [Fact]
        public void Constructor_WithParent_SetsParentReference()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var parent = new HpdfPages(xref);

            // Act
            var child = new HpdfPages(xref, parent);

            // Assert
            child.Parent.Should().BeSameAs(parent);
        }

        [Fact]
        public void Constructor_WithParent_AddsToParentKids()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var parent = new HpdfPages(xref);

            // Act
            var child = new HpdfPages(xref, parent);

            // Assert
            parent.GetChildren().Should().HaveCount(1);
            parent.GetChildren()[0].Should().BeSameAs(child);
        }

        [Fact]
        public void AddKid_AddsPageToKids()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var pages = new HpdfPages(xref);
            var page = new HpdfPage(xref);

            // Act
            pages.AddKid(page);

            // Assert
            pages.Dict.TryGetValue("Kids", out var kidsObj);
            var kids = kidsObj as HpdfArray;
            kids.Should().NotBeNull();
            kids.Count.Should().Be(1);
            kids[0].Should().BeSameAs(page.Dict);
        }

        [Fact]
        public void AddKid_UpdatesCount()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var pages = new HpdfPages(xref);
            var page = new HpdfPage(xref);

            // Act
            pages.AddKid(page);

            // Assert
            pages.Count.Should().Be(1);
        }

        [Fact]
        public void AddKid_SetsParentReference()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var pages = new HpdfPages(xref);
            var page = new HpdfPage(xref);

            // Act
            pages.AddKid(page);

            // Assert
            page.Parent.Should().BeSameAs(pages);
            page.Dict.TryGetValue("Parent", out var parentRef);
            parentRef.Should().BeSameAs(pages.Dict);
        }

        [Fact]
        public void AddKid_AddsChildPagesNode()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var parent = new HpdfPages(xref);
            var child = new HpdfPages(xref);

            // Act
            parent.AddKid(child);

            // Assert
            parent.GetChildren().Should().HaveCount(1);
            parent.GetChildren()[0].Should().BeSameAs(child);
            parent.Count.Should().Be(0); // No actual pages yet
        }

        [Fact]
        public void AddKid_UpdatesCountRecursively()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var root = new HpdfPages(xref);
            var child = new HpdfPages(xref, root);
            var page = new HpdfPage(xref);

            // Act
            child.AddKid(page);

            // Assert
            child.Count.Should().Be(1);
            root.Count.Should().Be(1); // Should propagate to parent
        }

        [Fact]
        public void AddKid_ThrowsWhenNull()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var pages = new HpdfPages(xref);

            // Act
            var act = () => pages.AddKid(null);

            // Assert
            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.InvalidParameter);
        }

        [Fact]
        public void AddKid_ThrowsWhenKidAlreadyHasParent()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var pages1 = new HpdfPages(xref);
            var pages2 = new HpdfPages(xref);
            var page = new HpdfPage(xref);
            pages1.AddKid(page);

            // Act
            var act = () => pages2.AddKid(page);

            // Assert
            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.PageCannotSetParent);
        }

        [Fact]
        public void GetAllPages_ReturnsAllPagesInTree()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var root = new HpdfPages(xref);
            var page1 = new HpdfPage(xref);
            var page2 = new HpdfPage(xref);
            var page3 = new HpdfPage(xref);
            root.AddKid(page1);
            root.AddKid(page2);
            root.AddKid(page3);

            // Act
            var allPages = root.GetAllPages();

            // Assert
            allPages.Should().HaveCount(3);
            allPages.Should().Contain(page1);
            allPages.Should().Contain(page2);
            allPages.Should().Contain(page3);
        }

        [Fact]
        public void GetAllPages_ReturnsAllPagesFromNestedTree()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var root = new HpdfPages(xref);
            var child1 = new HpdfPages(xref, root);
            var child2 = new HpdfPages(xref, root);
            var page1 = new HpdfPage(xref);
            var page2 = new HpdfPage(xref);
            var page3 = new HpdfPage(xref);
            child1.AddKid(page1);
            child1.AddKid(page2);
            child2.AddKid(page3);

            // Act
            var allPages = root.GetAllPages();

            // Assert
            allPages.Should().HaveCount(3);
            allPages.Should().Contain(page1);
            allPages.Should().Contain(page2);
            allPages.Should().Contain(page3);
        }

        [Fact]
        public void GetChildren_ReturnsDirectChildren()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var pages = new HpdfPages(xref);
            var page1 = new HpdfPage(xref);
            var page2 = new HpdfPage(xref);
            pages.AddKid(page1);
            pages.AddKid(page2);

            // Act
            var children = pages.GetChildren();

            // Assert
            children.Should().HaveCount(2);
            children[0].Should().BeSameAs(page1);
            children[1].Should().BeSameAs(page2);
        }

        [Fact]
        public void MultiplePages_UpdateCountCorrectly()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var root = new HpdfPages(xref);

            // Act
            for (int i = 0; i < 5; i++)
            {
                var page = new HpdfPage(xref);
                root.AddKid(page);
            }

            // Assert
            root.Count.Should().Be(5);
            root.GetAllPages().Should().HaveCount(5);
        }
    }
}
