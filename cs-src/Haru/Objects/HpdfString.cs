using System;
using System.Text;
using Haru.Streams;

namespace Haru.Objects
{
    /// <summary>
    /// PDF string object (text in parentheses or hex)
    /// </summary>
    public class HpdfString : HpdfObject
    {
        private byte[] _value;

        /// <summary>
        /// Gets or sets the string value as bytes
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
        /// Gets or sets whether to write as hex string (&lt;...&gt;) instead of literal string (...)
        /// </summary>
        public bool WriteAsHex { get; set; }

        /// <inheritdoc/>
        public override HpdfObjectClass ObjectClass => HpdfObjectClass.String;

        /// <summary>
        /// Creates a new string object from bytes
        /// </summary>
        public HpdfString(byte[] value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new string object from text
        /// </summary>
        public HpdfString(string text)
            : this(text, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Creates a new string object from text with specified encoding
        /// </summary>
        public HpdfString(string text, Encoding encoding)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            Value = encoding.GetBytes(text);
        }

        /// <summary>
        /// Gets the string value as text using the specified encoding
        /// </summary>
        public string GetText(Encoding encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(Value);
        }

        /// <inheritdoc/>
        public override void WriteValue(HpdfStream stream)
        {
            if (WriteAsHex)
            {
                stream.WriteHexString(Value);
            }
            else
            {
                // Convert bytes to string for escape processing
                string text = Encoding.Latin1.GetString(Value);
                stream.WriteEscapedText(text);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            string text = GetText();
            return WriteAsHex ? $"<{BitConverter.ToString(Value).Replace("-", "")}>" : $"({text})";
        }
    }
}
