using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IAG.Infrastructure.Formatter;

[ExcludeFromCodeCoverage]
// Translated from Python located at:
// http://stackoverflow.com/a/188877/448
// to add new special characters, add them to the SpecialCharacters dictionary and with the correct unicode hex value. Then add a new Replace statement to the fluent syntax below.
public static class RtfCleaner
{
    private class StackEntry
    {
        public int NumberOfCharactersToSkip { get; }
        public bool Ignorable { get; }

        public StackEntry(int numberOfCharactersToSkip, bool ignorable)
        {
            NumberOfCharactersToSkip = numberOfCharactersToSkip;
            Ignorable = ignorable;
        }
    }

    private static readonly Regex RtfRegex = new(@"\\([a-z]{1,32})(-?\d{1,10})?[ ]?|\\'([0-9a-f]{2})|\\([^a-z])|([{}])|[\r\n]+|(.)",
        RegexOptions.Singleline | RegexOptions.IgnoreCase);

    private static readonly List<string> Destinations = new()
    {
        "aftncn", "aftnsep", "aftnsepc", "annotation", "atnauthor", "atndate", "atnicn", "atnid",
        "atnparent", "atnref", "atntime", "atrfend", "atrfstart", "author", "background",
        "bkmkend", "bkmkstart", "blipuid", "buptim", "category", "colorschememapping",
        "colortbl", "comment", "company", "creatim", "datafield", "datastore", "defchp", "defpap",
        "do", "doccomm", "docvar", "dptxbxtext", "ebcend", "ebcstart", "factoidname", "falt",
        "fchars", "ffdeftext", "ffentrymcr", "ffexitmcr", "ffformat", "ffhelptext", "ffl",
        "ffname", "ffstattext", "field", "file", "filetbl", "fldinst", "fldrslt", "fldtype",
        "fname", "fontemb", "fontfile", "fonttbl", "footer", "footerf", "footerl", "footerr",
        "footnote", "formfield", "ftncn", "ftnsep", "ftnsepc", "g", "generator", "gridtbl",
        "header", "headerf", "headerl", "headerr", "hl", "hlfr", "hlinkbase", "hlloc", "hlsrc",
        "hsv", "htmltag", "info", "keycode", "keywords", "latentstyles", "lchars", "levelnumbers",
        "leveltext", "lfolevel", "linkval", "list", "listlevel", "listname", "listoverride",
        "listoverridetable", "listpicture", "liststylename", "listtable", "listtext",
        "lsdlockedexcept", "macc", "maccPr", "mailmerge", "maln", "malnScr", "manager", "margPr",
        "mbar", "mbarPr", "mbaseJc", "mbegChr", "mborderBox", "mborderBoxPr", "mbox", "mboxPr",
        "mchr", "mcount", "mctrlPr", "md", "mdeg", "mdegHide", "mden", "mdiff", "mdPr", "me",
        "mendChr", "meqArr", "meqArrPr", "mf", "mfName", "mfPr", "mfunc", "mfuncPr", "mgroupChr",
        "mgroupChrPr", "mgrow", "mhideBot", "mhideLeft", "mhideRight", "mhideTop", "mhtmltag",
        "mlim", "mlimloc", "mlimlow", "mlimlowPr", "mlimupp", "mlimuppPr", "mm", "mmaddfieldname",
        "mmath", "mmathPict", "mmathPr", "mmaxdist", "mmc", "mmcJc", "mmconnectstr",
        "mmconnectstrdata", "mmcPr", "mmcs", "mmdatasource", "mmheadersource", "mmmailsubject",
        "mmodso", "mmodsofilter", "mmodsofldmpdata", "mmodsomappedname", "mmodsoname",
        "mmodsorecipdata", "mmodsosort", "mmodsosrc", "mmodsotable", "mmodsoudl",
        "mmodsoudldata", "mmodsouniquetag", "mmPr", "mmquery", "mmr", "mnary", "mnaryPr",
        "mnoBreak", "mnum", "mobjDist", "moMath", "moMathPara", "moMathParaPr", "mopEmu",
        "mphant", "mphantPr", "mplcHide", "mpos", "mr", "mrad", "mradPr", "mrPr", "msepChr",
        "mshow", "mshp", "msPre", "msPrePr", "msSub", "msSubPr", "msSubSup", "msSubSupPr", "msSup",
        "msSupPr", "mstrikeBLTR", "mstrikeH", "mstrikeTLBR", "mstrikeV", "msub", "msubHide",
        "msup", "msupHide", "mtransp", "mtype", "mvertJc", "mvfmf", "mvfml", "mvtof", "mvtol",
        "mzeroAsc", "mzeroDesc", "mzeroWid", "nesttableprops", "nextfile", "nonesttables",
        "objalias", "objclass", "objdata", "object", "objname", "objsect", "objtime", "oldcprops",
        "oldpprops", "oldsprops", "oldtprops", "oleclsid", "operator", "panose", "password",
        "passwordhash", "pgp", "pgptbl", "picprop", "pict", "pn", "pnseclvl", "pntext", "pntxta",
        "pntxtb", "printim", "private", "propname", "protend", "protstart", "protusertbl", "pxe",
        "result", "revtbl", "revtim", "rsidtbl", "rxe", "shp", "shpgrp", "shpinst",
        "shppict", "shprslt", "shptxt", "sn", "sp", "staticval", "stylesheet", "subject", "sv",
        "svb", "tc", "template", "themedata", "title", "txe", "ud", "upr", "userprops",
        "wgrffmtfilter", "windowcaption", "writereservation", "writereservhash", "xe", "xform",
        "xmlattrname", "xmlattrvalue", "xmlclose", "xmlname", "xmlnstbl",
        "xmlopen"
    };

    private static readonly Dictionary<string, string> SpecialCharacters = new()
    {
        {"par", "\n"},
        {"sect", "\n\n"},
        {"page", "\n\n"},
        {"line", "\n"},
        {"tab", "\t"},
        {"emdash", "\u2014"},
        {"endash", "\u2013"},
        {"emspace", "\u2003"},
        {"enspace", "\u2002"},
        {"qmspace", "\u2005"},
        {"bullet", "\u2022"},
        {"lquote", "\u2018"},
        {"rquote", "\u2019"},
        {"ldblquote", "\u201C"},
        {"rdblquote", "\u201D"},
        {"bdquo", "\u201E"},
    };

    /// <summary>
    /// Strip RTF Tags from RTF Text
    /// </summary>
    /// <param name="inputRtf">RTF formatted text</param>
    /// <returns>Plain text from RTF</returns>
    public static string Clean(string inputRtf)
    {
        if (inputRtf == null)
        {
            return null;
        }

        // add new replace here
        inputRtf = inputRtf.Replace(@"\'96", @"\endash ")
            .Replace(@"\'97", @"\emdash ")
            .Replace(@"\'84", @"\bdquo ");

        var stack = new Stack<StackEntry>();
        var ignorable = false; // Whether this group (and all inside it) are "ignorable".
        var ucskip = 0; // Number of ASCII characters to skip after a unicode character.
        var curskip = 0; // Number of ASCII characters left to skip
        var outList = new List<string>(); // Output buffer.

        var match = RtfRegex.Match(inputRtf);

        if (!match.Success)
        {
            // Didn't match the regex
            return inputRtf;
        }

        while (match.Success)
        {
            var word = match.Groups[1].Value;
            var arg = match.Groups[2].Value;
            var hex = match.Groups[3].Value;
            var character = match.Groups[4].Value;
            var brace = match.Groups[5].Value;
            var tchar = match.Groups[6].Value;

            if (!string.IsNullOrEmpty(brace))
            {
                curskip = 0;
                switch (brace)
                {
                    case "{":
                        // Push state
                        stack.Push(new StackEntry(ucskip, ignorable));
                        break;
                    case "}":
                    {
                        // Pop state
                        StackEntry entry = stack.Pop();
                        ucskip = entry.NumberOfCharactersToSkip;
                        ignorable = entry.Ignorable;
                        break;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(character)) // \x (not a letter)
            {
                curskip = 0;
                if (character == "~")
                {
                    if (!ignorable)
                    {
                        outList.Add("\xA0");
                    }
                }
                else if ("{}\\".Contains(character))
                {
                    if (!ignorable)
                    {
                        outList.Add(character);
                    }
                }
                else if (character == "*")
                {
                    ignorable = true;
                }
            }
            else if (!string.IsNullOrEmpty(word)) // \foo
            {
                curskip = 0;
                if (Destinations.Contains(word))
                {
                    ignorable = true;
                }
                else if (ignorable)
                {
                }
                else if (SpecialCharacters.ContainsKey(word))
                {
                    outList.Add(SpecialCharacters[word]);
                }
                else if (word == "uc")
                {
                    ucskip = int.Parse(arg);
                }
                else if (word == "u")
                {
                    var c = int.Parse(arg, NumberStyles.HexNumber);
                    if (c < 0)
                    {
                        c += 0x10000;
                    }

                    outList.Add(char.ConvertFromUtf32(c));
                    curskip = ucskip;
                }
            }
            else if (!string.IsNullOrEmpty(hex)) // \'xx
            {
                if (curskip > 0)
                {
                    curskip -= 1;
                }
                else if (!ignorable)
                {
                    var c = int.Parse(hex, NumberStyles.HexNumber);
                    outList.Add(char.ConvertFromUtf32(c));
                }
            }
            else if (!string.IsNullOrEmpty(tchar))
            {
                if (curskip > 0)
                {
                    curskip -= 1;
                }
                else if (!ignorable)
                {
                    outList.Add(tchar);
                }
            }

            // Get the next match
            match = match.NextMatch();
        }

        var returnString = string.Join(string.Empty, outList.ToArray());

        return returnString.Trim('\n', '\0');
    }
}