namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public interface IFileCompareInfo
{
    string Name { get; }
    string FileVersion { get; }
    string ProductVersion { get; }
    byte[] Checksum { get; }
}