/*
 * << Haru Free PDF Library >> -- IElementAppearance.cs
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
    /// Interface for generating PDF appearance stream content for form field elements.
    /// Implementations provide the PDF drawing commands for different form element types
    /// (checkboxes, radio buttons, etc.).
    /// </summary>
    public interface IElementAppearance
    {
        /// <summary>
        /// Generates the PDF content stream for the "on" or "checked" state.
        /// </summary>
        /// <param name="width">Width of the annotation rectangle in points.</param>
        /// <param name="height">Height of the annotation rectangle in points.</param>
        /// <returns>PDF content stream commands as a string.</returns>
        string GenerateOnStateAppearance(float width, float height);

        /// <summary>
        /// Generates the PDF content stream for the "off" or "unchecked" state.
        /// </summary>
        /// <param name="width">Width of the annotation rectangle in points.</param>
        /// <param name="height">Height of the annotation rectangle in points.</param>
        /// <returns>PDF content stream commands as a string.</returns>
        string GenerateOffStateAppearance(float width, float height);
    }
}
