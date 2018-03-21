using Internal;
using Microsoft.VisualBasic;
using static Data;
using static Microsoft.VisualBasic.Strings;

namespace ConverterSupport
{
    public class Mappings
    {
        public static void BuildMappings(string sCodePg)
        {
            //----------------------------------------------------------------------------------------------
            //Special Characters
            //----------------------------------------------------------------------------------------------
            for (int iASCIIcount = 0; iASCIIcount <= 255; iASCIIcount++)
            {
                InternalConstants.aSpecH[iASCIIcount] = "";
            }

            BuildBasic(ref InternalConstants.aSpecH);
            //add the remaining ones
            switch (sCodePg)
            {
                case "CP437":
                    BuildCodePage437and850(ref InternalConstants.aSpecH);
                    BuildCodePage437(ref InternalConstants.aSpecH);
                    break;

                case "CP850":
                    BuildCodePage437and850(ref InternalConstants.aSpecH);
                    BuildCodePage850(ref InternalConstants.aSpecH);
                    break;
            }
            if (InStr(1, "|" + InternalConstants.sWinCPL + "|", "|" + sCodePg + "|", CompareMethod.Text) > 0)
            {
                BuildWindows(ref InternalConstants.aSpecH);
            }

            for (int a = 0; a <= CPS.CodePages.Count - 1; a++)
            {
                if (CPS.CodePages[a].Name == sCodePg)
                {
                    InternalConstants.aUniCode = CPS.CodePages[a].Map;
                    InternalConstants.aCPdesc = CPS.CodePages[a].Desc;
                }
            }
        }

        //---------------------------------------------------------------------------------------------------------
        //Named HTML Entities Mapping
        //---------------------------------------------------------------------------------------------------------
        private static void BuildCodePage437(ref string[] aWork)
        {
            //Named HTML Entities Specific for Codepages 437
            aWork[155] = "&cent;";
            //CP437
            aWork[157] = "&yen;";
            //CP437
            aWork[159] = "&fnof;";
            //CP437
            aWork[224] = "&alpha;";
            //CP473
            aWork[226] = "&Gamma;";
            //CP473
            aWork[227] = "&pi;";
            //CP473
            aWork[228] = "&Sigma;";
            //CP473
            aWork[229] = "&sigma;";
            //CP473
            aWork[231] = "&tau;";
            //CP473
            aWork[232] = "&Phi;";
            //CP473
            aWork[233] = "&Theta;";
            //CP473
            aWork[234] = "&Omega;";
            //CP473
            aWork[235] = "&delta;";
            //CP473
            aWork[236] = "&infin;";
            //CP473
            aWork[237] = "&phi;";
            //CP473
            //   aWor(238] = "&epsi;"	'CP473
            aWork[239] = "&cap;";
            //CP473
            aWork[240] = "&equiv;";
            //CP473
            aWork[242] = "&ge;";
            //CP473
            aWork[243] = "&le;";
            //CP473
            aWork[244] = "&lceil;";
            //CP473
            aWork[245] = "&rfloor;";
            //CP473
            aWork[247] = "&asymp;";
            //CP473
            aWork[249] = "&sdot;";
            //CP473
            aWork[251] = "&radic;";
            //CP473
        }

        //---------------------------------------------------------------------------------------------------------
        private static void BuildCodePage850(ref string[] aWork)
        {
            //Named HTML Entities Specific for Codepages 850
            aWork[155] = "&oslash;";
            //CP850
            aWork[157] = "&Oslash;";
            //CP850
            aWork[158] = "&times;";
            //CP850
            aWork[169] = "&reg;";
            //CP850
            aWork[181] = "&Aacute;";
            //CP850
            aWork[182] = "&Acirc;";
            //CP850
            aWork[183] = "&Agrave;";
            //CP850
            aWork[184] = "&copy;";
            //CP850
            aWork[189] = "&cent;";
            //CP850
            aWork[190] = "&yen;";
            //CP850
            aWork[198] = "&atilde;";
            //CP850
            aWork[199] = "&Atilde;";
            //CP850
            aWork[207] = "&curren;";
            //CP850
            aWork[208] = "&eth;";
            //CP850
            aWork[209] = "&ETH;";
            //CP850
            aWork[210] = "&Ecirc;";
            //CP850
            aWork[211] = "&Euml;";
            //CP850
            aWork[212] = "&Egrave;";
            //CP850
            aWork[214] = "&Iacute;";
            //CP850
            aWork[215] = "&Icirc;";
            //CP850
            aWork[216] = "&Iuml;";
            //CP850
            aWork[221] = "&brvbar;";
            //CP850
            aWork[222] = "&Igrave;";
            //CP850
            aWork[224] = "&Oacute;";
            //CP850
            aWork[226] = "&Ocirc;";
            //CP850
            aWork[227] = "&Ograve;";
            //CP850
            aWork[228] = "&otilde;";
            //CP850
            aWork[229] = "&Otilde;";
            //CP850
            aWork[231] = "&thorn;";
            //CP850
            aWork[232] = "&THORN;";
            //CP850
            aWork[233] = "&Uacute;";
            //CP850
            aWork[234] = "&Ucirc;";
            //CP850
            aWork[235] = "&Ugrave;";
            //CP850
            aWork[236] = "&yacute;";
            //CP850
            aWork[237] = "&Yacute;";
            //CP850
            aWork[238] = "&macr;";
            //CP850
            aWork[239] = "&acute;";
            //CP850
            aWork[240] = "&shy;";
            //CP850
            aWork[243] = "&frac34;";
            //CP850
            aWork[244] = "&para;";
            //CP850
            aWork[245] = "&sect;";
            //CP850
            aWork[247] = "&cedil;";
            //CP850
            aWork[249] = "&uml;";
            //CP850
            aWork[251] = "&sup1;";
            //CP850
            aWork[252] = "&sup3;";
            //CP850
        }

        //---------------------------------------------------------------------------------------------------------

        private static void BuildCodePage437and850(ref string[] aWork)
        {
            //Named HTML Entities shared by CP 437 and CP 850

            aWork[128] = "&Ccedil;";
            //CP437 & 850
            aWork[129] = "&uuml;";
            //CP437 & 850
            aWork[130] = "&eacute;";
            //CP437 & 850
            aWork[131] = "&acirc;";
            //CP437 & 850
            aWork[132] = "&auml;";
            //CP437 & 850
            aWork[133] = "&agrave;";
            //CP437 & 850
            aWork[134] = "&aring;";
            //CP437 & 850
            aWork[135] = "&ccedil;";
            //CP437 & 850
            aWork[136] = "&ecirc;";
            //CP437 & 850
            aWork[137] = "&euml;";
            //CP437 & 850
            aWork[138] = "&egrave;";
            //CP437 & 850
            aWork[139] = "&iuml;";
            //CP437 & 850
            aWork[140] = "&icirc;";
            //CP437 & 850
            aWork[141] = "&igrave;";
            //CP437 & 850
            aWork[142] = "&Auml;";
            //CP437 & 850
            aWork[143] = "&Aring;";
            //CP437 & 850
            aWork[144] = "&Eacute;";
            //CP437 & 850
            aWork[145] = "&aelig;";
            //CP437 & 850
            aWork[146] = "&AElig;";
            //CP437 & 850
            aWork[147] = "&ocirc;";
            //CP437 & 850
            aWork[148] = "&ouml;";
            //CP437 & 850
            aWork[149] = "&ograve;";
            //CP437 & 850
            aWork[150] = "&ucirc;";
            //CP437 & 850
            aWork[151] = "&ugrave;";
            //CP437 & 850
            aWork[152] = "&yuml;";
            //CP437 & 850
            aWork[153] = "&Ouml;";
            //CP437 & 850
            aWork[154] = "&Uuml;";
            //CP437 & 850
            aWork[156] = "&pound;";
            //CP437 & 850
            aWork[160] = "&aacute;";
            //CP437 & 850
            aWork[161] = "&iacute;";
            //CP437 & 850
            aWork[162] = "&oacute;";
            //CP437 & 850
            aWork[163] = "&uacute;";
            //CP437 & 850
            aWork[164] = "&ntilde;";
            //CP437 & 850
            aWork[165] = "&Ntilde;";
            //CP437 & 850
            aWork[166] = "&ordf;";
            //CP437 & 850
            aWork[167] = "&ordm;";
            //CP437 & 850
            aWork[168] = "&iquest;";
            //CP437 & 850
            aWork[170] = "&not;";
            //CP437 & 850
            aWork[171] = "&frac12;";
            //CP437 & 850
            aWork[172] = "&frac14;";
            //CP437 & 850
            aWork[173] = "&iexcl;";
            //CP437 & 850
            aWork[174] = "&laquo;";
            //CP437 & 850
            aWork[175] = "&raquo;";
            //CP437 & 850
            aWork[225] = "&szlig;";
            //CP437 & 850
            aWork[230] = "&micro;";
            //CP473 & 850
            aWork[241] = "&plusmn;";
            //CP473 & 850
            aWork[246] = "&divide;";
            //CP473 & 850
            aWork[248] = "&deg;";
            //CP473 & 850
            aWork[250] = "&middot;";
            //CP473 & 850
            aWork[253] = "&sup2;";
            //CP473 & 850
            aWork[255] = "&nbsp;";
            //CP473 & 850
        }

        //---------------------------------------------------------------------------------------------------------

        private static void BuildWindows(ref string[] aWork)
        {
            //Named HTML Entities Specific for Windows Character Set
            //Incomplete
            aWork[160] = "&nbsp;";
            //Windows
            aWork[161] = "&iexcl;";
            //Windows
            aWork[162] = "&cent;";
            //Windows
            aWork[163] = "&pound;";
            //Windows
            aWork[164] = "&curren;";
            //Windows
            aWork[165] = "&yen;";
            //Windows
            aWork[166] = "&brvbar;";
            //Windows
            aWork[167] = "&sect;";
            //Windows
            aWork[168] = "&uml;";
            //Windows
            aWork[169] = "&copy;";
            //Windows
            aWork[170] = "&ordf;";
            //Windows
            aWork[171] = "&laquo;";
            //Windows
            aWork[172] = "&not;";
            //Windows
            aWork[173] = "&shy;";
            //Windows
            aWork[174] = "&reg;";
            //Windows
            aWork[175] = "&macr;";
            //Windows
        }

        //---------------------------------------------------------------------------------------------------------

        private static void BuildBasic(ref string[] aWork)
        {
            //Named HTML Entities unique to CP437, CP850 and Windows Character Set (almost)
            aWork[34] = "&quot;";
            aWork[38] = "&amp;";
            aWork[45] = "&ndash;";
            aWork[60] = "&lt;";
            aWork[62] = "&gt;";
            aWork[94] = "&circ;";
            aWork[126] = "&tilde;";

            aWork[92] = "&#8726;";
            //\ Backslash "\" to Unicode because of issues with TERMINAL Font
            //aWork[95] = "&#9601;" '_
            //aWork[47] = "&#8260;" '/
            //aWork[32] = "&#32;"
            if (InStr(1, "|" + InternalConstants.sWinCPL + "|", "|" + sCodePg + "|", CompareMethod.Text) > 0)
            {
                aWork[160] = "&nbsp;";
            }
            else
            {
                aWork[255] = "&nbsp;";
            }
        }
    }
}