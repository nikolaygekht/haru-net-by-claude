/*
 * << Haru Free PDF Library >> -- HpdfPage.cs
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
using Haru.Graphics;
using Haru.Types;
using Haru.Font;
using Haru.Annotations;
using Haru.Forms;
using Haru.Streams;
using System.Collections.Generic;

namespace Haru.Doc
{
    /// <summary>
    /// Represents a single page in a PDF document.
    /// </summary>
    public class HpdfPage : IDrawable
    {
        private readonly HpdfDict _dict;
        private readonly HpdfXref _xref;
        private readonly HpdfStreamObject _contents;
        private HpdfGraphicsState _graphicsState;
        private HpdfPoint _currentPos;
        private HpdfTransMatrix _textMatrix;
        private HpdfTransMatrix _textLineMatrix;
        private HpdfFont _currentFont;
        private readonly Dictionary<string, HpdfFont> _fontResources;
        private readonly Dictionary<string, HpdfExtGState> _extGStateResources;
        private readonly Dictionary<string, HpdfImage> _imageResources;

        /// <summary>
        /// Default page width in points (595.276 = A4 width).
        /// </summary>
        public const float DefaultWidth = 595.276f;

        /// <summary>
        /// Default page height in points (841.89 = A4 height).
        /// </summary>
        public const float DefaultHeight = 841.89f;

        /// <summary>
        /// Gets the underlying dictionary object for this page.
        /// </summary>
        public HpdfDict Dict => _dict;

        /// <summary>
        /// Gets the parent pages object.
        /// </summary>
        public HpdfPages Parent { get; internal set; }

        /// <summary>
        /// Gets the contents stream for this page.
        /// </summary>
        public HpdfStreamObject Contents => _contents;

        /// <summary>
        /// Gets the underlying stream to write PDF operators to.
        /// Implementation of IDrawable interface.
        /// </summary>
        public HpdfMemoryStream Stream => _contents.Stream;

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
        /// Gets or sets the current text matrix.
        /// </summary>
        public HpdfTransMatrix TextMatrix
        {
            get => _textMatrix;
            internal set => _textMatrix = value;
        }

        /// <summary>
        /// Gets or sets the current text line matrix.
        /// </summary>
        public HpdfTransMatrix TextLineMatrix
        {
            get => _textLineMatrix;
            internal set => _textLineMatrix = value;
        }

        /// <summary>
        /// Gets or sets the current font.
        /// </summary>
        public HpdfFont CurrentFont
        {
            get => _currentFont;
            internal set => _currentFont = value;
        }

        /// <summary>
        /// Gets or sets the page width in points.
        /// </summary>
        public float Width
        {
            get
            {
                var mediaBox = GetMediaBox();
                return mediaBox[2] - mediaBox[0];
            }
            set
            {
                var mediaBox = GetMediaBox();
                SetMediaBox(mediaBox[0], mediaBox[1], mediaBox[0] + value, mediaBox[3]);
            }
        }

        /// <summary>
        /// Gets or sets the page height in points.
        /// </summary>
        public float Height
        {
            get
            {
                var mediaBox = GetMediaBox();
                return mediaBox[3] - mediaBox[1];
            }
            set
            {
                var mediaBox = GetMediaBox();
                SetMediaBox(mediaBox[0], mediaBox[1], mediaBox[2], mediaBox[1] + value);
            }
        }

        /// <summary>
        /// Creates a new page with default size.
        /// </summary>
        /// <param name="xref">The cross-reference table.</param>
        public HpdfPage(HpdfXref xref)
        {
            _xref = xref ?? throw new HpdfException(HpdfErrorCode.InvalidParameter, "Xref cannot be null");

            _dict = new HpdfDict();
            _dict.Add("Type", new HpdfName("Page"));

            // Create default MediaBox (0, 0, 595.276, 841.89) - A4 size
            var mediaBox = new HpdfArray();
            mediaBox.Add(new HpdfNumber(0));
            mediaBox.Add(new HpdfNumber(0));
            mediaBox.Add(new HpdfReal(DefaultWidth));
            mediaBox.Add(new HpdfReal(DefaultHeight));
            _dict.Add("MediaBox", mediaBox);

            // Create contents stream
            _contents = new HpdfStreamObject();
            xref.Add(_contents);
            _dict.Add("Contents", _contents);

            // Create empty Resources dictionary
            var resources = new HpdfDict();
            _dict.Add("Resources", resources);

            // Initialize graphics state
            _graphicsState = new HpdfGraphicsState();
            _currentPos = new HpdfPoint(0, 0);

            // Initialize text state
            _textMatrix = new HpdfTransMatrix(1, 0, 0, 1, 0, 0);
            _textLineMatrix = new HpdfTransMatrix(1, 0, 0, 1, 0, 0);
            _fontResources = new Dictionary<string, HpdfFont>();
            _extGStateResources = new Dictionary<string, HpdfExtGState>();
            _imageResources = new Dictionary<string, HpdfImage>();

            // Add page to xref
            xref.Add(_dict);
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

        /// <summary>
        /// Gets the MediaBox array [llx, lly, urx, ury].
        /// </summary>
        /// <returns>Array of four floats representing the media box.</returns>
        private float[] GetMediaBox()
        {
            if (!_dict.TryGetValue("MediaBox", out var mediaBoxObjRaw))
                return new float[] { 0, 0, DefaultWidth, DefaultHeight };
            var mediaBoxObj = mediaBoxObjRaw as HpdfArray;
            if (mediaBoxObj == null || mediaBoxObj.Count != 4)
            {
                // Return default
                return new float[] { 0, 0, DefaultWidth, DefaultHeight };
            }

            var result = new float[4];
            for (int i = 0; i < 4; i++)
            {
                var item = mediaBoxObj[i];
                if (item is HpdfNumber num)
                    result[i] = num.Value;
                else if (item is HpdfReal real)
                    result[i] = real.Value;
                else
                    result[i] = 0;
            }
            return result;
        }

        /// <summary>
        /// Sets the MediaBox for this page.
        /// </summary>
        /// <param name="llx">Lower-left x coordinate.</param>
        /// <param name="lly">Lower-left y coordinate.</param>
        /// <param name="urx">Upper-right x coordinate.</param>
        /// <param name="ury">Upper-right y coordinate.</param>
        public void SetMediaBox(float llx, float lly, float urx, float ury)
        {
            var mediaBox = new HpdfArray();
            mediaBox.Add(new HpdfReal(llx));
            mediaBox.Add(new HpdfReal(lly));
            mediaBox.Add(new HpdfReal(urx));
            mediaBox.Add(new HpdfReal(ury));
            _dict["MediaBox"] = mediaBox;
        }

        /// <summary>
        /// Sets the page size using a predefined size and orientation.
        /// </summary>
        /// <param name="size">The page size.</param>
        /// <param name="direction">The page orientation.</param>
        public void SetSize(HpdfPageSize size, HpdfPageDirection direction)
        {
            HpdfPageSizes.GetSize(size, direction, out float width, out float height);
            SetMediaBox(0, 0, width, height);
        }

        /// <summary>
        /// Sets the page size using custom width and height.
        /// </summary>
        /// <param name="width">The page width in points.</param>
        /// <param name="height">The page height in points.</param>
        public void SetSize(float width, float height)
        {
            SetMediaBox(0, 0, width, height);
        }

        /// <summary>
        /// Gets the Resources dictionary for this page.
        /// </summary>
        /// <returns>The resources dictionary.</returns>
        public HpdfDict GetResources()
        {
            HpdfDict resources = null;
            if (_dict.TryGetValue("Resources", out var resourcesObj))
                resources = resourcesObj as HpdfDict;
            if (resources == null)
            {
                resources = new HpdfDict();
                _dict.Add("Resources", resources);
            }
            return resources;
        }

        /// <summary>
        /// Adds a font to the page resources.
        /// </summary>
        /// <param name="font">The font to add.</param>
        internal void AddFontResource(HpdfFont font)
        {
            if (font == null)
                return;

            // Check if already added
            if (_fontResources.ContainsKey(font.LocalName))
                return;

            _fontResources[font.LocalName] = font;

            // Add to Resources/Font dictionary
            var resources = GetResources();
            HpdfDict fontDict;
            if (resources.TryGetValue("Font", out var fontObj))
            {
                fontDict = fontObj as HpdfDict;
            }
            else
            {
                fontDict = new HpdfDict();
                resources["Font"] = fontDict;
            }

            fontDict[font.LocalName] = font.Dict;
        }

        /// <summary>
        /// Adds an extended graphics state resource to the page.
        /// </summary>
        /// <param name="extGState">The extended graphics state to add.</param>
        internal void AddExtGStateResource(HpdfExtGState extGState)
        {
            if (extGState == null)
                return;

            // Check if already added
            if (_extGStateResources.ContainsKey(extGState.LocalName))
                return;

            _extGStateResources[extGState.LocalName] = extGState;

            // Add to Resources/ExtGState dictionary
            var resources = GetResources();
            HpdfDict extGStateDict;
            if (resources.TryGetValue("ExtGState", out var extGStateObj))
            {
                extGStateDict = extGStateObj as HpdfDict;
            }
            else
            {
                extGStateDict = new HpdfDict();
                resources["ExtGState"] = extGStateDict;
            }

            extGStateDict[extGState.LocalName] = extGState.Dict;
        }

        /// <summary>
        /// Adds an image resource to the page.
        /// </summary>
        /// <param name="image">The image to add.</param>
        internal void AddImageResource(HpdfImage image)
        {
            if (image == null)
                return;

            // Check if already added
            if (_imageResources.ContainsKey(image.LocalName))
                return;

            _imageResources[image.LocalName] = image;

            // Add to Resources/XObject dictionary
            var resources = GetResources();
            HpdfDict xobjectDict;
            if (resources.TryGetValue("XObject", out var xobjectObj))
            {
                xobjectDict = xobjectObj as HpdfDict;
            }
            else
            {
                xobjectDict = new HpdfDict();
                resources["XObject"] = xobjectDict;
            }

            xobjectDict[image.LocalName] = image.Dict;
        }

        /// <summary>
        /// Creates a new text annotation (sticky note) on this page.
        /// </summary>
        /// <param name="rect">The annotation rectangle.</param>
        /// <param name="text">The text content of the annotation.</param>
        /// <param name="icon">The icon to display (default: Note).</param>
        /// <returns>The created text annotation.</returns>
        public HpdfTextAnnotation CreateTextAnnotation(HpdfRect rect, string text,
            HpdfAnnotationIcon icon = HpdfAnnotationIcon.Note)
        {
            var annot = new HpdfTextAnnotation(_xref, rect, text, icon);
            AddAnnotation(annot);
            return annot;
        }

        /// <summary>
        /// Creates a new URI link annotation on this page.
        /// </summary>
        /// <param name="rect">The annotation rectangle.</param>
        /// <param name="uri">The URI to link to.</param>
        /// <returns>The created link annotation.</returns>
        public HpdfLinkAnnotation CreateLinkAnnotation(HpdfRect rect, string uri)
        {
            var annot = new HpdfLinkAnnotation(_xref, rect, uri);
            AddAnnotation(annot);
            return annot;
        }

        /// <summary>
        /// Creates a new internal link annotation (GoTo) on this page.
        /// </summary>
        /// <param name="rect">The annotation rectangle.</param>
        /// <param name="destination">The destination array.</param>
        /// <returns>The created link annotation.</returns>
        public HpdfLinkAnnotation CreateLinkAnnotation(HpdfRect rect, HpdfArray destination)
        {
            var annot = new HpdfLinkAnnotation(_xref, rect, destination);
            AddAnnotation(annot);
            return annot;
        }

        /// <summary>
        /// Adds a widget annotation (form field) to this page.
        /// This method is used to place form field widgets on the page.
        /// </summary>
        /// <param name="widget">The widget annotation to add.</param>
        public void AddWidgetAnnotation(HpdfWidgetAnnotation widget)
        {
            if (widget == null)
                throw new HpdfException(HpdfErrorCode.InvalidParameter, "Widget cannot be null");

            // Set page reference in widget
            widget.SetPage(_dict);

            // Add to page's Annots array
            AddAnnotation(widget);
        }

        /// <summary>
        /// Adds an annotation to the page's Annots array.
        /// </summary>
        /// <param name="annot">The annotation to add.</param>
        private void AddAnnotation(HpdfAnnotation annot)
        {
            // Find or create the Annots array
            HpdfArray annotsArray;
            if (_dict.TryGetValue("Annots", out var annotsObj))
            {
                annotsArray = annotsObj as HpdfArray;
                if (annotsArray == null)
                    throw new HpdfException(HpdfErrorCode.InvalidObject, "Annots is not an array");
            }
            else
            {
                annotsArray = new HpdfArray();
                _dict.Add("Annots", annotsArray);
            }

            annotsArray.Add(annot.Dict);
        }

        /// <summary>
        /// Sets the CropBox for this page.
        /// The CropBox defines the region to which the contents of the page shall be clipped when displayed or printed.
        /// </summary>
        /// <param name="left">Lower-left x coordinate.</param>
        /// <param name="bottom">Lower-left y coordinate.</param>
        /// <param name="right">Upper-right x coordinate.</param>
        /// <param name="top">Upper-right y coordinate.</param>
        public void SetCropBox(float left, float bottom, float right, float top)
        {
            var cropBox = new HpdfArray();
            cropBox.Add(new HpdfReal(left));
            cropBox.Add(new HpdfReal(bottom));
            cropBox.Add(new HpdfReal(right));
            cropBox.Add(new HpdfReal(top));
            _dict["CropBox"] = cropBox;
        }

        /// <summary>
        /// Sets the CropBox for this page using an HpdfBox structure.
        /// </summary>
        /// <param name="box">The crop box.</param>
        public void SetCropBox(HpdfBox box)
        {
            SetCropBox(box.Left, box.Bottom, box.Right, box.Top);
        }

        /// <summary>
        /// Gets the CropBox for this page, if set.
        /// </summary>
        /// <returns>The crop box, or null if not set.</returns>
        public HpdfBox? GetCropBox()
        {
            if (!_dict.TryGetValue("CropBox", out var cropBoxObj))
                return null;

            if (cropBoxObj is HpdfArray arr && arr.Count == 4)
            {
                float[] values = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    if (arr[i] is HpdfNumber num)
                        values[i] = num.Value;
                    else if (arr[i] is HpdfReal real)
                        values[i] = real.Value;
                }
                return new HpdfBox(values[0], values[1], values[2], values[3]);
            }
            return null;
        }

        /// <summary>
        /// Sets the BleedBox for this page.
        /// The BleedBox defines the region to which the contents of the page shall be clipped
        /// when output in a production environment (e.g., for printing with bleed).
        /// </summary>
        /// <param name="left">Lower-left x coordinate.</param>
        /// <param name="bottom">Lower-left y coordinate.</param>
        /// <param name="right">Upper-right x coordinate.</param>
        /// <param name="top">Upper-right y coordinate.</param>
        public void SetBleedBox(float left, float bottom, float right, float top)
        {
            var bleedBox = new HpdfArray();
            bleedBox.Add(new HpdfReal(left));
            bleedBox.Add(new HpdfReal(bottom));
            bleedBox.Add(new HpdfReal(right));
            bleedBox.Add(new HpdfReal(top));
            _dict["BleedBox"] = bleedBox;
        }

        /// <summary>
        /// Sets the BleedBox for this page using an HpdfBox structure.
        /// </summary>
        /// <param name="box">The bleed box.</param>
        public void SetBleedBox(HpdfBox box)
        {
            SetBleedBox(box.Left, box.Bottom, box.Right, box.Top);
        }

        /// <summary>
        /// Gets the BleedBox for this page, if set.
        /// </summary>
        /// <returns>The bleed box, or null if not set.</returns>
        public HpdfBox? GetBleedBox()
        {
            if (!_dict.TryGetValue("BleedBox", out var bleedBoxObj))
                return null;

            if (bleedBoxObj is HpdfArray arr && arr.Count == 4)
            {
                float[] values = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    if (arr[i] is HpdfNumber num)
                        values[i] = num.Value;
                    else if (arr[i] is HpdfReal real)
                        values[i] = real.Value;
                }
                return new HpdfBox(values[0], values[1], values[2], values[3]);
            }
            return null;
        }

        /// <summary>
        /// Sets the TrimBox for this page.
        /// The TrimBox defines the intended dimensions of the finished page after trimming.
        /// </summary>
        /// <param name="left">Lower-left x coordinate.</param>
        /// <param name="bottom">Lower-left y coordinate.</param>
        /// <param name="right">Upper-right x coordinate.</param>
        /// <param name="top">Upper-right y coordinate.</param>
        public void SetTrimBox(float left, float bottom, float right, float top)
        {
            var trimBox = new HpdfArray();
            trimBox.Add(new HpdfReal(left));
            trimBox.Add(new HpdfReal(bottom));
            trimBox.Add(new HpdfReal(right));
            trimBox.Add(new HpdfReal(top));
            _dict["TrimBox"] = trimBox;
        }

        /// <summary>
        /// Sets the TrimBox for this page using an HpdfBox structure.
        /// </summary>
        /// <param name="box">The trim box.</param>
        public void SetTrimBox(HpdfBox box)
        {
            SetTrimBox(box.Left, box.Bottom, box.Right, box.Top);
        }

        /// <summary>
        /// Gets the TrimBox for this page, if set.
        /// </summary>
        /// <returns>The trim box, or null if not set.</returns>
        public HpdfBox? GetTrimBox()
        {
            if (!_dict.TryGetValue("TrimBox", out var trimBoxObj))
                return null;

            if (trimBoxObj is HpdfArray arr && arr.Count == 4)
            {
                float[] values = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    if (arr[i] is HpdfNumber num)
                        values[i] = num.Value;
                    else if (arr[i] is HpdfReal real)
                        values[i] = real.Value;
                }
                return new HpdfBox(values[0], values[1], values[2], values[3]);
            }
            return null;
        }

        /// <summary>
        /// Sets the ArtBox for this page.
        /// The ArtBox defines the extent of the page's meaningful content
        /// (including potential white space) as intended by the page's creator.
        /// </summary>
        /// <param name="left">Lower-left x coordinate.</param>
        /// <param name="bottom">Lower-left y coordinate.</param>
        /// <param name="right">Upper-right x coordinate.</param>
        /// <param name="top">Upper-right y coordinate.</param>
        public void SetArtBox(float left, float bottom, float right, float top)
        {
            var artBox = new HpdfArray();
            artBox.Add(new HpdfReal(left));
            artBox.Add(new HpdfReal(bottom));
            artBox.Add(new HpdfReal(right));
            artBox.Add(new HpdfReal(top));
            _dict["ArtBox"] = artBox;
        }

        /// <summary>
        /// Sets the ArtBox for this page using an HpdfBox structure.
        /// </summary>
        /// <param name="box">The art box.</param>
        public void SetArtBox(HpdfBox box)
        {
            SetArtBox(box.Left, box.Bottom, box.Right, box.Top);
        }

        /// <summary>
        /// Gets the ArtBox for this page, if set.
        /// </summary>
        /// <returns>The art box, or null if not set.</returns>
        public HpdfBox? GetArtBox()
        {
            if (!_dict.TryGetValue("ArtBox", out var artBoxObj))
                return null;

            if (artBoxObj is HpdfArray arr && arr.Count == 4)
            {
                float[] values = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    if (arr[i] is HpdfNumber num)
                        values[i] = num.Value;
                    else if (arr[i] is HpdfReal real)
                        values[i] = real.Value;
                }
                return new HpdfBox(values[0], values[1], values[2], values[3]);
            }
            return null;
        }

        /// <summary>
        /// Sets a transition effect (slide show) for this page.
        /// </summary>
        /// <param name="type">The transition style.</param>
        /// <param name="displayTime">Display time in seconds before transitioning.</param>
        /// <param name="transTime">Transition duration in seconds.</param>
        public void SetSlideShow(HpdfTransitionStyle type, float displayTime, float transTime)
        {
            // Create Trans dictionary for page transition
            var transDict = new HpdfDict();
            transDict.Add("Type", new HpdfName("Trans"));

            // Map transition style to PDF name
            string styleName = type switch
            {
                HpdfTransitionStyle.WipeRight => "Wipe",
                HpdfTransitionStyle.WipeUp => "Wipe",
                HpdfTransitionStyle.WipeLeft => "Wipe",
                HpdfTransitionStyle.WipeDown => "Wipe",
                HpdfTransitionStyle.BarnDoorsHorizontalOut => "Split",
                HpdfTransitionStyle.BarnDoorsHorizontalIn => "Split",
                HpdfTransitionStyle.BarnDoorsVerticalOut => "Split",
                HpdfTransitionStyle.BarnDoorsVerticalIn => "Split",
                HpdfTransitionStyle.BoxOut => "Box",
                HpdfTransitionStyle.BoxIn => "Box",
                HpdfTransitionStyle.BlindsHorizontal => "Blinds",
                HpdfTransitionStyle.BlindsVertical => "Blinds",
                HpdfTransitionStyle.Dissolve => "Dissolve",
                HpdfTransitionStyle.GlitterRight => "Glitter",
                HpdfTransitionStyle.GlitterDown => "Glitter",
                HpdfTransitionStyle.GlitterTopLeftToBottomRight => "Glitter",
                HpdfTransitionStyle.Replace => "R",
                _ => "R"
            };

            transDict.Add("S", new HpdfName(styleName));
            transDict.Add("D", new HpdfReal(transTime));

            // Add direction/dimension based on transition type
            if (type == HpdfTransitionStyle.WipeRight)
                transDict.Add("Di", new HpdfNumber(0));
            else if (type == HpdfTransitionStyle.WipeDown)
                transDict.Add("Di", new HpdfNumber(270));
            else if (type == HpdfTransitionStyle.WipeLeft)
                transDict.Add("Di", new HpdfNumber(180));
            else if (type == HpdfTransitionStyle.WipeUp)
                transDict.Add("Di", new HpdfNumber(90));
            else if (type == HpdfTransitionStyle.BarnDoorsHorizontalOut)
            {
                transDict.Add("Dm", new HpdfName("H"));
                transDict.Add("M", new HpdfName("O"));
            }
            else if (type == HpdfTransitionStyle.BarnDoorsHorizontalIn)
            {
                transDict.Add("Dm", new HpdfName("H"));
                transDict.Add("M", new HpdfName("I"));
            }
            else if (type == HpdfTransitionStyle.BarnDoorsVerticalOut)
            {
                transDict.Add("Dm", new HpdfName("V"));
                transDict.Add("M", new HpdfName("O"));
            }
            else if (type == HpdfTransitionStyle.BarnDoorsVerticalIn)
            {
                transDict.Add("Dm", new HpdfName("V"));
                transDict.Add("M", new HpdfName("I"));
            }
            else if (type == HpdfTransitionStyle.BoxOut)
                transDict.Add("M", new HpdfName("O"));
            else if (type == HpdfTransitionStyle.BoxIn)
                transDict.Add("M", new HpdfName("I"));
            else if (type == HpdfTransitionStyle.BlindsHorizontal)
                transDict.Add("Dm", new HpdfName("H"));
            else if (type == HpdfTransitionStyle.BlindsVertical)
                transDict.Add("Dm", new HpdfName("V"));
            else if (type == HpdfTransitionStyle.GlitterRight)
                transDict.Add("Di", new HpdfNumber(0));
            else if (type == HpdfTransitionStyle.GlitterDown)
                transDict.Add("Di", new HpdfNumber(270));
            else if (type == HpdfTransitionStyle.GlitterTopLeftToBottomRight)
                transDict.Add("Di", new HpdfNumber(315));

            _dict.Add("Trans", transDict);

            // Set page duration (display time)
            if (displayTime > 0)
                _dict.Add("Dur", new HpdfReal(displayTime));
        }
    }
}
