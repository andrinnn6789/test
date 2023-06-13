using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Globalisation.TranslationExchanger;
using IAG.Infrastructure.TestHelper.Globalization.ResourceProvider;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace IAG.Infrastructure.Test.Resource;

public class ResourceExcelSyncerTest
{
    private readonly ResourceContext _resourceContext;
    private readonly Infrastructure.Globalisation.Model.Resource _resource;

    public ResourceExcelSyncerTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ResourceContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _resourceContext = ResourceContextBuilder.GetNewContext();
        _resourceContext.Resources.AddRange(new Infrastructure.Globalisation.Model.Resource { Name = "test" });
        _resourceContext.Cultures.AddRange(
            new Culture { Name = "de" },
            new Culture { Name = "de-CH" },
            new Culture { Name = "fr" },
            new Culture { Name = "fr-CH" },
            new Culture { Name = "en" });
        _resourceContext.SaveChanges();
        _resource = _resourceContext.Resources.First(r => r.Name == "test");
        _resourceContext.Translations.AddRange(
            new Translation
            {
                CultureId = _resourceContext.Cultures.First(c => c.Name == "de").Id,
                Resource = _resource,
                Value = "de"
            },
            new Translation
            {
                CultureId = _resourceContext.Cultures.First(c => c.Name == "de-CH").Id,
                Resource = _resource,
                Value = "de-CH"
            },
            new Translation
            {
                CultureId = _resourceContext.Cultures.First(c => c.Name == "fr").Id,
                Resource = _resource,
                Value = "fr"
            });
        _resourceContext.SaveChanges();
    }

    [Fact]
    public async Task GetResources()
    {
        var response = new ResourceExcelSyncer(_resourceContext).DownloadResources();
        Assert.NotNull(response);
        Assert.True(response.FileContents != null);
        await File.WriteAllBytesAsync(response.FileDownloadName, response.FileContents);
    }

    [Fact]
    public void UploadResourcesWithAdd()
    {
        var resNotHere = new Infrastructure.Globalisation.Model.Resource
        {
            Id = Guid.NewGuid(), 
            Name = "I'm not here"
        };
        _resourceContext.Resources.Add(resNotHere);
        var cultureEs = new Culture {Name = "es"};
        _resourceContext.Cultures.Add(cultureEs);
        var transEn = new Translation
        {
            Id = Guid.NewGuid(),
            CultureId = _resourceContext.Cultures.First(c => c.Name == "en").Id,
            Resource = _resource,
            Value = "test-en"
        };
        var transEs = new Translation
        {
            Id = Guid.NewGuid(),
            CultureId = cultureEs.Id,
            Resource = _resource,
            Value = "test-es"
        };
        var transEmpty = new Translation
        {
            Id = Guid.NewGuid(),
            CultureId = _resourceContext.Cultures.First(c => c.Name == "en").Id,
            Resource = resNotHere
        }; 
        var transNotHere = new Translation
        {
            Id = Guid.NewGuid(),
            CultureId = _resourceContext.Cultures.First(c => c.Name == "de").Id,
            Resource = resNotHere,
            Value = "not here"
        };
        _resourceContext.Translations.AddRange(transEn, transEs, transEmpty, transNotHere);
        _resourceContext.SaveChanges();
        var syncer = new ResourceExcelSyncer(_resourceContext);
        var excel = syncer.DownloadResources();
        File.WriteAllBytes(excel.FileDownloadName, excel.FileContents);
        _resourceContext.Translations.RemoveRange(transEn, transEs, transEmpty, transNotHere);
        _resourceContext.Translations.First(t => t.Resource == _resource && t.Culture.Name == "de").Value = "sdkfskdj";
        _resourceContext.Cultures.Remove(cultureEs);
        _resourceContext.Resources.Remove(resNotHere);
        _resourceContext.SaveChanges();
        syncer.UploadResources(excel.FileContents);
        Assert.Equal(6, _resourceContext.Translations.Count());
    }
}