using System;

namespace IAG.InstallClient.BusinessLogic.Model;

public class InstallationSetup
{
    public string InstanceName { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProductId { get; set; }
    public Guid ReleaseId { get; set; }
    public Guid? CustomerExtensionReleaseId { get; set; }
    public Guid? ConfigurationProductId { get; set; }
    public Guid? ConfigurationReleaseId { get; set; }
}