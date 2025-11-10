using System.Text;
using FluentAssertions;
using Haru.Objects;
using Haru.Streams;
using Xunit;


namespace Haru.Test.Objects
{
    public class HpdfPrimitiveObjectsTests
    {
        private static string WriteToString(HpdfObject obj)
        {
            using var stream = new HpdfMemoryStream();
            obj.WriteValue(stream);
            return System.Text.Encoding.ASCII.GetString(stream.ToArray());
        }

        [Fact]
        public void Null_WritesCorrectly()
        {
            var obj = new HpdfNull();

            string result = WriteToString(obj);

            result.Should().Be("null");
        }

        [Fact]
        public void Null_Singleton_IsSame()
        {
            var obj1 = HpdfNull.Instance;
            var obj2 = HpdfNull.Instance;

            obj1.Should().BeSameAs(obj2);
        }

        [Fact]
        public void Boolean_True_WritesCorrectly()
        {
            var obj = new HpdfBoolean(true);

            string result = WriteToString(obj);

            result.Should().Be("true");
        }

        [Fact]
        public void Boolean_False_WritesCorrectly()
        {
            var obj = new HpdfBoolean(false);

            string result = WriteToString(obj);

            result.Should().Be("false");
        }

        [Fact]
        public void Boolean_Of_ReturnsCorrectSingleton()
        {
            HpdfBoolean.Of(true).Should().BeSameAs(HpdfBoolean.True);
            HpdfBoolean.Of(false).Should().BeSameAs(HpdfBoolean.False);
        }

        [Fact]
        public void Number_Positive_WritesCorrectly()
        {
            var obj = new HpdfNumber(12345);

            string result = WriteToString(obj);

            result.Should().Be("12345");
        }

        [Fact]
        public void Number_Negative_WritesCorrectly()
        {
            var obj = new HpdfNumber(-999);

            string result = WriteToString(obj);

            result.Should().Be("-999");
        }

        [Fact]
        public void Number_Zero_WritesCorrectly()
        {
            var obj = new HpdfNumber(0);

            string result = WriteToString(obj);

            result.Should().Be("0");
        }

        [Fact]
        public void Real_WritesCorrectly()
        {
            var obj = new HpdfReal(3.14159f);

            string result = WriteToString(obj);

            result.Should().Be("3.14159");
        }

        [Fact]
        public void Real_RemovesTrailingZeros()
        {
            var obj = new HpdfReal(5.0f);

            string result = WriteToString(obj);

            result.Should().Be("5");
        }

        [Fact]
        public void Real_InvalidValue_ThrowsException()
        {
            var act = () => new HpdfReal(float.NaN);

            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.RealOutOfRange);
        }

        [Fact]
        public void Name_Simple_WritesCorrectly()
        {
            var obj = new HpdfName("Font");

            string result = WriteToString(obj);

            result.Should().Be("/Font");
        }

        [Fact]
        public void Name_WithSpaces_EscapesCorrectly()
        {
            var obj = new HpdfName("My Font");

            string result = WriteToString(obj);

            result.Should().Be("/My#20Font");
        }

        [Fact]
        public void Name_Empty_ThrowsException()
        {
            var act = () => new HpdfName("");

            act.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.NameInvalidValue);
        }

        [Fact]
        public void Name_Equality_WorksCorrectly()
        {
            var name1 = new HpdfName("Test");
            var name2 = new HpdfName("Test");
            var name3 = new HpdfName("Other");

            name1.Equals(name2).Should().BeTrue();
            name1.Equals(name3).Should().BeFalse();
        }
    }
}
