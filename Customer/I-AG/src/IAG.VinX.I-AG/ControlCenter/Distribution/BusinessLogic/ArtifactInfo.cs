using IAG.ControlCenter.Distribution.DataLayer.Model;

namespace IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

public struct ArtifactInfo
{
    public string ProductName { get; set; }
    public string ArtifactPath { get; set; }
    public string ArtifactName { get; set; }
    public string Version { get; set; }
    public ProductType ProductType { get; set; }
    public string DependingProductName { get; set; }
}