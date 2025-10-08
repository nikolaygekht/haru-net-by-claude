/*
 * << Haru Free PDF Library >> -- HpdfAnnotationType.cs
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

namespace Haru.Annotations
{
    /// <summary>
    /// Types of PDF annotations.
    /// </summary>
    public enum HpdfAnnotationType
    {
        /// <summary>
        /// Text annotation (sticky note).
        /// </summary>
        Text = 0,

        /// <summary>
        /// Link annotation (hyperlink or internal link).
        /// </summary>
        Link,

        /// <summary>
        /// Sound annotation.
        /// </summary>
        Sound,

        /// <summary>
        /// Free text annotation.
        /// </summary>
        FreeText,

        /// <summary>
        /// Stamp annotation.
        /// </summary>
        Stamp,

        /// <summary>
        /// Square annotation.
        /// </summary>
        Square,

        /// <summary>
        /// Circle annotation.
        /// </summary>
        Circle,

        /// <summary>
        /// Strike-out annotation.
        /// </summary>
        StrikeOut,

        /// <summary>
        /// Highlight annotation.
        /// </summary>
        Highlight,

        /// <summary>
        /// Underline annotation.
        /// </summary>
        Underline,

        /// <summary>
        /// Ink annotation.
        /// </summary>
        Ink,

        /// <summary>
        /// File attachment annotation.
        /// </summary>
        FileAttachment,

        /// <summary>
        /// Popup annotation.
        /// </summary>
        Popup,

        /// <summary>
        /// 3D annotation.
        /// </summary>
        ThreeD,

        /// <summary>
        /// Squiggly underline annotation.
        /// </summary>
        Squiggly,

        /// <summary>
        /// Line annotation.
        /// </summary>
        Line,

        /// <summary>
        /// Projection annotation.
        /// </summary>
        Projection,

        /// <summary>
        /// Widget annotation (form field).
        /// </summary>
        Widget
    }

    /// <summary>
    /// Annotation icon types for text annotations.
    /// </summary>
    public enum HpdfAnnotationIcon
    {
        /// <summary>
        /// Comment icon.
        /// </summary>
        Comment = 0,

        /// <summary>
        /// Key icon.
        /// </summary>
        Key,

        /// <summary>
        /// Note icon.
        /// </summary>
        Note,

        /// <summary>
        /// Help icon.
        /// </summary>
        Help,

        /// <summary>
        /// New paragraph icon.
        /// </summary>
        NewParagraph,

        /// <summary>
        /// Paragraph icon.
        /// </summary>
        Paragraph,

        /// <summary>
        /// Insert icon.
        /// </summary>
        Insert
    }

    /// <summary>
    /// Border styles for annotations.
    /// </summary>
    public enum HpdfBorderStyle
    {
        /// <summary>
        /// Solid border.
        /// </summary>
        Solid = 0,

        /// <summary>
        /// Dashed border.
        /// </summary>
        Dashed,

        /// <summary>
        /// Beveled border.
        /// </summary>
        Beveled,

        /// <summary>
        /// Inset border.
        /// </summary>
        Inset,

        /// <summary>
        /// Underlined border.
        /// </summary>
        Underlined
    }

    /// <summary>
    /// Highlight modes for link annotations.
    /// </summary>
    public enum HpdfHighlightMode
    {
        /// <summary>
        /// No highlighting.
        /// </summary>
        NoHighlight = 0,

        /// <summary>
        /// Invert the annotation rectangle.
        /// </summary>
        InvertBox,

        /// <summary>
        /// Invert the annotation border.
        /// </summary>
        InvertBorder,

        /// <summary>
        /// Display the annotation's down appearance.
        /// </summary>
        DownAppearance
    }
}
