using System.Drawing;
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
namespace MediaSupport
{


	public class AviWriter
	{
		private int aviFile = 0;
		private IntPtr aviStream = IntPtr.Zero;
		private UInt32 frameRate = 0;
		private int countFrames = 0;
		private int width = 0;
		private int height = 0;
		private UInt32 stride = 0;
		private UInt32 fccType = MediaFormats.Avi.StreamtypeVIDEO;

		private UInt32 fccHandler = 1668707181;
		private int strideInt;
		private uint strideU;
		private uint heightU;

		private uint widthU;
		public void OpenAVI(string fileName, UInt32 frameRate, int w = 0, int h = 0)
		{
			this.frameRate = frameRate;

			MediaFormats.Avi.AVIFileInit();

			if (w != 0) {
				this.width = w;
			}
			if (h != 0) {
				this.height = h;
			}
			int OpeningError = MediaFormats.Avi.AVIFileOpen(aviFile, fileName, 4097, 0);
			if (OpeningError != 0) {
				throw new Exception("Error in AVIFileOpen: " + OpeningError.ToString());
			}
		}
		public void AddFrame(Bitmap bmp_)
		{
			Graphics gr;
			Bitmap bmp;
			if (this.width == 0) {
				this.width = bmp_.Width;
			}
			if (this.height == 0) {
				this.height = bmp_.Height;
			}

			bmp = new Bitmap(this.width, this.height, PixelFormat.Format24bppRgb);
			gr = Graphics.FromImage(bmp);
			gr.DrawImage(bmp_, new Point(0, 0));

			bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
			if (countFrames == 0) {
				uint bmpDatStride = bmpData.Stride;
				this.stride = (UInt32)bmpDatStride;
				//Me.width = bmp.Width
				//Me.height = bmp.Height
				CreateStream();
			}

			strideInt = stride;
			int writeResult = MediaFormats.Avi.AVIStreamWrite(aviStream, countFrames, 1, bmpData.Scan0, (Int32)(strideInt * height), 0, 0, 0);

			if (writeResult != 0) {
				throw new Exception("Error in AVIStreamWrite: " + writeResult.ToString());
			}
			bmp.UnlockBits(bmpData);
			System.Math.Max(System.Threading.Interlocked.Increment(countFrames), countFrames - 1);

		}
		private void CreateStream()
		{
			MediaFormats.Avi.AVISTREAMINFOstruc strhdr = new MediaFormats.Avi.AVISTREAMINFOstruc();
			strhdr.fccType = fccType;
			strhdr.fccHandler = fccHandler;
			strhdr.dwScale = 1;
			strhdr.dwRate = frameRate;
			strideU = stride;
			heightU = height;
			strhdr.dwSuggestedBufferSize = (UInt32)(stride * strideU);
			strhdr.dwQuality = 10000;

			heightU = height;
			widthU = width;
			strhdr.rcFrame.bottom = (UInt32)heightU;
			strhdr.rcFrame.right = (UInt32)widthU;
			strhdr.szName = new UInt16[64] {
				
			};
			int createResult = MediaFormats.Avi.AVIFileCreateStream(aviFile, aviStream, strhdr);
			if (createResult != 0) {
				throw new Exception("Error in AVIFileCreateStream: " + createResult.ToString());
			}
			MediaFormats.Avi.BITMAPINFOHEADERstruc bi = new MediaFormats.Avi.BITMAPINFOHEADERstruc();
			uint bisize = Marshal.SizeOf(bi);
			bi.biSize = (UInt32)bisize;
			bi.biWidth = (Int32)width;
			bi.biHeight = (Int32)height;
			bi.biPlanes = 1;
			bi.biBitCount = 24;

			strideU = stride;
			heightU = height;
			bi.biSizeImage = (UInt32)(strideU * heightU);
			int formatResult = MediaFormats.Avi.AVIStreamSetFormat(aviStream, 0, bi, Marshal.SizeOf(bi));
			if (formatResult != 0) {
				throw new Exception("Error in AVIStreamSetFormat: " + formatResult.ToString());
			}
		}
		public void Close()
		{
			if (aviStream != IntPtr.Zero) {
				MediaFormats.Avi.AVIStreamRelease(aviStream);
				aviStream = IntPtr.Zero;
			}
			if (aviFile != 0) {
				MediaFormats.Avi.AVIFileRelease(aviFile);
				aviFile = 0;
			}
			MediaFormats.Avi.AVIFileExit();
		}

		private void CreateFile()
		{
			AviWriter Writer = new AviWriter();
			Bitmap[] screenBimaps = new Bitmap[99];
			int currentBitmap = 0;
			Writer.OpenAVI("C:\\Test.Avi", 10);
			for (int Frame = 0; Frame <= currentBitmap - 1; Frame++) {
				Writer.AddFrame(screenBimaps(Frame));
			}
			Writer.Close();
		}
	}
}