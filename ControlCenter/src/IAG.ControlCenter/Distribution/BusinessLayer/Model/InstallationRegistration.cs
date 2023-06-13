using System;

namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class InstallationRegistration
{
    public Guid? ProductId { get; set; }
    public string ReleaseVersion { get; set; }
    public string Platform { get; set; }
    public string InstanceName { get; set; }
    public string Description { get; set; }
}