/*
 * << Haru Free PDF Library >> -- HpdfCheckboxAppearance.cs
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

namespace Haru.Forms.Appearance
{
    /// <summary>
    /// Generates PDF appearance streams for checkbox form fields.
    /// Uses PDF path drawing commands to create a checkmark (✓) for the checked state.
    /// </summary>
    public class HpdfCheckboxAppearance : IElementAppearance
    {
        /// <summary>
        /// Generates the PDF content stream for a checked checkbox (checkmark appearance).
        /// Uses normalized coordinates with transformation matrix for scaling.
        /// </summary>
        /// <param name="width">Width of the annotation rectangle in points.</param>
        /// <param name="height">Height of the annotation rectangle in points.</param>
        /// <returns>PDF content stream commands drawing a checkmark.</returns>
        public string GenerateOnStateAppearance(float width, float height)
        {
            // Checkmark path in normalized coordinates (0-1 range)
            // This creates a stylized checkmark using line and Bezier curve commands
            const string checkPath =
                "0 0 m\n" +                                          // Move to origin
                "0.066 -0.026 l\n" +                                 // Line to
                "0.137 -0.15 l\n" +                                  // Line to
                "0.259 0.081 0.46 0.391 0.553 0.461 c\n" +          // Bezier curve (cp1_x cp1_y cp2_x cp2_y end_x end_y)
                "0.604 0.489 l\n" +                                  // Line to
                "0.703 0.492 l\n" +                                  // Line to
                "0.543 0.312 0.255 -0.205 0.154 -0.439 c\n" +       // Bezier curve
                "0.069 -0.399 l\n" +                                 // Line to
                "0.035 -0.293 -0.039 -0.136 -0.091 -0.057 c\n" +   // Bezier curve
                "h\n" +                                               // Close path
                "f\n";                                                // Fill path

            var sb = new StringBuilder();

            // Save graphics state
            sb.Append("q\n");

            // Set fill color to black (RGB: 0, 0, 0)
            sb.Append("0 0 0 rg\n");

            // Apply transformation matrix to scale and position the checkmark
            // Matrix format: a b c d e f cm
            // where: a=scaleX, b=0, c=0, d=scaleY, e=translateX, f=translateY
            // Scale to 80% of widget size and center it
            float scaleX = width * 0.8f;
            float scaleY = height * 0.8f;
            float translateX = width * 0.3f;
            float translateY = height * 0.5f;

            sb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                "{0:0.####} 0 0 {1:0.####} {2:0.####} {3:0.####} cm\n",
                scaleX, scaleY, translateX, translateY);

            // Append the checkmark path
            sb.Append(checkPath);

            // Restore graphics state
            sb.Append("Q\n");

            return sb.ToString();
        }

        /// <summary>
        /// Generates the PDF content stream for an unchecked checkbox (empty appearance).
        /// Returns minimal valid PDF content stream.
        /// </summary>
        /// <param name="width">Width of the annotation rectangle in points.</param>
        /// <param name="height">Height of the annotation rectangle in points.</param>
        /// <returns>PDF content stream commands (empty appearance).</returns>
        public string GenerateOffStateAppearance(float width, float height)
        {
            // For "Off" state, we just need minimal valid PDF content
            // The border will be drawn by the annotation's border properties
            return "q\nQ\n";
        }
    }
}
