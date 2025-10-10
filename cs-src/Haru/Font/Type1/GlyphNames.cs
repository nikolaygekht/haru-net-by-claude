/*
 * << Haru Free PDF Library >> -- GlyphNames.cs
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

namespace Haru.Font.Type1
{
    /// <summary>
    /// Maps PostScript glyph names to Unicode code points.
    /// This is a simplified implementation covering the most common glyphs.
    /// </summary>
    public static class GlyphNames
    {
        private static readonly Dictionary<string, ushort> _glyphToUnicode = new Dictionary<string, ushort>
        {
            // Basic Latin
            { ".notdef", 0x0000 },
            { "space", 0x0020 },
            { "exclam", 0x0021 },
            { "quotedbl", 0x0022 },
            { "numbersign", 0x0023 },
            { "dollar", 0x0024 },
            { "percent", 0x0025 },
            { "ampersand", 0x0026 },
            { "quotesingle", 0x0027 },
            { "parenleft", 0x0028 },
            { "parenright", 0x0029 },
            { "asterisk", 0x002A },
            { "plus", 0x002B },
            { "comma", 0x002C },
            { "hyphen", 0x002D },
            { "period", 0x002E },
            { "slash", 0x002F },
            { "zero", 0x0030 },
            { "one", 0x0031 },
            { "two", 0x0032 },
            { "three", 0x0033 },
            { "four", 0x0034 },
            { "five", 0x0035 },
            { "six", 0x0036 },
            { "seven", 0x0037 },
            { "eight", 0x0038 },
            { "nine", 0x0039 },
            { "colon", 0x003A },
            { "semicolon", 0x003B },
            { "less", 0x003C },
            { "equal", 0x003D },
            { "greater", 0x003E },
            { "question", 0x003F },
            { "at", 0x0040 },
            { "A", 0x0041 },
            { "B", 0x0042 },
            { "C", 0x0043 },
            { "D", 0x0044 },
            { "E", 0x0045 },
            { "F", 0x0046 },
            { "G", 0x0047 },
            { "H", 0x0048 },
            { "I", 0x0049 },
            { "J", 0x004A },
            { "K", 0x004B },
            { "L", 0x004C },
            { "M", 0x004D },
            { "N", 0x004E },
            { "O", 0x004F },
            { "P", 0x0050 },
            { "Q", 0x0051 },
            { "R", 0x0052 },
            { "S", 0x0053 },
            { "T", 0x0054 },
            { "U", 0x0055 },
            { "V", 0x0056 },
            { "W", 0x0057 },
            { "X", 0x0058 },
            { "Y", 0x0059 },
            { "Z", 0x005A },
            { "bracketleft", 0x005B },
            { "backslash", 0x005C },
            { "bracketright", 0x005D },
            { "asciicircum", 0x005E },
            { "underscore", 0x005F },
            { "grave", 0x0060 },
            { "a", 0x0061 },
            { "b", 0x0062 },
            { "c", 0x0063 },
            { "d", 0x0064 },
            { "e", 0x0065 },
            { "f", 0x0066 },
            { "g", 0x0067 },
            { "h", 0x0068 },
            { "i", 0x0069 },
            { "j", 0x006A },
            { "k", 0x006B },
            { "l", 0x006C },
            { "m", 0x006D },
            { "n", 0x006E },
            { "o", 0x006F },
            { "p", 0x0070 },
            { "q", 0x0071 },
            { "r", 0x0072 },
            { "s", 0x0073 },
            { "t", 0x0074 },
            { "u", 0x0075 },
            { "v", 0x0076 },
            { "w", 0x0077 },
            { "x", 0x0078 },
            { "y", 0x0079 },
            { "z", 0x007A },
            { "braceleft", 0x007B },
            { "bar", 0x007C },
            { "braceright", 0x007D },
            { "asciitilde", 0x007E },

            // Extended Latin
            { "exclamdown", 0x00A1 },
            { "cent", 0x00A2 },
            { "sterling", 0x00A3 },
            { "currency", 0x00A4 },
            { "yen", 0x00A5 },
            { "brokenbar", 0x00A6 },
            { "section", 0x00A7 },
            { "dieresis", 0x00A8 },
            { "copyright", 0x00A9 },
            { "ordfeminine", 0x00AA },
            { "guillemotleft", 0x00AB },
            { "guilsinglleft", 0x2039 },
            { "guilsinglright", 0x203A },
            { "logicalnot", 0x00AC },
            { "registered", 0x00AE },
            { "macron", 0x00AF },
            { "degree", 0x00B0 },
            { "plusminus", 0x00B1 },
            { "twosuperior", 0x00B2 },
            { "threesuperior", 0x00B3 },
            { "acute", 0x00B4 },
            { "mu", 0x00B5 },
            { "paragraph", 0x00B6 },
            { "periodcentered", 0x00B7 },
            { "cedilla", 0x00B8 },
            { "onesuperior", 0x00B9 },
            { "ordmasculine", 0x00BA },
            { "guillemotright", 0x00BB },
            { "onequarter", 0x00BC },
            { "onehalf", 0x00BD },
            { "threequarters", 0x00BE },
            { "questiondown", 0x00BF },
            { "Agrave", 0x00C0 },
            { "Aacute", 0x00C1 },
            { "Acircumflex", 0x00C2 },
            { "Atilde", 0x00C3 },
            { "Adieresis", 0x00C4 },
            { "Aring", 0x00C5 },
            { "AE", 0x00C6 },
            { "Ccedilla", 0x00C7 },
            { "Egrave", 0x00C8 },
            { "Eacute", 0x00C9 },
            { "Ecircumflex", 0x00CA },
            { "Edieresis", 0x00CB },
            { "Igrave", 0x00CC },
            { "Iacute", 0x00CD },
            { "Icircumflex", 0x00CE },
            { "Idieresis", 0x00CF },
            { "Eth", 0x00D0 },
            { "Ntilde", 0x00D1 },
            { "Ograve", 0x00D2 },
            { "Oacute", 0x00D3 },
            { "Ocircumflex", 0x00D4 },
            { "Otilde", 0x00D5 },
            { "Odieresis", 0x00D6 },
            { "multiply", 0x00D7 },
            { "Oslash", 0x00D8 },
            { "Ugrave", 0x00D9 },
            { "Uacute", 0x00DA },
            { "Ucircumflex", 0x00DB },
            { "Udieresis", 0x00DC },
            { "Yacute", 0x00DD },
            { "Thorn", 0x00DE },
            { "germandbls", 0x00DF },
            { "agrave", 0x00E0 },
            { "aacute", 0x00E1 },
            { "acircumflex", 0x00E2 },
            { "atilde", 0x00E3 },
            { "adieresis", 0x00E4 },
            { "aring", 0x00E5 },
            { "ae", 0x00E6 },
            { "ccedilla", 0x00E7 },
            { "egrave", 0x00E8 },
            { "eacute", 0x00E9 },
            { "ecircumflex", 0x00EA },
            { "edieresis", 0x00EB },
            { "igrave", 0x00EC },
            { "iacute", 0x00ED },
            { "icircumflex", 0x00EE },
            { "idieresis", 0x00EF },
            { "eth", 0x00F0 },
            { "ntilde", 0x00F1 },
            { "ograve", 0x00F2 },
            { "oacute", 0x00F3 },
            { "ocircumflex", 0x00F4 },
            { "otilde", 0x00F5 },
            { "odieresis", 0x00F6 },
            { "divide", 0x00F7 },
            { "oslash", 0x00F8 },
            { "ugrave", 0x00F9 },
            { "uacute", 0x00FA },
            { "ucircumflex", 0x00FB },
            { "udieresis", 0x00FC },
            { "yacute", 0x00FD },
            { "thorn", 0x00FE },
            { "ydieresis", 0x00FF },

            // Special characters
            { "quoteright", 0x2019 },
            { "quoteleft", 0x2018 },
            { "quotedblleft", 0x201C },
            { "quotedblright", 0x201D },
            { "quotedblbase", 0x201E },
            { "quotesinglbase", 0x201A },
            { "endash", 0x2013 },
            { "emdash", 0x2014 },
            { "bullet", 0x2022 },
            { "ellipsis", 0x2026 },
            { "dagger", 0x2020 },
            { "daggerdbl", 0x2021 },
            { "perthousand", 0x2030 },
            { "fi", 0xFB01 },
            { "fl", 0xFB02 },
            { "fraction", 0x2044 },
            { "florin", 0x0192 },
            { "circumflex", 0x02C6 },
            { "tilde", 0x02DC },
            { "breve", 0x02D8 },
            { "dotaccent", 0x02D9 },
            { "ring", 0x02DA },
            { "hungarumlaut", 0x02DD },
            { "ogonek", 0x02DB },
            { "caron", 0x02C7 },

            // Cyrillic glyphs (afii10xxx range)
            { "afii10017", 0x0410 },  // А
            { "afii10018", 0x0411 },  // Б
            { "afii10019", 0x0412 },  // В
            { "afii10020", 0x0413 },  // Г
            { "afii10021", 0x0414 },  // Д
            { "afii10022", 0x0415 },  // Е
            { "afii10023", 0x0401 },  // Ё
            { "afii10024", 0x0416 },  // Ж
            { "afii10025", 0x0417 },  // З
            { "afii10026", 0x0418 },  // И
            { "afii10027", 0x0419 },  // Й
            { "afii10028", 0x041A },  // К
            { "afii10029", 0x041B },  // Л
            { "afii10030", 0x041C },  // М
            { "afii10031", 0x041D },  // Н
            { "afii10032", 0x041E },  // О
            { "afii10033", 0x041F },  // П
            { "afii10034", 0x0420 },  // Р
            { "afii10035", 0x0421 },  // С
            { "afii10036", 0x0422 },  // Т
            { "afii10037", 0x0423 },  // У
            { "afii10038", 0x0424 },  // Ф
            { "afii10039", 0x0425 },  // Х
            { "afii10040", 0x0426 },  // Ц
            { "afii10041", 0x0427 },  // Ч
            { "afii10042", 0x0428 },  // Ш
            { "afii10043", 0x0429 },  // Щ
            { "afii10044", 0x042A },  // Ъ
            { "afii10045", 0x042B },  // Ы
            { "afii10046", 0x042C },  // Ь
            { "afii10047", 0x042D },  // Э
            { "afii10048", 0x042E },  // Ю
            { "afii10049", 0x042F },  // Я
            { "afii10065", 0x0430 },  // а
            { "afii10066", 0x0431 },  // б
            { "afii10067", 0x0432 },  // в
            { "afii10068", 0x0433 },  // г
            { "afii10069", 0x0434 },  // д
            { "afii10070", 0x0435 },  // е
            { "afii10071", 0x0451 },  // ё
            { "afii10072", 0x0436 },  // ж
            { "afii10073", 0x0437 },  // з
            { "afii10074", 0x0438 },  // и
            { "afii10075", 0x0439 },  // й
            { "afii10076", 0x043A },  // к
            { "afii10077", 0x043B },  // л
            { "afii10078", 0x043C },  // м
            { "afii10079", 0x043D },  // н
            { "afii10080", 0x043E },  // о
            { "afii10081", 0x043F },  // п
            { "afii10082", 0x0440 },  // р
            { "afii10083", 0x0441 },  // с
            { "afii10084", 0x0442 },  // т
            { "afii10085", 0x0443 },  // у
            { "afii10086", 0x0444 },  // ф
            { "afii10087", 0x0445 },  // х
            { "afii10088", 0x0446 },  // ц
            { "afii10089", 0x0447 },  // ч
            { "afii10090", 0x0448 },  // ш
            { "afii10091", 0x0449 },  // щ
            { "afii10092", 0x044A },  // ъ
            { "afii10093", 0x044B },  // ы
            { "afii10094", 0x044C },  // ь
            { "afii10095", 0x044D },  // э
            { "afii10096", 0x044E },  // ю
            { "afii10097", 0x044F },  // я
        };

        /// <summary>
        /// Gets the Unicode value for a PostScript glyph name.
        /// </summary>
        public static ushort GetUnicode(string glyphName)
        {
            if (string.IsNullOrEmpty(glyphName))
                return 0;

            if (_glyphToUnicode.TryGetValue(glyphName, out ushort unicode))
                return unicode;

            // Unknown glyph - return 0 (.notdef)
            return 0;
        }
    }
}
