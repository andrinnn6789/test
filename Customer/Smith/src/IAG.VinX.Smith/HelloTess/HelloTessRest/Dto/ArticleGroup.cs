using IAG.VinX.Smith.HelloTess.SyncLogic;

namespace IAG.VinX.Smith.HelloTess.HelloTessRest.Dto;

public class ArticleGroup : ISyncableSource
{
    public string Id { get; set; }

    public string Name { get; set; }

    public int MainArticleGroup { get; set; }

    // ReSharper disable once InconsistentNaming
    public string ExternalPLU { get; set; }

    public int TaxInhouse { get; set; }

    public int TaxTakeaway { get; set; }

    public bool Active { get; set; }

    public string Key => Id;
}