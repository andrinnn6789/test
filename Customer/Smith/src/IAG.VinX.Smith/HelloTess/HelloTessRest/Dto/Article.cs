using IAG.VinX.Smith.HelloTess.SyncLogic;

namespace IAG.VinX.Smith.HelloTess.HelloTessRest.Dto;

public class Article : ISyncableSource
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Barcode { get; set; }
        
    public long? Plu { get; set; }

    public string Description { get; set; }

    public int Type { get; set; }

    public string ArticleGroupId { get; set; }

    public string Price { get; set; }

    public string TaxInhouse { get; set; }

    public int? TaxTakeaway { get; set; }

    public bool InheritTaxrate { get; set; }

    public bool InheritColors { get; set; }

    public bool ShowInMenu { get; set; }

    public int NetProductionCosts { get; set; }

    public string Key => Id;
}