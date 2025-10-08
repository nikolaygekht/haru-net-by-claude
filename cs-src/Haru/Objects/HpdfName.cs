using System;
using Haru.Streams;
using Haru.Types;

namespace Haru.Objects
{
    /// <summary>
    /// PDF name object (/Name)
    /// </summary>
    public class HpdfName : HpdfObject
    {
        private string _value;

        /// <summary>
        /// Gets or sets the name value (without the leading slash)
        /// </summary>
        public string Value
        {
            get => _value;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length == 0)
                    throw new HpdfException(HpdfErrorCode.NameInvalidValue, "Name cannot be empty");
                if (value.Length > HpdfConst.LimitMaxNameLen)
                    throw new HpdfException(HpdfErrorCode.NameOutOfRange,
                        $"Name length cannot exceed {HpdfConst.LimitMaxNameLen} characters");
                _value = value;
            }
        }

        /// <inheritdoc/>
        public override HpdfObjectClass ObjectClass => HpdfObjectClass.Name;

        /// <summary>
        /// Creates a new name object
        /// </summary>
        public HpdfName(string value)
        {
            Value = value;
        }

        /// <inheritdoc/>
        public override void WriteValue(HpdfStream stream)
        {
            stream.WriteEscapedName(Value);
        }

        /// <inheritdoc/>
        public override string ToString() => "/" + Value;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is HpdfName other && Value == other.Value;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Value.GetHashCode();
    }
}
