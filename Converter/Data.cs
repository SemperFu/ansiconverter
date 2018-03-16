using Converter.Properties;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

public class Data
{
    //===============================
    public static bool bDebug = false;

    //===============================
    public static MediaFormats.ANSI oAnsi = new MediaFormats.ANSI();

    public static string VidFmt = "AVI";
    public static int cBPF = 0;
    public static string VidCodec = "LIBXVID";
    public static string pOut = "ANS";
    public static string pIn = "ASC";

    //Strip"
    public static string pSauce = "Keep";

    //"Static"
    public static string pAnim = "Static";

    public static int pOutExist = 0;
    public static bool pSanitize = true;
    public static string pCP = "CP437";
    public static bool pHTMLEncode = true;
    public static bool pHTMLComplete = true;
    public static bool bRemoveCompleted = false;

    //"UTF16"/"UTF8"
    public static string selUTF = "UTF16";

    public static bool pNoColors = false;
    public static bool pSmallFont = false;
    public static bool bCreateThumbs = false;
    public static int iThumbsResOpt = 1;
    public static double iThumbsScale = 50;
    public static int iThumbsWidth = 0;
    public static int iThumbsHeight = 0;
    public static string txtExt;
    public static bool rOutPathInput;
    public static bool rReplaceExt;
    public static string outPath;
    public static MediaSupport.AviWriter oAVIFile = new MediaSupport.AviWriter();

    //public Windows.Forms.Form MForm;
    //public Windows.Forms.ToolTip ToolTip;
    public static double FPS = 29.97;

    public static int BPS = 14400;
    public static int LastFrame = 3;
    public static bool bMakeVideo = false;
    public static string ffmpegpath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ansiconvffmpeg.exe");
    public static int iFramesCount = 0;
    public static string sCodePg;
    public static string sCodePgOut;
    public static string sCodePgIn;
    public static string TempVideoFolder;
    public static bool bHTMLEncode;
    public bool bHTMLComplete = true;
    public static bool bSanitize;
    public static bool bCUFon = false;
    public static int OutputFileExists = 0;
    public static bool bForceOverwrite = false;
    public static string sHTMLFont = "Default";
    public static bool bConv2Unicode;
    public static string sOutPutFormat;
    public static string sInputFormat;
    public static AnsiCPMaps.AnsiCPMaps CPS = AnsiCPMaps.AnsiCPMaps.Instance;
    public static int ForeColor = 7;
    public static int BackColor = 0;
    public static bool Blink = false;
    public static int Bold = 0;
    public static int Intense = 0;
    public static bool Reversed = false;
    public static int XPos = 1;
    public static int YPos = 1;
    public static bool LineWrap = false;
    public static int LinesUsed = 0;
    public static int iLoop = 0;

    //Public DosFnt80x25 As New MediaSupport.Ansifntdef(8, 16, Drawing.Color.FromArgb(255, 32, 32, 32), My.Resources.dosfont80x25c16b2, My.Resources.dosfontback16c)
    //Public DosFnt80x50 As New MediaSupport.Ansifntdef(8, 8, Drawing.Color.FromArgb(255, 32, 32, 32), My.Resources.dosfont80x50c16b2, My.Resources.dosfontback16c)
    public static MediaSupport.Ansifntdef DosFnt80x25 = new MediaSupport.Ansifntdef(8, 16, System.Drawing.Color.FromArgb(255, 0, 0, 0), Resources.fnt80x25, Resources.dosfontback16c);

    public static MediaSupport.Ansifntdef DosFnt80x50 = new MediaSupport.Ansifntdef(8, 8, Color.FromArgb(255, 0, 0, 0), Resources.fnt80x50, Resources.dosfontback16c);
    public static ConverterSupport.ScreenChar[,] Screen;
    public static object sCSSDef = "";

    public static bool bAnimation = false;
    public static string OutFileWrite = "";
    public static int ProcFilesCounter = 0;
    public static bool bExtInp = false;

    public List<FileListItem> ListInputFiles = new List<FileListItem>();
    public static string pBBS = "PCB";

    //Public currCPBlockzoompanel As xPanel
    public const int DefForeColor = 7;

    public const int DefBackColor = 0;
    public const int DefBold = 0;
    public ConverterSupport.SauceMeta oSauce = new ConverterSupport.SauceMeta();
    public bool bHasSauce = false;

    public static bool bOutputSauce = false;
    public static int Yoffset = 0;
    //Img

    //    Public m_FontBmp As Bitmap

    //Public m_TopOffSet As Integer = 0
    //Public m_Margins As New Windows.Forms.Padding(0)
    // Public DosFont As MediaSupport.Ansifntdef = Nothing
    //Public m_BackgroundColor As Drawing.Color = Color.Black
    //Public m_TextColor As Drawing.Color = Color.FromArgb(255, 168, 168, 168)
    // Public DosFontSml As MediaSupport.Ansifntdef = Nothing
    public ArrayList WebFontList = new ArrayList();

    public int SelectedWebFont = 0;
    public const int minX = 1;
    public static int maxX = 80;
    public const int minY = 1;

    public int maxY = 1500;
}