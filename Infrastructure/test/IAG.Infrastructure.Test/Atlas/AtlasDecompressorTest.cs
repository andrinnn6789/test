using System.Diagnostics;
using System.IO;

using IAG.Infrastructure.Atlas;

using Xunit;

namespace IAG.Infrastructure.Test.Atlas;

[Collection("UseCurrentDirectory")]
public class AtlasDeCompressorTest
{
    [Fact]
    public void DeCompressTest()
    {
        var deCompressor = new AtlasDeCompressor();
        var compressed = File.ReadAllBytes(Path.Combine("Atlas", "AtlasCompressed"));
        var deCompressed = deCompressor.DeCompress(compressed);
        Assert.True(deCompressed.Length > compressed.Length);
        if (Debugger.IsAttached)
            File.WriteAllBytes("deCompressed.pdf", deCompressed);
    }

    [Fact]
    public void DeCompressUncompressedTest()
    {
        var deCompressor = new AtlasDeCompressor();
        var compressed = File.ReadAllBytes(Path.Combine("Atlas", "AtlasUnCompressed"));
        var deCompressed = deCompressor.DeCompress(compressed);
        Assert.True(deCompressed.Length == compressed.Length);
    }
}