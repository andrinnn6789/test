using System;

using IAG.Infrastructure.Rest;
using IAG.Infrastructure.Rest.Atlas;

using Xunit;

namespace IAG.Infrastructure.Test.Rest.Atlas;

public class AtlasConfigTest
{
    [Fact]
    public void ConstructorTest()
    {
        var atlasConfigCompact = new AtlasConfig(); // MetaLevel.Compact

        AssertAcceptHeader(atlasConfigCompact, MetaLevel.Compact);
    }

    [Fact]
    public void ConstructorWithCredentialsWithAuthTest()
    {
        var config = new AtlasConfig(new AtlasCredentials
        {
            BaseUrl = "url",
            User = "user",
            Password = "pwd"
        });

        Assert.NotNull(config.Authentication);
        Assert.Equal("url", config.BaseUrl);
    }

    [Fact]
    public void ConstructorWithCredentialsTest()
    {
        var config = new AtlasConfig();
        config.Configure(new AtlasCredentials
        {
            BaseUrl = "url",
            User = null
        });

        Assert.Null(config.Authentication);
        Assert.Equal("url", config.BaseUrl);
    }

    // ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local
    private void AssertAcceptHeader(IHttpConfig config, MetaLevel metaLevel)
    {
        Assert.NotNull(config);
        Assert.NotNull(config.HttpHeaders);
        Assert.Contains(
            config.HttpHeaders,
            p => p.Key.Equals("Accept", StringComparison.InvariantCultureIgnoreCase)
                 && p.Value.StartsWith(ContentTypes.ApplicationJson, StringComparison.CurrentCultureIgnoreCase) && p.Value.EndsWith(metaLevel.ToString(), StringComparison.CurrentCultureIgnoreCase));
    }

    // ReSharper enable ParameterOnlyUsedForPreconditionCheck.Local
}