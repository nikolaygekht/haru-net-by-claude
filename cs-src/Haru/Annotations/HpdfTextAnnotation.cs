/*
 * << Haru Free PDF Library >> -- HpdfTextAnnotation.cs
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
    /// Text annotation - creates sticky notes.
    /// </summary>
    public class HpdfTextAnnotation : HpdfAnnotation
    {
        private static readonly string[] IconNames = new[]
        {
            "Comment",
            "Key",
            "Note",
            "Help",
            "NewParagraph",
            "Paragraph",
            "Insert"
        };

        /// <summary>
        /// Creates a new text annotation.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="rect">The annotation rectangle.</param>
        /// <param name="text">The text content of the annotation.</param>
        /// <param name="icon">The icon to display (default: Note).</param>
        public HpdfTextAnnotation(HpdfXref xref, HpdfRect rect, string text,
            HpdfAnnotationIcon icon = HpdfAnnotationIcon.Note)
            : base(xref, HpdfAnnotationType.Text, rect)
        {
            if (string.IsNullOrEmpty(text))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Text cannot be null or empty");

            _dict.Add("Contents", new HpdfString(text));
            _dict.Add("Name", new HpdfName(IconNames[(int)icon]));
        }

        /// <summary>
        /// Sets whether the annotation is initially open.
        /// </summary>
        /// <param name="open">True to show the annotation open initially.</param>
        public void SetOpened(bool open)
        {
            _dict.Add("Open", new HpdfBoolean(open));
        }
    }
}
