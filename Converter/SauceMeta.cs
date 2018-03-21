using Converter.Properties;
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

namespace ConverterSupport
{


	public class SauceMeta
	{
		[VBFixedString(5)]
			//5 Bytes 'SAUCE'                   0
		public string ID;
			//2 Bytes 00 00                               5
		public byte[] Version = new byte[1];
		[VBFixedString(35)]
			//35 Bytes                   7
		public string Title;
		[VBFixedString(20)]
			//20 Bytes                 42
		public string Author;
		[VBFixedString(20)]
			//20 Bytes                 62
		public string Group;
		[VBFixedString(8)]
			//8 Bytes CCYYMMDD         82
		public string CreatedDate;
			//4 Bytes (Long) (-2147483648 to -2147483647) 90
		public int FileSize;
			//1 Byte                                         94 
		public byte DataType;
			//1 Byte                                         95
		public byte FileType;
			//2 Bytes (Word) (0 to 65535)                    96
		public Int16 TInfo1;
			//2 Bytes (Word)                                98
		public Int16 TInfo2;
			//2 Bytes (Word)                               100
		public Int16 TInfo3;
			//2 Bytes (Word)                               102 
		public Int16 TInfo4;
			//1 Byte                                        104
		public byte Comments;
			//1 Byte                                        105
		public byte Flags;
			//22 Bytes   all 00                        106 
		public byte[] Filler = new byte[21];
		//--- Total 128 bytes
		//Comments Optional started with 'COMNT' (5 bytes), followed by (Comments (=Num Comments) * 64 bytes) with the actual comments itself
			//Note, Dimension 0 isn't used, only "1" to "Comments", length fixed = 64 bytes
		public string[] aComments;

		private int iError = 0;
		public event SauceParseErrorEventHandler SauceParseError;
	public delegate void SauceParseErrorEventHandler(object sender, EventArgs e);

		public int ErrorStatus {
			get { return iError; }
		}
		public string DataTypeName {
			get {
				if (DataType >= 0 & DataType <= 8) {
					return aDT[DataType];
				} else {
					return "n/a";
				}
			}
		}
		public string FileTypeName {
			get {
				if (DataType >= 0 & DataType <= 8) {
					if (FileType >= 0 & FileType <= UBound((Array)aFT[DataType])) {
						return ((string[])aFT[DataType])[FileType];
					} else {
						return "n/a";
					}
				} else {
					return "n/a";
				}
			}
		}


		private string[] aDT = new string[] {
			"None",
			"Character (ANSI, ASCII, RIP)",
			"Graphics",
			"Vector",
			"Sound",
			"Binary Text",
			"XBIN",
			"Archive",
			"Executable"
		};
		private static object[] aFT = new object[] {
			new string[] { "" },
			new string[] {
				"ASCII",
				"ANSI",
				"ANSIMation",
				"RIP",
				"PcBoard",
				"AVATAR",
				"HTML",
				"SOURCE"
			},
			new string[] {
				"Gif",
				"PCX",
				"LBM/IFF",
				"TGA",
				"FLI",
				"FLC",
				"BMP",
				"GL",
				"DL",
				"WPG",
				"PNG ",
				"JPG",
				"MPG",
				"AVI"
			},
			new string[] {
				"DXF",
				"DWG",
				"WPG",
				"3DS"
			},
			new string[] {
				"MOD (4, 6 or 8 channel MOD/NST file) ",
				"669 (Renaissance 8 channel 669 format) ",
				"STM (Future Crew 4 channel ScreamTracker format) ",
				"S3M (Future Crew variable channel ScreamTracker3 format) ",
				"MTM (Renaissance variable channel MultiTracker Module) ",
				"FAR (Farandole composer module) ",
				"ULT (UltraTracker module) ",
				"AMF (DMP/DSMI Advanced Module Format) ",
				"DMF (Delusion Digital Music Format (XTracker)) ",
				"OKT (Oktalyser module) ",
				"ROL (AdLib ROL file (FM)) ",
				"CMF (Creative Labs FM) ",
				"MIDI (MIDI file) ",
				"SADT (SAdT composer FM Module) ",
				"VOC (Creative Labs Sample) ",
				"WAV (Windows Wave file) ",
				"SMP8 (8 Bit Sample, TInfo1 holds sampling rate) ",
				"SMP8S (8 Bit sample stereo, TInfo1 holds sampling rate) ",
				"SMP16 (16 Bit sample, TInfo1 holds sampling rate) ",
				"SMP16S (16 Bit sample stereo, TInfo1 holds sampling rate) ",
				"PATCH8 (8 Bit patch-file) ",
				"PATCH16(16 Bit Patch-file) ",
				"XM (FastTracker ][ Module) ",
				"HSC (HSC Module) ",
				"IT (Impulse Tracker) "
			},
			new string[] { "" },
			new string[] { "" },
			new string[] {
				"ZIP (PKWare)",
				"ARJ (Robert K. Jung)",
				"LZH (Haruyasu Yoshizaki (Yoshi))",
				"ARC (SEA)",
				"TAR (Unix TAR format)",
				"ZOO",
				"RAR",
				"UC2",
				"PAK",
				"SQZ"
			},
			new string[] { "" }
		};
		private int inArrPos = 0;
		//DateTypes
		//0 None
		//  FileTypes
		//  -
		//1 Character (ANSI, ASCII, RIP)
		//  FileTypes
		//  0 ASCII
		//    TInfo1 = Width
		//    TInfo2 = Num Lines
		//  1 ANSI
		//    TInfo1 = Width
		//    TInfo2 = Num Lines
		//  2 ANSIMation
		//    TInfo1 = Width
		//    TInfo2 = Num Lines
		//  3 RIP
		//    TInfo1 = Width
		//    TInfo2 = Height
		//    TInfo3 = Num Colors
		//  4 PcBoard
		//    TInfo1 = Width
		//    TInfo2 = Num Lines
		//  5 AVATAR
		//    TInfo1 = Width
		//    TInfo2 = Num Lines
		//  6 HTML
		//  7 SOURCE
		//
		//  Flag
		//    ICe Color, No Blinking and 16 back and fore colors
		//    Bit  7 6 5 4 3 2 1 0
		//    Set  0 0 0 0 0 0 0 X
		//
		//2 Graphics
		//  TInfo1 = Width
		//  TInfo2 = Height
		//  TInfo3 = Bits Per Pixel
		//
		//  FileTypes
		//  0 Gif
		//  1 PCX
		//  2 LBM/IFF
		//  3 TGA
		//  4 FLI
		//  5 FLC
		//  6 BMP
		//  7 GL
		//  8 DL
		//  9 WPG
		// 10 PNG 
		// 11 JPG
		// 12 MPG
		// 13 AVI
		//
		//3 Vector
		//
		//  FileTypes
		//  0 DXF
		//  1 DWG
		//  2 WPG
		//  3 3DS
		//
		//4 Sound
		//  FileTypes
		//   0) MOD (4, 6 or 8 channel MOD/NST file) 
		//   1) 669 (Renaissance 8 channel 669 format) 
		//   2) STM (Future Crew 4 channel ScreamTracker format) 
		//   3) S3M (Future Crew variable channel ScreamTracker3 format) 
		//   4) MTM (Renaissance variable channel MultiTracker Module) 
		//   5) FAR (Farandole composer module) 
		//   6) ULT (UltraTracker module) 
		//   7) AMF (DMP/DSMI Advanced Module Format) 
		//   8) DMF (Delusion Digital Music Format (XTracker)) 
		//   9) OKT (Oktalyser module) 
		//   10) ROL (AdLib ROL file (FM)) 
		//   11) CMF (Creative Labs FM) 
		//   12) MIDI (MIDI file) 
		//   13) SADT (SAdT composer FM Module) 
		//   14) VOC (Creative Labs Sample) 
		//   15) WAV (Windows Wave file) 
		//   16) SMP8 (8 Bit Sample, TInfo1 holds sampling rate) 
		//   17) SMP8S (8 Bit sample stereo, TInfo1 holds sampling rate) 
		//   18) SMP16 (16 Bit sample, TInfo1 holds sampling rate) 
		//   19) SMP16S (16 Bit sample stereo, TInfo1 holds sampling rate) 
		//   20) PATCH8 (8 Bit patch-file) 
		//   21) PATCH16(16 Bit Patch-file) 
		//   22) XM (FastTracker ][ Module) 
		//   23) HSC (HSC Module) 
		//   24) IT (Impulse Tracker) 
		//   
		//
		//5 Binary Text
		//  Flag
		//    ICe Color, No Blinking and 16 back and fore colors
		//    Bit  7 6 5 4 3 2 1 0
		//    Set  0 0 0 0 0 0 0 X    
		//
		//6 XBIN
		//  TInfo1  = Width
		//  TInfo2 = Height
		//
		//7 Archive
		//  FileTypes
		//  0) ZIP (PKWare) 
		//  1) ARJ (Robert K. Jung) 
		//  2) LZH (Haruyasu Yoshizaki (Yoshi)) 
		//  3) ARC (SEA) 
		//  4) TAR (Unix TAR format) 
		//  5) ZOO 
		//  6) RAR 
		//  7) UC2 
		//  8) PAK 
		//  9) SQZ 
		//
		//
		//8 Executable

		public SauceMeta()
		{
			ID = "SAUCE";
			Title = "";
			Author = "";
			Group = "";
			CreatedDate = "19000101";
			FileSize = 0;
			DataType = 0;
			FileType = 0;
			TInfo1 = 0;
			TInfo2 = 0;
			TInfo3 = 0;
			TInfo4 = 0;
			Comments = 0;
			Flags = 0;
			iError = 0;
			inArrPos = 0;
			//Filler = Replace(Space(22), " ", "00", 1, -1, 1)
			 // ERROR: Not supported in C#: ReDimStatement

		}

		public void SetComments(int iNum)
		{
			int xxx;
			 // ERROR: Not supported in C#: ReDimStatement

			for (xxx = 0; xxx <= iNum; xxx++) {
				aComments[xxx] = "";
			}
			Comments = System.Convert.ToByte(iNum);
			System.Windows.Forms.Application.DoEvents();
		}
		public void AddComment(string sComment)
		{
			Comments += 1;
			Array.Resize(ref aComments, Comments + 1);
			if (sComment.Length > 64) {
				aComments[Comments] = Strings.Left(sComment, 64);
			} else {
				aComments[Comments] = sComment;
			}
		}
		public override string ToString()
		{
			string sRes = "";
			sRes += Chr(26) + this.ID;
			sRes += this.ByteArrayToString(this.Version, Chr(48).ToString());
			sRes += Strings.Left(this.Title + Space(35), 35);
			sRes += Strings.Left(this.Author + Space(20), 20);
			sRes += Strings.Left(this.Group + Space(20), 20);
			sRes += Strings.Left(this.CreatedDate + Space(8), 8);
			sRes += this.ByteArrayToString(this.HexStringToByteArray(this.FlipHexVal(Strings.Right("0000" + Hex(this.FileSize), 4))));
			sRes += Chr(this.DataType);
			sRes += Chr(this.FileType);
			sRes += this.ByteArrayToString(this.HexStringToByteArray(this.FlipHexVal(Strings.Right("00" + Hex(this.TInfo1), 2))));
			sRes += this.ByteArrayToString(this.HexStringToByteArray(this.FlipHexVal(Strings.Right("00" + Hex(this.TInfo2), 2))));
			sRes += this.ByteArrayToString(this.HexStringToByteArray(this.FlipHexVal(Strings.Right("00" + Hex(this.TInfo3), 2))));
			sRes += this.ByteArrayToString(this.HexStringToByteArray(this.FlipHexVal(Strings.Right("00" + Hex(this.TInfo4), 2))));
			sRes += Chr(this.Comments);
			sRes += Chr(this.Flags);
			sRes += this.ByteArrayToString(this.Filler);
			if (this.Comments > 0) {
				sRes += "COMNT";
				for (int x = 0; x <= UBound(this.aComments); x++) {
					sRes += Strings.Left(this.aComments[x] + Space(64), 64);
				}
			}
			System.Windows.Forms.Application.DoEvents();
			return sRes;

		}
		public byte[] AppendToByteArray(byte[] bte)
		{
			byte[] bSauce = this.toByteArray();
			return this.MergeByteArrays(bte, bSauce);
		}

		public byte[] toByteArray()
		{
			byte[] bte = new byte[] { 0 };
			if (this.Comments > 0) {
				 // ERROR: Not supported in C#: ReDimStatement

			} else {
				 // ERROR: Not supported in C#: ReDimStatement

			}
			bte = new byte[] { 26 };
			bte = this.MergeByteArrays(bte, this.StringToBytearray(this.ID));
			bte = this.MergeByteArrays(bte, this.Version);
			bte = this.MergeByteArrays(bte, this.StringToBytearray(this.Title, 35));
			bte = this.MergeByteArrays(bte, this.StringToBytearray(this.Author, 20));
			bte = this.MergeByteArrays(bte, this.StringToBytearray(this.Group, 20));
			bte = this.MergeByteArrays(bte, this.StringToBytearray(this.CreatedDate, 8));
			bte = this.MergeByteArrays(bte, this.HexStringToByteArray(this.FlipHexVal(Strings.Right("00000000" + Hex(this.FileSize), 8))));
			bte = this.MergeByteArrays(bte, new byte[] { this.DataType });
			bte = this.MergeByteArrays(bte, new byte[] { this.FileType });
			bte = this.MergeByteArrays(bte, this.HexStringToByteArray(this.FlipHexVal(Strings.Right("0000" + Hex(this.TInfo1), 4))));
			bte = this.MergeByteArrays(bte, this.HexStringToByteArray(this.FlipHexVal(Strings.Right("0000" + Hex(this.TInfo2), 4))));
			bte = this.MergeByteArrays(bte, this.HexStringToByteArray(this.FlipHexVal(Strings.Right("0000" + Hex(this.TInfo3), 4))));
			bte = this.MergeByteArrays(bte, this.HexStringToByteArray(this.FlipHexVal(Strings.Right("0000" + Hex(this.TInfo4), 4))));
			bte = this.MergeByteArrays(bte, new byte[] { this.Comments });
			bte = this.MergeByteArrays(bte, new byte[] { this.Flags });
			bte = this.MergeByteArrays(bte, this.Filler);
			if (this.Comments > 0) {
				bte = this.MergeByteArrays(bte, this.StringToBytearray("COMNT"));
				for (int x = 0; x <= UBound(this.aComments); x++) {
					bte = this.MergeByteArrays(bte, this.StringToBytearray(this.aComments[x], 64));
				}
			}
			System.Windows.Forms.Application.DoEvents();
			return bte;
		}

		public int GetFromByteArray(byte[] aArray, int Offset)
		{
			string sStr = "";
			inArrPos = Offset;
			iError = 0;
			if (UBound(aArray) >= Offset + 128 - 1) {

				try {
					this.ID = this.ReadByteArray(ref aArray, 5, "s", ref Offset);
					this.Version[0] = System.Convert.ToByte(ReadByteArray(ref aArray, 1, "b", ref Offset));
					this.Version[1] = System.Convert.ToByte(ReadByteArray(ref aArray, 1, "b", ref Offset));
					this.Title = this.ReadByteArray(ref aArray, 35, "s", ref Offset);
					this.Author = this.ReadByteArray(ref aArray, 20, "s", ref Offset);
					this.Group = this.ReadByteArray(ref aArray, 20, "s", ref Offset);
					this.CreatedDate = this.ReadByteArray(ref aArray, 8, "s", ref Offset);
					this.FileSize = System.Convert.ToInt32(this.Hex2Int64(this.FlipHexVal(this.ReadByteArray(ref aArray, 4, "h", ref Offset))));
					this.DataType = System.Convert.ToByte(ReadByteArray(ref aArray, 1, "b", ref Offset));
					this.FileType = System.Convert.ToByte(ReadByteArray(ref aArray, 1, "b", ref Offset));
					this.TInfo1 = System.Convert.ToInt16(Hex2Int64(this.FlipHexVal(this.ReadByteArray(ref aArray, 2, "h", ref Offset))));
					this.TInfo2 = System.Convert.ToInt16(Hex2Int64(this.FlipHexVal(this.ReadByteArray(ref aArray, 2, "h", ref Offset))));
					this.TInfo3 = System.Convert.ToInt16(Hex2Int64(this.FlipHexVal(this.ReadByteArray(ref aArray, 2, "h", ref Offset))));
					this.TInfo4 = System.Convert.ToInt16(Hex2Int64(this.FlipHexVal(this.ReadByteArray(ref aArray, 2, "h", ref Offset))));
					this.Comments = System.Convert.ToByte(ReadByteArray(ref aArray, 1, "b", ref Offset));
					this.Flags = System.Convert.ToByte(this.ReadByteArray(ref aArray, 1, "b", ref Offset));
					for (int x = 0; x <= 21; x++) {
						this.Filler[x] = System.Convert.ToByte(ReadByteArray(ref aArray, 1, "b", ref Offset));
					}
					if (UBound(aArray) >= Offset + (this.Comments * 64) + 5) {
						if (this.Comments != 0) {
							sStr = this.ReadByteArray(ref aArray, 5, "s", ref Offset);
							if (sStr == "COMNT") {
								this.SetComments((int)this.Comments);
								for (int iLoop2 = 1; iLoop2 <= this.Comments; iLoop2++) {
									this.aComments[iLoop2] = this.ReadByteArray(ref aArray, 64, "s", ref Offset);
								}
							}
						}
					}

				} catch (Exception ex) {
					iError = 1;
					if (SauceParseError != null) {
						SauceParseError(this, EventArgs.Empty);
					}
					Offset = inArrPos;
				}
			}
			System.Windows.Forms.Application.DoEvents();
			return Offset;
		}
		public void AddToFile(string sFile)
		{
			if (File.Exists(sFile)) {
				FileStream ofile;
				int iSize = 0;
				byte[] bte = toByteArray();
				ofile = File.Open(sFile, FileMode.Open, FileAccess.ReadWrite);
				ofile.Seek(0, SeekOrigin.Begin);
				iSize = System.Convert.ToInt32(ofile.Length);
				ofile.Seek(0, SeekOrigin.End);
				object iNewSize = iSize + bte.Length;
				ofile.SetLength((long)iNewSize);
				ofile.Write(bte, 0, bte.Length);
				ofile.Close();
			}
		}
		public bool RemoveFromFile(string sFile)
		{
			bool bRemoved = false;
			if (File.Exists(sFile)) {
				FileStream ofile;
				int iSize = 0;
				byte[] bte =null;
				ofile = File.Open(sFile, FileMode.Open, FileAccess.ReadWrite);
				ofile.Seek(0, SeekOrigin.Begin);
				iSize = System.Convert.ToInt32(ofile.Length);
				int iNumRead = 0;
				int iReadOFf = 0;
				if (iSize > 511) {
					iReadOFf = iSize - 512;
					iNumRead = 512;
				} else {
					iNumRead = iSize;
				}
				 // ERROR: Not supported in C#: ReDimStatement

				ofile.Seek(iReadOFf, SeekOrigin.Begin);
				ofile.Read(bte, 0, iNumRead);
				int xLoop = 0;
				while (xLoop <= UBound(bte)) {
					int CurChr = (int)bte[xLoop];
					//SUB or "S"
					if (CurChr == 26 | CurChr == 83) {
						int iSauceOffset = (int)IIf(CurChr == 83, 1, 0);
						if (UBound(bte) >= iLoop + 128 - iSauceOffset) {
							string sStr = "";
							for (int iLoop2 = 1 - iSauceOffset; iLoop2 <= 5 - iSauceOffset; iLoop2++) {
								sStr += Chr(bte[xLoop + iLoop2]);
							}
							if (sStr == "SAUCE") {
								xLoop += 1 - iSauceOffset;
								//
								int iNewxLoop = this.GetFromByteArray(bte, xLoop);
								ofile.SetLength(iSize - iNumRead + iNewxLoop);
								bRemoved = true;
								if (bDebug == true)
									Console.WriteLine("Sauce Meta found");
							}
						}
					}
					xLoop += 1;
				}
				ofile.Close();

			}
			return bRemoved;
		}
		public bool GetFromFile(string sFile)
		{
			bool bFound = false;
			iError = 0;
			if (File.Exists(sFile)) {
				byte[] bte =null;
				FileStream ofile;
				int iSize = 0;

				try {
					ofile = File.Open(sFile, FileMode.Open, FileAccess.Read);
					iSize = System.Convert.ToInt32(ofile.Length);
					int iNumRead = 0;
					int iReadOFf = 0;
					if (iSize > 511) {
						iReadOFf = iSize - 512;
						iNumRead = 512;
					} else {
						iNumRead = iSize;
					}
					 // ERROR: Not supported in C#: ReDimStatement

					ofile.Seek(iReadOFf, SeekOrigin.Begin);
					ofile.Read(bte, 0, iNumRead);
					ofile.Close();
					int xLoop = 0;
					while (xLoop <= UBound(bte)) {
						int CurChr = (int)bte[xLoop];
						//SUB or "S"
						if (CurChr == 26 | CurChr == 83) {
							int iSauceOffset = (int)IIf(CurChr == 83, 1, 0);
							if (UBound(bte) >= iLoop + 128 - iSauceOffset) {
								string sStr = "";
								for (int iLoop2 = 1 - iSauceOffset; iLoop2 <= 5 - iSauceOffset; iLoop2++) {
									sStr += Chr(bte[xLoop + iLoop2]);
								}
								if (sStr == "SAUCE") {
									xLoop += 1 - iSauceOffset;
									xLoop = this.GetFromByteArray(bte, xLoop);
									bFound = true;
									if (bDebug == true)
										Console.WriteLine("Sauce Meta found");
								}
							}
						}
						xLoop += 1;
					}
				} catch (Exception ex) {
					iError = 1;
					if (SauceParseError != null) {
						SauceParseError(this, EventArgs.Empty);
					}

				}

			}
			System.Windows.Forms.Application.DoEvents();
			return bFound;
		}

		public string BuildHTML(bool bGenCmts = true)
		{
			string sOutput = "";
			int cDay;
			int cMonth;
			int cYear;

			sOutput = "<div class=\"sauce\">";
			if (Strings.Trim(this.Title) != "") {
				sOutput += "<div class=\"saucetitle\"><span class=\"saucelabel\">Title:</span><span class=\"saucedata\">" + Strings.Trim(this.Title) + "</span></div>";
			}
			if (Strings.Trim(this.Author) != "") {
				sOutput += "<div class=\"sauceauthor\"><span class=\"saucelabel\">Author:</span><span class=\"saucedata\">" + Strings.Trim(this.Author) + "</span></div>";
			}
			if (Strings.Trim(this.Group) != "") {
				sOutput += "<div class=\"saucegroup\"><span class=\"saucelabel\">Group:</span><span class=\"saucedata\">" + Strings.Trim(this.Group) + "</span></div>";
			}
			if (this.DataType != 0) {
				sOutput += "<div class=\"saucedatatype\"><span class=\"saucelabel\">Data Type:</span><span class=\"saucedata\">" + Strings.Trim(this.DataTypeName) + " (" + this.DataType + ")</span></div>";
			}
			if (this.FileType != 0) {
				sOutput += "<div class=\"saucefiletype\"><span class=\"saucelabel\">File Type:</span><span class=\"saucedata\">" + Strings.Trim(this.FileTypeName) + " (" + this.FileType + ")</span></div>";
				//me.FileTypeName
			}
			if (Strings.Trim(this.CreatedDate) != "" & IsNumeric(this.CreatedDate)) {
				cYear = System.Convert.ToInt32(Left(CreatedDate, 4));
				cMonth = System.Convert.ToInt32(Mid(CreatedDate, 5, 2));
				cDay = System.Convert.ToInt32(Right(CreatedDate, 2));
				sOutput += "<div class=\"saucecreatedate\"><span class=\"saucelabel\">Creation Date:</span><span class=\"saucedata\">" + cDay.ToString()+ "." + DateAndTime.MonthName(cMonth) + " " + cYear.ToString()+ "</span></div>";
			}
			if ((this.DataType == 1 & this.FileType >= 0 & this.FileType <= 5) | this.DataType == 2 | this.DataType == 6) {
				//width
				if (this.TInfo1 != 0) {
					sOutput += "<div class=\"saucetinfo1\"><span class=\"saucelabel\">Width:</span><span class=\"saucedata\">" + this.TInfo1.ToString()+ "</span></div>";
				}
				//height
				if (this.TInfo2 != 0) {
					sOutput += "<div class=\"saucetinfo2\"><span class=\"saucelabel\">Height:</span><span class=\"saucedata\">" + this.TInfo2.ToString()+ "</span></div>";
				}
			}
			if (this.DataType == 1 & this.FileType == 3 & this.TInfo3 != 0) {
				//# RIP colors
				sOutput += "<div class=\"saucetinfo3\"><span class=\"saucelabel\"># Colors:</span><span class=\"saucedata\">" + this.TInfo3.ToString()+ "</span></div>";
			}
			// Ice Colors
			if (this.DataType == 1 & this.ExamineBit(this.Flags, 0) == true) {
				sOutput += "<div class=\"sauceice\">ICE Colors Enabled</div>";
			}
			if (this.DataType == 2 & this.TInfo3 != 0) {
				//Bits per Pixel
				sOutput += "<div class=\"saucetinfo3\"><span class=\"saucelabel\">Bits per Pixel:</span><span class=\"saucedata\">" + this.TInfo3.ToString() + "</span></div>";
			}

			sOutput += "</div>";

			if (this.Comments > 0 & bGenCmts == true) {
				sOutput += "<div class=\"saucecomments\"><ol>";
				for (int x = 0; x <= UBound(this.aComments); x++) {
					sOutput += "<li>" + Strings.Trim(this.aComments[x]) + "</li>";
				}
				sOutput += "</ol></div>";
			}
			System.Windows.Forms.Application.DoEvents();
			return sOutput;
		}

		private string ReadByteArray(ref byte[] Arr, int iLen, string DtaType, ref int iPos)
		{
			int a;
			string sRes = "";
			if (DtaType == "b") {
				iLen = 1;
			}
			for (a = 1; a <= iLen; a++) {
				if (UBound(Arr) >= inArrPos) {
					switch (DtaType) {
						case "s":
							sRes = sRes + Chr(Arr[iPos]);
							break;
						case "h":
							sRes = sRes + Strings.Right("0" + Hex(Arr[iPos]), 2);
							break;
						case "b":
							sRes = Arr[iPos].ToString();
							break;
					}
				}
				iPos += 1;
			}
			System.Windows.Forms.Application.DoEvents();
			return sRes;
		}
		// The ExamineBit function will return True or False 
		// depending on the value of the 1 based, nth bit (MyBit) 
		// of an integer (MyByte).
		private bool ExamineBit(byte MyByte, byte MyBit)
		{
			Int16 BitMask;
			MyByte = System.Convert.ToByte(MyByte & 0xff);
			BitMask = System.Convert.ToInt16(Math.Pow(2, (MyBit - 1)));
			return ((MyByte & BitMask) > 0);
		}
		private string FlipHexVal(string sHexStr)
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

		private Int64 Hex2Int64(string strHex)
		{
			Int64 lngResult = 0;
			int intIndex;
			string strDigit;
			Int64 intDigit;
			Int64 intValue;
			for (intIndex = strHex.Length; intIndex >= 1; intIndex += -1) {
				strDigit = Strings.Mid(strHex, intIndex, 1);
				intDigit = InStr("0123456789ABCDEF", UCase(strDigit)) - 1;
				if (intDigit >= 0) {
					intValue = System.Convert.ToInt64(intDigit * (Math.Pow(16, (strHex.Length - (long)intIndex))));
					lngResult = lngResult + intValue;
				} else {
					lngResult = 0;
					intIndex = 0;
					// stop the loop
				}
			}
			System.Windows.Forms.Application.DoEvents();
			return lngResult;
		}

		private byte[] HexStringToByteArray(string sHexStr)
		{
			int a;
			byte[] dByte =null;
			if (sHexStr.Length % 2 != 0) {
				return null;
			
			}
			 // ERROR: Not supported in C#: ReDimStatement

			if (sHexStr.Length > 2) {
				for (a = 1; a <= sHexStr.Length - 1; a += 2) {
					dByte[(a - 1) / 2] = (byte)this.Hex2Int64(Mid(sHexStr, a, 2));
				}
			} else {
				dByte[0] = (byte)this.Hex2Int64(sHexStr);
			}
			System.Windows.Forms.Application.DoEvents();
			return dByte;
		}
		private byte[] StringToBytearray(string sStr, int fixlength = 0)
		{
			byte[] bte = new byte[sStr.Length - 1];
			int i = 0;
			for (i = 1; i <= sStr.Length; i++) {
				bte[i - 1] = System.Convert.ToByte(Asc(Mid(sStr, i, 1)));
			}
			if (fixlength > 0 & sStr.Length < fixlength) {
				Array.Resize(ref bte, fixlength);
				while (i <= fixlength) {
					i += 1;
					bte[i - 1] = 32;
				}
			}
			System.Windows.Forms.Application.DoEvents();
			return bte;
		}
		private string ByteArrayToString(byte[] ByteArray, string NullByte = "")
		{
			string result = "";
			for (int a = 0; a <= ByteArray.Length - 1; a++) {
				result += IIf(ByteArray[a] == 0, NullByte, Chr(ByteArray[a]));
			}
			System.Windows.Forms.Application.DoEvents();
			return result;
		}
		private byte[] MergeByteArrays(byte[] bArr1, byte[] bArr2)
		{
			int iDim1 = 0;
			int iDim2 = 0;
			int iDimOut = 0;
			int i = 0;
			byte[] bArrOut = null;
			iDim1 = UBound(bArr1);
			iDim2 = UBound(bArr2);
			iDimOut = iDim1 + iDim2 + 1;
			 // ERROR: Not supported in C#: ReDimStatement

			for (i = 0; i <= iDim1; i++) {
				bArrOut[i] = bArr1[i];
			}
			System.Windows.Forms.Application.DoEvents();
			for (i = 0; i <= iDim2; i++) {
				bArrOut[iDim1 + 1 + i] = bArr2[i];
			}
			System.Windows.Forms.Application.DoEvents();
			return  bArrOut;
		}
	}
}