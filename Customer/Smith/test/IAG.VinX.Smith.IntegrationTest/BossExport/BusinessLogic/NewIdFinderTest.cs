using System.Collections.Generic;

using IAG.VinX.Smith.BossExport.BusinessLogic;
using IAG.VinX.Smith.BossExport.Dto;

using Xunit;

namespace IAG.VinX.Smith.IntegrationTest.BossExport.BusinessLogic;

public class NewIdFinderTest
{
    private readonly NewIdFinder _idFinder;
    private readonly List<ArticleBoss> _sourceList;
    private readonly List<int> _exportedIds;

    public NewIdFinderTest()
    {
        _idFinder = new NewIdFinder();
        _sourceList = new List<ArticleBoss>
        {
            new() {Id = 1},
            new() {Id = 2},
            new() {Id = 3}
        };
        _exportedIds = new List<int>
        {
            1, 2, 6
        };
    }

    [Fact]
    public void UpdateTest()
    {
        var newList = _idFinder.GetNewArticles(_sourceList, _exportedIds, true, false);
        Assert.Single(newList);
        Assert.Equal(3, newList[0].Id);
        Assert.Equal(4, _exportedIds.Count);
    }

    [Fact]
    public void UpdateAllTest()
    {
        var newList = _idFinder.GetNewArticles(_sourceList, _exportedIds, true, true);
        Assert.Equal(3, newList.Count);
        Assert.Equal(3, _exportedIds.Count);
    }

    [Fact]
    public void NoUpdateTest()
    {
        var newList = _idFinder.GetNewArticles(_sourceList, _exportedIds, false, false);
        Assert.Single(newList);
        Assert.Equal(3, newList[0].Id);
        Assert.Equal(3, _exportedIds.Count);
    }

    [Fact]
    public void NoUpdateAllTest()
    {
        var newList = _idFinder.GetNewArticles(_sourceList, _exportedIds, false, true);
        Assert.Equal(3, newList.Count);
        Assert.Equal(3, _exportedIds.Count);
        Assert.Equal(6, _exportedIds[2]);
    }
}