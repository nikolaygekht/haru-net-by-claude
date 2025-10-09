/*
 * << Haru Free PDF Library >> -- HpdfDestination.cs
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

namespace Haru.Doc
{
    /// <summary>
    /// Represents a destination in a PDF document (navigation target for links and bookmarks).
    /// </summary>
    public class HpdfDestination
    {
        private readonly HpdfArray _destArray;
        private readonly HpdfPage _page;

        /// <summary>
        /// Gets the destination array.
        /// </summary>
        public HpdfArray DestArray => _destArray;

        /// <summary>
        /// Gets the target page.
        /// </summary>
        public HpdfPage Page => _page;

        /// <summary>
        /// Creates a new destination for the specified page.
        /// </summary>
        /// <param name="page">The target page.</param>
        public HpdfDestination(HpdfPage page)
        {
            _page = page ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Page cannot be null");
            _destArray = new HpdfArray();
            _destArray.Add(page.Dict);
        }

        /// <summary>
        /// Sets the destination to display the page at a specific position with zoom.
        /// Format: [page /XYZ left top zoom]
        /// </summary>
        /// <param name="left">Left coordinate (null for current position).</param>
        /// <param name="top">Top coordinate (null for current position).</param>
        /// <param name="zoom">Zoom factor (null for current zoom).</param>
        public void SetXYZ(float left, float top, float zoom)
        {
            // Clear any existing destination type
            if (_destArray.Count > 1)
            {
                while (_destArray.Count > 1)
                    _destArray.RemoveAt(_destArray.Count - 1);
            }

            _destArray.Add(new HpdfName("XYZ"));
            _destArray.Add(new HpdfReal(left));
            _destArray.Add(new HpdfReal(top));
            _destArray.Add(new HpdfReal(zoom));
        }

        /// <summary>
        /// Sets the destination to fit the entire page in the window.
        /// Format: [page /Fit]
        /// </summary>
        public void SetFit()
        {
            if (_destArray.Count > 1)
            {
                while (_destArray.Count > 1)
                    _destArray.RemoveAt(_destArray.Count - 1);
            }

            _destArray.Add(new HpdfName("Fit"));
        }

        /// <summary>
        /// Sets the destination to fit the page width in the window.
        /// Format: [page /FitH top]
        /// </summary>
        /// <param name="top">Top coordinate.</param>
        public void SetFitH(float top)
        {
            if (_destArray.Count > 1)
            {
                while (_destArray.Count > 1)
                    _destArray.RemoveAt(_destArray.Count - 1);
            }

            _destArray.Add(new HpdfName("FitH"));
            _destArray.Add(new HpdfReal(top));
        }

        /// <summary>
        /// Sets the destination to fit the page height in the window.
        /// Format: [page /FitV left]
        /// </summary>
        /// <param name="left">Left coordinate.</param>
        public void SetFitV(float left)
        {
            if (_destArray.Count > 1)
            {
                while (_destArray.Count > 1)
                    _destArray.RemoveAt(_destArray.Count - 1);
            }

            _destArray.Add(new HpdfName("FitV"));
            _destArray.Add(new HpdfReal(left));
        }

        /// <summary>
        /// Sets the destination to fit a specific rectangle in the window.
        /// Format: [page /FitR left bottom right top]
        /// </summary>
        public void SetFitR(float left, float bottom, float right, float top)
        {
            if (_destArray.Count > 1)
            {
                while (_destArray.Count > 1)
                    _destArray.RemoveAt(_destArray.Count - 1);
            }

            _destArray.Add(new HpdfName("FitR"));
            _destArray.Add(new HpdfReal(left));
            _destArray.Add(new HpdfReal(bottom));
            _destArray.Add(new HpdfReal(right));
            _destArray.Add(new HpdfReal(top));
        }

        /// <summary>
        /// Sets the destination to fit the bounding box of the page contents.
        /// Format: [page /FitB]
        /// </summary>
        public void SetFitB()
        {
            if (_destArray.Count > 1)
            {
                while (_destArray.Count > 1)
                    _destArray.RemoveAt(_destArray.Count - 1);
            }

            _destArray.Add(new HpdfName("FitB"));
        }

        /// <summary>
        /// Sets the destination to fit the bounding box width.
        /// Format: [page /FitBH top]
        /// </summary>
        public void SetFitBH(float top)
        {
            if (_destArray.Count > 1)
            {
                while (_destArray.Count > 1)
                    _destArray.RemoveAt(_destArray.Count - 1);
            }

            _destArray.Add(new HpdfName("FitBH"));
            _destArray.Add(new HpdfReal(top));
        }

        /// <summary>
        /// Sets the destination to fit the bounding box height.
        /// Format: [page /FitBV left]
        /// </summary>
        public void SetFitBV(float left)
        {
            if (_destArray.Count > 1)
            {
                while (_destArray.Count > 1)
                    _destArray.RemoveAt(_destArray.Count - 1);
            }

            _destArray.Add(new HpdfName("FitBV"));
            _destArray.Add(new HpdfReal(left));
        }
    }
}
