using Xunit;
using FluentAssertions;
using Haru.Xref;
using Haru.Objects;
using Haru.Streams;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Xref
{
    public class HpdfXrefEntryTests
    {
        [Fact]
        public void Constructor_ShouldCreateInUseEntry()
        {
            // Act
            var entry = new HpdfXrefEntry();

            // Assert
            entry.EntryType.Should().Be(HpdfXrefEntryType.InUse);
            entry.ByteOffset.Should().Be(0);
            entry.GenerationNumber.Should().Be(0);
            entry.Object.Should().BeNull();
        }

        [Fact]
        public void CreateFreeEntry_ShouldReturnFreeEntry()
        {
            // Act
            var entry = HpdfXrefEntry.CreateFreeEntry();

            // Assert
            entry.EntryType.Should().Be(HpdfXrefEntryType.Free);
            entry.ByteOffset.Should().Be(0);
            entry.GenerationNumber.Should().Be(HpdfXrefEntry.MaxGenerationNumber);
            entry.Object.Should().BeNull();
        }

        [Fact]
        public void CreateInUseEntry_ShouldReturnInUseEntryWithObject()
        {
            // Arrange
            var obj = new HpdfNull();

            // Act
            var entry = HpdfXrefEntry.CreateInUseEntry(obj);

            // Assert
            entry.EntryType.Should().Be(HpdfXrefEntryType.InUse);
            entry.ByteOffset.Should().Be(0);
            entry.GenerationNumber.Should().Be(0);
            entry.Object.Should().Be(obj);
        }

        [Fact]
        public void WriteToStream_ShouldWriteInUseEntryCorrectly()
        {
            // Arrange
            var stream = new HpdfMemoryStream();
            var entry = new HpdfXrefEntry
            {
                ByteOffset = 17,
                GenerationNumber = 0,
                EntryType = HpdfXrefEntryType.InUse
            };

            // Act
            entry.WriteToStream(stream);

            // Assert
            var output = stream.ToArray();
            var text = System.Text.Encoding.ASCII.GetString(output);
            text.Should().Be("0000000017 00000 n \r\n");
        }

        [Fact]
        public void WriteToStream_ShouldWriteFreeEntryCorrectly()
        {
            // Arrange
            var stream = new HpdfMemoryStream();
            var entry = HpdfXrefEntry.CreateFreeEntry();

            // Act
            entry.WriteToStream(stream);

            // Assert
            var output = stream.ToArray();
            var text = System.Text.Encoding.ASCII.GetString(output);
            text.Should().Be("0000000000 65535 f \r\n");
        }

        [Fact]
        public void WriteToStream_ShouldFormatLargeOffsetCorrectly()
        {
            // Arrange
            var stream = new HpdfMemoryStream();
            var entry = new HpdfXrefEntry
            {
                ByteOffset = 123456789,
                GenerationNumber = 99,
                EntryType = HpdfXrefEntryType.InUse
            };

            // Act
            entry.WriteToStream(stream);

            // Assert
            var output = stream.ToArray();
            var text = System.Text.Encoding.ASCII.GetString(output);
            text.Should().Be("0123456789 00099 n \r\n");
        }

        [Fact]
        public void EntryType_ShouldUseCorrectCharValues()
        {
            // Assert
            ((char)HpdfXrefEntryType.Free).Should().Be('f');
            ((char)HpdfXrefEntryType.InUse).Should().Be('n');
        }

        [Fact]
        public void MaxGenerationNumber_ShouldBe65535()
        {
            // Assert
            HpdfXrefEntry.MaxGenerationNumber.Should().Be(65535);
        }

        [Fact]
        public void Properties_ShouldBeSettable()
        {
            // Arrange
            var entry = new HpdfXrefEntry();
            var obj = new HpdfBoolean(true);

            // Act
            entry.ByteOffset = 1000;
            entry.GenerationNumber = 5;
            entry.EntryType = HpdfXrefEntryType.Free;
            entry.Object = obj;

            // Assert
            entry.ByteOffset.Should().Be(1000);
            entry.GenerationNumber.Should().Be(5);
            entry.EntryType.Should().Be(HpdfXrefEntryType.Free);
            entry.Object.Should().Be(obj);
        }

        [Fact]
        public void WriteToStream_ShouldHandleMaximumValues()
        {
            // Arrange
            var stream = new HpdfMemoryStream();
            var entry = new HpdfXrefEntry
            {
                ByteOffset = 4294967295, // Max uint value (fits in 10 digits)
                GenerationNumber = HpdfXrefEntry.MaxGenerationNumber,
                EntryType = HpdfXrefEntryType.InUse
            };

            // Act
            entry.WriteToStream(stream);

            // Assert
            var output = stream.ToArray();
            var text = System.Text.Encoding.ASCII.GetString(output);
            text.Should().Be("4294967295 65535 n \r\n");
        }
    }
}
