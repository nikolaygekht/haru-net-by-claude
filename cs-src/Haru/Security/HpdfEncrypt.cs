using System;
using System.Security.Cryptography;
using Haru.Types;

namespace Haru.Security
{
    /// <summary>
    /// Handles PDF encryption for document security.
    /// Implements standard PDF encryption algorithms (RC4 and AES).
    /// </summary>
    public class HpdfEncrypt
    {
        private const int IdLength = 16;
        private const int PasswordLength = 32;
        private const int EncryptKeyMax = 16;
        private const int Md5KeyLength = 16;

        // Standard PDF password padding string (from PDF specification)
        private static readonly byte[] PaddingString = new byte[]
        {
            0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75, 0x8A, 0x41,
            0x64, 0x00, 0x4E, 0x56, 0xFF, 0xFA, 0x01, 0x08,
            0x2E, 0x2E, 0x00, 0xB6, 0xD0, 0x68, 0x3E, 0x80,
            0x2F, 0x0C, 0xA9, 0xFE, 0x64, 0x53, 0x69, 0x7A
        };

        /// <summary>
        /// Gets or sets the encryption mode (revision).
        /// </summary>
        public HpdfEncryptMode Mode { get; set; }

        /// <summary>
        /// Gets or sets the encryption key length in bytes.
        /// Must be between 5 (40-bit) and 16 (128-bit).
        /// </summary>
        public int KeyLength { get; set; }

        /// <summary>
        /// Gets or sets the document permissions.
        /// </summary>
        public HpdfPermission Permission { get; set; }

        /// <summary>
        /// Gets the owner password (padded, not encrypted).
        /// </summary>
        public byte[] OwnerPassword { get; private set; }

        /// <summary>
        /// Gets the user password (padded, not encrypted).
        /// </summary>
        public byte[] UserPassword { get; private set; }

        /// <summary>
        /// Gets the encrypted owner key.
        /// </summary>
        public byte[] OwnerKey { get; private set; }

        /// <summary>
        /// Gets the encrypted user key.
        /// </summary>
        public byte[] UserKey { get; private set; }

        /// <summary>
        /// Gets or sets the encryption ID (file identifier).
        /// </summary>
        public byte[] EncryptId { get; set; }

        /// <summary>
        /// Gets the encryption key.
        /// </summary>
        public byte[] EncryptionKey { get; private set; }

        /// <summary>
        /// Gets the MD5-based encryption key (with object ID).
        /// </summary>
        public byte[] Md5EncryptionKey { get; private set; }

        private readonly HpdfArc4 _arc4;
        private HpdfAes? _aes;

        /// <summary>
        /// Initializes a new instance of the HpdfEncrypt class.
        /// </summary>
        public HpdfEncrypt()
        {
            Mode = HpdfEncryptMode.R2;
            KeyLength = 5; // 40-bit default
            Permission = HpdfPermission.All | HpdfPermission.PaddingBits;

            OwnerPassword = new byte[PasswordLength];
            UserPassword = new byte[PasswordLength];
            OwnerKey = new byte[PasswordLength];
            UserKey = new byte[PasswordLength];
            EncryptId = new byte[IdLength];
            // EncryptionKey needs space for: key (16) + object ID (3) + gen (2) + "sAlT" (4) = 25 bytes
            EncryptionKey = new byte[Md5KeyLength + 9];
            Md5EncryptionKey = new byte[Md5KeyLength];

            _arc4 = new HpdfArc4();

            // Initialize with default padding
            Array.Copy(PaddingString, OwnerPassword, PasswordLength);
            Array.Copy(PaddingString, UserPassword, PasswordLength);
        }

        /// <summary>
        /// Sets the owner password for the document.
        /// </summary>
        /// <param name="password">The owner password (can open document with full permissions).</param>
        public void SetOwnerPassword(string password)
        {
            PadOrTruncatePassword(password, OwnerPassword);
        }

        /// <summary>
        /// Sets the user password for the document.
        /// </summary>
        /// <param name="password">The user password (opens document with restricted permissions).</param>
        public void SetUserPassword(string password)
        {
            PadOrTruncatePassword(password, UserPassword);
        }

        /// <summary>
        /// Pads or truncates a password to the standard PDF password length (32 bytes).
        /// </summary>
        private static void PadOrTruncatePassword(string password, byte[] output)
        {
            Array.Clear(output, 0, PasswordLength);

            if (string.IsNullOrEmpty(password))
            {
                Array.Copy(PaddingString, output, PasswordLength);
                return;
            }

            // PDF spec requires Latin-1 (ISO-8859-1) encoding for passwords
            byte[] passwordBytes = System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(password);
            int len = Math.Min(passwordBytes.Length, PasswordLength);

            if (len >= PasswordLength)
            {
                Array.Copy(passwordBytes, output, PasswordLength);
            }
            else
            {
                Array.Copy(passwordBytes, output, len);
                Array.Copy(PaddingString, 0, output, len, PasswordLength - len);
            }
        }

        /// <summary>
        /// Creates the encrypted owner key using Algorithm 3.3 from PDF specification.
        /// </summary>
        public void CreateOwnerKey()
        {
            using (var md5 = MD5.Create())
            {
                // Algorithm 3.3 step 2: Compute MD5 hash of owner password
                byte[] digest = md5.ComputeHash(OwnerPassword);

                // Algorithm 3.3 step 3 (Revision 3 or higher): Hash 50 times
                if (Mode >= HpdfEncryptMode.R3)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        digest = md5.ComputeHash(digest, 0, KeyLength);
                    }
                }

                // Algorithm 3.3 step 4-6: Initialize RC4 and encrypt user password
                _arc4.Init(digest, KeyLength);
                byte[] tmpPwd = new byte[PasswordLength];
                _arc4.Crypt(UserPassword, tmpPwd, PasswordLength);

                // Algorithm 3.3 step 7 (Revision 3 or higher): Encrypt 19 more times with modified key
                if (Mode >= HpdfEncryptMode.R3)
                {
                    byte[] tmpPwd2 = new byte[PasswordLength];
                    for (int i = 1; i <= 19; i++)
                    {
                        byte[] newKey = new byte[KeyLength];
                        for (int j = 0; j < KeyLength; j++)
                        {
                            newKey[j] = (byte)(digest[j] ^ i);
                        }

                        Array.Copy(tmpPwd, tmpPwd2, PasswordLength);
                        _arc4.Init(newKey, KeyLength);
                        _arc4.Crypt(tmpPwd2, tmpPwd, PasswordLength);
                    }
                }

                // Algorithm 3.3 step 8: Store result
                Array.Copy(tmpPwd, OwnerKey, PasswordLength);
            }
        }

        /// <summary>
        /// Creates the encryption key using Algorithm 3.2 from PDF specification.
        /// </summary>
        public void CreateEncryptionKey()
        {
            using (var md5 = MD5.Create())
            {
                // Algorithm 3.2 step 2: Hash user password
                md5.TransformBlock(UserPassword, 0, PasswordLength, null, 0);

                // Algorithm 3.2 step 3: Hash owner key
                md5.TransformBlock(OwnerKey, 0, PasswordLength, null, 0);

                // Algorithm 3.2 step 4: Hash permission flags (little-endian)
                byte[] permBytes = new byte[4];
                uint perm = (uint)Permission;
                permBytes[0] = (byte)(perm);
                permBytes[1] = (byte)(perm >> 8);
                permBytes[2] = (byte)(perm >> 16);
                permBytes[3] = (byte)(perm >> 24);
                md5.TransformBlock(permBytes, 0, 4, null, 0);

                // Algorithm 3.2 step 5: Hash encryption ID
                md5.TransformFinalBlock(EncryptId, 0, IdLength);
                byte[]? hash = md5.Hash;

                // Algorithm 3.2 step 6 (Revision 3 or higher): Hash 50 times
                if (Mode >= HpdfEncryptMode.R3)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        hash = md5.ComputeHash(hash!, 0, KeyLength);
                    }
                }

                Array.Copy(hash!, EncryptionKey, KeyLength);
            }
        }

        /// <summary>
        /// Creates the encrypted user key using Algorithm 3.4/3.5 from PDF specification.
        /// </summary>
        public void CreateUserKey()
        {
            if (Mode == HpdfEncryptMode.R2)
            {
                // Algorithm 3.4: Simply encrypt padding string
                _arc4.Init(EncryptionKey, KeyLength);
                _arc4.Crypt(PaddingString, UserKey, PasswordLength);
            }
            else // R3 or higher
            {
                using (var md5 = MD5.Create())
                {
                    // Algorithm 3.5 step 2: Hash padding string
                    md5.TransformBlock(PaddingString, 0, PasswordLength, null, 0);

                    // Algorithm 3.5 step 3: Hash encryption ID
                    md5.TransformFinalBlock(EncryptId, 0, IdLength);
                    byte[]? digest = md5.Hash;

                    // Algorithm 3.5 step 4: Encrypt digest
                    _arc4.Init(EncryptionKey, KeyLength);
                    byte[] digest2 = new byte[Md5KeyLength];
                    _arc4.Crypt(digest!, digest2, Md5KeyLength);

                    // Algorithm 3.5 step 5: Encrypt 19 more times
                    for (int i = 1; i <= 19; i++)
                    {
                        byte[] newKey = new byte[KeyLength];
                        for (int j = 0; j < KeyLength; j++)
                        {
                            newKey[j] = (byte)(EncryptionKey[j] ^ i);
                        }

                        Array.Copy(digest2, digest!, Md5KeyLength);
                        _arc4.Init(newKey, KeyLength);
                        _arc4.Crypt(digest!, digest2, Md5KeyLength);
                    }

                    // Store result (first 16 bytes, rest is arbitrary padding)
                    Array.Clear(UserKey, 0, PasswordLength);
                    Array.Copy(digest2, UserKey, Md5KeyLength);
                }
            }
        }

        /// <summary>
        /// Initializes the encryption key for a specific PDF object.
        /// </summary>
        /// <param name="objectId">The PDF object ID.</param>
        /// <param name="genNo">The generation number.</param>
        public void InitKey(uint objectId, ushort genNo)
        {
            // Append object ID and generation number to encryption key
            EncryptionKey[KeyLength] = (byte)objectId;
            EncryptionKey[KeyLength + 1] = (byte)(objectId >> 8);
            EncryptionKey[KeyLength + 2] = (byte)(objectId >> 16);
            EncryptionKey[KeyLength + 3] = (byte)genNo;
            EncryptionKey[KeyLength + 4] = (byte)(genNo >> 8);

            int hashLength = KeyLength + 5;

            // For AES (R4), append "sAlT" per PDF spec Algorithm 3.1a
            if (Mode == HpdfEncryptMode.R4)
            {
                EncryptionKey[KeyLength + 5] = 0x73; // 's'
                EncryptionKey[KeyLength + 6] = 0x41; // 'A'
                EncryptionKey[KeyLength + 7] = 0x6C; // 'l'
                EncryptionKey[KeyLength + 8] = 0x54; // 'T'
                hashLength = KeyLength + 9;
            }

            // Hash to create object-specific encryption key
            using (var md5 = MD5.Create())
            {
                Md5EncryptionKey = md5.ComputeHash(EncryptionKey, 0, hashLength);
            }

            // Initialize RC4 cipher with object key (not used for AES, but needed for consistency)
            int keyLen = Math.Min(KeyLength + 5, EncryptKeyMax);
            _arc4.Init(Md5EncryptionKey, keyLen);
        }

        /// <summary>
        /// Resets the RC4 cipher to its initialized state.
        /// </summary>
        public void Reset()
        {
            _arc4.Reset();
        }

        /// <summary>
        /// Encrypts a buffer of data using the appropriate algorithm (RC4 or AES).
        /// </summary>
        /// <param name="src">The source data.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="length">The number of bytes to encrypt.</param>
        /// <returns>The length of encrypted data (may be longer for AES due to IV and padding).</returns>
        public int CryptBuffer(byte[] src, byte[] dst, int length)
        {
            ArgumentNullException.ThrowIfNull(src);
            ArgumentNullException.ThrowIfNull(dst);
            if (Mode == HpdfEncryptMode.R4)
            {
                // AES encryption
                _aes ??= new HpdfAes();
                // Always reinitialize with current object-specific key
                _aes.Init(Md5EncryptionKey);
                return _aes.EncryptBuffer(src, dst, length);
            }
            else
            {
                // RC4 encryption
                _arc4.Crypt(src, dst, length);
                return length;
            }
        }

        /// <summary>
        /// Encrypts data and returns a new byte array (convenience method).
        /// </summary>
        /// <param name="data">The data to encrypt.</param>
        /// <returns>The encrypted data.</returns>
        public byte[] Encrypt(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);
            if (Mode == HpdfEncryptMode.R4)
            {
                if (_aes is null)
                {
                    _aes = new HpdfAes();
                }
                // Always reinitialize with current object-specific key
                _aes.Init(Md5EncryptionKey);
                return _aes.Encrypt(data);
            }
            else
            {
                byte[] result = new byte[data.Length];
                _arc4.Crypt(data, result, data.Length);
                return result;
            }
        }

        /// <summary>
        /// Disposes of encryption resources.
        /// </summary>
        public void Dispose()
        {
            _aes?.Dispose();
        }
    }
}
