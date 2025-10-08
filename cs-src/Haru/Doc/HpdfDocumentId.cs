/*
 * << Haru Free PDF Library >> -- HpdfDocumentId.cs
 *
 * C# port of Haru Free PDF Library
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 * provided that the above copyright notice appear in all copies and
 * that both that copyright notice and this permission notice appear
 * in supporting documentation.
 * It is provided "as is" without express or implied warranty.
 *
 */

using System;
using System.Security.Cryptography;
using System.Text;
using Haru.Objects;

namespace Haru.Doc
{
    /// <summary>
    /// Generates unique Document IDs for PDFs.
    /// Required for PDF/A compliance - uses MD5 hash of timestamp and document metadata.
    /// </summary>
    internal class HpdfDocumentId
    {
        /// <summary>
        /// Generates a Document ID array for the PDF trailer.
        /// Returns an array with two identical IDs (permanent ID and changing ID).
        /// </summary>
        /// <param name="info">Document information (optional).</param>
        /// <returns>Array containing two binary strings (permanent ID, changing ID).</returns>
        public static HpdfArray GenerateId(HpdfInfo info = null)
        {
            byte[] idBytes = GenerateIdBytes(info);

            var idArray = new HpdfArray();
            idArray.Add(new HpdfBinary(idBytes));
            idArray.Add(new HpdfBinary(idBytes)); // Same ID for both (PDF/A requirement)

            return idArray;
        }

        /// <summary>
        /// Generates the actual ID bytes using MD5 hash.
        /// </summary>
        private static byte[] GenerateIdBytes(HpdfInfo info)
        {
            using (var md5 = MD5.Create())
            {
                var sb = new StringBuilder();

                // Add timestamp
                sb.Append(DateTime.UtcNow.Ticks.ToString());

                // Add document metadata if available
                if (info?.Dict != null)
                {
                    AppendIfExists(sb, info, "Title");
                    AppendIfExists(sb, info, "Author");
                    AppendIfExists(sb, info, "Subject");
                    AppendIfExists(sb, info, "Keywords");
                    AppendIfExists(sb, info, "Creator");
                }

                // Add random component for uniqueness
                sb.Append(Guid.NewGuid().ToString());

                byte[] inputBytes = Encoding.UTF8.GetBytes(sb.ToString());
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return hashBytes;
            }
        }

        /// <summary>
        /// Appends a value from Info dictionary to the string builder if it exists.
        /// </summary>
        private static void AppendIfExists(StringBuilder sb, HpdfInfo info, string key)
        {
            if (info.Dict.TryGetValue(key, out var obj))
            {
                if (obj is HpdfString str)
                {
                    string value = Encoding.UTF8.GetString(str.Value);
                    if (!string.IsNullOrEmpty(value))
                        sb.Append(value);
                }
            }
        }
    }
}
