using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System;
using Internal;
using Microsoft.VisualBasic;
using System.Linq;
using static Data;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.Interaction;

using Converter.Properties;
using System.Drawing;

namespace ConverterSupport
{


	/// <summary>
	/// Conversion Routines
	/// </summary>
	/// <remarks></remarks>
	public class Convert
	{

		//---------------------------------------------------------------------------------
		//Conversion Routine 
		//---------------------------------------------------------------------------------
		/// <summary>
		/// Converts a Single Unicode ASCII Code to DOS ASCII String
		/// </summary>
		/// <param name="iUChr">Unicode Character Value</param>
		/// <returns>String (ASCII)</returns>
		/// <remarks></remarks>
		public string UnicodeToAscii(int iUChr)
		{

			//Converts a Single Unicode ASCII Code to DOS ASCII String
			int a;
			string result = "";
			if ((int)iUChr < 128 & (int)iUChr > 0) {
				result = Strings.Chr(iUChr).ToString();
			} else {
				for (a = 128; a <= 255; a++) {
					if (iUChr.ToString() == InternalConstants.aUniCode[a]) {
						result = Strings.Chr(a).ToString();
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				System.Windows.Forms.Application.DoEvents();
				if (result == "") {
					//UnicodeToAscii = ChrW(iUChr)
					result = Strings.Space(1);
				}
			}
			return result;
		}
		/// <summary>
		///    Converts a Single DOS ASCII Code to a Unicode String
		/// </summary>
		/// <param name="iChr">ASCII Character Value (0-255)</param>
		/// <returns>Unicode String</returns>
		/// <remarks>If <see cref="bHTMLEncode"/> is set to 'True', Unicode String will be converted to HTML Entity value</remarks>
		public string AsciiToUnicode(int iChr)
		{
			//Converts a Single DOS ASCII Code to a Unicode String

			string result = "";
			int iUChr =  System.Convert.ToInt32(InternalConstants.aUniCode[iChr]);
			if (Data.bHTMLEncode == true & InternalConstants.aSpecH[iChr] != "") {
				result = InternalConstants.aSpecH[iChr];
			} else {
				result = Strings.ChrW(iUChr).ToString();
			}

			if (Data.bSanitize == true) {
				if (iChr < 32) {
					switch (iChr) {
						case 9:
							//Horizontal Tabulator (Tab)
							if (Data.bHTMLEncode == true) {
								result = Strings.Replace(Strings.Space(8), " ", "&nbsp;", 1, -1, CompareMethod.Text);
								//Replace with eight spaces, the default Tab-Stop for DOS
							} else {
								result = "        ";
								//Replace with eight spaces, the default Tab-Stop for DOS
							}
							//Line Feed (LF) 
							break;
						case 10:
							//okay 
							//Carriage Return (CR)
							break;
						case 13:
							//okay
							break;
						default:
							result = "";
							break;
					}
				}
			}
			return result;
		}
		/// <summary>
		///   Converts an entire DOS ASCII String to Unicode or HTML Encoded Unicode ASCII
		/// </summary>
		/// <param name="sInput">DOS Ascii String</param>
		/// <returns>Unicode or HTML Encoded String</returns>
		/// <remarks></remarks>
		public string convascuni(string sInput)
		{
			//Converts an entire DOS ASCII String to Unicode or HTML Encoded Unicode ASCII
			string swork = sInput;
			int a;
			string DestChr;
			//Take care of Ampersand first
			a = 38;
			DestChr = AscConv(a);
			swork = Strings.Replace(swork, Strings.Chr(a).ToString(), DestChr, 1, -1, CompareMethod.Binary);
			for (a = 1; a <= 255; a++) {
				if ((a < 48 | a > 57) & (a < 65 | a > 90) & (a < 97 | a > 122) & a != 35 & a != 59 & a != 38) {
					//Ignore 0-9, a-z, A-Z, # , ; and of course &  
					DestChr = AscConv(a);
					swork = Strings.Replace(swork, Strings.Chr(a).ToString(), DestChr, 1, -1, CompareMethod.Binary);
				}
			}
			System.Windows.Forms.Application.DoEvents();
			return swork;
		}
		//--------------------------------------------------------------
		/// <summary>
		/// Converts String of HTML Encoded Unicode Entities to Unicode String
		/// </summary>
		/// <param name="sInput">HTML Encoded String</param>
		/// <returns>Unicode String</returns>
		/// <remarks></remarks>
		public string convuniuni(string sInput)
		{
			string sWork = sInput;
			int iPos = 1;
			int iStart = 0;
			string sChar = "";
			int iEnd = 0;
			string sSearchStr = "";
			if (sWork.Length > 5) {
				while (Strings.InStr(iPos, sWork, "&#", CompareMethod.Text) > 0) {
					iStart = Strings.InStr(iPos, sWork, "&#", CompareMethod.Text);
					if (iStart > 0) {
						iStart += 2;
						iEnd = Strings.InStr(iPos, sWork, ";", CompareMethod.Text);
						if (iEnd > iStart) {
							sChar = Strings.Mid(sWork, iStart, iEnd - iStart);
							sSearchStr = "&#" + sChar + ";";
							if (Strings.LCase(Strings.Left(sChar, 1)) == "x") {
								sChar = Strings.Right(sChar, sChar.Length - 1);
								if (isHex(sChar)) {
									sWork = Strings.Replace(sWork, sSearchStr, Strings.ChrW((int)HexToDec(sChar)).ToString(), 1, -1, CompareMethod.Text);
									iStart -= 2;
								}
							} else {
								if (Information.IsNumeric(sChar)) {
									sWork = Strings.Replace(sWork, sSearchStr, Strings.ChrW(System.Convert.ToInt32(sChar)).ToString(), 1, -1, CompareMethod.Text);
									iStart -= 2;
								}
							}
						}
						iPos = iStart;
					}
				}
			}
			return sWork;
		}
		//&#(x[0-9A-F]{1,5}|[0-9]{1,5});
		/// <summary>
		/// Converts HTML Encoded Unicode String to DOS ASCII
		/// </summary>
		/// <param name="sInput"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public string convuniasc(string sInput)
		{
			//Converts HTML Encoded Unicode String to DOS ASCII
			string swork = sInput;
			int a;
			string sHex;
			string sChar;
			for (a = 128; a <= 255; a++) {
				sHex = "&#x" + (string)Conversion.Hex(InternalConstants.aUniCode[a]) + ";";
				sChar = "&#" + (string)InternalConstants.aUniCode[a] + ";";
				swork = Strings.Replace(swork, sHex, Strings.Chr(a).ToString(), 1, -1, CompareMethod.Text);
				swork = Strings.Replace(swork, sChar, Strings.Chr(a).ToString(), 1, -1, CompareMethod.Text);
			}
			System.Windows.Forms.Application.DoEvents();
			for (a = 32; a <= 255; a++) {
				if (InternalConstants.aSpecH[a] != "") {
					swork = Strings.Replace(swork, InternalConstants.aSpecH[a], Strings.Chr(a).ToString(), 1, -1, CompareMethod.Text);
				}
			}
			System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex("&#(?:x[0-9A-F]{1,5}|[0-9]{1,5});", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			swork = re.Replace(swork, " ", 1);

			System.Windows.Forms.Application.DoEvents();
			return swork;
		}
		/// <summary>
		/// Uses Global Settings "<see cref="bHTMLEncode"/>",if true, HTML Encoded String is returned, else the Unicode String
		/// and "<see cref="bSanitize"/>", if True, TABs are converted to Spaces and Control Chars &lt; 32 are being removed, except line breaks
		/// </summary>
		/// <param name="iChr"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public string AscConv(int iChr)
		{
			//Uses Global Settings "bHTMLEncode",if true, HTML Encoded String is returned, else the Unicode String
			//and "bSanitize", if True, TABs are converted to Spaces and Control Chars < 32 are being removed, except line breaks
			string sResult = "";
			string s4 = "";
			if (InternalConstants.aSpecH[iChr] != "") {
				//Return Unicode Character or Special HTML Entity value for Character
				sResult = (string) Interaction.IIf(Data.bHTMLEncode, InternalConstants.aSpecH[iChr], Strings.ChrW(System.Convert.ToInt32(InternalConstants.aUniCode[iChr])));
			} else {
				if (System.Convert.ToInt32(InternalConstants.aUniCode[iChr]) != iChr) {
					//Return Unicode Character or HTML Encoded Unicode Char
					sResult = (string)Interaction.IIf(Data.bHTMLEncode, "&#" + InternalConstants.aUniCode[iChr] + ";", Strings.ChrW(System.Convert.ToInt32(InternalConstants.aUniCode[iChr])));
				} else {
					//Keep Original
					sResult = Strings.Chr(iChr).ToString();
				}
			}
			if (Data.bSanitize == true) {
				if (iChr < 32) {
					switch (iChr) {
						case 9:
							//Horizontal Tabulator (Tab), Replace with eight spaces, the default Tab-Stop for DOS
							sResult = "        ";
							break;
							//Line Feed (LF) OKAY
						case 10:
							break;
							//Carriage Return (CR) OKAY
						case 13:
						default:
							//NOT OKAY
							sResult = "";
							break;
					}
				}
			}
			return sResult;
		}
		/// <summary>
		/// Convert a Single DOS ASCII String Character to Unicode or HTML Encoded Unicode
		/// </summary>
		/// <param name="schr"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public string seekascuni(string schr)
		{
			//Convert a Single DOS ASCII String Character to Unicode or HTML Encoded Unicode
			int iASC = Strings.Asc(schr);
			return AscConv(iASC);
		}


		/// <summary>
		/// Read SAUCE Meta Tag Content from Byte Array to Special Sauce Meta Class
		/// </summary>
		/// <param name="aAnsi">Byte Array</param>
		/// <param name="Offset">Offset Location</param>
		/// <returns><see cref="SauceMeta"/></returns>
		/// <remarks></remarks>
		SauceMeta ReadSauce(byte[] aAnsi, int Offset)
		{
			//Read SAUCE Meta Tag Content from Byte Array to Special Sauce Meta Class
			SauceMeta oSauce;
			int iLoop = Offset;
			string sStr = "";
			oSauce = new SauceMeta();
			oSauce.ID = ReadByteArray(ref aAnsi, 5, "s");
			oSauce.Version[0] = System.Convert.ToByte(ReadByteArray(ref aAnsi, 1, "b"));
			oSauce.Version[1] = System.Convert.ToByte(ReadByteArray(ref aAnsi, 1, "b"));
			oSauce.Title = ReadByteArray(ref aAnsi, 35, "s");
			oSauce.Author = ReadByteArray(ref aAnsi, 20, "s");
			oSauce.Group = ReadByteArray(ref aAnsi, 20, "s");
			oSauce.CreatedDate = ReadByteArray(ref aAnsi, 8, "s");
			oSauce.FileSize = (int)HexToDec(FlipHex(ReadByteArray(ref aAnsi, 4, "h")));
			oSauce.DataType = System.Convert.ToByte(ReadByteArray(ref aAnsi, 1, "b"));
			oSauce.FileType = System.Convert.ToByte(ReadByteArray(ref aAnsi, 1, "b"));
			oSauce.TInfo1 = (short)HexToDec(FlipHex(ReadByteArray(ref aAnsi, 2, "h")));
			oSauce.TInfo2 = (short)HexToDec(FlipHex(ReadByteArray(ref aAnsi, 2, "h")));
			oSauce.TInfo3 = (short)HexToDec(FlipHex(ReadByteArray(ref aAnsi, 2, "h")));
			oSauce.TInfo4 = (short)HexToDec(FlipHex(ReadByteArray(ref aAnsi, 2, "h")));
			oSauce.Comments = System.Convert.ToByte(ReadByteArray(ref aAnsi, 1, "b"));
			oSauce.Flags = System.Convert.ToByte(ReadByteArray(ref aAnsi, 1, "b"));
			for (int x = 0; x <= 21; x++) {
				oSauce.Filler[x] = System.Convert.ToByte(ReadByteArray(ref aAnsi, 1, "b"));
			}
			if (Information.UBound(aAnsi) >= iLoop + (oSauce.Comments * 64) + 5) {
				if (oSauce.Comments != 0) {
					sStr = ReadByteArray(ref aAnsi, 5, "s");
					if (sStr == "COMNT") {
						oSauce.SetComments((int)oSauce.Comments);
						for (int iLoop2 = 1; iLoop2 <= oSauce.Comments; iLoop2++) {
							oSauce.aComments[iLoop2] = ReadByteArray(ref aAnsi, 64, "s");
						}
					}
				}
			}
			return oSauce;
		}
		/// <summary>
		/// Resizes Array of <see cref="ScreenChar"/> to new Width and Height
		/// </summary>
		/// <param name="aArr">2 Dimensional Array of <see cref="ScreenChar"/></param>
		/// <param name="iWidth">New Width</param>
		/// <param name="iHeight">New Height</param>
		/// <returns>2 Dimensional Array of <see cref="ScreenChar"/></returns>
		/// <remarks></remarks>
		public ScreenChar[,] ResizeScreen(ScreenChar[,] aArr, int iWidth, int iHeight)
		{
			ScreenChar[,] aNewScr = new ScreenChar[iWidth, iHeight];
			for (int X = Data.minX; X <= iWidth; X++) {
				for (int y = Data.minY; y <= iHeight; y++) {
					if (X <= Information.UBound(aArr, 1) & y <= Information.UBound(aArr, 2)) {
						aNewScr[X, y] = aArr[X, y];
					} else {
						aNewScr[X, y] = new ScreenChar();
					}
				}
			}
			return aNewScr;
		}
		/// <summary>
		/// Reads specified <paramref>iLen</paramref> from Byte Array <paramref>Arr</paramref> and returns results depending on <paramref>DtaType</paramref>
		/// </summary>
		/// <param name="Arr">Byte Array</param>
		/// <param name="iLen">Number of Bytes to Read (always set to 1 if <paramref>DtaType</paramref> = 'b' (Byte))</param>
		/// <param name="DtaType">'b' = Byte, 's' = ASCII String, 'h' = Hex String</param>
		/// <returns>String or Byte</returns>
		/// <remarks>Always starts at position 1 of <paramref>Arr</paramref></remarks>
		string ReadByteArray(ref byte[] Arr, int iLen, string DtaType)
		{
			int a;
			string sRes = "";
			if (DtaType == "b") {
				iLen = 1;
			}
			for (a = 1; a <= iLen; a++) {
				if (Information.UBound(Arr) >= Data.iLoop) {
					switch (DtaType) {
						case "s":
							sRes = sRes + Strings.Chr(Arr[Data.iLoop]);
							break;
						case "h":
							sRes = sRes + Strings.Right("0" + Conversion.Hex(Arr[Data.iLoop]), 2);
							break;
						case "b":
							sRes = Arr[Data.iLoop].ToString();
							break;
					}
				}
				Data.iLoop = Data.iLoop + 1;
				if (a / 100 == (int)a / 100)
					System.Windows.Forms.Application.DoEvents();
			}
			return sRes;
		}
		//----------------------------------------------------
		/// <summary>
		/// Converts <see cref="Screen"/> Object to Byte Array
		/// </summary>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public byte[] ANSIScreenToASCIIByteArray()
		{
			byte[] bte;
			int cnt = 0;
			// ERROR: Not supported in C#: ReDimStatement


			Data.XPos = 1;
			Data.YPos = 1;
			Data.ForeColor = 7;
			Data.BackColor = 0;
			for (int y = 1; y <= Data.LinesUsed; y++) {
				for (int x = 1; x <= 79; x++) {
					// If x = 1 Then
					if (Data.Screen[x, y].Chr != Strings.Chr(0).ToString()) {
						if (Data.Screen[x, y].Chr == "-1") {
							break; // TODO: might not be correct. Was : Exit For
						} else {
							if (Data.Screen[x, y].Chr != Strings.Chr(255).ToString()) {
								cnt += 1;
								bte[cnt] = System.Convert.ToByte(Strings.Asc(Data.Screen[x, y].Chr));
							} else {
								cnt += 1;
								bte[cnt] = 32;
							}
						}
					} else {
						break; // TODO: might not be correct. Was : Exit For
					}
					//End If
				}
				cnt += 1;
				bte[cnt] = 13;
				cnt += 1;
				bte[cnt] = 10;
				if (y / 100 == (int)y / 100)
					System.Windows.Forms.Application.DoEvents();
			}

			Array.Resize(ref bte, cnt + 1);
			return bte;

		}
		//----------------------------------------------------
		/// <summary>
		/// Sets current character in <see cref="Screen"/> Object at position X: <see cref="XPos"/>, Y: <see cref="YPos"/>
		/// Sets also properties <see cref="BackColor"/>, <see cref="ForeColor"/>, <see cref="Bold"/>, <see cref="Blink"/> and <see cref="Reversed"/>
		/// </summary>
		/// <param name="sChar">String Representation of Character to Set</param>
		/// <returns>True, if character was set successfully</returns>
		/// <remarks>Always adjusts values of <see cref="XPos"/> and if <see cref="maxX"/> was reached also <see cref="YPos"/>. May also adjusts global variables for <see cref="LinesUsed"/> and <see cref="Yoffset"/> if necessary</remarks>
		public bool SetChar(string sChar)
		{
			//Set Character in Global "Screen" Array at the Position of the global Loop Counter variable "iLoop"
			//Character is automatically converted to unicode, if global parameter "bConv2Unicode" is set to "True"
			//If global paramter "bHTMLEncode" is set to "True", the HTML Encoded version of the Unicode character is saved instead
			//Updates Global Variables: iLoop, XPos, YPos, LinesUsed, Screen
			//Also uses Global Variables: ForeColor, BackColor, Blink, Intense and Reversed; and Constants minX, maxX and maxY
			//Returns FALSE, if YPos reached its maximum
			// cBPF += 1
			bool bIncr = true;
			bool bReturn;
			bReturn = true;
			if (Data.XPos >= Data.minX & Data.XPos <= Data.maxX) {
				if ((string)sChar == "-1" | (string)sChar == "-2" | (string)sChar == "-3") {
					switch ((string)sChar) {
						case "-1":
							Data.Screen[Data.XPos, Data.YPos] = new ScreenChar(Data.XPos);
							break;
						case "-2":
						case "-3":
							bIncr = false;
							break;
					}
				} else {
					if (sChar.Length == 1) {
						Data.Screen[Data.XPos, Data.YPos].DosChar = System.Convert.ToByte(Strings.Asc(sChar));
					}
					Data.Screen[Data.XPos, Data.YPos].ForeColor = Data.ForeColor;
					Data.Screen[Data.XPos, Data.YPos].BackColor = Data.BackColor;
					Data.Screen[Data.XPos, Data.YPos].Bold = Data.Bold;
					if (Data.bConv2Unicode == true) {
						Data.Screen[Data.XPos, Data.YPos].Chr = (string)Interaction.IIf(Data.bHTMLEncode, Strings.Replace(seekascuni(sChar), " ", "&nbsp;", 1, -1, CompareMethod.Text), seekascuni(sChar));
					} else {
						Data.Screen[Data.XPos, Data.YPos].Chr = sChar;
					}
					Data.Screen[Data.XPos, Data.YPos].Blink = Data.Blink;
					Data.Screen[Data.XPos, Data.YPos].Bold = Data.Bold;
					Data.Screen[Data.XPos, Data.YPos].Reversed = Data.Reversed;
				}
			}
			if (bIncr) {
				Data.XPos += 1;
				if (Data.XPos > Data.maxX) {
					Data.YPos += 1;
					if (Data.YPos > Data.maxY - 1) {
						Data.YPos = Data.maxY - 1;
						bReturn = false;
					} else {
						Data.XPos = 1;
					}
				}
				if (Data.YPos > Data.LinesUsed) {
					if (Data.YPos > Data.LinesUsed) {
						if (Data.LinesUsed > 25) {
							Data.Yoffset += (Data.YPos - Data.LinesUsed);
						}
						Data.LinesUsed = Data.YPos;
					}
				}
				return bReturn;
			} else {
				Data.XPos -= 1;
				if (Data.XPos < Data.minX) {
					Data.YPos -= 1;
					if (Data.YPos < Data.minY) {
						Data.YPos = Data.minY;
						Data.XPos = Data.minX;
						bReturn = false;
					} else {
						Data.XPos = Data.maxX;
					}
				}
				if (Data.YPos < Data.Yoffset) {
					Data.Yoffset = Data.YPos - 1;
				}
				return bReturn;
			}

		}

		//===========================================================
		//Generic Conversion Functions
		//===========================================================
		/// <summary>
		/// Checks if <paramref>iVal</paramref> = 0 and returns 1 instead. <paramref>iVal</paramref> is returned unchanged, if it is not equal 0
		/// </summary>
		/// <param name="iVal">Value to check</param>
		/// <returns>1 or <paramref>iVal</paramref></returns>
		/// <remarks></remarks>
		public int NotZero(int iVal)
		{
			return (int) Interaction.IIf(iVal == 0, 1, iVal);
		}
		//----------------------------------------------------
		/// <summary>
		/// Checks if <paramref>iVal</paramref> is Numeric or Not. If it is not numeric, 0 is returned, otherwise <paramref>iVal</paramref> converted to Integer is returned instead
		/// </summary>
		/// <param name="iVal">Value (as Object) to check</param>
		/// <returns>Integer Value</returns>
		/// <remarks></remarks>
		public int ChkNum(object iVal)
		{
			if (Strings.Trim((string)iVal) != "" & !(iVal == null) & Information.IsNumeric(iVal)) {
				return (int)iVal;
			} else {
				return 0;
			}
		}
		/// <summary>
		/// Converts Integer Value to Hex String of 4 Characters Length (representing a 2 bytes Word value)
		/// </summary>
		/// <param name="iNum">16 Bit Integer Value</param>
		/// <returns>Hex String e.g. 000F</returns>
		/// <remarks></remarks>
		public string UniHex(int iNum)
		{
			return Strings.Right("0000" + Decimal2BaseN(iNum, 16), 4);
		}

		//--------------------------------------------------------------
		/// <summary>
		/// Converts Integer value of Base 10 to new value of Base <paramref>outBase</paramref>
		/// </summary>
		/// <param name="value">Integer Value</param>
		/// <param name="outBase">Base e.g. 2 for Binary, 16 for Hex etc.</param>
		/// <returns>Converted Value as String representation</returns>
		/// <remarks></remarks>
		public string Decimal2BaseN(int value, int outBase)
		{
			int quotient;
			object reminder;
			int denominator;
			string result = "";
			denominator = outBase;
			quotient = value;
			do {
				reminder = quotient % denominator;
				quotient = (int)Math.Floor((double)(quotient / denominator));
				if ((int)reminder >= 10) {
					reminder = Strings.Chr(65 + ((int)reminder - 10)).ToString();
				}
				result += reminder.ToString();
			} while (!(quotient == 0));
			System.Windows.Forms.Application.DoEvents();
			return Strings.StrReverse(result);
		}

		/// <summary>
		/// Converts Hex String between Little Endian and Big Endian Representation or the other way around
		/// </summary>
		/// <param name="sHexStr">Hex value as String</param>
		/// <returns>Hex String reverted</returns>
		/// <remarks></remarks>
		public string FlipHex(string sHexStr)
		{
			//(hex)
			int a;
			string sResult = "";
			if (sHexStr.Length % 2 != 0) {
				return sHexStr;
			}
			for (a = sHexStr.Length - 1; a >= 1; a += -2) {
				sResult += Strings.Mid(sHexStr, a, 2);
			}
			System.Windows.Forms.Application.DoEvents();
			return sResult;
		}
		/// <summary>
		/// Converts a string value to a "Byte Array"
		/// </summary>
		/// <param name="str">string</param>
		/// <param name="enc">optional encoding format as <see cref="FFormats"/>, Default is 'FFormats.us_ascii'</param>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public byte[] StrToByteArray(string str, FFormats enc = FFormats.us_ascii)
		{
			switch (enc) {

				case FFormats.utf_7:
					System.Text.UTF7Encoding u7encoding = new System.Text.UTF7Encoding();
					return u7encoding.GetBytes(str);
				case FFormats.utf_8:
					System.Text.UTF8Encoding u8encoding = new System.Text.UTF8Encoding();
					return u8encoding.GetBytes(str);
				case FFormats.utf_16:
					System.Text.UnicodeEncoding u16encoding = new System.Text.UnicodeEncoding();
					return u16encoding.GetBytes(str);
				case FFormats.utf_32:
					System.Text.UTF32Encoding u32encoding = new System.Text.UTF32Encoding();
					return u32encoding.GetBytes(str);

				case FFormats.us_ascii:
				//System.Text.ASCIIEncoding aencoding = new System.Text.ASCIIEncoding();
				//return encoding.GetBytes(str);
				default:
					System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
					return encoding.GetBytes(str);
			}

		}
		//StrToByteArray

		/// <summary>
		/// Converts a Byte Array back to a string value
		/// </summary>
		/// <param name="ByteArray">Byte Array</param>
		/// <param name="enc">optional encoding format as <see cref="FFormats"/>, Default is 'FFormats.us_ascii'</param>
		/// <returns>String value</returns>
		/// <remarks></remarks>
		public string ByteArrayToStr(byte[] ByteArray, FFormats enc = FFormats.us_ascii)
		{
			switch (enc) {
				
				case FFormats.utf_7:
					System.Text.UTF7Encoding u7encoding = new System.Text.UTF7Encoding();
					return u7encoding.GetString(ByteArray);
				case FFormats.utf_8:
					System.Text.UTF8Encoding u8encoding = new System.Text.UTF8Encoding();
					return u8encoding.GetString(ByteArray);
				case FFormats.utf_16:
					System.Text.UnicodeEncoding u16encoding = new System.Text.UnicodeEncoding();
					return u16encoding.GetString(ByteArray);
				case FFormats.utf_32:
					System.Text.UTF32Encoding u32encoding = new System.Text.UTF32Encoding();
					return u32encoding.GetString(ByteArray);
				case FFormats.us_ascii:
					//System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
					//return encoding.GetString(ByteArray);
				default:
					System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
					return encoding.GetString(ByteArray);
			}
			//str = encoding.GetString(ByteArray)
		}
		/// <summary>
		/// Alternative Conversion function of Byte Array to ASCII String
		/// </summary>
		/// <param name="ByteArray">Byte Array</param>
		/// <returns>String</returns>
		/// <remarks></remarks>
		public string ByteArrayToString(byte[] ByteArray)
		{
			string result = "";
			for (int a = 0; a <= ByteArray.Length - 1; a++) {
				if (ByteArray[a] != 0) {
					result += Strings.Chr(ByteArray[a]);
				}
				if (a / 1000 == (int)a / 1000)
					System.Windows.Forms.Application.DoEvents();
			}
			return result;
		}
		/// <summary>
		/// Alternative Conversion Function from ASCII String to Byte Array 
		/// </summary>
		/// <param name="sStr">ASCII String</param>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public byte[] StringToByteArray(string sStr)
		{
			byte[] bte = new byte[] { 0 };
			if (sStr.Length > 0) {
				 // ERROR: Not supported in C#: ReDimStatement

				for (int a = 1; a <= sStr.Length; a++) {
					bte[a - 1] = System.Convert.ToByte(Asc(Mid(sStr, a, 1)));
					if (a / 1000 == (int)a / 1000)
						System.Windows.Forms.Application.DoEvents();
				}
			}
			return bte;
		}
		/// <summary>
		/// Converts String of Hex Values to Byte Array
		/// </summary>
		/// <param name="sHexStr">Hex Value String</param>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public byte[] HexStringToByteArray(string sHexStr)
		{
			int a;
			byte[] dByte = new byte[]();
			if (sHexStr.Length % 2 != 0) {
				return null;
			
			}
			 // ERROR: Not supported in C#: ReDimStatement

			if (sHexStr.Length > 2) {
				for (a = 1; a <= sHexStr.Length - 1; a += 2) {
					dByte[(a - 1) / 2] = (byte)HexToDec(Strings.Mid(sHexStr, a, 2));
					if (a / 1000 == (int)a / 1000)
						System.Windows.Forms.Application.DoEvents();
				}
			} else {
				dByte[0] = (byte)HexToDec(sHexStr);
			}
			return dByte;
		}

		/// <summary>
		/// Merges Byte Array <paramref>bArr1</paramref> with Byte Array <paramref>bArr2</paramref> to new Byte Array
		/// </summary>
		/// <param name="bArr1">Byte Array</param>
		/// <param name="bArr2">Byte Array</param>
		/// <returns>Combined Byte Array</returns>
		/// <remarks></remarks>
		public static byte[] MergeByteArrays(byte[] bArr1, byte[] bArr2)
		{
			int iDim1 = 0;
			int iDim2 = 0;
			int iDimOut = 0;
			int i = 0;
			byte[] bArrOut;
			iDim1 = Information.UBound(bArr1);
			iDim2 = Information.UBound(bArr2);
			iDimOut = iDim1 + iDim2 + 1;
			 // ERROR: Not supported in C#: ReDimStatement

			for (i = 0; i <= iDim1; i++) {
				bArrOut[i] = bArr1[i];
				if (i / 1000 == (int)i / 1000)
					System.Windows.Forms.Application.DoEvents();
			}
			for (i = 0; i <= iDim2; i++) {
				bArrOut[iDim1 + 1 + i] = bArr2[i];
				if (i / 1000 == (int)i / 1000)
					System.Windows.Forms.Application.DoEvents();
			}
			return bArrOut;
		}

		/// <summary>
		/// Returns a Byte Array of Length = 1 with value 0
		/// </summary>
		/// <returns>Byte() = New Byte(){0}</returns>
		/// <remarks></remarks>
		public byte[] NullByteArray()
		{
			byte[] aByte = { 0 };
			return aByte;
		}

		/// <summary>
		/// Returns a Byte Array initialized as 0 of specified size <paramref>numb</paramref>
		/// </summary>
		/// <param name="numb">Size of Byte Array</param>
		/// <returns>0 initialized Byte Array of length <paramref>numb</paramref></returns>
		/// <remarks></remarks>
		public byte[] NullByteArray(int numb)
		{
			int i = 0;
			byte[] bArrOut = null; //declar
			 // ERROR: Not supported in C#: ReDimStatement

			for (i = 0; i <= numb - 1; i++) {
				bArrOut[i] = 0;
				if (i / 1000 == (int)i / 1000)
					System.Windows.Forms.Application.DoEvents();
			}
			return bArrOut;
		}

		/// <summary>
		/// Converts Hex String to 64 bit Integer Value
		/// </summary>
		/// <param name="strHex">Hex Values String</param>
		/// <returns>Int64 Integer (Long)</returns>
		/// <remarks></remarks>
		public Int64 HexToDec(string strHex)
		{
			Int64 lngResult = 0;
			int intIndex;
			string strDigit;
			Int64 intDigit;
			Int64 intValue;
			for (intIndex = strHex.Length; intIndex >= 1; intIndex += -1) {
				strDigit = Strings.Mid(strHex, intIndex, 1);
				intDigit = Strings.InStr("0123456789ABCDEF", strDigit.ToUpper()) - 1;
				if (intDigit >= 0) {
					//intValue = intDigit * (Math.Pow(16, (strHex.Length - (long)intIndex)));
					intValue = intDigit * (16 ^ (strHex.Length - (long)(intIndex)));

					lngResult = lngResult + intValue;
				} else {
					lngResult = 0;
					intIndex = 0;
					// stop the loop
				}
			}
			return lngResult;
		}

		/// <summary>
		///The ClearBit Sub clears the 1 based, nth bit (<paramref>MyBit</paramref>) of a Byte (<paramref>MyByte</paramref>).
		/// </summary>
		/// <param name="MyByte">Byte where Bit is to be cleared</param>
		/// <param name="MyBit">Bit to be cleared (1-8)</param>
		/// <remarks>Note: Bit Location is 1 based and not 0</remarks>
		public void ClearBit(ref byte MyByte, byte MyBit)
		{
			Int16 BitMask;
			MyByte = System.Convert.ToByte(MyByte & 0xff);
            // Create a bitmask with the 2 to the nth power bit set:
            BitMask = (short)(2 ^ (MyBit - 1));
            // Clear the nth Bit:
            MyByte = MyByte & !BitMask;
		}

		/// <summary>
		/// The ExamineBit function will return True or False depending on the value of the 1 based, nth bit (<paramref>MyBit</paramref>) of a Byte (<paramref>MyByte</paramref>).
		/// </summary>
		/// <param name="MyByte">Byte to Check</param>
		/// <param name="MyBit">Bit to Check</param>
		/// <returns>True or False</returns>
		/// <remarks>Note: Bit Location is 1 based and not 0</remarks>
		public bool ExamineBit(byte MyByte, byte MyBit)
		{
			Int16 BitMask;
			MyByte = System.Convert.ToByte(MyByte & 0xff);
			BitMask = System.Convert.ToInt16(Math.Pow(2, (MyBit - 1)));
			return ((MyByte & BitMask) > 0);
		}

		/// <summary>
		///   The SetBit Sub will set the 1 based, nth bit (<paramref>MyBit</paramref>) of a Byte (<paramref>MyByte</paramref>).
		/// </summary>
		/// <param name="MyByte">Byte (passed as Reference)</param>
		/// <param name="MyBit">Bit Position to set</param>
		/// <remarks>Note: Bit Location is 1 based and not 0</remarks>
		public void SetBit(ref byte MyByte, byte MyBit)
		{
			Int16 BitMask;
			MyByte = System.Convert.ToByte(MyByte & 0xff);
			BitMask = System.Convert.ToInt16(Math.Pow(2, (MyBit - 1)));
			MyByte = System.Convert.ToByte(MyByte | BitMask);
		}

		/// <summary>
		///  The ToggleBit Sub will change the state of the 1 based, nth bit (<paramref>MyBit</paramref>) of a Byte (<paramref>MyByte</paramref>).
		/// </summary>
		/// <param name="MyByte">Byte to Change (passed as Reference)</param>
		/// <param name="MyBit">Bit Position to Toggle</param>
		/// <remarks>Note: Bit Location is 1 based and not 0</remarks>
		public void ToggleBit(ref byte MyByte, byte MyBit)
		{
			Int16 BitMask;
			MyByte = System.Convert.ToByte(MyByte & 0xff);
			BitMask = System.Convert.ToInt16(Math.Pow(2, (MyBit - 1)));
			MyByte = System.Convert.ToByte(MyByte ^ BitMask);
		}
		/// <summary>
		/// Returns en-US based <see cref="System.Globalization.CultureInfo"/> with <paramref>NumDigits</paramref> decimal digits
		/// </summary>
		/// <param name="NumDigits">Number of Decimal Digits</param>
		/// <returns><see cref="System.Globalization.CultureInfo"/> of 'en-US' with <paramref>NumDigits</paramref> decimal digits, '.' as Decimal Separator and ',' as Group Separator</returns>
		/// <remarks></remarks>
		public System.Globalization.CultureInfo SetCultureInfo(int NumDigits)
		{
			System.Globalization.CultureInfo nfio = new System.Globalization.CultureInfo("en-US");
			nfio.NumberFormat.NumberDecimalDigits = NumDigits;
			nfio.NumberFormat.NumberDecimalSeparator = ".";
			nfio.NumberFormat.NumberGroupSeparator = ",";

			return nfio;
		}
		/// <summary>
		/// Rounds <paramref>Value</paramref> to <paramref>numdigits</paramref> decimal places and returns formatted value as string (using <see cref="System.Globalization.CultureInfo"/> = 'en-US' for formatting)
		/// </summary>
		/// <param name="Value">Floating Point Value</param>
		/// <param name="numdigits">Numer of Decimal Point to round to</param>
		/// <returns>Formated Value as String</returns>
		/// <remarks></remarks>
		public string USStringRound(float Value, int numdigits)
		{
			System.Globalization.CultureInfo USNum = new System.Globalization.CultureInfo("en-US", false);
			return Math.Round((float)Value.ToString("F", SetCultureInfo(numdigits)), numdigits).ToString(USNum);
		}


		/// <summary>
		/// Regular Expression Replace function
		/// </summary>
		/// <param name="sStr">String to Evaluate and Parse</param>
		/// <param name="sRet">String with the Replacement Value/Pattern</param>
		/// <param name="sPattern">Regular Expression Pattern</param>
		/// <param name="RegExopt">RegEx Options</param>
		/// <param name="isGlobal">Global Replace True/False (False = only replaces first found match, True = replace all matches in string</param>
		/// <returns>String with replaced content</returns>
		/// <remarks></remarks>
		public string RegExReplace(string sStr, string sRet, string sPattern, System.Text.RegularExpressions.RegexOptions RegExOpt = 0, bool isGlobal = true)
		{

			Regex re = new Regex(sPattern, RegExOpt);
			string sData;
			if (isGlobal == true) {
				sData = re.Replace(sStr, sRet);
			} else {
				sData = re.Replace(sStr, sRet, 1);
			}
			return sData;
		}
		/// <summary>
		/// Regular Expression Test Function to check if a string matches a provided RegEx pattern or not
		/// </summary>
		/// <param name="sStr">String to Evaluate</param>
		/// <param name="sPattern">Regular Expression Pattern</param>
		/// <param name="RegExopt">RegEx Options</param>
		/// <returns>True is pattern matches, False if it does not</returns>
		/// <remarks></remarks>
		public bool RegExTest(string sStr, string sPattern, System.Text.RegularExpressions.RegexOptions RegExOpt = 0)
		{
			if (RegExOpt && RegexOptions.Multiline) {
				sStr = Strings.Replace(sStr, Environment.NewLine, "\n", 1, -1, CompareMethod.Text);
			}
			Regex re = new Regex(sPattern, RegExOpt);
			return re.IsMatch(sStr);
		}
		/// <summary>
		/// Check function to verify that string only contains digits 0-9 and no other characters
		/// </summary>
		/// <param name="Value">String value to check</param>
		/// <returns>True, if <paramref>Value</paramref> only contains digits, else False</returns>
		/// <remarks></remarks>
		public bool isInt(string Value)
		{
			return RegExTest(Value, "^[\\d]{1,*}$");
		}
		/// <summary>
		/// Flexible 'CUT OUT' and 'SEARCH &amp; REPLACE' Function with multiple options
		/// </summary>
		/// <param name="SInp">Input String</param>
		/// <param name="sFrom">Start Location String if <paramref>sMode</paramref> = 'C' (Special Value of 'A' for 'Beginning of String' <paramref>sInp</paramref>) or String to Find and Replace, if <paramref>sMode</paramref> = 'R'</param>
		/// <param name="sTo">End Location String if <paramref>sMode</paramref> = 'C' (Special Value of 'Z' for 'End of String' <paramref>sInp</paramref>) or Replacement String, if <paramref>sMode</paramref> = 'R'</param>
		/// <param name="sFromIE">Only relevant for <paramref>sMode</paramref> = 'C'. 'I' = Include <paramref>sFrom</paramref> Value in Cut-Out Results, 'E' = Exlcude <paramref>sFrom</paramref> Value.</param>
		/// <param name="sToIE">Only relevant for <paramref>sMode</paramref> = 'C'. 'I' = Include <paramref>sTo</paramref> Value in Cut-Out Results, 'E' = Exlcude <paramref>sTo</paramref> Value</param>
		/// <param name="sMode">'R' = Replace, 'C' = Cut out</param>
		/// <param name="Num">Number of Times to Execute on Results, '' = Unlimited or <paramref>Num</paramref> > 0 to limit loop</param>
		/// <returns>Processed String Result</returns>
		/// <remarks></remarks>
		public string CutorSandR(string SInp, string sFrom, string sTo, string sFromIE, string sToIE, string sMode, string Num)
		{

			string sPart1;
			string sPart2;
			bool bFound;
			int iFrom;
			int iTo;
			int iCount;
			bool Ende;
			string sTemp;
			//123456789
			//1AB45CD89
			//AB - CD
			//
			//InStr
			//AB  2
			//CD  6

			//Right CD
			//I Len(Str) - (InStr(CD) + Len(CD) - 1)
			//E Len(str) - (InStr(CD) - 1)
			//Left AB
			//I InStr(AB) - 1
			//E InStr(AB) + Len(AB) - 1

			iCount = 0;
			if (sMode.ToUpper() == "R") {
				if (Num != "" & Information.IsNumeric(Num)) {
					SInp = Strings.Replace(SInp, sFrom, sTo, 1, System.Convert.ToInt32(Num), CompareMethod.Text);
				} else {
					SInp = Strings.Replace(SInp, sFrom, sTo, 1, -1, CompareMethod.Text);
				}
			} else {
				if (sFromIE.ToUpper() == "A") {
					iFrom = 1;
					sFromIE = "I";
				} else {
					iFrom = Strings.InStr(1, SInp, sFrom, CompareMethod.Text);
				}
				if (iFrom > 0) {
					Ende = false;
				} else {
					Ende = true;
				}

				while (Ende == false) {
					iCount = iCount + 1;
					if (Num != "" & Information.IsNumeric(Num)) {
						if (iCount.ToString() == Num) {
							Ende = true;
						}
					}
					bFound = false;
					sPart1 = "";
					sPart2 = "";
					if (iFrom > 0) {
						//Incl
						if (sFromIE.ToUpper() == "I") {
							if (iFrom - 1 == 0) {
								sPart1 = "";
							} else {
								sPart1 = Strings.Left(SInp, iFrom - 1);
							}
						//Excl
						} else {
							sPart1 = Strings.Left(SInp, iFrom + sFrom.Length - 1);
						}
						sTemp = Strings.Right(SInp, SInp.Length - (iFrom + sFrom.Length - 1));
						if (sToIE.ToUpper() == "Z") {
							iTo = sTemp.Length + 1;
							sToIE = "I";
						} else {
							iTo = Strings.InStr(1, sTemp, sTo, CompareMethod.Text);
						}
						//sTemp = right(SInp,len(SInp)- instr(1,SInp,sFrom,1))
						if (iTo > 0) {
							bFound = true;
							//Incl
							if (sToIE.ToUpper() == "I") {
								if (sTemp.Length - (iTo + sTo.Length - 1) == 0) {
									sPart2 = "";
								} else {
									sPart2 = Strings.Right(sTemp, sTemp.Length - (iTo + sTo.Length - 1));
								}
							//Excl
							} else {
								sPart2 = Strings.Right(sTemp, sTemp.Length - (iTo - 1));
							}
						} else {
							Ende = true;
						}
					} else {
						Ende = true;
					}
					if (bFound == true) {
						SInp = sPart1 + sPart2;
					}
					iFrom = Strings.InStr(1, SInp, sFrom, CompareMethod.Text);
					System.Windows.Forms.Application.DoEvents();
				}
			}
			return SInp;
		}
		/// <summary>
		/// Returns 'True', if <paramref>sChar</paramref> is a valid Hex Values String (only containing 0-9 and A-F)
		/// </summary>
		/// <param name="sChar">String to Check</param>
		/// <returns>True if Hex Value String, False, if not</returns>
		/// <remarks></remarks>
		public bool isHex(string sChar)
		{
			bool bResult = true;
			string sChr = "";
			if (sChar.Length == 0) {
				return false;
			}
			for (int x = 0; x <= sChar.Length - 1; x++) {
				sChr = sChar.ToUpper().Substring(x, 1);
				if ((Strings.Asc(sChr) < 48 | Strings.Asc(sChr) > 57) & (Strings.Asc(sChr) < 65 | Strings.Asc(sChr) > 70)) {
					bResult = false;
				}
			}
			return bResult;
		}
		/// <summary>
		/// Converts ASCII String to Hex Values String
		/// </summary>
		/// <param name="sStr">ASCII String to Convert</param>
		/// <param name="sep">Optional Separator between each Character, default = ''</param>
		/// <returns>String of Hex Values separated by <paramref>sep</paramref> </returns>
		/// <remarks><paramref>sep</paramref> can be used to create for example a comma separated list of Hex values etc.</remarks>
		public object StringToHex(string sStr, string sep = "")
		{
			//as String
			//sStr as String
			int a;
			string sResult;
			sResult = "";
			for (a = 1; a <= sStr.Length; a++) {
				sResult += Strings.Right("0" + (string)Conversion.Hex(Strings.Asc(Strings.Mid(sStr, a, 1))), 2);
				if (a < sStr.Length) {
					sResult += sep;
				}
			}
			return (string)sResult;
		}
		/// <summary>
		/// Converts Integer number to Binary Value as String e.g. 0 to '00000000'
		/// </summary>
		/// <param name="IntegerNumber">Integer Value</param>
		/// <returns>Binary Value as String</returns>
		/// <remarks></remarks>
		public string Int2Bin(int IntegerNumber)
		{
			int IntNum;
			int TempValue;
			string BinValue = "";

			IntNum = IntegerNumber;
			do {
				//Use the Mod operator to get the current binary digit from the
				//Integer number
				TempValue = IntNum % 2;
				BinValue = TempValue.ToString() + BinValue;

				//Divide the current number by 2 and get the integer result
				IntNum = IntNum / 2;
			} while (!(IntNum == 0));

			return BinValue;

		}
		//-----------------------------------------------------------------------      
		/// <summary>
		/// Converts Binary String Value to Integer
		/// </summary>
		/// <param name="BinaryNumber">String of '0' and '1' characters</param>
		/// <returns>Integer</returns>
		/// <remarks></remarks>
		public int Bin2Int(string BinaryNumber)
		{
			int Length;
			int x;
			int TempValue = 0; //defau
			//Get the length of the binary string
			Length = ((string)BinaryNumber).Length;

			//Convert each binary digit to its corresponding integer value
			//and add the value to the previous sum
			//The string is parsed from the right (LSB - Least Significant Bit)
			//to the left (MSB - Most Significant Bit)
			for (x = 1; x <= Length; x++) {
				TempValue = TempValue + System.Convert.ToInt32(Strings.Mid(BinaryNumber, Length - x + 1, 1)) * 2 ^ (x - 1);

			}

			return TempValue;

		}
		/// <summary>
		/// Replaces two specified colors to new colors in provided <paramref>img</paramref>
		/// </summary>
		/// <param name="Img">Bitmap to replace colors in</param>
		/// <param name="SrcColor1">First Color to Replace</param>
		/// <param name="NewColor1">First New Color</param>
		/// <param name="SrcColor2">Second Color to Replace</param>
		/// <param name="NewColor2">Replacement for Second Color</param>
		/// <returns>Bitmap with Specified Colors Replaced</returns>
		/// <remarks></remarks>
		public System.Drawing.Bitmap Replace2ColorsInImage(System.Drawing.Bitmap Img, System.Drawing.Color SrcColor1, System.Drawing.Color NewColor1, System.Drawing.Color SrcColor2, System.Drawing.Color NewColor2)
		{
			System.Drawing.Color c;
			Int32 x;
			Int32 y;
			System.Drawing.Bitmap NewPic = new System.Drawing.Bitmap(Img.Width, Img.Height);
			NewPic.MakeTransparent(System.Drawing.Color.FromArgb(128, 0, 128, 255));
			//e.Graphics.DrawImage(bmp, 10, 30)
			for (x = 0; x <= Img.Width - 1; x++) {
				for (y = 0; y <= Img.Height - 1; y++) {
					c = Img.GetPixel(x, y);
					if (c.Equals(SrcColor1)) {
						c = NewColor1;
						NewPic.SetPixel(x, y, c);
					} else if (c.Equals(SrcColor2)) {
						c = NewColor2;
						NewPic.SetPixel(x, y, c);
					} else {
						NewPic.SetPixel(x, y, c);
					}
				}
			}
			return NewPic;
		}
		/// <summary>
		/// Replaces single color to new color in specified bitmap <paramref>img</paramref>
		/// </summary>
		/// <param name="Img">Bitmap where color is to be replaced</param>
		/// <param name="SrcColor">Color to replace</param>
		/// <param name="NewColor">New replacement color</param>
		/// <returns>Bitmap with color replaced</returns>
		/// <remarks></remarks>
		public System.Drawing.Bitmap ReplaceColorInImage(System.Drawing.Bitmap Img, System.Drawing.Color SrcColor, System.Drawing.Color NewColor)
		{
			System.Drawing.Color c;
			Int32 x;
			Int32 y;
			System.Drawing.Bitmap NewPic = new System.Drawing.Bitmap(Img.Width, Img.Height);

			//e.Graphics.DrawImage(bmp, 10, 30)
			for (x = 0; x <= Img.Width - 1; x++) {
				for (y = 0; y <= Img.Height - 1; y++) {
					c = Img.GetPixel(x, y);
					if (c.Equals(SrcColor)) {
						c = NewColor;
						NewPic.SetPixel(x, y, c);
					} else {
						NewPic.SetPixel(x, y, c);
					}
				}
			}
			return NewPic;
		}
		/// <summary>
		/// Duplicate of <see cref="ReplaceColorInImage"/>
		/// </summary>
		/// <param name="img"></param>
		/// <param name="col1"></param>
		/// <param name="col2"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public System.Drawing.Bitmap switchColorInImage(System.Drawing.Bitmap img, System.Drawing.Color col1, System.Drawing.Color col2)
		{
			System.Drawing.Color c;
			Int32 x;
			Int32 y;
			System.Drawing.Bitmap NewPic = img;

			//e.Graphics.DrawImage(bmp, 10, 30)
			for (x = 0; x <= img.Width - 1; x++) {
				for (y = 0; y <= img.Height - 1; y++) {
					c = img.GetPixel(x, y);
					if (c.Equals(col1)) {
						c = col2;
						NewPic.SetPixel(x, y, c);
					} else if (c.Equals(col2)) {
						c = col1;
						NewPic.SetPixel(x, y, c);
					}
				}
			}
			return NewPic;
		}
		/// <summary>
		/// Converts Byte Array <paramref>b</paramref> to <see cref="System.Drawing.Image"/>
		/// </summary>
		/// <param name="b">Byte Array</param>
		/// <returns><see cref="System.Drawing.Image"/></returns>
		/// <remarks></remarks>
		public System.Drawing.Image ByteArrayToImage(byte[] b)
		{
			MemoryStream stream = null;
			System.Drawing.Image img = null;

			//Read the byte array into a MemoryStream  
			stream = new MemoryStream(b, 0, b.Length);
			//Create the new Image from the stream  
			img = System.Drawing.Image.FromStream(stream);
			return img;
		}

		/// <summary>
		/// Converts <see cref="System.Drawing.Image"/> <paramref>i</paramref> to Byte Array
		/// </summary>
		/// <param name="i"><see cref="System.Drawing.Image"/></param>
		/// <param name="f">Optional <see cref="System.Drawing.Imaging.ImageFormat"/> of <paramref>i</paramref>, Default = Nothing which uses format <see cref="System.Drawing.Imaging.ImageFormat.MemoryBmp"/> </param>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public byte[] ImageToByteArray(System.Drawing.Image i, System.Drawing.Imaging.ImageFormat f = null)
		{
			byte[] imageBytes;
			if (f == null) {
				f = System.Drawing.Imaging.ImageFormat.MemoryBmp;
			}
			using (MemoryStream ms = new MemoryStream()) {
				// Convert Image to byte[]
				i.Save(ms, f);
				imageBytes = ms.ToArray();
			}
			return imageBytes;
		}
		/// <summary>
		/// Converts Byte Array of Image Data to Bitmap File at location <paramref>fn</paramref>
		/// </summary>
		/// <param name="b">Byte Array with Image Data</param>
		/// <param name="fn">File Name and Path of Image File to Write</param>
		/// <param name="f">Optional <see cref="System.Drawing.Imaging.ImageFormat"/> of <paramref>i</paramref>, Default = Nothing which uses format <see cref="System.Drawing.Imaging.ImageFormat.Bmp"/></param>
		/// <remarks></remarks>
		public void ByteArrayToImageFile(byte[] b, string fn, System.Drawing.Imaging.ImageFormat f = null)
		{
			if (f == null) {
				f = System.Drawing.Imaging.ImageFormat.Bmp;
			}
			System.Drawing.Image img;
			MemoryStream stream = null;
			//Read the byte array into a MemoryStream  
			stream = new MemoryStream(b, 0, b.Length);
			img = System.Drawing.Image.FromStream(stream);
			img.Save(fn, f);

		}
		/// <summary>
		/// Converts Image File at <paramref>sFile</paramref> to Byte Array
		/// </summary>
		/// <param name="sFile">Name and Path of Image File</param>
		/// <returns>Byte Array</returns>
		/// <remarks></remarks>
		public byte[] ImageFileToByteArray(string sFile)
		{
			byte[] imgData;
			//ImageToByteArray(New Image, 
			if (sFile == null || Information.IsDBNull(sFile)) {
				return null;
			
			}
			//FileInfo instance so we can get all the  
			//information we need regarding the image  
			FileInfo fInfo = new FileInfo(sFile);

			//Get the length of the image for the byte array  
			//we create later in the function          
			long len = fInfo.Length;

			//Open a FileStream the length of the image being inserted  
			using (FileStream stream = new FileStream(sFile.Trim(), FileMode.Open)) {
                //Create a new byte array the size of the length of the file  
                imgData = new byte[len - 1];

				//Read the byte array into the buffer  
				stream.Read(imgData, 0, (int)len);
			}
			return imgData;
		}
		/// <summary>
		/// Converts a String of Hex Values to Unicode String
		/// </summary>
		/// <param name="sHexStr">Hex Values String</param>
		/// <returns>Unicode String</returns>
		/// <remarks></remarks>
		public string HexStringToString(string sHexStr)
		{
			int a;
			string sResult = "";
			if (sHexStr.Length % 2 != 0) {
				return null;
			}
			if (sHexStr.Length > 2) {
				for (a = 1; a <= sHexStr.Length - 1; a += 2) {
					sResult += Strings.ChrW((int)HexToDec(Mid(sHexStr, a, 2)));
				}
			} else {
				sResult += Strings.ChrW((int)HexToDec(sHexStr));
			}
			return sResult;
		}
		/// <summary>
		/// Converts current see <see cref="Screen"/> Object to a Bitmap Image
		/// </summary>
		/// <param name="bSmallFnt">Optional, Use Small Font (Default=False)</param>
		/// <param name="nocolors">Optional, No Colors (=ASCII) (Default=False)</param>
		/// <returns>Bitmap Frame in Format <see cref="System.Windows.Media.PixelFormat"/> = 'Indexed8'</returns>
		/// <remarks></remarks>
		public System.Drawing.Bitmap ScreenToBitmap(bool bSmallFnt = false, bool nocolors = false)
		{
			byte FntCurrChar = 0;
			int iMaxWidth = 0;
			int iMaxHeight = 0;
			byte CurrBt = 0;
			int CharXPos = 0;
			int CharYPos = 0;
			int UseForeColID = 7;
			int UseBackColID = 0;
			int FWidth;
			int FHeight;
			MediaSupport.Ansifntdef fnt;
			if (bSmallFnt == true) {
				FWidth = 8;
				FHeight = 8;
				fnt = Data.DosFnt80x50;
			} else {
				FWidth = 8;
				FHeight = 16;
				fnt = Data.DosFnt80x25;
			}
			int ImgWidth = Data.maxX * FWidth;
			int ImgHeight = Data.LinesUsed * FHeight;
			System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Indexed8;
			int width = ImgWidth;
			int height = ImgHeight;
			int stride = width;
			//CType((width * pf.BitsPerPixel + 7) / 8, Integer)
			byte[] pixels = new byte[height * stride];
            BitmapPalette palnew = new System.Windows.Media.Imaging.BitmapPalette(InternalConstants.AnsiColorsARGBM);
			int iCnt = 0;
			byte ByteEntry = 0;
			bool bSet = false;
			byte colval = 0;
			for (int PosY = Data.minY; PosY <= Data.LinesUsed; PosY++) {
				for (int y = 0; y <= FHeight - 1; y++) {
					for (int PosX = Data.minX; PosX <= Data.maxX; PosX++) {
						FntCurrChar = Data.Screen[PosX, PosY].DosChar;
						CurrBt = (byte)HexToDec(Strings.Mid(fnt.FntBits[FntCurrChar], (y * 2) + 1, 2));
						UseForeColID = Data.Screen[PosX, PosY].ForeColor + Data.Screen[PosX, PosY].Bold;
						UseBackColID = Data.Screen[PosX, PosY].BackColor;
						for (int x = 0; x <= FWidth - 1; x++) {
							if (ExamineBit(CurrBt, System.Convert.ToByte(x + 1))) {
								if (nocolors == true) {
									colval = 7;
								} else {
									colval = System.Convert.ToByte(UseForeColID);
								}
							} else {
								if (nocolors == true) {
									colval = 0;
								} else {
									colval = System.Convert.ToByte(UseBackColID);
								}
							}
							pixels[iCnt] = colval;
							iCnt += 1;
						}
					}
				}
			}
			try {
				BitmapSource myBMSource = System.Windows.Media.Imaging.BitmapSource.Create(width, height, 96, 96, pf, palnew, pixels, stride);
				System.Windows.Media.Imaging.BmpBitmapEncoder enc = new System.Windows.Media.Imaging.BmpBitmapEncoder();
				enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(myBMSource));
				MemoryStream mem = new MemoryStream();
				enc.Save(mem);
				return new Bitmap(System.Drawing.Bitmap.FromStream(mem));

			} catch (Exception ex) {
				return new System.Drawing.Bitmap(width: 100, height: 100, stride: 100, format: System.Drawing.Imaging.PixelFormat.Format8bppIndexed, scan0: new IntPtr(0));
			}

		}
		/// <summary>
		/// Converts Byte Array to Bitmap
		/// </summary>
		/// <param name="Bte">Byte Array</param>
		/// <param name="TextWidth">Max Number of Characters in Line</param>
		/// <param name="NumLines">Number of Lines Total</param>
		/// <param name="bSmallFnt">Optional, Use Small Font (Default=False)</param>
		/// <returns>Bitmap Frame in Format <see cref="System.Windows.Media.PixelFormat"/> = 'Indexed8'</returns>
		/// <remarks>The Byte Array is typically a character definition from <see cref="MediaSupport.Ansifntdef.FntBits"/></remarks>
		public System.Drawing.Bitmap ByteArrayToBitmap(byte[] Bte, int TextWidth, int NumLines, bool bSmallFnt = false)
		{
			byte FntCurrChar = 0;
			int iMaxWidth = 0;
			int iMaxHeight = 0;
			byte CurrBt = 0;
			int CharXPos = 0;
			int CharYPos = 0;
			int UseForeColID = 7;
			int UseBackColID = 0;
			int FWidth;
			int FHeight;
			MediaSupport.Ansifntdef fnt;
			if (bSmallFnt == true) {
				FWidth = 8;
				FHeight = 8;
				fnt = Data.DosFnt80x50;
			} else {
				FWidth = 8;
				FHeight = 16;
				fnt = Data.DosFnt80x25;
			}
			int ImgWidth = TextWidth * FWidth;
			int ImgHeight = NumLines * FHeight;
			System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Indexed8;
			int width = ImgWidth;
			int height = ImgHeight;
			int stride = width;
			//CType((width * pf.BitsPerPixel + 7) / 8, Integer)
			byte[] pixels = new byte[height * stride];
            BitmapPalette palnew = new System.Windows.Media.Imaging.BitmapPalette(InternalConstants.AnsiColorsARGBM);
			int iCnt = 0;
			byte ByteEntry = 0;
			bool bSet = false;
			byte colval = 0;
			for (int PosY = 1; PosY <= NumLines; PosY++) {
				for (int y = 0; y <= FHeight - 1; y++) {
					for (int PosX = 1; PosX <= TextWidth; PosX++) {
						FntCurrChar = Bte[((PosY - 1) * TextWidth) + PosX - 1];
						CurrBt = (byte)HexToDec(Strings.Mid(fnt.FntBits[FntCurrChar], (y * 2) + 1, 2));
						for (int x = 0; x <= FWidth - 1; x++) {
							if (ExamineBit(CurrBt, System.Convert.ToByte(x + 1))) {
								colval = System.Convert.ToByte(UseForeColID);
							} else {
								colval = System.Convert.ToByte(UseBackColID);
							}
							pixels[iCnt] = colval;
							iCnt += 1;
						}
					}
				}
			}
			try {
				BitmapSource myBMSource = System.Windows.Media.Imaging.BitmapSource.Create(width, height, 96, 96, pf, palnew, pixels, stride);
				System.Windows.Media.Imaging.BmpBitmapEncoder enc = new System.Windows.Media.Imaging.BmpBitmapEncoder();
				enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(myBMSource));
				MemoryStream mem = new MemoryStream();
				enc.Save(mem);
				return new Bitmap(Image.FromStream(mem));

			} catch (Exception ex) {
				return new System.Drawing.Bitmap(width: 100, height: 100, stride: 100, format: System.Drawing.Imaging.PixelFormat.Format8bppIndexed, scan0: new IntPtr(0));
			}

		}
		/// <summary>
		/// Creates Video Frame Bitmap from current <see cref="Screen"/> Object.
		/// </summary>
		/// <param name="bSmallFnt">Optional, Use Small Font (Default = False)</param>
		/// <param name="nocolors">Optional, No Colors (= ASCII) (Default = False)</param>
		/// <param name="YIndex">Optional, <see cref="Yoffset"/> Location to use (Default = 0)</param>
		/// <returns>Bitmap Frame in Format <see cref="System.Windows.Media.PixelFormat"/> = 'Indexed8'</returns>
		/// <remarks></remarks>
		public System.Drawing.Bitmap CreateVideoFrame(bool bSmallFnt = false, bool nocolors = false, int YIndex = 0)
		{
			byte FntCurrChar = 0;
			int iMaxWidth = 0;
			int iMaxHeight = 0;
			byte CurrBt = 0;
			int UseForeColIDVid = 7;
			int UseBackColIDVid = 0;
			int FWidth;
			int FHeight;
			MediaSupport.Ansifntdef fnt;
			if (bSmallFnt == true) {
				FWidth = 8;
				FHeight = 8;
				fnt = Data.DosFnt80x50;
			} else {
				FWidth = 8;
				FHeight = 16;
				fnt = Data.DosFnt80x25;
			}
			int ImgWidth = 80 * FWidth;
			int ImgHeight = 25 * FHeight;
			System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Indexed8;
			int width = ImgWidth;
			int height = ImgHeight;
			int stride = width;
			//CType((width * pf.BitsPerPixel + 7) / 8, Integer)
			byte[] pixels = new byte[height * stride];
            BitmapPalette palnew = new System.Windows.Media.Imaging.BitmapPalette(InternalConstants.AnsiColorsARGBM);
			int iCntVid = 0;
			byte ByteEntry = 0;
			bool bSet = false;
			byte colvalVid = 0;
			int YEnd;
			int XPosV = 1;
			int YPosV = 1;

			if (YIndex + 24 > Data.LinesUsed) {
				YEnd = Data.LinesUsed;
			} else {
				YEnd = YIndex + 24;
			}
			//YIndex
			for (YPosV = YIndex + 1; YPosV <= YEnd; YPosV++) {
				for (int yVid = 0; yVid <= FHeight - 1; yVid++) {
					for (XPosV = 1; XPosV <= 80; XPosV++) {
						FntCurrChar = Data.Screen[XPosV, YPosV].DosChar;
						CurrBt = (byte)HexToDec(Strings.Mid(fnt.FntBits[FntCurrChar], (yVid * 2) + 1, 2));
						UseForeColIDVid = Data.Screen[XPosV, YPosV].ForeColor + Data.Screen[XPosV, YPosV].Bold;
						UseBackColIDVid = Data.Screen[XPosV, YPosV].BackColor;
						for (int xVid = 0; xVid <= FWidth - 1; xVid++) {
							if (ExamineBit(CurrBt, System.Convert.ToByte(xVid + 1))) {
								if (nocolors == true) {
									colvalVid = 7;
								} else {
									colvalVid = System.Convert.ToByte(UseForeColIDVid);
								}
							} else {
								if (nocolors == true) {
									colvalVid = 0;
								} else {
									colvalVid = System.Convert.ToByte(UseBackColIDVid);
								}
							}
							pixels[iCntVid] = colvalVid;
							iCntVid += 1;
						}
					}
				}
			}
			try {
				BitmapSource myBMSource = System.Windows.Media.Imaging.BitmapSource.Create(width, height, 96, 96, pf, palnew, pixels, stride);
				System.Windows.Media.Imaging.BmpBitmapEncoder enc = new System.Windows.Media.Imaging.BmpBitmapEncoder();
				enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(myBMSource));
				MemoryStream mem = new MemoryStream();
				enc.Save(mem);
				return new Bitmap(System.Drawing.Bitmap.FromStream(mem));
			} catch (Exception ex) {
				return new System.Drawing.Bitmap(width: 100, height: 100, stride: 100, format: System.Drawing.Imaging.PixelFormat.Format8bppIndexed, scan0: new IntPtr(0));


			}


		}
		/// <summary>
		/// Comverts Bitmap to <see cref="System.Windows.Media.PixelFormat"/> = 'Indexed8'
		/// </summary>
		/// <param name="bm">Source Bitmap</param>
		/// <param name="pal"><see cref="System.Drawing.Imaging.ColorPalette"/> to use for output 'Indexed8' Bitmap</param>
		/// <returns>Bitmap with <see cref="System.Windows.Media.PixelFormat"/> = 'Indexed8'</returns>
		/// <remarks></remarks>
		public System.Drawing.Bitmap BitmapToIndexed(System.Drawing.Bitmap bm, System.Drawing.Imaging.ColorPalette pal)
		{
			System.Drawing.Bitmap newbm = null;
			try {
				System.Windows.Media.PixelFormat pf = System.Windows.Media.PixelFormats.Indexed8;
				int width = bm.Width;
				int height = bm.Height;
				int stride = (width * pf.BitsPerPixel) / 8;
				//             height * (width * pf.BitsPerPixel / 8)

				byte[] pixels = new byte[height * stride];
				System.Drawing.Color c;
				List<System.Windows.Media.Color> clist = new List<System.Windows.Media.Color>();
				for (int id = 0; id <= pal.Entries.Count() - 1; id++) {
					clist.Add(System.Windows.Media.Color.FromArgb(pal.Entries[id].A, pal.Entries[id].R, pal.Entries[id].G, pal.Entries[id].B));
				}
				BitmapPalette palnew = new System.Windows.Media.Imaging.BitmapPalette(clist);
				byte palentry;
				int iCnt = 0;
				for (int x = 0; x <= width - 1; x++) {
					for (int y = 0; y <= height - 1; y++) {
						c = bm.GetPixel(x, y);
						palentry = FindPalettenEntry(c, pal);
						pixels[iCnt] = palentry;
						iCnt += 1;
					}
				}
				BitmapSource myBMSource = System.Windows.Media.Imaging.BitmapSource.Create(width, height, 96, 96, pf, palnew, pixels, stride);
				System.Windows.Media.Imaging.BmpBitmapEncoder enc = new System.Windows.Media.Imaging.BmpBitmapEncoder();
				enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(myBMSource));
				MemoryStream mem = new MemoryStream();
				enc.Save(mem);
				newbm = new Bitmap(System.Drawing.Bitmap.FromStream(mem));

			} catch (Exception ex) {
			}

			return newbm;
		}
		/// <summary>
		/// Find Palette Index Location for Color <paramref>col</paramref> in Palette <paramref>cp</paramref>
		/// </summary>
		/// <param name="col">Color to find in Palette <paramref>cp</paramref></param>
		/// <param name="cp">Color Palette</param>
		/// <param name="bIgnoreAlpha">Optional Ignore Alpha Values (Default=True)</param>
		/// <returns>Byte with Index location of color in Palette</returns>
		/// <remarks>If Color is not found in Palette, index of nearest color (see <see cref="GetClosestExistingRGBColor"/> is returned instead</remarks>
		public byte FindPalettenEntry(System.Drawing.Color col, System.Drawing.Imaging.ColorPalette cp, bool bIgnoreAlpha = true)
		{
			byte iResult = 0;
			bool bFound = false;
			System.Drawing.Color nearc;
			for (int a = 0; a <= cp.Entries.Count() - 1; a++) {
				if (cp.Entries[a].R == col.R & cp.Entries[a].G == col.G & cp.Entries[a].B == col.B & (bIgnoreAlpha == true | (bIgnoreAlpha == false & cp.Entries[a].A == col.A))) {
					bFound = true;
					iResult = System.Convert.ToByte(a);
					break; // TODO: might not be correct. Was : Exit For
				}
			}
			if (bFound == false) {
				nearc = GetClosestExistingRGBColor(col, cp);
				int newres = Array.FindIndex(cp.Entries, x => nearc.Equals(x));
				if (newres != -1) {
					iResult = (byte)newres;
				}
			}
			return iResult;
		}
		/// <summary>
		/// Returns True, if new Color <paramref>Color1</paramref> comes closer to Comparison Color <paramref>CompColor</paramref> than current nearest color <paramref>CurrNearCol</paramref>
		/// </summary>
		/// <param name="Color1">New Color to Test</param>
		/// <param name="CurrNearCol">Current Nearest Color</param>
		/// <param name="CompColor">Color to Compare To</param>
		/// <returns>True, if <paramref>Color1</paramref> comes closer to <paramref>CompColor</paramref> than <paramref>CurrNearCol</paramref>, False, if not</returns>
		/// <remarks></remarks>
		public bool TestColorCloser(System.Drawing.Color Color1, System.Drawing.Color CurrNearCol, System.Drawing.Color CompColor)
		{
			double MinDist;
			double DistCI;

			MinDist = (Math.Pow((CurrNearCol.R - CompColor.R), 2) + Math.Pow((CurrNearCol.G - CompColor.G), 2) + Math.Pow((CurrNearCol.B - CompColor.B), 2));
			DistCI = (Math.Pow((Color1.R - CompColor.R), 2) + Math.Pow((Color1.G - CompColor.G), 2) + Math.Pow((Color1.B - CompColor.B), 2));

			if (DistCI <= MinDist) {
				// distance is less than current minimum. set save variables.
				return true;
			} else {
				return false;
			}

		}

		/// <summary>
		/// Determines which color of Palette <paramref>pal</paramref> comes closest to color <paramref>ColorI</paramref>
		/// </summary>
		/// <param name="ColorI">Input Color</param>
		/// <param name="pal">Color Palette as <see cref="System.Drawing.Imaging.ColorPalette"/></param>
		/// <returns>Closest <see cref="System.Drawing.Color"/> to <paramref>ColorI</paramref> in Palette <paramref>pal</paramref></returns>
		/// <remarks></remarks>
		public System.Drawing.Color GetClosestExistingRGBColor(System.Drawing.Color ColorI, System.Drawing.Imaging.ColorPalette pal)
		{
			
			System.Drawing.Color CloseCol = System.Drawing.Color.Black;
			for (int a = 0; a <= pal.Entries.Count() - 1; a++) {
				if (a == 0) {
					CloseCol = (System.Drawing.Color)pal.Entries[a];
				} else {
					if (TestColorCloser((System.Drawing.Color)pal.Entries[a], CloseCol, ColorI) == true) {
						CloseCol = (System.Drawing.Color)pal.Entries[a];
					}
				}
			}
			return CloseCol;
		}
		/// <summary>
		/// Returns Larger of two values <paramref>val1</paramref> and <paramref>val2</paramref>
		/// </summary>
		/// <param name="val1">Integer Value 1 to compare</param>
		/// <param name="val2">Integer Value 2 to compare</param>
		/// <returns>Larger Integer Value</returns>
		/// <remarks></remarks>
		public int larger(int val1, int val2)
		{
			return (int) Interaction.IIf(val1 > val2, val1, val2);
		}
		/// <summary>
		/// Returns Smaller of two values <paramref>val1</paramref> and <paramref>val2</paramref>
		/// </summary>
		/// <param name="val1">Integer Value 1 to compare</param>
		/// <param name="val2">Integer Value 2 to compare</param>
		/// <returns>Smaller Integer Value</returns>
		/// <remarks></remarks>
		public int smaller(int val1, int val2)
		{
			return (int)Interaction.IIf(val1 < val2, val1, val2);
		}

	}
}