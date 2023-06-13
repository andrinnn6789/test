using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.VinX.Smith.HelloTess.Dto;

namespace IAG.VinX.Smith.HelloTess.VinX;

public class VxArticleCategoryClient : IVinXClient<VxArticleCategory>
{
    private readonly ISybaseConnection _connection;

    public VxArticleCategoryClient(ISybaseConnection connection)
    {
        _connection = connection;
    }

    public IEnumerable<VxArticleCategory> Get()
    {
        return _connection.GetQueryable<VxArticleCategory>().ToList();
    }
}