using System.Drawing.Imaging;
using System.Runtime.InteropServices;
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
	public class Avi
	{

		public const int StreamtypeVIDEO = 1935960438;
		public const int OF_SHARE_DENY_WRITE = 32;

		public const int BMP_MAGIC_COOKIE = 19778;


		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct RECTstruc
		{
			public UInt32 left;
			public UInt32 top;
			public UInt32 right;
			public UInt32 bottom;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct BITMAPINFOHEADERstruc
		{
			public UInt32 biSize;
			public Int32 biWidth;
			public Int32 biHeight;
			public Int16 biPlanes;
			public Int16 biBitCount;
			public UInt32 biCompression;
			public UInt32 biSizeImage;
			public Int32 biXPelsPerMeter;
			public Int32 biYPelsPerMeter;
			public UInt32 biClrUsed;
			public UInt32 biClrImportant;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct AVISTREAMINFOstruc
		{
			public UInt32 fccType;
			public UInt32 fccHandler;
			public UInt32 dwFlags;
			public UInt32 dwCaps;
			public UInt16 wPriority;
			public UInt16 wLanguage;
			public UInt32 dwScale;
			public UInt32 dwRate;
			public UInt32 dwStart;
			public UInt32 dwLength;
			public UInt32 dwInitialFrames;
			public UInt32 dwSuggestedBufferSize;
			public UInt32 dwQuality;
			public UInt32 dwSampleSize;
			public RECTstruc rcFrame;
			public UInt32 dwEditCount;
			public UInt32 dwFormatChangeCount;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public UInt16[] szName;
		}



		//Initialize the AVI library
		[DllImport("avifil32.dll")]
		public static void AVIFileInit()
		{
		}

		//Open an AVI file
		[DllImport("avifil32.dll", PreserveSig = true)]
		public static int AVIFileOpen(ref int ppfile, String szFile, int uMode, int pclsidHandler)
		{
		}

		//Create a new stream in an open AVI file
		[DllImport("avifil32.dll")]
		public static int AVIFileCreateStream(int pfile, ref IntPtr ppavi, ref AVISTREAMINFOstruc ptr_streaminfo)
		{
		}

		//Set the format for a new stream
		[DllImport("avifil32.dll")]
		public static int AVIStreamSetFormat(IntPtr aviStream, Int32 lPos, ref BITMAPINFOHEADERstruc lpFormat, Int32 cbFormat)
		{
		}

		//Write a sample to a stream
		[DllImport("avifil32.dll")]
		public static int AVIStreamWrite(IntPtr aviStream, Int32 lStart, Int32 lSamples, IntPtr lpBuffer, Int32 cbBuffer, Int32 dwFlags, Int32 dummy1, Int32 dummy2)
		{
		}

		//Release an open AVI stream
		[DllImport("avifil32.dll")]
		public static int AVIStreamRelease(IntPtr aviStream)
		{
		}

		//Release an open AVI file
		[DllImport("avifil32.dll")]
		public static int AVIFileRelease(int pfile)
		{
		}

		//Close the AVI library
		[DllImport("avifil32.dll")]
		public static void AVIFileExit()
		{
		}

	}
}