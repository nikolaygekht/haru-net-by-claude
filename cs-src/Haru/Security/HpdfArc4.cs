namespace Haru.Security
{
    /// <summary>
    /// RC4 (ARC4) stream cipher implementation for PDF encryption.
    /// </summary>
    internal class HpdfArc4
    {
        private const int StateSize = 256;

        private byte _idx1;
        private byte _idx2;
        private readonly byte[] _state;

        /// <summary>
        /// Initializes a new instance of the HpdfArc4 class.
        /// </summary>
        public HpdfArc4()
        {
            _state = new byte[StateSize];
        }

        /// <summary>
        /// Initializes the RC4 cipher with the specified key.
        /// </summary>
        /// <param name="key">The encryption key.</param>
        public void Init(byte[] key)
        {
            Init(key, key.Length);
        }

        /// <summary>
        /// Initializes the RC4 cipher with the specified key and length.
        /// </summary>
        /// <param name="key">The encryption key.</param>
        /// <param name="keyLen">The length of the key to use.</param>
        public void Init(byte[] key, int keyLen)
        {
            // Initialize state array with values 0..255
            for (int i = 0; i < StateSize; i++)
            {
                _state[i] = (byte)i;
            }

            // Create temporary array with repeated key
            byte[] tmpArray = new byte[StateSize];
            for (int i = 0; i < StateSize; i++)
            {
                tmpArray[i] = key[i % keyLen];
            }

            // Perform key-scheduling algorithm (KSA)
            int j = 0;
            for (int i = 0; i < StateSize; i++)
            {
                j = (j + _state[i] + tmpArray[i]) % StateSize;

                // Swap state[i] and state[j]
                byte tmp = _state[i];
                _state[i] = _state[j];
                _state[j] = tmp;
            }

            _idx1 = 0;
            _idx2 = 0;
        }

        /// <summary>
        /// Encrypts or decrypts data using RC4 (same operation for both).
        /// </summary>
        /// <param name="input">The input data.</param>
        /// <param name="output">The output buffer.</param>
        /// <param name="length">The number of bytes to process.</param>
        public void Crypt(byte[] input, byte[] output, int length)
        {
            for (int i = 0; i < length; i++)
            {
                // Update indices
                _idx1 = (byte)((_idx1 + 1) % StateSize);
                _idx2 = (byte)((_idx2 + _state[_idx1]) % StateSize);

                // Swap state[idx1] and state[idx2]
                byte tmp = _state[_idx1];
                _state[_idx1] = _state[_idx2];
                _state[_idx2] = tmp;

                // Generate keystream byte
                int t = (_state[_idx1] + _state[_idx2]) % StateSize;
                byte k = _state[t];

                // XOR with input to produce output
                output[i] = (byte)(input[i] ^ k);
            }
        }

        /// <summary>
        /// Resets the cipher to its initialized state.
        /// This allows reusing the same key without re-initialization.
        /// </summary>
        public void Reset()
        {
            _idx1 = 0;
            _idx2 = 0;
        }
    }
}
