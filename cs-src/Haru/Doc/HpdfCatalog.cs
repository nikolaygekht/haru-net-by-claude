/*
 * << Haru Free PDF Library >> -- HpdfCatalog.cs
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

namespace Haru.Doc
{
    /// <summary>
    /// Represents the document catalog (root object) of a PDF document.
    /// The catalog is the entry point for accessing the document's page tree and other document-level structures.
    /// </summary>
    public class HpdfCatalog
    {
        private readonly HpdfDict _dict;
        private readonly HpdfXref _xref;

        /// <summary>
        /// Gets the underlying dictionary object for this catalog.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets or sets the page layout to be used when the document is opened.
        /// </summary>
        public HpdfPageLayout PageLayout
        {
            get
            {
                if (!_dict.TryGetValue("PageLayout", out var layoutObj))
                return HpdfPageLayout.SinglePage;
            var layoutName = layoutObj as HpdfName;
                if (layoutName == null)
                    return HpdfPageLayout.SinglePage;

                return layoutName.Value switch
                {
                    "SinglePage" => HpdfPageLayout.SinglePage,
                    "OneColumn" => HpdfPageLayout.OneColumn,
                    "TwoColumnLeft" => HpdfPageLayout.TwoColumnLeft,
                    "TwoColumnRight" => HpdfPageLayout.TwoColumnRight,
                    "TwoPageLeft" => HpdfPageLayout.TwoPageLeft,
                    "TwoPageRight" => HpdfPageLayout.TwoPageRight,
                    _ => HpdfPageLayout.SinglePage
                };
            }
            set
            {
                var layoutName = value switch
                {
                    HpdfPageLayout.SinglePage => "SinglePage",
                    HpdfPageLayout.OneColumn => "OneColumn",
                    HpdfPageLayout.TwoColumnLeft => "TwoColumnLeft",
                    HpdfPageLayout.TwoColumnRight => "TwoColumnRight",
                    HpdfPageLayout.TwoPageLeft => "TwoPageLeft",
                    HpdfPageLayout.TwoPageRight => "TwoPageRight",
                    _ => "SinglePage"
                };
                _dict["PageLayout"] = new HpdfName(layoutName);
            }
        }

        /// <summary>
        /// Gets or sets the page mode to be used when the document is opened.
        /// </summary>
        public HpdfPageMode PageMode
        {
            get
            {
                if (!_dict.TryGetValue("PageMode", out var modeObj))
                return HpdfPageMode.UseNone;
            var modeName = modeObj as HpdfName;
                if (modeName == null)
                    return HpdfPageMode.UseNone;

                return modeName.Value switch
                {
                    "UseNone" => HpdfPageMode.UseNone,
                    "UseOutlines" => HpdfPageMode.UseOutlines,
                    "UseThumbs" => HpdfPageMode.UseThumbs,
                    "FullScreen" => HpdfPageMode.FullScreen,
                    "UseOC" => HpdfPageMode.UseOC,
                    "UseAttachments" => HpdfPageMode.UseAttachments,
                    _ => HpdfPageMode.UseNone
                };
            }
            set
            {
                var modeName = value switch
                {
                    HpdfPageMode.UseNone => "UseNone",
                    HpdfPageMode.UseOutlines => "UseOutlines",
                    HpdfPageMode.UseThumbs => "UseThumbs",
                    HpdfPageMode.FullScreen => "FullScreen",
                    HpdfPageMode.UseOC => "UseOC",
                    HpdfPageMode.UseAttachments => "UseAttachments",
                    _ => "UseNone"
                };
                _dict["PageMode"] = new HpdfName(modeName);
            }
        }

        /// <summary>
        /// Sets the page mode (convenience method for API compatibility).
        /// </summary>
        /// <param name="mode">The page mode to set.</param>
        public void SetPageMode(HpdfPageMode mode)
        {
            PageMode = mode;
        }

        /// <summary>
        /// Creates a new catalog with a root pages object.
        /// </summary>
        /// <param name="xref">The cross-reference table to add the catalog to.</param>
        /// <param name="rootPages">The root pages object.</param>
        public HpdfCatalog(HpdfXref xref, HpdfPages rootPages)
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");
            if (rootPages == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Root pages cannot be null");

            _dict = new HpdfDict();
            _dict.Add("Type", new HpdfName("Catalog"));
            _dict.Add("Pages", rootPages.Dict);

            // Add catalog to xref
            xref.Add(_dict);
        }

        /// <summary>
        /// Gets the root pages object from the catalog.
        /// </summary>
        /// <returns>The root pages dictionary, or null if not found.</returns>
        public HpdfDict GetRootPages()
        {
            if (_dict.TryGetValue("Pages", out var pages))
                return pages as HpdfDict;
            return null;
        }

        /// <summary>
        /// Sets the names dictionary for the catalog.
        /// </summary>
        /// <param name="namesDict">The names dictionary.</param>
        public void SetNames(HpdfDict namesDict)
        {
            if (namesDict == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Names dictionary cannot be null");

            _dict.Add("Names", namesDict);
        }

        /// <summary>
        /// Gets the names dictionary from the catalog.
        /// </summary>
        /// <returns>The names dictionary, or null if not set.</returns>
        public HpdfDict GetNames()
        {
            if (_dict.TryGetValue("Names", out var names))
                return names as HpdfDict;
            return null;
        }

        /// <summary>
        /// Sets the page labels dictionary for the catalog.
        /// Page labels control how page numbers are displayed in PDF viewers.
        /// </summary>
        /// <param name="pageLabelsDict">The page labels dictionary containing the number tree.</param>
        public void SetPageLabels(HpdfDict pageLabelsDict)
        {
            if (pageLabelsDict == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "PageLabels dictionary cannot be null");

            _dict["PageLabels"] = pageLabelsDict;
        }

        /// <summary>
        /// Gets the page labels dictionary from the catalog.
        /// </summary>
        /// <returns>The page labels dictionary, or null if not set.</returns>
        public HpdfDict GetPageLabels()
        {
            if (_dict.TryGetValue("PageLabels", out var pageLabels))
                return pageLabels as HpdfDict;
            return null;
        }
    }
}
