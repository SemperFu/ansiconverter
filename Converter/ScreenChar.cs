using Microsoft.VisualBasic;

namespace ConverterSupport
{
    public class ScreenChar
    {
        public int ForeColor;
        public int BackColor;
        public bool Blink;
        public int Bold;
        public bool Reversed;
        public string Chr;
        public byte DosChar;

        public ScreenChar()
        {
            this.Init(1);
        }

        public ScreenChar(int xPos)
        {
            this.Init(xPos);
        }

        public void Init(int xPos)
        {
            DosChar = 32;
            ForeColor = 7;
            BackColor = 0;
            Blink = false;
            Bold = 0;
            Reversed = false;
            Chr = Strings.Chr(32).ToString();
            if (Data.bHTMLEncode == true)
            {
                Chr = "&nbsp;";
            }
            if (xPos == 80)
            {
                Chr = Strings.Chr(255).ToString();
            }
        }
    }
}