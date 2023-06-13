using System;

namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class ReleaseInfo
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ReleaseVersion { get; set; }
    public string Platform { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string Description { get; set; }
    public string ReleasePath { get; set; }
    public string ArtifactPath { get; set; }
}