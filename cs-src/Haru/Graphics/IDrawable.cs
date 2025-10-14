/*
 * << Haru Free PDF Library >> -- IDrawable.cs
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

using Haru.Streams;
using Haru.Types;

namespace Haru.Graphics
{
    /// <summary>
    /// Interface for objects that support PDF drawing operations.
    /// This includes pages, appearance streams, patterns, and other drawable contexts.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Gets the underlying stream to write PDF operators to.
        /// </summary>
        HpdfMemoryStream Stream { get; }

        /// <summary>
        /// Gets the current graphics state.
        /// </summary>
        HpdfGraphicsState GraphicsState { get; }

        /// <summary>
        /// Gets or sets the current position in the path.
        /// </summary>
        HpdfPoint CurrentPos { get; set; }

        /// <summary>
        /// Pushes the current graphics state onto the stack.
        /// Called internally by GSave operations.
        /// </summary>
        void PushGraphicsState();

        /// <summary>
        /// Pops the graphics state from the stack.
        /// Called internally by GRestore operations.
        /// </summary>
        void PopGraphicsState();
    }
}
