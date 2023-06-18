using System;
using System.Collections.Generic;

namespace JPEG;

public class Compressor
{
    public CompressedImage Compress(Matrix matrix, int quality = 50)
    {
        var allQuantizedBytes = new List<byte>();

        for(var y = 0; y < matrix.Height; y += MatrixOperations.DCTSize)
        {
            for(var x = 0; x < matrix.Width; x += MatrixOperations.DCTSize)
            {
                foreach (var selector in new Func<Pixel, double>[] {p => p.Y, p => p.Cb, p => p.Cr})
                {
                    var subMatrix = MatrixOperations.GetSubMatrix(matrix, y, MatrixOperations.DCTSize, x, MatrixOperations.DCTSize, selector);
                    MatrixOperations.ShiftMatrixValues(subMatrix, -128);
                    var channelFreqs = DCT.DCT2D(subMatrix);
                    var quantizedFreqs = MatrixOperations.Quantize(channelFreqs, quality);
                    var quantizedBytes = MatrixOperations.ZigZagScan(quantizedFreqs);
                    allQuantizedBytes.AddRange(quantizedBytes);
                }
            }
        }

        long bitsCount;
        Dictionary<BitsWithLength, byte> decodeTable;
        var compressedBytes = HuffmanCodec.Encode(allQuantizedBytes, out decodeTable, out bitsCount);

        return new CompressedImage {Quality = quality, CompressedBytes = compressedBytes, BitsCount = bitsCount, DecodeTable = decodeTable, Height = matrix.Height, Width = matrix.Width};
    }
}