using System;

namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class FileRegistration : IFileCompareInfo
{
    public string Name { get; set; }
    public string FileVersion { get; set; }
    public string ProductVersion { get; set; }
    public byte[] Checksum { get; set; }
    public DateTime FileLastModifiedDate { get; set; }
}