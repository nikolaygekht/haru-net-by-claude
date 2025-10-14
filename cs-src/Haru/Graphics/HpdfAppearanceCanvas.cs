/*
 * << Haru Free PDF Library >> -- HpdfAppearanceCanvas.cs
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
using Haru.Streams;
using Haru.Types;

namespace Haru.Graphics
{
    /// <summary>
    /// A drawable canvas for creating appearance streams for PDF form field widgets.
    /// This class wraps a stream object (typically a Form XObject) and provides
    /// the same drawing API as HpdfPage through the IDrawable interface.
    /// </summary>
    public class HpdfAppearanceCanvas : IDrawable
    {
        private readonly HpdfStreamObject _streamObject;
        private HpdfGraphicsState _graphicsState;
        private HpdfPoint _currentPos;

        /// <summary>
        /// Gets the underlying stream object (Form XObject).
        /// </summary>
        public HpdfStreamObject StreamObject => _streamObject;

        /// <summary>
        /// Gets the underlying stream to write PDF operators to.
        /// Implementation of IDrawable interface.
        /// </summary>
        public HpdfMemoryStream Stream => _streamObject.Stream;

        /// <summary>
        /// Gets the current graphics state.
        /// </summary>
        public HpdfGraphicsState GraphicsState => _graphicsState;

        /// <summary>
        /// Gets or sets the current position in the path.
        /// </summary>
        public HpdfPoint CurrentPos
        {
            get => _currentPos;
            set => _currentPos = value;
        }

        /// <summary>
        /// Creates a new appearance canvas that wraps a stream object.
        /// </summary>
        /// <param name="streamObject">The stream object to draw on (typically a Form XObject for appearance streams).</param>
        public HpdfAppearanceCanvas(HpdfStreamObject streamObject)
        {
            _streamObject = streamObject ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Stream object cannot be null");
            _graphicsState = new HpdfGraphicsState();
            _currentPos = new HpdfPoint(0, 0);
        }

        /// <summary>
        /// Pushes the current graphics state onto the stack.
        /// Implementation of IDrawable interface.
        /// </summary>
        public void PushGraphicsState()
        {
            _graphicsState = _graphicsState.Clone();
        }

        /// <summary>
        /// Pops the graphics state from the stack.
        /// Implementation of IDrawable interface.
        /// </summary>
        public void PopGraphicsState()
        {
            if (_graphicsState.Previous != null)
            {
                _graphicsState = _graphicsState.Previous;
            }
        }
    }
}
