using System;

using IAG.Infrastructure.CoreServer.ActionFilter;
using IAG.Infrastructure.CoreServer.Plugin;

using Xunit;

namespace IAG.Infrastructure.Test.CoreServer.ActionFilter;

public class HideDisabledPluginActionFilterAttributeTest
{
    [Fact]
    public void ConstructorTest()
    {
        var attribute = new HideDisabledPluginActionFilterAttribute(typeof(TestConfig));

        Assert.NotNull(attribute);
        Assert.Throws<ArgumentException>(() => new HideDisabledPluginActionFilterAttribute(GetType()));
    }

    public class TestConfig : IPluginConfig {
        public Guid Id => Guid.Empty;
        public string PluginName => "Test";
        public bool Active => false;
    }
}