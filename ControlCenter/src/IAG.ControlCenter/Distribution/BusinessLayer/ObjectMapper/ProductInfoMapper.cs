using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Distribution.DataLayer.Model;
using IAG.Infrastructure.ObjectMapper;

namespace IAG.ControlCenter.Distribution.BusinessLayer.ObjectMapper;

public class ProductInfoMapper : ObjectMapper<Product, ProductInfo>
{
    protected override ProductInfo MapToDestination(Product source, ProductInfo destination)
    {
        destination.Id = source.Id;
        destination.ProductName = source.Name;
        destination.ProductType = source.ProductType;
        destination.DependsOnProductId = source.DependsOnProductId;
        destination.Description = source.Description;

        return destination;
    }
}