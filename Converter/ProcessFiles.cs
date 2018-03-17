using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.VisualBasic;
using Internal;
using System.Drawing;
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



public class ProcessFiles
    {

        public event InfoMsgEventHandler InfoMsg;
        public delegate void InfoMsgEventHandler(String Msg, Boolean nolinebreak, Boolean removelast);
        public event ErrMsgEventHandler ErrMsg;
        public delegate void ErrMsgEventHandler();
        public event AdjustnumUTF16EventHandler AdjustnumUTF16;
        public delegate void AdjustnumUTF16EventHandler();
        public event AdjustnumUTF8EventHandler AdjustnumUTF8;
        public delegate void AdjustnumUTF8EventHandler();
        public event AdjustnumSelEventHandler AdjustnumSel;
        public delegate void AdjustnumSelEventHandler();
        public event AdjustnumASCIIEventHandler AdjustnumASCII;
        public delegate void AdjustnumASCIIEventHandler();
        public event AdjustnumTotalEventHandler AdjustnumTotal;
        public delegate void AdjustnumTotalEventHandler();
        public event ProcessedFileEventHandler ProcessedFile;
        public delegate void ProcessedFileEventHandler();

        /// <summary>
        /// Triggered if Processing was finished or Cancelled (checked <see cref="Cancelled"/>)
        /// </summary>
        /// <param name="Sender">Converter Object</param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        public event ProcessFinishedEventHandler ProcessFinished;
        public delegate void ProcessFinishedEventHandler(object Sender, EventArgs e);
        /// <summary>
        /// Triggered if Processing Status Changes (see <see cref="eStatus"/> Enumerator)
        /// </summary>
        /// <param name="sender">Converter Object</param>
        /// <param name="NewStatus">New Status (see <see cref="eStatus"/> Enumerator)</param>
        /// <remarks></remarks>
        public event StatusChangedEventHandler StatusChanged;
        public delegate void StatusChangedEventHandler(object sender, eStatus NewStatus);
        /// <summary>
        /// Triggered if item was removed from <see cref="Data.ListInputFiles"/>
        /// </summary>
        /// <param name="sender">Converter Object</param>
        /// <param name="item">File Item (see <see cref="FileListItem"/>)</param>
        /// <remarks></remarks>
        public event ListItemRemovedEventHandler ListItemRemoved;
        public delegate void ListItemRemovedEventHandler(object sender, FileListItem item);
        private bool _cancelled = false;

        private eStatus _status = eStatus.Idle;
        /// <summary>
        /// Enumerator of Possible Processing States
        /// </summary>
        /// <remarks></remarks>
        public enum eStatus
        {
            Idle = 0,
            Processing = 1,
            Paused = 2
        }
        /// <summary>
        /// Indicated wheather or not the conversion process finished normally or if it was cancelled
        /// </summary>
        /// <value></value>
        /// <returns>True/False</returns>
        /// <remarks></remarks>
        public bool Cancelled {
            get { return this._cancelled; }
        }
        /// <summary>
        /// Returns the current processing status (see <see cref="eStatus"/>)
        /// </summary>
        /// <value></value>
        /// <returns><see cref="eStatus"/></returns>
        /// <remarks></remarks>
        public eStatus Status {
            get { return this._status; }
        }
        /// <summary>
        /// Read only ArrayList of <see cref="ConverterSupport.WebFontDef"/> objects
        /// </summary>
        /// <value></value>
        /// <returns></returns>
        /// <remarks></remarks>
        public ArrayList WebFonts {
            get { return WebFontList; }
        }
        public ProcessFiles()
        {
            InitConst();
        }
        /// <summary>
        /// Aborts the current processing and sets <see cref="Cancelled"/> Property to 'True'
        /// </summary>
        /// <remarks></remarks>
        public void CancelProcessing()
        {
            this._cancelled = true;
        }
        /// <summary>
        /// Pauses the current processing and sets <see cref="Status"/> to eStatus.Paused
        /// </summary>
        /// <remarks></remarks>
        public void PauseProcessing()
        {
            if (this._status == eStatus.Processing) {
                this._status = eStatus.Paused;
                if (StatusChanged != null) {
                    StatusChanged(this, eStatus.Paused);
                }
            }
        }
        /// <summary>
        /// Resumes Processing (if <see cref="Status"/> = 'eStatus.Paused') and returns <see cref="Status"/> to 'eStates.Processing'
        /// </summary>
        /// <remarks></remarks>
        public void ResumeProcessing()
        {
            if (this._status == eStatus.Paused) {
                this._status = eStatus.Processing;
                if (StatusChanged != null) {
                    StatusChanged(this, eStatus.Processing);
                }
            }
        }

        private void ProcessInfoMsg(string msg, bool nolinebreak, bool removelast)
        {
            if (InfoMsg != null) {
                InfoMsg(msg, nolinebreak, removelast);
            }
        }
        /// <summary>
        /// Begins Processing of all Items in <see cref="Data.ListInputFiles"/>
        /// </summary>
        /// <remarks></remarks>
        public void ConvertAllFiles()
        {
            _cancelled = false;
            Data.oAnsi = new MediaFormats.ANSI();
        Data.oAnsi.InfoMsg += ProcessInfoMsg;

            if (InternalConstants.AnsiColorsARGBM.Count < 256) {
            InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 0, 0, 173));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 0, 173, 0));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 0, 173, 173));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 173, 0, 0));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 173, 0, 173));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 173, 82, 0));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 173, 173, 173));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 82, 82, 82));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 82, 82, 255));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 82, 255, 82));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 82, 255, 255));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 255, 82, 82));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 255, 82, 255));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 255, 255, 82));
                InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
                for (int a = InternalConstants.AnsiColorsARGBM.Count; a <= 255; a++) {
                    InternalConstants.AnsiColorsARGBM.Add(System.Windows.Media.Color.FromArgb(255, 0, 0, 0));
                }
            }
            if (Data.DosFnt80x25.FontSet == false) {
            Data.DosFnt80x25 = new MediaSupport.Ansifntdef(8, 16, Color.FromArgb(255, 0, 0, 0), Resources.fnt80x25, Resources.dosfontback16c);
            }
            if (Data.DosFnt80x50.FontSet == false) {
            Data.DosFnt80x50 = new MediaSupport.Ansifntdef(8, 8, Color.FromArgb(255, 0, 0, 0), Resources.fnt80x50, Resources.dosfontback16c);
            }

            int ProcessedCount = 0;
            int ConvertedCount = 0;
            int ErrorCount = 0;
            int SkippedCount = 0;
            string[] Ret = new string[] {
            0,
            "",
            ""
        };
            byte[] bteWork1 = new byte[] { 0 };
            byte[] bteWork2 = new byte[] { 0 };
            int[] bteWork3 = new int[] { 0 };
            string strWork1 = "";
            string strWork2 = "";
            int iLp = 0;
            bool bResetMappings = false;
            bool bSkipIt = false;
            string PreviousInputFormat = "";
            sOutPutFormat = pOut;
            //sInputFormat = MainForm.pIn.Tag.ToString
            if (pSauce == "Strip") {
                bOutputSauce = false;
            } else {
                bOutputSauce = true;
            }
            if (pAnim == "Static") {
                bAnimation = false;
            } else {
                bAnimation = true;
            }
            if (sOutPutFormat == "AVI") {
                ConverterSupport.WriteFile(ffmpegpath, Resources.ffmpeg, true, 0, true, true);
            }
            bResetMappings = ResetMappings();
            OutputFileExists = (int)pOutExist;
            //For a As Integer = ListInputFiles.Count - 1 To 0 Step -1
            int iToDoCount = ListInputFiles.Count;

            this._status = eStatus.Processing;
            if (StatusChanged != null) {
                StatusChanged(this, eStatus.Processing);
            }

            for (int a = 0; a <= ListInputFiles.Count - 1; a += 1) {
                if (this._cancelled == true) {
                    break; // TODO: might not be correct. Was : Exit For
                }
                bool bPauseEnded = true;
                if (this._status == eStatus.Paused) {
                    if (InfoMsg != null) {
                        InfoMsg("[b]Processing Paused![/b]...", false, false);
                    }
                    bPauseEnded = false;
                }
                while (!bPauseEnded) {
                    if (this._status == eStatus.Processing) {
                        bPauseEnded = true;
                        if (InfoMsg != null) {
                            InfoMsg("[b]Continue Processing![/b]", true, true);
                        }
                    }
                    System.Threading.Thread.Sleep(100);
                    System.Windows.Forms.Application.DoEvents();
                }
                string sFileNam = ListInputFiles.Item(a).FullPath;
                cBPF = 0;
                FFormats FTyp = ListInputFiles.Item(a).Format;
                sInputFormat = Internal.aInp(ListInputFiles.Item(a).Type);
                if (PreviousInputFormat == "") {
                    PreviousInputFormat = sInputFormat;
                }
                ProcessedCount += 1;
                if (InfoMsg != null) {
                    InfoMsg("Processing (" + ProcessedCount.ToString + "/" + iToDoCount.ToString + ") [b]" + sFileNam + "[/b]", false, false);
                }
                string sOutF = ConverterSupport.DetermineOutputFileName(sFileNam);
                OutFileWrite = sOutF;
                //  RaiseEvent InfoMsg("Target Output: " & sOutF & ",rOutPathInput=" & rOutPathInput & ",rReplaceExt=" & rReplaceExt & ",txtExt=" & txtExt & ",outPath=" & outPath)
                ProcFilesCounter += 1;
                if (OutputFileExists == 1 & File.Exists(sOutF)) {
                    if (InfoMsg != null) {
                        InfoMsg("Output File [i]" + sOutF + "[/i] exists. SKIP!", false, false);
                    }
                    SkippedCount += 1;
                } else {
                    bSkipIt = false;
                    maxX = 80;
                    bHasSauce = false;
                    oSauce = new ConverterSupport.SauceMeta();

                    if (bResetMappings == true | PreviousInputFormat != sInputFormat) {
                        bResetMappings = ResetMappings();
                    }
                    PreviousInputFormat = sInputFormat;
                    switch (sInputFormat) {
                        case "ASC":
                            if (FTyp == FFormats.utf_16 | FTyp == FFormats.utf_8) {
                                if (ErrMsg != null) {
                                    ErrMsg("Input file: " + sFileNam + " is Unicode format and not ASCII. File Skipped!");
                                }
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false & sOutPutFormat == "ASC") {
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false) {
                                if (sOutPutFormat == "UTF" | sOutPutFormat == "ANS" | sOutPutFormat == "BBS") {
                                    //Read ASC File to a Byte Array
                                    bteWork1 = ConverterSupport.InputOutput.ReadBinaryFile(sFileNam);
                                }
                                if (sOutPutFormat == "HTML" | sOutPutFormat == "IMG") {
                                    //Read ASC File as a String
                                    strWork1 = ConverterSupport.ReadFile(sFileNam);
                                    bHasSauce = oSauce.GetFromFile(sFileNam);
                                    if (bHasSauce == true) {
                                        int iOff = InStr(1, strWork1, Chr(26) + "SAUCE00", CompareMethod.Binary);
                                        if (iOff == 0) {
                                            iOff = InStr(1, strWork1, "SAUCE00", CompareMethod.Binary);
                                        }
                                        if (iOff > 0) {
                                            strWork1 = Strings.Left(strWork1, iOff - 1);
                                        }
                                    }
                                    strWork1 = Strings.Replace(strWork1, Environment.NewLine, "\n", 1, -1, CompareMethod.Binary);
                                    strWork1 = Strings.Replace(strWork1, "\r", "\n", 1, -1, CompareMethod.Binary);
                                    strWork1 = Strings.Replace(strWork1, "\n", Environment.NewLine, 1, -1, CompareMethod.Binary);
                                }
                                if (sOutPutFormat == "BIN") {
                                    bConv2Unicode = false;
                                    //Read ASCII File and Convert it to Custom Class/Array
                                    oAnsi.ProcessANSIFile(sFileNam);
                                }
                            }
                        case "ANS":
                            if (FTyp == FFormats.utf_16 | FTyp == FFormats.utf_8) {
                                if (ErrMsg != null) {
                                    ErrMsg("Input file: " + sFileNam + " is Unicode format and not an ANSI in US-ASCII Encoded format. File Skipped!");
                                }
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false & sOutPutFormat == "ANS") {
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false) {
                                if (sOutPutFormat == "AVI") {
                                    string VideoFile = "";
                                    Ret = ConverterSupport.WriteFile(sOutF, "", bForceOverwrite, OutputFileExists, true, false);
                                    VideoFile = Ret(1);
                                    OutFileWrite = VideoFile;
                                    TempVideoFolder = Path.Combine(Path.GetTempPath, "ANSIToVideoTemp");
                                    if (!Directory.Exists(TempVideoFolder)) {
                                        Directory.CreateDirectory(TempVideoFolder);
                                    } else {
                                        foreach (string fil in Directory.GetFiles(TempVideoFolder)) {
                                            try {
                                                File.Delete(fil);
                                            } catch (Exception ex) {
                                            }
                                        }
                                    }
                                    bMakeVideo = true;
                                    //oAVIFile = New AviWriter

                                    //oAVIFile.OpenAVI(VideoFile, Math.Round(FPS, 0), iAVIWidth, iAVIHeight)
                                }
                                switch (sOutPutFormat) {
                                    case "HTML":
                                        bConv2Unicode = true;
                                    default:
                                        bConv2Unicode = false;
                                }
                                //Read ANSI File and Convert it to Custom Class/Array
                                if (sOutPutFormat == "HTML" & bAnimation == true) {
                                    Ret = MediaFormats.ProcessANSIAnimationFile(sFileNam, sOutF);
                                } else {
                                    //Also used for Video Conversion of Ansi Animations
                                    oAnsi.ProcessANSIFile(sFileNam);
                                }
                            }
                        case "HTML":
                            if (FTyp == FFormats.utf_16 | FTyp == FFormats.utf_8) {
                                if (ErrMsg != null) {
                                    ErrMsg("Input file: " + sFileNam + " is Unicode format and not HTML Code in ASCII Format. File Skipped!");
                                }
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false & sOutPutFormat == "HTML") {
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false) {
                                //Read HTML Document as ASCII Text to a String Variable
                                strWork1 = ConverterSupport.ReadFile(sFileNam);
                                //
                                strWork1 = ConverterSupport.CutorSandR(strWork1, "<html>", "<div class=ANSICSS>", "I", "I", "C", 1);
                                strWork1 = ConverterSupport.CutorSandR(strWork1, "</div>", "</html>", "I", "I", "C", 1);
                                strWork1 = ConverterSupport.CutorSandR(strWork1, "<style>", "</style>", "I", "I", "C", "");
                                strWork1 = ConverterSupport.CutorSandR(strWork1, "<script", "</script>", "I", "I", "C", "");
                                strWork1 = ConverterSupport.RegExReplace(strWork1, "", "</?\\w+((\\s+\\w+(\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?>", RegexOptions.IgnoreCase, true);
                            }
                        case "UTF":
                            if (FTyp != FFormats.utf_16 & FTyp != FFormats.utf_8) {
                                if (ErrMsg != null) {
                                    ErrMsg("Input file: " + sFileNam + " is neither UTF-8 nor UTF-16 format. Skipped!");
                                }
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false & sOutPutFormat == "UTF") {
                                if ((FTyp == FFormats.utf_16 & selUTF == "UTF16") | (FTyp == FFormats.utf_8 & selUTF == "UTF8")) {
                                    bSkipIt = true;
                                    SkippedCount += 1;
                                }
                            }
                            if (bSkipIt == false) {
                                bteWork1 = ConverterSupport.InputOutput.ReadBinaryFile(sFileNam);
                                if (FTyp == FFormats.utf_16) {
                                    //Read UTF-16 Encoded Text File to a String Variable
                                    strWork1 = ConverterSupport.ByteArrayToStr(bteWork1, FFormats.utf_16);
                                }
                                if (FTyp == FFormats.utf_8) {
                                    //Read UTF-8 Encoded Text File to a String Variable
                                    strWork1 = ConverterSupport.ByteArrayToStr(bteWork1, FFormats.utf_8);
                                }
                            }
                        case "PCB":
                            if (FTyp == FFormats.utf_16 | FTyp == FFormats.utf_8) {
                                if (ErrMsg != null) {
                                    ErrMsg("Input file: " + sFileNam + " is Unicode format and not an PCB @ Styled Ansi in US-ASCII Encoded format. File Skipped!");
                                }
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false & sOutPutFormat == "BBS") {
                                if (pBBS == "PCB") {
                                    bSkipIt = true;
                                    SkippedCount += 1;
                                }
                            }
                            if (bSkipIt == false) {
                                //Read PCB @ Styled ANSI File to Screen Array
                                switch (sOutPutFormat) {
                                    case "HTML":
                                        bConv2Unicode = true;
                                    default:
                                        bConv2Unicode = false;
                                }
                                //Read PCB File and Convert it to Custom Class/Array
                                MediaFormats.ProcessPCBFile(sFileNam);
                            }
                        case "WC2":
                            if (FTyp == FFormats.utf_16 | FTyp == FFormats.utf_8) {
                                if (ErrMsg != null) {
                                    ErrMsg("Input file: " + sFileNam + " is Unicode format and not an PCB @ Styled Ansi in US-ASCII Encoded format. File Skipped!");
                                }
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false & sOutPutFormat == "BBS") {
                                if (pBBS == "WC2") {
                                    bSkipIt = true;
                                    SkippedCount += 1;
                                }
                            }
                            if (bSkipIt == false) {
                                switch (sOutPutFormat) {
                                    case "HTML":
                                        bConv2Unicode = true;
                                    default:
                                        bConv2Unicode = false;
                                }
                                //Read WC2 File and Convert it to Custom Class/Array
                                MediaFormats.ProcessWC2File(sFileNam);
                            }
                        case "WC3":
                            if (FTyp == FFormats.utf_16 | FTyp == FFormats.utf_8) {
                                if (ErrMsg != null) {
                                    ErrMsg("Input file: " + sFileNam + " is Unicode format and not an PCB @ Styled Ansi in US-ASCII Encoded format. File Skipped!");
                                }
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false & sOutPutFormat == "BBS") {
                                if (pBBS == "WC3") {
                                    bSkipIt = true;
                                    SkippedCount += 1;
                                }
                            }
                            if (bSkipIt == false) {
                                switch (sOutPutFormat) {
                                    case "HTML":
                                        bConv2Unicode = true;
                                    default:
                                        bConv2Unicode = false;
                                }
                                //Read WC3 File and Convert it to Custom Class/Array
                                MediaFormats.ProcessWC3File(sFileNam);
                            }
                        case "AVT":
                            if (FTyp == FFormats.utf_16 | FTyp == FFormats.utf_8) {
                                if (ErrMsg != null) {
                                    ErrMsg("Input file: " + sFileNam + " is Unicode format and not an PCB @ Styled Ansi in US-ASCII Encoded format. File Skipped!");
                                }
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false & sOutPutFormat == "BBS") {
                                if (pBBS == "AVT") {
                                    bSkipIt = true;
                                    SkippedCount += 1;
                                }
                            }
                            if (bSkipIt == false) {
                                switch (sOutPutFormat) {
                                    case "HTML":
                                        bConv2Unicode = true;
                                    default:
                                        bConv2Unicode = false;
                                }
                                //Read AVT File and Convert it to Custom Class/Array
                                MediaFormats.ProcessAVTFile(sFileNam);
                            }
                        case "BIN":
                            if (FTyp == FFormats.utf_16 | FTyp == FFormats.utf_8) {
                                if (ErrMsg != null) {
                                    ErrMsg("Input file: " + sFileNam + " is Unicode format and not a DOS Binary Ansi. File Skipped!");
                                }
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false & sOutPutFormat == "BIN") {
                                bSkipIt = true;
                                SkippedCount += 1;
                            }
                            if (bSkipIt == false) {
                                //Read DOS Binary ANSI File to a Byte Array
                                switch (sOutPutFormat) {
                                    case "HTML":
                                        bConv2Unicode = true;
                                    default:
                                        bConv2Unicode = false;
                                }
                                //Read BIN File and Convert it to Custom Class/Array
                                maxX = 160;
                                MediaFormats.ProcessBINFile(sFileNam);
                            }
                    }
                    System.Windows.Forms.Application.DoEvents();
                    if (bSkipIt == false) {
                        //Input seams okay, no skipping of file 

                        bForceOverwrite = false;
                        switch (sOutPutFormat) {
                            case "ASC":
                                if (sInputFormat == "UTF") {
                                    //Convert Unicode Text File to ASC File
                                    strWork2 = "";
                                    for (iLp = 2; iLp <= strWork1.Length; iLp++) {
                                        strWork2 += Microsoft.VisualBasic.Right("0" + Hex(Asc(ConverterSupport.UnicodeToAscii(AscW(Mid(strWork1, iLp, 1))))), 2);
                                    }
                                    Ret = ConverterSupport.WriteFile(sOutF, ConverterSupport.HexStringToByteArray(strWork2), bForceOverwrite, OutputFileExists, false, true);
                                }
                                if (sInputFormat == "HTML") {
                                    //Convert HTML Encoded Unicode ASCII to ASC File
                                    strWork2 = ConverterSupport.convuniasc(strWork1);
                                    strWork2 = Replace(strWork2, Chr(255), " ", 1, -1, CompareMethod.Binary);
                                    if (InStr(strWork2, Environment.NewLine, CompareMethod.Text) > 0) {
                                        string[] aTmp1 = Split(strWork2, Environment.NewLine);
                                        for (int b = 0; b <= UBound(aTmp1); b++) {
                                            aTmp1(b) = RTrim(aTmp1(b));
                                        }
                                        strWork2 = Join(aTmp1, Environment.NewLine);
                                    } else {
                                        strWork2 = RTrim(strWork2);
                                    }
                                    int iLen = Microsoft.VisualBasic.Len(strWork2);
                                    // ERROR: Not supported in C#: ReDimStatement

                                    for (iLp = 1; iLp <= iLen; iLp++) {
                                        bteWork1(iLp - 1) = Asc(Mid(strWork2, iLp, 1));
                                    }
                                    Ret = ConverterSupport.WriteFile(sOutF, bteWork1, bForceOverwrite, OutputFileExists, false, true);
                                }
                                if (sInputFormat == "ANS" | sInputFormat == "PCB" | sInputFormat == "BIN" | sInputFormat == "WC2" | sInputFormat == "WC3" | sInputFormat == "AVT") {
                                    //Save ANSI as ASCII
                                    bteWork1 = ConverterSupport.ANSIScreenToASCIIByteArray();
                                    Ret = ConverterSupport.WriteFile(sOutF, bteWork1, bForceOverwrite, OutputFileExists, false, true);
                                }
                            case "ANS":
                                if (sInputFormat == "ASC") {
                                    //Create ANSI from ASC File
                                    Ret = ConverterSupport.WriteFile(sOutF, ConverterSupport.Convert.MergeByteArrays(Internal.ANSIHdr, bteWork1), bForceOverwrite, OutputFileExists, false, true);
                                }
                                if (sInputFormat == "PCB" | sInputFormat == "WC2" | sInputFormat == "WC3" | sInputFormat == "AVT" | sInputFormat == "BIN") {
                                    Ret = ConverterSupport.OutputANS(sOutF);
                                }
                                if (sInputFormat == "HTML") {
                                    strWork2 = ConverterSupport.convuniasc(strWork1);
                                    strWork2 = Replace(strWork2, Chr(255), " ", 1, -1, CompareMethod.Binary);
                                    if (InStr(strWork2, Environment.NewLine, CompareMethod.Text) > 0) {
                                        string[] aTmp1 = Split(strWork2, Environment.NewLine);
                                        for (int b = 0; b <= UBound(aTmp1); b++) {
                                            aTmp1(b) = RTrim(aTmp1(b));
                                        }
                                        strWork2 = Join(aTmp1, Environment.NewLine);
                                    } else {
                                        strWork2 = RTrim(strWork2);
                                    }
                                    int iLen = Microsoft.VisualBasic.Len(strWork2);
                                    // ERROR: Not supported in C#: ReDimStatement

                                    for (iLp = 1; iLp <= iLen; iLp++) {
                                        bteWork1(iLp - 1) = Asc(Mid(strWork2, iLp, 1));
                                    }
                                    Ret = ConverterSupport.WriteFile(sOutF, ConverterSupport.Convert.MergeByteArrays(Internal.ANSIHdr, bteWork1), bForceOverwrite, OutputFileExists, false, true);
                                }
                                if (sInputFormat == "UTF") {
                                    strWork2 = "";
                                    for (iLp = 2; iLp <= strWork1.Length; iLp++) {
                                        strWork2 += Microsoft.VisualBasic.Right("0" + Hex(Asc(ConverterSupport.UnicodeToAscii(AscW(Mid(strWork1, iLp, 1))))), 2);
                                    }
                                    bteWork1 = ConverterSupport.HexStringToByteArray(strWork2);
                                    Ret = ConverterSupport.WriteFile(sOutF, ConverterSupport.Convert.MergeByteArrays(Internal.ANSIHdr, bteWork1), bForceOverwrite, OutputFileExists, false, true);
                                }
                            case "HTML":
                                if (sInputFormat == "ANS" | sInputFormat == "PCB" | sInputFormat == "BIN" | sInputFormat == "WC2" | sInputFormat == "WC3" | sInputFormat == "AVT") {
                                    //Convert ANSI to Html Encoded Unicode and CSS
                                    if (bAnimation == false) {
                                        Ret = ConverterSupport.OutputHTML(sOutF);
                                    }
                                }
                                if (sInputFormat == "ASC") {
                                    //Convert ASC to HTML Encoded Unicode
                                    strWork2 = ConverterSupport.convascuni(strWork1);
                                    Ret = ConverterSupport.OutputASCHTML(sOutF, strWork2);
                                }
                                if (sInputFormat == "UTF") {
                                    //Convert Unicode Text File to HTML Encoded Unicode 
                                    //Currently converting the Text File back to an ASCII and then the 
                                    //ASCII back again to Encoded Unicode. Not very efficient and should 
                                    //be changed when there is time.
                                    strWork2 = "";
                                    for (iLp = 2; iLp <= strWork1.Length; iLp++) {
                                        strWork2 += Microsoft.VisualBasic.Right("0" + Hex(Asc(ConverterSupport.UnicodeToAscii(AscW(Mid(strWork1, iLp, 1))))), 2);
                                    }
                                    strWork1 = ConverterSupport.ByteArrayToString(ConverterSupport.HexStringToByteArray(strWork2));
                                    strWork2 = ConverterSupport.convascuni(strWork1);
                                    Ret = ConverterSupport.OutputASCHTML(sOutF, strWork2);
                                }
                            case "UTF":
                                if (sInputFormat == "ASC" | sInputFormat == "HTML") {
                                    //Convert Unicode Encoded HTML ASCII to ASC Byte Array
                                    if (sInputFormat == "HTML") {
                                        strWork2 = ConverterSupport.convuniasc(strWork1);
                                        strWork1 = Replace(strWork2, Chr(255), " ", 1, -1, CompareMethod.Binary);
                                        strWork2 = ConverterSupport.convuniuni(strWork1);
                                        if (InStr(strWork2, Environment.NewLine, CompareMethod.Text) > 0) {
                                            string[] aTmp1 = Split(strWork2, Environment.NewLine);
                                            for (int b = 0; b <= UBound(aTmp1); b++) {
                                                aTmp1(b) = RTrim(aTmp1(b));
                                            }
                                            strWork2 = Join(aTmp1, Environment.NewLine);
                                        } else {
                                            strWork2 = RTrim(strWork2);
                                        }
                                        //Dim iLen As Integer = Microsoft.VisualBasic.Len(strWork2)
                                        //ReDim bteWork3(iLen - 1)
                                        //For iLp = 1 To iLen
                                        // bteWork3(iLp - 1) = AscW(Mid(strWork2, iLp, 1))
                                        //Next
                                        //bConv2Unicode = False
                                        //bHTMLEncode = False
                                        //Call BuildMappings(sCodePg)
                                        //bResetMappings = True
                                    }
                                }
                                if (sInputFormat == "ANS" | sInputFormat == "BIN" | sInputFormat == "PCB" | sInputFormat == "WC2" | sInputFormat == "WC3" | sInputFormat == "AVT") {
                                    bteWork1 = ConverterSupport.ANSIScreenToASCIIByteArray();
                                }
                                //Convert ASC File to Unicode Text File
                                strWork1 = "";
                                if (sInputFormat == "HTML") {
                                    strWork1 = strWork2;
                                } else {
                                    for (iLp = 0; iLp <= UBound(bteWork1); iLp++) {
                                        strWork1 += ConverterSupport.AsciiToUnicode(bteWork1(iLp));
                                    }
                                }
                                //MainForm.rUTF16.Checked = True Then
                                if (selUTF == "UTF16") {
                                    //Save as UTF-16 Encoded Text File
                                    bteWork2 = ConverterSupport.StrToByteArray(strWork1, FFormats.utf_16);
                                    Ret = ConverterSupport.WriteFile(sOutF, ConverterSupport.Convert.MergeByteArrays(Internal.UTF16Hdr, bteWork2), bForceOverwrite, OutputFileExists, false, true);
                                }
                                //MainForm.rUTF8.Checked = True Then
                                if (selUTF == "UTF8") {
                                    //Save as UTF-8 Encoded Text File
                                    bteWork2 = ConverterSupport.StrToByteArray(strWork1, FFormats.utf_8);
                                    Ret = ConverterSupport.WriteFile(sOutF, ConverterSupport.Convert.MergeByteArrays(Internal.UTF8Hdr, bteWork2), bForceOverwrite, OutputFileExists, false, true);

                                }

                            case "BBS":
                                if (sInputFormat == "ANS" | sInputFormat == "BIN" | sInputFormat == "WC2" | sInputFormat == "WC3" | sInputFormat == "AVT") {
                                    //Save ANSI to PCBoard @ Styled ANSI
                                    switch (pBBS) {
                                        case "PCB":
                                            Ret = ConverterSupport.OutputPCB(sOutF);
                                        case "AVT":
                                            Ret = ConverterSupport.OutputAVT(sOutF);
                                        case "WC2":
                                            Ret = ConverterSupport.OutputWC2(sOutF);
                                        case "WC3":
                                            Ret = ConverterSupport.OutputWC3(sOutF);
                                    }
                                }
                                if (sInputFormat == "ASC") {
                                    //Create PCBoard @ Styled ANSI from ASC File
                                    Ret = ConverterSupport.WriteFile(sOutF, ConverterSupport.Convert.MergeByteArrays(Internal.PCBHdr, bteWork1), bForceOverwrite, OutputFileExists, false, true);
                                }
                                if (sInputFormat == "HTML") {
                                    strWork2 = ConverterSupport.convuniasc(strWork1);
                                    strWork2 = Replace(strWork2, Chr(255), " ", 1, -1, CompareMethod.Binary);
                                    if (InStr(strWork2, Environment.NewLine, CompareMethod.Text) > 0) {
                                        string[] aTmp1 = Split(strWork2, Environment.NewLine);
                                        for (int b = 0; b <= UBound(aTmp1); b++) {
                                            aTmp1(b) = RTrim(aTmp1(b));
                                        }
                                        strWork2 = Join(aTmp1, Environment.NewLine);
                                    } else {
                                        strWork2 = RTrim(strWork2);
                                    }
                                    int iLen = Microsoft.VisualBasic.Len(strWork2);
                                    // ERROR: Not supported in C#: ReDimStatement

                                    for (iLp = 1; iLp <= iLen; iLp++) {
                                        bteWork1(iLp - 1) = Asc(Mid(strWork2, iLp, 1));
                                    }
                                    bConv2Unicode = false;
                                    bHTMLEncode = false;
                                    //Convert ASCII from Byte Array to Custom Class/Array
                                    oAnsi.ProcessANSIFile(sFileNam, bteWork1);
                                    switch (pBBS) {
                                        case "PCB":
                                            Ret = ConverterSupport.OutputPCB(sOutF);
                                        case "AVT":
                                            Ret = ConverterSupport.OutputAVT(sOutF);
                                        case "WC2":
                                            Ret = ConverterSupport.OutputWC2(sOutF);
                                        case "WC3":
                                            Ret = ConverterSupport.OutputWC3(sOutF);
                                    }

                                }
                                if (sInputFormat == "UTF") {
                                    //Convert Unicode Text File to ASCII Byte Array and from there to Custom Class to PCB
                                    strWork2 = "";
                                    for (iLp = 2; iLp <= strWork1.Length; iLp++) {
                                        strWork2 += Microsoft.VisualBasic.Right("0" + Hex(Asc(ConverterSupport.UnicodeToAscii(AscW(Mid(strWork1, iLp, 1))))), 2);
                                    }
                                    bConv2Unicode = false;
                                    bHTMLEncode = false;
                                    oAnsi.ProcessANSIFile(sFileNam, ConverterSupport.HexStringToByteArray(strWork2));
                                    Ret = ConverterSupport.OutputPCB(sOutF);
                                }
                            case "BIN":
                                if (sInputFormat == "ANS" | sInputFormat == "PCB" | sInputFormat == "ASC" | sInputFormat == "WC2" | sInputFormat == "WC3" | sInputFormat == "AVT") {
                                    //Save ANSI as Binary DOS File
                                    if (bDebug == true)
                                        Console.WriteLine("output filename:" + sOutF);
                                    maxX = 160;
                                    Screen = ConverterSupport.ResizeScreen(Screen, maxX, LinesUsed);
                                    Ret = ConverterSupport.OutputBin(sOutF);
                                }
                                if (sInputFormat == "HTML") {
                                    strWork2 = ConverterSupport.convuniasc(strWork1);
                                    strWork2 = Replace(strWork2, Chr(255), " ", 1, -1, CompareMethod.Binary);
                                    if (InStr(strWork2, Environment.NewLine, CompareMethod.Text) > 0) {
                                        string[] aTmp1 = Split(strWork2, Environment.NewLine);
                                        for (int b = 0; b <= UBound(aTmp1); b++) {
                                            aTmp1(b) = RTrim(aTmp1(b));
                                        }
                                        strWork2 = Join(aTmp1, Environment.NewLine);
                                    } else {
                                        strWork2 = RTrim(strWork2);
                                    }
                                    int iLen = Microsoft.VisualBasic.Len(strWork2);
                                    // ERROR: Not supported in C#: ReDimStatement

                                    for (iLp = 1; iLp <= iLen; iLp++) {
                                        bteWork1(iLp - 1) = Asc(Mid(strWork2, iLp, 1));
                                    }
                                    bConv2Unicode = false;
                                    bHTMLEncode = false;
                                    //Convert ASCII from Byte Array to Custom Class/Array
                                    oAnsi.ProcessANSIFile(sFileNam, bteWork1);
                                    maxX = 160;
                                    Screen = ConverterSupport.ResizeScreen(Screen, maxX, LinesUsed);
                                    Ret = ConverterSupport.OutputBin(sOutF);
                                }
                                if (sInputFormat == "UTF") {
                                    //Convert Unicode Text File to ASCII Byte Array and from there to Custom Class to Binary
                                    strWork2 = "";
                                    for (iLp = 2; iLp <= strWork1.Length; iLp++) {
                                        strWork2 += Strings.Right("0" + Hex(Asc(ConverterSupport.UnicodeToAscii(AscW(Mid(strWork1, iLp, 1))))), 2);
                                    }
                                    bConv2Unicode = false;
                                    bHTMLEncode = false;
                                    oAnsi.ProcessANSIFile(sFileNam, ConverterSupport.HexStringToByteArray(strWork2));
                                    maxX = 160;
                                    Screen = ConverterSupport.ResizeScreen(Screen, maxX, LinesUsed);
                                    Ret = ConverterSupport.OutputBin(sOutF);
                                }
                            case "IMG":
                                Drawing.Bitmap i;
                                bool bLocNoColors = pNoColors;

                                if (sInputFormat == "UTF") {
                                    strWork2 = "";
                                    for (iLp = 2; iLp <= strWork1.Length; iLp++) {
                                        strWork2 += Strings.Right("0" + Hex(Asc(ConverterSupport.UnicodeToAscii(AscW(Mid(strWork1, iLp, 1))))), 2);
                                    }
                                    strWork1 = ConverterSupport.HexStringToString(strWork2);
                                    bLocNoColors = true;
                                }
                                if (sInputFormat == "HTML") {
                                    //Convert HTML Encoded Unicode ASCII to ASC File
                                    strWork2 = ConverterSupport.convuniasc(strWork1);
                                    strWork2 = Replace(strWork2, Chr(255), " ", 1, -1, CompareMethod.Binary);
                                    if (InStr(strWork2, Environment.NewLine, CompareMethod.Text) > 0) {
                                        string[] aTmp1 = Split(strWork2, Environment.NewLine);
                                        for (int b = 0; b <= UBound(aTmp1); b++) {
                                            aTmp1(b) = RTrim(aTmp1(b));
                                        }
                                        strWork1 = Join(aTmp1, Environment.NewLine);
                                    } else {
                                        strWork1 = RTrim(strWork2);
                                    }
                                    bLocNoColors = true;
                                }
                                if (sInputFormat == "ANS" | sInputFormat == "PCB" | sInputFormat == "BIN" | sInputFormat == "WC2" | sInputFormat == "WC3" | sInputFormat == "AVT") {
                                    //Save ANSI as ASCII
                                    if (pNoColors == true) {
                                        bteWork1 = ConverterSupport.ANSIScreenToASCIIByteArray();
                                        strWork1 = ConverterSupport.ByteArrayToString(bteWork1);
                                    }
                                    i = MediaFormats.CreateImageFromScreenChars();
                                } else {
                                    i = MediaFormats.CreateImageFromASCII(strWork1, null, null);
                                }

                                if (bLocNoColors == true) {

                                } else {
                                }

                                Ret = ConverterSupport.WriteFile(sOutF, i, bForceOverwrite, OutputFileExists, false, true);
                            case "AVI":
                                //oAVIFile.Close()
                                if (InfoMsg != null) {
                                    InfoMsg("Compiling Video File", false, false);
                                }

                                int iAVIWidth;
                                int iAVIHeight;
                                if (pSmallFont) {
                                    iAVIWidth = DosFnt80x50.Width * 80;
                                    iAVIHeight = DosFnt80x50.Height * 25;
                                } else {
                                    iAVIWidth = DosFnt80x25.Width * 80;
                                    iAVIHeight = DosFnt80x25.Height * 25;
                                }
                                MediaSupport.VideoConverterFFMPEG vc2 = new MediaSupport.VideoConverterFFMPEG();
                                vc2.FFMPEGPath = ffmpegpath;
                                vc2.Imagelist = Path.Combine(TempVideoFolder, Path.GetFileNameWithoutExtension(OutFileWrite) + "_%05d.PNG");
                                vc2.Output = OutFileWrite;
                                vc2.FPS = FPS;
                                vc2.Numframes = iFramesCount;
                                vc2.VideoHeight = iAVIHeight;
                                vc2.VideoWidth = iAVIWidth;
                                vc2.VCodec = VidCodec;
                                switch (VidFmt) {
                                    case "AVI":
                                        vc2.OutFormat = MediaSupport.VideoConverterFFMPEG.OutTypes.AVI;
                                    case "WMV":
                                        vc2.OutFormat = MediaSupport.VideoConverterFFMPEG.OutTypes.WMV;
                                    case "FLV":
                                        vc2.OutFormat = MediaSupport.VideoConverterFFMPEG.OutTypes.FLV;
                                    case "MKV":
                                        vc2.OutFormat = MediaSupport.VideoConverterFFMPEG.OutTypes.MKV;
                                    case "GIF":
                                        vc2.OutFormat = MediaSupport.VideoConverterFFMPEG.OutTypes.GIF;
                                    //vc2.VCodec = "gif"
                                    case "VOB":
                                        vc2.OutFormat = MediaSupport.VideoConverterFFMPEG.OutTypes.VOB;
                                    case "MPG":
                                        vc2.OutFormat = MediaSupport.VideoConverterFFMPEG.OutTypes.MPG;
                                    case "MP4":
                                        vc2.OutFormat = MediaSupport.VideoConverterFFMPEG.OutTypes.MP4;
                                }
                                //iFramesCount

                                if (vc2.Status == MediaSupport.VideoConverterFFMPEG.ConvStates.Ready) {
                                    vc2.Convert();
                                    while (vc2.Status != MediaSupport.VideoConverterFFMPEG.ConvStates.Aborted & vc2.Status != MediaSupport.VideoConverterFFMPEG.ConvStates.Finished) {
                                        System.Threading.Thread.Sleep(200);
                                        System.Windows.Forms.Application.DoEvents();
                                    }
                                }
                                if (Data.bDebug == false) {
                                    foreach (string fil in Directory.GetFiles(TempVideoFolder)) {
                                        try {
                                                File.Delete(fil);
                                        } catch (Exception ex) {
                                        }
                                    }


                                }



                        }


                        if ((int)Ret(0) >= 0) {
                            if (InfoMsg != null) {
                                InfoMsg("Output generated at [b]" + Ret(1) + "[/b]. \r\n" + Ret(2), false, false);
                            }
                            ConvertedCount += 1;
                        } else {
                            if (ErrMsg != null) {
                                ErrMsg("Error writing: " + Ret(1) + ". \r\n" + Ret(2));
                            }
                            ErrorCount += 1;
                        }

                        //Skipped
                    }

                    //Existing output skipped
                }
                //  If MainForm.bRemoveCompleted.Checked = True Then
                if (bRemoveCompleted == true) {
                    switch (ListInputFiles.Item(a).Format) {
                        case FFormats.utf_16:

                            if (AdjustnumUTF16 != null) {
                                AdjustnumUTF16(-1);
                            }

                        case FFormats.utf_8:

                            if (AdjustnumUTF8 != null) {
                                AdjustnumUTF8(-1);
                            }

                        default:

                            if (AdjustnumASCII != null) {
                                AdjustnumASCII(-1);
                            }

                    }

                    if (AdjustnumTotal != null) {
                        AdjustnumTotal(-1);
                    }
                    if (ListInputFiles.Item(a).Selected == true) {
                        if (AdjustnumSel != null) {
                            AdjustnumSel(-1);
                        }
                    }
                    FileListItem item = ListInputFiles.Item(a);
                    ListInputFiles.Remove(item);
                    if (ListItemRemoved != null) {
                        ListItemRemoved(this, item);
                    }
                    if (ProcessedFile != null) {
                        ProcessedFile(a);
                    }
                    if (InfoMsg != null) {
                        InfoMsg("[s]-[/s]", false, false);
                    }
                    a -= 1;
                    if (ListInputFiles.Count == 0) {
                        break; // TODO: might not be correct. Was : Exit For
                    }
                    // MainForm.listFiles.DataSource = Nothing
                    // MainForm.listFiles.DataSource = ListInputFiles
                    // UpdateCounts()
                    // End If
                } else {
                    if (ProcessedFile != null) {
                        ProcessedFile(a);
                    }
                    if (InfoMsg != null) {
                        InfoMsg("[s]-[/s]", false, false);
                    }
                }

                System.Windows.Forms.Application.DoEvents();
            }
            if (sOutPutFormat == "AVI") {
                try {
                    File.Delete(ffmpegpath);
                    Directory.Delete(TempVideoFolder);

                } catch (Exception ex) {
                }
            }
            string sFinalMessage = "";

            if (ProcessedCount > ConvertedCount) {
                sFinalMessage = "Total Processed: [b]" + ProcessedCount + "[/b] ";
                if (ConvertedCount > 0) {
                    sFinalMessage += ", Converted: [t]" + ConvertedCount + "[/t]";
                }
                if (SkippedCount > 0) {
                    sFinalMessage += ", Skipped: [b]" + SkippedCount + "[/b]";
                }
                if (ErrorCount > 0) {
                    sFinalMessage += ", Errors: [e]" + ErrorCount + "[/e]";
                }
            } else {
                sFinalMessage = "Total Converted: [t]" + ConvertedCount + "[/t] ";
            }
            if (InfoMsg != null) {
                InfoMsg(sFinalMessage, false, false);
            }
            if (this._cancelled) {
                if (InfoMsg != null) {
                    InfoMsg("[e]Processing cancelled by user![/e]", false, false);
                }
            } else {
                if (InfoMsg != null) {
                    InfoMsg("[f]Done Processing[/f]", false, false);
                }
            }

            if (ProcessFinished != null) {
                ProcessFinished(this, EventArgs.Empty);
            }

            this._status = eStatus.Idle;
            if (StatusChanged != null) {
                StatusChanged(this, eStatus.Idle);
            }
        }

        /// <summary>
        /// Resets Code Page Mapper Settings to Initial Value
        /// </summary>
        /// <returns>False</returns>
        /// <remarks></remarks>
        public bool ResetMappings()
        {
        Data.bSanitize = Data.pSanitize;
        Data.sCodePg = Data.pCP;
        Data.bHTMLEncode = Data.pHTMLEncode;
        Data.bHTMLComplete = Data.pHTMLComplete;
        ConverterSupport.Mappings.BuildMappings(Data.sCodePg);

            return false;

        }

        /// <summary>
        /// Initializes Required Constants for HTML Processing and Code Page Mappings
        /// </summary>
        /// <remarks></remarks>

        public void InitConst()
        {
        InternalConstants.aCSS[0, 0] = "B0";
        InternalConstants.aCSS[1, 0] = "C0";
        InternalConstants.aCSS[2, 0]= "#000000";
        InternalConstants.aCSS[0, 1]= "B1";
        InternalConstants.aCSS[1, 1]= "C1";
        InternalConstants.aCSS[2, 1] = "#0000AA";
        InternalConstants.aCSS[0, 2] = "B2";
        InternalConstants.aCSS[1, 2] = "C2";
        InternalConstants.aCSS[2, 2] = "#00AA00";
        InternalConstants.aCSS[0, 3] = "B3";
        InternalConstants.aCSS[1, 3] = "C3";
        InternalConstants.aCSS[2, 3] = "#00AAAA";
        InternalConstants.aCSS[0, 4] = "B4";
        InternalConstants.aCSS[1, 4] = "C4";
        InternalConstants.aCSS[2, 4] = "#AA0000";
        InternalConstants.aCSS[0, 5] = "B5";
        InternalConstants.aCSS[1, 5] = "C5";
        InternalConstants.aCSS[2, 5] = "#AA00AA";
        InternalConstants.aCSS[0, 6] = "B6";
        InternalConstants.aCSS[1, 6] = "C6";
        InternalConstants.aCSS[2, 6] = "#AA5500";
        InternalConstants.aCSS[0, 7] = "B7";
        InternalConstants.aCSS[1, 7] = "C7";
        InternalConstants.aCSS[2, 7] = "#AAAAAA";
        InternalConstants.aCSS[1, 8] = "C8";
        InternalConstants.aCSS[2, 8] = "#555555";
        InternalConstants.aCSS[1, 9] = "C9";
        InternalConstants.aCSS[2, 9] = "#5555FF";
        InternalConstants.aCSS[1, 10] = "CA";
        InternalConstants.aCSS[2, 10]= "#55FF55";
        InternalConstants.aCSS[1, 11] = "CB";
        InternalConstants.aCSS[2, 11] = "#55FFFF";
        InternalConstants.aCSS[1, 12] = "CC";
        InternalConstants.aCSS[2, 12] = "#FF5555";
        InternalConstants.aCSS[1, 13] = "CD";
        InternalConstants.aCSS[2, 13] = "#FF55FF";
        InternalConstants.aCSS[1, 14] = "CE";
        InternalConstants.aCSS[2, 14] = "#FFFF55";
        InternalConstants.aCSS[1, 15] = "CF";
        InternalConstants.aCSS[2, 15] = "#FFFFFF";
    
        InternalConstants.aCPLN = Strings.Split(InternalConstants.sCPLN, "|");
        InternalConstants.aCPL = Strings.Split(InternalConstants.sCPL, "|");
        InternalConstants.aCPLISO = Strings.Split(InternalConstants.sCPLISO, "|");
        InternalConstants.aWinCPL = Strings.Split(InternalConstants.sWinCPL, "|");
        InternalConstants.aWinCPLN = Strings.Split(InternalConstants.sWinCPLN, "|");
            InternalConstants.aWinCPLISO = Strings.Split(InternalConstants.sWinCPLISO, "|");

            if (Data.DosFnt80x50 == null) {
            Data.DosFnt80x50 = new MediaSupport.Ansifntdef(8, 8, Color.FromArgb(255, 32, 32, 32), Resources.dosfont80x50c16b2, Resources.dosfontback16c);
            }
            if (Data.DosFnt80x25 == null) {
            Data.DosFnt80x25 = new MediaSupport.Ansifntdef(8, 16, Color.FromArgb(255, 32, 32, 32), Resources.dosfont80x25c16b2, Resources.dosfontback16c);
            }
            Data.WebFontList.Add(new ConverterSupport.WebFontDef("Default", "font-size:16px;", "", "font-family:DOS,monospace;font-size:16px;", "font-size:16px;", "", "font-family:DOS,monospace;font-size:16px;width:8px;height:16px;overflow:hidden;border:none;"));
        Data.WebFontList.Add(new ConverterSupport.WebFontDef("Terminal", "font-size:16px;", "", "font-family:Terminal,monospace;font-size:16px;", "font-size:16px;", "", "font-family:Terminal,monospace;font-size:16px;width:8px;height:16px;overflow:hidden;border:none;"));
        Data.WebFontList.Add(new ConverterSupport.WebFontDef("Lucida Console", "font-size:16px;", "", "font-family:\"Lucida Console\", monospace;font-size:16px;", "font-size:16px;", "", "font-family:\"Lucida Console\",monospace;font-size:16px;width:8px;height:16px;overflow:hidden;border:none;"));
        Data.WebFontList.Add(new ConverterSupport.WebFontDef("FixedSys", "font-size:100%;", "", "font-family:FixedSys,monospace;font-size:100%;", "font-size:100%;", "", "font-family:FixedSys,monospace;font-size:100%;width:8px;height:16px;overflow:hidden;border:none;"));
        Data.WebFontList.Add(new ConverterSupport.WebFontDef("VT-100", "font-size:16px;", "", "font-family:VT-100,monospace;font-size:16px;", "font-size:16px;", "", "font-family:VT-100,monospace;font-size:16px;width:8px;height:16px;overflow:hidden;border:none;"));
        Data.WebFontList.Add(new ConverterSupport.WebFontDef("Monaco", "font-size:100%;", "", "font-family:Monaco,monospace;font-size:100%;", "font-size:100%;", "", "font-family:Monaco,monospace;font-size:100%;width:8px;height:16px;overflow:hidden;border:none;"));
        Data.WebFontList.Add(new ConverterSupport.WebFontDef("Andale Mono", "font-size:100%;", "", "font-family:\"Andale Mono\",monospace;font-size:100%;", "font-size:100%;", "", "font-family:\"Andale Mono\",monospace;font-size:100%;width:8px;height:16px;overflow:hidden;border:none;"));
        Data.WebFontList.Add(new ConverterSupport.WebFontDef("MS Gothic", "font-size:16px;", "", "font-family:\"MS Gothic\",monospace;font-size:16px;", "font-size:16px;", "", "font-family:\"MS Gothic\",monospace;font-size:16px;width:8px;height:16px;overflow:hidden;border:none;"));
        Data.WebFontList.Add(new ConverterSupport.WebFontDef("Courier", "font-size:100%;", "", "font-family:Courier, monospace;font-size:100%;", "font-size:100%;", "", "font-family:Courier,monospace;font-size:100%;width:8px;height:16px;overflow:hidden;border:none;"));
        Data.WebFontList.Add(new ConverterSupport.WebFontDef("Courier New", "font-size:100%;", "", "font-family:\"Courier New\",monospace;font-size:100%;", "font-size:100%;", "", "font-family:\"Courier New\",monospace;font-size:100%;width:8px;height:16px;overflow:hidden;border:none;"));


            Data.sCSSDef = "";
            if (Data.bCUFon == true) {
            Data.sCSSDef = Data.sCSSDef + "DIV.ANSICSS {background-color:#000; color:#C6C6C6;width:%WIDTH%;padding: 5px;}\r\n" + "DIV.ANSICSS PRE {margin:0px;padding:0px;line-height:100%;font-size: 100%;}\r\n";
            } else {
            Data.sCSSDef = "DIV.ANSICSS {background-color:#000;color:#C6C6C6;display:inline-block;width:%WIDTH%;margin:0px;padding:0px;letter-spacing:0px;line-height:16px;%EXTRACSSDIV%}\r\n" + "DIV.ANSICSS PRE {margin:0px;padding:0px;display:block;width:100%;%EXTRACSSPRE%}\r\n" + "DIV.ANSICSS PRE SPAN {padding:0;margin-top:-1px;line-height:16px;letter-spacing:0;display:inline-block;white-space:nowrap;%EXTRACSSSPAN%}\r\n";

                //sCSSDef = sCSSDef & "DIV.ANSICSS {background-color:#000;color:#C6C6C6;width:%WIDTH%;padding:5px;margin:0px;}" & Environment.NewLine & _
                //                    "DIV.ANSICSS PRE {margin:0px;padding:0px;line-height:16px;font-size:16px;letter-spacing:0;width:%WIDTH%;%EXTRACSSPRE%}" & Environment.NewLine & _
                //                    "DIV.ANSICSS PRE SPAN {margin:-1px;* margin:0px;padding:0px;line-height:16px;font-size:16px;letter-spacing:0px;white-space:nowrap;%EXTRACSSSPAN%}" & Environment.NewLine
            }

            for (int x = 0; x <= 15; x++) {
                if (x < 8) {
                Data.sCSSDef = Data.sCSSDef + "DIV.ANSICSS PRE SPAN." + InternalConstants.aCSS[0, x] + " {background-color:" + InternalConstants.aCSS[2, x] + ";}\r\n";
                }
            Data.sCSSDef = Data.sCSSDef + "DIV.ANSICSS PRE SPAN." + InternalConstants.aCSS[1, x] + " {color:" + InternalConstants.aCSS[2, x] + ";}\r\n";
            }
        Data.sCSSDef = Data.sCSSDef + "DIV.ANSICSS PRE SPAN.II{background-color:" + InternalConstants.aCSS[2, 0] + ";color:" + InternalConstants.aCSS[2, 7] + ";}\r\n";

        InternalConstants.sSauceCSS = "";
        InternalConstants.sSauceCSS += "div.sauce,div.saucecomments{margin-top:5px;margin-left:20px;background-color:#000000;border:3px solid #C1C1C1;color:#555555;padding:10px;width:600px;align:right;font-family:dos, monospace;font-size:0.75em;display:inline-block;position:relative;}\r\n" + "div.sauce span.saucelabel{font-weight: bold; width: 170px;text-align: right; line-height: 1.5em;border-bottom: 1px dotted #222266;}" + "div.sauce span.saucedata{width: 400px; position: absolute;right: 5; text-align: left; color: #CCCCCC;border-bottom: 1px dotted #222266;}\r\n" + "div.sauce div.saucetitle span.saucelabel { color: #FFFF55;}\r\n" + "div.sauce div.saucetitle span.saucedata { color: #858500;}\r\n" + "div.sauce div.sauceauthor span.saucelabel { color: #55FF55;}\r\n" + "div.sauce div.sauceauthor span.saucedata { color: #008500;}\r\n" + "div.sauce div.saucegroup span.saucelabel { color: #C1C1C1;}\r\n" + "div.sauce div.saucegroup span.saucedata { color: #555555;}\r\n" + "div.sauce div.saucedatatype span.saucelabel { color: #555555;}\r\n" + "div.sauce div.saucedatatype span.saucedata { color: #444444;}\r\n" + "div.sauce div.saucefiletype span.saucelabel { color: #444444;}\r\n" + "div.sauce div.saucefiletype span.saucedata { color: #333333;}\r\n" + "div.sauce div.saucecreatedate span.saucelabel { color: #5555FF;}\r\n" + "div.sauce div.saucecreatedate span.saucedata { color: #555599;}\r\n" + "div.sauce div.saucetinfo1 span.saucelabel { color: #FFFF55;}\r\n" + "div.sauce div.saucetinfo1 span.saucedata { color: #858500;}\r\n" + "div.sauce div.saucetinfo2 span.saucelabel { color: #FFFF55;}\r\n" + "div.sauce div.saucetinfo2 span.saucedata { color: #858500;}\r\n" + "div.sauce div.saucetinfo3 span.saucelabel { color: #FFFF55;}\r\n" + "div.sauce div.saucetinfo3 span.saucedata { color: #858500;}\r\n" + "div.sauce div.sauceice{color: #FFFFFF;}\r\n";


        }
    }
