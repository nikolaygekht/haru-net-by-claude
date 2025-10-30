/*
 * << Haru Free PDF Library >> -- HpdfGraphicsState.cs
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

using Haru.Types;

namespace Haru.Graphics
{
    /// <summary>
    /// Represents the current graphics state for a page.
    /// This includes line properties, colors, transformation matrix, text state, etc.
    /// </summary>
    public class HpdfGraphicsState
    {
        /// <summary>
        /// Current transformation matrix
        /// </summary>
        public HpdfTransMatrix TransMatrix { get; set; }

        /// <summary>
        /// Line width (default: 1.0)
        /// </summary>
        public float LineWidth { get; set; }

        /// <summary>
        /// Line cap style
        /// </summary>
        public HpdfLineCap LineCap { get; set; }

        /// <summary>
        /// Line join style
        /// </summary>
        public HpdfLineJoin LineJoin { get; set; }

        /// <summary>
        /// Miter limit (default: 10.0)
        /// </summary>
        public float MiterLimit { get; set; }

        /// <summary>
        /// Dash pattern
        /// </summary>
        public HpdfDashMode DashMode { get; set; }

        /// <summary>
        /// Flatness tolerance (default: 1.0)
        /// </summary>
        public float Flatness { get; set; }

        // Text state properties
        /// <summary>
        /// Character spacing
        /// </summary>
        public float CharSpace { get; set; }

        /// <summary>
        /// Word spacing
        /// </summary>
        public float WordSpace { get; set; }

        /// <summary>
        /// Horizontal scaling (default: 100)
        /// </summary>
        public float HorizontalScaling { get; set; }

        /// <summary>
        /// Text leading (line spacing)
        /// </summary>
        public float TextLeading { get; set; }

        /// <summary>
        /// Text rendering mode
        /// </summary>
        public int RenderingMode { get; set; }

        /// <summary>
        /// Text rise (baseline adjustment)
        /// </summary>
        public float TextRise { get; set; }

        // Color state properties
        /// <summary>
        /// Fill color space
        /// </summary>
        public HpdfColorSpace FillColorSpace { get; set; }

        /// <summary>
        /// Stroke color space
        /// </summary>
        public HpdfColorSpace StrokeColorSpace { get; set; }

        /// <summary>
        /// RGB fill color
        /// </summary>
        public HpdfRgbColor RgbFill { get; set; }

        /// <summary>
        /// RGB stroke color
        /// </summary>
        public HpdfRgbColor RgbStroke { get; set; }

        /// <summary>
        /// CMYK fill color
        /// </summary>
        public HpdfCmykColor CmykFill { get; set; }

        /// <summary>
        /// CMYK stroke color
        /// </summary>
        public HpdfCmykColor CmykStroke { get; set; }

        /// <summary>
        /// Grayscale fill color (0.0-1.0)
        /// </summary>
        public float GrayFill { get; set; }

        /// <summary>
        /// Grayscale stroke color (0.0-1.0)
        /// </summary>
        public float GrayStroke { get; set; }

        /// <summary>
        /// Font size
        /// </summary>
        public float FontSize { get; set; }

        /// <summary>
        /// Previous graphics state (for state stack)
        /// </summary>
        public HpdfGraphicsState? Previous { get; set; }

        /// <summary>
        /// Depth in the graphics state stack
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Creates a new graphics state with default values
        /// </summary>
        public HpdfGraphicsState()
        {
            // Initialize with PDF defaults
            TransMatrix = new HpdfTransMatrix(1, 0, 0, 1, 0, 0);
            LineWidth = 1.0f;
            LineCap = HpdfLineCap.ButtEnd;
            LineJoin = HpdfLineJoin.MiterJoin;
            MiterLimit = 10.0f;
            DashMode = new HpdfDashMode();
            Flatness = 1.0f;

            CharSpace = 0;
            WordSpace = 0;
            HorizontalScaling = 100;
            TextLeading = 0;
            RenderingMode = 0;
            TextRise = 0;

            FillColorSpace = HpdfColorSpace.DeviceGray;
            StrokeColorSpace = HpdfColorSpace.DeviceGray;
            RgbFill = new HpdfRgbColor(0, 0, 0);
            RgbStroke = new HpdfRgbColor(0, 0, 0);
            CmykFill = new HpdfCmykColor(0, 0, 0, 1);
            CmykStroke = new HpdfCmykColor(0, 0, 0, 1);
            GrayFill = 0;
            GrayStroke = 0;

            FontSize = 0;
            Previous = (HpdfGraphicsState?)null;
            Depth = 0;
        }

        /// <summary>
        /// Creates a copy of the current state (for pushing onto stack)
        /// </summary>
        public HpdfGraphicsState Clone()
        {
            return new HpdfGraphicsState
            {
                TransMatrix = new HpdfTransMatrix(TransMatrix.A, TransMatrix.B, TransMatrix.C,
                    TransMatrix.D, TransMatrix.X, TransMatrix.Y),
                LineWidth = LineWidth,
                LineCap = LineCap,
                LineJoin = LineJoin,
                MiterLimit = MiterLimit,
                DashMode = new HpdfDashMode(DashMode.GetActivePattern(), DashMode.Phase),
                Flatness = Flatness,
                CharSpace = CharSpace,
                WordSpace = WordSpace,
                HorizontalScaling = HorizontalScaling,
                TextLeading = TextLeading,
                RenderingMode = RenderingMode,
                TextRise = TextRise,
                FillColorSpace = FillColorSpace,
                StrokeColorSpace = StrokeColorSpace,
                RgbFill = RgbFill,
                RgbStroke = RgbStroke,
                CmykFill = CmykFill,
                CmykStroke = CmykStroke,
                GrayFill = GrayFill,
                GrayStroke = GrayStroke,
                FontSize = FontSize,
                Previous = this,
                Depth = Depth + 1
            };
        }
    }
}
