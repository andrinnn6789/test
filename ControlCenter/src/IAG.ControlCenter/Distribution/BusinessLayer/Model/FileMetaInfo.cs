using System;

namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class FileMetaInfo : IFileCompareInfo
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FileVersion { get; set; }
    public string ProductVersion { get; set; }
    public byte[] Checksum { get; set; }
    public DateTime FileLastModifiedDate { get; set; }
}