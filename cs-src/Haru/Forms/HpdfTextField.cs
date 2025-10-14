/*
 * << Haru Free PDF Library >> -- HpdfTextField.cs
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

using Haru.Objects;
using Haru.Xref;

namespace Haru.Forms
{
    /// <summary>
    /// Text alignment for text fields.
    /// </summary>
    public enum HpdfTextAlignment
    {
        /// <summary>
        /// Left-aligned text (default).
        /// </summary>
        Left = 0,

        /// <summary>
        /// Center-aligned text.
        /// </summary>
        Center = 1,

        /// <summary>
        /// Right-aligned text.
        /// </summary>
        Right = 2
    }

    /// <summary>
    /// Text field for single or multi-line text input.
    /// </summary>
    public class HpdfTextField : HpdfField
    {
        /// <summary>
        /// Creates a new text field.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="name">The field name.</param>
        public HpdfTextField(HpdfXref xref, string name)
            : base(xref, HpdfFieldType.Tx, name)
        {
        }

        /// <summary>
        /// Sets the maximum length of the text field.
        /// </summary>
        /// <param name="maxLength">The maximum number of characters.</param>
        public void SetMaxLength(int maxLength)
        {
            if (maxLength <= 0)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Max length must be positive");

            _dict.Remove("MaxLen");
            _dict.Add("MaxLen", new HpdfNumber(maxLength));
        }

        /// <summary>
        /// Sets the text alignment (quadding) for the field.
        /// </summary>
        /// <param name="alignment">The text alignment.</param>
        public void SetAlignment(HpdfTextAlignment alignment)
        {
            _dict.Remove("Q");
            _dict.Add("Q", new HpdfNumber((int)alignment));
        }

        /// <summary>
        /// Sets whether this is a multiline text field.
        /// </summary>
        /// <param name="multiline">True for multiline, false for single line.</param>
        public void SetMultiline(bool multiline)
        {
            if (multiline)
                Flags |= HpdfFieldFlags.Multiline;
            else
                Flags &= ~HpdfFieldFlags.Multiline;
        }

        /// <summary>
        /// Sets whether this is a password field (characters are masked).
        /// </summary>
        /// <param name="password">True for password field, false for normal text.</param>
        public void SetPassword(bool password)
        {
            if (password)
                Flags |= HpdfFieldFlags.Password;
            else
                Flags &= ~HpdfFieldFlags.Password;
        }

        /// <summary>
        /// Sets whether this field is read-only.
        /// </summary>
        /// <param name="readOnly">True for read-only, false for editable.</param>
        public void SetReadOnly(bool readOnly)
        {
            if (readOnly)
                Flags |= HpdfFieldFlags.ReadOnly;
            else
                Flags &= ~HpdfFieldFlags.ReadOnly;
        }

        /// <summary>
        /// Sets whether this field is required.
        /// </summary>
        /// <param name="required">True if the field must have a value.</param>
        public void SetRequired(bool required)
        {
            if (required)
                Flags |= HpdfFieldFlags.Required;
            else
                Flags &= ~HpdfFieldFlags.Required;
        }
    }
}
