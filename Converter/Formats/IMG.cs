using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.MarshalByRefObject;
using System.Runtime.InteropServices;
namespace MediaFormats
{


	public class IMG
	{

		//Friend DosFontSml As New FontDef(8, 8, 0, 0, 0, 0, -1, 0, 256, 0, 255, Color.Black.ToArgb, Color.FromArgb(255, 168, 168, 168).ToArgb)
		//Friend DosFont As New FontDef(8, 16, 0, 0, 0, 0, -1, 1, 32, 1, 255, Color.Black.ToArgb, Color.FromArgb(255, 168, 168, 168).ToArgb)
		public Bitmap CreateImageFromASCII(string NFOText, int TextColor = null, int BackColor = null)
		{

			Internal.m_NFOText = NFOText;
			if (Internal.m_NFOText == "")
				Internal.m_NFOText = "THIS IS A TEST.";

			if (pSmallFont) {
				Internal.m_NFOTextImg = DosFnt80x50.DrawTextFromString(Internal.m_NFOText);
			} else {
				Internal.m_NFOTextImg = DosFnt80x25.DrawTextFromString(Internal.m_NFOText);
			}
			return Internal.m_NFOTextImg;
		}

		public Bitmap CreateImageFromScreenChars()
		{
			Internal.m_NFOTextImg = ConverterSupport.ScreenToBitmap(pSmallFont, pNoColors);
			//If pSmallFont Then
			// m_NFOTextImg = DosFontSml.DrawText(False)
			// Else
			// m_NFOTextImg = DosFont.DrawText(False)
			// End If
			return Internal.m_NFOTextImg;
		}
	}
}