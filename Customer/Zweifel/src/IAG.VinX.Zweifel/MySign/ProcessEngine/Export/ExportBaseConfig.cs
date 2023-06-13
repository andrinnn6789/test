namespace IAG.VinX.Zweifel.MySign.ProcessEngine.Export;

public class ExportBaseConfig
{
    public string ConnectionString { get; set; } = "$$sybaseConnection$";

    public string ExportFolder { get; set; } = "$$exchangeRoot$\\Export\\Produkte";
}