/*
 * << Haru Free PDF Library >> -- CIDWidth.cs
 *
 * Copyright (c) 1999-2025 Haru Free PDF Library
 *
 * Permission to use, copy, modify, distribute and sell this software
 * and its documentation for any purpose is hereby granted without fee,
 */

namespace Haru.Font.CID
{
    /// <summary>
    /// Represents a CID (Character ID) to width mapping.
    /// Width is in 1000-unit glyph space.
    /// </summary>
    public struct CIDWidth
    {
        /// <summary>
        /// Gets or sets the Character ID.
        /// </summary>
        public ushort CID { get; set; }

        /// <summary>
        /// Gets or sets the character width in 1000-unit glyph space.
        /// </summary>
        public short Width { get; set; }
    }
}
