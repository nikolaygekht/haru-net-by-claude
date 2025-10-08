using System.Text;
using FluentAssertions;
using Haru.Objects;
using Haru.Streams;
using Haru.Types;
using Xunit;

namespace Haru.Test.Objects
{
    public class HpdfArrayTests
    {
        private static string WriteToString(HpdfObject obj)
        {
            var stream = new HpdfMemoryStream();
            obj.WriteValue(stream);
            return Encoding.ASCII.GetString(stream.ToArray());
        }

        [Fact]
        public void Array_Empty_WritesCorrectly()
        {
            var array = new HpdfArray();

            string result = WriteToString(array);

            result.Should().Be("[]");
        }

        [Fact]
        public void Array_WithItems_WritesCorrectly()
        {
            var array = new HpdfArray
            {
                new HpdfNumber(1),
                new HpdfNumber(2),
                new HpdfNumber(3)
            };

            string result = WriteToString(array);

            result.Should().Be("[1 2 3]");
        }

        [Fact]
        public void Array_Mixed_WritesCorrectly()
        {
            var array = new HpdfArray
            {
                new HpdfName("Type"),
                new HpdfString("Value"),
                new HpdfNumber(42)
            };

            string result = WriteToString(array);

            result.Should().Be("[/Type (Value) 42]");
        }

        [Fact]
        public void Array_FromRect_CreatesCorrectArray()
        {
            var rect = new HpdfRect(10, 20, 100, 200);
            var array = new HpdfArray(rect);

            array.Count.Should().Be(4);
            ((HpdfReal)array[0]).Value.Should().Be(10);
            ((HpdfReal)array[1]).Value.Should().Be(20);
            ((HpdfReal)array[2]).Value.Should().Be(100);
            ((HpdfReal)array[3]).Value.Should().Be(200);
        }

        [Fact]
        public void Array_Add_AddsItem()
        {
            var array = new HpdfArray();

            array.Add(new HpdfNumber(5));

            array.Count.Should().Be(1);
        }

        [Fact]
        public void Array_Remove_RemovesItem()
        {
            var item = new HpdfNumber(5);
            var array = new HpdfArray { item };

            bool removed = array.Remove(item);

            removed.Should().BeTrue();
            array.Count.Should().Be(0);
        }

        [Fact]
        public void Array_IndexOf_FindsItem()
        {
            var item = new HpdfNumber(5);
            var array = new HpdfArray
            {
                new HpdfNumber(1),
                item,
                new HpdfNumber(3)
            };

            int index = array.IndexOf(item);

            index.Should().Be(1);
        }

        [Fact]
        public void Array_Clear_RemovesAllItems()
        {
            var array = new HpdfArray
            {
                new HpdfNumber(1),
                new HpdfNumber(2),
                new HpdfNumber(3)
            };

            array.Clear();

            array.Count.Should().Be(0);
        }

        [Fact]
        public void Array_Indexer_GetsAndSets()
        {
            var array = new HpdfArray { new HpdfNumber(1) };

            array[0] = new HpdfNumber(99);

            ((HpdfNumber)array[0]).Value.Should().Be(99);
        }

        [Fact]
        public void Array_Contains_FindsItem()
        {
            var item = new HpdfNumber(5);
            var array = new HpdfArray { item };

            array.Contains(item).Should().BeTrue();
        }

        [Fact]
        public void Array_Enumeration_Works()
        {
            var array = new HpdfArray
            {
                new HpdfNumber(1),
                new HpdfNumber(2),
                new HpdfNumber(3)
            };

            int sum = 0;
            foreach (var item in array)
            {
                sum += ((HpdfNumber)item).Value;
            }

            sum.Should().Be(6);
        }
    }
}
