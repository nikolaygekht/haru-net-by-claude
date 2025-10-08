/*
 * << Haru Free PDF Library >> -- HpdfPages.cs
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

using System.Collections.Generic;
using Haru.Objects;
using Haru.Xref;

namespace Haru.Doc
{
    /// <summary>
    /// Represents a page tree node in a PDF document.
    /// The page tree is a hierarchical structure that organizes all pages in the document.
    /// </summary>
    public class HpdfPages
    {
        private readonly HpdfDict _dict;
        private readonly HpdfXref _xref;
        private readonly HpdfArray _kids;
        private readonly List<object> _children; // Can contain HpdfPages or HpdfPage

        /// <summary>
        /// Gets the underlying dictionary object for this pages object.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets the parent pages object, or null if this is the root.
        /// </summary>
        public HpdfPages Parent { get; private set; }

        /// <summary>
        /// Gets the number of pages in this node and all descendants.
        /// </summary>
        public int Count
        {
            get
            {
                if (_dict.TryGetValue("Count", out var countObj) && countObj is HpdfNumber num)
                    return num.Value;
                return 0;
            }
        }

        /// <summary>
        /// Creates a new pages object.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        /// <param name="parent">The parent pages object, or null for the root.</param>
        public HpdfPages(HpdfXref xref, HpdfPages parent = null)
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            Parent = parent;
            _children = new List<object>();

            _dict = new HpdfDict();
            _dict.Add("Type", new HpdfName("Pages"));

            _kids = new HpdfArray();
            _dict.Add("Kids", _kids);
            _dict.Add("Count", new HpdfNumber(0));

            // Add to xref
            xref.Add(_dict);

            // If this has a parent, add to parent's kids
            if (parent != null)
            {
                parent.AddKid(this);
            }
        }

        /// <summary>
        /// Adds a child pages node or page to this pages node.
        /// </summary>
        /// <param name="kid">The child to add (HpdfPages or HpdfPage).</param>
        public void AddKid(object kid)
        {
            if (kid == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Kid cannot be null");

            HpdfDict kidDict = null;
            int pageCount = 0;

            if (kid is HpdfPages pages)
            {
                // Check if kid already has a parent
                if (pages.Parent != null && pages.Parent != this)
                    throw new HpdfException(HpdfErrorCode.PageCannotSetParent, "Kid already has a parent");

                kidDict = pages.Dict;
                pageCount = pages.Count;
                pages.Parent = this;
            }
            else if (kid is HpdfPage page)
            {
                // Check if kid already has a parent
                if (page.Parent != null && page.Parent != this)
                    throw new HpdfException(HpdfErrorCode.PageCannotSetParent, "Kid already has a parent");

                kidDict = page.Dict;
                pageCount = 1;
                page.Parent = this;
            }
            else
            {
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Kid must be HpdfPages or HpdfPage");
            }

            // Set parent reference in kid
            kidDict.Add("Parent", _dict);

            // Add to kids array
            _kids.Add(kidDict);
            _children.Add(kid);

            // Update count recursively up the tree
            UpdateCount(pageCount);
        }

        /// <summary>
        /// Updates the page count for this node and all ancestors.
        /// </summary>
        /// <param name="delta">The change in page count (positive or negative).</param>
        private void UpdateCount(int delta)
        {
            var currentCount = Count;
            _dict["Count"] = new HpdfNumber(currentCount + delta);

            // Propagate to parent
            Parent?.UpdateCount(delta);
        }

        /// <summary>
        /// Gets all child pages and page nodes.
        /// </summary>
        /// <returns>A list of children (HpdfPages or HpdfPage objects).</returns>
        public IReadOnlyList<object> GetChildren()
        {
            return _children.AsReadOnly();
        }

        /// <summary>
        /// Recursively collects all pages in this page tree.
        /// </summary>
        /// <returns>A list of all HpdfPage objects in this tree.</returns>
        public List<HpdfPage> GetAllPages()
        {
            var pages = new List<HpdfPage>();
            CollectPages(pages);
            return pages;
        }

        private void CollectPages(List<HpdfPage> pages)
        {
            foreach (var child in _children)
            {
                if (child is HpdfPage page)
                {
                    pages.Add(page);
                }
                else if (child is HpdfPages pagesNode)
                {
                    pagesNode.CollectPages(pages);
                }
            }
        }
    }
}
