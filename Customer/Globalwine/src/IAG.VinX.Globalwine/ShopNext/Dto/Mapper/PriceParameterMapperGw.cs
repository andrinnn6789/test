using System.Collections.Generic;

using IAG.Infrastructure.ObjectMapper;
using IAG.VinX.Basket.Dto;
using IAG.VinX.Globalwine.ShopNext.Dto.Rest;

namespace IAG.VinX.Globalwine.ShopNext.Dto.Mapper;

public class PriceParameterMapperFromGw: ObjectMapper<PriceParameterGw, PriceParameter>
{
    protected override PriceParameter MapToDestination(PriceParameterGw source, PriceParameter destination)
    {
        destination.AddressId = source.AddressId;
        destination.ValidDate = source.ValidDate;
        destination.PriceGroupId = source.PriceGroupId;
        destination.Division = source.Division;
        destination.ArticleParameters = new List<ArticleParameter>();
        var artMapper = new ArticleParameterMapperFromGw();
        foreach (var articleParameterShop in source.ArticleParameters)
        {
            destination.ArticleParameters.Add(artMapper.NewDestination(articleParameterShop));
        }

        return destination;
    }
}

public class ArticleParameterMapperFromGw: ObjectMapper<ArticleParameterGw, ArticleParameter>
{
    protected override ArticleParameter MapToDestination(ArticleParameterGw source, ArticleParameter destination)
    {
        destination.ArticleId = source.ArticleId;
        destination.Quantity = source.Quantity;

        return destination;
    }
}