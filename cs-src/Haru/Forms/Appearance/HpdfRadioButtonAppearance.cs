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
using System.Text;

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
        /// Generates the PDF content stream for a selected radio button (filled circle).
        /// Uses four Bezier curves to approximate a perfect circle.
        /// </summary>
        /// <param name="width">Width of the annotation rectangle in points.</param>
        /// <param name="height">Height of the annotation rectangle in points.</param>
        /// <returns>PDF content stream commands drawing a filled circle.</returns>
        public string GenerateOnStateAppearance(float width, float height)
        {
            var sb = new StringBuilder();

            // Save graphics state
            sb.Append("q\n");

            // Set fill color to black (RGB: 0, 0, 0)
            sb.Append("0 0 0 rg\n");

            // Calculate circle center and radius
            float centerX = width / 2f;
            float centerY = height / 2f;
            float radius = Math.Min(width, height) / 2f * _fillRatio;

            // Draw circle using 4 Bezier curves (standard technique)
            // Magic constant for circle approximation with cubic Bezier curves:
            // kappa = 4/3 * tan(π/8) ≈ 0.5522847498
            // This value ensures the Bezier curve closely approximates a circular arc
            const float kappa = 0.5522847498f;
            float controlDist = radius * kappa;

            // Move to the rightmost point of the circle (3 o'clock position)
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:0.####} {1:0.####} m\n",
                centerX + radius, centerY);

            // Draw top-right quarter (3 o'clock to 12 o'clock)
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n",
                centerX + radius, centerY + controlDist,        // Control point 1
                centerX + controlDist, centerY + radius,        // Control point 2
                centerX, centerY + radius);                      // End point (12 o'clock)

            // Draw top-left quarter (12 o'clock to 9 o'clock)
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n",
                centerX - controlDist, centerY + radius,        // Control point 1
                centerX - radius, centerY + controlDist,        // Control point 2
                centerX - radius, centerY);                      // End point (9 o'clock)

            // Draw bottom-left quarter (9 o'clock to 6 o'clock)
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n",
                centerX - radius, centerY - controlDist,        // Control point 1
                centerX - controlDist, centerY - radius,        // Control point 2
                centerX, centerY - radius);                      // End point (6 o'clock)

            // Draw bottom-right quarter (6 o'clock to 3 o'clock)
            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:0.####} {1:0.####} {2:0.####} {3:0.####} {4:0.####} {5:0.####} c\n",
                centerX + controlDist, centerY - radius,        // Control point 1
                centerX + radius, centerY - controlDist,        // Control point 2
                centerX + radius, centerY);                      // End point (3 o'clock, back to start)

            // Fill the path
            sb.Append("f\n");

            // Restore graphics state
            sb.Append("Q\n");

            return sb.ToString();
        }

        /// <summary>
        /// Generates the PDF content stream for an unselected radio button (empty appearance).
        /// Returns minimal valid PDF content stream.
        /// </summary>
        /// <param name="width">Width of the annotation rectangle in points.</param>
        /// <param name="height">Height of the annotation rectangle in points.</param>
        /// <returns>PDF content stream commands (empty appearance).</returns>
        public string GenerateOffStateAppearance(float width, float height)
        {
            // For "Off" state, we just need minimal valid PDF content
            // The circular border will be drawn by the annotation's border properties
            return "q\nQ\n";
        }
    }
}
