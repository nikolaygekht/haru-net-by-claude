namespace Haru.Types
{
    /// <summary>
    /// PDF version enumeration.
    /// </summary>
    public enum HpdfPdfVer
    {
        Ver12 = 0,
        Ver13,
        Ver14,
        Ver15,
        Ver16,
        Ver17,
        VerEof
    }

    /// <summary>
    /// PDF/A types.
    /// </summary>
    public enum HpdfPdfAType
    {
        PdfA1A = 0,
        PdfA1B = 1
    }

    /// <summary>
    /// Encryption mode.
    /// </summary>
    public enum HpdfEncryptMode
    {
        EncryptR2 = 2,
        EncryptR3 = 3
    }

    /// <summary>
    /// Color space enumeration.
    /// </summary>
    public enum HpdfColorSpace
    {
        DeviceGray = 0,
        DeviceRgb,
        DeviceCmyk,
        CalGray,
        CalRgb,
        Lab,
        IccBased,
        Separation,
        DeviceN,
        Indexed,
        Pattern,
        Eof
    }

    /// <summary>
    /// Line cap style.
    /// </summary>
    public enum HpdfLineCap
    {
        ButtEnd = 0,
        RoundEnd,
        ProjectingSquareEnd,
        LineCapEof
    }

    /// <summary>
    /// Line join style.
    /// </summary>
    public enum HpdfLineJoin
    {
        MiterJoin = 0,
        RoundJoin,
        BevelJoin,
        LineJoinEof
    }

    /// <summary>
    /// Text rendering mode.
    /// </summary>
    public enum HpdfTextRenderingMode
    {
        Fill = 0,
        Stroke,
        FillThenStroke,
        Invisible,
        FillClipping,
        StrokeClipping,
        FillStrokeClipping,
        Clipping,
        RenderingModeEof
    }

    /// <summary>
    /// Writing mode (horizontal or vertical).
    /// </summary>
    public enum HpdfWritingMode
    {
        Horizontal = 0,
        Vertical,
        Eof
    }

    /// <summary>
    /// Page layout.
    /// </summary>
    public enum HpdfPageLayout
    {
        Single = 0,
        OneColumn,
        TwoColumnLeft,
        TwoColumnRight,
        TwoPageLeft,
        TwoPageRight,
        Eof
    }

    /// <summary>
    /// Page mode.
    /// </summary>
    public enum HpdfPageMode
    {
        UseNone = 0,
        UseOutline,
        UseThumbs,
        FullScreen,
        Eof
    }

    /// <summary>
    /// Page number style.
    /// </summary>
    public enum HpdfPageNumStyle
    {
        Decimal = 0,
        UpperRoman,
        LowerRoman,
        UpperLetters,
        LowerLetters,
        Eof
    }

    /// <summary>
    /// Destination type for links and bookmarks.
    /// </summary>
    public enum HpdfDestinationType
    {
        Xyz = 0,
        Fit,
        FitH,
        FitV,
        FitR,
        FitB,
        FitBh,
        FitBv,
        DstEof
    }

    /// <summary>
    /// Text alignment.
    /// </summary>
    public enum HpdfTextAlignment
    {
        Left = 0,
        Right,
        Center,
        Justify
    }

    /// <summary>
    /// Annotation type.
    /// </summary>
    public enum HpdfAnnotType
    {
        TextNotes,
        Link,
        Sound,
        FreeText,
        Stamp,
        Square,
        Circle,
        StrikeOut,
        Highlight,
        Underline,
        Ink,
        FileAttachment,
        Popup,
        ThreeD,
        Squiggly,
        Line,
        Projection,
        Widget
    }

    /// <summary>
    /// Annotation flags.
    /// </summary>
    public enum HpdfAnnotFlags
    {
        Invisible,
        Hidden,
        Print,
        NoZoom,
        NoRotate,
        NoView,
        ReadOnly
    }

    /// <summary>
    /// Annotation highlight mode.
    /// </summary>
    public enum HpdfAnnotHighlightMode
    {
        NoHighlight = 0,
        InvertBox,
        InvertBorder,
        DownAppearance,
        HighlightModeEof
    }

    /// <summary>
    /// Annotation icon.
    /// </summary>
    public enum HpdfAnnotIcon
    {
        Comment = 0,
        Key,
        Note,
        Help,
        NewParagraph,
        Paragraph,
        Insert,
        Eof
    }

    /// <summary>
    /// Annotation intent.
    /// </summary>
    public enum HpdfAnnotIntent
    {
        FreeTextCallout = 0,
        FreeTextTypewriter,
        LineArrow,
        LineDimension,
        PolygonCloud,
        PolylineDimension,
        PolygonDimension
    }

    /// <summary>
    /// Line annotation ending style.
    /// </summary>
    public enum HpdfLineAnnotEndingStyle
    {
        None = 0,
        Square,
        Circle,
        Diamond,
        OpenArrow,
        ClosedArrow,
        Butt,
        ROpenArrow,
        RClosedArrow,
        Slash
    }

    /// <summary>
    /// Line annotation caption position.
    /// </summary>
    public enum HpdfLineAnnotCapPosition
    {
        Inline = 0,
        Top
    }

    /// <summary>
    /// Stamp annotation name.
    /// </summary>
    public enum HpdfStampAnnotName
    {
        Approved = 0,
        Experimental,
        NotApproved,
        AsIs,
        Expired,
        NotForPublicRelease,
        Confidential,
        Final,
        Sold,
        Departmental,
        ForComment,
        TopSecret,
        Draft,
        ForPublicRelease
    }

    /// <summary>
    /// Border style subtype.
    /// </summary>
    public enum HpdfBSSubtype
    {
        Solid,
        Dashed,
        Beveled,
        Inset,
        Underlined
    }

    /// <summary>
    /// Blend mode.
    /// </summary>
    public enum HpdfBlendMode
    {
        Normal,
        Multiply,
        Screen,
        Overlay,
        Darken,
        Lighten,
        ColorDodge,
        ColorBurn,
        HardLight,
        SoftLight,
        Difference,
        Exclusion,
        Eof
    }

    /// <summary>
    /// Transition style for slide shows.
    /// </summary>
    public enum HpdfTransitionStyle
    {
        WipeRight = 0,
        WipeUp,
        WipeLeft,
        WipeDown,
        BarnDoorsHorizontalOut,
        BarnDoorsHorizontalIn,
        BarnDoorsVerticalOut,
        BarnDoorsVerticalIn,
        BoxOut,
        BoxIn,
        BlindsHorizontal,
        BlindsVertical,
        Dissolve,
        GlitterRight,
        GlitterDown,
        GlitterTopLeftToBottomRight,
        Replace,
        Eof
    }

    /// <summary>
    /// Standard page sizes.
    /// </summary>
    public enum HpdfPageSizes
    {
        Letter = 0,
        Legal,
        A3,
        A4,
        A5,
        B4,
        B5,
        Executive,
        Us4x6,
        Us4x8,
        Us5x7,
        Comm10,
        Eof
    }

    /// <summary>
    /// Page direction (orientation).
    /// </summary>
    public enum HpdfPageDirection
    {
        Portrait = 0,
        Landscape
    }

    /// <summary>
    /// Encoder type.
    /// </summary>
    public enum HpdfEncoderType
    {
        SingleByte,
        DoubleByte,
        Uninitialized,
        Unknown
    }

    /// <summary>
    /// Byte type classification.
    /// </summary>
    public enum HpdfByteType
    {
        Single = 0,
        Lead,
        Trial,
        Unknown
    }

    /// <summary>
    /// Info type for document metadata.
    /// </summary>
    public enum HpdfInfoType
    {
        CreationDate = 0,
        ModDate,
        Author,
        Creator,
        Producer,
        Title,
        Subject,
        Keywords,
        Trapped,
        GtsPdfx,
        Eof
    }

    /// <summary>
    /// Name dictionary keys.
    /// </summary>
    public enum HpdfNameDictKey
    {
        EmbeddedFiles = 0,
        Eof
    }
}
