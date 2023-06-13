using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

namespace IAG.ControlCenter.Distribution.DataLayer.Model;

[ExcludeFromCodeCoverage]
public class Release : BaseEntityWithTenant
{
    public Guid ProductId { get; set; }
    public string ReleaseVersion { get; set; }
    public string Platform { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Description { get; set; }
    public string ReleasePath { get; set; }
    public string ArtifactPath { get; set; }

    public Product Product { get; set; }
    public ICollection<ReleaseFileStore> ReleaseFileStores { set; get; }
}