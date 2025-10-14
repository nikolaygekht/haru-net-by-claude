/*
 * << Haru Free PDF Library >> -- HpdfFieldType.cs
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

namespace Haru.Forms
{
    /// <summary>
    /// PDF form field types (/FT entry in field dictionary).
    /// </summary>
    public enum HpdfFieldType
    {
        /// <summary>
        /// Button field (pushbutton, checkbox, or radio button).
        /// </summary>
        Btn,

        /// <summary>
        /// Text field (single or multi-line text input).
        /// </summary>
        Tx,

        /// <summary>
        /// Choice field (list box or combo box).
        /// </summary>
        Ch,

        /// <summary>
        /// Signature field (digital signature).
        /// </summary>
        Sig
    }
}
