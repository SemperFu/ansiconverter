using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Text;
using Microsoft.VisualBasic;
using Internal;

using System.Linq;
using static Data;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.Interaction;

using Converter.Properties;


namespace ConverterSupport
{


	public class InputOutput
	{


		public static byte[] ReadBinaryFile(string sFile)
		{
			byte[] bte;
			System.FileStream ofile;
			int iSize = 0;
			if (File.Exists(sFile)) {
				ofile = File.Open(sFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				ofile.Seek(0, SeekOrigin.Begin);
				iSize = ofile.Length - 1;
				 // ERROR: Not supported in C#: ReDimStatement

				ofile.Read(bte, 0, iSize + 1);
				ofile.Close();
				return bte;
			} else {
				return null;
			}
		}


		/// <summary>
		/// Reads a text file and returns the content as string)
		/// </summary>
		/// <param name="sFile">Path/File Name of Textfile to read</param>
		/// <returns>String</returns>
		public static string ReadFile(string sFile)
		{
			string sAll = "";
			byte[] Bte;
			try {
				// Create an instance of StreamReader to read from a file.
				// The using statement also closes the StreamReader.
				//File.GetAttributes(sFile).
				using (StreamReader sr = new StreamReader(sFile, true)) {
					// Dim encoding As New System.Text.ASCIIEncoding
					 // ERROR: Not supported in C#: ReDimStatement

					sr.BaseStream.Read(Bte, 0, sr.BaseStream.Length);
					//sAll = encoding.GetString(Bte)
					sAll = ConverterSupport.ByteArrayToString(Bte);
				}
			} catch (Exception e) {
			}

			return sAll;
		}
		/// <summary>
		/// Write a string value to the specified output file (with extended options)
		/// </summary>
		/// <param name="OutFileName">Path/File Name of the output text file</param>
		/// <param name="sStr">The string value to export</param>
		/// <param name="bForceOverwrite">Force Overwriting of an existing file at the specified Path/File Name location</param>
		/// <param name="iOutExists">Extended Options for Existing File Handling
		/// 
		/// <para>
		/// Output Array Structure 
		/// Dimension   Usage
		///    0        Result Code (Int), See List below
		///    1        Path and Name of Output File Written (if there was one written)
		///    2        Info/Error Message in Text Format
		/// </para>
		/// <para>iOutExists - General Program Settings for Existing Output Files</para>
		/// <para>  0 = Overwrite </para>
		/// <para>  1 = Skip (no Output)</para>
		/// <para>  2 = Auto Rename New File (Path\Filename[x].Ext)</para>
		/// <para>  3 = Ask (Message Dialog)</para>
		/// <para>  4 = Auto Rename Existing File</para>
		/// 
		///</param>
		///<returns>Response Message Array (3 dimensions) (0) = Return Code*, (1) Written File Name, (2) Extended Info/Error Message</returns>
		///<remarks>* Result Codes:
		/// 
		/// <para>-4 - Error Writing Output File</para>
		/// <para> 0 - Output Written, did not exist</para>
		/// <para> 1 - Output Written, Existing Overwritten, Force Overwrite Enabled</para>
		/// <para> 2 - Output Written, Existing Overwritten, iOutExists = Overwrite</para>
		/// <para> 3 - Output Skipped, No Output, iOutExists = Skip</para>
		/// <para> 4 - Output Written, Existing Overwritten, iOutExists = Ask, Result = Overwrite</para>
		/// <para> 5 - Output Skipped, No Output, iOutExists = Ask, Result = Skip</para>
		/// <para> 6 - Output Written, Different File Name, iOutExists = Auto Rename New File</para>
		/// <para> 7 - Output Written, Existing File Renamed, iOutExists = Auto Rename Existing File</para>
		/// 
		///</remarks>
		public static object WriteFile(string OutFileName, object sStr, bool bForceOverwrite = false, int iOutExists = 0, bool m_NoMsg = false, bool isBinary = false)
		{
			bool bProceed = true;
			string SF = "";
			string sP = "";
			string sE = "";
			string sB = "";
			int DC = 1;
			string sWorkFN = "";
			string[] aResult = new string[2];
			string sParentDir = "";

			sParentDir = Path.GetDirectoryName(OutFileName);
			if (Directory.Exists(sParentDir) == false && sParentDir != "") {
				Directory.CreateDirectory(sParentDir);
			}

			if (File.Exists(OutFileName)) {
				if (bForceOverwrite == true) {
					try {
						File.Delete(OutFileName);
						bProceed = true;
						aResult[0] = "1";
						aResult[1] = OutFileName;
						aResult[2] = "  - [b]Force Overwrite enabled.[/b] \r\n" + "  - Existing Output file overwritten.";
					} catch (Exception ex) {
						bProceed = false;
						aResult[0] = "-4";
						aResult[1] = OutFileName;
						aResult[2] = "  - Unable to Delete Existing Output File [i]'" + OutFileName + "'[/i].\r\n" + "  - File Skipped.";
					}
				} else {
					switch (iOutExists) {
						case 0:
							//Delete Output File, if it already exists
							try {
								File.Delete(OutFileName);
								bProceed = true;
								aResult[0] = "2";
								aResult[1] = OutFileName;
								aResult[2] = "  - Program Setting [i]'Overwrite Existing Output Files'[/i].\r\n" + "  - Existing Output file overwritten.";
							} catch (Exception ex) {
								bProceed = false;
								aResult[0] = "-4";
								aResult[1] = OutFileName;
								aResult[2] = "  - Unable to Delete Existing Output File [i]'" + OutFileName + "'[/i].\r\n" + "  - File Skipped.";

							}
                            break;
						case 1:
							//Skip
							bProceed = false;
							aResult[0] = "3";
							aResult[1] = "";
							aResult[2] = "  - Program Setting [i]'Skip Existing Output Files'[/i].\r\n" + "  - Output [b]'" + OutFileName + "'[/b] Skipped.";
                            break;
                        case 3:
							//Ask    
							if (MsgBox("Output File: " + OutFileName + " already exists.\r\n" + "Do you want to overwrite it?", vbYesNo, "File Exists") == vbYes) {
								File.Delete(OutFileName);
								bProceed = true;
								aResult[0] = "4";
								aResult[1] = OutFileName;
								aResult[2] = "  - Manual Decision [i]'Overwrite Existing Output File'[/i].\r\n" + "  - Existing Output file overwritten.";
							} else {
								bProceed = false;
								aResult[0] = "5";
								aResult[1] = "";
								aResult[2] = "  - Manual Decision [i]'Skip Existing Output File'[/i].\r\n" + "  - Output [b]'" + OutFileName + "'[/b] Skipped.";
							}
                            break;
                        default:
							//Auto-Ren
							SF = Path.GetFileNameWithoutExtension(OutFileName) + Path.GetExtension(OutFileName);
							sP = Strings.Left(OutFileName, Len(OutFileName) - Len(SF) - 1);
							sE = Path.GetExtension(OutFileName);
							sB = Strings.Left(SF, Len(SF) - Len(sE));
							DC = 1;
							sWorkFN = OutFileName;
							while (File.Exists(sWorkFN) == true) {
								DC += 1;
								sWorkFN = Path.Combine(sP, sB + "(" + DC + ")" + sE);
							}
							//Auto-Ren New
							if (iOutExists == 2) {
								aResult[0] = "6";
								aResult[1] = sWorkFN;
								aResult[2] = "  - Program Setting [i]'Auto-Rename New Output Files'[/i].\r\n" + "  - Original Output file name: [b]'" + OutFileName + "'[/b].";
								OutFileName = sWorkFN;
							}
							//Auto-Ren Existing
							if (iOutExists == 4) {
								aResult[0] = "7";
								aResult[1] = OutFileName;
								aResult[2] = "  - Program Setting [i]'Auto-Rename Existing Output Files'[/i]. \r\n" + "  - Exisiting File Renamed to: [b]'" + sWorkFN + "'[/b].";
								File.Move(OutFileName, sWorkFN);
							}
							
                            break;
					}
				}
			} else {
				bProceed = true;
				aResult[0] = "0";
				aResult[1] = OutFileName;
				aResult[2] = "";
			}

			if (bProceed == true) {
				try {
					if (isBinary == false) {
						using (StreamWriter outfile = new StreamWriter(OutFileName)) {
							outfile.Write(sStr);
						}
					} else {
						//
						if (sStr is Bitmap) {
							System.Drawing.Imaging.ImageFormat f = System.Drawing.Imaging.ImageFormat.Png;
							Image img = (Image)sStr;

							int iThWidth = 150;
							double iSFact = iThWidth / img.Width;
							int iThHeight = img.Height * iSFact;

							if (bCreateThumbs == true) {
								switch (iThumbsResOpt) {
									case 0:
										//Scale Percent
										iThWidth = (img.Width / 100) * iThumbsScale;
										iThHeight = (img.Height / 100) * iThumbsScale;
									case 1:
										iThWidth = iThumbsWidth;
										iSFact = iThWidth / img.Width;
										iThHeight = img.Height * iSFact;
									case 2:
										iThHeight = iThumbsHeight;
										iSFact = iThHeight / img.Height;
										iThWidth = img.Width * iSFact;
									case 3:
										iThWidth = iThumbsWidth;
										iThHeight = iThumbsHeight;
								}
								img = img.GetThumbnailImage(iThWidth, iThHeight, null, null);
							}
							switch (LCase(Path.GetExtension(OutFileName))) {
								case ".png":
									f = System.Drawing.Imaging.ImageFormat.Png;
                                    break;
								case ".bmp":
									f = System.Drawing.Imaging.ImageFormat.Bmp;
                                    break;
                                case ".gif":
									f = System.Drawing.Imaging.ImageFormat.Gif;
                                    break;
                                case ".ico":
									f = System.Drawing.Imaging.ImageFormat.Icon;
                                    break;
                                case ".emf":
									f = System.Drawing.Imaging.ImageFormat.Emf;
                                    break;
                                case ".jpg":
									f = System.Drawing.Imaging.ImageFormat.Jpeg;
                                    break;
                                case ".tif":
									f = System.Drawing.Imaging.ImageFormat.Tiff;
                                    break;
                                case ".wmf":
									f = System.Drawing.Imaging.ImageFormat.Wmf;
                                    break;

								default:
									throw new Exception();
                                    break;
                            }
							img.Save(OutFileName, f);
						} else if (sStr is byte[]) {
							long lngByte = sStr.Length;
							FileStream sw2 = new FileStream(OutFileName, FileMode.CreateNew, FileAccess.Write);
							sw2.Write(sStr, 0, lngByte);
							sw2.Close();
						} else {
							throw new Exception();
						}


					}
				} catch (Exception ex) {
					aResult[0] = "-4";
					aResult[1] = OutFileName;
					aResult[2] = ex.Message;
				}

			}
			return aResult;
		}

		public FFormats checkFileFormat(string sFile)
		{
			int ReadTo = 3;
			byte[] Buf = new byte[] {
				0,
				0,
				0
			};
			FFormats sRes = FFormats.us_ascii;
			int i = 0;
			System.FileStream ofile;
			if (File.Exists(sFile)) {
				if (GetFileSizeNum(sFile) > 0) {
					ofile = File.Open(sFile, FileMode.Open, FileAccess.Read);
					ofile.Seek(0, SeekOrigin.Begin);
					while (i < ReadTo) {
						Buf[i] = ofile.ReadByte;
						i += 1;
					}
					ofile.Close();
					if (Buf[0] == 239 & Buf[1] == 187 & Buf[2] == 191) {
						sRes = FFormats.utf_8;
					} else {
						if (Buf[0] == 255 & Buf[1] == 254) {
							sRes = FFormats.utf_16;
						}
					}
				}
			}
			return sRes;
		}
		public FTypes DetermineFileType(string sFile)
		{
			FTypes sRes = FTypes.ASCII;
			byte[] eBte;
			string sData = "";
			System.FileStream ofile;
			if (File.Exists(sFile)) {
				if (GetFileSizeNum(sFile) > 0) {
					ofile = File.Open(sFile, FileMode.Open, FileAccess.Read);
					int iSize = ofile.Length - 1;
					if (iSize > 10000)
						iSize = 10000;
					 // ERROR: Not supported in C#: ReDimStatement

					ofile.Seek(0, SeekOrigin.Begin);
					ofile.Read(eBte, 0, iSize + 1);
					ofile.Close();
					sData = ConverterSupport.ByteArrayToString(eBte);
				}
			}
			if (sData != "" & !sData == null) {
				if (InStr(1, sData,Strings.Chr(27) + "[", CompareMethod.Text) > 0) {
					if (InStr(1, sData,Strings.Chr(27) + "[0;0;40m", CompareMethod.Text) > 0) {
						sRes = FTypes.WC2;
					} else {
						sRes = FTypes.ANSI;
					}
				} else {
					if (InStr(1, sData, "<div", CompareMethod.Text) > 0 | InStr(1, sData, "&lt;", CompareMethod.Text) > 0 | InStr(1, sData, "&nbsp;", CompareMethod.Text) > 0 | InStr(1, sData, ";&#", CompareMethod.Text) > 0) {
						sRes = FTypes.HTML;
					} else {
						if (ConverterSupport.RegExTest(sData, "@X[0-9A-F]{2}", RegularExpressions.RegexOptions.Singleline) == true) {
							sRes = FTypes.PCB;
						} else {
							if (ConverterSupport.RegExTest(sData, "@[0-9A-F]{2}@", RegularExpressions.RegexOptions.Singleline) == true) {
								sRes = FTypes.WC3;
							} else {
								if (InStr(1, sData,Strings.Chr(22) +Strings.Chr(1), CompareMethod.Binary) > 0) {
									sRes = FTypes.AVT;
								} else {
									if (InStr(1, sData,Strings.Chr(7), CompareMethod.Binary) > 0) {
										sRes = FTypes.Bin;
									} else {
										sRes = FTypes.ASCII;
									}
								}

							}
						}
					}
				}
			}
			return sRes;
		}
		public string GetFileSize(string sFile)
		{
			FileInfo oFile;
			oFile = new FileInfo(sFile);
			double lSize = oFile.Length;
			string slSize = "";
			//GB
			if (lSize > 1024000000) {
				slSize = (string)ConverterSupport.USStringRound(lSize / 1024000000, 2) + " GB";
			} else {
				//MB
				if (lSize > 1024000) {
					slSize = (string)ConverterSupport.USStringRound(lSize / 1024000, 2) + " MB";
				} else {
					//KB
					if (lSize > 1024) {
						slSize = (string)ConverterSupport.USStringRound(lSize / 1024, 2) + " KB";
					//Bytes
					} else {
						slSize = (string)lSize + " B";
					}
				}
			}
			return lSize;
		}

		public long GetFileSizeNum(string sFile)
		{
			FileInfo oFile;
			oFile = new FileInfo(sFile);
			return oFile.Length;
			oFile = null;
		}

		public static string DetermineOutputFileName(string sInpFile)
		{
			string sDir = "";
			string sResult = "";

			sInpFile = Path.GetFullPath(sInpFile);
			string sFile = Path.GetFileName(sInpFile);
			string sFileBase = Path.GetFileNameWithoutExtension(sInpFile);
			string sExt = "." + txtExt;

			sDir = Interaction.IIf(Data.rOutPathInput, Path.GetDirectoryName(sInpFile), Data.outPath);
			sFile = Interaction.IIf(Data.rReplaceExt, sFileBase + sExt, sFile + sExt);
			sResult = Path.Combine(sDir, sFile);

			return sResult;
		}
		//----------------------------------------------------
		//  Output Specific Formats from Gloabl "Screen" Array
		//  HTML Enc ASCII
		//  HTML Enc ANSI
		//  PCBoard @
		//  DOS Binary
		//----------------------------------------------------
		public static string[] OutputASCHTML(string sOutFile, string sASC)
		{
			string sOut = "";
			string sObjWidth = (Data.maxX * 8).ToString() + "px";
			byte[] bteWork1;
			int iLp2 = 0;
			int iLen = 0;

			if (Data.bHTMLComplete == true) {
				sOut += "<html><head>\r\n";
				sOut += BuildCSSforHTML();
				sOut += "</head><body>\r\n";
			}
			sOut += "<div class=ANSICSS><pre>\r\n" + "<span>";
			sOut += sASC;
			sOut += "</span>\r\n" + "</pre></div>\r\n";

			if (Data.bHTMLComplete == true) {
				sOut += "</body></html>";
			}
			iLen = Strings.Len(sOut);
			 // ERROR: Not supported in C#: ReDimStatement

			for (iLp2 = 1; iLp2 <= iLen; iLp2++) {
				bteWork1[iLp2 - 1] = Strings.Asc(Strings.Mid(sOut, iLp2, 1));
				System.Windows.Forms.Application.DoEvents();
			}
			if (Data.bOutputSauce == true & Data.bHasSauce == true) {
				bteWork1 = ConverterSupport.Convert.MergeByteArrays(bteWork1, ConverterSupport.StrToByteArray(oSauce.BuildHTML(true)));
			}
			return WriteFile(sOutFile, bteWork1, Data.bForceOverwrite, Data.OutputFileExists, false, true);

		}

		public static string BuildCSSforHTML()
		{
			string sWorkCSS = Data.sCSSDef;
			string sObjWidth = (Data.maxX * 8).ToString() + "px";
			string sOut = "";

			if (Data.bCUFon == true) {
				sOut += "<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">\r\n" + "<script  type=\"text/javascript\" src=\"typeface-0.15.js\"></script>\r\n" + "<script  type=\"text/javascript\" src=\"msdos_fnt_vga_cp437_regular.typeface.js\"></script>\r\n";
			} else {
				if (Data.bAnimation == true) {
                    
                    sWorkCSS = Strings.Replace(sWorkCSS, "%EXTRACSSDIV%", Data.WebFontList[Data.SelectedWebFont].AnimEXTRACSSDIV, 1, -1, CompareMethod.Text);
					sWorkCSS = Strings.Replace(sWorkCSS, "%EXTRACSSPRE%", Data.WebFontList.Item(Data.SelectedWebFont).AnimEXTRACSSPRE, 1, -1, CompareMethod.Text);
					sWorkCSS = Strings.Replace(sWorkCSS, "%EXTRACSSSPAN%", Data.WebFontList.Item(Data.SelectedWebFont).AnimEXTRACSSSPAN, 1, -1, CompareMethod.Text);
				} else {
                    
                    sWorkCSS = Strings.Replace(sWorkCSS, "%EXTRACSSDIV%", Data.WebFontList.Item(Data.SelectedWebFont).StaticEXTRACSSDIV, 1, -1, CompareMethod.Text);
					sWorkCSS = Strings.Replace(sWorkCSS, "%EXTRACSSPRE%", Data.WebFontList.Item(Data.SelectedWebFont).StaticEXTRACSSPRE, 1, -1, CompareMethod.Text);
					sWorkCSS = Strings.Replace(sWorkCSS, "%EXTRACSSSPAN%", Data.WebFontList.Item(Data.SelectedWebFont).StaticEXTRACSSSPAN, 1, -1, CompareMethod.Text);
				}
				sWorkCSS = Strings.Replace(sWorkCSS, "%WIDTH%", sObjWidth, 1, -1, CompareMethod.Text);
				if (Data.bOutputSauce == true & Data.bHasSauce == true) {
					sWorkCSS += Environment.NewLine + InternalConstants.sSauceCSS + Environment.NewLine;
				}
				sOut += "<style>\r\n" + sWorkCSS + Environment.NewLine + "</style>\r\n";
			}
			return sOut;
		}
		public static string[] OutputHTML(string sOutFile)
		{
			string sOut = "";
			string sObjWidth = (Data.maxX * 8).ToString() + "px";
            Data.XPos = Data.minX;
            Data.YPos = Data.minY;
            Data.ForeColor = 7;
            Data.BackColor = 0;
			if (Data.bHTMLComplete == true) {
				sOut += "<html><head>\r\n";
				sOut += BuildCSSforHTML();
				sOut += "</head><body>";
			}

			if (Data.bCUFon == true) {
				sOut += "<div class=\"ANSICSS typeface-js\" style=\"font-family:msdos_fnt_vga_cp437, Regular;font-weight: normal;font-size:1em;" + "width:" + sObjWidth + ";background-color:#000;color:#C6C6C6; padding: 0; margin: 0;\"><pre>\r\n";
			} else {
				sOut += "<div class=ANSICSS><pre>\r\n";
			}
			for (int y = Data.minY; y <= Data.LinesUsed; y++) {
				for (int x = Data.minX; x <= Data.maxX; x++) {
					if (x == Data.minX) {
						sOut += "<span class=\"";
						if (Data.bCUFon == true) {
							sOut += "typeface-js ";
						}
						sOut += InternalConstants.aCSS[0, Data.Screen[x, y].BackColor] + " " + InternalConstants.aCSS[1, Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold];
						sOut += "\">";
					}
					if (Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold != Data.ForeColor | Data.Screen[x, y].BackColor != Data.BackColor) {
						sOut += "</span><span class=\"";
						if (Data.bCUFon == true) {
							sOut += "typeface-js ";
						}
						sOut += InternalConstants.aCSS[0, Data.Screen[x, y].BackColor] + " " + InternalConstants.aCSS[1, Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold] + "\">";
                        Data.ForeColor = Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold;
                        Data.BackColor = Data.Screen[x, y].BackColor;
					}
					if (Data.Screen[x, y].Chr != Strings.Chr(0).ToString()) {
						if (Data.Screen[x, y].Chr != Strings.Chr(255).ToString()) {
							sOut += Data.Screen[x, y].Chr;
						} else {
							sOut += "&nbsp;";
						}
					} else {
						x = Data.maxX;
					}
					if (x == Data.maxX) {
						sOut += "</span>";
					}
				}
				sOut += Environment.NewLine;
				System.Windows.Forms.Application.DoEvents();
			}
			sOut += "</span></pre></div>";
			if (Data.bCUFon == true) {
				sOut += "<script language=\"Javascript\">\r\n" + " _typeface_js.initialize();\r\n" + "</script>\r\n";
			}
			if (Data.bOutputSauce == true & Data.bHasSauce == true) {
				sOut += Data.oSauce.BuildHTML(true);
			}
			if (Data.bHTMLComplete == true) {
				sOut += "</body></html>";
			}

			return WriteFile(sOutFile, sOut, Data.bForceOverwrite, Data.OutputFileExists, false, false);

		}

		public static string[] OutputPCB(string sOutFile)
		{
			string sOut = "";

            Data.XPos = Data.minX;
            Data.YPos = Data.minY;
            Data.ForeColor = 7;
            Data.BackColor = 0;
			sOut = "@X" + Conversion.Hex(Data.BackColor) + Conversion.Hex(Data.ForeColor);

			for (int y = Data.minY; y <= Data.LinesUsed; y++) {
				for (int x = Data.minX; x <= Data.maxX; x++) {
					if (Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold != Data.ForeColor | Data.Screen[x, y].BackColor != Data.BackColor) {
						sOut = sOut + "@X" + Conversion.Hex(Data.Screen[x, y].BackColor) +Conversion.Hex(Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold);
                        Data.ForeColor = Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold;
                        Data.BackColor = Data.Screen[x, y].BackColor;
					}
					if (Data.Screen[x, y].Chr != Strings.Chr(0).ToString()) {
						if (Data.Screen[x, y].Chr != Strings.Chr(255).tostring()) {
							sOut = sOut + Data.Screen[x, y].Chr;
						} else {
							sOut = sOut;
						}
					} else {
						x = Data.maxX;
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				sOut += Environment.NewLine;
				System.Windows.Forms.Application.DoEvents();
			}
			if (Data.bOutputSauce == true & Data.bHasSauce == true) {
				sOut += Data.oSauce.toString();
			}
			return WriteFile(sOutFile, sOut, bForceOverwrite, OutputFileExists, false, false);

		}

		public static string[] OutputAVT(string sOutFile)
		{
			string sOut = "";

			XPos = minX;
			YPos = minY;
			ForeColor = 7;
			BackColor = 0;

			for (int y = minY; y <= LinesUsed; y++) {
				for (int x = minX; x <= maxX; x++) {
					if (Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold != ForeColor | Data.Screen[x, y].BackColor != BackColor) {
                        Data.ForeColor = Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold;
                        Data.BackColor = Data.Screen[x, y].BackColor;
						sOut = sOut +Strings.Chr(22) +Strings.Chr(1) +Strings.Chr(BackColor * 16 + ForeColor);
					}
					if (Data.Screen[x, y].Chr !=Strings.Chr(0)) {
						if (Data.Screen[x, y].Chr !=Strings.Chr(255)) {
							sOut = sOut + Data.Screen[x, y].Chr;
						} else {
							sOut = sOut;
						}
					} else {
						x = maxX;
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				sOut += Environment.NewLine;
				System.Windows.Forms.Application.DoEvents();
			}
			if (Data.bOutputSauce == true & Data.bHasSauce == true) {
				sOut += Data.oSauce.toString();
			}
			return WriteFile(sOutFile, sOut, Data.bForceOverwrite, Data.OutputFileExists, false, false);

		}
		public static string[] OutputWC2(string sOutFile)
		{
			string sOut = "";
			byte[] aWCF = new byte[] {
				30,
				34,
				32,
				36,
				31,
				35,
				33,
				37
			};
			byte[] aWCB = new byte[] {
				40,
				44,
				42,
				46,
				41,
				45,
				43,
				47
			};

			Data.XPos = Data.minX;
            Data.YPos = Data.minY;
            Data.ForeColor = 7;
            Data.BackColor = 0;
            Data.Bold = 0;

			for (int y = Data.minY; Data.y <= Data.LinesUsed; y++) {
				for (int x = Data.minX; x <= Data.maxX; x++) {
					if (Data.Screen[x, y].ForeColor != Data.ForeColor | Data.Screen[x, y].BackColor != Data.BackColor | Data.Screen[x, y].Bold != Data.Bold) {
                        Data.ForeColor = Data.Screen[x, y].ForeColor;
                        Data.Bold = Data.Screen[x, y].Bold;
                        Data.BackColor = Data.Screen[x, y].BackColor;
						if (Data.ForeColor + Data.Bold == 7 & Data.BackColor == 0) {
							sOut = sOut + Strings.Chr(27) + "[0;0;40m";
						} else {
							sOut = sOut + Strings.Chr(27) + "[" + Data.Bold / 8 + ";" + aWCF[Data.ForeColor] + ";" + aWCB[Data.BackColor] + "m";
						}
					}
					if (Data.Screen[x, y].Chr != Strings.Chr(0).ToString()) {
						if (Data.Screen[x, y].Chr != Strings.Chr(255).ToString()) {
							sOut = sOut + Data.Screen[x, y].Chr;
						} else {
							sOut = sOut;
						}
					} else {
						x = Data.maxX;
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				sOut += Environment.NewLine;
				System.Windows.Forms.Application.DoEvents();
			}
			if (Data.bOutputSauce == true & Data.bHasSauce == true) {
				sOut += Data.oSauce.toString();
			}
			return WriteFile(sOutFile, sOut, Data.bForceOverwrite, Data.OutputFileExists, false, false);

		}
		public static string[] OutputWC3(string sOutFile)
		{
			string sOut = "";

            Data.XPos = Data.minX;
            Data.YPos = Data.minY;
            Data.ForeColor = 7;
            Data.BackColor = 0;
			sOut = "@" + Conversion.Hex(Data.BackColor) +Conversion.Hex(Data.ForeColor) + "@";

			for (int y = Data.minY; y <= Data.LinesUsed; y++) {
				for (int x = Data.minX; x <= Data.maxX; x++) {
					if (Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold != Data.ForeColor | Data.Screen[x, y].BackColor != Data.BackColor) {
                        Data.ForeColor = Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold;
                        Data.BackColor = Data.Screen[x, y].BackColor;
						sOut = sOut + "@" +Conversion.Hex(Data.BackColor) +Conversion.Hex(Data.ForeColor) + "@";
					}
					if (Data.Screen[x, y].Chr !=Strings.Chr(0)) {
						if (Data.Screen[x, y].Chr !=Strings.Chr(255)) {
							sOut = sOut + Data.Screen[x, y].Chr;
						} else {
							sOut = sOut;
						}
					} else {
						x = Data.maxX;
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				sOut += Environment.NewLine;
				System.Windows.Forms.Application.DoEvents();
			}
			if (Data.bOutputSauce == true & Data.bHasSauce == true) {
				sOut += Data.oSauce.toString();
			}
			return WriteFile(sOutFile, sOut, Data.bForceOverwrite, Data.OutputFileExists, false, false);

		}

		public static string[] OutputANS(string sOutFile)
		{
			byte[] aOut = new byte[1000];
			int Cnt = 0;
			int iWhatChange = 0;
			int x;
			int y;
			bool bRest = false;
            Data.XPos = Data.minX;
            Data.YPos = Data.minY;
            Data.ForeColor = 7;
            Data.BackColor = 0;
            Data.Bold = 0;
            Data.Reversed = false;
			for (y = Data.minY; y <= Data.LinesUsed; y++) {
				for (x = Data.minX; x <= Data.maxX; x++) {
					if (Data.Screen[x, y].Chr == "-1") {
                        Data.Screen[x, y].Chr =Strings.Chr(32).ToString();
					}
				}
			}

			for (y = Data.minY; y <= Data.LinesUsed; y++) {
				if (y == Data.minY) {
					//<[255D
					aOut[Cnt] = 27;
					Cnt += 1;
					aOut[Cnt] = 91;
					Cnt += 1;
					aOut[Cnt] = 50;
					Cnt += 1;
					aOut[Cnt] = 50;
					Cnt += 1;
					aOut[Cnt] = 53;
					Cnt += 1;
					aOut[Cnt] = 68;
					Cnt += 1;
				}
				bRest = false;
				for (x = Data.minX; x <= Data.maxX; x++) {
					//0 = 48, 1 = 49, 59 = ; 109 = m
					iWhatChange = 0;
					if (x == Data.minX & y > Data.minY) {
						iWhatChange = 14;
					} else {
						if (Data.Screen[x, y].ForeColor != Data.ForeColor) {
							iWhatChange += 2;
						}
						if (Data.Screen[x, y].Bold != Data.Bold) {
							iWhatChange += 4;
						}
						if (Data.Screen[x, y].BackColor != Data.BackColor) {
							iWhatChange += 8;
						}
					}

					if (iWhatChange != 0) {
                        Data.ForeColor = Data.Screen[x, y].ForeColor;
                        Data.Bold = Data.Screen[x, y].Bold;
                        Data.BackColor = Data.Screen[x, y].BackColor;
						//all changed
						if (iWhatChange > 9) {
							aOut[Cnt] = 27;
							Cnt += 1;
							aOut[Cnt] = 91;
							Cnt += 1;
							aOut[Cnt] = Interaction.IIf(Bold != 0, 49, 48);
							Cnt += 1;
							aOut[Cnt] = 59;
							Cnt += 1;
							aOut[Cnt] =Strings.Asc(Strings.Left(InternalConstants.AnsiForeColors[Data.ForeColor], 1));
							Cnt += 1;
							aOut[Cnt] =Strings.Asc(Strings.Right(InternalConstants.AnsiForeColors[Data.ForeColor], 1));
							Cnt += 1;
							aOut[Cnt] = 59;
							Cnt += 1;
							aOut[Cnt] =Strings.Asc(Strings.Left(InternalConstants.AnsiBackColors[Data.BackColor], 1));
							Cnt += 1;
							aOut[Cnt] =Strings.Asc(Strings.Right(InternalConstants.AnsiBackColors[Data.BackColor], 1));
							Cnt += 1;
							aOut[Cnt] = 109;
							Cnt += 1;
						} else {
							//Back color only
							if (iWhatChange == 8) {
								aOut[Cnt] = 27;
								Cnt += 1;
								aOut[Cnt] = 91;
								Cnt += 1;
								aOut[Cnt] =Strings.Asc(Strings.Left(InternalConstants.AnsiBackColors[BackColor], 1));
								Cnt += 1;
								aOut[Cnt] =Strings.Asc(Strings.Right(InternalConstants.AnsiBackColors[BackColor], 1));
								Cnt += 1;
								aOut[Cnt] = 109;
								Cnt += 1;
							} else {
								aOut[Cnt] = 27;
								Cnt += 1;
								aOut[Cnt] = 91;
								Cnt += 1;
								aOut[Cnt] = Interaction.IIf(Data.Bold != 0, 49, 48);
								Cnt += 1;
								aOut[Cnt] = 59;
								Cnt += 1;
								aOut[Cnt] =Strings.Asc(Strings.Left(InternalConstants.AnsiForeColors[Data.ForeColor], 1));
								Cnt += 1;
								aOut[Cnt] =Strings.Asc(Strings.Right(InternalConstants.AnsiForeColors[Data.ForeColor], 1));
								Cnt += 1;
								aOut[Cnt] = 109;
								Cnt += 1;
							}
						}

					}
					if (Data.Screen[x, y].Chr !=Strings.Chr(0).ToString()) {
						if (Data.Screen[x, y].Chr ==Strings.Chr(32).ToString()) {
							int iSpc = NumSpaces(x, y);
							if (iSpc > Data.maxX) {
								//<[XC move cursor right
								aOut[Cnt] = 27;
								Cnt += 1;
								aOut[Cnt] = 91;
								Cnt += 1;
								for (int iLoop2 = 1; iLoop2 <= iSpc.ToString().Length; iLoop2++) {
									aOut[Cnt] =Strings.Asc(Strings.Mid(iSpc.ToString(), iLoop2, 1));
									Cnt += 1;
								}
								aOut[Cnt] = 67;
								Cnt += 1;
								x += iSpc - 1;
							} else {
								aOut[Cnt] =Strings.Asc(Data.Screen[x, y].Chr);
								Cnt += 1;
							}
						} else {
							aOut[Cnt] =Strings.Asc(Data.Screen[x, y].Chr);
							Cnt += 1;
						}
					} else {
						aOut[Cnt] = 32;
						Cnt += 1;
					}
					if (x >= Data.maxX) {
						//<[s
						aOut[Cnt] = 27;
						Cnt += 1;
						aOut[Cnt] = 91;
						Cnt += 1;
						aOut[Cnt] = 115;
						Cnt += 1;
						bRest = true;
					}
					if (Cnt > Information.UBound(aOut) - 20) {
						Array.Resize(ref aOut, Information.UBound(aOut) + 1001);
					}
				}
				//linebreak
				aOut[Cnt] = 13;
				Cnt += 1;
				aOut[Cnt] = 10;
				Cnt += 1;
				if (bRest == true) {
					//<[u
					aOut[Cnt] = 27;
					Cnt += 1;
					aOut[Cnt] = 91;
					Cnt += 1;
					aOut[Cnt] = 117;
					Cnt += 1;

				}

				if (Cnt > Information.UBound(aOut) - 20) {
					Array.Resize(ref aOut, Information.UBound(aOut) + 1001);
				}
			}
			Array.Resize(ref aOut, Cnt + 1);
			if (Data.bOutputSauce == true & Data.bHasSauce == true) {
				aOut = Convert.MergeByteArrays(aOut, Data.oSauce.toByteArray());
			}
			return WriteFile(sOutFile, aOut, Data.bForceOverwrite, Data.OutputFileExists, false, true);
			aOut = null;
		}
		public int NumSpaces(int xOffset, int yOffset)
		{
			int iNums = 1;
			bool bEnd = false;
			int iCnter = xOffset;
			while (bEnd == false) {
				if (Data.maxX >= iCnter + 1) {
					iCnter += 1;
					if (Data.Screen[iCnter, yOffset].Chr ==Strings.Chr(32).ToString()) {
						iNums += 1;
					} else {
						bEnd = true;
					}
				} else {
					bEnd = true;
				}
			}
			return iNums;
		}
		public static string[] OutputBin(string sOutFile)
		{
            Data.XPos = Data.minX;
            Data.YPos = Data.minY;
            Data.ForeColor = 7;
            Data.BackColor = 0;
			byte[] aOut = new byte[(Data.LinesUsed * (Data.maxX * 2)) - 1];
			int Cnt = 0;
			int intC = 0;
			bool bFiller = false;

			for (int y = Data.minY; y <= Data.LinesUsed; y++) {
				for (int x = Data.minX; x <= Data.maxX; x++) {
					if (Data.Screen[x, y].Chr !=Strings.Chr(0).ToString()) {
						if (Data.Screen[x, y].Chr ==Strings.Chr(255).ToString()) {
							bFiller = true;
						}
					} else {
						bFiller = true;
					}
					if (bFiller == true) {
                        Data.ForeColor = Data.DefForeColor;
                        Data.BackColor = Data.DefBackColor;
						intC = 0;
					} else {
                        Data.ForeColor = Data.Screen[x, y].ForeColor + Data.Screen[x, y].Bold;
                        Data.BackColor = Data.Screen[x, y].BackColor;
						intC =Strings.Asc(Data.Screen[x, y].Chr);
					}
					aOut[Cnt] = intC;
					Cnt += 1;
					aOut[Cnt] = System.Convert.ToInt32(Conversion.Hex(Data.BackColor) + Conversion.Hex(Data.ForeColor), 16);
					Cnt += 1;
				}
				bFiller = false;
				System.Windows.Forms.Application.DoEvents();
			}
			if (Data.bOutputSauce == true & Data.bHasSauce == true) {
				aOut = Convert.MergeByteArrays(aOut, Data.oSauce.toByteArray);
			}
			return WriteFile(sOutFile, aOut, Data.bForceOverwrite, Data.OutputFileExists, false, true);
			aOut = null;

		}




	}
}