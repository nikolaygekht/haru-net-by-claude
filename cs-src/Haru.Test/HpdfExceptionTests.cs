using System;
using FluentAssertions;
using Xunit;

namespace Haru.Test
{
    public class HpdfExceptionTests
    {
        [Fact]
        public void Constructor_WithErrorCode_SetsProperties()
        {
            var exception = new HpdfException(HpdfErrorCode.InvalidPage);

            exception.ErrorCode.Should().Be(HpdfErrorCode.InvalidPage);
            exception.DetailCode.Should().Be(0);
            exception.Message.Should().Contain("Invalid page");
        }

        [Fact]
        public void Constructor_WithErrorCodeAndDetailCode_SetsProperties()
        {
            var exception = new HpdfException(HpdfErrorCode.FileIoError, 0x42);

            exception.ErrorCode.Should().Be(HpdfErrorCode.FileIoError);
            exception.DetailCode.Should().Be(0x42);
            exception.Message.Should().Contain("File I/O error");
            exception.Message.Should().Contain("0x0042");
        }

        [Fact]
        public void Constructor_WithCustomMessage_UsesCustomMessage()
        {
            var customMessage = "Custom error message";
            var exception = new HpdfException(HpdfErrorCode.InvalidParameter, customMessage);

            exception.ErrorCode.Should().Be(HpdfErrorCode.InvalidParameter);
            exception.Message.Should().Be(customMessage);
        }

        [Fact]
        public void Constructor_WithInnerException_SetsInnerException()
        {
            var innerException = new InvalidOperationException("Inner error");
            var exception = new HpdfException(HpdfErrorCode.InvalidOperation, innerException);

            exception.ErrorCode.Should().Be(HpdfErrorCode.InvalidOperation);
            exception.InnerException.Should().BeSameAs(innerException);
        }

        [Fact]
        public void Constructor_WithDetailCodeAndInnerException_SetsAllProperties()
        {
            var innerException = new ArgumentException("Argument error");
            var exception = new HpdfException(HpdfErrorCode.InvalidParameter, 0x123, innerException);

            exception.ErrorCode.Should().Be(HpdfErrorCode.InvalidParameter);
            exception.DetailCode.Should().Be(0x123);
            exception.InnerException.Should().BeSameAs(innerException);
            exception.Message.Should().Contain("0x0123");
        }

        [Theory]
        [InlineData(HpdfErrorCode.ArrayCountError, "Array count error")]
        [InlineData(HpdfErrorCode.DictItemNotFound, "Dictionary item not found")]
        [InlineData(HpdfErrorCode.InvalidPngImage, "Invalid PNG image")]
        [InlineData(HpdfErrorCode.FailedToAllocMem, "Failed to allocate memory")]
        [InlineData(HpdfErrorCode.PageInvalidFontSize, "Page has invalid font size")]
        [InlineData(HpdfErrorCode.ZLibError, "ZLib compression error")]
        [InlineData(HpdfErrorCode.InvalidColorSpace, "Invalid color space")]
        [InlineData(HpdfErrorCode.UnsupportedJpegFormat, "Unsupported JPEG format")]
        public void ErrorMessages_ContainExpectedText(HpdfErrorCode errorCode, string expectedText)
        {
            var exception = new HpdfException(errorCode);

            exception.Message.Should().Contain(expectedText);
        }

        [Fact]
        public void Exception_CanBeThrown()
        {
            Action action = () => throw new HpdfException(HpdfErrorCode.InvalidDocument);

            action.Should().Throw<HpdfException>()
                .Which.ErrorCode.Should().Be(HpdfErrorCode.InvalidDocument);
        }

        [Fact]
        public void Exception_CanBeCaught()
        {
            HpdfException caughtException = null;

            try
            {
                throw new HpdfException(HpdfErrorCode.FileOpenError, 0x999);
            }
            catch (HpdfException ex)
            {
                caughtException = ex;
            }

            caughtException.Should().NotBeNull();
            caughtException.ErrorCode.Should().Be(HpdfErrorCode.FileOpenError);
            caughtException.DetailCode.Should().Be(0x999);
        }

        [Fact]
        public void Exception_InheritsFromException()
        {
            var exception = new HpdfException(HpdfErrorCode.InvalidFont);

            exception.Should().BeAssignableTo<Exception>();
        }

        [Fact]
        public void ErrorCode_AllValuesMapped()
        {
            // Test a representative sample of error codes to ensure they all have messages
            var errorCodes = new[]
            {
                HpdfErrorCode.ArrayCountError,
                HpdfErrorCode.BinaryLengthError,
                HpdfErrorCode.CannotGetPalette,
                HpdfErrorCode.EncryptInvalidPassword,
                HpdfErrorCode.ExceedGStateLimit,
                HpdfErrorCode.InvalidAnnotation,
                HpdfErrorCode.InvalidDateTime,
                HpdfErrorCode.InvalidEncoder,
                HpdfErrorCode.InvalidFont,
                HpdfErrorCode.LibPngError,
                HpdfErrorCode.PageOutOfRange,
                HpdfErrorCode.StreamEof,
                HpdfErrorCode.TtfInvalidCmap,
                HpdfErrorCode.UnsupportedFunc,
                HpdfErrorCode.XRefCountError,
                HpdfErrorCode.InvalidIccComponentNum
            };

            foreach (var errorCode in errorCodes)
            {
                var exception = new HpdfException(errorCode);
                exception.Message.Should().NotStartWith("Unknown error code");
            }
        }

        [Fact]
        public void UnknownErrorCode_GeneratesGenericMessage()
        {
            var unknownCode = (HpdfErrorCode)0x9999;
            var exception = new HpdfException(unknownCode);

            exception.Message.Should().Contain("Unknown error code");
            exception.Message.Should().Contain("0x9999");
        }
    }
}
