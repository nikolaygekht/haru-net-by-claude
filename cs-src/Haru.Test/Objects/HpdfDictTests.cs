using System.Text;
using FluentAssertions;
using Haru.Objects;
using Haru.Streams;
using Xunit;

namespace Haru.Test.Objects
{
    public class HpdfDictTests
    {
        private static string WriteToString(HpdfObject obj)
        {
            var stream = new HpdfMemoryStream();
            obj.WriteValue(stream);
            return Encoding.ASCII.GetString(stream.ToArray());
        }

        [Fact]
        public void Dict_Empty_WritesCorrectly()
        {
            var dict = new HpdfDict();

            string result = WriteToString(dict);

            result.Should().Contain("<<");
            result.Should().Contain(">>");
        }

        [Fact]
        public void Dict_WithEntries_WritesCorrectly()
        {
            var dict = new HpdfDict
            {
                { "Type", new HpdfName("Page") },
                { "Count", new HpdfNumber(42) }
            };

            string result = WriteToString(dict);

            result.Should().Contain("/Type /Page");
            result.Should().Contain("/Count 42");
        }

        [Fact]
        public void Dict_Add_AddsEntry()
        {
            var dict = new HpdfDict();

            dict.Add("Key", new HpdfString("Value"));

            dict.Count.Should().Be(1);
            dict.ContainsKey("Key").Should().BeTrue();
        }

        [Fact]
        public void Dict_Remove_RemovesEntry()
        {
            var dict = new HpdfDict { { "Key", new HpdfNumber(1) } };

            bool removed = dict.Remove("Key");

            removed.Should().BeTrue();
            dict.Count.Should().Be(0);
        }

        [Fact]
        public void Dict_Indexer_GetsAndSets()
        {
            var dict = new HpdfDict { { "Key", new HpdfNumber(1) } };

            dict["Key"] = new HpdfNumber(99);

            ((HpdfNumber)dict["Key"]).Value.Should().Be(99);
        }

        [Fact]
        public void Dict_TryGetValue_FindsEntry()
        {
            var dict = new HpdfDict { { "Key", new HpdfNumber(42) } };

            bool found = dict.TryGetValue("Key", out var value);

            found.Should().BeTrue();
            ((HpdfNumber)value).Value.Should().Be(42);
        }

        [Fact]
        public void Dict_TryGetValue_MissingKey_ReturnsFalse()
        {
            var dict = new HpdfDict();

            bool found = dict.TryGetValue("Missing", out var value);

            found.Should().BeFalse();
            value.Should().BeNull();
        }

        [Fact]
        public void Dict_Keys_ReturnsAllKeys()
        {
            var dict = new HpdfDict
            {
                { "Key1", new HpdfNumber(1) },
                { "Key2", new HpdfNumber(2) }
            };

            dict.Keys.Should().Contain("Key1").And.Contain("Key2");
        }

        [Fact]
        public void Dict_Values_ReturnsAllValues()
        {
            var dict = new HpdfDict
            {
                { "Key1", new HpdfNumber(1) },
                { "Key2", new HpdfNumber(2) }
            };

            dict.Values.Should().HaveCount(2);
        }

        [Fact]
        public void Dict_Clear_RemovesAllEntries()
        {
            var dict = new HpdfDict
            {
                { "Key1", new HpdfNumber(1) },
                { "Key2", new HpdfNumber(2) }
            };

            dict.Clear();

            dict.Count.Should().Be(0);
        }

        [Fact]
        public void Dict_Enumeration_Works()
        {
            var dict = new HpdfDict
            {
                { "Key1", new HpdfNumber(1) },
                { "Key2", new HpdfNumber(2) }
            };

            int sum = 0;
            foreach (var kvp in dict)
            {
                sum += ((HpdfNumber)kvp.Value).Value;
            }

            sum.Should().Be(3);
        }

        [Fact]
        public void Dict_Nested_WritesCorrectly()
        {
            var inner = new HpdfDict
            {
                { "Inner", new HpdfNumber(1) }
            };

            var outer = new HpdfDict
            {
                { "Outer", inner }
            };

            string result = WriteToString(outer);

            result.Should().Contain("/Outer");
            result.Should().Contain("/Inner");
        }
    }
}
