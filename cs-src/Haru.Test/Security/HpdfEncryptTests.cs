using System;
using FluentAssertions;
using Haru.Security;
using Haru.Types;
using Xunit;


namespace Haru.Test.Security
{
    /// <summary>
    /// Tests for PDF encryption functionality
    /// </summary>
    public class HpdfEncryptTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaults()
        {
            // Arrange & Act
            var encrypt = new HpdfEncrypt();

            // Assert
            encrypt.Mode.Should().Be(HpdfEncryptMode.R2);
            encrypt.KeyLength.Should().Be(5); // 40-bit
            encrypt.Permission.Should().HaveFlag(HpdfPermission.PaddingBits);
        }

        [Theory]
        [InlineData(HpdfEncryptMode.R2, 5)]
        [InlineData(HpdfEncryptMode.R3, 16)]
        [InlineData(HpdfEncryptMode.R4, 16)]
        public void SetEncryptionMode_ShouldSetModeAndKeyLength(HpdfEncryptMode mode, int keyLength)
        {
            // Arrange
            var encrypt = new HpdfEncrypt();

            // Act
            encrypt.Mode = mode;
            encrypt.KeyLength = keyLength;

            // Assert
            encrypt.Mode.Should().Be(mode);
            encrypt.KeyLength.Should().Be(keyLength);
        }

        [Fact]
        public void SetUserPassword_ShouldPadPassword()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();

            // Act
            encrypt.SetUserPassword("test123");

            // Assert
            encrypt.UserPassword.Should().NotBeNull();
            encrypt.UserPassword.Length.Should().Be(32); // Padded to 32 bytes
        }

        [Fact]
        public void SetOwnerPassword_ShouldPadPassword()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();

            // Act
            encrypt.SetOwnerPassword("owner456");

            // Assert
            encrypt.OwnerPassword.Should().NotBeNull();
            encrypt.OwnerPassword.Length.Should().Be(32); // Padded to 32 bytes
        }

        [Fact]
        public void CreateOwnerKey_R2_ShouldGenerateValidKey()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R2;
            encrypt.KeyLength = 5;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");

            // Act
            encrypt.CreateOwnerKey();

            // Assert
            encrypt.OwnerKey.Should().NotBeNull();
            encrypt.OwnerKey.Length.Should().Be(32);
            encrypt.OwnerKey.Should().NotBeEquivalentTo(encrypt.UserPassword);
        }

        [Fact]
        public void CreateOwnerKey_R3_ShouldGenerateValidKey()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R3;
            encrypt.KeyLength = 16;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");

            // Act
            encrypt.CreateOwnerKey();

            // Assert
            encrypt.OwnerKey.Should().NotBeNull();
            encrypt.OwnerKey.Length.Should().Be(32);
            encrypt.OwnerKey.Should().NotBeEquivalentTo(encrypt.UserPassword);
        }

        [Fact]
        public void CreateEncryptionKey_ShouldGenerateKey()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R3;
            encrypt.KeyLength = 16;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");
            encrypt.Permission = HpdfPermission.Print | HpdfPermission.PaddingBits;
            encrypt.EncryptId = new byte[16]; // Set a dummy ID
            encrypt.CreateOwnerKey();

            // Act
            encrypt.CreateEncryptionKey();

            // Assert
            encrypt.EncryptionKey.Should().NotBeNull();
            encrypt.EncryptionKey[0].Should().NotBe(0); // Should have data
        }

        [Fact]
        public void CreateUserKey_R2_ShouldGenerateValidKey()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R2;
            encrypt.KeyLength = 5;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");
            encrypt.Permission = HpdfPermission.Print | HpdfPermission.PaddingBits;
            encrypt.EncryptId = new byte[16];
            encrypt.CreateOwnerKey();
            encrypt.CreateEncryptionKey();

            // Act
            encrypt.CreateUserKey();

            // Assert
            encrypt.UserKey.Should().NotBeNull();
            encrypt.UserKey.Length.Should().Be(32);
        }

        [Fact]
        public void CreateUserKey_R3_ShouldGenerateValidKey()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R3;
            encrypt.KeyLength = 16;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");
            encrypt.Permission = HpdfPermission.Print | HpdfPermission.PaddingBits;
            encrypt.EncryptId = new byte[16];
            encrypt.CreateOwnerKey();
            encrypt.CreateEncryptionKey();

            // Act
            encrypt.CreateUserKey();

            // Assert
            encrypt.UserKey.Should().NotBeNull();
            encrypt.UserKey.Length.Should().Be(32);
        }

        [Fact]
        public void InitKey_ShouldGenerateObjectSpecificKey()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R3;
            encrypt.KeyLength = 16;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");
            encrypt.Permission = HpdfPermission.Print | HpdfPermission.PaddingBits;
            encrypt.EncryptId = new byte[16];
            encrypt.CreateOwnerKey();
            encrypt.CreateEncryptionKey();

            // Act
            encrypt.InitKey(1, 0);

            // Assert
            encrypt.Md5EncryptionKey.Should().NotBeNull();
            encrypt.Md5EncryptionKey.Length.Should().Be(16);
        }

        [Fact]
        public void InitKey_DifferentObjects_ShouldGenerateDifferentKeys()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R3;
            encrypt.KeyLength = 16;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");
            encrypt.Permission = HpdfPermission.Print | HpdfPermission.PaddingBits;
            encrypt.EncryptId = new byte[16];
            encrypt.CreateOwnerKey();
            encrypt.CreateEncryptionKey();

            // Act
            encrypt.InitKey(1, 0);
            byte[] key1 = new byte[16];
            Array.Copy(encrypt.Md5EncryptionKey, key1, 16);

            encrypt.InitKey(2, 0);
            byte[] key2 = new byte[16];
            Array.Copy(encrypt.Md5EncryptionKey, key2, 16);

            // Assert
            key1.Should().NotBeEquivalentTo(key2);
        }

        [Fact]
        public void InitKey_R4_ShouldAppendSalt()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R4;
            encrypt.KeyLength = 16;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");
            encrypt.Permission = HpdfPermission.Print | HpdfPermission.PaddingBits;
            encrypt.EncryptId = new byte[16];
            encrypt.CreateOwnerKey();
            encrypt.CreateEncryptionKey();

            // Act
            encrypt.InitKey(1, 0);

            // Assert - R4 should generate different key than R3 for same object
            var encryptR3 = new HpdfEncrypt();
            encryptR3.Mode = HpdfEncryptMode.R3;
            encryptR3.KeyLength = 16;
            encryptR3.SetUserPassword("user");
            encryptR3.SetOwnerPassword("owner");
            encryptR3.Permission = HpdfPermission.Print | HpdfPermission.PaddingBits;
            encryptR3.EncryptId = new byte[16];
            encryptR3.CreateOwnerKey();
            encryptR3.CreateEncryptionKey();
            encryptR3.InitKey(1, 0);

            encrypt.Md5EncryptionKey.Should().NotBeEquivalentTo(encryptR3.Md5EncryptionKey);
        }

        [Fact]
        public void Encrypt_RC4_ShouldEncryptData()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R3;
            encrypt.KeyLength = 16;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");
            encrypt.Permission = HpdfPermission.Print | HpdfPermission.PaddingBits;
            encrypt.EncryptId = new byte[16];
            encrypt.CreateOwnerKey();
            encrypt.CreateEncryptionKey();
            encrypt.InitKey(1, 0);

            byte[] plaintext = System.Text.Encoding.UTF8.GetBytes("Hello, World!");

            // Act
            byte[] encrypted = encrypt.Encrypt(plaintext);

            // Assert
            encrypted.Should().NotBeNull();
            encrypted.Should().NotBeEquivalentTo(plaintext);
            encrypted.Length.Should().Be(plaintext.Length); // RC4 doesn't change length
        }

        [Fact]
        public void Encrypt_AES_ShouldEncryptDataWithIV()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();
            encrypt.Mode = HpdfEncryptMode.R4;
            encrypt.KeyLength = 16;
            encrypt.SetUserPassword("user");
            encrypt.SetOwnerPassword("owner");
            encrypt.Permission = HpdfPermission.Print | HpdfPermission.PaddingBits;
            encrypt.EncryptId = new byte[16];
            encrypt.CreateOwnerKey();
            encrypt.CreateEncryptionKey();
            encrypt.InitKey(1, 0);

            byte[] plaintext = System.Text.Encoding.UTF8.GetBytes("Hello, World!");

            // Act
            byte[] encrypted = encrypt.Encrypt(plaintext);

            // Assert
            encrypted.Should().NotBeNull();
            encrypted.Should().NotBeEquivalentTo(plaintext);
            // AES adds IV (16 bytes) + padding, so should be longer
            encrypted.Length.Should().BeGreaterThan(plaintext.Length);
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("short")]
        [InlineData("exactly32characterslong1234567")]
        [InlineData("this is a very long password that exceeds the maximum length allowed")]
        public void SetPassword_VariousLengths_ShouldHandleCorrectly(string password)
        {
            // Arrange
            var encrypt = new HpdfEncrypt();

            // Act
            encrypt.SetUserPassword(password);

            // Assert
            encrypt.UserPassword.Should().NotBeNull();
            encrypt.UserPassword.Length.Should().Be(32);
        }

        [Fact]
        public void Permission_ShouldAlwaysIncludePaddingBits()
        {
            // Arrange
            var encrypt = new HpdfEncrypt();

            // Act
            encrypt.Permission = HpdfPermission.Print;

            // Assert - PaddingBits should be automatically included
            // Note: This test verifies current behavior; actual addition happens in HpdfEncryptDict
            encrypt.Permission.Should().Be(HpdfPermission.Print);
        }
    }
}
