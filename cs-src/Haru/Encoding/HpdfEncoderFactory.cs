using System;
using System.Collections.Generic;
using System.Linq;

namespace Haru.Encoding
{
    /// <summary>
    /// Factory for creating and managing PDF text encoders.
    /// Provides centralized registration and retrieval of all supported encodings.
    /// </summary>
    public static class HpdfEncoderFactory
    {
        private static readonly Dictionary<string, IHpdfEncoder> _encoders = new();

        /// <summary>
        /// Static constructor that registers all built-in encoders.
        /// </summary>
        static HpdfEncoderFactory()
        {
            RegisterBuiltInEncoders();
        }

        /// <summary>
        /// Gets an encoder by name (case-insensitive).
        /// </summary>
        /// <param name="encodingName">The encoding name (e.g., "WinAnsiEncoding", "CP1251", "ISO8859-5").</param>
        /// <returns>The encoder instance, or WinAnsiEncoding as default if not found.</returns>
        public static IHpdfEncoder GetEncoder(string encodingName)
        {
            if (string.IsNullOrEmpty(encodingName))
                return _encoders["WINANSIENCODING"];

            return _encoders.TryGetValue(encodingName.ToUpperInvariant(), out var encoder)
                ? encoder
                : _encoders["WINANSIENCODING"];
        }

        /// <summary>
        /// Registers a custom encoder.
        /// </summary>
        /// <param name="encoder">The encoder to register.</param>
        public static void RegisterEncoder(IHpdfEncoder encoder)
        {
            if (encoder == null)
                throw new ArgumentNullException(nameof(encoder));

            _encoders[encoder.Name.ToUpperInvariant()] = encoder;
        }

        /// <summary>
        /// Gets all registered encoder names.
        /// </summary>
        public static IEnumerable<string> GetEncoderNames()
        {
            return _encoders.Values.Select(e => e.Name);
        }

        /// <summary>
        /// Registers all built-in PDF encoders matching libharu's encoder system.
        /// </summary>
        private static void RegisterBuiltInEncoders()
        {
            // Base encodings (no differences arrays)
            RegisterEncoder(new HpdfBasicEncoder(
                "StandardEncoding",
                "StandardEncoding",
                EncodingMaps.StandardEncoding));

            RegisterEncoder(new HpdfBasicEncoder(
                "MacRomanEncoding",
                "MacRomanEncoding",
                EncodingMaps.MacRomanEncoding));

            RegisterEncoder(new HpdfBasicEncoder(
                "WinAnsiEncoding",
                "WinAnsiEncoding",
                EncodingMaps.WinAnsiEncoding));

            // Windows Code Pages (CP125x series) - use WinAnsiEncoding as base
            RegisterEncoder(CreateCP1250());
            RegisterEncoder(CreateCP1251());
            RegisterEncoder(CreateCP1252());
            RegisterEncoder(CreateCP1253());
            RegisterEncoder(CreateCP1254());
            RegisterEncoder(CreateCP1255());
            RegisterEncoder(CreateCP1256());
            RegisterEncoder(CreateCP1257());
            RegisterEncoder(CreateCP1258());

            // KOI8-R (Russian Cyrillic) - use StandardEncoding as base
            RegisterEncoder(CreateKOI8R());

            // ISO8859 series - use StandardEncoding as base
            RegisterEncoder(CreateISO8859_2());
            RegisterEncoder(CreateISO8859_3());
            RegisterEncoder(CreateISO8859_4());
            RegisterEncoder(CreateISO8859_5());
            RegisterEncoder(CreateISO8859_6());
            RegisterEncoder(CreateISO8859_7());
            RegisterEncoder(CreateISO8859_8());
            RegisterEncoder(CreateISO8859_9());
            RegisterEncoder(CreateISO8859_10());
            RegisterEncoder(CreateISO8859_11());
            RegisterEncoder(CreateISO8859_13());
            RegisterEncoder(CreateISO8859_14());
            RegisterEncoder(CreateISO8859_15());
            RegisterEncoder(CreateISO8859_16());
        }

        // Windows Code Page encoders
        private static IHpdfEncoder CreateCP1250()
        {
            var differences = CalculateDifferences(EncodingMaps.WinAnsiEncoding, EncodingMaps.CP1250);
            return new HpdfBasicEncoder("CP1250", "WinAnsiEncoding", EncodingMaps.CP1250, differences);
        }

        private static IHpdfEncoder CreateCP1251()
        {
            var differences = CalculateDifferences(EncodingMaps.WinAnsiEncoding, EncodingMaps.CP1251);
            return new HpdfBasicEncoder("CP1251", "WinAnsiEncoding", EncodingMaps.CP1251, differences);
        }

        private static IHpdfEncoder CreateCP1252()
        {
            var differences = CalculateDifferences(EncodingMaps.WinAnsiEncoding, EncodingMaps.CP1252);
            return new HpdfBasicEncoder("CP1252", "WinAnsiEncoding", EncodingMaps.CP1252, differences);
        }

        private static IHpdfEncoder CreateCP1253()
        {
            var differences = CalculateDifferences(EncodingMaps.WinAnsiEncoding, EncodingMaps.CP1253);
            return new HpdfBasicEncoder("CP1253", "WinAnsiEncoding", EncodingMaps.CP1253, differences);
        }

        private static IHpdfEncoder CreateCP1254()
        {
            var differences = CalculateDifferences(EncodingMaps.WinAnsiEncoding, EncodingMaps.CP1254);
            return new HpdfBasicEncoder("CP1254", "WinAnsiEncoding", EncodingMaps.CP1254, differences);
        }

        private static IHpdfEncoder CreateCP1255()
        {
            var differences = CalculateDifferences(EncodingMaps.WinAnsiEncoding, EncodingMaps.CP1255);
            return new HpdfBasicEncoder("CP1255", "WinAnsiEncoding", EncodingMaps.CP1255, differences);
        }

        private static IHpdfEncoder CreateCP1256()
        {
            var differences = CalculateDifferences(EncodingMaps.WinAnsiEncoding, EncodingMaps.CP1256);
            return new HpdfBasicEncoder("CP1256", "WinAnsiEncoding", EncodingMaps.CP1256, differences);
        }

        private static IHpdfEncoder CreateCP1257()
        {
            var differences = CalculateDifferences(EncodingMaps.WinAnsiEncoding, EncodingMaps.CP1257);
            return new HpdfBasicEncoder("CP1257", "WinAnsiEncoding", EncodingMaps.CP1257, differences);
        }

        private static IHpdfEncoder CreateCP1258()
        {
            var differences = CalculateDifferences(EncodingMaps.WinAnsiEncoding, EncodingMaps.CP1258);
            return new HpdfBasicEncoder("CP1258", "WinAnsiEncoding", EncodingMaps.CP1258, differences);
        }

        // KOI8-R encoder
        private static IHpdfEncoder CreateKOI8R()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.KOI8_R);
            return new HpdfBasicEncoder("KOI8-R", "StandardEncoding", EncodingMaps.KOI8_R, differences);
        }

        // ISO8859 encoders
        private static IHpdfEncoder CreateISO8859_2()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_2);
            return new HpdfBasicEncoder("ISO8859-2", "StandardEncoding", EncodingMaps.ISO8859_2, differences);
        }

        private static IHpdfEncoder CreateISO8859_3()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_3);
            return new HpdfBasicEncoder("ISO8859-3", "StandardEncoding", EncodingMaps.ISO8859_3, differences);
        }

        private static IHpdfEncoder CreateISO8859_4()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_4);
            return new HpdfBasicEncoder("ISO8859-4", "StandardEncoding", EncodingMaps.ISO8859_4, differences);
        }

        private static IHpdfEncoder CreateISO8859_5()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_5);
            return new HpdfBasicEncoder("ISO8859-5", "StandardEncoding", EncodingMaps.ISO8859_5, differences);
        }

        private static IHpdfEncoder CreateISO8859_6()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_6);
            return new HpdfBasicEncoder("ISO8859-6", "StandardEncoding", EncodingMaps.ISO8859_6, differences);
        }

        private static IHpdfEncoder CreateISO8859_7()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_7);
            return new HpdfBasicEncoder("ISO8859-7", "StandardEncoding", EncodingMaps.ISO8859_7, differences);
        }

        private static IHpdfEncoder CreateISO8859_8()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_8);
            return new HpdfBasicEncoder("ISO8859-8", "StandardEncoding", EncodingMaps.ISO8859_8, differences);
        }

        private static IHpdfEncoder CreateISO8859_9()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_9);
            return new HpdfBasicEncoder("ISO8859-9", "StandardEncoding", EncodingMaps.ISO8859_9, differences);
        }

        private static IHpdfEncoder CreateISO8859_10()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_10);
            return new HpdfBasicEncoder("ISO8859-10", "StandardEncoding", EncodingMaps.ISO8859_10, differences);
        }

        private static IHpdfEncoder CreateISO8859_11()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_11);
            return new HpdfBasicEncoder("ISO8859-11", "StandardEncoding", EncodingMaps.ISO8859_11, differences);
        }

        private static IHpdfEncoder CreateISO8859_13()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_13);
            return new HpdfBasicEncoder("ISO8859-13", "StandardEncoding", EncodingMaps.ISO8859_13, differences);
        }

        private static IHpdfEncoder CreateISO8859_14()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_14);
            return new HpdfBasicEncoder("ISO8859-14", "StandardEncoding", EncodingMaps.ISO8859_14, differences);
        }

        private static IHpdfEncoder CreateISO8859_15()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_15);
            return new HpdfBasicEncoder("ISO8859-15", "StandardEncoding", EncodingMaps.ISO8859_15, differences);
        }

        private static IHpdfEncoder CreateISO8859_16()
        {
            var differences = CalculateDifferences(EncodingMaps.StandardEncoding, EncodingMaps.ISO8859_16);
            return new HpdfBasicEncoder("ISO8859-16", "StandardEncoding", EncodingMaps.ISO8859_16, differences);
        }

        /// <summary>
        /// Calculates which bytes differ between base encoding and target encoding.
        /// Returns a bool[256] array where true indicates the byte differs from base.
        /// </summary>
        private static bool[] CalculateDifferences(ushort[] baseMap, ushort[] targetMap)
        {
            var differences = new bool[256];
            for (int i = 0; i < 256; i++)
            {
                differences[i] = baseMap[i] != targetMap[i];
            }
            return differences;
        }
    }
}
