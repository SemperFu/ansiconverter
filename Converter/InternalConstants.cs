using System.Collections.Generic;
using System.Drawing;

namespace Internal
{
	/// <summary>
	/// Constants for Internal Use Only
	/// </summary>
	/// <remarks></remarks>
	public class InternalConstants
	{
		public static string[] aInp = new string[] {
			"ASC",
			"ANS",
			"HTML",
			"UTF",
			"PCB",
			"BIN",
			"WC2",
			"WC3",
			"AVT",
			"IMG",
			"VID"
		};
		public static byte[] UTF16Hdr = new byte[] {
			255,
			254
		};
		public static byte[] UTF8Hdr = new byte[] {
			239,
			187,
			191
		};
		public static byte[] ANSIHdr = new byte[] {
			27,
			91,
			50,
			53,
			53,
			68,
			27,
			91,
			52,
			48,
			109
			//<[255D<[40m "<" = ESC
		};
		public byte[] PCBHdr = new byte[] {
			64,
			88,
			48,
			55
			//@X07
		};
		public static string[] aCPLN;
		public static string[] aCPL;
		public static string[] aCPLISO;
		public static string[] aWinCPL;
		public static string[] aWinCPLN;
		public static string[] aWinCPLISO;
		public static string[] aSpecH = new string[255];
		public static string[] aCPdesc;
		public static string[] aUniCode;
		public static string[,] aCSS = new string[2, 15];
		public static string sSauceCSS = "";
		public static string MainThreadID = "";
		public static System.Drawing.Bitmap m_NFOTextImg;
		public static string m_NFOText = "";
		public static List<System.Windows.Media.Color> AnsiColorsARGBM = new List<System.Windows.Media.Color>();
		public static Color[] AnsiColorsARGB = new Color[] {
			Color.FromArgb(255, 0, 0, 0),
			Color.FromArgb(255, 0, 0, 173),
			Color.FromArgb(255, 0, 173, 0),
			Color.FromArgb(255, 0, 173, 173),
			Color.FromArgb(255, 173, 0, 0),
			Color.FromArgb(255, 173, 0, 173),
			Color.FromArgb(255, 173, 82, 0),
			Color.FromArgb(255, 173, 173, 173),
			Color.FromArgb(255, 82, 82, 82),
			Color.FromArgb(255, 82, 82, 255),
			Color.FromArgb(255, 82, 255, 82),
			Color.FromArgb(255, 82, 255, 255),
			Color.FromArgb(255, 255, 82, 82),
			Color.FromArgb(255, 255, 82, 255),
			Color.FromArgb(255, 255, 255, 82),
			Color.FromArgb(255, 255, 255, 255)

		};
		public static string[] AnsiForeColors = new string[] {
			"30",
			"34",
			"32",
			"36",
			"31",
			"35",
			"33",
			"37"
		};
		public static string[] AnsiBackColors = new string[] {
			"40",
			"44",
			"42",
			"46",
			"41",
			"45",
			"43",
			"47"
		};
		public string[] aCtrlChars = new string[] {
			"NUL",
			"SOH",
			"STX",
			"ETX",
			"EOT",
			"ENQ",
			"ACK",
			"BEL",
			"BS",
			"HT",
			"LF",
			"VT",
			"FF",
			"CR",
			"SO",
			"SI",
			"DLE",
			"DC1",
			"DC2",
			"DC3",
			"DC4",
			"NAK",
			"SYN",
			"ETB",
			"CAN",
			"EM",
			"SUB",
			"ESC",
			"FS",
			"GS",
			"RS",
			"US",
			"SP"

		};
		public int[] TypeCounts = new int[] {
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0

		};




		internal const string sCPLN = "Latin US/United States/Canada|Greek|Baltic Rim|Latin 1 (Western Europe: DE, FR, ES)|" + "Latin 2 (Slavic: PL, RU, BA, HR, HU, CZ, SK)|" + "Cyrillic (RU, BG, UA)|Turkish, TR|Latin 1 Alt (= 850, 0xD5 = U+20AC EURO SYM)|" + "Portuguese, PT|Islandic, IS|Hebrew, IL|Canada, CA (French)|Arabic|Nordic (except IS) (DK, SE, NO, FI)|" + "Cyrillic Russian (based on GOST 19768-87)|Greek 2 (IBM Modern GR)|MS-DOS Thai";

		internal const string  sWinCPLN = "Windows Latin-2|Windows Cyrillic|Windows Latin-1|Windows Greek|Windows Turkish|Windows Hebrew|Windows Arabic|" + "Windows Baltic (1)|Windows Vietnamese|Windows Thai|Windows Japanese|Windows Chinese (VRCN)|Windows Korean|Windows Chinese (HK)";

		internal const string  sCPLISO = "iso-8859-1|iso-8859-7|iso-8859-4|iso-8859-1|iso-8859-2|iso-8859-5|iso-8859-9|iso-8859-1" + "|iso-8859-15|iso-8859-8|iso-8859-1|iso-8859-1|iso-8859-5|iso-8859-7|tactis|-|-";

		internal const string sWinCPLISO = "iso-8859-2|iso-8859-5|us-ascii|iso-8859-7|iso-8859-9|iso-8859-8|-|iso-8859-4|-|tactis|-|-|-|-";
		internal const string sCPL = "CP437|CP737|CP775|CP850|CP852|CP855|CP857|CP858|CP860|CP861|CP862|CP863|CP864|CP865|CP866|CP869|CP874";
		internal const string sWinCPL = "CP1250|CP1251|CP1252|CP1253|CP1254|CP1255|CP1256|CP1257|CP1258|CP874|CP932|CP936|CP949|CP950";
	}
}