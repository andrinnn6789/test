using System.Collections.Generic;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.VinX.Greiner.EslManager.BusinessLogic;
using IAG.VinX.Greiner.EslManager.Dto;

using Moq;

using Xunit;

namespace IAG.VinX.Greiner.IntegrationTest.EslManager.BusinessLogic;

public class DataExtractorTest
{
    [Fact]
    public void ExtractArticlesWithGtinsEmptyTest()
    {
        var connectionMock = new Mock<ISybaseConnection>();
        var extractor = new DataExtractor(connectionMock.Object);
        Assert.Empty(extractor.ExtractArticles().Items);
    }
    [Fact]
    public void ExtractArticlesWithGtinsTest()
    {
        var connectionMock = new Mock<ISybaseConnection>();
        var article = new Article
        {
            ArtId = 1000,
            ArtNr = 10000,
            Description = "Description",
            Price = 20.20m,
            PromotionPrice = 16.40m,
            Category = "Category",
            ArticleGroup = "Article Group",
            Deposit = 0.3000m,
            Content = "Content",
            PackageName = "Har./ Wein",
            PackageContent = 15.000m,
            PackageDeposit = 0.95m,
            TaxRate = 7.7000m
        };
        connectionMock.Setup(m => m.GetQueryable<Article>())
            .Returns(new List<Article>
            {
                article
            }.AsQueryable());
        connectionMock.Setup(m => m.GetQueryable<GtinGroup>())
            .Returns(new List<GtinGroup>
            {
                new() {ArtId = 1000, Gtin = 1234},
                new() {ArtId = 1000, Gtin = 5678},
                new() {ArtId = 1001, Gtin = 1234}
            }.AsQueryable());
        var extractor = new DataExtractor(connectionMock.Object);
        var extractedArticle = extractor.ExtractArticles().Items.First();

        Assert.Single(extractor.ExtractArticles().Items);
        Assert.Equal(article.ArtNr, extractedArticle.ArtNr);
        Assert.Equal(article.Description, extractedArticle.Description);
        Assert.Equal(article.Price, extractedArticle.Price);
        Assert.Equal(article.PromotionPrice, extractedArticle.PromotionPrice);
        Assert.Equal(article.Category, extractedArticle.Category);
        Assert.Equal(article.ArticleGroup, extractedArticle.ArticleGroup);
        Assert.Equal(article.Deposit, extractedArticle.Deposit);
        Assert.Equal(article.Content, extractedArticle.Content);
        Assert.Equal(2, extractedArticle.Gtins.Count);
    }
}