using System;
using System.Security.Cryptography;

namespace Haru.Security
{
    /// <summary>
    /// AES encryption support for PDF documents (Revision 4+).
    /// Implements AES-128 in CBC mode as required by PDF specification.
    /// </summary>
    internal class HpdfAes
    {
        private const int BlockSize = 16; // AES block size is always 128 bits (16 bytes)
        private const int KeySize = 16;   // AES-128 uses 16-byte (128-bit) key

        private byte[] _key = Array.Empty<byte>();
        private Aes _aes;

        /// <summary>
        /// Initializes a new instance of the HpdfAes class.
        /// </summary>
        public HpdfAes()
        {
            _aes = Aes.Create();
            _aes.Mode = CipherMode.CBC;
            _aes.Padding = PaddingMode.PKCS7;
            _aes.KeySize = 128; // 128-bit AES
            _aes.BlockSize = 128;
        }

        /// <summary>
        /// Initializes the AES cipher with the specified key.
        /// </summary>
        /// <param name="key">The encryption key (must be 16 bytes for AES-128).</param>
        public void Init(byte[] key)
        {
            if (key.Length < KeySize)
            {
                // Pad key if necessary
                _key = new byte[KeySize];
                Array.Copy(key, _key, key.Length);
            }
            else
            {
                _key = new byte[KeySize];
                Array.Copy(key, _key, KeySize);
            }

            _aes.Key = _key;
        }

        /// <summary>
        /// Encrypts data using AES-128 in CBC mode.
        /// The first 16 bytes of output will be the IV, followed by the encrypted data.
        /// </summary>
        /// <param name="input">The plaintext data to encrypt.</param>
        /// <returns>The encrypted data with IV prepended.</returns>
        public byte[] Encrypt(byte[] input)
        {
            // Generate random IV
            _aes.GenerateIV();
            byte[] iv = _aes.IV;

            using (var encryptor = _aes.CreateEncryptor(_aes.Key, iv))
            {
                byte[] encrypted = encryptor.TransformFinalBlock(input, 0, input.Length);

                // Prepend IV to encrypted data (PDF requirement)
                byte[] result = new byte[BlockSize + encrypted.Length];
                Array.Copy(iv, 0, result, 0, BlockSize);
                Array.Copy(encrypted, 0, result, BlockSize, encrypted.Length);

                return result;
            }
        }

        /// <summary>
        /// Decrypts data using AES-128 in CBC mode.
        /// Expects the first 16 bytes to be the IV, followed by encrypted data.
        /// </summary>
        /// <param name="input">The encrypted data with IV prepended.</param>
        /// <returns>The decrypted plaintext data.</returns>
        public byte[] Decrypt(byte[] input)
        {
            if (input.Length < BlockSize)
            {
                throw new ArgumentException("Input too short - must include IV");
            }

            // Extract IV from first 16 bytes
            byte[] iv = new byte[BlockSize];
            Array.Copy(input, 0, iv, 0, BlockSize);

            // Decrypt remaining bytes
            using (var decryptor = _aes.CreateDecryptor(_aes.Key, iv))
            {
                return decryptor.TransformFinalBlock(input, BlockSize, input.Length - BlockSize);
            }
        }

        /// <summary>
        /// Encrypts data in-place using AES-128.
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="output">The output buffer (must be large enough for IV + encrypted data + padding).</param>
        /// <param name="length">The length of input data.</param>
        /// <returns>The total length of encrypted output (IV + encrypted data).</returns>
        public int EncryptBuffer(byte[] input, byte[] output, int length)
        {
            byte[] inputCopy = new byte[length];
            Array.Copy(input, inputCopy, length);

            byte[] encrypted = Encrypt(inputCopy);
            Array.Copy(encrypted, output, encrypted.Length);

            return encrypted.Length;
        }

        /// <summary>
        /// Decrypts data in-place using AES-128.
        /// </summary>
        /// <param name="input">The input data (IV + encrypted data).</param>
        /// <param name="output">The output buffer.</param>
        /// <param name="length">The length of input data.</param>
        /// <returns>The length of decrypted output.</returns>
        public int DecryptBuffer(byte[] input, byte[] output, int length)
        {
            byte[] inputCopy = new byte[length];
            Array.Copy(input, inputCopy, length);

            byte[] decrypted = Decrypt(inputCopy);
            Array.Copy(decrypted, output, decrypted.Length);

            return decrypted.Length;
        }

        /// <summary>
        /// Disposes of the AES resources.
        /// </summary>
        public void Dispose()
        {
            _aes?.Dispose();
        }
    }
}
