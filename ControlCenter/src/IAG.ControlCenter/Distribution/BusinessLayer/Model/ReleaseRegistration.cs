
namespace IAG.ControlCenter.Distribution.BusinessLayer.Model;

public class ReleaseRegistration
{
    public string ReleaseVersion { get; set; }
    public string Platform { get; set; }
    public string Description { get; set; }
    public string ReleasePath { get; set; }
    public string ArtifactPath { get; set; }
}