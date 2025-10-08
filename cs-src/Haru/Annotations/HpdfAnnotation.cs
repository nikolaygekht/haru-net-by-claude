/*
 * << Haru Free PDF Library >> -- HpdfAnnotation.cs
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

using Haru.Objects;
using Haru.Types;
using Haru.Xref;

namespace Haru.Annotations
{
    /// <summary>
    /// Base class for all PDF annotations.
    /// </summary>
    public class HpdfAnnotation
    {
        private static readonly string[] AnnotationTypeNames = new[]
        {
            "Text",
            "Link",
            "Sound",
            "FreeText",
            "Stamp",
            "Square",
            "Circle",
            "StrikeOut",
            "Highlight",
            "Underline",
            "Ink",
            "FileAttachment",
            "Popup",
            "3D",
            "Squiggly",
            "Line",
            "Projection",
            "Widget"
        };

        protected readonly HpdfDict _dict;
        protected readonly HpdfXref _xref;
        protected readonly HpdfAnnotationType _type;

        /// <summary>
        /// Gets the underlying dictionary object.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets the annotation type.
        /// </summary>
        public HpdfAnnotationType Type => _type;

        /// <summary>
        /// Creates a new annotation.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="type">The annotation type.</param>
        /// <param name="rect">The annotation rectangle.</param>
        protected HpdfAnnotation(HpdfXref xref, HpdfAnnotationType type, HpdfRect rect)
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            _type = type;
            _dict = new HpdfDict();

            // Add to xref
            _xref.Add(_dict);

            // Normalize rectangle (ensure top > bottom)
            if (rect.Top < rect.Bottom)
            {
                float tmp = rect.Top;
                rect.Top = rect.Bottom;
                rect.Bottom = tmp;
            }

            // Create Rect array
            var rectArray = new HpdfArray();
            rectArray.Add(new HpdfReal(rect.Left));
            rectArray.Add(new HpdfReal(rect.Bottom));
            rectArray.Add(new HpdfReal(rect.Right));
            rectArray.Add(new HpdfReal(rect.Top));

            _dict.Add("Type", new HpdfName("Annot"));
            _dict.Add("Subtype", new HpdfName(AnnotationTypeNames[(int)type]));
            _dict.Add("Rect", rectArray);
        }

        /// <summary>
        /// Sets the border style for the annotation.
        /// </summary>
        /// <param name="style">The border style.</param>
        /// <param name="width">The border width (default: 1.0).</param>
        /// <param name="dashOn">The length of dashes for dashed border (default: 3).</param>
        /// <param name="dashOff">The length of gaps for dashed border (default: 3).</param>
        public void SetBorderStyle(HpdfBorderStyle style, float width = 1.0f, ushort dashOn = 3, ushort dashOff = 3)
        {
            var bs = new HpdfDict();
            _dict.Add("BS", bs);

            bs.Add("Type", new HpdfName("Border"));

            // Set style
            switch (style)
            {
                case HpdfBorderStyle.Solid:
                    bs.Add("S", new HpdfName("S"));
                    break;
                case HpdfBorderStyle.Dashed:
                    bs.Add("S", new HpdfName("D"));
                    var dashArray = new HpdfArray();
                    dashArray.Add(new HpdfNumber(dashOn));
                    dashArray.Add(new HpdfNumber(dashOff));
                    bs.Add("D", dashArray);
                    break;
                case HpdfBorderStyle.Beveled:
                    bs.Add("S", new HpdfName("B"));
                    break;
                case HpdfBorderStyle.Inset:
                    bs.Add("S", new HpdfName("I"));
                    break;
                case HpdfBorderStyle.Underlined:
                    bs.Add("S", new HpdfName("U"));
                    break;
                default:
                    throw new HpdfException(HpdfErrorCode.InvalidParameter, "Invalid border style");
            }

            if (width != 1.0f)
                bs.Add("W", new HpdfReal(width));
        }

        /// <summary>
        /// Sets the RGB color for the annotation.
        /// </summary>
        /// <param name="r">Red component (0.0 - 1.0).</param>
        /// <param name="g">Green component (0.0 - 1.0).</param>
        /// <param name="b">Blue component (0.0 - 1.0).</param>
        public void SetRgbColor(float r, float g, float b)
        {
            if (r < 0 || r > 1 || g < 0 || g > 1 || b < 0 || b > 1)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Color values must be between 0 and 1");

            var colorArray = new HpdfArray();
            colorArray.Add(new HpdfReal(r));
            colorArray.Add(new HpdfReal(g));
            colorArray.Add(new HpdfReal(b));
            _dict.Add("C", colorArray);
        }

        /// <summary>
        /// Sets the CMYK color for the annotation.
        /// </summary>
        /// <param name="c">Cyan component (0.0 - 1.0).</param>
        /// <param name="m">Magenta component (0.0 - 1.0).</param>
        /// <param name="y">Yellow component (0.0 - 1.0).</param>
        /// <param name="k">Black component (0.0 - 1.0).</param>
        public void SetCmykColor(float c, float m, float y, float k)
        {
            if (c < 0 || c > 1 || m < 0 || m > 1 || y < 0 || y > 1 || k < 0 || k > 1)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Color values must be between 0 and 1");

            var colorArray = new HpdfArray();
            colorArray.Add(new HpdfReal(c));
            colorArray.Add(new HpdfReal(m));
            colorArray.Add(new HpdfReal(y));
            colorArray.Add(new HpdfReal(k));
            _dict.Add("C", colorArray);
        }
    }
}
