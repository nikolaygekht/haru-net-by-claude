using System;
using System.Collections.Generic;
using System.Linq;
using Haru.Font;
using Haru.Objects;
using Haru.Types;

namespace Haru.Encoding
{
    /// <summary>
    /// Basic single-byte encoder implementation matching libharu's HPDF_BasicEncoderAttr.
    /// </summary>
    public class HpdfBasicEncoder : IHpdfEncoder
    {
        private readonly string _name;
        private readonly string _baseEncodingName;
        private readonly ushort[] _unicodeMap;
        private readonly bool[] _differences;
        private readonly Dictionary<char, byte> _reverseMap;

        public string Name => _name;
        public string BaseEncodingName => _baseEncodingName;
        public bool HasDifferences => _differences.Any(d => d);

        public HpdfBasicEncoder(
            string name,
            string baseEncodingName,
            ushort[] unicodeMap,
            bool[]? differences = null)
        {
            if (unicodeMap == null)
                throw new ArgumentNullException(nameof(unicodeMap));
            if (unicodeMap.Length != 256)
                throw new ArgumentException("Unicode map must have exactly 256 entries", nameof(unicodeMap));

            _name = name;
            _baseEncodingName = baseEncodingName;
            _unicodeMap = unicodeMap;
            _differences = differences ?? new bool[256];

            if (_differences.Length != 256)
                throw new ArgumentException("Differences array must have exactly 256 entries", nameof(differences));

            _reverseMap = new Dictionary<char, byte>();
            for (int i = 0; i < 256; i++)
            {
                if (_unicodeMap[i] != 0)
                {
                    char ch = (char)_unicodeMap[i];
                    if (!_reverseMap.ContainsKey(ch))
                        _reverseMap[ch] = (byte)i;
                }
            }
        }

        public ushort GetUnicode(byte byteValue) => _unicodeMap[byteValue];

        public byte? GetByteValue(char unicodeChar)
        {
            return _reverseMap.TryGetValue(unicodeChar, out byte b) ? b : null;
        }

        public HpdfArray? CreateDifferencesArray()
        {
            if (!HasDifferences)
                return null;

            var array = new HpdfArray();
            int startByte = -1;

            for (int i = 0; i < 256; i++)
            {
                if (_differences[i])
                {
                    if (startByte == -1)
                    {
                        startByte = i;
                        array.Add(new HpdfNumber(i));
                    }

                    ushort unicode = _unicodeMap[i];
                    string? glyphName = HpdfGlyphNames.GetGlyphName(unicode);
                    if (glyphName != null)
                    {
                        array.Add(new HpdfName(glyphName));
                    }
                }
                else
                {
                    startByte = -1;
                }
            }

            return array.Count > 0 ? array : null;
        }

        public byte[] EncodeText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<byte>();

            var result = new List<byte>();
            foreach (char ch in text)
            {
                byte? b = GetByteValue(ch);
                if (b.HasValue)
                {
                    result.Add(b.Value);
                }
                else
                {
                    result.Add((byte)'?');
                }
            }
            return result.ToArray();
        }
    }
}
