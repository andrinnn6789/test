using System;
using System.Linq;

using IAG.Infrastructure.Exception;

using JetBrains.Annotations;

using Xunit;

namespace IAG.Infrastructure.Test.Globalisation.ResourceProvider;

public class ResourceProviderTest
{
    [UsedImplicitly]
    private enum TestEnum
    {
        Hello = 0,
        [UsedImplicitly]
        World = 1
    }

    [Fact]
    public void GetEnumResourceIdTest()
    {
        var enumResourceId = Infrastructure.Globalisation.ResourceProvider.ResourceProvider.GetEnumResourceId(TestEnum.Hello);

        Assert.NotNull(enumResourceId);
        Assert.NotEmpty(enumResourceId);
        Assert.StartsWith(GetType().FullName ?? throw new InvalidOperationException(), enumResourceId);
        Assert.EndsWith(TestEnum.Hello.ToString(), enumResourceId);
    }

    [Fact]
    public void AddEnumTemplatesTest()
    {
        Infrastructure.Globalisation.ResourceProvider.ResourceProvider resourceProvider = new TestResourceProvider();
        resourceProvider.AddEnumTemplates(typeof(TestEnum));

        Assert.NotNull(resourceProvider.ResourceTemplates);
        Assert.NotEmpty(resourceProvider.ResourceTemplates);
        Assert.Equal(Enum.GetValues(typeof(TestEnum)).Length, resourceProvider.ResourceTemplates.Count());
        Assert.All(resourceProvider.ResourceTemplates, (tmplt) => Assert.StartsWith(GetType().FullName ?? throw new InvalidOperationException(), tmplt.Name));
        Assert.All(resourceProvider.ResourceTemplates, (tmplt) => Assert.EndsWith(tmplt.Translation, tmplt.Name));
    }

    [Fact]
    public void AddEnumTemplatesFailTest()
    {
        Infrastructure.Globalisation.ResourceProvider.ResourceProvider resourceProvider = new TestResourceProvider();
        Assert.Throws<ArgumentException>(() => resourceProvider.AddEnumTemplates(typeof(string)));
    }

    [Fact]
    public void AddDuplicateTemplatesTest()
    {
        Infrastructure.Globalisation.ResourceProvider.ResourceProvider resourceProvider = new TestResourceProvider();
        resourceProvider.AddTemplate("test", "en", "test");
        Assert.Throws<LocalizableException>(() => resourceProvider.AddTemplate("test", "en", "test"));
    }

    private class TestResourceProvider : Infrastructure.Globalisation.ResourceProvider.ResourceProvider
    {
    }
}