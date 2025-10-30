/*
 * << Haru Free PDF Library >> -- HpdfXmpMetadata.cs
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

using System.Text;
using Haru.Objects;
using Haru.Xref;

namespace Haru.Doc
{
    /// <summary>
    /// Generates XMP (Extensible Metadata Platform) metadata for PDF/A compliance.
    /// XMP uses RDF/XML format with Dublin Core, XMP, and PDF namespaces.
    /// </summary>
    internal class HpdfXmpMetadata
    {
        private readonly HpdfInfo _info;
        private readonly string _pdfAConformance;

        /// <summary>
        /// Creates XMP metadata generator.
        /// </summary>
        /// <param name="info">Document information dictionary (may be null).</param>
        /// <param name="conformance">PDF/A conformance level (e.g., "1B", "2B", "3B").</param>
        public HpdfXmpMetadata(HpdfInfo info, string conformance = "1B")
        {
            _info = info;
            _pdfAConformance = conformance ?? "1B";
        }

        /// <summary>
        /// Generates complete XMP metadata packet as a stream.
        /// </summary>
        public HpdfStreamObject GenerateXmpStream(HpdfXref xref)
        {
            if (xref is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");

            string xmpContent = GenerateXmpContent();
            byte[] xmpBytes = Encoding.UTF8.GetBytes(xmpContent);

            var stream = new HpdfStreamObject();
            xref.Add(stream);

            stream.Add("Type", new HpdfName("Metadata"));
            stream.Add("Subtype", new HpdfName("XML"));

            stream.WriteToStream(xmpBytes, 0, xmpBytes.Length);

            return stream;
        }

        /// <summary>
        /// Generates XMP metadata content in RDF/XML format.
        /// </summary>
        private string GenerateXmpContent()
        {
            var sb = new StringBuilder();

            // XMP packet header
            sb.AppendLine("<?xpacket begin=\"\ufeff\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>");
            sb.AppendLine("<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Haru Free PDF Library\">");
            sb.AppendLine("  <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">");
            sb.AppendLine("    <rdf:Description rdf:about=\"\"");
            sb.AppendLine("        xmlns:dc=\"http://purl.org/dc/elements/1.1/\"");
            sb.AppendLine("        xmlns:xmp=\"http://ns.adobe.com/xap/1.0/\"");
            sb.AppendLine("        xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\"");
            sb.AppendLine("        xmlns:pdfaid=\"http://www.aiim.org/pdfa/ns/id/\">");

            // PDF/A identification
            sb.AppendLine($"      <pdfaid:part>1</pdfaid:part>");
            sb.AppendLine($"      <pdfaid:conformance>{_pdfAConformance}</pdfaid:conformance>");

            // Dublin Core metadata (from HpdfInfo if available)
            if (_info != null)
            {
                string? title = GetInfoValue("Title");
                if (!string.IsNullOrEmpty(title))
                {
                    sb.AppendLine("      <dc:title>");
                    sb.AppendLine("        <rdf:Alt>");
                    sb.AppendLine($"          <rdf:li xml:lang=\"x-default\">{EscapeXml(title)}</rdf:li>");
                    sb.AppendLine("        </rdf:Alt>");
                    sb.AppendLine("      </dc:title>");
                }

                string? author = GetInfoValue("Author");
                if (!string.IsNullOrEmpty(author))
                {
                    sb.AppendLine("      <dc:creator>");
                    sb.AppendLine("        <rdf:Seq>");
                    sb.AppendLine($"          <rdf:li>{EscapeXml(author)}</rdf:li>");
                    sb.AppendLine("        </rdf:Seq>");
                    sb.AppendLine("      </dc:creator>");
                }

                string? subject = GetInfoValue("Subject");
                if (!string.IsNullOrEmpty(subject))
                {
                    sb.AppendLine("      <dc:description>");
                    sb.AppendLine("        <rdf:Alt>");
                    sb.AppendLine($"          <rdf:li xml:lang=\"x-default\">{EscapeXml(subject)}</rdf:li>");
                    sb.AppendLine("        </rdf:Alt>");
                    sb.AppendLine("      </dc:description>");
                }

                string? keywords = GetInfoValue("Keywords");
                if (!string.IsNullOrEmpty(keywords))
                {
                    sb.AppendLine("      <pdf:Keywords>");
                    sb.AppendLine($"        {EscapeXml(keywords)}");
                    sb.AppendLine("      </pdf:Keywords>");
                }

                string? creator = GetInfoValue("Creator");
                if (!string.IsNullOrEmpty(creator))
                {
                    sb.AppendLine($"      <xmp:CreatorTool>{EscapeXml(creator)}</xmp:CreatorTool>");
                }

                string? producer = GetInfoValue("Producer");
                if (!string.IsNullOrEmpty(producer))
                {
                    sb.AppendLine($"      <pdf:Producer>{EscapeXml(producer)}</pdf:Producer>");
                }

                string? creationDate = GetInfoValue("CreationDate");
                if (!string.IsNullOrEmpty(creationDate))
                {
                    string? xmpDate = ConvertPdfDateToXmp(creationDate);
                    if (!string.IsNullOrEmpty(xmpDate))
                        sb.AppendLine($"      <xmp:CreateDate>{xmpDate}</xmp:CreateDate>");
                }

                string? modDate = GetInfoValue("ModDate");
                if (!string.IsNullOrEmpty(modDate))
                {
                    string? xmpDate = ConvertPdfDateToXmp(modDate);
                    if (!string.IsNullOrEmpty(xmpDate))
                        sb.AppendLine($"      <xmp:ModifyDate>{xmpDate}</xmp:ModifyDate>");
                }
            }

            sb.AppendLine("    </rdf:Description>");
            sb.AppendLine("  </rdf:RDF>");
            sb.AppendLine("</x:xmpmeta>");

            // XMP packet trailer with padding (PDF/A requires fixed-size packets)
            sb.AppendLine("<?xpacket end=\"w\"?>");

            return sb.ToString();
        }

        /// <summary>
        /// Gets a value from the Info dictionary.
        /// </summary>
        private string? GetInfoValue(string key)
        {
            if (_info?.Dict is null)
                return null;

            if (_info.Dict.TryGetValue(key, out var obj))
            {
                if (obj is HpdfString str)
                    return Encoding.UTF8.GetString(str.Value);
            }

            return null;
        }

        /// <summary>
        /// Converts PDF date format (D:YYYYMMDDHHmmSSOHH'mm') to XMP format (YYYY-MM-DDTHH:mm:SS+HH:mm).
        /// </summary>
        private string? ConvertPdfDateToXmp(string pdfDate)
        {
            if (string.IsNullOrEmpty(pdfDate))
                return null;

            // Remove "D:" prefix if present
            if (pdfDate.StartsWith("D:"))
                pdfDate = pdfDate.Substring(2);

            // Parse: YYYYMMDDHHmmSSOHH'mm'
            if (pdfDate.Length < 14)
                return null;

            try
            {
                string year = pdfDate.Substring(0, 4);
                string month = pdfDate.Substring(4, 2);
                string day = pdfDate.Substring(6, 2);
                string hour = pdfDate.Substring(8, 2);
                string minute = pdfDate.Substring(10, 2);
                string second = pdfDate.Substring(12, 2);

                // Timezone (optional)
                string timezone = "Z";
                if (pdfDate.Length > 14)
                {
                    char tzSign = pdfDate[14];
                    if (tzSign == '+' || tzSign == '-')
                    {
                        if (pdfDate.Length >= 19)
                        {
                            string tzHour = pdfDate.Substring(15, 2);
                            string tzMinute = pdfDate.Substring(18, 2);
                            timezone = $"{tzSign}{tzHour}:{tzMinute}";
                        }
                    }
                }

                return $"{year}-{month}-{day}T{hour}:{minute}:{second}{timezone}";
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Escapes XML special characters.
        /// </summary>
        private string EscapeXml(string? text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return text
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}
