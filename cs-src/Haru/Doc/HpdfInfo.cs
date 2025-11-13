/*
 * << Haru Free PDF Library >> -- HpdfInfo.cs
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
using Haru.Objects;
using Haru.Xref;

namespace Haru.Doc
{
    /// <summary>
    /// Document information dictionary containing metadata about the PDF.
    /// </summary>
    public class HpdfInfo
    {
        private readonly HpdfDict _dict;
        private readonly HpdfXref _xref;

        /// <summary>
        /// Gets the underlying dictionary object.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Creates a new document information dictionary.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        public HpdfInfo(HpdfXref xref)
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            _dict = new HpdfDict();

            // Add to xref
            _xref.Add(_dict);

            // Set default values
            Producer = "Haru Free PDF Library (C# Port)";
            // Note: CreationDate is not set automatically to allow for reproducible PDFs
            // Call SetCreationDate(DateTime.Now) explicitly if timestamp is needed
        }

        /// <summary>
        /// Gets or sets the document title.
        /// </summary>
        public string? Title
        {
            get => GetStringValue("Title");
            set => SetStringValue("Title", value);
        }

        /// <summary>
        /// Gets or sets the document author.
        /// </summary>
        public string? Author
        {
            get => GetStringValue("Author");
            set => SetStringValue("Author", value);
        }

        /// <summary>
        /// Gets or sets the document subject.
        /// </summary>
        public string? Subject
        {
            get => GetStringValue("Subject");
            set => SetStringValue("Subject", value);
        }

        /// <summary>
        /// Gets or sets the document keywords.
        /// </summary>
        public string? Keywords
        {
            get => GetStringValue("Keywords");
            set => SetStringValue("Keywords", value);
        }

        /// <summary>
        /// Gets or sets the application that created the document.
        /// </summary>
        public string? Creator
        {
            get => GetStringValue("Creator");
            set => SetStringValue("Creator", value);
        }

        /// <summary>
        /// Gets or sets the PDF producer (the application that converted to PDF).
        /// </summary>
        public string? Producer
        {
            get => GetStringValue("Producer");
            set => SetStringValue("Producer", value);
        }

        /// <summary>
        /// Gets or sets the trapped status.
        /// Valid values: "True", "False", or "Unknown".
        /// </summary>
        public string? Trapped
        {
            get => GetNameValue("Trapped");
            set
            {
                if (value != null && value != "True" && value != "False" && value != "Unknown")
                    throw new HpdfException(HpdfErrorCode.InvalidParameter,
                        "Trapped must be 'True', 'False', or 'Unknown'");
                SetNameValue("Trapped", value);
            }
        }

        /// <summary>
        /// Sets the creation date of the document.
        /// </summary>
        /// <param name="date">The creation date.</param>
        public void SetCreationDate(DateTime date)
        {
            SetDateValue("CreationDate", date);
        }

        /// <summary>
        /// Sets the modification date of the document.
        /// </summary>
        /// <param name="date">The modification date.</param>
        public void SetModificationDate(DateTime date)
        {
            SetDateValue("ModDate", date);
        }

        /// <summary>
        /// Gets the creation date string.
        /// </summary>
        public string? GetCreationDate()
        {
            return GetStringValue("CreationDate");
        }

        /// <summary>
        /// Gets the modification date string.
        /// </summary>
        public string? GetModificationDate()
        {
            return GetStringValue("ModDate");
        }

        /// <summary>
        /// Sets a custom metadata entry.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The metadata value.</param>
        public void SetCustomMetadata(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Key cannot be null or empty");

            SetStringValue(key, value);
        }

        /// <summary>
        /// Gets a custom metadata entry.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <returns>The metadata value, or null if not found.</returns>
        public string? GetCustomMetadata(string key)
        {
            if (string.IsNullOrEmpty(key))
                return (string?)null;

            return GetStringValue(key);
        }

        private void SetStringValue(string key, string? value)
        {
            if (value is null)
            {
                _dict.Remove(key);
                return;
            }

            var stringObj = new HpdfString(value);
            _dict[key] = stringObj;
        }

        private string? GetStringValue(string key)
        {
            if (_dict.TryGetValue(key, out var obj) && obj is HpdfString str)
                return System.Text.Encoding.UTF8.GetString(str.Value);
            return (string?)null;
        }

        private void SetNameValue(string key, string? value)
        {
            if (value is null)
            {
                _dict.Remove(key);
                return;
            }

            var nameObj = new HpdfName(value);
            _dict[key] = nameObj;
        }

        private string? GetNameValue(string key)
        {
            if (_dict.TryGetValue(key, out var obj) && obj is HpdfName name)
                return name.Value;
            return (string?)null;
        }

        private void SetDateValue(string key, DateTime date)
        {
            // PDF date format: D:YYYYMMDDHHmmSSOHH'mm'
            // Example: D:20231215143045+05'30'

            string dateStr = string.Format("D:{0:yyyyMMddHHmmss}", date);

            // Add timezone offset
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(date);
            if (offset.TotalMinutes == 0)
            {
                dateStr += "Z";
            }
            else
            {
                char sign = offset.TotalMinutes >= 0 ? '+' : '-';
                int hours = Math.Abs((int)offset.TotalHours);
                int minutes = Math.Abs(offset.Minutes);
                dateStr += string.Format("{0}{1:D2}'{2:D2}'", sign, hours, minutes);
            }

            var stringObj = new HpdfString(dateStr);
            _dict[key] = stringObj;
        }
    }
}
