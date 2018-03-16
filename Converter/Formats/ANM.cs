﻿namespace MediaFormats
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

		public string[] ProcessANSIAnimationFile(string sFile, string sOutFile, byte[] aFile = null)
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
			object aOutAnm;
			 // ERROR: Not supported in C#: ReDimStatement

			object iCmdCnt;
			iCmdCnt = 0;
			if (!aFile == null) {
				aAnsi = ConverterSupport.MergeByteArrays(ConverterSupport.NullByteArray, aFile);
			} else {
				if (IO.File.Exists(sFile)) {
					aAnsi = ConverterSupport.ReadBinaryFile(sFile);
					aAnsi = ConverterSupport.MergeByteArrays(ConverterSupport.NullByteArray, aAnsi);
				}
			}

			ConverterSupport.BuildMappings(sCodePg);
			System.Windows.Forms.Application.DoEvents();
			ForeColor = 7;
			BackColor = 0;
			LineWrap = true;
			Blink = false;
			Bold = 0;
			Reversed = false;
			LinesUsed = 0;

			XPos = minX;
			YPos = minY;
			if (!aAnsi == null) {
				if (UBound(aAnsi) > 0) {
					iLoop = 1;
					while (iLoop <= UBound(aAnsi)) {
						if (iCmdCnt > UBound(aOutAnm) - 100) {
							Array.Resize(ref aOutAnm, UBound(aOutAnm) + 1001);
						}
						bDrawChar = true;
						CurChr = (int)aAnsi(iLoop);
						switch (AnsiMode) {
							case 0:
								//ESC
								if (CurChr == 27) {
									AnsiMode = 1;
									bDrawChar = false;
								}
								//SUB or "S"
								if (CurChr == 26 | CurChr == 83) {
									int iSauceOffset = IIf(CurChr == 83, 1, 0);
									if (UBound(aAnsi) >= iLoop + 128 - iSauceOffset) {
										sStr = "";
										for (iLoop2 = 1 - iSauceOffset; iLoop2 <= 5 - iSauceOffset; iLoop2++) {
											sStr += Chr(aAnsi(iLoop + iLoop2));
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
								//[
								if (CurChr == 91) {
									AnsiMode = 2;
									bDrawChar = false;
								} else {
									aOutAnm(iCmdCnt) = "X" + Hex(27);
									iCmdCnt += 1;
									bDrawChar = true;
									AnsiMode = 0;
								}
							case 2:
								object aRRSize;
								aRRSize = UBound(aAnsi);
								sEsc = Chr(CurChr);
								//0-9 or ;
								if ((CurChr >= 48 & CurChr <= 57) | CurChr == 59) {
									bEnde = false;
									while (bEnde == false) {
										iLoop += 1;
										if (iLoop > aRRSize) {
											bEnde = true;
										} else {
											CurChr = aAnsi(iLoop);
											sEsc += (string)Chr(CurChr);
											if ((CurChr >= 65 & CurChr <= 90) | (CurChr >= 97 & CurChr <= 122)) {
												bEnde = true;
											}
										}
									}
								}
								aPar = oAnsi.BuildParams(sEsc);
								int NumParams = UBound(aPar);
								switch (Chr(CurChr)) {
									case "A":
										//Move Cursor Up
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.ChkNum(aPar(1));
											if (iLoop2 == 0) {
												iLoop2 = 1;
											}
										} else {
											iLoop2 = 1;
										}
										aOutAnm(iCmdCnt) = "A" + Right("0" + Hex(iLoop2), 2);
										iCmdCnt += 1;
									case "B":
										//Move Cursor Down
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.ChkNum(aPar(1));
											if (iLoop2 == 0) {
												iLoop2 = 1;
											}
										} else {
											iLoop2 = 1;
										}
										YPos = YPos + iLoop2;
										aOutAnm(iCmdCnt) = "B" + Right("0" + Hex(iLoop2), 2);
										iCmdCnt += 1;
									case "C":
										//Move Cursor Right
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.ChkNum(aPar(1));
											if (iLoop2 == 0) {
												iLoop2 = 1;
											}
										} else {
											iLoop2 = 1;
										}
										aOutAnm(iCmdCnt) = "C" + Right("0" + Hex(iLoop2), 2);
										iCmdCnt += 1;
									case "D":
										//Move Cursor left
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.ChkNum(aPar(1));
											if (iLoop2 == 0) {
												iLoop2 = 1;
											}
										} else {
											iLoop2 = 1;
										}
										aOutAnm(iCmdCnt) = "D" + Right("0" + Hex(iLoop2), 2);
										iCmdCnt += 1;
									case "H":
										//Move Cursor
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.ChkNum(aPar(1));
											YPos = IIf(iLoop2 > 0, minY + (iLoop2 - 1), minY);
										} else {
											YPos = minY;
										}
										if (NumParams > 1) {
											iLoop2 = ConverterSupport.ChkNum(aPar(2));
											XPos = IIf(iLoop2 > 0, minX + (iLoop2 - 1), minX);
										} else {
											XPos = minX;
										}
										aOutAnm(iCmdCnt) = "P" + escapchr(Chr(XPos + 45)) + escapchr(Chr(YPos + 96));
										iCmdCnt += 1;
									case "f":
										//Move Cursor
										if (NumParams > 0) {
											iLoop2 = ConverterSupport.ChkNum(aPar(1));
											YPos = IIf(iLoop2 > 0, minY + (iLoop2 - 1), minY);
										} else {
											YPos = minY;
										}
										if (NumParams > 1) {
											iLoop2 = ConverterSupport.ChkNum(aPar(2));
											XPos = IIf(iLoop2 > 0, minX + (iLoop2 - 1), minX);
										} else {
											XPos = minX;
										}
										aOutAnm(iCmdCnt) = "P" + escapchr(Chr(XPos + 45)) + escapchr(Chr(YPos + 96));
										iCmdCnt += 1;
									case "J":
										//ForeColor = 7 : BackColor = 0 : Blink = False : Bold = 0 : Reversed = False
										if (NumParams > 0) {
											switch (ConverterSupport.ChkNum(aPar(1))) {
												case 0:
													//erase from cursor to end of screen
													aOutAnm(iCmdCnt) = "K";
													iCmdCnt += 1;
												case 1:
													//Erase from beginning of screen to cursor
													aOutAnm(iCmdCnt) = "J";
													iCmdCnt += 1;
												case 2:
													//Clear screen and home cursor
													aOutAnm(iCmdCnt) = "F";
													iCmdCnt += 1;
											}
										//Erase from cursor to end of screen
										} else {
											aOutAnm(iCmdCnt) = "K";
											iCmdCnt += 1;

										}

									case "m":
										//Set Attribute
										if (NumParams == 0) {
											ForeColor = 7;
											BackColor = 0;
											Blink = false;
											Bold = 0;
											Reversed = false;
											aOutAnm(iCmdCnt) = "T07";
											iCmdCnt += 1;
											aOutAnm(iCmdCnt) = "L0";
											iCmdCnt += 1;
										}
										for (iLoop3 = 1; iLoop3 <= NumParams; iLoop3++) {
											object i2;
											iLoop2 = ConverterSupport.ChkNum(aPar(iLoop3));
											switch (iLoop2) {
												case 0:
													if ((string)Left(aPar(iLoop3), 1) == "0") {
														ForeColor = 7;
														BackColor = 0;
														Blink = false;
														Bold = 0;
														Reversed = false;
														aOutAnm(iCmdCnt) = "T07";
														iCmdCnt += 1;
														aOutAnm(iCmdCnt) = "L0";
														iCmdCnt += 1;
													}
												case 1:
													Bold = 8;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 2:
													Bold = 0;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 5:
													Blink = true;
													aOutAnm(iCmdCnt) = "L1";
													iCmdCnt += 1;
												case 7:
													i2 = ForeColor;
													ForeColor = BackColor;
													BackColor = i2;
													Reversed = true;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 22:
													Bold = 0;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 25:
													Blink = false;
													aOutAnm(iCmdCnt) = "L0";
													iCmdCnt += 1;
												case 27:
													i2 = ForeColor;
													ForeColor = BackColor;
													BackColor = i2;
													Reversed = false;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 30:
													ForeColor = 0;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 31:
													ForeColor = 4;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 32:
													ForeColor = 2;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 33:
													ForeColor = 6;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 34:
													ForeColor = 1;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 35:
													ForeColor = 5;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 36:
													ForeColor = 3;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 37:
													ForeColor = 7;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 40:
													BackColor = 0;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 41:
													BackColor = 4;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 42:
													BackColor = 2;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 43:
													BackColor = 6;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 44:
													BackColor = 1;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 45:
													BackColor = 5;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 46:
													BackColor = 3;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
												case 47:
													BackColor = 7;
													aOutAnm(iCmdCnt) = "T" + Hex(BackColor) + Hex(ForeColor + Bold);
													iCmdCnt += 1;
											}
										}

									case "K":
										if (NumParams > 0) {
											switch (ConverterSupport.ChkNum(aPar(1))) {
												case 0:
													//clear to the end of the line
													aOutAnm(iCmdCnt) = "G";
													iCmdCnt += 1;
												case 1:
													//Erase from beginning of line to cursor
													aOutAnm(iCmdCnt) = "H";
													iCmdCnt += 1;
												case 2:
													//Erase line containing the cursor
													aOutAnm(iCmdCnt) = "I";
													iCmdCnt += 1;
											}
										} else {
											//clear to the end of the line
											aOutAnm(iCmdCnt) = "G";
											iCmdCnt += 1;
										}
									case "s":
										//save cursor position
										aOutAnm(iCmdCnt) = "S";
										iCmdCnt += 1;
									case "u":
										//Restore Position
										aOutAnm(iCmdCnt) = "R";
										iCmdCnt += 1;
									case "n":
										//Report Cursor Position
										aOutAnm(iCmdCnt) = "W01";
										iCmdCnt += 1;
										//Set Display Mode
									case "h":
									//<[=Xh   X = Mode, 7 = Turn ON linewrap

										//Set Display Mode
									case "l":
									//<[=7l  = Truncate Lines longer than 80 chars
									default:

										Console.WriteLine("Unknown ANSI Command: " + Chr(CurChr) + " (" + CurChr.ToString + ") Params: " + Join(aPar, "|").ToString + ",File: " + sFile + ", Pos: " + iLoop.ToString);
								}
								bDrawChar = false;
								AnsiMode = 0;
						}

						if (bDrawChar == true & AnsiMode == 0) {
							switch (CurChr) {
								case 10:
									aOutAnm(iCmdCnt) = "N";
									iCmdCnt += 1;
									// restore X in linefeed so's to support *nix type files
								case 13:
								// XPos = 1
								case 32:
									aOutAnm(iCmdCnt) = "Z01";
									iCmdCnt += 1;
									//ignore
								case 26:
								default:
									// If CurChr >= 1 Then
									aOutAnm(iCmdCnt) = "X" + Hex(CurChr);
									iCmdCnt += 1;
								//End If
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
			string[] aTmp;
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
					sCurCmd = Microsoft.VisualBasic.Left(aOutAnm(iLoop2), 1);
					if (sPrevCmd != sCurCmd) {
						if (sPrevCmd != "") {
							switch (sPrevCmd) {
								//Case "P" : SubBlock &= " " & sCombStr : SubCnt += 1
								case "T":
									SubBlock += " " + sCombStr;
									SubCnt += 1;
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
								case "W":
									SubBlock += " W" + Right("0" + Hex(ExecCnt), 2);
									SubCnt += 1;
								case "Z":
									SubBlock += " Z" + Right("0" + Hex(ExecCnt), 2);
									SubCnt += 1;
								case "X":
									SubBlock += IIf(ExecCnt > 1, " x" + sCombStr, " X" + sCombStr);
									SubCnt += 1;
								default:
								//shouldn't be here
							}
							if (SubCnt > 1000) {
								aTmp(iNewPos) = Trim(SubBlock);
								iNewPos += 1;
								SubCnt = 0;
								SubBlock = "";
							}
						}
						sCombStr = "";
						ExecCnt = 1;
						switch (sCurCmd) {
							//Case "P" : sCombStr = aOutAnm(iLoop2) : sPrevCmd = sCurCmd
							case "T":
								sCombStr = aOutAnm(iLoop2);
								sPrevCmd = sCurCmd;
							case "S":
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
							case "R":
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
							case "W":
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
							case "Z":
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
							case "X":
								sCombStr = Right(aOutAnm(iLoop2), aOutAnm(iLoop2).ToString.Length - 1);
								ExecCnt = 1;
								sPrevCmd = sCurCmd;
							default:
								SubBlock += " " + aOutAnm(iLoop2);
								SubCnt += 1;
								sPrevCmd = "";
						}
					} else {
						switch (sCurCmd) {
							//Case "P" : sCombStr = aOutAnm(iLoop2)
							case "T":
								sCombStr = aOutAnm(iLoop2);
							case "S":
								ExecCnt += 1;
							case "R":
								ExecCnt += 1;
							case "W":
								ExecCnt += 1;
							case "Z":
								ExecCnt += 1;
							case "X":
								sCombStr += ";" + Right(aOutAnm(iLoop2), aOutAnm(iLoop2).ToString.Length - 1);
								ExecCnt += 1;
						}
					}
				}
				if (sPrevCmd != "") {
					switch (sPrevCmd) {
						//Case "P" : SubBlock &= " " & sCombStr : SubCnt += 1
						case "T":
							SubBlock += " " + sCombStr;
							SubCnt += 1;
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
						case "W":
							SubBlock += " W" + Right("0" + Hex(ExecCnt), 2);
							SubCnt += 1;
						case "Z":
							SubBlock += " Z" + Right("0" + Hex(ExecCnt), 2);
							SubCnt += 1;
						case "X":
							SubBlock += IIf(ExecCnt > 1, " x" + sCombStr, " X" + sCombStr);
							SubCnt += 1;
						default:
							//shouldn't be here
							Console.WriteLine("shouldn't be here. sPrevCmd='" + sPrevCmd + "'");
					}

					aTmp(iNewPos) = Trim(SubBlock);
					iNewPos += 1;
					SubCnt = 0;
					SubBlock = "";
				}
				if (SubCnt > 0) {
					aTmp(iNewPos) = Trim(SubBlock);
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
				sOut += ConverterSupport.BuildCSSforHTML();
				sOut += "</head><body>\r\n";
			}

			sOut += "<div class=ANSICSS id=\"anm" + ProcFilesCounter + "\"><pre>\r\n";
			iLoop = 0;
			for (a = 1; a <= 25; a++) {
				for (b = minX; b <= maxX; b++) {
					sOut += "<span class=\"II\" id=\"" + Hex(iLoop) + "\">&nbsp;</span>";
					iLoop += 1;
				}
				sOut += vbCrLf;
			}
			sOut += vbCrLf + "</pre></div>\r\n";

			sOut += "<script language=\"Javascript\">\r\n";

			string sJS;
			//= ByteArrayToString(My.Resources.ANSIJS)
			sJS = ConverterSupport.ByteArrayToStr(My.Resources.ANSIJS, FFormats.us_ascii);
			//sJS = ByteArrayToStr(My.Resources.ANSIJS2, FFormats.us_ascii)

			sJS = ConverterSupport.CutorSandR(sJS, "//VARDEFSTART", "//VARDEFEND", "I", "I", "C", 1);
			sJS = ConverterSupport.CutorSandR(sJS, "//CALLSTART", "//CALLEND", "I", "I", "C", 1);

			sJS = Replace(sJS, "//AANIMVAR", sRes, 1, 1, CompareMethod.Text);

			string sMap = "";
			sMap += "var aMappings" + ProcFilesCounter + "=[";
			for (a = 0; a <= 255; a++) {
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

			return ConverterSupport.WriteFile(sOutFile, sOut, bForceOverwrite, OutputFileExists, false, false);

		}
		string escapchr(string sChar)
		{
			if (sChar == "\\") {
				return sChar + sChar;
			} else {
				return sChar;
			}
		}
		string HTMLUniEncode(int iChr)
		{
			if (Internal.aSpecH(iChr) != "") {
				return Internal.aSpecH(iChr);
			} else {
				if (Int(Internal.aUniCode(iChr)) != Int(iChr) | iChr == 44) {
					return "&#" + Internal.aUniCode(iChr) + ";";
				} else {
					if (iChr == 92) {
						return Chr(iChr) + Chr(iChr);
					} else {
						if (iChr >= 0 & iChr <= 32) {
							return "&#" + (string)9216 + iChr + ";";
						} else {
							return Chr(iChr);
						}
					}
				}
			}
		}
	}
}