using ConverterSupport;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
namespace MediaSupport
{


	public class Ansifntdef
	{
		private int m_Fontwidth;
		private int m_FontHeight;
		private System.Drawing.Color m_FontTranspColor;
		private Bitmap m_FontImage;
		private Bitmap m_FontBGImage;
		private System.Drawing.Imaging.ColorPalette m_Palette;
		public int DefaultForecolorID = 7;
		public int DefaultBackColorID = 0;
		public string[] FntBits = new string[255];

		public bool FontSet = false;
		public int Width {
			get { return m_Fontwidth; }
		}
		public int Height {
			get { return m_FontHeight; }
		}
		public Ansifntdef()
		{
			FontSet = false;
		}
		public Ansifntdef(int w, int h, System.Drawing.Color tc, Bitmap fimg, Bitmap fbgimg)
		{
			m_Fontwidth = w;
			m_FontHeight = h;
			m_FontTranspColor = tc;
			m_FontBGImage = fbgimg;
			m_Palette = m_FontBGImage.Palette;
			//m_FontImage = BitmapToIndexed(fimg, m_Palette)
			//m_FontImage.Palette = m_Palette
			m_FontImage = fimg;
			m_FontImage.MakeTransparent(m_FontTranspColor);
			if (m_Fontwidth == 8) {
				BuildFntBits();
			}
			FontSet = true;
		}
		private void BuildFntBits()
		{
			System.Drawing.Color c;
			int btCnt = 0;
			int crCnt = 0;
			byte bt = 0;
			for (int a = 0; a <= 255; a++) {
				FntBits[a] = "";
			}
			for (int y = 0; y <= m_FontImage.Height - 1; y++) {
				btCnt = 0;
				crCnt = 0;
				bt = 0;
				for (int x = 0; x <= m_FontImage.Width - 1; x++) {
					c = m_FontImage.GetPixel(x, y);
					btCnt += 1;
					if (c.R == m_FontTranspColor.R & c.G == m_FontTranspColor.G & c.B == m_FontTranspColor.B) {
					} else {
						Convert.SetBit(ref bt, btCnt);
					}
					if (btCnt == 8) {
						FntBits(crCnt) += Strings.Right("00" + Hex(bt).ToString, 2);
						bt = 0;
						btCnt = 0;
						crCnt += 1;
					}
				}
			}
		}

		public Bitmap DrawText(bool bNoCols = false)
		{
			int FntCurrChar;
			int iMaxWidth = 0;
			int iMaxHeight = 0;
			Graphics gr;
			int CharXPos = 0;
			int CharYPos = 0;
			int UseForeColID = DefaultForecolorID;
			int UseBackColID = DefaultBackColorID;
			iMaxWidth = maxX;
			iMaxHeight = LinesUsed;
			Bitmap bm = null;
			try {
				bm = ConverterSupport.BitmapToIndexed(new Bitmap(iMaxWidth * m_Fontwidth, iMaxHeight * m_FontHeight), m_Palette);

			} catch (Exception ex) {
			}

			bm.Palette = m_Palette;
			bm.MakeTransparent(m_FontTranspColor);

			int ForeColorID = DefaultForecolorID;
			int BackColorID = DefaultBackColorID;
			gr = Graphics.FromImage(bm);
			gr.PageUnit = GraphicsUnit.Pixel;
			gr.Clear(Color.Black);

			for (int a = 0; a <= iMaxHeight - 1; a++) {
				CharXPos = 0;
				for (int FntCharPos = 1; FntCharPos <= iMaxWidth; FntCharPos++) {
					Bitmap charimg = new Bitmap(m_Fontwidth, m_FontHeight);
					charimg = ConverterSupport.BitmapToIndexed(new Bitmap(m_Fontwidth, m_FontHeight), m_Palette);
					charimg.Palette = m_Palette;
					charimg.MakeTransparent(m_FontTranspColor);
					Graphics grchar = Graphics.FromImage(charimg);
					grchar.Clear(Color.Black);
					grchar.PageUnit = GraphicsUnit.Pixel;

					FntCurrChar = Screen(FntCharPos, a + 1).DosChar;
					BackColorID = Screen(FntCharPos, a + 1).BackColor;
					ForeColorID = Screen(FntCharPos, a + 1).ForeColor + Screen(FntCharPos, a + 1).Bold;
					Rectangle destRect = new Rectangle(CharXPos, CharYPos, m_Fontwidth, m_FontHeight);
					if (bNoCols == false) {
						UseForeColID = ForeColorID;
						UseBackColID = UseBackColID;
					}
					//Dim srcRectBG As Rectangle = New Rectangle(UseBackColID * m_Fontwidth, 0, m_Fontwidth, m_FontHeight)
					Rectangle srcRectCR = new Rectangle(FntCurrChar * m_Fontwidth, UseForeColID * m_FontHeight, m_Fontwidth, m_FontHeight);

					//gr.DrawImage(m_FontBGImage, destRect, srcRectBG, GraphicsUnit.Pixel)
					//gr.Clear(AnsiColorsARGB(UseBackColID))
					//gr.DrawRectangle(New Drawing.Pen(AnsiColorsARGB(UseBackColID)), destRect)
					gr.FillRectangle(new SolidBrush(Internal.AnsiColorsARGB(UseBackColID)), destRect);
					gr.DrawImage(m_FontImage, destRect, srcRectCR, GraphicsUnit.Pixel);

					CharXPos += m_Fontwidth;
				}
				CharYPos += m_FontHeight;
			}
			gr.Dispose();
			return bm;

		}
		public Bitmap DrawTextFromString(string stext)
		{
			string[] aworktxt = new string[] { "" };
			int iMaxWidth = 0;
			int iMaxHeight = 0;
			byte[] bte;
			bool bIsSmallFont = false;
			if (m_FontHeight == 8) {
				bIsSmallFont = true;
			}
			aworktxt = Split(stext, vbCrLf);
			for (int a = 0; a <= UBound(aworktxt); a++) {
				if (Len(aworktxt(a)) > iMaxWidth) {
					iMaxWidth = Len(aworktxt(a));
				}
			}
			iMaxHeight = (UBound(aworktxt) + 1);
			for (int a = 0; a <= UBound(aworktxt); a++) {
				if (Len(aworktxt(a)) < iMaxWidth) {
					aworktxt(a) += Space(iMaxWidth - Len(aworktxt(a)));
				}
			}
			bte = ConverterSupport.StringToByteArray(Join(aworktxt, ""));


			return ConverterSupport.ByteArrayToBitmap(bte, iMaxWidth, iMaxHeight, bIsSmallFont);
		}

	}
}