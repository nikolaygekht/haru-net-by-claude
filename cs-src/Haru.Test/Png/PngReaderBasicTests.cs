using System;
using System.IO;
using FluentAssertions;
using Haru.Png;
using Xunit;


namespace Haru.Test.Png
{
    /// <summary>
    /// Basic validation tests for PNG reader.
    /// </summary>
    public class PngReaderBasicTests
    {
        private readonly IPngReader _pngReader;

        public PngReaderBasicTests()
        {
            _pngReader = PngReaderFactory.Create();
        }

        [Fact]
        public void ValidateSignature_WithValidPngStream_ReturnsTrue()
        {
            // Arrange
            byte[] validPngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            using (var stream = new MemoryStream(validPngSignature))
            {
                // Act
                bool result = _pngReader.ValidateSignature(stream);

                // Assert
                result.Should().BeTrue();
                stream.Position.Should().Be(0); // Position should be restored
            }
        }

        [Fact]
        public void ValidateSignature_WithInvalidSignature_ReturnsFalse()
        {
            // Arrange
            byte[] invalidSignature = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (var stream = new MemoryStream(invalidSignature))
            {
                // Act
                bool result = _pngReader.ValidateSignature(stream);

                // Assert
                result.Should().BeFalse();
            }
        }

        [Fact]
        public void ValidateSignature_WithNullStream_ReturnsFalse()
        {
            // Act
            bool result = _pngReader.ValidateSignature(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateSignature_WithShortStream_ReturnsFalse()
        {
            // Arrange
            byte[] shortData = new byte[] { 137, 80, 78 };
            using (var stream = new MemoryStream(shortData))
            {
                // Act
                bool result = _pngReader.ValidateSignature(stream);

                // Assert
                result.Should().BeFalse();
            }
        }

        [Fact]
        public void ReadImageInfo_WithNullStream_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () => _pngReader.ReadImageInfo(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReadPixelData_WithNullStream_ThrowsArgumentNullException()
        {
            // Arrange
            var info = new PngImageInfo { Width = 100, Height = 100 };

            // Act & Assert
            Action act = () => _pngReader.ReadPixelData(null!, info);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReadPixelData_WithNullInfo_ThrowsArgumentNullException()
        {
            // Arrange
            using (var stream = new MemoryStream())
            {
                // Act & Assert
                Action act = () => _pngReader.ReadPixelData(stream, null!);
                act.Should().Throw<ArgumentNullException>();
            }
        }

        [Fact]
        public void ReadPixelDataByRow_WithNullStream_ThrowsArgumentNullException()
        {
            // Arrange
            var info = new PngImageInfo { Width = 100, Height = 100 };
            Action<byte[], int> callback = (data, row) => { };

            // Act & Assert
            Action act = () => _pngReader.ReadPixelDataByRow(null!, info, callback);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReadPixelDataByRow_WithNullInfo_ThrowsArgumentNullException()
        {
            // Arrange
            using (var stream = new MemoryStream())
            {
                Action<byte[], int> callback = (data, row) => { };

                // Act & Assert
                Action act = () => _pngReader.ReadPixelDataByRow(stream, null!, callback);
                act.Should().Throw<ArgumentNullException>();
            }
        }

        [Fact]
        public void ReadPixelDataByRow_WithNullCallback_ThrowsArgumentNullException()
        {
            // Arrange
            using (var stream = new MemoryStream())
            {
                var info = new PngImageInfo { Width = 100, Height = 100 };

                // Act & Assert
                Action act = () => _pngReader.ReadPixelDataByRow(stream, info, null!);
                act.Should().Throw<ArgumentNullException>();
            }
        }
    }
}
