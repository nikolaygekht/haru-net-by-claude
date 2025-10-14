/*
 * << Haru Free PDF Library >> -- ButtonElementType.cs
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

namespace Haru.Forms.Appearance
{
    /// <summary>
    /// Enumeration of button form field element types.
    /// Used by the appearance factory to determine which appearance generator to use.
    /// </summary>
    public enum ButtonElementType
    {
        /// <summary>
        /// Checkbox button (can be checked or unchecked independently).
        /// </summary>
        Checkbox,

        /// <summary>
        /// Radio button (mutually exclusive within a group).
        /// </summary>
        RadioButton
    }
}
