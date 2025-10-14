/*
 * << Haru Free PDF Library >> -- HpdfAppearanceFactory.cs
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

namespace Haru.Forms.Appearance
{
    /// <summary>
    /// Factory class for creating appropriate appearance generators for form field elements.
    /// Implements the Factory pattern to encapsulate appearance creation logic.
    /// </summary>
    public static class HpdfAppearanceFactory
    {
        /// <summary>
        /// Creates an appearance generator for the specified button element type.
        /// </summary>
        /// <param name="elementType">The type of button element (checkbox or radio button).</param>
        /// <returns>An IElementAppearance implementation appropriate for the element type.</returns>
        /// <exception cref="ArgumentException">Thrown if the element type is not supported.</exception>
        public static IElementAppearance CreateAppearance(ButtonElementType elementType)
        {
            return elementType switch
            {
                ButtonElementType.Checkbox => new HpdfCheckboxAppearance(),
                ButtonElementType.RadioButton => new HpdfRadioButtonAppearance(fillRatio: 0.5f),
                _ => throw new ArgumentException($"Unsupported element type: {elementType}", nameof(elementType))
            };
        }

        /// <summary>
        /// Creates a radio button appearance generator with a custom fill ratio.
        /// </summary>
        /// <param name="fillRatio">
        /// Ratio of the filled circle size relative to the widget size (0.0 to 1.0).
        /// Default is 0.5 for the characteristic "dot in circle" appearance.
        /// </param>
        /// <returns>A radio button appearance generator with the specified fill ratio.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if fillRatio is not between 0 and 1.</exception>
        public static IElementAppearance CreateRadioButtonAppearance(float fillRatio = 0.5f)
        {
            return new HpdfRadioButtonAppearance(fillRatio);
        }
    }
}
