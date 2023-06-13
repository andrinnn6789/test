using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Distribution.DataLayer.Model;

[ExcludeFromCodeCoverage]
public class FileStore : BaseEntityWithTenant, IFileCompareInfo
{
    public string Name { get; set; }
    public byte[] Data { get; set; }
    public string FileVersion { get; set; }
    public string ProductVersion { get; set; }
    public byte[] Checksum { get; set; }
    public DateTime FileLastModifiedDate { get; set; }

    public ICollection<ReleaseFileStore> ReleaseFileStores { set; get; }
}