using IAG.Infrastructure.Atlas;
using IAG.VinX.Smith.HelloTess.Dto;
using IAG.VinX.Smith.HelloTess.HelloTessRest.Dto;
using IAG.VinX.Smith.HelloTess.MainSyncConfig;

namespace IAG.VinX.Smith.HelloTess.DataMapper;

class ArticleGroupDataMapper : IDataMapper<VxArticleCategory, ArticleGroup>
{
    private readonly SyncSystemDefaults _syncSystemDefaults;

    public ArticleGroupDataMapper(SyncSystemDefaults syncSystemDefaults)
    {
        _syncSystemDefaults = syncSystemDefaults;
    }

    public bool CheckUpdate(VxArticleCategory source, ArticleGroup target)
    {
        var bezeichnung = _syncSystemDefaults.ArticleGroupNamePrefix + source.Bezeichnung;

        // check if data has changed
        if (target.Name == bezeichnung) return false;

        target.Name = bezeichnung;
        return true;
    }

    public ArticleGroup CreateTarget(VxArticleCategory source)
    {
        return new()
        {
            Id = new GuidConverter().ToBigEndianGuid(source.Guid).ToString(),
            Name = _syncSystemDefaults.ArticleGroupNamePrefix + source.Bezeichnung,
            MainArticleGroup = 1,
            ExternalPLU = _syncSystemDefaults.ArticleGroupExternalPlu,
            TaxInhouse = _syncSystemDefaults.ArticleGroupTaxrate,
            TaxTakeaway = _syncSystemDefaults.ArticleGroupTaxrate,
            Active = true
        };
    }

    public bool CheckDelete(ArticleGroup target)
    {
        if (!target.Active) return false;
        // only vinx-articlegroups has specific externalPLU, others are ignored
        if (target.ExternalPLU != _syncSystemDefaults.ArticleGroupExternalPlu) return false;

        target.Active = false;
        return true;
    }
}