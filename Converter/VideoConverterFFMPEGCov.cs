using System.Runtime.InteropServices;
//used for APIs
using System;
//System imports
using System.IO;
//File input / output
using System.Text;
//Advanced Text capabilities
//using System.Windows.Forms.Control;
using System.Diagnostics;

namespace MediaSupport
{


	public class VideoConverterFFMPEG : Windows.Forms.Control
	{

		public enum OutTypes
		{
			AVI = 1,
			MP4 = 2,
			WMV = 3,
			MKV = 4,
			VOB = 5,
			FLV = 6,
			GIF = 7,
			MPG = 8
		}
		public enum ConvStates
		{
			Idle = 0,
			WaitingForParams = 1,
			Converting = 2,
			Aborted = 3,
			Finished = 4,
			ConvError = 5,
			Ready = 6
		}
		public System.ComponentModel.BackgroundWorker bw = new System.ComponentModel.BackgroundWorker();
			//FFMPEG Process Object
		private Process prcFFMPEG = new Process();
			//Stores Final Output Location
		private string strFinalOutput;
			//Stores Filename Without Path & Extension
		private string strFWOE;
			//Stores Selected Output File Format
		private string strOutExt;
			//Stores Audio Bitrate
		private string strOutAudio;
			//Stores Video Size
		private string strVidSize;
		private string strOutFile;
		private string strInFile;
		private string strOutType;
		private OutTypes OutType = OutTypes.AVI;
			//Stores Output Quality
		private int intQuality;
		private string strFFMPEGPath;
		private ConvStates intStatus;
		private double dFPS = 29.97;
		private string sVCodec = "";
		private string sImageList = "";
		private int iNumFrames = 0;
		private int iWidth = 0;
		private int iHeight = 0;
		public string VCodec {
			get { return sVCodec; }
			set {
				if (sVCodec != value) {
					sVCodec = value;
				}
			}
		}
		public int VideoWidth {
			get { return iWidth; }
			set {
				if (iWidth != value) {
					iWidth = value;
				}
			}
		}
		public int VideoHeight {
			get { return iHeight; }
			set {
				if (iHeight != value) {
					iHeight = value;
				}
			}
		}
		public string Imagelist {
			get { return sImageList; }
			set {
				if (sImageList != value) {
					sImageList = value;
				}
			}
		}
		public int Numframes {
			get { return iNumFrames; }
			set {
				if (iNumFrames != value) {
					iNumFrames = value;
				}
			}
		}
		public double FPS {
			get { return dFPS; }
			set {
				if (dFPS != value) {
					dFPS = value;
				}
			}
		}
		public string Output {
			get { return strOutFile; }
			set {
				if (intStatus != ConvStates.Converting) {
					if (strOutFile != value) {
						strOutFile = value;
						intStatus = ConvStates.Idle;
						CheckParameter();
					}
				}
			}
		}
		public string Input {
			get { return strInFile; }
			set {
				if (intStatus != ConvStates.Converting) {
					if (strInFile != value) {
						strInFile = value;
						intStatus = ConvStates.Idle;
						CheckParameter();
					}
				}
			}
		}
		public string FFMPEGPath {
			get { return strFFMPEGPath; }
			set {
				if (intStatus != ConvStates.Converting) {
					if (IO.File.Exists(value) & value != strFFMPEGPath) {
						strFFMPEGPath = value;
						CheckParameter();
					}
				}
			}
		}
		public OutTypes OutFormat {
			get { return OutType; }
			set {
				if (intStatus != ConvStates.Converting) {
					if (OutType != value) {
						OutType = value;
					}
				}

			}
		}
		public int Quality {
			get { return intQuality; }
			set { intQuality = value; }
		}
		public ConvStates Status {
			get { return intStatus; }
		}


		public VideoConverterFFMPEG()
		{
			strFFMPEGPath = "C:\\Documents\\VisualStudioProjects\\DirectShowVideoTest\\ffmpeg.exe";
			intStatus = ConvStates.WaitingForParams;
		}

		public VideoConverterFFMPEG(string sourcefile, string destfile, OutTypes outformat, int quality = 0) : this() {
			strInFile = sourcefile;
			strOutFile = destfile;
			OutType = outformat;
			intQuality = quality;
			CheckParameter();
		}
		public void Cancel()
		{
			if (intStatus == ConvStates.Converting) {
				bw.CancelAsync();
				intStatus = ConvStates.Aborted;
				prcFFMPEG.Kill();
				//Done
			}
		}
		public void Convert()
		{
			if (intStatus == ConvStates.Ready | intStatus == ConvStates.Finished | intStatus == ConvStates.Aborted) {
				bw.RunWorkerAsync();
				//Start Conversion
				intStatus = ConvStates.Converting;
			}
		}
		private void CheckParameter()
		{
			if (intStatus == ConvStates.Aborted | intStatus == ConvStates.Finished | intStatus == ConvStates.ConvError) {
				intStatus = ConvStates.Idle;
			}
			if (intStatus == ConvStates.Idle | intStatus == ConvStates.WaitingForParams) {
				if ((strInFile == "" & sImageList == "") | (strInFile != "" & IO.File.Exists(strInFile) == false) | strOutFile == "" | IO.File.Exists(strFFMPEGPath) == false) {
					intStatus = ConvStates.WaitingForParams;
				} else {
					intStatus = ConvStates.Ready;
				}
			}
		}
		private string OutTypeToStr(OutTypes ot)
		{
			switch (ot) {
				case OutTypes.AVI:
					return "avi";
				case OutTypes.FLV:
					return "flv";
				case OutTypes.MKV:
					return "mkv";
				case OutTypes.MP4:
					return "mp4";
				case OutTypes.VOB:
					return "vob";
				case OutTypes.WMV:
					return "wmv";
				case OutTypes.GIF:
					return "gif";
				case OutTypes.MPG:
					return "mpg";
				default:
					return "avi";
			}
		}
		private object ConvertFile()
		{
			//Function To Convert File
			//        Dim cmd As String = " -i """ & input & """ -ar " & strOutAudio & " -b 64k -r 24 -s " & strVidSize & "-qscale " & intQuality & " -y """ & output & """" 'ffmpeg commands -y replace

			Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
			//Disable Illegal Crossthread Calls From Controls

			string strOutput = strOutFile;
			//Output File Name
			string strFFMPEGOut;
			//Lines Read From Input / Source File
			strOutExt = OutTypeToStr(OutFormat);
			string strSource = strInFile;

			//ffmpeg -y -f image2 -i "C:\Users\ccumbrowski\AppData\Local\Temp\ANSIToVideoTemp\ACID-TR.ANS_%05d.PNG"  -r 30 -vframes 373 -map 0:0 -pix_fmt rgb24  -s 640x400 "F:\ANSI\ANI\ACID-TR.ANS.GIF"
			string strFFMPEGCmd = "";
			strFFMPEGCmd += " -y";
			if (OutType != OutTypes.GIF) {
				strFFMPEGCmd += " -b:v 64k";
			}
			if (sImageList != "") {
				strFFMPEGCmd += " -f image2 -i \"" + sImageList + "\" ";
			} else {
				strFFMPEGCmd += " -i \"" + strSource + "\" ";
			}
			if (dFPS != 0) {
				strFFMPEGCmd += " -r " + dFPS;
				if (OutType != OutTypes.GIF) {
					strFFMPEGCmd += " -force_fps";
				}
			}
			if (iNumFrames != 0) {
				if (OutType == OutTypes.GIF) {
					strFFMPEGCmd += " -vframes " + iNumFrames + " -map 0:0 -pix_fmt rgb24";
				} else {
					strFFMPEGCmd += " -frames " + iNumFrames;
				}

			}
			if (iHeight != 0 & iWidth != 0) {
				strFFMPEGCmd += " -s " + iWidth + "x" + iHeight;
			}
			if (sVCodec != "") {
				strFFMPEGCmd += " -vcodec " + LCase(sVCodec);
			}
			strFFMPEGCmd += " \"" + strOutput + "\"";
			Console.WriteLine("FFMPEG Parameters: " + strFFMPEGCmd);
			System.Diagnostics.ProcessStartInfo psiProcInfo = new System.Diagnostics.ProcessStartInfo();
			//Proc Info Object For FFMPEG.EXE

			StreamReader srFFMPEG;
			//Reads Source File's Lines

			//        Dim cmd As String = " -i """ & input & """ -ar " & strOutAudio & " -b 64k -r 24 -s " & strVidSize & "-qscale " & intQuality & " -y """ & output & """" 'ffmpeg commands -y replace


			if (strFinalOutput != "" & strFWOE != "" & strOutExt != "" & strOutAudio != "" & strVidSize != "") {
				strOutput = strFinalOutput + strFWOE + strOutExt;


			} else {
				//MessageBox.Show("Ensure all settings are properly made!") 'If Something Not Set

				//Exit Function

			}


			psiProcInfo.FileName = strFFMPEGPath;
			//Location Of FFMPEG.EXE
			psiProcInfo.Arguments = strFFMPEGCmd;
			//Command String
			psiProcInfo.UseShellExecute = false;

			psiProcInfo.WindowStyle = ProcessWindowStyle.Hidden;

			psiProcInfo.RedirectStandardError = true;
			psiProcInfo.RedirectStandardOutput = true;
			psiProcInfo.CreateNoWindow = true;

			prcFFMPEG.StartInfo = psiProcInfo;

			prcFFMPEG.Start();
			//Start Process
			//

			srFFMPEG = prcFFMPEG.StandardError;
			//Enable Error Checking For FFMPEG.EXE

			//Me.btnStart.Enabled = False


			do {
				//Cancelled?
				if (bw.CancellationPending) {
					return 1;
					return;

				}

				strFFMPEGOut = srFFMPEG.ReadLine;
				//Read Source File Line By Line

			} while (!(prcFFMPEG.HasExited & strFFMPEGOut == null | strFFMPEGOut == ""));
			//Read Until There Is Nothing Left

			intStatus = ConvStates.Finished;
			return 0;

		}


		// ERROR: Handles clauses are not supported in C#
	private void bgwConvert_DoWork(System.Object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			ConvertFile();
			//Function / Sub That Must Start

		}

	}
}