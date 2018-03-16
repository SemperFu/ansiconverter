class Module1
{
		//"1.04.00"
	public string ToolVersion = My.Application.Info.Version.ToString;
	public const  ToolVersionDate = "03.2012";
	public const bool bDebug = false;
	public bool bInputFile = true;

	public Converter.ProcessFiles oConv = new Converter.ProcessFiles();
	public System.Windows.Forms.Form F = new System.Windows.Forms.Form();
	public System.Windows.Forms.ToolTip TT = new System.Windows.Forms.ToolTip();
	public string sCodePg = "CP437";
	public bool bHTMLEncode = true;
	public bool bHTMLComplete = true;
	public bool bSanitize = false;
	public string pSauce = "Strip";
	public string pAnim = "Static";
	public string pUTF = "UTF16";
	public bool bCUFon = false;
	public int OutputFileExists = 0;
	public bool bConv2Unicode;
	public string sOutPutFormat = "";
	public bool bOutputSauce = false;
	public bool bAnimation = false;
	public string txtExt = "";
	public bool bOutPathInput = true;
	public string sHTMLFont = "Default";
	public bool bReplaceExt = false;
	public string soutPath = "";
	public bool bForceOverwrite = false;
	public AnsiCPMaps.AnsiCPMaps CPS = AnsiCPMaps.AnsiCPMaps.Instance;
	public bool bShowHelp = false;
	public bool bShowCPList = false;
	public string sExtFilter = "";
	public string[] aExtFilter;
	public string sInputPath = "";
	private int[] aFTCounts = new int[5];
	private int iNumProc = 0;
	private int iNumDone = 0;
	public bool bNoColors = false;
	public bool bSmallFont = false;
	private FTypes iOutFormat;
	private bool bThumb = false;
	private int iThumbResOpt = 0;
	private int iThumbProp = 0;
	private int iThumbWidth = 0;
	private int iThumbHeight = 0;
	private double pFPS = 30.0;
	private int pBPS = 28800;
	private int pExtend = 3;
	private string sCodec = "";
	private object[,] aCodecs = new object[, ] {
		{
			"AVI",
			new string[] {
				"ZMBV",
				"H264",
				"FFVI",
				"MPEG4",
				"MJPEG",
				"LIBX264",
				"LIBXVID"
			}
		},
		{
			"MPG",
			new string[] {
				"MPEG1",
				"MPEG2"
			}
		},
		{
			"WMV",
			null
		},
		{
			"MP4",
			null
		},
		{
			"FLV",
			null
		},
		{
			"GIF",
			null
		},
		{
			"MKV",
			null
		},
		{
			"VOB",
			null
		}
	};
	private string[] aImages = new string[] {
		"PNG",
		"GIF",
		"BMP",
		"JPG",
		"TIF",
		"ICO",
		"WMF",
		"EMF"
	};
	private string[] aHTML = new string[] {
		"HTM",
		"WEB"
	};
	private string[] aOutputs = new string[] {
		"ASC",
		"ANS",
		"HTML",
		"UTF8",
		"UTF16",
		"PCB",
		"WC2",
		"WC3",
		"AVT",
		"BIN",
		"IMG",
		"VID"
	};
	private object[] aExt = new object[] {
		null,
		null,
		aHTML,
		new string[] { "TXT" },
		new string[] { "TXT" },
		null,
		null,
		null,
		null,
		null,
		aImages,
		aCodecs

	};
	public enum FFormats
	{
		us_ascii = 0,
		utf_8 = 1,
		utf_16 = 2,
		utf_7 = 3,
		utf_32 = 4
	}
	public enum FTypes
	{
		ASCII = 0,
		ANSI = 1,
		HTML = 2,
		Unicode = 3,
		PCB = 4,
		Bin = 5,
		WC2 = 6,
		WC3 = 7,
		AVT = 8,
		IMG = 9,
		VID = 10
	}

	public void evHandlerInfoMsg(string Msg, bool nolinebreak, bool removelast)
	{
		Console.Out.WriteLine(StripMessageFormatting(Msg));
	}
	public void evHandlerErrMsg(string Msg)
	{
		Console.Error.WriteLine(StripMessageFormatting(Msg));
	}


	public void evHandlerAdjustnumTotal(int value)
	{
	}
	public void evHandlerProcessedFile(int idx)
	{
		iNumDone += 1;
		Console.WriteLine(iNumDone.ToString + " of " + iNumProc.ToString + " files processed.");
		Console.WriteLine("");
	}

	public string StripMessageFormatting(string sMsg)
	{
		string sResult = sMsg;
		sResult = Replace(sResult, "[b]", "", 1, -1, CompareMethod.Text);
		sResult = Replace(sResult, "[/b]", "", 1, -1, CompareMethod.Text);
		sResult = Replace(sResult, "[i]", "", 1, -1, CompareMethod.Text);
		sResult = Replace(sResult, "[/i]", "", 1, -1, CompareMethod.Text);
		sResult = Replace(sResult, "[u]", "", 1, -1, CompareMethod.Text);
		sResult = Replace(sResult, "[/u]", "", 1, -1, CompareMethod.Text);
		return sResult;
	}
	void Main()
	{
		oConv.ProcessedFile += evHandlerProcessedFile;
		oConv.InfoMsg += evHandlerInfoMsg;
		oConv.ErrMsg += evHandlerErrMsg;
		oConv.AdjustnumTotal += evHandlerAdjustnumTotal;
		Converter.MForm = F;
		Converter.ToolTip = TT;


		int iArgs = My.Application.CommandLineArgs.Count();
		int iVal = 0;
		double dVal = 0;
		string sVal = "";
		int a = 0;
		int index = 0;
		bool bNamedParam = false;
		string sNamedParam = "";
		string sPath = "";
		bool bError = false;
		if (iArgs > 0) {
			if (bDebug == true) {
				for (a = 0; a <= iArgs - 1; a++) {
					string sParam = My.Application.CommandLineArgs.Item(a);
					Console.WriteLine(Strings.Right("     " + a.ToString, 5) + "." + sParam);
				}
				Console.WriteLine("... End Params");
			}

			for (a = 0; a <= iArgs - 1; a++) {
				bNamedParam = false;
				string sParam = My.Application.CommandLineArgs.Item(a);
				if (Microsoft.VisualBasic.Left(sParam, 1) == "/" & InStr(sParam, ":", CompareMethod.Text) > 2) {
					sNamedParam = Strings.Left(sParam, InStr(sParam, ":", CompareMethod.Text));
					bNamedParam = true;
				}
				if (bNamedParam == true) {
					sVal = Microsoft.VisualBasic.Right(sParam, Len(sParam) - Len(sNamedParam));
					switch (LCase(sNamedParam)) {
						case "/ftyp:":
							sExtFilter = sVal;
							aExtFilter = Split(sVal, ",");
						case "/out:":
							sVal = Trim(UCase(sVal));
							switch (sVal) {
								case "ASC":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.ASCII;
								case "ANS":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.ANSI;
								case "HTML":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.HTML;
								case "UTF8":
									pUTF = "UTF8";
									sOutPutFormat = "UTF";
									iOutFormat = FTypes.Unicode;
								case "UTF16":
									pUTF = "UTF16";
									sOutPutFormat = "UTF";
									iOutFormat = FTypes.Unicode;
								case "PCB":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.PCB;
								case "WC2":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.WC2;
								case "WC3":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.WC3;
								case "AVT":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.AVT;
								case "VID":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.VID;
								case "BIN":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.Bin;
								case "IMG":
									sOutPutFormat = sVal;
									iOutFormat = FTypes.IMG;
								default:
									Console.Error.WriteLine("'" + sVal + "' is not a valid value for the /OUT: Format Option.");
									bError = true;

							}
						case "/font:":
							sHTMLFont = sVal;
						case "/nocol:":
							bNoColors = true;
						case "/small:":
							bSmallFont = true;
						case "/save:":
							sVal = Trim(sVal);
							try {
								if (IO.Directory.Exists(sVal)) {
									soutPath = sVal;
									bOutPathInput = false;
								} else {
									Console.Error.WriteLine("'" + sVal + "' is not a valid output path or does not exist.");
									bError = true;
								}
							} catch (Exception ex) {
								Console.Error.WriteLine("'" + sVal + "' is not a valid output path or does not exist.");
								bError = true;
							}
						case "/ext:":
							bReplaceExt = true;
							txtExt = sVal;
						case "/newext:":
							bReplaceExt = false;
							txtExt = sVal;
						case "/over:":
							sVal = Trim(UCase(sVal));
							switch (sVal) {
								case "OVER":
									OutputFileExists = 0;
								case "SKIP":
									OutputFileExists = 1;
								case "REN":
									OutputFileExists = 2;
								default:
									Console.Error.WriteLine("'" + sVal + "' is not a valid option for the /OVER: Option.");
									bError = true;
							}
						case "/thumbscale:":
							sVal = Trim(UCase(sVal));
							switch (sVal) {
								case "PROP":
									iThumbResOpt = 0;
								case "WIDTH":
									iThumbResOpt = 1;
								case "HEIGHT":
									iThumbResOpt = 2;
								case "CUSTOM":
									iThumbResOpt = 3;
							}
						case "/scale:":
							if (IsNumeric(sVal)) {
								iThumbProp = Math.Abs((int)sVal);

							}
						case "/width:":
							if (IsNumeric(sVal)) {
								iThumbWidth = Math.Abs((int)sVal);
							}
						case "/height:":
							if (IsNumeric(sVal)) {
								iThumbHeight = Math.Abs((int)sVal);
							}
						case "/cp:":
							if (CodePageExists(Trim(sVal))) {
								sCodePg = Trim(UCase(sVal));
							} else {
								Console.Error.WriteLine("'" + sVal + "' is not a valid code page value.");
								bError = true;
							}
						case "/codec:":
							sCodec = sVal;
						case "/fps:":
							if (IsNumeric(sVal)) {
								pFPS = Math.Abs((double)sVal);
							}
						case "/bps:":
							if (IsNumeric(sVal)) {
								pBPS = Math.Abs((int)sVal);
							}
						case "/extend:":
							if (IsNumeric(sVal)) {
								pExtend = Math.Abs((int)sVal);
							}
					}
				} else {
					switch (LCase(sParam)) {
						case "/?":
							bShowHelp = true;
						case "/h":
							bShowHelp = true;
						case "/help":
							bShowHelp = true;
						case "/cplist":
							bShowCPList = true;
						case "/sanitize":
							bSanitize = true;
						case "/anim":
							pAnim = "Anim";
							bAnimation = true;
						case "/sauce":
							pSauce = "Keep";
							bOutputSauce = true;
						case "/object":
							bHTMLComplete = false;
						case "/thumb":
							bThumb = true;
						default:
							sInputPath = sParam;
					}
				}
			}

			if (bThumb == true) {
				switch (iThumbResOpt) {
					case 0:
						if (iThumbProp == 0) {
							Console.Error.WriteLine("/SCALE parameter not provided for proportional thumbnail scaling.");
							bError = true;
						}
					case 1:
						if (iThumbWidth == 0) {
							Console.Error.WriteLine("/WIDTH parameter not provided for proportional fixed width thumbnail scaling.");
							bError = true;

						}
					case 2:
						if (iThumbHeight == 0) {
							Console.Error.WriteLine("/HEIGHT parameter not provided for proportional fixed height thumbnail scaling.");
							bError = true;


						}
					case 3:
						if (iThumbWidth == 0 | iThumbHeight == 0) {
							Console.Error.WriteLine("/WIDTH and/or /HEIGHT parameter(s) not provided for custom size thumbnail scaling.");
							bError = true;
						}
				}
			}
			if (bShowHelp == true) {
				HelpMessage();
				System.Environment.Exit(0);
			}
			if (bShowCPList == true) {
				BuildCPList();
				System.Environment.Exit(0);
			}
			if (sInputPath == "") {
				Console.Error.WriteLine("Input File/Folder Path are missing.");
				bError = true;
			} else {
				try {
					if (IO.Directory.Exists(sInputPath)) {
						bInputFile = false;
						BuildFileList(sInputPath);
						if (Converter.ListInputFiles.Count == 0) {
							Console.Error.WriteLine("Input Folder: '" + sInputPath + "' no files to process found.");
							bError = true;
						} else {
							Console.WriteLine("# Files to Process: " + Converter.ListInputFiles.Count + ", Break down by Type: ");
							string sTmp = "";
							for (int b = 0; b <= 5; b++) {
								if (aFTCounts(b) != 0) {
									switch (b) {
										case 0:
											sTmp += ", ASC: ";
										case 1:
											sTmp += ", ANS: ";
										case 2:
											sTmp += ", HTM: ";
										case 3:
											sTmp += ", UTF: ";
										case 4:
											sTmp += ", PCB: ";
										case 5:
											sTmp += ", BIN: ";
										case 6:
											sTmp += ", WC2: ";
										case 7:
											sTmp += ", WC3: ";
										case 8:

											sTmp += ", AVT: ";
									}
									sTmp += aFTCounts(b).ToString;
								}
							}
							if (sTmp != "") {
								sTmp = Strings.Right(sTmp, sTmp.Length - 2);
							}
							Console.WriteLine(sTmp);
							Console.WriteLine("");
						}
					} else {
						if (IO.File.Exists(sInputPath)) {
							bInputFile = true;
							try {
								AddFile(sInputPath);

							} catch (Exception ex) {
							}
							if (Converter.ListInputFiles.Count == 0) {
								Console.Error.WriteLine("Input File: '" + sInputPath + "' cannot be processed. Unknow/Unsupported File Type or not accessible.");
								bError = true;
							}
						} else {
							Console.Error.WriteLine("Input File/Folder Path: '" + sInputPath + "' does not exist.");
							bError = true;
						}
					}
				} catch (Exception ex) {
					Console.Error.WriteLine("Invalid Input File/Folder Path Value: '" + sInputPath + "'.");
					bError = true;
				}
			}
			if (sOutPutFormat == "") {
				Console.Error.WriteLine("Output Format was not specified.");
				bError = true;
			}
			if (bError == true) {
				Console.Error.WriteLine("Start with /h option for Help.");
				System.Environment.Exit(0);
			}
			if (txtExt == "") {
				switch (sOutPutFormat) {
					case "HTML":
						if (bHTMLComplete == false) {
							txtExt = "WEB";
						} else {
							txtExt = "HTM";
						}
					case "UTF":
						txtExt = "TXT";
					case "IMG":
						txtExt = "PNG";
					case "VID":
						txtExt = "AVI";
					default:
						txtExt = sOutPutFormat;
				}
			}
			string sStr = "";
			iNumProc = Converter.ListInputFiles.Count;
			Console.WriteLine("Output Format: " + sOutPutFormat);
			switch (sOutPutFormat) {
				case "HTML":
					sStr = "Sanitize: ";
					if (bSanitize) {
						sStr += "YES";
					} else {
						sStr += "NO";
					}
					sStr += ", Full HTML: ";
					if (bHTMLComplete) {
						sStr += "YES";
					} else {
						sStr += "NO";
					}
					sStr += ", Anim: ";
					if (bAnimation) {
						sStr += "YES";
					} else {
						sStr += "NO";
					}
					Console.WriteLine(sStr);
					Console.WriteLine("Font: " + sHTMLFont);
				case "UTF":
					sStr = "Unicode Format: " + pUTF;
					Console.WriteLine(sStr);
				case "IMG":
					sStr = "No Colors?: " + bNoColors.ToString;
					sStr += ", Small Font: " + bSmallFont.ToString + vbCrLf;
					Console.WriteLine(sStr);
					if (bThumb) {
						sStr = "Create Thumbnails.";
						switch (iThumbResOpt) {
							case 0:
								sStr += " Scale to: " + iThumbProp + "% of org image size.";
							case 1:
								sStr += " Fixed width: " + iThumbWidth + " pixels.";
							case 2:
								sStr += " Fixed Height: " + iThumbHeight + " pixels.";
							case 3:
								sStr += " Custom Size: " + iThumbWidth + "x" + iThumbHeight + " pixels.";
						}
						Console.WriteLine(sStr);
					}
				case "VID":
					sStr = "FPS: " + pFPS.ToString + ", BPS: " + pBPS.ToString;
					if (UCase(txtExt) == "AVI" | UCase(txtExt) == "MPG") {
						sStr += ", Codec: " + sCodec;
					}
					Console.WriteLine(sStr);
					sStr = "Extend Last Frame by " + pExtend + " seconds.";
					Console.WriteLine(sStr);
			}
			sStr = "Output Path: ";
			if (bOutPathInput) {
				sStr += " as input";
			} else {
				sStr += soutPath;
			}
			Console.WriteLine(sStr);
			sStr = "Extension: " + txtExt;
			if (bReplaceExt) {
				sStr += " (Replace)";
			} else {
				sStr += " (Add)";
			}
			Console.WriteLine(sStr);
			sStr = "Keep Sauce Meta: ";
			if (bOutputSauce) {
				sStr += "YES";
			} else {
				sStr += "NO";
			}
			Console.WriteLine("CodePage: " + sCodePg);

			Console.WriteLine(sStr);
			Console.WriteLine("----------------------------------------------------------");
			Converter.txtExt = txtExt;
			Converter.rOutPathInput = bOutPathInput;
			Converter.rReplaceExt = bReplaceExt;
			Converter.outPath = soutPath;
			Converter.bForceOverwrite = bForceOverwrite;
			Converter.sHTMLFont = sHTMLFont;
			Converter.pOutExist = OutputFileExists;
			Converter.bCreateThumbs = bThumb;
			Converter.iThumbsResOpt = iThumbResOpt;
			Converter.iThumbsScale = iThumbProp;
			Converter.iThumbsWidth = iThumbWidth;
			Converter.iThumbsHeight = iThumbHeight;
			Converter.sOutPutFormat = sOutPutFormat;
			Converter.pOut = sOutPutFormat;
			//sInputFormat = MainForm.pIn.Tag.ToString
			Converter.bOutputSauce = bOutputSauce;
			Converter.pAnim = pAnim;
			Converter.pSauce = pSauce;

			Converter.bAnimation = bAnimation;
			Converter.bRemoveCompleted = true;

			Converter.bSanitize = bSanitize;
			Converter.pSanitize = bSanitize;
			Converter.pCP = sCodePg;
			Converter.sCodePg = sCodePg;
			Converter.pNoColors = bNoColors;
			Converter.pSmallFont = bSmallFont;
			Converter.pHTMLEncode = bHTMLEncode;
			Converter.pHTMLComplete = bHTMLComplete;
			Converter.selUTF = pUTF;
			Converter.BPS = pBPS;
			Converter.FPS = pFPS;
			Converter.LastFrame = pExtend;
			if (sOutPutFormat == "VID") {
				switch (UCase(txtExt)) {
					case "AVI":
						Converter.VidCodec = sCodec;
					case "MPG":
						Converter.VidCodec = sCodec;
					default:
						Converter.VidCodec = "";
				}
			}
			oConv.ConvertAllFiles();
			Console.WriteLine("DONE PROCESSING!");
			Console.WriteLine("... bye See you next time. Roy/SAC.");
		} else {
			HelpMessage();
		}
	}


	private void HelpMessage()
	{
		Console.WriteLine("");
		Console.WriteLine("============================================================================");
		Console.WriteLine("ANSI/ASCII Converter CLI by Carsten Cumbrowski aka Roy/SAC (Freeware)");
		Console.WriteLine("Version: " + ToolVersion + ", From: " + ToolVersionDate);
		Console.WriteLine("============================================================================");
		Console.WriteLine("");
		Console.WriteLine("----------------------------------------------------------------------------");
		Console.WriteLine("ANSI/ASCII Converter CLI Usage:");
		Console.WriteLine("----------------------------------------------------------------------------");
		Console.WriteLine("ANSIASCIIConverterCLI \"[drive:][path][filename]\" /OUT:<FORMAT> [/FTYP:<EXT>,..]");
		Console.WriteLine("                   [/SAVE:<PATH>] [/EXT:<EXT>]|[/NEWEXT:<EXT>] [/OVER:<OPTION>]");
		Console.WriteLine("             [/CP<CP>] [/FONT:<FONTNAME>] [/SANITIZE] [/SAUCE] [OBJECT] [/ANIM]");
		Console.WriteLine("   [/THUMB] [/THUMBSCALE: <OPT>] [/SCALE: <NUM>] [/WIDTH:<NUM>] [/HEIGHT:<NUM>]");
		Console.WriteLine("                    [/CODEC: <ID>] [/FPS: <NUM>] [/BPS: <NUM>] [/EXTEND: <NUM>]");
		Console.WriteLine("");
		Console.WriteLine("   -or-");
		Console.WriteLine("");
		Console.WriteLine("ANSIASCIIConverterCLI [/H]|[/?]|[/HELP]");
		Console.WriteLine("");
		Console.WriteLine("   -or-");
		Console.WriteLine("");
		Console.WriteLine("ANSIASCIIConverterCLI [/CPLIST]");
		Console.WriteLine("");
		Console.WriteLine("----------------------------------------------------------------------------");
		Console.WriteLine("Required Parameters:");
		Console.WriteLine("----------------------------------------------------------------------------");
		Console.WriteLine("");
		Console.WriteLine("\"[drive:][path][filename]\"");
		Console.WriteLine("  Specifies a folder or file to process");
		Console.WriteLine("");
		Console.WriteLine("/OUT:<ASC|ANS|HTML|UTF8|UTF16|PCB|WC2|WC3|AVT|BIN|IMG|VID>");
		Console.WriteLine("   (Required) Output-format. ");
		Console.WriteLine("    Supported Values: ASC|ANS|HTML|UTF8|UTF16|PCB|WC2|WC3|AVT|BIN|IMG|VID");
		Console.WriteLine("");
		Console.WriteLine("   For Image outputs does the Extension determine the Image format.");
		Console.WriteLine("   Supported are: PNG, BMP, JPG, GIF, TIF, ICO, WMF, EMF");
		Console.WriteLine("");
		Console.WriteLine("   For Video outputs does the Extension determine the Video format.");
		Console.WriteLine("   Supported are: AVI, WMV, MP4, MKV, FLI, GIF (Animated), MPG, VOB");
		Console.WriteLine("");
		Console.WriteLine("----------------------------------------------------------------------------");
		Console.WriteLine("Optional Parameters:");
		Console.WriteLine("----------------------------------------------------------------------------");
		Console.WriteLine("");
		Console.WriteLine("/FTYP:<EXT>,...");
		Console.WriteLine("   (Optional) List of file extensions to include (Folder processing). ");
		Console.WriteLine("   Separated by comma, without the \".\"");
		Console.WriteLine("");
		Console.WriteLine("/SAVE:<PATH>");
		Console.WriteLine("   (Optional) New Output Path (default uses Input Folder for outputs)");
		Console.WriteLine("");
		Console.WriteLine("/EXT:<EXT>");
		Console.WriteLine("   (Optional) Output, replaces extension of source with <EXT>");
		Console.WriteLine("");
		Console.WriteLine(" -or-");
		Console.WriteLine("");
		Console.WriteLine("/NEWEXT:<EXT>");
		Console.WriteLine("   (Optional) Output, add <EXT> to source for output file name. (Default)");
		Console.WriteLine("");
		Console.WriteLine("/OVER:<OVER|SKIP|REN> Default: OVER");
		Console.WriteLine("   (Optional) Output, handleing of existing output files. ");
		Console.WriteLine("   Values: \"OVER\"=Overwrite, \"SKIP\"=Skip File or \"REN\"=Auto-Rename");
		Console.WriteLine("");
		Console.WriteLine("/CP:<CP> Default: CP437 (US)");
		Console.WriteLine("   (Optional) CodePage. Only if conversion involves any ASCII Format ");
		Console.WriteLine("   (ASC, ANS, PCB, BIN) and any Unicode Format (UTF, HTML) in any direction.");
		Console.WriteLine("   For list of available CodePage values, use /CPLIST paramter.");
		Console.WriteLine("");
		Console.WriteLine("/FONT:<FontName> Default: Default");
		Console.WriteLine("   (Optional) For HTML Output, Font Name for CSS");
		Console.WriteLine("");
		Console.WriteLine("/SANITIZE");
		Console.WriteLine("   (Optional) For HTML Output, Convert TABs to SPACES etc.");
		Console.WriteLine("");
		Console.WriteLine("/SAUCE");
		Console.WriteLine("   (Optional) Convert SAUCE Meta Tag to new Format (if supported)");
		Console.WriteLine("");
		Console.WriteLine("/OBJECT");
		Console.WriteLine("   (Optional) For HTML Output. Instead of FULL HTML Document, ");
		Console.WriteLine("   Export Main HTML Object ONLY (no CSS, no JS Func).");
		Console.WriteLine("");
		Console.WriteLine("/ANIM");
		Console.WriteLine("   (Optional) For HTML Output. Generate Dynamic Animation (CSS/JS) ");
		Console.WriteLine("   instead of Static Picture. Only use for ANSIAnimations and ");
		Console.WriteLine("   ANSIS that use the Blinking Formatting.");
		Console.WriteLine("");
		Console.WriteLine("/NOCOL");
		Console.WriteLine("   (Optional) For Image Output. Render output without colors (ASCII) ");
		Console.WriteLine("");
		Console.WriteLine("/SMALL");
		Console.WriteLine("   (Optional) For Image Output. Use Small 8x8 font (instead of 8x16) ");
		Console.WriteLine("");
		Console.WriteLine("/THUMB");
		Console.WriteLine("   (Optional) For Image Output. Create Thumbnail Image ");
		Console.WriteLine("");
		Console.WriteLine("/THUMBSCALE: <PROP>|<WIDTH>|<HEIGHT>|<CUSTOM>");
		Console.WriteLine("   (Optional) For Thumbnail Output. Scaling Option");
		Console.WriteLine("");
		Console.WriteLine("/SCALE: <PERCENT>");
		Console.WriteLine("   (Optional) For Thumbnail Scaleing <PROP>. Percent of Full Image Size");
		Console.WriteLine("");
		Console.WriteLine("/WIDTH: <NUM>");
		Console.WriteLine("   (Optional) For Thumbnail Scaleing <WIDTH>/<CUSTOM>. Thumbnail Width");
		Console.WriteLine("");
		Console.WriteLine("/HEIGHT: <NUM>");
		Console.WriteLine("   (Optional) For Thumbnail Scaleing <HEIGHT>/<CUSTOM>. Thumbnail Height");
		Console.WriteLine("");
		Console.WriteLine("/CODEC: <ID>");
		Console.WriteLine("    (Optional) For Video outputs in .AVI or .MPG Format. Following values");
		Console.WriteLine("    are(supported)");
		Console.WriteLine("");
		Console.WriteLine("    For .MPG: MPEG1 (Default) or MPEG2 ");
		Console.WriteLine("    For .AVI: ZMBV (Default), FFVI, H264, MPEG4, MJPEG, LIBX264 or LIBXVID");
		Console.WriteLine("");
		Console.WriteLine("/FPS: <NUM>");
		Console.WriteLine("    (Optional) For Video Outputs. The Video Frame Rate. Default = 30.00");
		Console.WriteLine("");
		Console.WriteLine("/BPS: <NUM>");
		Console.WriteLine("    (Optional) For Video Outputs. The Simulated Modem Speed in BPS ");
		Console.WriteLine("    (Bits per Second). Default = 28800");
		Console.WriteLine("");
		Console.WriteLine("/EXTEND: <NUM>");
		Console.WriteLine("    (Optional) For Video Outputs. Number of Seconds to extend the last Frame");
		Console.WriteLine("    of the Animation. Default = 3 (Seconds)");
		Console.WriteLine("");
		Console.WriteLine("----------------------------------------------------------------------------");
		Console.WriteLine("Extra Parameters:");
		Console.WriteLine("----------------------------------------------------------------------------");
		Console.WriteLine("");
		Console.WriteLine("/H , /? , /HELP ");
		Console.WriteLine("   Returns this help text");
		Console.WriteLine("");
		Console.WriteLine("/CPLIST");
		Console.WriteLine("   All other parameters will be ignored. ");
		Console.WriteLine("   Returns list of available CodePages to be used for the /CP Param.");
		Console.WriteLine("");
		Console.WriteLine("============================================================================");
		Console.WriteLine("");

	}

	private void BuildCPList()
	{
		string sStr = "";
		Console.WriteLine("");
		Console.WriteLine("List of Supported Code Pages");
		Console.WriteLine("--------+-----+------------+-----------------------------------");
		Console.WriteLine("     CP | OS  | ISO        | Desc");
		Console.WriteLine("--------+-----+------------+-----------------------------------");
		for (int a = 0; a <= CPS.CodePages.Count - 1; a++) {
			sStr = " " + Strings.Right("      " + CPS.CodePages.Item(a).Name, 6) + " | ";
			sStr += Strings.Right("   " + CPS.CodePages.Item(a).OS, 3) + " | ";
			sStr += Strings.Right("          " + CPS.CodePages.Item(a).ISO, 10) + " | ";
			sStr += CPS.CodePages.Item(a).Description;
			Console.WriteLine(sStr);
		}
		Console.WriteLine("--------+-----+------------+-----------------------------------");
		Console.WriteLine("");
	}

	private bool CodePageExists(string sstr)
	{
		bool bReturn = false;
		for (int a = 0; a <= CPS.CodePages.Count - 1; a++) {
			if (LCase(CPS.CodePages.Item(a).Name) == sstr) {
				bReturn = true;
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		return bReturn;
	}

	private void AddFile(string sFile)
	{
		bool bAdd = true;
		sFile = IO.Path.GetFullPath(sFile);
		if (sFile != "") {
			if (IO.File.Exists(sFile) == false) {
				bAdd = false;
			} else {
				if (Converter.ConverterSupport.GetFileSizeNum(sFile) == 0) {
					bAdd = false;
				}
			}
		} else {
			bAdd = false;
		}
		if (Converter.ListInputFiles.Count > 0 & bAdd == true) {
			for (int a = 0; a <= Converter.ListInputFiles.Count - 1; a++) {
				if (Converter.ListInputFiles.Item(a).FullPath.Equals(sFile)) {
					bAdd = false;
				}
			}
		}
		if (bAdd == true) {
			FFormats ff = Converter.ConverterSupport.checkFileFormat(sFile);
			FTypes ft;
			if (ff == FFormats.us_ascii) {
				ft = Converter.ConverterSupport.DetermineFileType(sFile);
			} else {
				ft = FTypes.Unicode;
			}
			if (ft != iOutFormat) {
				aFTCounts(ft) += 1;
				Converter.ListInputFiles.Add(new Converter.FileListItem(IO.Path.GetFileName(sFile), sFile, ff, ft));
			}
		}
	}

	private void BuildFileList(string sFold)
	{
		IO.FileInfo sFile;
		IO.DirectoryInfo DI = new IO.DirectoryInfo(sFold);
		IO.FileInfo[] sFiles = DI.GetFiles();
		bool bAdd = true;
		if (sFiles.Length > 0) {
			foreach ( sFile in sFiles) {
				bAdd = true;
				if (sExtFilter != "") {
					if (InclExt(sFile.Extension) == false) {
						bAdd = false;
					}
				}
				if (bAdd == true) {
					try {
						AddFile(sFile.FullName.ToString);
					} catch (Exception ex) {
					}
				}
			}
		}
	}

	private bool InclExt(string sStr)
	{
		bool bResult = false;
		if (Strings.Left(sStr, 1) == ".") {
			if (sStr.Length > 1) {
				sStr = Strings.Right(sStr, sStr.Length - 1);
			} else {
				sStr = "";
			}
		}
		for (int a = 0; a <= aExtFilter.Length - 1; a++) {
			if (LCase(sStr) == LCase(aExtFilter(a))) {
				bResult = true;
				break; // TODO: might not be correct. Was : Exit For
			}
		}
		return bResult;
	}
}