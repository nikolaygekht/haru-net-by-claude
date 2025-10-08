/*
 * << Haru Free PDF Library >> -- HpdfPageLayout.cs
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
    /// Specifies the page layout to be used when the document is opened.
    /// </summary>
    public enum HpdfPageLayout
    {
        /// <summary>
        /// Display one page at a time.
        /// </summary>
        SinglePage = 0,

        /// <summary>
        /// Display pages in one column.
        /// </summary>
        OneColumn,

        /// <summary>
        /// Display pages in two columns, with odd-numbered pages on the left.
        /// </summary>
        TwoColumnLeft,

        /// <summary>
        /// Display pages in two columns, with odd-numbered pages on the right.
        /// </summary>
        TwoColumnRight,

        /// <summary>
        /// Display pages two at a time, with odd-numbered pages on the left (PDF 1.5).
        /// </summary>
        TwoPageLeft,

        /// <summary>
        /// Display pages two at a time, with odd-numbered pages on the right (PDF 1.5).
        /// </summary>
        TwoPageRight
    }
}
