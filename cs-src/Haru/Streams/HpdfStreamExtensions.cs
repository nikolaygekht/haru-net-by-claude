using System;
using System.Globalization;
using System.Text;

namespace Haru.Streams
{
    /// <summary>
    /// Extension methods for writing PDF data to streams
    /// </summary>
    public static class HpdfStreamExtensions
    {
        static HpdfStreamExtensions()
        {
            // Register code page encoding provider for Windows-1252 and other encodings
            // This is required on .NET Core/5+ where these encodings are not available by default
            System.Text.Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Writes a character to the stream
        /// </summary>
        public static void WriteChar(this HpdfStream stream, char value)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes a string to the stream (without encoding)
        /// </summary>
        public static void WriteString(this HpdfStream stream, string value)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(value);
            stream.Write(bytes);
        }

        /// <summary>
        /// Writes an integer to the stream as ASCII text
        /// </summary>
        public static void WriteInt(this HpdfStream stream, int value)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            string text = value.ToString(CultureInfo.InvariantCulture);
            stream.WriteString(text);
        }

        /// <summary>
        /// Writes an unsigned integer to the stream as ASCII text
        /// </summary>
        public static void WriteUInt(this HpdfStream stream, uint value)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            string text = value.ToString(CultureInfo.InvariantCulture);
            stream.WriteString(text);
        }

        /// <summary>
        /// Writes a float to the stream as ASCII text
        /// </summary>
        public static void WriteReal(this HpdfStream stream, float value)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            // Format with appropriate precision for PDF
            string text = FormatReal(value);
            stream.WriteString(text);
        }

        /// <summary>
        /// Writes a line to the stream (string followed by newline)
        /// </summary>
        public static void WriteLine(this HpdfStream stream, string value = "")
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (!string.IsNullOrEmpty(value))
                stream.WriteString(value);
            stream.WriteByte((byte)'\n');
        }

        /// <summary>
        /// Writes an escaped PDF name to the stream
        /// </summary>
        public static void WriteEscapedName(this HpdfStream stream, string name)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            stream.WriteByte((byte)'/');

            foreach (char c in name)
            {
                // Escape special characters in PDF names
                if (c < 33 || c > 126 || c == '#' || c == '/' || c == '(' || c == ')' ||
                    c == '<' || c == '>' || c == '[' || c == ']' || c == '{' || c == '}' ||
                    c == '%' || c == ' ')
                {
                    stream.WriteByte((byte)'#');
                    stream.WriteByte(ToHexDigit((c >> 4) & 0x0F));
                    stream.WriteByte(ToHexDigit(c & 0x0F));
                }
                else
                {
                    stream.WriteByte((byte)c);
                }
            }
        }

        /// <summary>
        /// Writes an escaped PDF text string to the stream (enclosed in parentheses).
        /// Uses WinAnsiEncoding (Windows-1252 / CP1252) by default for characters outside ASCII range.
        /// This is the default encoding for standard PDF fonts (Helvetica, Times, Courier, etc.).
        /// </summary>
        public static void WriteEscapedText(this HpdfStream stream, string text)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            // Delegate to the codePage overload with WinAnsiEncoding (CP1252)
            WriteEscapedText(stream, text, 1252);
        }

        /// <summary>
        /// Writes an escaped PDF text string using a specific encoding code page.
        /// </summary>
        public static void WriteEscapedText(this HpdfStream stream, string text, int codePage)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            // Convert text to bytes using the specified code page
            System.Text.Encoding encoding;
            try
            {
                encoding = System.Text.Encoding.GetEncoding(codePage);
            }
            catch
            {
                // Fallback to Latin-1 if code page not available
                encoding = System.Text.Encoding.GetEncoding(28591); // ISO-8859-1
            }

            byte[] bytes = encoding.GetBytes(text);

            // Write as literal string format (text) with proper escaping
            // This is the standard PDF format for text strings
            stream.WriteByte((byte)'(');

            foreach (byte b in bytes)
            {
                // Escape special characters in PDF strings
                if (b == (byte)'(' || b == (byte)')' || b == (byte)'\\')
                {
                    stream.WriteByte((byte)'\\');
                    stream.WriteByte(b);
                }
                else if (b == (byte)'\r')
                {
                    stream.WriteByte((byte)'\\');
                    stream.WriteByte((byte)'r');
                }
                else if (b == (byte)'\n')
                {
                    stream.WriteByte((byte)'\\');
                    stream.WriteByte((byte)'n');
                }
                else if (b < 32 || b > 126)
                {
                    // Write as octal escape for non-printable characters
                    stream.WriteByte((byte)'\\');
                    stream.WriteByte((byte)('0' + ((b >> 6) & 0x07)));
                    stream.WriteByte((byte)('0' + ((b >> 3) & 0x07)));
                    stream.WriteByte((byte)('0' + (b & 0x07)));
                }
                else
                {
                    stream.WriteByte(b);
                }
            }

            stream.WriteByte((byte)')');
        }

        /// <summary>
        /// Writes text as MBCS hex string for CID fonts with Identity-H encoding.
        /// Text is encoded using the font's code page (CP932, CP936, CP949, CP950)
        /// and written as hex string format: <XXXX> where XXXX is the multi-byte value.
        /// </summary>
        public static void WriteEscapedTextMBCS(this HpdfStream stream, string text, int codePage)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (text is null)
                throw new ArgumentNullException(nameof(text));

            // Convert text to MBCS bytes using the code page
            System.Text.Encoding encoding;
            try
            {
                encoding = System.Text.Encoding.GetEncoding(codePage);
            }
            catch
            {
                // Fallback to UTF-8 if code page not available
                encoding = System.Text.Encoding.UTF8;
            }

            byte[] mbcsBytes = encoding.GetBytes(text);

            // Write as hex string
            stream.WriteByte((byte)'<');

            foreach (byte b in mbcsBytes)
            {
                stream.WriteByte(ToHexDigit((b >> 4) & 0x0F));
                stream.WriteByte(ToHexDigit(b & 0x0F));
            }

            stream.WriteByte((byte)'>');
        }

        /// <summary>
        /// Writes binary data as hexadecimal to the stream (enclosed in angle brackets)
        /// </summary>
        public static void WriteHexString(this HpdfStream stream, byte[] data)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            stream.WriteByte((byte)'<');

            foreach (byte b in data)
            {
                stream.WriteByte(ToHexDigit((b >> 4) & 0x0F));
                stream.WriteByte(ToHexDigit(b & 0x0F));
            }

            stream.WriteByte((byte)'>');
        }

        /// <summary>
        /// Reads a line from the stream
        /// </summary>
        public static string? ReadLine(this HpdfStream stream, int maxLength = 1024)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            StringBuilder sb = new StringBuilder();
            byte[] buffer = new byte[1];
            bool foundCr = false;

            while (sb.Length < maxLength)
            {
                int bytesRead = stream.Read(buffer, 0, 1);
                if (bytesRead == 0)
                {
                    if (sb.Length > 0)
                        return sb.ToString();
                    return null; // EOF
                }

                char c = (char)buffer[0];

                if (c == '\r')
                {
                    foundCr = true;
                    continue;
                }

                if (c == '\n')
                {
                    return sb.ToString();
                }

                if (foundCr)
                {
                    // CR without LF - treat as line ending and put back character
                    stream.Seek(-1, System.IO.SeekOrigin.Current);
                    return sb.ToString();
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        private static string FormatReal(float value)
        {
            // Handle special cases
            if (float.IsNaN(value) || float.IsInfinity(value))
                return "0";

            // Format with enough precision, but remove trailing zeros
            string text = value.ToString("0.######", CultureInfo.InvariantCulture);

            // Remove trailing zeros after decimal point
            if (text.Contains('.'))
            {
                text = text.TrimEnd('0').TrimEnd('.');
            }

            return text;
        }

        private static byte ToHexDigit(int value)
        {
            if (value < 10)
                return (byte)('0' + value);
            return (byte)('A' + (value - 10));
        }
    }
}
