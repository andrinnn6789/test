using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Map;

[ExcludeFromCodeCoverage]
public static class MappingHelper
{
    public static IEnumerable<Type> GetMappings()
    {
        return new List<Type>
        {
            typeof(AddressMap),
            typeof(ArticleCategoryMap),
            typeof(ArticleECommerceGroupMap),
            typeof(ArticleMap),
            typeof(BulkPackageMap),
            typeof(CountryMap),
            typeof(CycleMap),
            typeof(ErrorLogMap),
            typeof(FillingMap),
            typeof(GrapeMap),
            typeof(OnlineOrderMap),
            typeof(OnlineOrderLineMap),
            typeof(PriceGroupMap),
            typeof(RecommendationMap),
            typeof(RegionMap),
            typeof(SalesPriceMap),
            typeof(ServiceProviderMap),
            typeof(SpecialPriceMap),
            typeof(StockMap),
            typeof(StockMovementMap),
            typeof(VatMap),
            typeof(WarehouseMap),
            typeof(WineInfoMap)
        };
    }
}
