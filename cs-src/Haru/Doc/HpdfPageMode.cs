/*
 * << Haru Free PDF Library >> -- HpdfPageMode.cs
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

namespace Haru.Doc
{
    /// <summary>
    /// Specifies how the document should be displayed when opened.
    /// </summary>
    public enum HpdfPageMode
    {
        /// <summary>
        /// Neither document outline nor thumbnail images visible.
        /// </summary>
        UseNone = 0,

        /// <summary>
        /// Document outline visible.
        /// </summary>
        UseOutlines,

        /// <summary>
        /// Thumbnail images visible.
        /// </summary>
        UseThumbs,

        /// <summary>
        /// Full-screen mode, with no menu bar, window controls, or any other window visible.
        /// </summary>
        FullScreen,

        /// <summary>
        /// Optional content group panel visible (PDF 1.5).
        /// </summary>
        UseOC,

        /// <summary>
        /// Attachments panel visible (PDF 1.6).
        /// </summary>
        UseAttachments
    }
}
