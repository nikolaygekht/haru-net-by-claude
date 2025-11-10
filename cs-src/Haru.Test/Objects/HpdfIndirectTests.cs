using Xunit;
using FluentAssertions;
using Haru.Objects;
using Haru.Xref;
using Haru.Streams;
using System.Text;


namespace Haru.Test.Objects
{
    public class HpdfIndirectTests
    {
        [Fact]
        public void DirectObject_NotAddedToXref_RemainsDirect()
        {
            // Arrange
            var array = new HpdfArray();
            array.Add(new HpdfNumber(0));

            // Assert
            array.IsIndirect.Should().BeFalse();
            array.ObjectId.Should().Be(0);
        }

        [Fact]
        public void IndirectObject_AddedToXref_BecomesIndirect()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var dict = new HpdfDict();

            // Act
            xref.Add(dict);

            // Assert
            dict.IsIndirect.Should().BeTrue();
            dict.ObjectId.Should().BeGreaterThan(0);
        }

        [Fact]
        public void DirectChildObject_InIndirectParent_RemainsDirectInOutput()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var dict = new HpdfDict();
            var array = new HpdfArray();
            array.Add(new HpdfNumber(0));
            array.Add(new HpdfNumber(612));

            xref.Add(dict);
            dict.Add("MediaBox", array);

            // Act
            using var stream = new HpdfMemoryStream();
            dict.WriteValue(stream);
            var output = System.Text.Encoding.ASCII.GetString(stream.ToArray());

            // Assert
            output.Should().Contain("[");  // Should contain direct array
            output.Should().Contain("0 612");  // Array contents
            output.Should().NotContain("R");  // Should NOT contain indirect reference
        }

        [Fact]
        public void IndirectChildObject_InIndirectParent_WrittenAsReference()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var parentDict = new HpdfDict();
            var childDict = new HpdfDict();

            xref.Add(childDict);  // Make child indirect
            xref.Add(parentDict);  // Make parent indirect
            parentDict.Add("Child", childDict);

            // Act
            using var stream = new HpdfMemoryStream();
            parentDict.WriteValue(stream);
            var output = System.Text.Encoding.ASCII.GetString(stream.ToArray());

            // Assert
            output.Should().Contain("1 0 R");  // Should contain indirect reference to child
            output.Should().Contain("<<");  // Parent dict still uses << >>
            output.Should().Contain("/Child");  // Should have Child key
        }
    }
}
