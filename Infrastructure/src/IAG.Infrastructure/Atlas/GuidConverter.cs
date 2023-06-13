using System;

namespace IAG.Infrastructure.Atlas;

public class GuidConverter
{
    public Guid ToBigEndianGuid(byte[] bytes)
    {
        var bigEndianBytes = BitConverter.IsLittleEndian ? FlipGuidEndian(bytes) : bytes;

        return new Guid(bigEndianBytes);
    }

    public Guid ToBigEndianGuid(string base64)
    {
        var decodedBytes = Convert.FromBase64String(base64);

        return ToBigEndianGuid(decodedBytes);
    }

    public Guid ToMixedEndianGuid(byte[] bytes)
    {
        var mixedEndianBytes = BitConverter.IsLittleEndian ? bytes : FlipGuidEndian(bytes);

        return new Guid(mixedEndianBytes);
    }

    public Guid ToMixedEndianGuid(string base64)
    {
        var decodedBytes = Convert.FromBase64String(base64);

        return ToMixedEndianGuid(decodedBytes);
    }

    public byte[] FlipGuidEndian(byte[] oldBytes)
    {
        var newBytes = new byte[16];

        for (var i = 8; i < 16; i++)
            newBytes[i] = oldBytes[i];

        newBytes[3] = oldBytes[0];
        newBytes[2] = oldBytes[1];
        newBytes[1] = oldBytes[2];
        newBytes[0] = oldBytes[3];
        newBytes[5] = oldBytes[4];
        newBytes[4] = oldBytes[5];
        newBytes[6] = oldBytes[7];
        newBytes[7] = oldBytes[6];

        return newBytes;
    }
}