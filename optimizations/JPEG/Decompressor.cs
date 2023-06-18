using System.IO;
using JPEG.Images;

namespace JPEG
{
    public class Decompressor
    {
        public Matrix Decompress(CompressedImage image)
        {
            var result = new Matrix(image.Height, image.Width);
            using (var allQuantizedBytes =
                   new MemoryStream(HuffmanCodec.Decode(image.CompressedBytes, image.DecodeTable, image.BitsCount)))
            {
                for (var y = 0; y < image.Height; y += MatrixOperations.DCTSize)
                {
                    for (var x = 0; x < image.Width; x += MatrixOperations.DCTSize)
                    {
                        var _y = new double[MatrixOperations.DCTSize, MatrixOperations.DCTSize];
                        var cb = new double[MatrixOperations.DCTSize, MatrixOperations.DCTSize];
                        var cr = new double[MatrixOperations.DCTSize, MatrixOperations.DCTSize];
                        foreach (var channel in new []{_y, cb, cr})
                        {
                            var quantizedBytes = new byte[MatrixOperations.DCTSize * MatrixOperations.DCTSize];
                            allQuantizedBytes.ReadAsync(quantizedBytes, 0, quantizedBytes.Length).Wait();
                            var quantizedFreqs = MatrixOperations.ZigZagUnScan(quantizedBytes);
                            var channelFreqs = MatrixOperations.DeQuantize(quantizedFreqs, image.Quality);
                            DCT.IDCT2D(channelFreqs, channel);
                            MatrixOperations.ShiftMatrixValues(channel, 128);
                        }

                        MatrixOperations.SetPixels(result, _y, cb, cr, PixelFormat.YCbCr, y, x);
                    }
                }
            }

            return result;
        }
    }
}