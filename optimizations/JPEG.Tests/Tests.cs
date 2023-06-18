using System.Drawing;
using System.Drawing.Imaging;
using FluentAssertions;
using NUnit.Framework;

namespace JPEG.Tests;

[TestFixture]
internal class Tests
{
    private const int CompressionQuality = 70;
    private const string FileName = @"C:\Users\Алексей\source\repos\shpora-debug-optimizations\optimizations\JPEG.Tests\original.bmp";
    private const string ExpectedCompressedPath =
        @"C:\Users\Алексей\source\repos\shpora-debug-optimizations\optimizations\JPEG.Tests\compressed";
    private const string ActualCompressedPath =
        @"C:\Users\Алексей\source\repos\shpora-debug-optimizations\optimizations\JPEG.Tests\compressed.actual";
    private const string ExpectedDecompressedPath =
        @"C:\Users\Алексей\source\repos\shpora-debug-optimizations\optimizations\JPEG.Tests\decompressed.bmp";
    private const string ActualDecompressedPath =
        @"C:\Users\Алексей\source\repos\shpora-debug-optimizations\optimizations\JPEG.Tests\decompressed.actual.bmp";

    [TearDown]
    public void TearDown()
    {
        File.Delete(ActualCompressedPath);
        File.Delete(ActualDecompressedPath);
    }
        
    [Test]
    public void IsCorrectCompress()
    {
        var compressor = new Compressor();

        using var fileStream = File.OpenRead(FileName);
        using var bitmap = (Bitmap) Image.FromStream(fileStream, false, false);
        var imageMatrix = (Matrix) bitmap;

        var compressionResult = compressor.Compress(imageMatrix, CompressionQuality);
        compressionResult.Save(ActualCompressedPath);

        var expectedImage = File.ReadAllBytes(ExpectedCompressedPath);
        var actualImage = File.ReadAllBytes(ActualCompressedPath);

        actualImage.Should().BeEquivalentTo(expectedImage);
    }
        
    [Test]
    public void IsCorrectDecompress()
    {
        var decompressor = new Decompressor();
            
        var compressedImage = CompressedImage.Load(ExpectedCompressedPath);
        var uncompressedImage = decompressor.Decompress(compressedImage);
        var resultBmp = (Bitmap) uncompressedImage;
        resultBmp.Save(ActualDecompressedPath, ImageFormat.Bmp);
            
        var expectedImage = File.ReadAllBytes(ExpectedDecompressedPath);
        var actualImage = File.ReadAllBytes(ActualDecompressedPath);

        actualImage.Should().BeEquivalentTo(expectedImage);
    }
}