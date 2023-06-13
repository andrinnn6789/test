using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.VinX.Smith.HelloTess.Dto;

namespace IAG.VinX.Smith.HelloTess.VinX;

public class VxArticleClient : IVinXClient<VxArticle>
{
    private readonly ISybaseConnection _connection;
    private int _priceGroupId;
    private int _customerId;

    public VxArticleClient(ISybaseConnection connection)
    {
        _connection = connection;
    }

    public void SetFilter(int priceGroupId, int customerId)
    {
        _customerId = customerId;
        _priceGroupId = priceGroupId;
    }

    public IEnumerable<VxArticle> Get()
    {
        return _connection.QueryBySql<VxArticle>(VxArticle.Select, _priceGroupId, _customerId).ToList();
    }
}