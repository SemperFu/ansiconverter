using Internal;
using System.Drawing;
using static Data;

namespace MediaFormats
{
    public class IMG
    {
        //Friend DosFontSml As New FontDef(8, 8, 0, 0, 0, 0, -1, 0, 256, 0, 255, Color.Black.ToArgb, Color.FromArgb(255, 168, 168, 168).ToArgb)
        //Friend DosFont As New FontDef(8, 16, 0, 0, 0, 0, -1, 1, 32, 1, 255, Color.Black.ToArgb, Color.FromArgb(255, 168, 168, 168).ToArgb)
        public Bitmap CreateImageFromASCII(string NFOText, int TextColor = new int(), int BackColor = new int())
        {
            InternalConstants.m_NFOText = NFOText;
            if (InternalConstants.m_NFOText == "")
                InternalConstants.m_NFOText = "THIS IS A TEST.";

            if (pSmallFont)
            {
                InternalConstants.m_NFOTextImg = DosFnt80x50.DrawTextFromString(InternalConstants.m_NFOText);
            }
            else
            {
                InternalConstants.m_NFOTextImg = DosFnt80x25.DrawTextFromString(InternalConstants.m_NFOText);
            }
            return InternalConstants.m_NFOTextImg;
        }

        public Bitmap CreateImageFromScreenChars()
        {
            InternalConstants.m_NFOTextImg = ConverterSupport.Convert.ScreenToBitmap(pSmallFont, pNoColors);
            //If pSmallFont Then
            // m_NFOTextImg = DosFontSml.DrawText(False)
            // Else
            // m_NFOTextImg = DosFont.DrawText(False)
            // End If
            return InternalConstants.m_NFOTextImg;
        }
    }
}