using System;
using Haru.Streams;

namespace Haru.Objects
{
    /// <summary>
    /// PDF binary data object (hex string)
    /// </summary>
    public class HpdfBinary : HpdfObject
    {
        private byte[] _value;

        /// <summary>
        /// Gets or sets the binary data
        /// </summary>
        public byte[] Value
        {
            get => _value;
            set
            {
                _value = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        /// <summary>
        /// Gets the length of the binary data
        /// </summary>
        public int Length => Value.Length;

        /// <inheritdoc/>
        public override HpdfObjectClass ObjectClass => HpdfObjectClass.Binary;

        /// <summary>
        /// Creates a new binary object
        /// </summary>
        public HpdfBinary(byte[] value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override void WriteValue(HpdfStream stream)
        {
            byte[] dataToWrite = Value;

            // Encrypt if encryption context is set
            if (stream.EncryptionContext != null)
            {
                dataToWrite = stream.EncryptionContext.Encrypt(Value);
            }

            stream.WriteHexString(dataToWrite);
        }

        /// <inheritdoc/>
        public override string ToString() => $"<{BitConverter.ToString(Value).Replace("-", "")}>";
    }
}
