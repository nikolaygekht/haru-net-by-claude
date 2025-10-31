/*
 * << Haru Free PDF Library >> -- HpdfLinkAnnotation.cs
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
    /// Link annotation - creates clickable links to URIs or internal destinations.
    /// </summary>
    public class HpdfLinkAnnotation : HpdfAnnotation
    {
        /// <summary>
        /// Creates a new URI link annotation.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="rect">The annotation rectangle.</param>
        /// <param name="uri">The URI to link to.</param>
        public HpdfLinkAnnotation(HpdfXref xref, HpdfRect rect, string uri)
            : base(xref, HpdfAnnotationType.Link, rect)
        {
            if (string.IsNullOrEmpty(uri))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "URI cannot be null or empty");

            // Create action dictionary for URI link
            var action = new HpdfDict();
            action.Add("Type", new HpdfName("Action"));
            action.Add("S", new HpdfName("URI"));
            action.Add("URI", new HpdfString(uri));

            _dict.Add("A", action);
        }

        /// <summary>
        /// Creates a new internal link annotation (GoTo action).
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="rect">The annotation rectangle.</param>
        /// <param name="destination">The destination array.</param>
        public HpdfLinkAnnotation(HpdfXref xref, HpdfRect rect, HpdfArray destination)
            : base(xref, HpdfAnnotationType.Link, rect)
        {
            if (destination is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Destination cannot be null");

            _dict.Add("Dest", destination);
        }

        /// <summary>
        /// Sets the highlight mode for the link.
        /// </summary>
        /// <param name="mode">The highlight mode.</param>
        public void SetHighlightMode(HpdfHighlightMode mode)
        {
            string modeName = mode switch
            {
                HpdfHighlightMode.NoHighlight => "N",
                HpdfHighlightMode.InvertBox => "I",
                HpdfHighlightMode.InvertBorder => "O",
                HpdfHighlightMode.DownAppearance => "P",
                _ => throw new HpdfException(HpdfErrorCode.InvalidParameter, "Invalid highlight mode")
            };

            _dict.Add("H", new HpdfName(modeName));
        }
    }
}
