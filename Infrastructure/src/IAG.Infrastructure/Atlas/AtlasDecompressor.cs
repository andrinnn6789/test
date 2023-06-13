using System;
using System.IO;
using System.Text;

using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace IAG.Infrastructure.Atlas;

public class AtlasDeCompressor
{
    private const int HeaderSize = 12;
    public byte[] DeCompress(byte[] compressedData)
    {
        if (compressedData.Length < HeaderSize || Encoding.ASCII.GetString(compressedData, 0, 4) != "OZL1")
            return compressedData;
        var zlibData = new byte[compressedData.Length - HeaderSize];
        Array.Copy(compressedData, HeaderSize, zlibData, 0, zlibData.Length);
        using var compressedStream = new MemoryStream(zlibData);
        using var decompressedStream = new MemoryStream();
        using var decompressionStream = new InflaterInputStream(compressedStream);
        decompressionStream.CopyTo(decompressedStream);
        return decompressedStream.ToArray();
    }
}