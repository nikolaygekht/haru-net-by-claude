using System;
using Xunit;
using FluentAssertions;
using Haru.Xref;
using Haru.Objects;
using Haru.Streams;

namespace Haru.Test.Xref
{
    public class HpdfXrefTests
    {
        [Fact]
        public void Constructor_WithZeroOffset_ShouldAddFreeEntry()
        {
            // Act
            var xref = new HpdfXref(0);

            // Assert
            xref.StartOffset.Should().Be(0);
            xref.Entries.Should().HaveCount(1);
            xref.Entries[0].EntryType.Should().Be(HpdfXrefEntryType.Free);
            xref.Entries[0].GenerationNumber.Should().Be(HpdfXrefEntry.MaxGenerationNumber);
            xref.Address.Should().Be(0);
            xref.Previous.Should().BeNull();
            xref.Trailer.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNonZeroOffset_ShouldNotAddFreeEntry()
        {
            // Act
            var xref = new HpdfXref(100);

            // Assert
            xref.StartOffset.Should().Be(100);
            xref.Entries.Should().BeEmpty();
        }

        [Fact]
        public void Add_ShouldAddObjectAndAssignId()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var obj = new HpdfNull();

            // Act
            var objectId = xref.Add(obj);

            // Assert
            objectId.Should().Be(1); // Object 0 is the free entry
            xref.Entries.Should().HaveCount(2);
            xref.Entries[1].Object.Should().Be(obj);
            xref.Entries[1].EntryType.Should().Be(HpdfXrefEntryType.InUse);
            obj.ObjectId.Should().Be(1);
            obj.GenerationNumber.Should().Be(0);
        }

        [Fact]
        public void Add_WithStartOffset_ShouldCalculateCorrectObjectId()
        {
            // Arrange
            var xref = new HpdfXref(100);
            var obj = new HpdfBoolean(true);

            // Act
            var objectId = xref.Add(obj);

            // Assert
            objectId.Should().Be(100);
            obj.ObjectId.Should().Be(100);
        }

        [Fact]
        public void Add_MultipleObjects_ShouldAssignSequentialIds()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var obj1 = new HpdfNull();
            var obj2 = new HpdfBoolean(true);
            var obj3 = new HpdfNumber(42);

            // Act
            var id1 = xref.Add(obj1);
            var id2 = xref.Add(obj2);
            var id3 = xref.Add(obj3);

            // Assert
            id1.Should().Be(1);
            id2.Should().Be(2);
            id3.Should().Be(3);
            xref.Entries.Should().HaveCount(4); // Including free entry
        }

        [Fact]
        public void Add_NullObject_ShouldThrowArgumentNullException()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act & Assert
            Action act = () => xref.Add(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetEntry_ValidIndex_ShouldReturnEntry()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var obj = new HpdfNull();
            xref.Add(obj);

            // Act
            var entry = xref.GetEntry(1);

            // Assert
            entry.Object.Should().Be(obj);
        }

        [Fact]
        public void GetEntry_InvalidIndex_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act & Assert
            Action act = () => xref.GetEntry(10);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void GetEntryByObjectId_ShouldFindCorrectEntry()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var obj = new HpdfNumber(42);
            xref.Add(obj);

            // Act
            var entry = xref.GetEntryByObjectId(1);

            // Assert
            entry.Should().NotBeNull();
            entry.Object.Should().Be(obj);
        }

        [Fact]
        public void GetEntryByObjectId_NotFound_ShouldReturnNull()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            var entry = xref.GetEntryByObjectId(999);

            // Assert
            entry.Should().BeNull();
        }

        [Fact]
        public void GetEntryByObjectId_WithPreviousXref_ShouldSearchChain()
        {
            // Arrange
            var xref1 = new HpdfXref(0);
            var obj1 = new HpdfNumber(10);
            xref1.Add(obj1); // Object ID 1

            var xref2 = new HpdfXref(10);
            var obj2 = new HpdfNumber(20);
            xref2.Add(obj2); // Object ID 10
            xref2.Previous = xref1;

            // Act
            var entry1 = xref2.GetEntryByObjectId(1);  // From previous xref
            var entry10 = xref2.GetEntryByObjectId(10); // From current xref

            // Assert
            entry1.Should().NotBeNull();
            entry1.Object.Should().Be(obj1);
            entry10.Should().NotBeNull();
            entry10.Object.Should().Be(obj2);
        }

        [Fact]
        public void GetTotalEntryCount_SingleXref_ShouldReturnCount()
        {
            // Arrange
            var xref = new HpdfXref(0);
            xref.Add(new HpdfNull());
            xref.Add(new HpdfBoolean(true));

            // Act
            var count = xref.GetTotalEntryCount();

            // Assert
            count.Should().Be(3); // Free entry + 2 objects
        }

        [Fact]
        public void GetTotalEntryCount_WithPreviousXref_ShouldReturnTotalCount()
        {
            // Arrange
            var xref1 = new HpdfXref(0);
            xref1.Add(new HpdfNull());
            xref1.Add(new HpdfBoolean(true));

            var xref2 = new HpdfXref(10);
            xref2.Add(new HpdfNumber(42));
            xref2.Previous = xref1;

            // Act
            var count = xref2.GetTotalEntryCount();

            // Assert
            count.Should().Be(4); // 3 from xref1 + 1 from xref2
        }

        [Fact]
        public void WriteToStream_SingleObject_ShouldProduceValidPdf()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var obj = new HpdfBoolean(true);
            xref.Add(obj);

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);

            // Assert
            var output = System.Text.Encoding.ASCII.GetString(stream.ToArray());

            // Should contain object definition
            output.Should().Contain("1 0 obj");
            output.Should().Contain("true");
            output.Should().Contain("endobj");

            // Should contain xref table
            output.Should().Contain("xref");
            output.Should().Contain("0 2"); // 0 objects starting at 0 (free entry + 1 object)
            output.Should().Contain("0000000000 65535 f"); // Free entry

            // Should contain trailer
            output.Should().Contain("trailer");
            output.Should().Contain("/Size 2");
            output.Should().Contain("startxref");
            output.Should().Contain("%%EOF");
        }

        [Fact]
        public void WriteToStream_MultipleObjects_ShouldWriteAllObjects()
        {
            // Arrange
            var xref = new HpdfXref(0);
            xref.Add(new HpdfNumber(42));
            xref.Add(new HpdfBoolean(false));
            xref.Add(new HpdfNull());

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);

            // Assert
            var output = System.Text.Encoding.ASCII.GetString(stream.ToArray());

            output.Should().Contain("1 0 obj");
            output.Should().Contain("42");
            output.Should().Contain("2 0 obj");
            output.Should().Contain("false");
            output.Should().Contain("3 0 obj");
            output.Should().Contain("null");
            output.Should().Contain("0 4"); // Free entry + 3 objects
        }

        [Fact]
        public void WriteToStream_ShouldUpdateByteOffsets()
        {
            // Arrange
            var xref = new HpdfXref(0);
            xref.Add(new HpdfNumber(1));
            xref.Add(new HpdfNumber(2));

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);

            // Assert
            xref.Entries[1].ByteOffset.Should().Be(0); // First object starts at offset 0
            xref.Entries[2].ByteOffset.Should().BeGreaterThan(xref.Entries[1].ByteOffset);
        }

        [Fact]
        public void WriteToStream_ShouldSetXrefAddress()
        {
            // Arrange
            var xref = new HpdfXref(0);
            xref.Add(new HpdfNull());

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);

            // Assert
            xref.Address.Should().BeGreaterThan(0);
        }

        [Fact]
        public void WriteToStream_WithPreviousXref_ShouldWriteBoth()
        {
            // Arrange
            var xref1 = new HpdfXref(0);
            xref1.Add(new HpdfNumber(10));

            var xref2 = new HpdfXref(10);
            xref2.Add(new HpdfNumber(20));
            xref2.Previous = xref1;

            var stream = new HpdfMemoryStream();

            // Act
            xref2.WriteToStream(stream);

            // Assert
            var output = System.Text.Encoding.ASCII.GetString(stream.ToArray());

            // Should have both object IDs
            output.Should().Contain("1 0 obj");
            output.Should().Contain("10 0 obj");

            // Should have both xref sections
            output.Should().Contain("0 2"); // First xref
            output.Should().Contain("10 1"); // Second xref

            // Should have Prev entry in trailer
            output.Should().Contain("/Prev");
        }

        [Fact]
        public void Trailer_ShouldBeAccessible()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Act
            xref.Trailer.Add("Root", new HpdfName("1 0 R"));

            // Assert
            xref.Trailer.Count.Should().Be(1);
        }

        [Fact]
        public void MaxXrefEntries_ShouldBeCorrectValue()
        {
            // Assert
            HpdfXref.MaxXrefEntries.Should().Be(8388607);
        }
    }
}
