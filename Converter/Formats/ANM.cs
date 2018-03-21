using Microsoft.VisualBasic;
using static Data;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.Interaction;
using System;
using Converter.Properties;
using Internal;
using System.IO;

namespace MediaFormats
{


	public class ANM
	{
		//ANSI Animation
		//Screen 80x25
		//Each Char represented by its own SPAN with unique ID
		//ID, starting with "0" counting up in HEX up to "7CF" (1999)
		//data are stored in string arrays
		//Axx up
		//Bxx down
		//Cxx right
		//Dxx left
		//Pxy position x,y where x chars 46-125 (to represent 1-80) and y chars a-y (for 1-25)
		//Tbf change color where b = background color 0-7 and f = foreground color 0-F
		//S Save Pos
		//R Restore Pos
		//Wxx Wait xx cycles (equivalent to the "Report Position" tag thats often used in ANSI anims to cause delays
		//Zxx xx number of spaces , space is replaced by &nbsp; 
		//X.. followed by either ASCII text or unicode HTML coded content, multiple text characters can be joint in one array
		//    entry, separated by space, if they have the same attributes
		//F clear screen
		//G clear to the end of line
		//H clear to start of line
		//I clear line
		//J clear to 1/1
		//K clear to end
		//N new line
		public string[] ProcessANSIAnimationFile(string sOutFile, byte[] aFile)
		{
			return ProcessANSIAnimationFile("", sOutFile, aFile);
		}

		public static string[] ProcessANSIAnimationFile(string sFile, string sOutFile, byte[] aFile = null)
		{
			int StoreX = 1;
			int StoreY = 1;
			int AnsiMode = 0;
			byte[] aAnsi = null;
			string[] aPar;
			bool bDrawChar;
			bool bEnde;
			int CurChr;
			int iLoop2;
			string sStr = "";
			string sEsc = "";
			string sRes = "";
			LineWrap = false;
			object[] aOutAnm = new Object[5000];

			//aOutAnm = Data.RedimPreserve(aOutAnm, 5000);
			 // ERROR: Not supported in C#: ReDimStatement

			int iCmdCnt;
			iCmdCnt = 0;
			if (!(aFile == null)) {
				aAnsi = ConverterSupport.Convert.MergeByteArrays(ConverterSupport.Convert.NullByteArray(), aFile);
			} else {
				if (File.Exists(sFile)) {
					aAnsi = ConverterSupport.InputOutput.ReadBinaryFile(sFile);
					aAnsi = ConverterSupport.Convert.MergeByteArrays(ConverterSupport.Convert.NullByteArray(), aAnsi);
				}
			}

			ConverterSupport.Mappings.BuildMappings(sCodePg);
			System.Windows.Forms.Application.DoEvents();
			ForeColor = 7;
			Data.BackColor = 0;
			Data.LineWrap = true;
			Data.Blink = false;
			Data.Bold = 0;
			Data.Reversed = false;
			Data.LinesUsed = 0;

			Data.XPos = Data.minX;
			Data.YPos = Data.minY;
			if (!(aAnsi == null)) {
				if (Information.UBound(aAnsi) > 0) {
					Data.iLoop = 1;
					while (iLoop <= Information.UBound(aAnsi)) {
						if (iCmdCnt > Information.UBound((Array)aOutAnm) - 100) {
							Array.Resize(ref aOutAnm, Information.UBound((Array)aOutAnm) + 1001);
						}
						bDrawChar = true;
						CurChr = (int)aAnsi[iLoop];
						switch (AnsiMode) {
							case 0:
								//ESC
								if (CurChr == 27) {
									AnsiMode = 1;
									bDrawChar = false;
								}
								//SUB or "S"
								if (CurChr == 26 | CurChr == 83) {
									int iSauceOffset = (int) IIf(CurChr == 83, 1, 0);
									if (Information.UBound(aAnsi) >= iLoop + 128 - iSauceOffset) {
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
								break;
							case 1:
								//[
								if (CurChr == 91) {
									AnsiMode = 2;
									bDrawChar = false;
								} else {
									aOutAnm[iCmdCnt] = "X" + Hex(27);
									iCmdCnt += 1;
									bDrawChar = true;
									AnsiMode = 0;
								}
								break;
							case 2:
								int aRRSize;
								aRRSize = UBound(aAnsi);
								sEsc = Chr(CurChr).ToString();
								//0-9 or ;
								if ((CurChr >= 48 & CurChr <= 57) | CurChr == 59) {
									bEnde = false;
									while (bEnde == false) {
										iLoop += 1;
										if (iLoop > aRRSize) {
											bEnde = true;
										} else {
											CurChr = aAnsi[iLoop];
											sEsc += (string)Chr(CurChr).ToString();
											if ((CurChr >= 65 & CurChr <= 90) | (CurChr >= 97 & CurChr <= 122)) {
												bEnde = true;
											}
										}
									}
								}
								aPar = oAnsi.BuildParams(sEsc);
								int NumParams = UBound(aPar);
								switch (Chr(CurChr).ToString()) {
									case "A":
										//Move Cursor Up
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.Convert.ChkNum(aPar[1]);
											if (iLoop2 == 0) {
												iLoop2 = 1;
											}
										} else {
											iLoop2 = 1;
										}
										aOutAnm[iCmdCnt] = "A" + Right("0" + Conversion.Hex(iLoop2), 2);
										iCmdCnt += 1;
										break;
									case "B":
										//Move Cursor Down
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.Convert.ChkNum(aPar[1]);
											if (iLoop2 == 0) {
												iLoop2 = 1;
											}
										} else {
											iLoop2 = 1;
										}
										YPos = YPos + iLoop2;
										aOutAnm[iCmdCnt] = "B" + Strings.Right("0" + Conversion.Hex(iLoop2), 2);
										iCmdCnt += 1;
										break;
									case "C":
										//Move Cursor Right
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.Convert.ChkNum(aPar[1]);
											if (iLoop2 == 0) {
												iLoop2 = 1;
											}
										} else {
											iLoop2 = 1;
										}
										aOutAnm[iCmdCnt] = "C" + Strings.Right("0" + Conversion.Hex(iLoop2), 2);
										iCmdCnt += 1;
										break;
									case "D":
										//Move Cursor left
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.Convert.ChkNum(aPar[1]);
											if (iLoop2 == 0) {
												iLoop2 = 1;
											}
										} else {
											iLoop2 = 1;
										}
										aOutAnm[iCmdCnt] = "D" + Strings.Right("0" + Conversion.Hex(iLoop2), 2);
										iCmdCnt += 1;
										break;
									case "H":
										//Move Cursor
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.Convert.ChkNum(aPar[1]);
											YPos = (int) IIf(iLoop2 > 0, minY + (iLoop2 - 1), minY);
										} else {
											YPos = minY;
										}
										if (NumParams > 1) {
											iLoop2 = ConverterSupport.Convert.ChkNum(aPar[2]);
											XPos = (int) IIf(iLoop2 > 0, minX + (iLoop2 - 1), minX);
										} else {
											XPos = minX;
										}
										aOutAnm[iCmdCnt] = "P" + escapchr(Chr(XPos + 45).ToString()) + escapchr(Chr(YPos + 96).ToString());
										iCmdCnt += 1;
										break;
									case "f":
										//Move Cursor
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.Convert.ChkNum(aPar[1]);
											YPos = (int)IIf(iLoop2 > 0, minY + (iLoop2 - 1), minY);
										} else {
											YPos = minY;
										}
										if (NumParams > 1) {
											iLoop2 = ConverterSupport.Convert.ChkNum(aPar[2]);
											XPos = (int)IIf(iLoop2 > 0, minX + (iLoop2 - 1), minX);
										} else {
											XPos = minX;
										}
										aOutAnm[iCmdCnt] = "P" + escapchr(Chr(XPos + 45).ToString()) + escapchr(Chr(YPos + 96).ToString());
										iCmdCnt += 1;
										break;
									case "J":
										//ForeColor = 7 : BackColor = 0 : Blink = False : Bold = 0 : Reversed = False
										if (NumParams > 0) {
											switch (ConverterSupport.Convert.ChkNum(aPar[1])) {
												case 0:
													//erase from cursor to end of screen
													aOutAnm[iCmdCnt] = "K";
													iCmdCnt += 1;
													break;
												case 1:
													//Erase from beginning of screen to cursor
													aOutAnm[iCmdCnt] = "J";
													iCmdCnt += 1;
													break;
												case 2:
													//Clear screen and home cursor
													aOutAnm[iCmdCnt] = "F";
													iCmdCnt += 1;
													break;
											}
										//Erase from cursor to end of screen
										} else {
											aOutAnm[iCmdCnt] = "K";
											iCmdCnt += 1;

										}
										break;
									case "m":
										//Set Attribute
										if (NumParams == 0) {
											Data.ForeColor = 7;
											BackColor = 0;
											Blink = false;
											Bold = 0;
											Reversed = false;
											aOutAnm[iCmdCnt] = "T07";
											iCmdCnt += 1;
											aOutAnm[iCmdCnt] = "L0";
											iCmdCnt += 1;
										}
										for (int iLoop3 = 1; iLoop3 <= NumParams; iLoop3++) {
											object i2;
											iLoop2 = ConverterSupport.Convert.ChkNum(aPar[iLoop3]);
											switch (iLoop2) {
												case 0:
													if ((string)Left(aPar[iLoop3], 1) == "0") {
														ForeColor = 7;
														BackColor = 0;
														Blink = false;
														Bold = 0;
														Reversed = false;
														aOutAnm[iCmdCnt] = "T07";
														iCmdCnt += 1;
														aOutAnm[iCmdCnt] = "L0";
														iCmdCnt += 1;
													}
													break;
												case 1:
													Bold = 8;
													aOutAnm[iCmdCnt] = "T" + Conversion.Hex(BackColor) + Conversion.Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 2:
													Bold = 0;
													aOutAnm[iCmdCnt] = "T" + Conversion.Hex(BackColor) + Conversion.Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 5:
													Blink = true;
													aOutAnm[iCmdCnt] = "L1";
													iCmdCnt += 1;
													break;
												case 7:
													i2 = ForeColor;
													ForeColor = BackColor;
													BackColor = (int)i2;
													Reversed = true;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 22:
													Bold = 0;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 25:
													Blink = false;
													aOutAnm[iCmdCnt] = "L0";
													iCmdCnt += 1;
													break;
												case 27:
													i2 = ForeColor;
													ForeColor = BackColor;
													BackColor = (int)i2;
													Reversed = false;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 30:
													ForeColor = 0;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 31:
													ForeColor = 4;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 32:
													ForeColor = 2;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 33:
													ForeColor = 6;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 34:
													ForeColor = 1;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 35:
													ForeColor = 5;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
                                                    break;
												case 36:
													ForeColor = 3;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 37:
													ForeColor = 7;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 40:
													BackColor = 0;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 41:
													BackColor = 4;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 42:
													BackColor = 2;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 43:
													BackColor = 6;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 44:
													BackColor = 1;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 45:
													BackColor = 5;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 46:
													BackColor = 3;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
												case 47:
													BackColor = 7;
													aOutAnm[iCmdCnt] = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
													break;
											}
										}
                                        break;
									case "K":
										if (NumParams > 0) {
											switch (ConverterSupport.Convert.ChkNum(aPar[1])) {
												case 0:
													//clear to the end of the line
													aOutAnm[iCmdCnt] = "G";
													iCmdCnt += 1;
													break;
												case 1:
													//Erase from beginning of line to cursor
													aOutAnm[iCmdCnt] = "H";
													iCmdCnt += 1;
													break;
												case 2:
													//Erase line containing the cursor
													aOutAnm[iCmdCnt] = "I";
													iCmdCnt += 1;
													break;
											}
										} else {
											//clear to the end of the line
											aOutAnm[iCmdCnt] = "G";
											iCmdCnt += 1;
										}
										break;
									case "s":
										//save cursor position
										aOutAnm[iCmdCnt] = "S";
										iCmdCnt += 1;
										break;
									case "u":
										//Restore Position
										aOutAnm[iCmdCnt] = "R";
										iCmdCnt += 1;
										break;
									case "n":
										//Report Cursor Position
										aOutAnm[iCmdCnt] = "W01";
										iCmdCnt += 1;
										break;
									//Set Display Mode
									case "h":
									//<[=Xh   X = Mode, 7 = Turn ON linewrap

										//Set Display Mode
									case "l":
									//<[=7l  = Truncate Lines longer than 80 chars
									default:

										Console.WriteLine("Unknown ANSI Command: " + Chr(CurChr).ToString() + " (" + CurChr.ToString() + ") Params: " + Join(aPar, "|").ToString() + ",File: " + sFile + ", Pos: " + iLoop.ToString());
										break;
								}

								bDrawChar = false;
								AnsiMode = 0;
                                break;
						}

						if (bDrawChar == true & AnsiMode == 0) {
							switch (CurChr) {
								case 10:
									aOutAnm[iCmdCnt] = "N";
									iCmdCnt += 1;
									break;
								// restore X in linefeed so's to support *nix type files
								case 13:
									// XPos = 1
									break;
								case 32:
									aOutAnm[iCmdCnt] = "Z01";
									iCmdCnt += 1;
									//ignore
									break;
								case 26:
								default:
									// If CurChr >= 1 Then
									aOutAnm[iCmdCnt] = "X" + Hex(CurChr);
									iCmdCnt += 1;
									//End If
									break;
							}
						}
						iLoop += 1;
						if (iLoop / 1000 == (int)iLoop / 1000)
							System.Windows.Forms.Application.DoEvents();
					}
				}
			}

			string SubBlock = "F";
			int SubCnt = 1;
			string[] aTmp = new string[iCmdCnt-1];
			string sPrevCmd;
			int ExecCnt;
			string sCurCmd = "";
			int iNewPos = 0;
			string sCombStr = "";

			if (iCmdCnt > 0) {
				 // ERROR: Not supported in C#: ReDimStatement

				sPrevCmd = "";
				ExecCnt = 0;
				for (iLoop2 = 0; iLoop2 <= iCmdCnt - 1; iLoop2++) {
					sCurCmd = Strings.Left(aOutAnm[iLoop2].ToString(), 1);
					if (sPrevCmd != sCurCmd) {
						if (sPrevCmd != "") {
							switch (sPrevCmd) {
								//Case "P" : SubBlock &= " " & sCombStr : SubCnt += 1
								case "T":
									SubBlock += " " + sCombStr;
									SubCnt += 1;
									break;
								case "S":
									if (ExecCnt > 1) {
										SubBlock += " S";
										SubCnt += 1;
										SubBlock += " W" + Right("0" + Hex(ExecCnt - 1), 2);
										SubCnt += 1;
									} else {
										SubBlock += " S";
										SubCnt += 1;
									}
									break;
								case "R":
									if (ExecCnt > 1) {
										SubBlock += " R";
										SubCnt += 1;
										SubBlock += " W" + Right("0" + Hex(ExecCnt - 1), 2);
										SubCnt += 1;
									} else {
										SubBlock += " R";
										SubCnt += 1;
									}
									break;
								case "W":
									SubBlock += " W" + Right("0" + Hex(ExecCnt), 2);
									SubCnt += 1;
									break;
								case "Z":
									SubBlock += " Z" + Right("0" + Hex(ExecCnt), 2);
									SubCnt += 1;
									break;
								case "X":
									SubBlock += IIf(ExecCnt > 1, " x" + sCombStr, " X" + sCombStr);
									SubCnt += 1;
									break;
								default:
									//shouldn't be here
									break;
							}
							if (SubCnt > 1000) {
								aTmp[iNewPos] = Trim(SubBlock);
								iNewPos += 1;
								SubCnt = 0;
								SubBlock = "";
							}
						}
						sCombStr = "";
						ExecCnt = 1;
						switch (sCurCmd) {
							//Case "P" : sCombStr = aOutAnm[iLoop2] : sPrevCmd = sCurCmd
							case "T":
								sCombStr = aOutAnm[iLoop2].ToString();
								sPrevCmd = sCurCmd;
								break;
							case "S":
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
								break;
							case "R":
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
								break;
							case "W":
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
								break;
							case "Z":
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
								break;
							case "X":
								sCombStr = Right(aOutAnm[iLoop2].ToString(), aOutAnm[iLoop2].ToString().Length - 1);
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
								break;
							default:
								SubBlock += " " + aOutAnm[iLoop2];
								SubCnt += 1;
								sPrevCmd = "";
								break;
						}
					} else {
						switch (sCurCmd) {
							//Case "P" : sCombStr = aOutAnm[iLoop2]
							case "T":
								sCombStr = aOutAnm[iLoop2].ToString();
								break;
							case "S":
								ExecCnt += 1;
								break;
							case "R":
								ExecCnt += 1;
								break;
							case "W":
								ExecCnt += 1;
								break;
							case "Z":
								ExecCnt += 1;
								break;
							case "X":
								sCombStr += ";" + Right(aOutAnm[iLoop2].ToString(), aOutAnm[iLoop2].ToString().Length - 1);
								ExecCnt += 1;
								break;
						}
					}
				}
				if (sPrevCmd != "") {
					switch (sPrevCmd) {
						//Case "P" : SubBlock &= " " & sCombStr : SubCnt += 1
						case "T":
							SubBlock += " " + sCombStr;
							SubCnt += 1;
							break;
						case "S":
							if (ExecCnt > 1) {
								SubBlock += " S";
								SubCnt += 1;
								SubBlock += " W" + Right("0" + Hex(ExecCnt - 1), 2);
								SubCnt += 1;
							} else {
								SubBlock += " S";
								SubCnt += 1;
							}
							break;
						case "R":
							if (ExecCnt > 1) {
								SubBlock += " R";
								SubCnt += 1;
								SubBlock += " W" + Right("0" + Hex(ExecCnt - 1), 2);
								SubCnt += 1;
							} else {
								SubBlock += " R";
								SubCnt += 1;
							}
							break;
						case "W":
							SubBlock += " W" + Right("0" + Hex(ExecCnt), 2);
							SubCnt += 1;
							break;
						case "Z":
							SubBlock += " Z" + Right("0" + Hex(ExecCnt), 2);
							SubCnt += 1;
							break;
						case "X":
							SubBlock += IIf(ExecCnt > 1, " x" + sCombStr, " X" + sCombStr);
							SubCnt += 1;
							break;
						default:
							//shouldn't be here
							Console.WriteLine("shouldn't be here. sPrevCmd='" + sPrevCmd + "'");
							break;
					}

					aTmp[iNewPos] = Trim(SubBlock);
					iNewPos += 1;
					SubCnt = 0;
					SubBlock = "";
				}
				if (SubCnt > 0) {
					aTmp[iNewPos] = Trim(SubBlock);
					iNewPos += 1;
					SubCnt = 0;
					SubBlock = "";
				}
				Array.Resize(ref aTmp, iNewPos);
				sRes = Join(aTmp, ",");
				sRes = "var aAnim" + ProcFilesCounter + "=[\"" + Replace(sRes, ",", "\",\"", 1, -1, CompareMethod.Text) + "\"];";
			}
			string sOut = "";
			if (bHTMLComplete == true) {
				sOut += "<html><head>\r\n";
				sOut += ConverterSupport.InputOutput.BuildCSSforHTML();
				sOut += "</head><body>\r\n";
			}

			sOut += "<div class=ANSICSS id=\"anm" + ProcFilesCounter + "\"><pre>\r\n";
			iLoop = 0;
			for (int a = 1; a <= 25; a++) {
				for (int b = minX; b <= maxX; b++) {
					sOut += "<span class=\"II\" id=\"" + Hex(iLoop) + "\">&nbsp;</span>";
					iLoop += 1;
				}
				sOut += Environment.NewLine;
			}
			sOut += Environment.NewLine + "</pre></div>\r\n";

			sOut += "<script language=\"Javascript\">\r\n";

			string sJS;
			//= ByteArrayToString(Resources.ANSIJS)
			sJS = ConverterSupport.Convert.ByteArrayToStr(Resources.ANSIJSBA, FFormats.us_ascii); //NEw VS makes it a string not byte[] Added another file for now with diff ext
            
			//sJS = ByteArrayToStr(Resources.ANSIJS2, FFormats.us_ascii)

			sJS = ConverterSupport.Convert.CutorSandR(sJS, "//VARDEFSTART", "//VARDEFEND", "I", "I", "C", "1");
			sJS = ConverterSupport.Convert.CutorSandR(sJS, "//CALLSTART", "//CALLEND", "I", "I", "C", "1");

			sJS = Replace(sJS, "//AANIMVAR", sRes, 1, 1, CompareMethod.Text);

			string sMap = "";
			sMap += "var aMappings" + ProcFilesCounter + "=[";
			for (int a = 0; a <= 255; a++) {
				sMap += "\"" + HTMLUniEncode(a) + "\"";
				if (a < 255) {
					sMap += ",";
				}
			}
			sMap += "];\r\n";
			sJS = Replace(sJS, "//AMAPPINGSVAR", sMap, 1, 1, CompareMethod.Text);

			string sFN = "StartAnsiAnimation('anm" + ProcFilesCounter + "',aAnim" + ProcFilesCounter + ",aMappings" + ProcFilesCounter + ");\r\n";
			sJS = Replace(sJS, "//CALLPLACEHOLDER", sFN, 1, 1, CompareMethod.Text);

			sOut += sJS;
			sOut += "</script>\r\n";

			if (bHTMLComplete == true) {
				sOut += "</body></html>";
			}

			return (string[]) ConverterSupport.InputOutput.WriteFile(sOutFile, sOut, bForceOverwrite, OutputFileExists, false, false);

		}
		static string escapchr(string sChar)
		{
			if (sChar == "\\") {
				return sChar + sChar;
			} else {
				return sChar;
			}
		}
		static string HTMLUniEncode(int iChr)
		{
			if (InternalConstants.aSpecH[iChr] != "") {
				return InternalConstants.aSpecH[iChr];
			} else {
				if ((int)Int(InternalConstants.aUniCode[iChr]) != Int(iChr) | iChr == 44) {
					return "&#" + InternalConstants.aUniCode[iChr] + ";";
				} else {
					if (iChr == 92) {
						return (Chr(iChr) + Chr(iChr)).ToString();
					} else {
						if (iChr >= 0 & iChr <= 32) {
							return "&#" + 9216 + iChr.ToString() + ";";
						} else {
							return Chr(iChr).ToString();
						}
					}
				}
			}
		}
	}
}