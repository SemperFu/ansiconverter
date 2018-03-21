using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Strings;

internal class Module1
{
    //"1.04.00"

    public static string ToolVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    public const string ToolVersionDate = "03.2012";
    public const bool bDebug = false;
    public static bool bInputFile = true;

    public static ProcessFiles oConv = new ProcessFiles();
    public static System.Windows.Forms.Form F = new System.Windows.Forms.Form();
    public static System.Windows.Forms.ToolTip TT = new System.Windows.Forms.ToolTip();
    public static string sCodePg = "CP437";
    public static bool bHTMLEncode = true;
    public static bool bHTMLComplete = true;
    public static bool bSanitize = false;
    public static string pSauce = "Strip";
    public static string pAnim = "Static";
    public static string pUTF = "UTF16";
    public static bool bCUFon = false;
    public static int OutputFileExists = 0;
    public static  bool bConv2Unicode;
    public static string sOutPutFormat = "";
    public static bool bOutputSauce = false;
    public static bool bAnimation = false;
    public static string txtExt = "";
    public static bool bOutPathInput = true;
    public static string sHTMLFont = "Default";
    static bool bReplaceExt = false;
    public static string soutPath = "";
    public static bool bForceOverwrite = false;
    public static AnsiCPMaps.AnsiCPMaps CPS = AnsiCPMaps.AnsiCPMaps.Instance;
    public static bool bShowHelp = false;
    public static bool bShowCPList = false;
    public static string sExtFilter = "";
    public static string[] aExtFilter;
    public static string sInputPath = "";
    private static int[] aFTCounts = new int[5];
    private static int iNumProc = 0;
    private static int iNumDone = 0;
    public static bool bNoColors = false;
    public static bool bSmallFont = false;
    private static FTypes iOutFormat;
    private static bool bThumb = false;
    private static int iThumbResOpt = 0;
    private static int iThumbProp = 0;
    private static int iThumbWidth = 0;
    private static int iThumbHeight = 0;
    private static double pFPS = 30.0;
    private static int pBPS = 28800;
    private static int pExtend = 3;
    private static string sCodec = "";

    private static object[,] aCodecs = new object[,] {
        {
            "AVI",
            new string[] {
                "ZMBV",
                "H264",
                "FFVI",
                "MPEG4",
                "MJPEG",
                "LIBX264",
                "LIBXVID"
            }
        },
        {
            "MPG",
            new string[] {
                "MPEG1",
                "MPEG2"
            }
        },
        {
            "WMV",
            null
        },
        {
            "MP4",
            null
        },
        {
            "FLV",
            null
        },
        {
            "GIF",
            null
        },
        {
            "MKV",
            null
        },
        {
            "VOB",
            null
        }
    };

    private static string[] aImages = new string[] {
        "PNG",
        "GIF",
        "BMP",
        "JPG",
        "TIF",
        "ICO",
        "WMF",
        "EMF"
    };

    private static string[] aHTML = new string[] {
        "HTM",
        "WEB"
    };

    private string[] aOutputs = new string[] {
        "ASC",
        "ANS",
        "HTML",
        "UTF8",
        "UTF16",
        "PCB",
        "WC2",
        "WC3",
        "AVT",
        "BIN",
        "IMG",
        "VID"
    };

    private object[] aExt = new object[] {
        null,
        null,
        aHTML,
        new string[] { "TXT" },
        new string[] { "TXT" },
        null,
        null,
        null,
        null,
        null,
        aImages,
        aCodecs
    };

    public enum FFormats
    {
        us_ascii = 0,
        utf_8 = 1,
        utf_16 = 2,
        utf_7 = 3,
        utf_32 = 4
    }

    public enum FTypes
    {
        ASCII = 0,
        ANSI = 1,
        HTML = 2,
        Unicode = 3,
        PCB = 4,
        Bin = 5,
        WC2 = 6,
        WC3 = 7,
        AVT = 8,
        IMG = 9,
        VID = 10
    }

    public static void evHandlerInfoMsg(string Msg, bool nolinebreak, bool removelast)
    {
        Console.Out.WriteLine(StripMessageFormatting(Msg));
    }

    public static void evHandlerErrMsg(string Msg)
    {
        Console.Error.WriteLine(StripMessageFormatting(Msg));
    }

    public static void evHandlerAdjustnumTotal(int value)
    {
    }

    public static void evHandlerProcessedFile(int idx)
    {
        iNumDone += 1;
        Console.WriteLine(iNumDone.ToString() + " of " + iNumProc.ToString() + " files processed.");
        Console.WriteLine("");
    }

    public static string StripMessageFormatting(string sMsg)
    {
        string sResult = sMsg;
        sResult = Replace(sResult, "[b]", "", 1, -1, CompareMethod.Text);
        sResult = Replace(sResult, "[/b]", "", 1, -1, CompareMethod.Text);
        sResult = Replace(sResult, "[i]", "", 1, -1, CompareMethod.Text);
        sResult = Replace(sResult, "[/i]", "", 1, -1, CompareMethod.Text);
        sResult = Replace(sResult, "[u]", "", 1, -1, CompareMethod.Text);
        sResult = Replace(sResult, "[/u]", "", 1, -1, CompareMethod.Text);
        return sResult;
    }

     static void Main(string[] args)
    {
    
    oConv.ProcessedFile += evHandlerProcessedFile;
        oConv.InfoMsg += evHandlerInfoMsg;
        oConv.ErrMsg += evHandlerErrMsg;
        oConv.AdjustnumTotal += evHandlerAdjustnumTotal;
        Data.MForm = F;
        Data.ToolTip = TT;

        int iArgs = Environment.GetCommandLineArgs().Count();//My.Application.CommandLineArgs.Count();
        string[] paramsT = Environment.GetCommandLineArgs();

        int iVal = 0;
        double dVal = 0;
        string sVal = "";
        int a = 0;
        int index = 0;
        bool bNamedParam = false;
        string sNamedParam = "";
        string sPath = "";
        bool bError = false;
        if (iArgs > 0)
        {
            if (!bDebug == true)
            {
                for (a = 0; a <= iArgs - 1; a++)
                {
                    string sParam = paramsT[a];
                    Console.WriteLine(Strings.Right("     " + a.ToString(), 5) + "." + sParam);
                }
                Console.WriteLine("... End Params");
            }

            for (a = 0; a <= iArgs - 1; a++)
            {
                bNamedParam = false;
                string sParam = paramsT[a];
                if (Strings.Left(sParam, 1) == "/" & InStr(sParam, ":", CompareMethod.Text) > 2)
                {
                    sNamedParam = Strings.Left(sParam, InStr(sParam, ":", CompareMethod.Text));
                    bNamedParam = true;
                }
                if (bNamedParam == true)
                {
                    sVal = Strings.Right(sParam, Len(sParam) - Len(sNamedParam));
                    switch (LCase(sNamedParam))
                    {
                        case "/ftyp:":
                            sExtFilter = sVal;
                            aExtFilter = Split(sVal, ",");
                            break;

                        case "/out:":
                            sVal = Trim(UCase(sVal));
                            switch (sVal)
                            {
                                case "ASC":
                                    sOutPutFormat = sVal;
                                    iOutFormat = FTypes.ASCII;
                                    break;

                                case "ANS":
                                    sOutPutFormat = sVal;
                                    break;
                                    iOutFormat = FTypes.ANSI;
                                case "HTML":
                                    sOutPutFormat = sVal;
                                    iOutFormat = FTypes.HTML;
                                    break;

                                case "UTF8":
                                    pUTF = "UTF8";
                                    sOutPutFormat = "UTF";
                                    iOutFormat = FTypes.Unicode;
                                    break;

                                case "UTF16":
                                    pUTF = "UTF16";
                                    sOutPutFormat = "UTF";
                                    iOutFormat = FTypes.Unicode;
                                    break;

                                case "PCB":
                                    sOutPutFormat = sVal;
                                    iOutFormat = FTypes.PCB;
                                    break;

                                case "WC2":
                                    sOutPutFormat = sVal;
                                    iOutFormat = FTypes.WC2;
                                    break;

                                case "WC3":
                                    sOutPutFormat = sVal;
                                    iOutFormat = FTypes.WC3;
                                    break;

                                case "AVT":
                                    sOutPutFormat = sVal;
                                    iOutFormat = FTypes.AVT;
                                    break;

                                case "VID":
                                    sOutPutFormat = sVal;
                                    iOutFormat = FTypes.VID;
                                    break;

                                case "BIN":
                                    sOutPutFormat = sVal;
                                    iOutFormat = FTypes.Bin;
                                    break;

                                case "IMG":
                                    sOutPutFormat = sVal;
                                    iOutFormat = FTypes.IMG;
                                    break;

                                default:
                                    Console.Error.WriteLine("'" + sVal + "' is not a valid value for the /OUT: Format Option.");
                                    bError = true;
                                    break;
                            }
                            break;

                        case "/font:":
                            sHTMLFont = sVal;
                            break;

                        case "/nocol:":
                            bNoColors = true;
                            break;

                        case "/small:":
                            bSmallFont = true;
                            break;

                        case "/save:":
                            sVal = Trim(sVal);
                            try
                            {
                                if (Directory.Exists(sVal))
                                {
                                    soutPath = sVal;
                                    bOutPathInput = false;
                                }
                                else
                                {
                                    Console.Error.WriteLine("'" + sVal + "' is not a valid output path or does not exist.");
                                    bError = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Error.WriteLine("'" + sVal + "' is not a valid output path or does not exist.");
                                bError = true;
                            }
                            break;

                        case "/ext:":
                            bReplaceExt = true;
                            txtExt = sVal;
                            break;

                        case "/newext:":
                            bReplaceExt = false;
                            txtExt = sVal;
                            break;

                        case "/over:":
                            sVal = Trim(UCase(sVal));
                            break;
                            switch (sVal)
                            {
                                case "OVER":
                                    OutputFileExists = 0;
                                    break;

                                case "SKIP":
                                    OutputFileExists = 1;
                                    break;

                                case "REN":
                                    OutputFileExists = 2;
                                    break;

                                default:
                                    Console.Error.WriteLine("'" + sVal + "' is not a valid option for the /OVER: Option.");
                                    bError = true;
                                    break;
                            }
                        case "/thumbscale:":
                            sVal = Trim(UCase(sVal));
                            switch (sVal)
                            {
                                case "PROP":
                                    iThumbResOpt = 0;
                                    break;

                                case "WIDTH":
                                    iThumbResOpt = 1;
                                    break;

                                case "HEIGHT":
                                    iThumbResOpt = 2;
                                    break;

                                case "CUSTOM":
                                    iThumbResOpt = 3;
                                    break;
                            }
                            break;

                        case "/scale:":
                            if (IsNumeric(sVal))
                            {
                                iThumbProp = Math.Abs(Convert.ToInt32(sVal));
                            }
                            break;

                        case "/width:":
                            if (IsNumeric(sVal))
                            {
                                iThumbWidth = Math.Abs(Convert.ToInt32(sVal));
                            }
                            break;

                        case "/height:":
                            if (IsNumeric(sVal))
                            {
                                iThumbHeight = Math.Abs(Convert.ToInt32(sVal));
                            }
                            break;

                        case "/cp:":
                            if (CodePageExists(Trim(sVal)))
                            {
                                sCodePg = Trim(UCase(sVal));
                            }
                            else
                            {
                                Console.Error.WriteLine("'" + sVal + "' is not a valid code page value.");
                                bError = true;
                            }
                            break;

                        case "/codec:":
                            sCodec = sVal;
                            break;

                        case "/fps:":
                            if (IsNumeric(sVal))
                            {
                                pFPS = Math.Abs(Convert.ToInt32(sVal));
                            }
                            break;

                        case "/bps:":
                            if (IsNumeric(sVal))
                            {
                                pBPS = Math.Abs(Convert.ToInt32(sVal));
                            }
                            break;

                        case "/extend:":
                            if (IsNumeric(sVal))
                            {
                                pExtend = Math.Abs(System.Convert.ToInt32(sVal));
                            }
                            break;
                    }
                }
                else
                {
                    switch (LCase(sParam))
                    {
                        case "/?":
                            bShowHelp = true;
                            break;

                        case "/h":
                            bShowHelp = true;
                            break;

                        case "/help":
                            bShowHelp = true;
                            break;

                        case "/cplist":
                            bShowCPList = true;
                            break;

                        case "/sanitize":
                            bSanitize = true;
                            break;

                        case "/anim":
                            pAnim = "Anim";
                            bAnimation = true;
                            break;

                        case "/sauce":
                            pSauce = "Keep";
                            bOutputSauce = true;
                            break;

                        case "/object":
                            bHTMLComplete = false;
                            break;

                        case "/thumb":
                            bThumb = true;
                            break;

                        default:
                            sInputPath = sParam;
                            break;
                    }
                }
            }

            if (bThumb == true)
            {
                switch (iThumbResOpt)
                {
                    case 0:
                        if (iThumbProp == 0)
                        {
                            Console.Error.WriteLine("/SCALE parameter not provided for proportional thumbnail scaling.");
                            bError = true;
                        }
                        break;

                    case 1:
                        if (iThumbWidth == 0)
                        {
                            Console.Error.WriteLine("/WIDTH parameter not provided for proportional fixed width thumbnail scaling.");
                            bError = true;
                        }
                        break;

                    case 2:
                        if (iThumbHeight == 0)
                        {
                            Console.Error.WriteLine("/HEIGHT parameter not provided for proportional fixed height thumbnail scaling.");
                            bError = true;
                        }
                        break;

                    case 3:
                        if (iThumbWidth == 0 | iThumbHeight == 0)
                        {
                            Console.Error.WriteLine("/WIDTH and/or /HEIGHT parameter(s) not provided for custom size thumbnail scaling.");
                            bError = true;
                        }
                        break;
                }
            }
            if (bShowHelp == true)
            {
                HelpMessage();
                System.Environment.Exit(0);
            }
            if (bShowCPList == true)
            {
                BuildCPList();
                System.Environment.Exit(0);
            }
            if (sInputPath == "")
            {
                Console.Error.WriteLine("Input File/Folder Path are missing.");
                bError = true;
            }
            else
            {
                try
                {
                    if (Directory.Exists(sInputPath))
                    {
                        bInputFile = false;
                        BuildFileList(sInputPath);
                        if (Data.ListInputFiles.Count() == 0)
                        {
                            Console.Error.WriteLine("Input Folder: '" + sInputPath + "' no files to process found.");
                            bError = true;
                        }
                        else
                        {
                            Console.WriteLine("# Files to Process: " + Data.ListInputFiles.Count + ", Break down by Type: ");
                            string sTmp = "";
                            for (int b = 0; b <= 5; b++)
                            {
                                if (aFTCounts[b] != 0)
                                {
                                    switch (b)
                                    {
                                        case 0:
                                            sTmp += ", ASC: ";
                                            break;

                                        case 1:
                                            sTmp += ", ANS: ";
                                            break;

                                        case 2:
                                            sTmp += ", HTM: ";
                                            break;

                                        case 3:
                                            sTmp += ", UTF: ";
                                            break;

                                        case 4:
                                            sTmp += ", PCB: ";
                                            break;

                                        case 5:
                                            sTmp += ", BIN: ";
                                            break;

                                        case 6:
                                            sTmp += ", WC2: ";
                                            break;

                                        case 7:
                                            sTmp += ", WC3: ";
                                            break;

                                        case 8:

                                            sTmp += ", AVT: ";
                                            break;
                                    }
                                    sTmp += aFTCounts[b].ToString();
                                }
                            }
                            if (sTmp != "")
                            {
                                sTmp = Strings.Right(sTmp, sTmp.Length - 2);
                            }
                            Console.WriteLine(sTmp);
                            Console.WriteLine("");
                        }
                    }
                    else
                    {
                        if (File.Exists(sInputPath))
                        {
                            bInputFile = true;
                            try
                            {
                                AddFile(sInputPath);
                            }
                            catch (Exception ex)
                            {
                            }
                            if (Data.ListInputFiles.Count == 0)
                            {
                                Console.Error.WriteLine("Input File: '" + sInputPath + "' cannot be processed. Unknow/Unsupported File Type or not accessible.");
                                bError = true;
                            }
                        }
                        else
                        {
                            Console.Error.WriteLine("Input File/Folder Path: '" + sInputPath + "' does not exist.");
                            bError = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Invalid Input File/Folder Path Value: '" + sInputPath + "'.");
                    bError = true;
                }
            }
            if (sOutPutFormat == "")
            {
                Console.Error.WriteLine("Output Format was not specified.");
                bError = true;
            }
            if (bError == true)
            {
                Console.Error.WriteLine("Start with /h option for Help.");
                System.Environment.Exit(0);
            }
            if (txtExt == "")
            {
                switch (sOutPutFormat)
                {
                    case "HTML":
                        if (bHTMLComplete == false)
                        {
                            txtExt = "WEB";
                        }
                        else
                        {
                            txtExt = "HTM";
                        }
                        break;

                    case "UTF":
                        txtExt = "TXT";
                        break;

                    case "IMG":
                        txtExt = "PNG";
                        break;

                    case "VID":
                        txtExt = "AVI";
                        break;

                    default:
                        txtExt = sOutPutFormat;
                        break;
                }
            }
            string sStr = "";
            iNumProc = Data.ListInputFiles.Count;
            Console.WriteLine("Output Format: " + sOutPutFormat);
            switch (sOutPutFormat)
            {
                case "HTML":
                    sStr = "Sanitize: ";
                    if (bSanitize)
                    {
                        sStr += "YES";
                    }
                    else
                    {
                        sStr += "NO";
                    }
                    sStr += ", Full HTML: ";
                    if (bHTMLComplete)
                    {
                        sStr += "YES";
                    }
                    else
                    {
                        sStr += "NO";
                    }
                    sStr += ", Anim: ";
                    if (bAnimation)
                    {
                        sStr += "YES";
                    }
                    else
                    {
                        sStr += "NO";
                    }
                    Console.WriteLine(sStr);
                    Console.WriteLine("Font: " + sHTMLFont);
                    break;

                case "UTF":
                    sStr = "Unicode Format: " + pUTF;
                    Console.WriteLine(sStr);
                    break;

                case "IMG":
                    sStr = "No Colors?: " + bNoColors.ToString();
                    sStr += ", Small Font: " + bSmallFont.ToString() + "\r\n";

                    Console.WriteLine(sStr);
                    if (bThumb)
                    {
                        sStr = "Create Thumbnails.";
                        switch (iThumbResOpt)
                        {
                            case 0:
                                sStr += " Scale to: " + iThumbProp + "% of org image size.";
                                break;

                            case 1:
                                sStr += " Fixed width: " + iThumbWidth + " pixels.";
                                break;

                            case 2:
                                sStr += " Fixed Height: " + iThumbHeight + " pixels.";
                                break;

                            case 3:
                                sStr += " Custom Size: " + iThumbWidth + "x" + iThumbHeight + " pixels.";
                                break;
                        }
                        Console.WriteLine(sStr);
                    }
                    break;

                case "VID":
                    sStr = "FPS: " + pFPS.ToString() + ", BPS: " + pBPS.ToString();
                    if (UCase(txtExt) == "AVI" | UCase(txtExt) == "MPG")
                    {
                        sStr += ", Codec: " + sCodec;
                    }
                    Console.WriteLine(sStr);
                    sStr = "Extend Last Frame by " + pExtend + " seconds.";
                    Console.WriteLine(sStr);
                    break;
            }
            sStr = "Output Path: ";
            if (bOutPathInput)
            {
                sStr += " as input";
            }
            else
            {
                sStr += soutPath;
            }
            Console.WriteLine(sStr);
            sStr = "Extension: " + txtExt;
            if (bReplaceExt)
            {
                sStr += " (Replace)";
            }
            else
            {
                sStr += " (Add)";
            }
            Console.WriteLine(sStr);
            sStr = "Keep Sauce Meta: ";
            if (bOutputSauce)
            {
                sStr += "YES";
            }
            else
            {
                sStr += "NO";
            }
            Console.WriteLine("CodePage: " + sCodePg);

            Console.WriteLine(sStr);
            Console.WriteLine("----------------------------------------------------------");
            Data.txtExt = txtExt;
            Data.rOutPathInput = bOutPathInput;
            Data.rReplaceExt = bReplaceExt;
            Data.outPath = soutPath;
            Data.bForceOverwrite = bForceOverwrite;
            Data.sHTMLFont = sHTMLFont;
            Data.pOutExist = OutputFileExists;
            Data.bCreateThumbs = bThumb;
            Data.iThumbsResOpt = iThumbResOpt;
            Data.iThumbsScale = iThumbProp;
            Data.iThumbsWidth = iThumbWidth;
            Data.iThumbsHeight = iThumbHeight;
            Data.sOutPutFormat = sOutPutFormat;
            Data.pOut = sOutPutFormat;
            //sInputFormat = MainForm.pIn.Tag.ToString
            Data.bOutputSauce = bOutputSauce;
            Data.pAnim = pAnim;
            Data.pSauce = pSauce;

            Data.bAnimation = bAnimation;
            Data.bRemoveCompleted = true;

            Data.bSanitize = bSanitize;
            Data.pSanitize = bSanitize;
            Data.pCP = sCodePg;
            Data.sCodePg = sCodePg;
            Data.pNoColors = bNoColors;
            Data.pSmallFont = bSmallFont;
            Data.pHTMLEncode = bHTMLEncode;
            Data.pHTMLComplete = bHTMLComplete;
            Data.selUTF = pUTF;
            Data.BPS = pBPS;
            Data.FPS = pFPS;
            Data.LastFrame = pExtend;
            if (sOutPutFormat == "VID")
            {
                switch (UCase(txtExt))
                {
                    case "AVI":
                        Data.VidCodec = sCodec;
                        break;

                    case "MPG":
                        Data.VidCodec = sCodec;
                        break;

                    default:
                        Data.VidCodec = "";
                        break;
                }
            }
            oConv.ConvertAllFiles();
            Console.WriteLine("DONE PROCESSING!");
            Console.WriteLine("... bye See you next time. Roy/SAC.");
        }
        else
        {
            HelpMessage();
        }
    }

    private static void HelpMessage()
    {
        Console.WriteLine("");
        Console.WriteLine("============================================================================");
        Console.WriteLine("ANSI/ASCII Converter CLI by Carsten Cumbrowski aka Roy/SAC (Freeware)");
        Console.WriteLine("Version: " + ToolVersion + ", From: " + ToolVersionDate);
        Console.WriteLine("============================================================================");
        Console.WriteLine("");
        Console.WriteLine("----------------------------------------------------------------------------");
        Console.WriteLine("ANSI/ASCII Converter CLI Usage:");
        Console.WriteLine("----------------------------------------------------------------------------");
        Console.WriteLine("ANSIASCIIConverterCLI \"[drive:][path][filename]\" /OUT:<FORMAT> [/FTYP:<EXT>,..]");
        Console.WriteLine("                   [/SAVE:<PATH>] [/EXT:<EXT>]|[/NEWEXT:<EXT>] [/OVER:<OPTION>]");
        Console.WriteLine("             [/CP<CP>] [/FONT:<FONTNAME>] [/SANITIZE] [/SAUCE] [OBJECT] [/ANIM]");
        Console.WriteLine("   [/THUMB] [/THUMBSCALE: <OPT>] [/SCALE: <NUM>] [/WIDTH:<NUM>] [/HEIGHT:<NUM>]");
        Console.WriteLine("                    [/CODEC: <ID>] [/FPS: <NUM>] [/BPS: <NUM>] [/EXTEND: <NUM>]");
        Console.WriteLine("");
        Console.WriteLine("   -or-");
        Console.WriteLine("");
        Console.WriteLine("ANSIASCIIConverterCLI [/H]|[/?]|[/HELP]");
        Console.WriteLine("");
        Console.WriteLine("   -or-");
        Console.WriteLine("");
        Console.WriteLine("ANSIASCIIConverterCLI [/CPLIST]");
        Console.WriteLine("");
        Console.WriteLine("----------------------------------------------------------------------------");
        Console.WriteLine("Required Parameters:");
        Console.WriteLine("----------------------------------------------------------------------------");
        Console.WriteLine("");
        Console.WriteLine("\"[drive:][path][filename]\"");
        Console.WriteLine("  Specifies a folder or file to process");
        Console.WriteLine("");
        Console.WriteLine("/OUT:<ASC|ANS|HTML|UTF8|UTF16|PCB|WC2|WC3|AVT|BIN|IMG|VID>");
        Console.WriteLine("   (Required) Output-format. ");
        Console.WriteLine("    Supported Values: ASC|ANS|HTML|UTF8|UTF16|PCB|WC2|WC3|AVT|BIN|IMG|VID");
        Console.WriteLine("");
        Console.WriteLine("   For Image outputs does the Extension determine the Image format.");
        Console.WriteLine("   Supported are: PNG, BMP, JPG, GIF, TIF, ICO, WMF, EMF");
        Console.WriteLine("");
        Console.WriteLine("   For Video outputs does the Extension determine the Video format.");
        Console.WriteLine("   Supported are: AVI, WMV, MP4, MKV, FLI, GIF (Animated), MPG, VOB");
        Console.WriteLine("");
        Console.WriteLine("----------------------------------------------------------------------------");
        Console.WriteLine("Optional Parameters:");
        Console.WriteLine("----------------------------------------------------------------------------");
        Console.WriteLine("");
        Console.WriteLine("/FTYP:<EXT>,...");
        Console.WriteLine("   (Optional) List of file extensions to include (Folder processing). ");
        Console.WriteLine("   Separated by comma, without the \".\"");
        Console.WriteLine("");
        Console.WriteLine("/SAVE:<PATH>");
        Console.WriteLine("   (Optional) New Output Path (default uses Input Folder for outputs)");
        Console.WriteLine("");
        Console.WriteLine("/EXT:<EXT>");
        Console.WriteLine("   (Optional) Output, replaces extension of source with <EXT>");
        Console.WriteLine("");
        Console.WriteLine(" -or-");
        Console.WriteLine("");
        Console.WriteLine("/NEWEXT:<EXT>");
        Console.WriteLine("   (Optional) Output, add <EXT> to source for output file name. (Default)");
        Console.WriteLine("");
        Console.WriteLine("/OVER:<OVER|SKIP|REN> Default: OVER");
        Console.WriteLine("   (Optional) Output, handleing of existing output files. ");
        Console.WriteLine("   Values: \"OVER\"=Overwrite, \"SKIP\"=Skip File or \"REN\"=Auto-Rename");
        Console.WriteLine("");
        Console.WriteLine("/CP:<CP> Default: CP437 (US)");
        Console.WriteLine("   (Optional) CodePage. Only if conversion involves any ASCII Format ");
        Console.WriteLine("   (ASC, ANS, PCB, BIN) and any Unicode Format (UTF, HTML) in any direction.");
        Console.WriteLine("   For list of available CodePage values, use /CPLIST paramter.");
        Console.WriteLine("");
        Console.WriteLine("/FONT:<FontName> Default: Default");
        Console.WriteLine("   (Optional) For HTML Output, Font Name for CSS");
        Console.WriteLine("");
        Console.WriteLine("/SANITIZE");
        Console.WriteLine("   (Optional) For HTML Output, Convert TABs to SPACES etc.");
        Console.WriteLine("");
        Console.WriteLine("/SAUCE");
        Console.WriteLine("   (Optional) Convert SAUCE Meta Tag to new Format (if supported)");
        Console.WriteLine("");
        Console.WriteLine("/OBJECT");
        Console.WriteLine("   (Optional) For HTML Output. Instead of FULL HTML Document, ");
        Console.WriteLine("   Export Main HTML Object ONLY (no CSS, no JS Func).");
        Console.WriteLine("");
        Console.WriteLine("/ANIM");
        Console.WriteLine("   (Optional) For HTML Output. Generate Dynamic Animation (CSS/JS) ");
        Console.WriteLine("   instead of Static Picture. Only use for ANSIAnimations and ");
        Console.WriteLine("   ANSIS that use the Blinking Formatting.");
        Console.WriteLine("");
        Console.WriteLine("/NOCOL");
        Console.WriteLine("   (Optional) For Image Output. Render output without colors (ASCII) ");
        Console.WriteLine("");
        Console.WriteLine("/SMALL");
        Console.WriteLine("   (Optional) For Image Output. Use Small 8x8 font (instead of 8x16) ");
        Console.WriteLine("");
        Console.WriteLine("/THUMB");
        Console.WriteLine("   (Optional) For Image Output. Create Thumbnail Image ");
        Console.WriteLine("");
        Console.WriteLine("/THUMBSCALE: <PROP>|<WIDTH>|<HEIGHT>|<CUSTOM>");
        Console.WriteLine("   (Optional) For Thumbnail Output. Scaling Option");
        Console.WriteLine("");
        Console.WriteLine("/SCALE: <PERCENT>");
        Console.WriteLine("   (Optional) For Thumbnail Scaleing <PROP>. Percent of Full Image Size");
        Console.WriteLine("");
        Console.WriteLine("/WIDTH: <NUM>");
        Console.WriteLine("   (Optional) For Thumbnail Scaleing <WIDTH>/<CUSTOM>. Thumbnail Width");
        Console.WriteLine("");
        Console.WriteLine("/HEIGHT: <NUM>");
        Console.WriteLine("   (Optional) For Thumbnail Scaleing <HEIGHT>/<CUSTOM>. Thumbnail Height");
        Console.WriteLine("");
        Console.WriteLine("/CODEC: <ID>");
        Console.WriteLine("    (Optional) For Video outputs in .AVI or .MPG Format. Following values");
        Console.WriteLine("    are(supported)");
        Console.WriteLine("");
        Console.WriteLine("    For .MPG: MPEG1 (Default) or MPEG2 ");
        Console.WriteLine("    For .AVI: ZMBV (Default), FFVI, H264, MPEG4, MJPEG, LIBX264 or LIBXVID");
        Console.WriteLine("");
        Console.WriteLine("/FPS: <NUM>");
        Console.WriteLine("    (Optional) For Video Outputs. The Video Frame Rate. Default = 30.00");
        Console.WriteLine("");
        Console.WriteLine("/BPS: <NUM>");
        Console.WriteLine("    (Optional) For Video Outputs. The Simulated Modem Speed in BPS ");
        Console.WriteLine("    (Bits per Second). Default = 28800");
        Console.WriteLine("");
        Console.WriteLine("/EXTEND: <NUM>");
        Console.WriteLine("    (Optional) For Video Outputs. Number of Seconds to extend the last Frame");
        Console.WriteLine("    of the Animation. Default = 3 (Seconds)");
        Console.WriteLine("");
        Console.WriteLine("----------------------------------------------------------------------------");
        Console.WriteLine("Extra Parameters:");
        Console.WriteLine("----------------------------------------------------------------------------");
        Console.WriteLine("");
        Console.WriteLine("/H , /? , /HELP ");
        Console.WriteLine("   Returns this help text");
        Console.WriteLine("");
        Console.WriteLine("/CPLIST");
        Console.WriteLine("   All other parameters will be ignored. ");
        Console.WriteLine("   Returns list of available CodePages to be used for the /CP Param.");
        Console.WriteLine("");
        Console.WriteLine("============================================================================");
        Console.WriteLine("");
    }

    private static void BuildCPList()
    {
        string sStr = "";
        Console.WriteLine("");
        Console.WriteLine("List of Supported Code Pages");
        Console.WriteLine("--------+-----+------------+-----------------------------------");
        Console.WriteLine("     CP | OS  | ISO        | Desc");
        Console.WriteLine("--------+-----+------------+-----------------------------------");
        for (int a = 0; a <= CPS.CodePages.Count - 1; a++)
        {
            sStr = " " + Strings.Right("      " + CPS.CodePages[a].Name, 6) + " | ";
            sStr += Strings.Right("   " + CPS.CodePages[a].OS, 3) + " | ";
            sStr += Strings.Right("          " + CPS.CodePages[a].ISO, 10) + " | ";
            sStr += CPS.CodePages[a].Description;
            Console.WriteLine(sStr);
        }
        Console.WriteLine("--------+-----+------------+-----------------------------------");
        Console.WriteLine("");
    }

    private static bool CodePageExists(string sstr)
    {
        bool bReturn = false;
        for (int a = 0; a <= CPS.CodePages.Count - 1; a++)
        {
            if (LCase(CPS.CodePages[a].Name) == sstr)
            {
                bReturn = true;
                break; // TODO: might not be correct. Was : Exit For
            }
        }
        return bReturn;
    }

    private static void AddFile(string sFile)
    {
        bool bAdd = true;
        sFile = Path.GetFullPath(sFile);
        if (sFile != "")
        {
            if (File.Exists(sFile) == false)
            {
                bAdd = false;
            }
            else
            {
                if (ConverterSupport.InputOutput.GetFileSizeNum(sFile) == 0)
                {
                    bAdd = false;
                }
            }
        }
        else
        {
            bAdd = false;
        }
        if (Data.ListInputFiles.Count > 0 & bAdd == true)
        {
            for (int a = 0; a <= Data.ListInputFiles.Count - 1; a++)
            {
                if (Data.ListInputFiles[a].FullPath.Equals(sFile))
                {
                    bAdd = false;
                }
            }
        }
        if (bAdd == true)
        {
            FFormats ff = (FFormats)ConverterSupport.InputOutput.checkFileFormat(sFile);
            FTypes ft;
            if (ff == FFormats.us_ascii)
            {
                ft = (FTypes)ConverterSupport.InputOutput.DetermineFileType(sFile);
            }
            else
            {
                ft = FTypes.Unicode;
            }
            if (ft != iOutFormat)
            {
                aFTCounts[(int)ft] += 1;
                Data.ListInputFiles.Add(new FileListItem(Path.GetFileName(sFile), sFile, (global::FFormats)ff, (global::FTypes)ft));
            }
        }
    }

    private static void BuildFileList(string sFold)
    {
        //FileInfo sFile;
        DirectoryInfo DI = new DirectoryInfo(sFold);
        FileInfo[] sFiles = DI.GetFiles();
        bool bAdd = true;
        if (sFiles.Length > 0)
        {
            foreach (FileInfo sFile in sFiles)
            {
                bAdd = true;
                if (sExtFilter != "")
                {
                    if (InclExt(sFile.Extension) == false)
                    {
                        bAdd = false;
                    }
                }
                if (bAdd == true)
                {
                    try
                    {
                        AddFile(sFile.FullName.ToString());
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
    }

    private static bool InclExt(string sStr)
    {
        bool bResult = false;
        if (Strings.Left(sStr, 1) == ".")
        {
            if (sStr.Length > 1)
            {
                sStr = Strings.Right(sStr, sStr.Length - 1);
            }
            else
            {
                sStr = "";
            }
        }
        for (int a = 0; a <= aExtFilter.Length - 1; a++)
        {
            if (LCase(sStr) == LCase(aExtFilter[a]))
            {
                bResult = true;
                break; // TODO: might not be correct. Was : Exit For
            }
        }
        return bResult;
    }
}