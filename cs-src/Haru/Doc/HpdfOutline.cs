/*
 * << Haru Free PDF Library >> -- HpdfOutline.cs
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
using Haru.Xref;

namespace Haru.Doc
{
    /// <summary>
    /// Represents a PDF outline (bookmark) entry in the document.
    /// Outlines allow navigation within a PDF document.
    /// </summary>
    public class HpdfOutline
    {
        private readonly HpdfDict _dict;
        private readonly HpdfXref _xref;
        private bool _opened;

        /// <summary>
        /// Gets the underlying dictionary object.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets or sets whether the outline is opened (children visible) by default.
        /// </summary>
        public bool Opened
        {
            get => _opened;
            set => _opened = value;
        }

        /// <summary>
        /// Creates the root outline object for a document.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        internal HpdfOutline(HpdfXref xref)
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            _dict = new HpdfDict();
            _opened = true;

            // Add to xref
            _xref.Add(_dict);

            _dict.Add("Type", new HpdfName("Outlines"));
        }

        /// <summary>
        /// Creates a new outline entry as a child of this outline.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="parent">The parent outline.</param>
        /// <param name="title">The title of the outline entry.</param>
        internal HpdfOutline(HpdfXref xref, HpdfOutline parent, string title)
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            if (parent is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Parent cannot be null");
            if (string.IsNullOrEmpty(title))
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Title cannot be null or empty");

            _dict = new HpdfDict();
            _opened = true;

            // Add to xref
            _xref.Add(_dict);

            _dict.Add("Title", new HpdfString(title));

            // Add to parent's children
            AddToParent(parent);
        }

        /// <summary>
        /// Creates a child outline entry.
        /// </summary>
        /// <param name="title">The title of the child outline.</param>
        /// <returns>The newly created child outline.</returns>
        public HpdfOutline CreateChild(string title)
        {
            return new HpdfOutline(_xref, this, title);
        }

        /// <summary>
        /// Sets whether the outline is opened (children visible) by default.
        /// </summary>
        /// <param name="opened">True to show children by default, false to hide them.</param>
        public void SetOpened(bool opened)
        {
            _opened = opened;
        }

        /// <summary>
        /// Sets the destination for this outline (where it navigates to when clicked).
        /// </summary>
        /// <param name="destination">The destination array.</param>
        public void SetDestination(HpdfArray destination)
        {
            if (destination is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Destination cannot be null");

            _dict.Add("Dest", destination);
        }

        /// <summary>
        /// Sets the destination for this outline (where it navigates to when clicked).
        /// </summary>
        /// <param name="destination">The destination object.</param>
        public void SetDestination(HpdfDestination destination)
        {
            if (destination is null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Destination cannot be null");

            _dict.Add("Dest", destination.DestArray);
        }

        /// <summary>
        /// Adds this outline as a child of the specified parent.
        /// </summary>
        /// <param name="parent">The parent outline.</param>
        private void AddToParent(HpdfOutline parent)
        {
            // Get first and last children of parent
            HpdfDict? firstChild = null;
            HpdfDict? lastChild = null;

            if (parent._dict.TryGetValue("First", out var firstObj))
                firstChild = firstObj as HpdfDict;

            if (parent._dict.TryGetValue("Last", out var lastObj))
                lastChild = lastObj as HpdfDict;

            // If no first child, this becomes the first
            if (firstChild is null)
                parent._dict.Add("First", _dict);

            // If there's a last child, link them
            if (lastChild != null)
            {
                lastChild.Add("Next", _dict);
                _dict.Add("Prev", lastChild);
            }

            // This becomes the last child (replace if it exists)
            parent._dict["Last"] = _dict;

            // Set parent reference
            _dict.Add("Parent", parent._dict);
        }

        /// <summary>
        /// Prepares the outline for writing by calculating the Count field.
        /// </summary>
        internal void BeforeWrite()
        {
            int count = CountChildren();

            if (count > 0)
            {
                // If opened, count is positive; if closed, count is negative
                int signedCount = _opened ? count : -count;
                _dict.Add("Count", new HpdfNumber(signedCount));
            }
        }

        /// <summary>
        /// Counts the number of visible descendants.
        /// </summary>
        /// <returns>The count of visible descendants.</returns>
        private int CountChildren()
        {
            int count = 0;

            if (_dict.TryGetValue("First", out var firstObj) && firstObj is HpdfDict firstChild)
            {
                var current = firstChild;

                while (current != null)
                {
                    count++; // Count this child

                    // If this child is opened and has children, count them recursively
                    if (current.TryGetValue("First", out var _))
                    {
                        // Create temporary outline to count its children
                        var tempOutline = new HpdfOutline(current, _xref);
                        int childCount = tempOutline.CountChildren();

                        if (tempOutline._opened)
                            count += childCount;
                    }

                    // Move to next sibling
                    if (current.TryGetValue("Next", out var nextObj) && nextObj is HpdfDict next)
                        current = next;
                    else
                        break;
                }
            }

            return count;
        }

        /// <summary>
        /// Internal constructor for wrapping existing outline dictionaries.
        /// </summary>
        internal HpdfOutline(HpdfDict dict, HpdfXref xref)
        {
            _dict = dict;
            _xref = xref;
            _opened = true;
        }
    }
}
