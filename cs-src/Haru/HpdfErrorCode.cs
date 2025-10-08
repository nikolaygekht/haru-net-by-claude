namespace Haru
{
    /// <summary>
    /// Error codes for HPDF library operations
    /// </summary>
    public enum HpdfErrorCode : uint
    {
        // Array errors
        ArrayCountError = 0x1001,
        ArrayItemNotFound = 0x1002,
        ArrayItemUnexpectedType = 0x1003,

        // Binary errors
        BinaryLengthError = 0x1004,

        // Palette errors
        CannotGetPalette = 0x1005,

        // Dictionary errors
        DictCountError = 0x1007,
        DictItemNotFound = 0x1008,
        DictItemUnexpectedType = 0x1009,
        DictStreamLengthNotFound = 0x100A,

        // Document errors
        DocEncryptDictNotFound = 0x100B,
        DocInvalidObject = 0x100C,
        DuplicateRegistration = 0x100E,

        // JWW errors
        ExceedJwwCodeNumLimit = 0x100F,

        // Encryption errors
        EncryptInvalidPassword = 0x1011,

        // Class errors
        UnknownClass = 0x1013,

        // Graphics state errors
        ExceedGStateLimit = 0x1014,

        // Memory errors
        FailedToAllocMem = 0x1015,

        // File I/O errors
        FileIoError = 0x1016,
        FileOpenError = 0x1017,

        // Font errors
        FontExists = 0x1019,
        FontInvalidWidthsTable = 0x101A,
        InvalidAfmHeader = 0x101B,

        // Annotation errors
        InvalidAnnotation = 0x101C,

        // Image/Color errors
        InvalidBitPerComponent = 0x101E,
        InvalidCharMatricsData = 0x101F,
        InvalidColorSpace = 0x1020,
        InvalidCompressionMode = 0x1021,
        InvalidDateTime = 0x1022,

        // Destination errors
        InvalidDestination = 0x1023,

        // Document state errors
        InvalidDocument = 0x1025,
        InvalidDocumentState = 0x1026,

        // Encoder errors
        InvalidEncoder = 0x1027,
        InvalidEncoderType = 0x1028,
        InvalidEncodingName = 0x102B,
        InvalidEncryptKeyLen = 0x102C,

        // Font definition errors
        InvalidFontDefData = 0x102D,
        InvalidFontDefType = 0x102E,
        InvalidFontName = 0x102F,
        InvalidFont = 0x1075,

        // Image errors
        InvalidImage = 0x1030,
        InvalidJpegData = 0x1031,
        InvalidPngImage = 0x103B,

        // Object errors
        InvalidNData = 0x1032,
        InvalidObject = 0x1033,
        InvalidObjId = 0x1034,
        InvalidOperation = 0x1035,
        InvalidOutline = 0x1036,

        // Page errors
        InvalidPage = 0x1037,
        InvalidPages = 0x1038,
        InvalidParameter = 0x1039,
        InvalidStream = 0x103C,

        // File name errors
        MissingFileNameEntry = 0x103D,

        // TrueType errors
        InvalidTtcFile = 0x103F,
        InvalidTtcIndex = 0x1040,
        InvalidWxData = 0x1041,

        // Item errors
        ItemNotFound = 0x1042,

        // PNG errors
        LibPngError = 0x1043,

        // Name errors
        NameInvalidValue = 0x1044,
        NameOutOfRange = 0x1045,

        // Page errors (continued)
        PageInvalidParamCount = 0x1048,
        PagesMissingKidsEntry = 0x1049,
        PageCannotFindObject = 0x104A,
        PageCannotGetRootPages = 0x104B,
        PageCannotRestoreGState = 0x104C,
        PageCannotSetParent = 0x104D,
        PageFontNotFound = 0x104E,
        PageInvalidFont = 0x104F,
        PageInvalidFontSize = 0x1050,
        PageInvalidGMode = 0x1051,
        PageInvalidIndex = 0x1052,
        PageInvalidRotateValue = 0x1053,
        PageInvalidSize = 0x1054,
        PageInvalidXObject = 0x1055,
        PageOutOfRange = 0x1056,
        PageInsufficientSpace = 0x1076,
        PageInvalidDisplayTime = 0x1077,
        PageInvalidTransitionTime = 0x1078,
        InvalidPageSlideshowType = 0x1079,

        // Real number errors
        RealOutOfRange = 0x1057,

        // Stream errors
        StreamEof = 0x1058,
        StreamReadLnContinue = 0x1059,

        // String errors
        StringOutOfRange = 0x105B,

        // Function errors
        ThisFuncWasSkipped = 0x105C,

        // TrueType font errors
        TtfCannotEmbeddingFont = 0x105D,
        TtfInvalidCmap = 0x105E,
        TtfInvalidFormat = 0x105F,
        TtfMissingTable = 0x1060,

        // Unsupported features
        UnsupportedFontType = 0x1061,
        UnsupportedFunc = 0x1062,
        UnsupportedJpegFormat = 0x1063,
        UnsupportedType1Font = 0x1064,

        // XRef errors
        XRefCountError = 0x1065,

        // Compression errors
        ZLibError = 0x1066,

        // Additional page errors
        InvalidPageIndex = 0x1067,
        InvalidUri = 0x1068,
        PageLayoutOutOfRange = 0x1069,
        PageModeOutOfRange = 0x1070,
        PageNumStyleOutOfRange = 0x1071,
        PageInvalidDirection = 0x1074,

        // Annotation errors (continued)
        AnnotInvalidIcon = 0x1072,
        AnnotInvalidBorderStyle = 0x1073,

        // Graphics state errors (continued)
        ExtGStateOutOfRange = 0x1080,
        InvalidExtGState = 0x1081,
        ExtGStateReadOnly = 0x1082,

        // 3D and ICC errors
        InvalidU3dData = 0x1083,
        NameCannotGetNames = 0x1084,
        InvalidIccComponentNum = 0x1085
    }
}
