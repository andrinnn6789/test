using System.Collections;
using System.Collections.Generic;
using System.Text.Json;

using IAG.VinX.Basket.Enum;
using IAG.VinX.OrderBravo.Dto;
using IAG.VinX.Zweifel.OrderBravo.BusinessLogic;
using IAG.VinX.Zweifel.OrderBravo.Dto;

using Xunit;

namespace IAG.VinX.Zweifel.IntegrationTest.OrderBravo.BusinessLogic;

public class ObzCatalogMapperTest
{
    [Theory]
    [ClassData(typeof(ObzCatalogMapperTestData))]
    public void ObzCatalogMapper(ObzArticle vinxArticle, ObsCatalogProduct catalogProduct)
    {
        var articleMapper = new ObzCatalogMapper();
        articleMapper.SetGlobalMinimumStockAmount(6);
        var mappedArticle = articleMapper.MapArticle(vinxArticle);

        Assert.Equal(JsonSerializer.Serialize(mappedArticle), JsonSerializer.Serialize(catalogProduct));
    }
}

public class ObzCatalogMapperTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingId = 1,
                FillingDescription = "Filling",
                FillingWebDescription = "FillingWebDescription",
                PackingUnitId = 1,
                PackingUnitDescription = "6er Carton",
                PackingUnitQtyPerUnit = 6,
                PackingUnitBreakageAllowed = false,
                SortimentId = 0,
                CycleId = 0,
                ReplacementArticleNumber = 1,
                StockAmount = 6,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Wine,
                PackingUnitOnly = true,
                CustomerNumbers = null
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "6er Carton",
                        unitCode = "1",
                        measurementUnitQuantity = 6
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "in_stock",
                replacementProducts = new[] { "1" },
                customerNumbers = null,
                category = "Wein",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingDescription = "Filling",
                FillingWebDescription = "FillingWebDescription",
                PackingUnitId = 2,
                PackingUnitDescription = "12er Carton",
                PackingUnitQtyPerUnit = 12,
                PackingUnitBreakageAllowed = true,
                SortimentId = 0,
                CycleId = 4,
                ReplacementArticleNumber = null,
                StockAmount = 6,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Wine,
                PackingUnitOnly = true,
                CustomerNumbers = null
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description 2000 Producer Filling",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "12er Carton",
                        unitCode = "2",
                        measurementUnitQuantity = 12
                    }
                },
                hasLeadTime = true,
                currency = "CHF",
                availability = "in_stock",
                replacementProducts = null,
                customerNumbers = null,
                category = "Wein",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingDescription = "Filling",
                FillingWebDescription = "FillingWebDescription",
                PackingUnitId = 3,
                PackingUnitDescription = "24er Carton",
                PackingUnitQtyPerUnit = 24,
                PackingUnitBreakageAllowed = false,
                SortimentId = 0,
                CycleId = 0,
                ReplacementArticleNumber = null,
                StockAmount = 2,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Spirits,
                PackingUnitOnly = true,
                CustomerNumbers = null
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "24er Carton",
                        unitCode = "3",
                        measurementUnitQuantity = 24
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "in_stock",
                replacementProducts = null,
                customerNumbers = null,
                category = "Spirituosen",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingDescription = "Filling",
                FillingWebDescription = "FillingWebDescription",
                PackingUnitId = 3,
                PackingUnitDescription = "24er Carton",
                PackingUnitQtyPerUnit = 24,
                PackingUnitBreakageAllowed = true,
                SortimentId = 0,
                CycleId = 0,
                ReplacementArticleNumber = null,
                StockAmount = 2,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Spirits,
                PackingUnitOnly = true,
                CustomerNumbers = null
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description 1% Producer Filling",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "24er Carton",
                        unitCode = "3",
                        measurementUnitQuantity = 24
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "in_stock",
                replacementProducts = null,
                customerNumbers = null,
                category = "Spirituosen",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingDescription = "Filling",
                FillingWebDescription = "FillingWebDescription",
                PackingUnitId = 3,
                PackingUnitDescription = "24er Carton",
                PackingUnitQtyPerUnit = 24,
                PackingUnitBreakageAllowed = true,
                SortimentId = 0,
                CycleId = 0,
                ReplacementArticleNumber = null,
                StockAmount = 2,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Mineral,
                PackingUnitOnly = true,
                CustomerNumbers = null
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description FillingWebDescription",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "24er Carton",
                        unitCode = "3",
                        measurementUnitQuantity = 24
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "in_stock",
                replacementProducts = null,
                customerNumbers = null,
                category = "Mineral und Fruchtsäfte",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingDescription = "Filling",
                FillingWebDescription = "FillingWebDescription",
                PackingUnitId = 3,
                PackingUnitDescription = "24er Carton",
                PackingUnitQtyPerUnit = 24,
                PackingUnitBreakageAllowed = true,
                SortimentId = 0,
                CycleId = 0,
                ReplacementArticleNumber = null,
                StockAmount = 2,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Beer,
                PackingUnitOnly = true,
                CustomerNumbers = "1,2,3,4"
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description FillingWebDescription",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "24er Carton",
                        unitCode = "3",
                        measurementUnitQuantity = 24
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "in_stock",
                replacementProducts = null,
                customerNumbers = new List<string>()
                {
                    "1",
                    "2",
                    "3",
                    "4"
                },
                category = "Bier",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingId = 1,
                FillingDescription = "Dose",
                FillingWebDescription = "Dose",
                PackingUnitId = 3,
                PackingUnitDescription = "24er Carton",
                PackingUnitQtyPerUnit = 24,
                PackingUnitBreakageAllowed = true,
                SortimentId = 0,
                CycleId = 0,
                ReplacementArticleNumber = null,
                StockAmount = 2,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Beer,
                PackingUnitOnly = false,
                CustomerNumbers = "1,2,3,4"
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description Dose",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "24er Carton",
                        unitCode = "3",
                        measurementUnitQuantity = 24
                    },
                    new()
                    {
                        unit = "Flasche",
                        unitCode = "1",
                        measurementUnitQuantity = 1
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "in_stock",
                replacementProducts = null,
                customerNumbers = new List<string>()
                {
                    "1",
                    "2",
                    "3",
                    "4"
                },
                category = "Bier",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingDescription = "Filling",
                FillingWebDescription = "FillingWebDescription",
                PackingUnitDescription = null,
                PackingUnitBreakageAllowed = false,
                SortimentId = 0,
                CycleId = 112,
                ReplacementArticleNumber = null,
                StockAmount = 1,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Beer,
                PackingUnitOnly = false,
                CustomerNumbers = "1,2,3,4"
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "Bezeichnung nicht erfasst",
                        unitCode = "",
                        measurementUnitQuantity = 0
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "discontinued",
                replacementProducts = null,
                customerNumbers = new List<string>()
                {
                    "1",
                    "2",
                    "3",
                    "4"
                },
                category = "Bier",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingId = 1,
                FillingDescription = "Dose",
                FillingWebDescription = "Dose",
                PackingUnitDescription = null,
                PackingUnitBreakageAllowed = true,
                SortimentId = 0,
                CycleId = 112,
                ReplacementArticleNumber = null,
                StockAmount = 1,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Beer,
                PackingUnitOnly = false,
                CustomerNumbers = "1,2,3,4"
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description Dose",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "Flasche",
                        unitCode = "1",
                        measurementUnitQuantity = 1
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "discontinued",
                replacementProducts = null,
                customerNumbers = new List<string>()
                {
                    "1",
                    "2",
                    "3",
                    "4"
                },
                category = "Bier",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingId = 1,
                FillingDescription = "Dose",
                FillingWebDescription = "Dose",
                PackingUnitId = 7,
                PackingUnitDescription = "Tank",
                PackingUnitBreakageAllowed = false,
                PackingUnitQtyPerUnit = 1,
                SortimentId = 0,
                CycleId = 112,
                ReplacementArticleNumber = null,
                StockAmount = 1,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Beer,
                PackingUnitOnly = false,
                CustomerNumbers = "1,2,3,4"
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description Tank",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "Tank",
                        unitCode = "7",
                        measurementUnitQuantity = 1
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "discontinued",
                replacementProducts = null,
                customerNumbers = new List<string>()
                {
                    "1",
                    "2",
                    "3",
                    "4"
                },
                category = "Bier",
                subcategory = "Category"
            }
        };
        yield return new object[]
        {
            new ObzArticle
            {
                ArticleNumber = 1234,
                ArticleDescription = "Description",
                Year = 2000,
                Volume = 1,
                ProducerName = "Producer",
                FillingDescription = "Filling",
                FillingWebDescription = "FillingWebDescription",
                PackingUnitDescription = null,
                PackingUnitBreakageAllowed = true,
                SortimentId = 0,
                CycleId = 112,
                ReplacementArticleNumber = null,
                StockAmount = 1,
                ArticleCategory = "Category",
                ArticleType = ArticleCategoryKind.Set,
                PackingUnitOnly = false,
                CustomerNumbers = "1,2,3,4"
            },
            new ObsCatalogProduct
            {
                productCode = "1234",
                name = "Description",
                units = new List<ObsUnit>
                {
                    new()
                    {
                        unit = "Bezeichnung nicht erfasst",
                        unitCode = "",
                        measurementUnitQuantity = 0
                    }
                },
                hasLeadTime = false,
                currency = "CHF",
                availability = "discontinued",
                replacementProducts = null,
                customerNumbers = new List<string>()
                {
                    "1",
                    "2",
                    "3",
                    "4"
                },
                category = "Verkaufs-Set",
                subcategory = "Category"
            }
        };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}