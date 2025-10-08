namespace Haru.Types
{
    /// <summary>
    /// Constants and default values used in Haru PDF library.
    /// </summary>
    public static class HpdfConst
    {
        // Status codes
        public const int Ok = 0;
        public const int NoError = 0;

        // Buffer sizes
        public const int TmpBufSize = 512;
        public const int ShortBufSize = 32;
        public const int RealLen = 11;
        public const int IntLen = 11;
        public const int TextDefaultLen = 256;
        public const int UnicodeHeaderLen = 2;
        public const int DateTimeStrLen = 23;

        // PDF item lengths
        public const int ByteOffsetLen = 10;
        public const int ObjIdLen = 7;
        public const int GenNoLen = 5;

        // Default graphic state values
        public const string DefFont = "Helvetica";
        public const HpdfPageLayout DefPageLayout = HpdfPageLayout.Single;
        public const HpdfPageMode DefPageMode = HpdfPageMode.UseNone;
        public const float DefWordSpace = 0;
        public const float DefCharSpace = 0;
        public const float DefFontSize = 10;
        public const float DefHScaling = 100;
        public const float DefLeading = 0;
        public const HpdfTextRenderingMode DefRenderingMode = HpdfTextRenderingMode.Fill;
        public const float DefRise = 0;
        public const float DefLineWidth = 1;
        public const HpdfLineCap DefLineCap = HpdfLineCap.ButtEnd;
        public const HpdfLineJoin DefLineJoin = HpdfLineJoin.MiterJoin;
        public const float DefMiterLimit = 10;
        public const float DefFlatness = 1;
        public const int DefPageNum = 1;
        public const float BsDefWidth = 1;

        // Default page size (A4)
        public const float DefPageWidth = 595.276f;
        public const float DefPageHeight = 841.89f;

        // Compression mode flags
        public const byte CompNone = 0x00;
        public const byte CompText = 0x01;
        public const byte CompImage = 0x02;
        public const byte CompMetadata = 0x04;
        public const byte CompAll = 0x0F;
        public const byte CompMask = 0xFF;

        // Permission flags
        public const int EnableRead = 0;
        public const int EnablePrint = 4;
        public const int EnableEditAll = 8;
        public const int EnableCopy = 16;
        public const int EnableEdit = 32;

        // Viewer preferences
        public const int HideToolbar = 1;
        public const int HideMenubar = 2;
        public const int HideWindowUi = 4;
        public const int FitWindow = 8;
        public const int CenterWindow = 16;
        public const int PrintScalingNone = 32;

        // Object limitations (PDF 1.4+)
        public const int LimitMaxInt = 2147483647;
        public const int LimitMinInt = -2147483647;
        public const float LimitMaxReal = 3.4E38f;
        public const float LimitMinReal = -3.4E38f;
        public const int LimitMaxStringLen = 2147483646;
        public const int LimitMaxNameLen = 127;
        public const int LimitMaxArray = 8388607;
        public const int LimitMaxDictElement = 8388607;
        public const int LimitMaxXrefElement = 8388607;
        public const int LimitMaxGState = 28;
        public const int LimitMaxDeviceN = 8;
        public const int LimitMaxDeviceNV15 = 32;
        public const int LimitMaxCid = 65535;
        public const int MaxGenerationNum = 65535;

        // Page size limits
        public const float MinPageHeight = 3;
        public const float MinPageWidth = 3;
        public const float MaxPageHeight = 14400;
        public const float MaxPageWidth = 14400;
        public const float MinMagnificationFactor = 8;
        public const float MaxMagnificationFactor = 3200;

        // Property value limits
        public const float MinPageSize = 3;
        public const float MaxPageSize = 14400;
        public const float MinHorizontalScaling = 10;
        public const float MaxHorizontalScaling = 300;
        public const float MinWordSpace = -30;
        public const float MaxWordSpace = 300;
        public const float MinCharSpace = -30;
        public const float MaxCharSpace = 300;
        public const float MaxFontSize = 600;
        public const float MaxZoomSize = 10;
        public const float MaxLeading = 300;
        public const float MaxLineWidth = 100;
        public const float MaxDashPattern = 100;
        public const int MaxJwwNum = 128;

        // Graphics mode flags
        public const int GmodePageDescription = 0x0001;
        public const int GmodePathObject = 0x0002;
        public const int GmodeTextObject = 0x0004;
        public const int GmodeClippingPath = 0x0008;
        public const int GmodeShading = 0x0010;
        public const int GmodeInlineImage = 0x0020;
        public const int GmodeExternalObject = 0x0040;
    }

    /// <summary>
    /// ISO 3166-1 country codes.
    /// </summary>
    public static class HpdfCountry
    {
        public const string Us = "US";   // United States
        public const string Gb = "GB";   // United Kingdom
        public const string Ca = "CA";   // Canada
        public const string Au = "AU";   // Australia
        public const string De = "DE";   // Germany
        public const string Fr = "FR";   // France
        public const string It = "IT";   // Italy
        public const string Es = "ES";   // Spain
        public const string Jp = "JP";   // Japan
        public const string Cn = "CN";   // China
        public const string In = "IN";   // India
        public const string Br = "BR";   // Brazil
        public const string Mx = "MX";   // Mexico
        public const string Ru = "RU";   // Russia
        public const string Kr = "KR";   // Korea (South)
        // Additional countries can be added as needed
    }

    /// <summary>
    /// ISO 639-1 language codes.
    /// </summary>
    public static class HpdfLang
    {
        public const string En = "en";   // English
        public const string De = "de";   // German
        public const string Fr = "fr";   // French
        public const string Es = "es";   // Spanish
        public const string It = "it";   // Italian
        public const string Ja = "ja";   // Japanese
        public const string Zh = "zh";   // Chinese
        public const string Pt = "pt";   // Portuguese
        public const string Ru = "ru";   // Russian
        public const string Ar = "ar";   // Arabic
        public const string Hi = "hi";   // Hindi
        public const string Ko = "ko";   // Korean
        public const string Nl = "nl";   // Dutch
        public const string Sv = "sv";   // Swedish
        public const string Pl = "pl";   // Polish
        // Additional languages can be added as needed
    }
}
