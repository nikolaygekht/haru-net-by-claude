using System;

namespace Haru
{
    /// <summary>
    /// Exception thrown by HPDF library operations
    /// </summary>
    public class HpdfException : Exception
    {
        /// <summary>
        /// Gets the primary error code
        /// </summary>
        public HpdfErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets the detail error code (additional context about the error)
        /// </summary>
        public uint DetailCode { get; }

        /// <summary>
        /// Creates a new HpdfException with the specified error code
        /// </summary>
        /// <param name="errorCode">The error code</param>
        public HpdfException(HpdfErrorCode errorCode)
            : this(errorCode, 0)
        {
        }

        /// <summary>
        /// Creates a new HpdfException with the specified error code and detail code
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <param name="detailCode">Additional detail about the error</param>
        public HpdfException(HpdfErrorCode errorCode, uint detailCode)
            : base(GetErrorMessage(errorCode, detailCode))
        {
            ErrorCode = errorCode;
            DetailCode = detailCode;
        }

        /// <summary>
        /// Creates a new HpdfException with the specified error code and custom message
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <param name="message">Custom error message</param>
        public HpdfException(HpdfErrorCode errorCode, string message)
            : this(errorCode, 0, message)
        {
        }

        /// <summary>
        /// Creates a new HpdfException with the specified error code, detail code, and custom message
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <param name="detailCode">Additional detail about the error</param>
        /// <param name="message">Custom error message</param>
        public HpdfException(HpdfErrorCode errorCode, uint detailCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
            DetailCode = detailCode;
        }

        /// <summary>
        /// Creates a new HpdfException with the specified error code and inner exception
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <param name="innerException">The inner exception</param>
        public HpdfException(HpdfErrorCode errorCode, Exception innerException)
            : this(errorCode, 0, GetErrorMessage(errorCode, 0), innerException)
        {
        }

        /// <summary>
        /// Creates a new HpdfException with the specified error code, detail code, and inner exception
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <param name="detailCode">Additional detail about the error</param>
        /// <param name="innerException">The inner exception</param>
        public HpdfException(HpdfErrorCode errorCode, uint detailCode, Exception innerException)
            : this(errorCode, detailCode, GetErrorMessage(errorCode, detailCode), innerException)
        {
        }

        /// <summary>
        /// Creates a new HpdfException with the specified error code, custom message, and inner exception
        /// </summary>
        /// <param name="errorCode">The error code</param>
        /// <param name="detailCode">Additional detail about the error</param>
        /// <param name="message">Custom error message</param>
        /// <param name="innerException">The inner exception</param>
        public HpdfException(HpdfErrorCode errorCode, uint detailCode, string message, Exception innerException)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            DetailCode = detailCode;
        }

        private static string GetErrorMessage(HpdfErrorCode errorCode, uint detailCode)
        {
            var message = errorCode switch
            {
                HpdfErrorCode.ArrayCountError => "Array count error",
                HpdfErrorCode.ArrayItemNotFound => "Array item not found",
                HpdfErrorCode.ArrayItemUnexpectedType => "Array item has unexpected type",
                HpdfErrorCode.BinaryLengthError => "Binary length error",
                HpdfErrorCode.CannotGetPalette => "Cannot get palette",
                HpdfErrorCode.DictCountError => "Dictionary count error",
                HpdfErrorCode.DictItemNotFound => "Dictionary item not found",
                HpdfErrorCode.DictItemUnexpectedType => "Dictionary item has unexpected type",
                HpdfErrorCode.DictStreamLengthNotFound => "Dictionary stream length not found",
                HpdfErrorCode.DocEncryptDictNotFound => "Document encryption dictionary not found",
                HpdfErrorCode.DocInvalidObject => "Document has invalid object",
                HpdfErrorCode.DuplicateRegistration => "Duplicate registration",
                HpdfErrorCode.ExceedJwwCodeNumLimit => "Exceeded JWW code number limit",
                HpdfErrorCode.EncryptInvalidPassword => "Invalid encryption password",
                HpdfErrorCode.UnknownClass => "Unknown class",
                HpdfErrorCode.ExceedGStateLimit => "Exceeded graphics state limit",
                HpdfErrorCode.FailedToAllocMem => "Failed to allocate memory",
                HpdfErrorCode.FileIoError => "File I/O error",
                HpdfErrorCode.FileOpenError => "File open error",
                HpdfErrorCode.FontExists => "Font already exists",
                HpdfErrorCode.FontInvalidWidthsTable => "Font has invalid widths table",
                HpdfErrorCode.InvalidAfmHeader => "Invalid AFM header",
                HpdfErrorCode.InvalidAnnotation => "Invalid annotation",
                HpdfErrorCode.InvalidBitPerComponent => "Invalid bits per component",
                HpdfErrorCode.InvalidCharMatricsData => "Invalid character metrics data",
                HpdfErrorCode.InvalidColorSpace => "Invalid color space",
                HpdfErrorCode.InvalidCompressionMode => "Invalid compression mode",
                HpdfErrorCode.InvalidDateTime => "Invalid date/time",
                HpdfErrorCode.InvalidDestination => "Invalid destination",
                HpdfErrorCode.InvalidDocument => "Invalid document",
                HpdfErrorCode.InvalidDocumentState => "Invalid document state",
                HpdfErrorCode.InvalidEncoder => "Invalid encoder",
                HpdfErrorCode.InvalidEncoderType => "Invalid encoder type",
                HpdfErrorCode.InvalidEncodingName => "Invalid encoding name",
                HpdfErrorCode.InvalidEncryptKeyLen => "Invalid encryption key length",
                HpdfErrorCode.InvalidFontDefData => "Invalid font definition data",
                HpdfErrorCode.InvalidFontDefType => "Invalid font definition type",
                HpdfErrorCode.InvalidFontName => "Invalid font name",
                HpdfErrorCode.InvalidFont => "Invalid font",
                HpdfErrorCode.InvalidImage => "Invalid image",
                HpdfErrorCode.InvalidJpegData => "Invalid JPEG data",
                HpdfErrorCode.InvalidPngImage => "Invalid PNG image",
                HpdfErrorCode.InvalidNData => "Invalid N data",
                HpdfErrorCode.InvalidObject => "Invalid object",
                HpdfErrorCode.InvalidObjId => "Invalid object ID",
                HpdfErrorCode.InvalidOperation => "Invalid operation",
                HpdfErrorCode.InvalidOutline => "Invalid outline",
                HpdfErrorCode.InvalidPage => "Invalid page",
                HpdfErrorCode.InvalidPages => "Invalid pages",
                HpdfErrorCode.InvalidParameter => "Invalid parameter",
                HpdfErrorCode.InvalidStream => "Invalid stream",
                HpdfErrorCode.MissingFileNameEntry => "Missing file name entry",
                HpdfErrorCode.InvalidTtcFile => "Invalid TTC file",
                HpdfErrorCode.InvalidTtcIndex => "Invalid TTC index",
                HpdfErrorCode.InvalidWxData => "Invalid WX data",
                HpdfErrorCode.ItemNotFound => "Item not found",
                HpdfErrorCode.LibPngError => "PNG library error",
                HpdfErrorCode.NameInvalidValue => "Name has invalid value",
                HpdfErrorCode.NameOutOfRange => "Name out of range",
                HpdfErrorCode.PageInvalidParamCount => "Page has invalid parameter count",
                HpdfErrorCode.PagesMissingKidsEntry => "Pages missing kids entry",
                HpdfErrorCode.PageCannotFindObject => "Page cannot find object",
                HpdfErrorCode.PageCannotGetRootPages => "Page cannot get root pages",
                HpdfErrorCode.PageCannotRestoreGState => "Page cannot restore graphics state",
                HpdfErrorCode.PageCannotSetParent => "Page cannot set parent",
                HpdfErrorCode.PageFontNotFound => "Page font not found",
                HpdfErrorCode.PageInvalidFont => "Page has invalid font",
                HpdfErrorCode.PageInvalidFontSize => "Page has invalid font size",
                HpdfErrorCode.PageInvalidGMode => "Page has invalid graphics mode",
                HpdfErrorCode.PageInvalidIndex => "Page has invalid index",
                HpdfErrorCode.PageInvalidRotateValue => "Page has invalid rotation value",
                HpdfErrorCode.PageInvalidSize => "Page has invalid size",
                HpdfErrorCode.PageInvalidXObject => "Page has invalid XObject",
                HpdfErrorCode.PageOutOfRange => "Page out of range",
                HpdfErrorCode.PageInsufficientSpace => "Page has insufficient space",
                HpdfErrorCode.PageInvalidDisplayTime => "Page has invalid display time",
                HpdfErrorCode.PageInvalidTransitionTime => "Page has invalid transition time",
                HpdfErrorCode.InvalidPageSlideshowType => "Invalid page slideshow type",
                HpdfErrorCode.RealOutOfRange => "Real number out of range",
                HpdfErrorCode.StreamEof => "Stream end of file",
                HpdfErrorCode.StreamReadLnContinue => "Stream read line continue",
                HpdfErrorCode.StringOutOfRange => "String out of range",
                HpdfErrorCode.ThisFuncWasSkipped => "This function was skipped",
                HpdfErrorCode.TtfCannotEmbeddingFont => "Cannot embed TrueType font",
                HpdfErrorCode.TtfInvalidCmap => "TrueType font has invalid CMAP",
                HpdfErrorCode.TtfInvalidFormat => "TrueType font has invalid format",
                HpdfErrorCode.TtfMissingTable => "TrueType font missing table",
                HpdfErrorCode.UnsupportedFontType => "Unsupported font type",
                HpdfErrorCode.UnsupportedFunc => "Unsupported function",
                HpdfErrorCode.UnsupportedJpegFormat => "Unsupported JPEG format",
                HpdfErrorCode.UnsupportedType1Font => "Unsupported Type1 font",
                HpdfErrorCode.XRefCountError => "XRef count error",
                HpdfErrorCode.ZLibError => "ZLib compression error",
                HpdfErrorCode.InvalidPageIndex => "Invalid page index",
                HpdfErrorCode.InvalidUri => "Invalid URI",
                HpdfErrorCode.PageLayoutOutOfRange => "Page layout out of range",
                HpdfErrorCode.PageModeOutOfRange => "Page mode out of range",
                HpdfErrorCode.PageNumStyleOutOfRange => "Page number style out of range",
                HpdfErrorCode.PageInvalidDirection => "Page has invalid direction",
                HpdfErrorCode.AnnotInvalidIcon => "Annotation has invalid icon",
                HpdfErrorCode.AnnotInvalidBorderStyle => "Annotation has invalid border style",
                HpdfErrorCode.ExtGStateOutOfRange => "Extended graphics state out of range",
                HpdfErrorCode.InvalidExtGState => "Invalid extended graphics state",
                HpdfErrorCode.ExtGStateReadOnly => "Extended graphics state is read-only",
                HpdfErrorCode.InvalidU3dData => "Invalid U3D data",
                HpdfErrorCode.NameCannotGetNames => "Name cannot get names",
                HpdfErrorCode.InvalidIccComponentNum => "Invalid ICC component number",
                _ => $"Unknown error code: 0x{(uint)errorCode:X4}"
            };

            if (detailCode != 0)
            {
                message += $" (detail: 0x{detailCode:X4})";
            }

            return message;
        }
    }
}
