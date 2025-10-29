/*
 * << Haru Free PDF Library >> -- HpdfPageShapes.cs
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
using Haru.Graphics;

namespace Haru.Doc
{
    /// <summary>
    /// Shape drawing operations for HpdfPage
    /// </summary>
    public static class HpdfPageShapes
    {
        private const float KAPPA = 0.5522847498f; // 4/3 * (sqrt(2) - 1), magic number for circle approximation

        /// <summary>
        /// Draws a circle centered at (x, y) with the given radius.
        /// Uses Bezier curves to approximate the circle.
        /// </summary>
        public static void Circle(this IDrawable drawable, float x, float y, float radius)
        {
            if (radius <= 0)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Radius must be positive");

            // Start at leftmost point
            drawable.MoveTo(x - radius, y);

            // Draw four quarter circles using cubic Bezier curves
            // The control points are offset by KAPPA * radius from the endpoints

            // First quarter: left to top
            drawable.CurveTo(
                x - radius, y + radius * KAPPA,    // control point 1
                x - radius * KAPPA, y + radius,    // control point 2
                x, y + radius                      // end point
            );

            // Second quarter: top to right
            drawable.CurveTo(
                x + radius * KAPPA, y + radius,    // control point 1
                x + radius, y + radius * KAPPA,    // control point 2
                x + radius, y                      // end point
            );

            // Third quarter: right to bottom
            drawable.CurveTo(
                x + radius, y - radius * KAPPA,    // control point 1
                x + radius * KAPPA, y - radius,    // control point 2
                x, y - radius                      // end point
            );

            // Fourth quarter: bottom to left
            drawable.CurveTo(
                x - radius * KAPPA, y - radius,    // control point 1
                x - radius, y - radius * KAPPA,    // control point 2
                x - radius, y                      // end point (back to start)
            );
        }

        /// <summary>
        /// Draws an ellipse centered at (x, y) with the given x and y radii.
        /// </summary>
        public static void Ellipse(this IDrawable drawable, float x, float y, float xRadius, float yRadius)
        {
            if (xRadius <= 0 || yRadius <= 0)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Radii must be positive");

            // Start at leftmost point
            drawable.MoveTo(x - xRadius, y);

            // Draw four quarter ellipses using cubic Bezier curves

            // First quarter: left to top
            drawable.CurveTo(
                x - xRadius, y + yRadius * KAPPA,    // control point 1
                x - xRadius * KAPPA, y + yRadius,    // control point 2
                x, y + yRadius                       // end point
            );

            // Second quarter: top to right
            drawable.CurveTo(
                x + xRadius * KAPPA, y + yRadius,    // control point 1
                x + xRadius, y + yRadius * KAPPA,    // control point 2
                x + xRadius, y                       // end point
            );

            // Third quarter: right to bottom
            drawable.CurveTo(
                x + xRadius, y - yRadius * KAPPA,    // control point 1
                x + xRadius * KAPPA, y - yRadius,    // control point 2
                x, y - yRadius                       // end point
            );

            // Fourth quarter: bottom to left
            drawable.CurveTo(
                x - xRadius * KAPPA, y - yRadius,    // control point 1
                x - xRadius, y - yRadius * KAPPA,    // control point 2
                x - xRadius, y                       // end point (back to start)
            );
        }

        /// <summary>
        /// Draws an arc centered at (x, y) with the given radius from angle1 to angle2 (in degrees).
        /// Angles are measured counterclockwise from the positive x-axis.
        /// </summary>
        public static void Arc(this IDrawable drawable, float x, float y, float radius, float angle1, float angle2)
        {
            if (radius <= 0)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Radius must be positive");

            if (Math.Abs(angle2 - angle1) >= 360)
                throw new HpdfException(HpdfErrorCode.PageOutOfRange, "Arc angle range must be < 360 degrees");

            // Normalize angles to 0-360 range
            while (angle1 < 0 || angle2 < 0)
            {
                angle1 += 360;
                angle2 += 360;
            }

            bool continueArc = false;

            // Draw arc in segments of at most 90 degrees
            while (true)
            {
                if (Math.Abs(angle2 - angle1) <= 90)
                {
                    DrawArcSegment(drawable, x, y, radius, angle1, angle2, continueArc);
                    break;
                }
                else
                {
                    float tmpAngle = angle2 > angle1 ? angle1 + 90 : angle1 - 90;
                    DrawArcSegment(drawable, x, y, radius, angle1, tmpAngle, continueArc);
                    angle1 = tmpAngle;
                }

                if (Math.Abs(angle1 - angle2) < 0.1f)
                    break;

                continueArc = true;
            }
        }

        private static void DrawArcSegment(IDrawable drawable, float x, float y, float radius, float angle1, float angle2, bool continueArc)
        {
            const float PI = (float)Math.PI;

            // Convert angles to radians
            float deltaAngle = (90 - (angle1 + angle2) / 2) * PI / 180;
            float newAngle = (angle2 - angle1) / 2 * PI / 180;

            // Calculate control points for Bezier curve approximation
            double rx0 = radius * Math.Cos(newAngle);
            double ry0 = radius * Math.Sin(newAngle);
            double rx2 = (radius * 4.0 - rx0) / 3.0;
            double ry2 = ((radius * 1.0 - rx0) * (rx0 - radius * 3.0)) / (3.0 * ry0);
            double rx1 = rx2;
            double ry1 = -ry2;
            double rx3 = rx0;
            double ry3 = -ry0;

            // Rotate control points by deltaAngle
            double cos_delta = Math.Cos(deltaAngle);
            double sin_delta = Math.Sin(deltaAngle);

            double x0 = rx0 * cos_delta - ry0 * sin_delta + x;
            double y0 = rx0 * sin_delta + ry0 * cos_delta + y;
            double x1 = rx1 * cos_delta - ry1 * sin_delta + x;
            double y1 = rx1 * sin_delta + ry1 * cos_delta + y;
            double x2 = rx2 * cos_delta - ry2 * sin_delta + x;
            double y2 = rx2 * sin_delta + ry2 * cos_delta + y;
            double x3 = rx3 * cos_delta - ry3 * sin_delta + x;
            double y3 = rx3 * sin_delta + ry3 * cos_delta + y;

            if (!continueArc)
            {
                // Start new arc segment
                drawable.MoveTo((float)x0, (float)y0);
            }

            // Draw the arc segment using a cubic Bezier curve
            drawable.CurveTo((float)x1, (float)y1, (float)x2, (float)y2, (float)x3, (float)y3);
        }
    }
}
