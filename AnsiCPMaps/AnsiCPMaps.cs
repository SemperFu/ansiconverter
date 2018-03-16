using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AnsiCPMaps
{
    public enum CodePg
    {
        CP437 = 0,
        CP737 = 1,
        CP775 = 2,
        CP850 = 3,
        CP852 = 4,
        CP855 = 5,
        CP857 = 6,
        CP858 = 7,
        CP860 = 8,
        CP861 = 9,
        CP862 = 10,
        CP863 = 11,
        CP864 = 12,
        CP865 = 13,
        CP866 = 14,
        CP869 = 15,
        CP874 = 16,
        CP1250 = 17,
        CP1251 = 18,
        CP1252 = 19,
        CP1253 = 20,
        CP1254 = 21,
        CP1255 = 22,
        CP1256 = 23,
        CP1257 = 24,
        CP1258 = 25,
        CP874W = 26,
        CP932 = 27,
        CP936 = 28,
        CP949 = 29,
        CP950 = 30
    }

    public struct CP
    {
        public string Name;
        public string Description;
        public string OS;
        public string ISO;
        public string[] Map;
        public string[] Desc;

        public string[] HTML;
        public CP(string n, string D, string O, string I, string sMap, string sDesc, string sHTML)
        {
            Name = n;
            Description = D;
            OS = O;
            ISO = I;
            Map = Strings.Split(sMap, ",");
            Desc = Strings.Split(sDesc, ",");
            HTML = Strings.Split(sHTML, ",");
        }
        public string toUniHex(byte ascchar)
        {
            //var f = Convert.ToInt32(Map[ascchar]);

            return Strings.Right("0000" + Conversion.Hex(Convert.ToInt32(Map[ascchar])), 4);

            //return Strings.Right("0000" + Conversion.Hex((int)Map(ascchar)), 4);
        }
        public string toHex(byte ascchar)
        {
            return Strings.Right("00" + Conversion.Hex((int)ascchar), 2);
        }
        public byte toASC(int unichar)
        {
            byte bteOut = 32;
            if (unichar <= 127)
            {
                return (byte)unichar;
            }
            else
            {
                byte bteLoop;
                for (bteLoop = 0; bteLoop <= 255; bteLoop++)
                {
                    if (Map[bteLoop] == unichar.ToString())
                    {
                        bteOut = bteLoop;
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
                return bteOut;
            }
        }
        public string SpecialHTML(byte ascchar)
        {
            return HTML[ascchar];
        }
        public int toUNI(byte ascchar)
        {
            //Convert.ToInt32(Map[ascchar])
            return Convert.ToInt32(Map[ascchar]);
        }
        public string ASCtoHTML(byte ascchar)
        {
            if (HTML[ascchar] != "")
            {
                return HTML[ascchar];
            }
            else
            {
                return "&#" + Map[ascchar] + ";";
            }
        }
        public string ASCtoHTMLDec(byte ascchar)
        {
            return "&#" + Map[ascchar] + ";";
        }
        public string ASCtoHTMLHex(byte ascchar)
        {
            return "&#x" + Conversion.Hex(Map[ascchar]) + ";";
        }

    }

    public class clsCodePage
    {
        private CodePg myID;
        private string myCode;
        private string myISO;

        private string myName;
        public clsCodePage(CodePg ID, string Name, string Code, string ISO)
        {
            this.myID = ID;
            this.myCode = Code;
            this.myName = Name;
            this.myISO = ISO;
        }
        //New
        public CodePg ID
        {
            get { return myID; }
        }
        public string Description
        {
            get { return this.myCode + " (" + this.myName + ") " + this.myISO; }
        }
        public string Code
        {
            get { return myCode; }
        }

        public string Name
        {
            get { return myName; }
        }
        public string ISO
        {
            get { return myISO; }
        }

    }
    //CodePage

    public class AnsiCPMaps
    {
        private static AnsiCPMaps m_instance = null;
        public static AnsiCPMaps Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new AnsiCPMaps();
                }

                return m_instance;

            }
        }


        public ArrayList CodePagesSelection;
        public List<CP> CodePages;
        public CP CP437;
        public CP CP737;
        public CP CP775;
        public CP CP850;
        public CP CP852;
        public CP CP855;
        public CP CP857;
        public CP CP858;
        public CP CP860;
        public CP CP861;
        public CP CP862;
        public CP CP863;
        public CP CP864;
        public CP CP865;
        public CP CP866;
        public CP CP869;

        public CP CP874;
        public CP CP1250;
        public CP CP1251;
        public CP CP1252;
        public CP CP1253;
        public CP CP1254;
        public CP CP1255;
        public CP CP1256;
        public CP CP1257;
        public CP CP1258;
        public CP CP874W;
        public CP CP932;
        public CP CP936;
        public CP CP949;

        public CP CP950;
        public string[] aCPLN;
        public string[] aCPL;
        public string[] aCPLISO;
        public string[] aWinCPL;
        public string[] aWinCPLN;

        public string[] aWinCPLISO;

        private AnsiCPMaps()
        {
           
            aCPLN = Strings.Split(Maps.sCPLN, "|");
            aCPL = Strings.Split(Maps.sCPL, "|");
            aCPLISO = Strings.Split(Maps.sCPLISO, "|");
            aWinCPL = Strings.Split(Maps.sWinCPL, "|");
            aWinCPLN = Strings.Split(Maps.sWinCPLN, "|");
            aWinCPLISO = Strings.Split(Maps.sWinCPLISO, "|");

            CodePages = new List<CP>();
            CodePagesSelection = new ArrayList();
            CP437 = new CP("CP437", "Latin US/United States/Canada", "DOS", "iso-8859-1", Maps.sUniCodeCP437, Maps.sCP437desc, Maps.sCP437HTML);
            CodePages.Add(CP437);
            CP737 = new CP("CP737", "Greek", "DOS", "iso-8859-7", Maps.sUniCodeCP737, Maps.sCP737desc, Maps.sCP737HTML);
            CodePages.Add(CP737);
            CP775 = new CP("CP775", "Baltic Rim", "DOS", "iso-8859-4", Maps.sUniCodeCP775, Maps.sCP775desc, Maps.sCP775HTML);
            CodePages.Add(CP775);
            CP850 = new CP("CP850", "Latin 1 (Western Europe: DE, FR, ES)", "DOS", "iso-8859-1", Maps.sUniCodeCP850, Maps.sCP850desc, Maps.sCP850HTML);
            CodePages.Add(CP850);
            CP852 = new CP("CP852", "Latin 2 (Slavic: PL, RU, BA, HR, HU, CZ, SK)", "DOS", "iso-8859-2", Maps.sUniCodeCP852, Maps.sCP852desc, Maps.sCP852HTML);
            CodePages.Add(CP852);
            CP855 = new CP("CP855", "Cyrillic (RU, BG, UA)", "DOS", "iso-8859-5", Maps.sUniCodeCP855, Maps.sCP855desc, Maps.sCP855HTML);
            CodePages.Add(CP855);
            CP857 = new CP("CP857", "Turkish, TR", "DOS", "iso-8859-9", Maps.sUniCodeCP857, Maps.sCP857desc, Maps.sCP857HTML);
            CodePages.Add(CP857);
            CP858 = new CP("CP858", "Latin 1 Alt (= 850, 0xD5 = U+20AC EURO SYM)", "DOS", "iso-8859-1", Maps.sUniCodeCP858, Maps.sCP858desc, Maps.sCP858HTML);
            CodePages.Add(CP858);
            CP860 = new CP("CP860", "Portuguese, PT", "DOS", "iso-8859-15", Maps.sUniCodeCP860, Maps.sCP860desc, Maps.sCP860HTML);
            CodePages.Add(CP860);
            CP861 = new CP("CP861", "Islandic, IS", "DOS", "iso-8859-8", Maps.sUniCodeCP861, Maps.sCP861desc, Maps.sCP861HTML);
            CodePages.Add(CP861);
            CP862 = new CP("CP862", "Hebrew, IL", "DOS", "iso-8859-1", Maps.sUniCodeCP862, Maps.sCP862desc, Maps.sCP862HTML);
            CodePages.Add(CP862);
            CP863 = new CP("CP863", "Canada, CA (French)", "DOS", "iso-8859-1", Maps.sUniCodeCP863, Maps.sCP863desc, Maps.sCP863HTML);
            CodePages.Add(CP863);
            CP864 = new CP("CP864", "Arabic", "DOS", "iso-8859-5", Maps.sUniCodeCP864, Maps.sCP864desc, Maps.sCP864HTML);
            CodePages.Add(CP864);
            CP865 = new CP("CP865", "Nordic (except IS) (DK, SE, NO, FI)", "DOS", "iso-8859-7", Maps.sUniCodeCP865, Maps.sCP865desc, Maps.sCP865HTML);
            CodePages.Add(CP865);
            CP866 = new CP("CP866", "Cyrillic Russian (based on GOST 19768-87)", "DOS", "tactis", Maps.sUniCodeCP866, Maps.sCP866desc, Maps.sCP866HTML);
            CodePages.Add(CP866);
            CP869 = new CP("CP869", "Greek 2 (IBM Modern GR)", "DOS", "-", Maps.sUniCodeCP869, Maps.sCP869desc, Maps.sCP869HTML);
            CodePages.Add(CP869);
            CP874 = new CP("CP874", "MS-DOS Thai", "DOS", "-", Maps.sUniCodeCP874, Maps.sCP874desc, Maps.sCP874HTML);
            CodePages.Add(CP874);


            CP1250 = new CP("CP1250", "Windows Latin-2", "WIN", "iso-8859-2", Maps.sWinUniCodeCP1250, Maps.sWinCP1250desc, Maps.sCP1250HTML);
            CodePages.Add(CP1250);
            CP1251 = new CP("CP1251", "Windows Cyrillic", "WIN", "iso-8859-5", Maps.sWinUniCodeCP1251, Maps.sWinCP1251desc, Maps.sCP1251HTML);
            CodePages.Add(CP1251);
            CP1252 = new CP("CP1252", "Windows Latin-1", "WIN", "us-ascii", Maps.sWinUniCodeCP1252, Maps.sWinCP1252desc, Maps.sCP1252HTML);
            CodePages.Add(CP1252);
            CP1253 = new CP("CP1253", "Windows Greek", "WIN", "iso-8859-7", Maps.sWinUniCodeCP1253, Maps.sWinCP1253desc, Maps.sCP1253HTML);
            CodePages.Add(CP1253);
            CP1254 = new CP("CP1254", "Windows Turkish", "WIN", "iso-8859-9", Maps.sWinUniCodeCP1254, Maps.sWinCP1254desc, Maps.sCP1254HTML);
            CodePages.Add(CP1254);
            CP1255 = new CP("CP1255", "Windows Hebrew", "WIN", "iso-8859-8", Maps.sWinUniCodeCP1255, Maps.sWinCP1255desc, Maps.sCP1255HTML);
            CodePages.Add(CP1255);
            CP1256 = new CP("CP1256", "Windows Arabic", "WIN", "-", Maps.sWinUniCodeCP1256, Maps.sWinCP1256desc, Maps.sCP1256HTML);
            CodePages.Add(CP1256);
            CP1257 = new CP("CP1257", "Windows Baltic (1)", "WIN", "iso-8859-4", Maps.sWinUniCodeCP1257, Maps.sWinCP1257desc, Maps.sCP1257HTML);
            CodePages.Add(CP1257);
            CP1258 = new CP("CP1258", "Windows Vietnamese", "WIN", "-", Maps.sWinUniCodeCP1258, Maps.sWinCP1258desc, Maps.sCP1258HTML);
            CodePages.Add(CP1258);
            CP874W = new CP("CP874", "Windows Thai", "WIN", "tactis", Maps.sWinUniCodeCP874, Maps.sWinCP874desc, Maps.sCP874WHTML);
            CodePages.Add(CP874W);
            CP932 = new CP("CP932", "Windows Japanese", "WIN", "-", Maps.sWinUniCodeCP932, Maps.sWinCP932desc, Maps.sCP932HTML);
            CodePages.Add(CP932);
            CP936 = new CP("CP936", "Windows Chinese (VRCN)", "WIN", "-", Maps.sWinUniCodeCP936, Maps.sWinCP936desc, Maps.sCP936HTML);
            CodePages.Add(CP936);
            CP949 = new CP("CP949", "Windows Korean", "WIN", "-", Maps.sWinUniCodeCP949, Maps.sWinCP949desc, Maps.sCP949HTML);
            CodePages.Add(CP949);
            CP950 = new CP("CP950", "Windows Chinese (HK)", "WIN", "-", Maps.sWinUniCodeCP950, Maps.sWinCP950desc, Maps.sCP950HTML);
            CodePages.Add(CP950);

            int iEnumCnt = 0;

            for (int a = 0; a <= Information.UBound(aCPL); a++)
            {
                CodePagesSelection.Add(new clsCodePage((CodePg)iEnumCnt, aCPLN[a] + " (" + aCPL[a] + ")", aCPL[a], aCPLISO[a]));
                iEnumCnt += 1;
            }
            for (int a = 0; a <= Information.UBound(aWinCPL); a++)
            {
                CodePagesSelection.Add(new clsCodePage((CodePg)iEnumCnt, aWinCPLN[a] + " (" + aCPL[a] + ")", aWinCPL[a], aWinCPLISO[a]));
                iEnumCnt += 1;
            }

        }

    }


    public class UniBlock
    {
        public string sName = "";
        public double IFrom = 0;
        public double iTo = 0;
        public string myKey = "";
        public string sFrom = "";
        public string sTo = "";
        public string MyDisplay = "";
        public string sKey
        {
            get { return myKey; }
        }

        public string sDisplay
        {
            get { return MyDisplay; }
        }

        public UniBlock()
        {
            sName = "";
            IFrom = 0;
            iTo = 0;
            sFrom = "";
            sTo = "";
            myKey = "-";
            MyDisplay = "";
        }
        public UniBlock(string N, string F, string T)
        {
            sName = N;
            sFrom = F;
            IFrom = HexToDec(sFrom);
            sTo = T;
            iTo = HexToDec(sTo);
            myKey = sFrom + "-" + sTo;
            MyDisplay = myKey + " : " + sName + " (" + (iTo - IFrom + 1).ToString() + " characters)";
        }
        //---------------------------------------------------------------------------
        private double HexToDec(string strHex)
        {
            double lngResult = 0;
            int intIndex;
            string strDigit;
            long intDigit;
            double intValue;
            for (intIndex = Strings.Len(strHex); intIndex >= 1; intIndex += -1)
            {
                strDigit = Strings.Mid(strHex, intIndex, 1);
                intDigit = Strings.InStr("0123456789ABCDEF", Strings.UCase(strDigit)) - 1;
                if (intDigit >= 0)
                {
                    intValue = intDigit * (Math.Pow(16, (Strings.Len(strHex) - (long)intIndex)));
                    lngResult = lngResult + intValue;
                }
                else
                {
                    lngResult = 0;
                    intIndex = 0;
                    // stop the loop
                }
            }
            return lngResult;
        }

    }
}