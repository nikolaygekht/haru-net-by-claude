/*
 * << Haru Free PDF Library >> -- CIDWritingMode.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

namespace Haru.Font.CID
{
    /// <summary>
    /// Specifies the writing mode for CID fonts
    /// </summary>
    public enum CIDWritingMode
    {
        /// <summary>
        /// Horizontal writing mode (left to right)
        /// Used for: Chinese, Japanese, Korean (modern)
        /// PDF WMode: 0
        /// </summary>
        Horizontal = 0,

        /// <summary>
        /// Vertical writing mode (top to bottom, right to left)
        /// Used for: Traditional Japanese, Traditional Chinese
        /// PDF WMode: 1
        /// </summary>
        Vertical = 1
    }
}
