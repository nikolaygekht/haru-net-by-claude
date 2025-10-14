/*
 * << Haru Free PDF Library >> -- HpdfFieldFlags.cs
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

namespace Haru.Forms
{
    /// <summary>
    /// PDF form field flags as defined in PDF 1.4 Spec, Tables 8.70-8.77.
    /// These flags control the behavior and appearance of form fields.
    /// </summary>
    [Flags]
    public enum HpdfFieldFlags
    {
        /// <summary>
        /// No flags set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Bit 1: If set, the user may not change the value of the field.
        /// Applies to: All field types.
        /// </summary>
        ReadOnly = 1 << 0,

        /// <summary>
        /// Bit 2: If set, the field must have a value at the time it is exported.
        /// Applies to: All field types.
        /// </summary>
        Required = 1 << 1,

        /// <summary>
        /// Bit 3: If set, the field must not be exported.
        /// Applies to: All field types.
        /// </summary>
        NoExport = 1 << 2,

        /// <summary>
        /// Bit 13: If set, the text field can contain multiple lines.
        /// Applies to: Text fields only.
        /// </summary>
        Multiline = 1 << 12,

        /// <summary>
        /// Bit 14: If set, the text field is a password field.
        /// Applies to: Text fields only.
        /// </summary>
        Password = 1 << 13,

        /// <summary>
        /// Bit 16: If set, exactly one radio button must be selected.
        /// Applies to: Button fields (radio buttons).
        /// </summary>
        Radio = 1 << 15,

        /// <summary>
        /// Bit 17: If set, the button is a pushbutton (not a checkbox or radio).
        /// Applies to: Button fields only.
        /// </summary>
        Pushbutton = 1 << 16,

        /// <summary>
        /// Bit 18: If set, the choice field is a combo box; if clear, it's a list box.
        /// Applies to: Choice fields only.
        /// </summary>
        Combo = 1 << 17,

        /// <summary>
        /// Bit 19: If set, the combo box includes an editable text box.
        /// Applies to: Choice fields (combo boxes only).
        /// </summary>
        Edit = 1 << 18,

        /// <summary>
        /// Bit 20: If set, the field's option items should be sorted alphabetically.
        /// Applies to: Choice fields only.
        /// </summary>
        Sort = 1 << 19,

        /// <summary>
        /// Bit 22: If set, more than one option can be selected simultaneously.
        /// Applies to: Choice fields (list boxes only).
        /// </summary>
        MultiSelect = 1 << 21
    }
}
