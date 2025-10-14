/*
 * << Haru Free PDF Library >> -- HpdfWidgetAnnotation.cs
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

using Haru.Annotations;
using Haru.Forms.Appearance;
using Haru.Objects;
using Haru.Types;
using Haru.Xref;

namespace Haru.Forms
{
    /// <summary>
    /// Widget annotation for PDF form fields.
    /// Represents the visual appearance and placement of a form field on a page.
    /// </summary>
    public class HpdfWidgetAnnotation : HpdfAnnotation
    {
        /// <summary>
        /// Creates a new widget annotation.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="rect">The widget rectangle on the page.</param>
        public HpdfWidgetAnnotation(HpdfXref xref, HpdfRect rect)
            : base(xref, HpdfAnnotationType.Widget, rect)
        {
            // Set annotation flags: Print (bit 2 = 4) makes the widget visible/printable
            _dict.Add("F", new HpdfNumber(4));
        }

        /// <summary>
        /// Sets the parent field for this widget.
        /// </summary>
        /// <param name="field">The parent field.</param>
        public void SetParent(HpdfField field)
        {
            if (field == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Field cannot be null");

            _dict.Add("Parent", field.Dict);
        }

        /// <summary>
        /// Sets the page reference for this widget.
        /// </summary>
        /// <param name="page">The page dictionary.</param>
        public void SetPage(HpdfDict page)
        {
            if (page == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Page cannot be null");

            _dict.Add("P", page);
        }

        /// <summary>
        /// Sets the appearance state for this widget.
        /// </summary>
        /// <param name="state">The appearance state name (e.g., "Yes", "Off").</param>
        public void SetAppearanceState(string state)
        {
            if (string.IsNullOrEmpty(state))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "State cannot be null or empty");

            _dict.Remove("AS");
            _dict.Add("AS", new HpdfName(state));
        }

        /// <summary>
        /// Sets the highlighting mode for the widget.
        /// </summary>
        /// <param name="mode">The highlighting mode.</param>
        public void SetHighlightMode(HpdfHighlightMode mode)
        {
            string modeName = mode switch
            {
                HpdfHighlightMode.NoHighlight => "N",
                HpdfHighlightMode.InvertBox => "I",
                HpdfHighlightMode.InvertBorder => "O",
                HpdfHighlightMode.DownAppearance => "P",
                _ => "I"
            };

            _dict.Remove("H");
            _dict.Add("H", new HpdfName(modeName));
        }

        /// <summary>
        /// Sets appearance characteristics dictionary (/MK) for the widget.
        /// </summary>
        /// <param name="borderColor">The border color (RGB, each component 0-1), or null.</param>
        /// <param name="backgroundColor">The background color (RGB, each component 0-1), or null.</param>
        /// <param name="rotation">The rotation angle (0, 90, 180, or 270).</param>
        public void SetAppearanceCharacteristics(float[] borderColor = null, float[] backgroundColor = null, int rotation = 0)
        {
            var mk = new HpdfDict();

            if (borderColor != null && borderColor.Length == 3)
            {
                var bcArray = new HpdfArray();
                bcArray.Add(new HpdfReal(borderColor[0]));
                bcArray.Add(new HpdfReal(borderColor[1]));
                bcArray.Add(new HpdfReal(borderColor[2]));
                mk.Add("BC", bcArray);
            }

            if (backgroundColor != null && backgroundColor.Length == 3)
            {
                var bgArray = new HpdfArray();
                bgArray.Add(new HpdfReal(backgroundColor[0]));
                bgArray.Add(new HpdfReal(backgroundColor[1]));
                bgArray.Add(new HpdfReal(backgroundColor[2]));
                mk.Add("BG", bgArray);
            }

            if (rotation != 0)
            {
                if (rotation != 90 && rotation != 180 && rotation != 270)
                    throw new HpdfException(HpdfErrorCode.InvalidParameter, "Rotation must be 0, 90, 180, or 270");
                mk.Add("R", new HpdfNumber(rotation));
            }

            _dict.Remove("MK");
            if (mk.Count > 0)
                _dict.Add("MK", mk);
        }

        /// <summary>
        /// Creates appearance dictionary for button widgets (checkbox/radio) with actual drawing commands.
        /// Uses the IDrawable interface to generate PDF content streams for each state.
        /// </summary>
        /// <param name="elementType">The type of button element (checkbox or radio button).</param>
        /// <param name="onStateName">The "on" state name (e.g., "Yes" for checkbox, "Male" for radio).</param>
        /// <param name="offStateName">The "off" state name (typically "Off").</param>
        public void SetButtonAppearances(ButtonElementType elementType, string onStateName = "Yes", string offStateName = "Off")
        {
            // Get the widget rectangle to calculate appearance dimensions
            HpdfRect rect = GetRect();
            float width = rect.Right - rect.Left;
            float height = rect.Top - rect.Bottom;

            // Create the appropriate appearance generator using the factory
            IElementAppearance appearanceGenerator = HpdfAppearanceFactory.CreateAppearance(elementType);

            // Create appearance dictionary
            var ap = new HpdfDict();
            var normal = new HpdfDict();

            // Create "on" state stream with actual drawing commands
            // Appearance streams MUST be Form XObjects per PDF spec
            var onStream = new HpdfStreamObject();
            onStream.Add("Type", new HpdfName("XObject"));
            onStream.Add("Subtype", new HpdfName("Form"));
            onStream.Add("BBox", CreateBBoxArray(0, 0, width, height));
            onStream.Add("Resources", new HpdfDict()); // Empty resources dictionary
            _xref.Add(onStream);

            // Use HpdfAppearanceCanvas to draw using primitives
            var onCanvas = new Graphics.HpdfAppearanceCanvas(onStream);
            appearanceGenerator.GenerateOnStateAppearance(onCanvas, width, height);

            // Create "off" state stream with minimal content
            var offStream = new HpdfStreamObject();
            offStream.Add("Type", new HpdfName("XObject"));
            offStream.Add("Subtype", new HpdfName("Form"));
            offStream.Add("BBox", CreateBBoxArray(0, 0, width, height));
            offStream.Add("Resources", new HpdfDict()); // Empty resources dictionary
            _xref.Add(offStream);

            // Use HpdfAppearanceCanvas to draw using primitives
            var offCanvas = new Graphics.HpdfAppearanceCanvas(offStream);
            appearanceGenerator.GenerateOffStateAppearance(offCanvas, width, height);

            // Add states to normal appearance
            normal.Add(onStateName, onStream);
            normal.Add(offStateName, offStream);

            // Set up appearance dictionary
            ap.Add("N", normal);
            _dict.Remove("AP");
            _dict.Add("AP", ap);
        }

        /// <summary>
        /// Gets the rectangle of this widget annotation.
        /// </summary>
        /// <returns>The widget's rectangle.</returns>
        private HpdfRect GetRect()
        {
            // The Rect is stored in the annotation dictionary
            if (_dict.TryGetValue("Rect", out var rectObj) && rectObj is HpdfArray rectArray && rectArray.Count == 4)
            {
                float left = (rectArray[0] as HpdfNumber)?.Value ?? (rectArray[0] as HpdfReal)?.Value ?? 0;
                float bottom = (rectArray[1] as HpdfNumber)?.Value ?? (rectArray[1] as HpdfReal)?.Value ?? 0;
                float right = (rectArray[2] as HpdfNumber)?.Value ?? (rectArray[2] as HpdfReal)?.Value ?? 0;
                float top = (rectArray[3] as HpdfNumber)?.Value ?? (rectArray[3] as HpdfReal)?.Value ?? 0;

                return new HpdfRect(left, bottom, right, top);
            }

            // Default fallback
            return new HpdfRect(0, 0, 15, 15);
        }

        /// <summary>
        /// Creates a BBox (bounding box) array for Form XObjects.
        /// </summary>
        /// <param name="llx">Lower-left x coordinate.</param>
        /// <param name="lly">Lower-left y coordinate.</param>
        /// <param name="urx">Upper-right x coordinate.</param>
        /// <param name="ury">Upper-right y coordinate.</param>
        /// <returns>An HpdfArray representing the bounding box.</returns>
        private HpdfArray CreateBBoxArray(float llx, float lly, float urx, float ury)
        {
            var bbox = new HpdfArray();
            bbox.Add(new HpdfReal(llx));
            bbox.Add(new HpdfReal(lly));
            bbox.Add(new HpdfReal(urx));
            bbox.Add(new HpdfReal(ury));
            return bbox;
        }
    }
}
