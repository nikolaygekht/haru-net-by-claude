using System;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using FluentAssertions;
using Haru.Xref;
using Haru.Objects;
using Haru.Streams;

#pragma warning disable CA2000 // Dispose objects before losing scope

namespace Haru.Test.Xref
{
    /// <summary>
    /// Integration tests for HpdfXref using real PDF data structures
    /// </summary>
    public class HpdfXrefIntegrationTests
    {
        [Fact]
        public void WriteToStream_ShouldProduceValidMinimalPdf()
        {
            // Arrange - Create a minimal PDF document structure
            var xref = new HpdfXref(0);

            // Create a catalog (root)
            var catalog = new HpdfDict();
            catalog.Add("Type", new HpdfName("Catalog"));
            var catalogId = xref.Add(catalog);

            // Create a pages object
            var pages = new HpdfDict();
            pages.Add("Type", new HpdfName("Pages"));
            pages.Add("Count", new HpdfNumber(0));
            pages.Add("Kids", new HpdfArray());
            xref.Add(pages);

            // Link catalog to pages
            catalog.Add("Pages", new HpdfNumber(2)); // Reference to object 2

            // Set up trailer
            xref.Trailer.Add("Root", new HpdfNumber((int)catalogId));

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);

            // Assert
            var output = Encoding.ASCII.GetString(stream.ToArray());

            // Validate structure
            output.Should().Contain("1 0 obj"); // Catalog
            output.Should().Contain("/Type /Catalog");
            output.Should().Contain("2 0 obj"); // Pages
            output.Should().Contain("/Type /Pages");
            output.Should().Contain("xref");
            output.Should().Contain("trailer");
            output.Should().Contain("/Root 1");
            output.Should().Contain("startxref");
            output.Should().Contain("%%EOF");
        }

        [Fact]
        public void WriteToStream_ShouldProduceValidXrefTableFormat()
        {
            // Arrange
            var xref = new HpdfXref(0);
            xref.Add(new HpdfNumber(42));
            xref.Add(new HpdfBoolean(true));

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);
            var output = Encoding.ASCII.GetString(stream.ToArray());

            // Assert - Validate xref format
            var xrefMatch = Regex.Match(output, @"xref\n0 3\n");
            xrefMatch.Success.Should().BeTrue("xref header should be present");

            // Check free entry format (entry 0)
            var freeEntryMatch = Regex.Match(output, @"0000000000 65535 f \r\n");
            freeEntryMatch.Success.Should().BeTrue("free entry should be formatted correctly");

            // Check in-use entries have correct format (10 digit offset, 5 digit gen, type, CRLF)
            var entryMatches = Regex.Matches(output, @"\d{10} \d{5} [nf] \r\n");
            entryMatches.Count.Should().Be(3, "should have 3 xref entries (1 free + 2 objects)");
        }

        [Fact]
        public void WriteToStream_WithComplexObjects_ShouldMaintainReferences()
        {
            // Arrange - Create objects with references
            var xref = new HpdfXref(0);

            var dict1 = new HpdfDict();
            dict1.Add("Name", new HpdfName("First"));
            var id1 = xref.Add(dict1);

            var dict2 = new HpdfDict();
            dict2.Add("Name", new HpdfName("Second"));
            dict2.Add("Ref", new HpdfNumber((int)id1)); // Reference to first object
            xref.Add(dict2);

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);
            var output = Encoding.ASCII.GetString(stream.ToArray());

            // Assert
            output.Should().Contain("1 0 obj");
            output.Should().Contain("/Name /First");
            output.Should().Contain("2 0 obj");
            output.Should().Contain("/Name /Second");
            output.Should().Contain("/Ref 1"); // Reference preserved
        }

        [Fact]
        public void WriteToStream_WithArrayAndDict_ShouldProduceCorrectStructure()
        {
            // Arrange
            var xref = new HpdfXref(0);

            // Create an array
            var array = new HpdfArray();
            array.Add(new HpdfNumber(1));
            array.Add(new HpdfNumber(2));
            array.Add(new HpdfNumber(3));
            xref.Add(array);

            // Create a dict with nested structures
            var dict = new HpdfDict();
            dict.Add("Type", new HpdfName("Test"));
            dict.Add("Numbers", array);
            xref.Add(dict);

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);
            var output = Encoding.ASCII.GetString(stream.ToArray());

            // Assert
            output.Should().Contain("1 0 obj");
            output.Should().Contain("[1 2 3]");
            output.Should().Contain("2 0 obj");
            output.Should().Contain("/Type /Test");
        }

        [Fact]
        public void WriteToStream_IncrementalUpdate_ShouldLinkPreviousXref()
        {
            // Arrange - Simulate incremental update
            var xref1 = new HpdfXref(0);
            xref1.Add(new HpdfNumber(1));
            xref1.Add(new HpdfNumber(2));

            var stream1 = new HpdfMemoryStream();
            xref1.WriteToStream(stream1);

            // Create incremental update
            var xref2 = new HpdfXref(10);
            xref2.Add(new HpdfNumber(10));
            xref2.Previous = xref1;

            var stream2 = new HpdfMemoryStream();

            // Act
            xref2.WriteToStream(stream2);
            var output = Encoding.ASCII.GetString(stream2.ToArray());

            // Assert
            output.Should().Contain("1 0 obj"); // From previous xref
            output.Should().Contain("10 0 obj"); // From current xref
            output.Should().Contain("0 3"); // First xref subsection
            output.Should().Contain("10 1"); // Second xref subsection
            output.Should().Contain($"/Prev {xref1.Address}"); // Link to previous
        }

        [Fact]
        public void WriteToStream_VerifyByteOffsets_ShouldBeAccurate()
        {
            // Arrange
            var xref = new HpdfXref(0);
            var obj1 = new HpdfNumber(42);
            var obj2 = new HpdfBoolean(true);
            xref.Add(obj1);
            xref.Add(obj2);

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);

            // Assert - Verify byte offsets match actual positions
            var output = Encoding.ASCII.GetString(stream.ToArray());
            var offset1 = xref.Entries[1].ByteOffset;
            var offset2 = xref.Entries[2].ByteOffset;

            // Check that "1 0 obj" appears at the recorded offset
            var actualOffset1 = output.IndexOf("1 0 obj", StringComparison.Ordinal);
            actualOffset1.Should().Be((int)offset1);

            var actualOffset2 = output.IndexOf("2 0 obj", StringComparison.Ordinal);
            actualOffset2.Should().Be((int)offset2);
        }

        [Fact]
        public void WriteToStream_VerifyXrefAddress_ShouldPointToXrefTable()
        {
            // Arrange
            var xref = new HpdfXref(0);
            xref.Add(new HpdfNull());

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);

            // Assert
            var output = Encoding.ASCII.GetString(stream.ToArray());
            var xrefAddress = xref.Address;

            // Find where "xref" keyword appears
            var actualXrefPosition = output.IndexOf("xref\n", StringComparison.Ordinal);
            actualXrefPosition.Should().Be((int)xrefAddress);

            // Verify startxref points to the correct location
            var startxrefMatch = Regex.Match(output, @"startxref\n(\d+)");
            startxrefMatch.Success.Should().BeTrue();
            var startxrefValue = uint.Parse(startxrefMatch.Groups[1].Value);
            startxrefValue.Should().Be(xrefAddress);
        }

        [Fact]
        public void WriteToStream_WithStreamObject_ShouldProduceCompleteStructure()
        {
            // Arrange
            var xref = new HpdfXref(0);

            var streamObj = new HpdfStreamObject();
            streamObj.WriteToStream(Encoding.ASCII.GetBytes("Hello PDF"));
            streamObj.Add("Type", new HpdfName("Stream"));
            xref.Add(streamObj);

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);
            var output = Encoding.ASCII.GetString(stream.ToArray());

            // Assert
            output.Should().Contain("1 0 obj");
            output.Should().Contain("/Type /Stream");
            output.Should().Contain("stream\n");
            output.Should().Contain("Hello PDF");
            output.Should().Contain("endstream");
        }

        [Fact]
        public void WriteToStream_LargePdf_ShouldHandleManyObjects()
        {
            // Arrange - Create a PDF with many objects
            var xref = new HpdfXref(0);

            for (int i = 0; i < 100; i++)
            {
                var dict = new HpdfDict();
                dict.Add("Index", new HpdfNumber(i));
                xref.Add(dict);
            }

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);
            var output = Encoding.ASCII.GetString(stream.ToArray());

            // Assert
            output.Should().Contain("1 0 obj");
            output.Should().Contain("100 0 obj");
            output.Should().Contain("0 101"); // Free entry + 100 objects
            output.Should().Contain("/Size 101");

            // Verify all objects are present
            for (int i = 1; i <= 100; i++)
            {
                output.Should().Contain($"{i} 0 obj");
            }
        }

        [Fact]
        public void RealWorldScenario_PageTree_ShouldProduceValidStructure()
        {
            // Arrange - Build a more realistic page tree structure
            var xref = new HpdfXref(0);

            // 1. Catalog
            var catalog = new HpdfDict();
            catalog.Add("Type", new HpdfName("Catalog"));
            xref.Add(catalog);

            // 2. Pages (tree root)
            var pages = new HpdfDict();
            pages.Add("Type", new HpdfName("Pages"));
            pages.Add("Count", new HpdfNumber(1));
            var kidsArray = new HpdfArray();
            pages.Add("Kids", kidsArray);
            xref.Add(pages);

            // 3. Page object
            var page = new HpdfDict();
            page.Add("Type", new HpdfName("Page"));
            page.Add("Parent", new HpdfNumber(2)); // Reference to pages
            xref.Add(page);

            // Link everything together
            catalog.Add("Pages", new HpdfNumber(2));
            kidsArray.Add(new HpdfNumber(3)); // Reference to page

            var stream = new HpdfMemoryStream();

            // Act
            xref.WriteToStream(stream);
            var output = Encoding.ASCII.GetString(stream.ToArray());

            // Assert
            output.Should().Contain("1 0 obj");
            output.Should().Contain("/Type /Catalog");
            output.Should().Contain("/Pages 2");

            output.Should().Contain("2 0 obj");
            output.Should().Contain("/Type /Pages");
            output.Should().Contain("/Count 1");
            output.Should().Contain("/Kids [3]");

            output.Should().Contain("3 0 obj");
            output.Should().Contain("/Type /Page");
            output.Should().Contain("/Parent 2");

            // Validate PDF structure
            output.Should().Contain("xref");
            output.Should().Contain("trailer");
            output.Should().Contain("%%EOF");
        }
    }
}
