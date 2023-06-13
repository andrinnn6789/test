namespace IAG.VinX.Smith.HelloTess.MainSyncConfig;

public class HelloTessSystemConfig
{
    public string Name { get; set; }

    public string Url { get; set; }

    public string ApiKey { get; set; }

    /// <summary>
    /// Kunden-Preisgruppe deren VK-Preise nach NetProductionCosts übermittelt werden sollen.
    /// </summary>
    public int PriceGroupForProdCost { get; set; }

    /// <summary>
    /// Kunde dessen VK-SpezPreise nach NetProductionCosts übermittelt werden sollen. Übersteuert Preisgruppen-Preis
    /// </summary>
    public int CustomerForProdCost { get; set; }
}