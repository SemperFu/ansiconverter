using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Text;
namespace ConverterSupport
{


	public class InputOutput
	{


		public byte[] ReadBinaryFile(string sFile)
		{
			byte[] bte;
			System.IO.FileStream ofile;
			int iSize = 0;
			if (IO.File.Exists(sFile)) {
				ofile = IO.File.Open(sFile, IO.FileMode.Open, IO.FileAccess.Read, FileShare.ReadWrite);
				ofile.Seek(0, IO.SeekOrigin.Begin);
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
		public string ReadFile(string sFile)
		{
			string sAll = "";
			byte[] Bte;
			try {
				// Create an instance of StreamReader to read from a file.
				// The using statement also closes the StreamReader.
				//Io.File.GetAttributes(sFile).
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
		public object WriteFile(string OutFileName, object sStr, bool bForceOverwrite = false, int iOutExists = 0, bool m_NoMsg = false, bool isBinary = false)
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

			sParentDir = IO.Path.GetDirectoryName(OutFileName);
			if (IO.Directory.Exists(sParentDir) == false && sParentDir != "") {
				IO.Directory.CreateDirectory(sParentDir);
			}

			if (IO.File.Exists(OutFileName)) {
				if (bForceOverwrite == true) {
					try {
						IO.File.Delete(OutFileName);
						bProceed = true;
						aResult(0) = "1";
						aResult(1) = OutFileName;
						aResult(2) = "  - [b]Force Overwrite enabled.[/b] \r\n" + "  - Existing Output file overwritten.";
					} catch (Exception ex) {
						bProceed = false;
						aResult(0) = "-4";
						aResult(1) = OutFileName;
						aResult(2) = "  - Unable to Delete Existing Output File [i]'" + OutFileName + "'[/i].\r\n" + "  - File Skipped.";
					}
				} else {
					switch (iOutExists) {
						case 0:
							//Delete Output File, if it already exists
							try {
								IO.File.Delete(OutFileName);
								bProceed = true;
								aResult(0) = "2";
								aResult(1) = OutFileName;
								aResult(2) = "  - Program Setting [i]'Overwrite Existing Output Files'[/i].\r\n" + "  - Existing Output file overwritten.";
							} catch (Exception ex) {
								bProceed = false;
								aResult(0) = "-4";
								aResult(1) = OutFileName;
								aResult(2) = "  - Unable to Delete Existing Output File [i]'" + OutFileName + "'[/i].\r\n" + "  - File Skipped.";

							}
						case 1:
							//Skip
							bProceed = false;
							aResult(0) = "3";
							aResult(1) = "";
							aResult(2) = "  - Program Setting [i]'Skip Existing Output Files'[/i].\r\n" + "  - Output [b]'" + OutFileName + "'[/b] Skipped.";
						case 3:
							//Ask    
							if (MsgBox("Output File: " + OutFileName + " already exists.\r\n" + "Do you want to overwrite it?", vbYesNo, "File Exists") == vbYes) {
								IO.File.Delete(OutFileName);
								bProceed = true;
								aResult(0) = "4";
								aResult(1) = OutFileName;
								aResult(2) = "  - Manual Decision [i]'Overwrite Existing Output File'[/i].\r\n" + "  - Existing Output file overwritten.";
							} else {
								bProceed = false;
								aResult(0) = "5";
								aResult(1) = "";
								aResult(2) = "  - Manual Decision [i]'Skip Existing Output File'[/i].\r\n" + "  - Output [b]'" + OutFileName + "'[/b] Skipped.";
							}
						default:
							//Auto-Ren
							SF = IO.Path.GetFileNameWithoutExtension(OutFileName) + IO.Path.GetExtension(OutFileName);
							sP = Left(OutFileName, Len(OutFileName) - Len(SF) - 1);
							sE = IO.Path.GetExtension(OutFileName);
							sB = Left(SF, Len(SF) - Len(sE));
							DC = 1;
							sWorkFN = OutFileName;
							while (IO.File.Exists(sWorkFN) == true) {
								DC += 1;
								sWorkFN = IO.Path.Combine(sP, sB + "(" + DC + ")" + sE);
							}
							//Auto-Ren New
							if (iOutExists == 2) {
								aResult(0) = "6";
								aResult(1) = sWorkFN;
								aResult(2) = "  - Program Setting [i]'Auto-Rename New Output Files'[/i].\r\n" + "  - Original Output file name: [b]'" + OutFileName + "'[/b].";
								OutFileName = sWorkFN;
							}
							//Auto-Ren Existing
							if (iOutExists == 4) {
								aResult(0) = "7";
								aResult(1) = OutFileName;
								aResult(2) = "  - Program Setting [i]'Auto-Rename Existing Output Files'[/i]. \r\n" + "  - Exisiting File Renamed to: [b]'" + sWorkFN + "'[/b].";
								IO.File.Move(OutFileName, sWorkFN);
							}
							bProceed = true;
					}
				}
			} else {
				bProceed = true;
				aResult(0) = "0";
				aResult(1) = OutFileName;
				aResult(2) = "";
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
							switch (LCase(IO.Path.GetExtension(OutFileName))) {
								case ".png":
									f = System.Drawing.Imaging.ImageFormat.Png;
								case ".bmp":
									f = System.Drawing.Imaging.ImageFormat.Bmp;
								case ".gif":
									f = System.Drawing.Imaging.ImageFormat.Gif;
								case ".ico":
									f = System.Drawing.Imaging.ImageFormat.Icon;
								case ".emf":
									f = System.Drawing.Imaging.ImageFormat.Emf;
								case ".jpg":
									f = System.Drawing.Imaging.ImageFormat.Jpeg;
								case ".tif":
									f = System.Drawing.Imaging.ImageFormat.Tiff;
								case ".wmf":
									f = System.Drawing.Imaging.ImageFormat.Wmf;
								default:
									throw new Exception();
							}
							img.Save(OutFileName, f);
						} else if (sStr is byte[]) {
							long lngByte = sStr.Length;
							IO.FileStream sw2 = new IO.FileStream(OutFileName, IO.FileMode.CreateNew, IO.FileAccess.Write);
							sw2.Write(sStr, 0, lngByte);
							sw2.Close();
						} else {
							throw new Exception();
						}


					}
				} catch (Exception ex) {
					aResult(0) = "-4";
					aResult(1) = OutFileName;
					aResult(2) = ex.Message;
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
			System.IO.FileStream ofile;
			if (IO.File.Exists(sFile)) {
				if (GetFileSizeNum(sFile) > 0) {
					ofile = IO.File.Open(sFile, IO.FileMode.Open, IO.FileAccess.Read);
					ofile.Seek(0, IO.SeekOrigin.Begin);
					while (i < ReadTo) {
						Buf(i) = ofile.ReadByte;
						i += 1;
					}
					ofile.Close();
					if (Buf(0) == 239 & Buf(1) == 187 & Buf(2) == 191) {
						sRes = FFormats.utf_8;
					} else {
						if (Buf(0) == 255 & Buf(1) == 254) {
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
			System.IO.FileStream ofile;
			if (IO.File.Exists(sFile)) {
				if (GetFileSizeNum(sFile) > 0) {
					ofile = IO.File.Open(sFile, IO.FileMode.Open, IO.FileAccess.Read);
					int iSize = ofile.Length - 1;
					if (iSize > 10000)
						iSize = 10000;
					 // ERROR: Not supported in C#: ReDimStatement

					ofile.Seek(0, IO.SeekOrigin.Begin);
					ofile.Read(eBte, 0, iSize + 1);
					ofile.Close();
					sData = ConverterSupport.ByteArrayToString(eBte);
				}
			}
			if (sData != "" & !sData == null) {
				if (InStr(1, sData, Chr(27) + "[", CompareMethod.Text) > 0) {
					if (InStr(1, sData, Chr(27) + "[0;0;40m", CompareMethod.Text) > 0) {
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
								if (InStr(1, sData, Chr(22) + Chr(1), CompareMethod.Binary) > 0) {
									sRes = FTypes.AVT;
								} else {
									if (InStr(1, sData, Chr(7), CompareMethod.Binary) > 0) {
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

		public string DetermineOutputFileName(string sInpFile)
		{
			string sDir = "";
			string sResult = "";

			sInpFile = IO.Path.GetFullPath(sInpFile);
			string sFile = IO.Path.GetFileName(sInpFile);
			string sFileBase = IO.Path.GetFileNameWithoutExtension(sInpFile);
			string sExt = "." + txtExt;

			sDir = IIf(rOutPathInput, IO.Path.GetDirectoryName(sInpFile), outPath);
			sFile = IIf(rReplaceExt, sFileBase + sExt, sFile + sExt);
			sResult = IO.Path.Combine(sDir, sFile);

			return sResult;
		}
		//----------------------------------------------------
		//  Output Specific Formats from Gloabl "Screen" Array
		//  HTML Enc ASCII
		//  HTML Enc ANSI
		//  PCBoard @
		//  DOS Binary
		//----------------------------------------------------
		public string[] OutputASCHTML(string sOutFile, string sASC)
		{
			string sOut = "";
			string sObjWidth = (string)maxX * 8 + "px";
			byte[] bteWork1;
			int iLp2 = 0;
			int iLen = 0;

			if (bHTMLComplete == true) {
				sOut += "<html><head>\r\n";
				sOut += BuildCSSforHTML();
				sOut += "</head><body>\r\n";
			}
			sOut += "<div class=ANSICSS><pre>\r\n" + "<span>";
			sOut += sASC;
			sOut += "</span>\r\n" + "</pre></div>\r\n";

			if (bHTMLComplete == true) {
				sOut += "</body></html>";
			}
			iLen = Microsoft.VisualBasic.Len(sOut);
			 // ERROR: Not supported in C#: ReDimStatement

			for (iLp2 = 1; iLp2 <= iLen; iLp2++) {
				bteWork1(iLp2 - 1) = Asc(Mid(sOut, iLp2, 1));
				System.Windows.Forms.Application.DoEvents();
			}
			if (bOutputSauce == true & bHasSauce == true) {
				bteWork1 = ConverterSupport.MergeByteArrays(bteWork1, ConverterSupport.StrToByteArray(oSauce.BuildHTML(true)));
			}
			return WriteFile(sOutFile, bteWork1, bForceOverwrite, OutputFileExists, false, true);

		}

		public string BuildCSSforHTML()
		{
			string sWorkCSS = sCSSDef;
			string sObjWidth = (string)maxX * 8 + "px";
			string sOut = "";

			if (bCUFon == true) {
				sOut += "<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">\r\n" + "<script  type=\"text/javascript\" src=\"typeface-0.15.js\"></script>\r\n" + "<script  type=\"text/javascript\" src=\"msdos_fnt_vga_cp437_regular.typeface.js\"></script>\r\n";
			} else {
				if (bAnimation == true) {
					sWorkCSS = Replace(sWorkCSS, "%EXTRACSSDIV%", WebFontList.Item(SelectedWebFont).AnimEXTRACSSDIV, 1, -1, CompareMethod.Text);
					sWorkCSS = Replace(sWorkCSS, "%EXTRACSSPRE%", WebFontList.Item(SelectedWebFont).AnimEXTRACSSPRE, 1, -1, CompareMethod.Text);
					sWorkCSS = Replace(sWorkCSS, "%EXTRACSSSPAN%", WebFontList.Item(SelectedWebFont).AnimEXTRACSSSPAN, 1, -1, CompareMethod.Text);
				} else {
					sWorkCSS = Replace(sWorkCSS, "%EXTRACSSDIV%", WebFontList.Item(SelectedWebFont).StaticEXTRACSSDIV, 1, -1, CompareMethod.Text);
					sWorkCSS = Replace(sWorkCSS, "%EXTRACSSPRE%", WebFontList.Item(SelectedWebFont).StaticEXTRACSSPRE, 1, -1, CompareMethod.Text);
					sWorkCSS = Replace(sWorkCSS, "%EXTRACSSSPAN%", WebFontList.Item(SelectedWebFont).StaticEXTRACSSSPAN, 1, -1, CompareMethod.Text);
				}
				sWorkCSS = Replace(sWorkCSS, "%WIDTH%", sObjWidth, 1, -1, CompareMethod.Text);
				if (bOutputSauce == true & bHasSauce == true) {
					sWorkCSS += vbCrLf + Internal.sSauceCSS + vbCrLf;
				}
				sOut += "<style>\r\n" + sWorkCSS + vbCrLf + "</style>\r\n";
			}
			return sOut;
		}
		public string[] OutputHTML(string sOutFile)
		{
			string sOut = "";
			string sObjWidth = (string)maxX * 8 + "px";
			XPos = minX;
			YPos = minY;
			ForeColor = 7;
			BackColor = 0;
			if (bHTMLComplete == true) {
				sOut += "<html><head>\r\n";
				sOut += BuildCSSforHTML();
				sOut += "</head><body>";
			}

			if (bCUFon == true) {
				sOut += "<div class=\"ANSICSS typeface-js\" style=\"font-family:msdos_fnt_vga_cp437, Regular;font-weight: normal;font-size:1em;" + "width:" + sObjWidth + ";background-color:#000;color:#C6C6C6; padding: 0; margin: 0;\"><pre>\r\n";
			} else {
				sOut += "<div class=ANSICSS><pre>\r\n";
			}
			for (int y = minY; y <= LinesUsed; y++) {
				for (int x = minX; x <= maxX; x++) {
					if (x == minX) {
						sOut += "<span class=\"";
						if (bCUFon == true) {
							sOut += "typeface-js ";
						}
						sOut += Internal.aCSS(0, Screen(x, y).BackColor) + " " + Internal.aCSS(1, Screen(x, y).ForeColor + Screen(x, y).Bold);
						sOut += "\">";
					}
					if (Screen(x, y).ForeColor + Screen(x, y).Bold != ForeColor | Screen(x, y).BackColor != BackColor) {
						sOut += "</span><span class=\"";
						if (bCUFon == true) {
							sOut += "typeface-js ";
						}
						sOut += Internal.aCSS(0, Screen(x, y).BackColor) + " " + Internal.aCSS(1, Screen(x, y).ForeColor + Screen(x, y).Bold) + "\">";
						ForeColor = Screen(x, y).ForeColor + Screen(x, y).Bold;
						BackColor = Screen(x, y).BackColor;
					}
					if (Screen(x, y).Chr != Chr(0)) {
						if (Screen(x, y).Chr != Chr(255)) {
							sOut += Screen(x, y).Chr;
						} else {
							sOut += "&nbsp;";
						}
					} else {
						x = maxX;
					}
					if (x == maxX) {
						sOut += "</span>";
					}
				}
				sOut += vbCrLf;
				System.Windows.Forms.Application.DoEvents();
			}
			sOut += "</span></pre></div>";
			if (bCUFon == true) {
				sOut += "<script language=\"Javascript\">\r\n" + " _typeface_js.initialize();\r\n" + "</script>\r\n";
			}
			if (bOutputSauce == true & bHasSauce == true) {
				sOut += oSauce.BuildHTML(true);
			}
			if (bHTMLComplete == true) {
				sOut += "</body></html>";
			}

			return WriteFile(sOutFile, sOut, bForceOverwrite, OutputFileExists, false, false);

		}

		public string[] OutputPCB(string sOutFile)
		{
			string sOut = "";

			XPos = minX;
			YPos = minY;
			ForeColor = 7;
			BackColor = 0;
			sOut = "@X" + Hex(BackColor) + Hex(ForeColor);

			for (int y = minY; y <= LinesUsed; y++) {
				for (int x = minX; x <= maxX; x++) {
					if (Screen(x, y).ForeColor + Screen(x, y).Bold != ForeColor | Screen(x, y).BackColor != BackColor) {
						sOut = sOut + "@X" + Hex(Screen(x, y).BackColor) + Hex(Screen(x, y).ForeColor + Screen(x, y).Bold);
						ForeColor = Screen(x, y).ForeColor + Screen(x, y).Bold;
						BackColor = Screen(x, y).BackColor;
					}
					if (Screen(x, y).Chr != Chr(0)) {
						if (Screen(x, y).Chr != Chr(255)) {
							sOut = sOut + Screen(x, y).Chr;
						} else {
							sOut = sOut;
						}
					} else {
						x = maxX;
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				sOut += vbCrLf;
				System.Windows.Forms.Application.DoEvents();
			}
			if (bOutputSauce == true & bHasSauce == true) {
				sOut += oSauce.toString();
			}
			return WriteFile(sOutFile, sOut, bForceOverwrite, OutputFileExists, false, false);

		}

		public string[] OutputAVT(string sOutFile)
		{
			string sOut = "";

			XPos = minX;
			YPos = minY;
			ForeColor = 7;
			BackColor = 0;

			for (int y = minY; y <= LinesUsed; y++) {
				for (int x = minX; x <= maxX; x++) {
					if (Screen(x, y).ForeColor + Screen(x, y).Bold != ForeColor | Screen(x, y).BackColor != BackColor) {
						ForeColor = Screen(x, y).ForeColor + Screen(x, y).Bold;
						BackColor = Screen(x, y).BackColor;
						sOut = sOut + Chr(22) + Chr(1) + Chr(BackColor * 16 + ForeColor);
					}
					if (Screen(x, y).Chr != Chr(0)) {
						if (Screen(x, y).Chr != Chr(255)) {
							sOut = sOut + Screen(x, y).Chr;
						} else {
							sOut = sOut;
						}
					} else {
						x = maxX;
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				sOut += vbCrLf;
				System.Windows.Forms.Application.DoEvents();
			}
			if (bOutputSauce == true & bHasSauce == true) {
				sOut += oSauce.toString();
			}
			return WriteFile(sOutFile, sOut, bForceOverwrite, OutputFileExists, false, false);

		}
		public string[] OutputWC2(string sOutFile)
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

			XPos = minX;
			YPos = minY;
			ForeColor = 7;
			BackColor = 0;
			Bold = 0;

			for (int y = minY; y <= LinesUsed; y++) {
				for (int x = minX; x <= maxX; x++) {
					if (Screen(x, y).ForeColor != ForeColor | Screen(x, y).BackColor != BackColor | Screen(x, y).Bold != Bold) {
						ForeColor = Screen(x, y).ForeColor;
						Bold = Screen(x, y).Bold;
						BackColor = Screen(x, y).BackColor;
						if (ForeColor + Bold == 7 & BackColor == 0) {
							sOut = sOut + Chr(27) + "[0;0;40m";
						} else {
							sOut = sOut + Chr(27) + "[" + Bold / 8 + ";" + aWCF(ForeColor) + ";" + aWCB(BackColor) + "m";
						}
					}
					if (Screen(x, y).Chr != Chr(0)) {
						if (Screen(x, y).Chr != Chr(255)) {
							sOut = sOut + Screen(x, y).Chr;
						} else {
							sOut = sOut;
						}
					} else {
						x = maxX;
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				sOut += vbCrLf;
				System.Windows.Forms.Application.DoEvents();
			}
			if (bOutputSauce == true & bHasSauce == true) {
				sOut += oSauce.toString();
			}
			return WriteFile(sOutFile, sOut, bForceOverwrite, OutputFileExists, false, false);

		}
		public string[] OutputWC3(string sOutFile)
		{
			string sOut = "";

			XPos = minX;
			YPos = minY;
			ForeColor = 7;
			BackColor = 0;
			sOut = "@" + Hex(BackColor) + Hex(ForeColor) + "@";

			for (int y = minY; y <= LinesUsed; y++) {
				for (int x = minX; x <= maxX; x++) {
					if (Screen(x, y).ForeColor + Screen(x, y).Bold != ForeColor | Screen(x, y).BackColor != BackColor) {
						ForeColor = Screen(x, y).ForeColor + Screen(x, y).Bold;
						BackColor = Screen(x, y).BackColor;
						sOut = sOut + "@" + Hex(BackColor) + Hex(ForeColor) + "@";
					}
					if (Screen(x, y).Chr != Chr(0)) {
						if (Screen(x, y).Chr != Chr(255)) {
							sOut = sOut + Screen(x, y).Chr;
						} else {
							sOut = sOut;
						}
					} else {
						x = maxX;
						break; // TODO: might not be correct. Was : Exit For
					}
				}
				sOut += vbCrLf;
				System.Windows.Forms.Application.DoEvents();
			}
			if (bOutputSauce == true & bHasSauce == true) {
				sOut += oSauce.toString();
			}
			return WriteFile(sOutFile, sOut, bForceOverwrite, OutputFileExists, false, false);

		}

		public string[] OutputANS(string sOutFile)
		{
			byte[] aOut = new byte[1000];
			int Cnt = 0;
			int iWhatChange = 0;
			int x;
			int y;
			bool bRest = false;
			XPos = minX;
			YPos = minY;
			ForeColor = 7;
			BackColor = 0;
			Bold = 0;
			Reversed = false;
			for (y = minY; y <= LinesUsed; y++) {
				for (x = minX; x <= maxX; x++) {
					if (Screen(x, y).Chr == "-1") {
						Screen(x, y).Chr = Chr(32);
					}
				}
			}

			for (y = minY; y <= LinesUsed; y++) {
				if (y == minY) {
					//<[255D
					aOut(Cnt) = 27;
					Cnt += 1;
					aOut(Cnt) = 91;
					Cnt += 1;
					aOut(Cnt) = 50;
					Cnt += 1;
					aOut(Cnt) = 50;
					Cnt += 1;
					aOut(Cnt) = 53;
					Cnt += 1;
					aOut(Cnt) = 68;
					Cnt += 1;
				}
				bRest = false;
				for (x = minX; x <= maxX; x++) {
					//0 = 48, 1 = 49, 59 = ; 109 = m
					iWhatChange = 0;
					if (x == minX & y > minY) {
						iWhatChange = 14;
					} else {
						if (Screen(x, y).ForeColor != ForeColor) {
							iWhatChange += 2;
						}
						if (Screen(x, y).Bold != Bold) {
							iWhatChange += 4;
						}
						if (Screen(x, y).BackColor != BackColor) {
							iWhatChange += 8;
						}
					}

					if (iWhatChange != 0) {
						ForeColor = Screen(x, y).ForeColor;
						Bold = Screen(x, y).Bold;
						BackColor = Screen(x, y).BackColor;
						//all changed
						if (iWhatChange > 9) {
							aOut(Cnt) = 27;
							Cnt += 1;
							aOut(Cnt) = 91;
							Cnt += 1;
							aOut(Cnt) = IIf(Bold != 0, 49, 48);
							Cnt += 1;
							aOut(Cnt) = 59;
							Cnt += 1;
							aOut(Cnt) = Asc(Left(Internal.AnsiForeColors(ForeColor), 1));
							Cnt += 1;
							aOut(Cnt) = Asc(Right(Internal.AnsiForeColors(ForeColor), 1));
							Cnt += 1;
							aOut(Cnt) = 59;
							Cnt += 1;
							aOut(Cnt) = Asc(Left(Internal.AnsiBackColors(BackColor), 1));
							Cnt += 1;
							aOut(Cnt) = Asc(Right(Internal.AnsiBackColors(BackColor), 1));
							Cnt += 1;
							aOut(Cnt) = 109;
							Cnt += 1;
						} else {
							//Back color only
							if (iWhatChange == 8) {
								aOut(Cnt) = 27;
								Cnt += 1;
								aOut(Cnt) = 91;
								Cnt += 1;
								aOut(Cnt) = Asc(Left(Internal.AnsiBackColors(BackColor), 1));
								Cnt += 1;
								aOut(Cnt) = Asc(Right(Internal.AnsiBackColors(BackColor), 1));
								Cnt += 1;
								aOut(Cnt) = 109;
								Cnt += 1;
							} else {
								aOut(Cnt) = 27;
								Cnt += 1;
								aOut(Cnt) = 91;
								Cnt += 1;
								aOut(Cnt) = IIf(Bold != 0, 49, 48);
								Cnt += 1;
								aOut(Cnt) = 59;
								Cnt += 1;
								aOut(Cnt) = Asc(Left(Internal.AnsiForeColors(ForeColor), 1));
								Cnt += 1;
								aOut(Cnt) = Asc(Right(Internal.AnsiForeColors(ForeColor), 1));
								Cnt += 1;
								aOut(Cnt) = 109;
								Cnt += 1;
							}
						}

					}
					if (Screen(x, y).Chr != Chr(0)) {
						if (Screen(x, y).Chr == Chr(32)) {
							int iSpc = NumSpaces(x, y);
							if (iSpc > maxX) {
								//<[XC move cursor right
								aOut(Cnt) = 27;
								Cnt += 1;
								aOut(Cnt) = 91;
								Cnt += 1;
								for (int iLoop2 = 1; iLoop2 <= iSpc.ToString.Length; iLoop2++) {
									aOut(Cnt) = Asc(Mid(iSpc.ToString, iLoop2, 1));
									Cnt += 1;
								}
								aOut(Cnt) = 67;
								Cnt += 1;
								x += iSpc - 1;
							} else {
								aOut(Cnt) = Asc(Screen(x, y).Chr);
								Cnt += 1;
							}
						} else {
							aOut(Cnt) = Asc(Screen(x, y).Chr);
							Cnt += 1;
						}
					} else {
						aOut(Cnt) = 32;
						Cnt += 1;
					}
					if (x >= maxX) {
						//<[s
						aOut(Cnt) = 27;
						Cnt += 1;
						aOut(Cnt) = 91;
						Cnt += 1;
						aOut(Cnt) = 115;
						Cnt += 1;
						bRest = true;
					}
					if (Cnt > UBound(aOut) - 20) {
						Array.Resize(ref aOut, UBound(aOut) + 1001);
					}
				}
				//linebreak
				aOut(Cnt) = 13;
				Cnt += 1;
				aOut(Cnt) = 10;
				Cnt += 1;
				if (bRest == true) {
					//<[u
					aOut(Cnt) = 27;
					Cnt += 1;
					aOut(Cnt) = 91;
					Cnt += 1;
					aOut(Cnt) = 117;
					Cnt += 1;

				}

				if (Cnt > UBound(aOut) - 20) {
					Array.Resize(ref aOut, UBound(aOut) + 1001);
				}
			}
			Array.Resize(ref aOut, Cnt + 1);
			if (bOutputSauce == true & bHasSauce == true) {
				aOut = ConverterSupport.MergeByteArrays(aOut, oSauce.toByteArray);
			}
			return WriteFile(sOutFile, aOut, bForceOverwrite, OutputFileExists, false, true);
			aOut = null;
		}
		public int NumSpaces(int xOffset, int yOffset)
		{
			int iNums = 1;
			bool bEnd = false;
			int iCnter = xOffset;
			while (bEnd == false) {
				if (maxX >= iCnter + 1) {
					iCnter += 1;
					if (Screen(iCnter, yOffset).Chr == Chr(32)) {
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
		public string[] OutputBin(string sOutFile)
		{
			XPos = minX;
			YPos = minY;
			ForeColor = 7;
			BackColor = 0;
			byte[] aOut = new byte[(LinesUsed * (maxX * 2)) - 1];
			int Cnt = 0;
			int intC = 0;
			bool bFiller = false;

			for (int y = minY; y <= LinesUsed; y++) {
				for (int x = minX; x <= maxX; x++) {
					if (Screen(x, y).Chr != Chr(0)) {
						if (Screen(x, y).Chr == Chr(255)) {
							bFiller = true;
						}
					} else {
						bFiller = true;
					}
					if (bFiller == true) {
						ForeColor = DefForeColor;
						BackColor = DefBackColor;
						intC = 0;
					} else {
						ForeColor = Screen(x, y).ForeColor + Screen(x, y).Bold;
						BackColor = Screen(x, y).BackColor;
						intC = Asc(Screen(x, y).Chr);
					}
					aOut(Cnt) = intC;
					Cnt += 1;
					aOut(Cnt) = System.Convert.ToInt32(Hex(BackColor) + Hex(ForeColor), 16);
					Cnt += 1;
				}
				bFiller = false;
				System.Windows.Forms.Application.DoEvents();
			}
			if (bOutputSauce == true & bHasSauce == true) {
				aOut = ConverterSupport.MergeByteArrays(aOut, oSauce.toByteArray);
			}
			return WriteFile(sOutFile, aOut, bForceOverwrite, OutputFileExists, false, true);
			aOut = null;

		}




	}
}