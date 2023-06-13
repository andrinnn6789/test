using System;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Distribution.DataLayer.Model;

[ExcludeFromCodeCoverage]
public class ReleaseFileStore : BaseEntityWithTenant
{
    public Guid FileStoreId { get; set; }
    public Guid ReleaseId { get; set; }

    public FileStore FileStore { get; set; }
}