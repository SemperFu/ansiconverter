using Microsoft.VisualBasic;
using static Data;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.Interaction;
using System;
using Converter.Properties;
using Internal;

namespace MediaFormats
{


	public class AVT
	{
		public void ProcessAVTFile(byte[] aFile)
		{
			ProcessAVTFile("", aFile);
		}

		public static void ProcessAVTFile(string sFile, byte[] aFile = null)
		{
			if (bDebug) {
				Console.WriteLine("Process AVT File: " + sFile);
			}
			byte[] aAnsi = null;
			int AnsiMode = 0;
			bool bDrawChar;
			int CurChr;
			int iLoop2;
			string sStr = "";

			if (!(aFile == null)) {
				aAnsi = ConverterSupport.Convert.MergeByteArrays(ConverterSupport.Convert.NullByteArray(), aFile);
			} else {
				if (File.Exists(sFile)) {
					aAnsi = ConverterSupport.InputOutput.ReadBinaryFile(sFile);
					aAnsi = ConverterSupport.Convert.MergeByteArrays(ConverterSupport.Convert.NullByteArray(), aAnsi);
				}
			}

			ConverterSupport.Mappings.BuildMappings(sCodePg);

			ForeColor = 7;
			BackColor = 0;
			LineWrap = true;
			Blink = false;
			Bold = 0;
			Reversed = false;
			LinesUsed = 0;
			 // ERROR: Not supported in C#: ReDimStatement

			for (int x = minX; x <= maxX; x++) {
				for (int Y = minY; Y <= maxY; Y++) {
					Screen(x, Y) = new ConverterSupport.ScreenChar(x);
				}
			}
			System.Windows.Forms.Application.DoEvents();
			XPos = minX;
			YPos = minY;
			if (!aAnsi == null) {
				if (UBound(aAnsi) > 0) {
					iLoop = 1;
					while (iLoop <= UBound(aAnsi)) {
						bDrawChar = true;
						CurChr = (int)aAnsi[iLoop];
						switch (AnsiMode) {
							case 0:
								//@
								if (CurChr == 22) {
									AnsiMode = 1;
									bDrawChar = false;
								}
								//SUB or "S"
								if (CurChr == 26 | CurChr == 83) {
									int iSauceOffset = IIf(CurChr == 83, 1, 0);
									if (UBound(aAnsi) >= iLoop + 128 - iSauceOffset) {
										sStr = "";
										for (iLoop2 = 1 - iSauceOffset; iLoop2 <= 5 - iSauceOffset; iLoop2++) {
											sStr += Chr(aAnsi[iLoop + iLoop2]);
										}
										if (sStr == "SAUCE") {
											bDrawChar = false;
											iLoop += 1 - iSauceOffset;
											iLoop = oSauce.GetFromByteArray(aAnsi, iLoop);
											bHasSauce = true;
											//Read Sauce
											if (bDebug == true)
												Console.WriteLine("Sauce Meta found");
										}
									}
								}
							case 1:
								if (CurChr == 1) {
									AnsiMode = 2;
									bDrawChar = false;
								} else {
									if (ConverterSupport.Convert.SetChar(Chr(64).ToString()) == false) {
										iLoop = UBound(aAnsi) + 1;
									} else {
										bDrawChar = true;
										AnsiMode = 0;
									}
								}
                                break;
							case 2:
								string sCol = Strings.Right("0" + Hex(CurChr).ToString(), 2);
								string sFC = Strings.Right(sCol, 1);
								string sBC = Strings.Left(sCol, 1);
								BackColor = ConverterSupport.HexToDec((string)sBC);
								ForeColor = ConverterSupport.HexToDec((string)sFC);
								if (ForeColor > 7) {
									Bold = 8;
									ForeColor -= 8;
								} else {
									Bold = 0;
								}
								AnsiMode = 0;
								bDrawChar = false;
                                break;
						}
						if (bDrawChar == true & AnsiMode == 0) {
							switch (CurChr) {
								case 10:
									YPos += 1;
									if (YPos > maxY - 1) {
										YPos = maxY - 1;
										iLoop = UBound(aAnsi) + 1;
									} else {
										XPos = minX;
									}
									if (YPos > LinesUsed) {
										LinesUsed = YPos;
									}
                                    break;
									// restore X in linefeed so's to support *nix type files
								case 13:
									//ignore
								case 26:
								default:
									if (ConverterSupport.Convert.SetChar(Chr(CurChr).ToString()) == false) {
										iLoop = UBound(aAnsi) + 1;
									}
                                    break;
							}
						}
						iLoop += 1;
						if (iLoop / 1000 == (int)iLoop / 1000)
							System.Windows.Forms.Application.DoEvents();
					}
				}
			}
		}
	}
}