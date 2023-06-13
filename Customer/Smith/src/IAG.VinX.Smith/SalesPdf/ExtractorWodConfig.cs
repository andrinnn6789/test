namespace IAG.VinX.Smith.SalesPdf;

public class ExtractorWodConfig
{
    public string WineInfo { get; set; } = "i-ag:vinx:smithsmith:verkaufsmappe";
    public string Producer { get; set; } = "i-ag:vinx:smithsmith:verkaufsmappe";
    public string ExportRoot { get; set; } = "..\\Verkaufsmappe\\Sortiment";
    public int MaxThreads { get; set; } = 4;
    public bool WithXml { get; set; } = false;
}