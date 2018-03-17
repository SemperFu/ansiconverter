
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


	public class WC2
	{
		public void ProcessWC2File(byte[] aFile)
		{
			ProcessWC2File("", aFile);
		}


		public void ProcessWC2File(string sFile, byte[] aFile = null)
		{
			byte[] aAnsi = null;
			int AnsiMode = 0;
			bool bDrawChar;
			int CurChr;
			int iLoop2;
			string sStr = "";
			int aRRSize;
			string sEsc;
			bool bEnde;
			string[] aPar;
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
									if (ConverterSupport.Convert.SetChar(Chr(27).ToString()) == false) {
										iLoop = UBound(aAnsi) + 1;
									} else {
										bDrawChar = true;
										AnsiMode = 0;
									}
								}
                                break;
							case 2:


								aRRSize = UBound(aAnsi);
								sEsc = Chr(CurChr).ToString();
								//0-9 or ; or ?
								if ((CurChr >= 48 & CurChr <= 57) | CurChr == 59 | CurChr == 63) {
									bEnde = false;
									while (bEnde == false) {
										iLoop += 1;
										if (iLoop > aRRSize) {
											bEnde = true;
										} else {
											CurChr = aAnsi[iLoop];
											sEsc += (string)Chr(CurChr).ToString();
											//A-Z, a-z
											if ((CurChr >= 65 & CurChr <= 90) | (CurChr >= 97 & CurChr <= 122) | CurChr == 27) {
												bEnde = true;
											}
										}
										if (CurChr == 27) {
											AnsiMode = 1;
											bDrawChar = false;
										}
									}
								}

								if (AnsiMode == 2) {
									if (bDebug) {
										Console.WriteLine("." + sEsc + ".");
									}
									aPar = BuildParams(sEsc);
									int NumParams = UBound(aPar);
									if (Chr(CurChr).ToString() == "m" & NumParams == 3) {
										int iPar1;
										int iPar2;
										int iPar3;
										iPar1 = Convert.ToInt32(aPar[1]);
										iPar2 = Convert.ToInt32(aPar[2]);
										iPar3 = Convert.ToInt32(aPar[3]);
										if (iPar1 == 0 & iPar2 == 0 & iPar3 == 40) {
											ForeColor = 7;
											BackColor = 0;
											Blink = false;
											Bold = 0;
											Reversed = false;
										} else {
											if (iPar1 == 1) {
												Bold = 8;
											} else {
												Bold = 0;
											}
											switch (iPar2) {
												case 30:
													ForeColor = 0;
                                                    break;
												case 31:
													ForeColor = 4;
                                                    break;
                                                case 32:
													ForeColor = 2;
                                                    break;
                                                case 33:
													ForeColor = 6;
                                                    break;
                                                case 34:
													ForeColor = 1;
                                                    break;
                                                case 35:
													ForeColor = 5;
                                                    break;
                                                case 36:
													ForeColor = 3;
                                                    break;
                                                case 37:
													ForeColor = 7;
                                                    break;
                                            }
											switch (iPar3) {
												case 40:
													BackColor = 0;
                                                    break;
                                                case 41:
													BackColor = 4;
                                                    break;
                                                case 42:
													BackColor = 2;
                                                    break;
                                                case 43:
													BackColor = 6;
                                                    break;
                                                case 44:
													BackColor = 1;
                                                    break;
                                                case 45:
													BackColor = 5;
                                                    break;
                                                case 46:
													BackColor = 3;
                                                    break;
                                                case 47:
													BackColor = 7;
                                                    break;
                                            }
										}
									} else {
										if (bDebug) {
											Console.WriteLine("Unsupported WildCat 2.X Command: " + Chr(CurChr).ToString() + " or invalid number of parameters: " + NumParams.ToString());
										}
									}
									bDrawChar = false;
									AnsiMode = 0;

								}
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

		//----------------------------------------------------
		public string[] BuildParams(string str)
		{
			int pcount = 0;
			int iLp2 = 0;
			string sWork = str;
			string sCurr = "";
			bool bEnde2 = false;
			string[] aParams = null;
			 // ERROR: Not supported in C#: ReDimStatement

			aParams[0] = 0;
			if (sWork.Length > 1) {
				while (sWork.Length > 0) {
					iLp2 = 0;
					sCurr = "";
					bEnde2 = false;
					while (bEnde2 == false & iLp2 < sWork.Length) {
						//0-9 or ?
						if ((Asc(Mid(sWork, iLp2 + 1, 1)) >= 48 & Asc(Mid(sWork, iLp2 + 1, 1)) <= 57) | Asc(Mid(sWork, iLp2 + 1, 1)) == 63) {
							sCurr += (string)Mid(sWork, iLp2 + 1, 1);
							iLp2 += 1;
						} else {
							iLp2 += 1;
							bEnde2 = true;
						}
						if (iLp2 >= Len(sWork)) {
							bEnde2 = true;
						}
					}
					if (iLp2 > 0) {
						sWork = Right(sWork, sWork.Length - iLp2);
					}
					pcount += 1;
					Array.Resize(ref aParams, pcount + 1);
					aParams[pcount] = sCurr;
				}
			}
			//System.Windows.Forms.Application.DoEvents()
			return aParams;
		}
	}
}