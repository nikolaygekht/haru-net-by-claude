/*
 * << Haru Free PDF Library >> -- HpdfRadioButtonAppearance.cs
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
using Haru.Doc;
using Haru.Graphics;

namespace Haru.Forms.Appearance
{
    /// <summary>
    /// Generates PDF appearance streams for radio button form fields.
    /// Uses PDF path drawing commands to create a filled circle for the selected state.
    /// </summary>
    public class HpdfRadioButtonAppearance : IElementAppearance
    {
        private readonly float _fillRatio;

        /// <summary>
        /// Creates a new radio button appearance generator.
        /// </summary>
        /// <param name="fillRatio">
        /// Ratio of the filled circle size relative to the widget size (default 0.5 for half-size).
        /// This creates the characteristic "dot in circle" appearance of radio buttons.
        /// </param>
        public HpdfRadioButtonAppearance(float fillRatio = 0.5f)
        {
            if (fillRatio <= 0 || fillRatio > 1)
                throw new ArgumentOutOfRangeException(nameof(fillRatio), "Fill ratio must be between 0 and 1");

            _fillRatio = fillRatio;
        }

        /// <summary>
        /// Draws the selected radio button appearance (filled circle) using drawing primitives.
        /// </summary>
        /// <param name="drawable">The drawable canvas to draw on.</param>
        /// <param name="width">Width of the annotation rectangle in points.</param>
        /// <param name="height">Height of the annotation rectangle in points.</param>
        public void GenerateOnStateAppearance(IDrawable drawable, float width, float height)
        {
            // Save graphics state
            drawable.GSave();

            // Set fill color to black (RGB: 0, 0, 0)
            drawable.SetRgbFill(0, 0, 0);

            // Calculate circle center and radius
            float centerX = width / 2f;
            float centerY = height / 2f;
            float radius = Math.Min(width, height) / 2f * _fillRatio;

            // Draw filled circle using the built-in Circle primitive
            drawable.Circle(centerX, centerY, radius);
            drawable.Fill();

            // Restore graphics state
            drawable.GRestore();
        }

        /// <summary>
        /// Draws the unselected radio button appearance (empty) using drawing primitives.
        /// This creates a minimal valid appearance stream (empty circle).
        /// </summary>
        /// <param name="drawable">The drawable canvas to draw on.</param>
        /// <param name="width">Width of the annotation rectangle in points.</param>
        /// <param name="height">Height of the annotation rectangle in points.</param>
        public void GenerateOffStateAppearance(IDrawable drawable, float width, float height)
        {
            // For "Off" state, we just need minimal valid PDF content
            // The circular border will be drawn by the annotation's border properties
            drawable.GSave();
            drawable.GRestore();
        }
    }
}
