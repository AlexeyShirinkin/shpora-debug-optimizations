using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using JPEG.Images;

namespace JPEG
{
	class Program
	{
		const int CompressionQuality = 70;

		static void Main(string[] args)
		{
			var compressor = new Compressor();
			var decompressor = new Decompressor();

			try
			{
				Console.WriteLine(IntPtr.Size == 8 ? "64-bit version" : "32-bit version");
				var sw = Stopwatch.StartNew();
				var fileName = @"sample.bmp";
//				var fileName = "Big_Black_River_Railroad_Bridge.bmp";
				var compressedFileName = fileName + ".compressed." + CompressionQuality;
				var uncompressedFileName = fileName + ".uncompressed." + CompressionQuality + ".bmp";
				
				using (var fileStream = File.OpenRead(fileName))
				using (var bmp = (Bitmap) Image.FromStream(fileStream, false, false))
				{
					var imageMatrix = (Matrix) bmp;

					sw.Stop();
					Console.WriteLine($"{bmp.Width}x{bmp.Height} - {fileStream.Length / (1024.0 * 1024):F2} MB");
					sw.Start();

					var compressionResult = compressor.Compress(imageMatrix, CompressionQuality);
					compressionResult.Save(compressedFileName);
				}

				sw.Stop();
				Console.WriteLine("Compression: " + sw.Elapsed);
				sw.Restart();
				var compressedImage = CompressedImage.Load(compressedFileName);
				var uncompressedImage = decompressor.Decompress(compressedImage);
				var resultBmp = (Bitmap) uncompressedImage;
				resultBmp.Save(uncompressedFileName, ImageFormat.Bmp);
				Console.WriteLine("Decompression: " + sw.Elapsed);
				Console.WriteLine($"Peak commit size: {MemoryMeter.PeakPrivateBytes() / (1024.0*1024):F2} MB");
				Console.WriteLine($"Peak working set: {MemoryMeter.PeakWorkingSet() / (1024.0*1024):F2} MB");
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
