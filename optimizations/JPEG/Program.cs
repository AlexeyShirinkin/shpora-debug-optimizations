using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using JPEG.Images;

namespace JPEG
{
	internal static class Program
	{
		private const int CompressionQuality = 70;
		private const string FileName = @"sample.bmp";
		private static readonly string CompressedFileName = $"{FileName}.compressed.{CompressionQuality}";
		private static readonly string UncompressedFileName = $"{FileName}.uncompressed.new.{CompressionQuality}.bmp";

		internal static void Main(string[] args)
		{
			var compressor = new Compressor();
			var decompressor = new Decompressor();

			Console.WriteLine(IntPtr.Size == 8 ? "64-bit version" : "32-bit version");

			var stopwatch = Stopwatch.StartNew();
			using (var fileStream = File.OpenRead(FileName))
			using (var bitmap = (Bitmap)Image.FromStream(fileStream, false, false))
			{
				var imageMatrix = (Matrix)bitmap;

				stopwatch.Stop();
				Console.WriteLine($"{bitmap.Width}x{bitmap.Height} - {fileStream.Length / (1024.0 * 1024):F2} MB");
				stopwatch.Start();

				var compressionResult = compressor.Compress(imageMatrix, CompressionQuality);
				compressionResult.Save(CompressedFileName);
			}

			stopwatch.Stop();
			Console.WriteLine("Compression: " + stopwatch.Elapsed);

			stopwatch.Restart();
			var compressedImage = CompressedImage.Load(CompressedFileName);
			var uncompressedImage = decompressor.Decompress(compressedImage);
			var resultBitmap = (Bitmap)uncompressedImage;
			stopwatch.Stop();

			resultBitmap.Save(UncompressedFileName, ImageFormat.Bmp);
			Console.WriteLine("Decompression: " + stopwatch.Elapsed);
			Console.WriteLine($"Peak commit size: {MemoryMeter.PeakPrivateBytes() / (1024.0 * 1024):F2} MB");
			Console.WriteLine($"Peak working set: {MemoryMeter.PeakWorkingSet() / (1024.0 * 1024):F2} MB");
		}
	}
}
