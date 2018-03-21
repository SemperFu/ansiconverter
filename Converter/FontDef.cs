using Internal;
using Microsoft.VisualBasic;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.VisualBasic;
using static Data;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.Interaction;
using System;
using Converter.Properties;
using Internal;

namespace MediaSupport
{


	/// <summary>
	/// 
	/// </summary>
	/// <remarks></remarks>
	public class FontDef
	{


		public Bitmap FontBmp {
			//Bitmap image of the Font
			get { return m_FontBmp; }
			set {
				if (!m_FontBmp.Equals(value)) {
					m_FontBmp = value;
					FontBmpBak = value;
				}
			}
		}
		public Image[] FontImgs {
			//Array, which will hold images for each character of the font set
			get { return m_FontImgs; }
		}
		public int FontWidth {
			// = 8  Width used in the Image for a Single Character
			get { return m_FontWidth; }
			set {
				if (m_FontWidth != value) {
					m_FontWidth = value;
				}
			}
		}
		public int FontHeight {
			//= 16  Height used in the Image for a Single Character
			get { return m_FontHeight; }
			set {
				if (m_FontHeight != value) {
					m_FontHeight = value;
				}
			}
		}
		public int FontMarginLeft {
			// = 0 Top, Bottom, Left, Right offset to use
			get { return m_FontMarginLeft; }
			set {
				if (m_FontMarginLeft != value) {
					m_FontMarginLeft = value;
				}
			}
		}
		public int FontMarginRight {
			// = 0
			get { return m_FontMarginRight; }
			set {
				if (m_FontMarginRight != value) {
					m_FontMarginRight = value;
				}
			}
		}
		public int FontMarginTop {
			//= 0
			get { return m_FontMarginTop; }
			set {
				if (m_FontMarginTop != value) {
					m_FontMarginTop = value;
				}
			}
		}
		public int FontMarginBottom {
			// = 0
			get { return m_FontMarginBottom; }
			set {
				if (m_FontMarginBottom != value) {
					m_FontMarginBottom = value;
				}
			}
		}
		public int FontSpaceWidth {
			//= -1  with of the space character, -1 = Font Width
			get { return m_FontSpaceWidth; }
			set {
				if (m_FontSpaceWidth != value) {
					m_FontSpaceWidth = value;
				}
			}
		}
		public int FontInitChar {
			//= 1 Asc Code of first character in the Image File
			get { return m_FontInitChar; }
			set {
				if (m_FontInitChar != value) {
					m_FontInitChar = value;
				}
			}
		}
		public int FontCharsperLine {
			//= 32  Number of Characters per line defined in Image
			get { return m_FontCharsperLine; }
			set {
				if (m_FontCharsperLine != value) {
					m_FontCharsperLine = value;
				}
			}
		}
		public int FontCharFrom {
			//Font Image Contains characters starting from ASC this code 
			get { return m_FontCharFrom; }
			set {
				if (m_FontCharFrom != value) {
					m_FontCharFrom = value;
				}
			}
		}
		public int FontCharTo {
			//to this ASCII code
			get { return m_FontCharTo; }
			set {
				if (m_FontCharTo != value) {
					m_FontCharTo = value;
				}
			}
		}
		public Color FontTranspColor {
			// Transparent Color /Background color
			get { return m_FontTranspColor; }
			set {
				if (!m_FontTranspColor.Equals(value)) {
					m_FontTranspColor = value;
					if (bBuildFont == true) {
						this.BuildFont();
					}
				}
			}
		}




		private Bitmap m_FontBmp;
		private Image[] m_FontImgs;
		private IntPtr[,,] m_FontArrays;
		private int m_FontWidth;
		private int m_FontHeight;
			// = 0 Top, Bottom, Left, Right offset to use
		private int m_FontMarginLeft;
			// = 0
		private int m_FontMarginRight;
			//= 0
		private int m_FontMarginTop;
			// = 0
		private int m_FontMarginBottom;
			//= -1  with of the space character, -1 = Font Width
		private int m_FontSpaceWidth;
			//= 1 Asc Code of first character in the Image File
		private int m_FontInitChar;
			//= 32  Number of Characters per line defined in Image
		private int m_FontCharsperLine;
			//Font Image Contains characters starting from ASC this code 
		private int m_FontCharFrom;
			//to this ASCII code
		private int m_FontCharTo;
			// Transparent Color /Background color
		private Color m_FontTranspColor;
		private Color m_FontColor;
			//Bitmap image of the Font (Backup)
		private Bitmap FontBmpBak;
		private bool bBuildFont;
		private int m_FontColorPalIndex;

		private int m_FontTranspColorPalIndex;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="w"></param>
		/// <param name="h"></param>
		/// <param name="marginlf"></param>
		/// <param name="marginrt"></param>
		/// <param name="margintp"></param>
		/// <param name="marginbt"></param>
		/// <param name="spcw"></param>
		/// <param name="initc"></param>
		/// <param name="chrspln"></param>
		/// <param name="chrf"></param>
		/// <param name="chrt"></param>
		/// <remarks></remarks>
		public FontDef(int w, int h, int marginlf, int marginrt, int margintp, int marginbt, int spcw, int initc, int chrspln, int chrf,
		int chrt, int backcolor, int forecolor)
		{
			 // ERROR: Not supported in C#: ReDimStatement

			 // ERROR: Not supported in C#: ReDimStatement

			m_FontBmp = null;
			m_FontWidth = w;
			m_FontHeight = h;
			m_FontMarginLeft = marginlf;
			m_FontMarginRight = marginrt;
			m_FontMarginTop = margintp;
			m_FontMarginBottom = marginbt;
			m_FontSpaceWidth = spcw;
			m_FontInitChar = initc;
			m_FontCharsperLine = chrspln;
			//m_FontTranspColor = Color.Black
			m_FontCharFrom = chrf;
			m_FontCharTo = chrt;
			m_FontTranspColor = Color.FromArgb(backcolor);
			m_FontColor = Color.FromArgb(forecolor);
			bBuildFont = false;
			//   For a As Integer = 0 To 255
			//FontImgs(a) = New Bitmap(FontWidth, FontHeight)
			//Next
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bm"></param>
		/// <remarks></remarks>
		public void SetBitmap(Bitmap bm)
		{
			m_FontBmp = bm;
			FontBmpBak = bm;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="col"></param>
		/// <remarks></remarks>
		public void SetTranspColor(Color col)
		{
			if (!m_FontTranspColor.Equals(col)) {
				m_FontTranspColor = col;
				if (bBuildFont == true) {
					this.BuildFont();
				}
			}
		}

		public void SetFontColor(Color NewColor)
		{
			if (!(m_FontBmp == null)) {
				Color c;
				Int32 x;
				Int32 y;
				//e.Graphics.DrawImage(bmp, 10, 30)
				for (x = 0; x <= FontBmpBak.Width - 1; x++) {
					for (y = 0; y <= FontBmpBak.Height - 1; y++) {
						c = FontBmpBak.GetPixel(x, y);
						if (c.Equals(m_FontColor)) {
							c = NewColor;
							m_FontBmp.SetPixel(x, y, c);
						}
					}
				}
				if (bBuildFont == true) {
					this.BuildFont();
				}
			}
		}
		public ColorPalette ExportPalette()
		{
			ColorPalette cp = new Bitmap(1, 1).Palette;
			if (!(m_FontBmp == null)) {
				cp = m_FontBmp.Palette;
			}
			return cp;
		}
		public void ChangePalettenEntry(int id, Color Col)
		{
			if (!(m_FontBmp == null)) {
				if (id >= 0 & id <= m_FontBmp.Palette.Entries.Count() - 1) {
					m_FontBmp.Palette.Entries[id] = Col;
				}
			}
		}
		public Color GetPalettenEntry(int id)
		{
            if (!(m_FontBmp == null))
            {
                if (id >= 0 & id <= m_FontBmp.Palette.Entries.Count() - 1)
                {
                    return m_FontBmp.Palette.Entries[id];
                }
                else return new Color();//Not sure what it  needs.
            }
            else
            {
                return new Color(); //Not sure what it  needs.
            }
		}
		public int FindPalettenEntry(Color col)
		{
			ColorPalette cp = new Bitmap(1, 1).Palette;
			int a;
			int iResult = -1;
			if (!(m_FontBmp == null)) {
				if (m_FontBmp.PixelFormat == PixelFormat.Indexed) {
					cp = m_FontBmp.Palette;
					for (a = 0; a <= cp.Entries.Count() - 1; a++) {
						if (cp.Entries[a].Equals(col)) {
							iResult = a;
							break; // TODO: might not be correct. Was : Exit For
						}
					}
				} else {
					iResult = -4;
				}
			}
			return iResult;
		}
		public Color[] PaletteArray()
		{
			ColorPalette cp = new Bitmap(1, 1).Palette;
			Color[] aCol = new Color[] { Color.Black };
			if (!(m_FontBmp == null)) {
				cp = m_FontBmp.Palette;
				 // ERROR: Not supported in C#: ReDimStatement

				for (int a = 0; a <= cp.Entries.Count() - 1; a++) {
					aCol[a] = cp.Entries[a];
				}
			}
			return aCol;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks></remarks>
		public void BuildFont()
		{
			if (!(m_FontBmp == null)) {
				//Create images for all characters included in the font
				m_FontColorPalIndex = FindPalettenEntry(m_FontColor);
				m_FontTranspColorPalIndex = FindPalettenEntry(m_FontTranspColor);

				for (int a = m_FontCharFrom; a <= m_FontCharTo; a++) {
					GetFont(a);
				}
				bBuildFont = true;
			}
		}
		private void BuildBitmapArray(Bitmap bmap, byte code)
		{
			Array[] bArray = new Array[(16 * 8) - 1];
			int b;
			int bc;
			int fc;
			for (b = 0; b <= (16 * 8) - 1; b++) {
				byte[] cTemp = new byte[(bmap.Width * bmap.Height) - 1];
				bArray[b] = cTemp;
			}
			int a = 0;
			Color c;
			//Dim MyPointer As IntPtr
			for (int x = 0; x <= bmap.Width - 1; x++) {
				for (int y = 0; y <= bmap.Height - 1; y++) {
					c = bmap.GetPixel(x, y);
					if (c.Equals(m_FontTranspColor)) {
						int d = 0;
						for (b = 0; b <= (16 * 8) - 1; b++) {
							bArray[b][a] = d;
							if (b + 1 % 16 == 0) {
								d += 1;
								if (d == 16)
									d = 0;
							}
						}
					} else if (c.Equals(m_FontColor)) {
						int d = 0;
						for (b = 0; b <= (16 * 8) - 1; b++) {
							bArray(b)(a) = d;
							if (b + 1 % 8 == 0) {
								d += 1;
								if (d == 8)
									d = 0;
							}
						}
					}
					a += 1;
				}
			}
			fc = 0;
			bc = 0;
			for (b = 0; b <= (16 * 8) - 1; b++) {
				GCHandle MyGC = GCHandle.Alloc(bArray[b], GCHandleType.Pinned);
				//MyPointer = Marshal.AllocHGlobal(Marshal.SizeOf(bArray(b)))
				//Marshal.StructureToPtr(bArray(b), MyPointer, False)
				if (b + 1 % 8 == 0) {
					bc += 1;
					if (bc == 8)
						bc = 0;
				}
				if (b + 1 % 16 == 0) {
					fc += 1;
					if (fc == 16)
						fc = 0;
				}
				m_FontArrays[code, fc, bc] = MyGC;
			}
		}
		struct Bte128
		{
			public byte b001;
			public byte b002;
			public byte b003;
			public byte b004;
			public byte b005;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="charcode"></param>
		/// <remarks></remarks>
		private void GetFont(int charcode)
		{
			//Charcode = Character ASCII Code
			//Cut Out character from the Font Bitmap and store it in the font chars array
			int FntX;
			int FntY;
			int FntPOS;
			int CutLen;
			int CutHgt;
			FntPOS = charcode - m_FontInitChar;
			FntX = ((FntPOS % m_FontCharsperLine) * m_FontWidth) + m_FontMarginLeft;
			FntY = (Conversion.Int(FntPOS / m_FontCharsperLine) * m_FontHeight) + m_FontMarginTop;
			CutLen = m_FontWidth - m_FontMarginLeft - m_FontMarginRight;
			CutHgt = m_FontHeight - m_FontMarginTop - m_FontMarginBottom;
			Bitmap bm = new Bitmap(m_FontWidth, m_FontHeight);

			//bm.MakeTransparent(Color.FromArgb(128, 0, 128, 255))
			//Dim bmcol As Bitmap = Resources.ansibackgrounds256
			//Dim pal As Drawing.Imaging.ColorPalette = bmcol.Palette
			//bm.Palette = pal
			Graphics gr = Graphics.FromImage(bm);
			gr.Clear(m_FontTranspColor);
			gr.PageUnit = GraphicsUnit.Pixel;

			Rectangle destRect = new Rectangle(0, 0, m_FontWidth, m_FontHeight);
			Rectangle srcRect = new Rectangle(FntX, FntY, CutLen, CutHgt);
			try {
				gr.DrawImage(m_FontBmp, destRect, srcRect, gr.PageUnit);

			} catch (Exception ex) {
			}
			gr.Dispose();
			m_FontImgs[charcode] = bm;
			//Call BuildBitmapArray(bm, charcode)
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="gr"></param>
		/// <param name="xpos"></param>
		/// <param name="ypos"></param>
		/// <param name="stext"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public Bitmap DrawText(ref Graphics gr, int xpos, int ypos, ref object stext, bool bDrawNoColors)
		{
			if (bBuildFont == false) {
				this.BuildFont();
			}
			ConverterSupport.ScreenChar[,] aText = null;
			int CharXPos = xpos;
			int CharYPos = ypos;
			string sworktxt = "";
			string[] aworktxt = new string[] { "" };
			int CutLen;
			int CutHgt;
			int FntCurrChar;
			int iMaxWidth = 0;
			int iMaxHeight = 0;
			int UsedWidth = m_FontWidth - m_FontMarginLeft - m_FontMarginRight;

			bool bIsASCII = true;
			if (stext is string) {
				bIsASCII = true;
			} else if (stext is ConverterSupport.ScreenChar[,]) {
				bIsASCII = false;
			}
			if (bDrawNoColors == true) {
				aworktxt = Strings.Split((string)stext, Environment.NewLine);
				for (int a = 0; a <= Information.UBound(aworktxt); a++) {
					if (Strings.Len(aworktxt[a]) > iMaxWidth) {
						iMaxWidth = Strings.Len(aworktxt[a]);
					}
				}
				iMaxHeight = (Information.UBound(aworktxt) + 1);
			} else {
				iMaxWidth = Data.maxX;
				iMaxHeight = Data.LinesUsed;
				aText = Data.Screen;
				//CType(stext, ScreenChar(,))
			}
			int iLineLen = iMaxWidth;
			//Dim bmcol As Bitmap = Resources.ansibackgrounds
			//Dim pal As Drawing.Imaging.ColorPalette = bmcol.Palette

			Bitmap bm = new Bitmap(iMaxWidth * m_FontWidth, iMaxHeight * m_FontHeight);
			// bm.MakeTransparent(Color.FromArgb(128, 0, 128, 255))

			int ForeColorID = 7;
			int BackColorID = 0;
			Color NewForeColor = new Color();
			Color NewBackColor= new Color();
			//bm.Palette = pal
			gr = Graphics.FromImage(bm);
			gr.PageUnit = GraphicsUnit.Pixel;
			gr.Clear(Color.Black);
			//m_BackgroundColor
			//gr.DrawRectangle(New Drawing.Pen(m_FontTranspColor), New Drawing.Rectangle(0, 0, bm.Width, bm.Height))
			for (int a = 0; a <= iMaxHeight - 1; a++) {
				CharXPos = xpos;
				if (bDrawNoColors == true) {
					sworktxt = aworktxt[a];
					iLineLen = Strings.Len(sworktxt);
				}

				for (int FntCharPos = 1; FntCharPos <= iLineLen; FntCharPos++) {
					ImageAttributes ImgAttributes = new ImageAttributes();
					ColorMap[] ImgColorMap = null;
					int MapCount = -1;
					Bitmap charimg = new Bitmap(m_FontWidth, m_FontHeight);
					Graphics grchar = Graphics.FromImage(charimg);
					grchar.Clear(m_FontTranspColor);

					if (bDrawNoColors == true) {
						FntCurrChar = Strings.Asc(Strings.Mid(sworktxt, FntCharPos, 1));
					} else {
						FntCurrChar = Data.Screen[FntCharPos, a + 1].DosChar;
						BackColorID = Data.Screen[FntCharPos, a + 1].BackColor;
						ForeColorID = Data.Screen[FntCharPos, a + 1].ForeColor + Data.Screen[FntCharPos, a + 1].Bold;


						NewForeColor = InternalConstants.AnsiColorsARGB[ForeColorID];
						NewBackColor = InternalConstants.AnsiColorsARGB[BackColorID];
						if (!NewBackColor.Equals(m_FontTranspColor) & 1 == 2) {
							MapCount += 1;
							Array.Resize(ref ImgColorMap, MapCount + 1);
							ImgColorMap[MapCount] = new ColorMap();
							ImgColorMap[MapCount].OldColor = new Color();
							ImgColorMap[MapCount].OldColor = m_FontTranspColor;
							ImgColorMap[MapCount].NewColor = new Color();
							ImgColorMap[MapCount].NewColor = NewBackColor;
						}
						if (!NewForeColor.Equals(m_FontColor) & 1 == 2) {
							MapCount += 1;
							Array.Resize(ref ImgColorMap, MapCount + 1);
							ImgColorMap[MapCount] = new ColorMap();
							ImgColorMap[MapCount].OldColor = new Color();
							ImgColorMap[MapCount].OldColor = m_FontColor;
							ImgColorMap[MapCount].NewColor = new Color();
							ImgColorMap[MapCount].NewColor = NewForeColor;
						}
						if (MapCount > -1) {
							ImgAttributes.SetRemapTable(ImgColorMap, ColorAdjustType.Bitmap);
						}
						if (FntCurrChar == 0) {
							FntCurrChar = 32;
						}
					}
					if (FntCurrChar == 32 & m_FontSpaceWidth != -1) {
						CharXPos = CharXPos + m_FontSpaceWidth;
					} else {
						CutLen = m_FontWidth - m_FontMarginLeft - m_FontMarginRight;
						CutHgt = m_FontHeight - m_FontMarginTop - m_FontMarginBottom;
						Rectangle destRect = new Rectangle(CharXPos, CharYPos, m_FontWidth, m_FontHeight);
						Rectangle srcRect = new Rectangle(0, 0, CutLen, CutHgt);

						Bitmap imgChar;
						if (bDrawNoColors == false) {
							//imgChar = New Bitmap(m_FontWidth, m_FontHeight, m_FontWidth, PixelFormat.Format8bppIndexed, m_FontArrays(FntCurrChar, ForeColorID, BackColorID))
							imgChar = (Bitmap) m_FontImgs[FntCurrChar];
							// Dim bmcol As Bitmap = Resources.ansibackgrounds256
							// Dim pal As Drawing.Imaging.ColorPalette = bmcol.Palette
							// imgChar.Palette = pal
							Bitmap drawimg = ConverterSupport.Convert.Replace2ColorsInImage(imgChar, m_FontColor, NewForeColor, m_FontTranspColor, NewBackColor);
							imgChar = drawimg;
							//imgChar2.Palette = 
						} else {
							imgChar = (Bitmap) m_FontImgs[FntCurrChar];
						}

						grchar.DrawImage(imgChar, 0, 0, m_FontWidth, m_FontHeight);

						//Dim bmdo As BitmapData = bm.LockBits(New Rectangle(0, 0, m_FontWidth, m_FontHeight), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed)


						//Dim chrimg As New Bitmap(CutLen, CutHgt)
						//chrimg.Palette = imgChar.Palette
						//Dim gr2 As Graphics = Graphics.FromImage(chrimg)
						//gr2.Clear(NewBackColor)

						//                    gr.DrawRectangle(New Drawing.Pen(AnsiColorsARGB(BackColorID)), destRect)
						// If bDrawNoColors = False And Not imgChar Is Nothing Then


						//Dim drawimg As Bitmap = Replace2ColorsInImage(imgChar, Color.FromArgb(255, 168, 168, 168), NewForeColor, Color.White, NewBackColor)
						//imgChar = ReplaceColorInImage(imgChar, Me.m_BackgroundColor, AnsiColorsARGB(Screen(FntCharPos, a + 1).BackColor))
						//If MapCount > -1 Then
						//    gr2.DrawImage(imgChar, destRect, 0, 0, CutLen, CutHgt, GraphicsUnit.Pixel, ImgAttributes)
						// gr.DrawImage(charimg, destRect, srcRect, GraphicsUnit.Pixel)
						//  Else
						//       gr.DrawImage(imgChar, destRect, srcRect, GraphicsUnit.Pixel)
						//    End If

						// Else
						//     gr.DrawImage(imgChar, destRect, srcRect, GraphicsUnit.Pixel)
						// End If

						//gr2.Dispose()

						gr.DrawImage(charimg, destRect, srcRect, GraphicsUnit.Pixel);

						CharXPos = CharXPos + UsedWidth;
					}
				}
				CharYPos += m_FontHeight;
			}
			gr.Dispose();
			return bm;

		}


	}
}