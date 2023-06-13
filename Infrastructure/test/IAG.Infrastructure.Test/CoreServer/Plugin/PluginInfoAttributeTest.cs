using System;

using IAG.Infrastructure.DI;

using Xunit;

namespace IAG.Infrastructure.Test.CoreServer.Plugin;

public class PluginInfoAttributeTest
{
    [Fact]
    public void PluginInfoTest()
    {
        var testType = typeof(TestClass);

        Assert.NotNull(PluginInfoAttribute.GetPluginInfo(testType));
        Assert.Equal(new Guid("21F90665-A929-47E2-BE48-1F9DA06F470C"), PluginInfoAttribute.GetPluginId(testType));
        Assert.Equal("TestName", PluginInfoAttribute.GetPluginName(testType));
    }

    [Fact]
    public void WrongTypeWithoutAttributeTest()
    {
        var testType = typeof(string);

        Assert.Throws<System.Exception>(() => PluginInfoAttribute.GetPluginInfo(testType));
        Assert.Throws<System.Exception>(() => PluginInfoAttribute.GetPluginId(testType));
        Assert.Throws<System.Exception>(() => PluginInfoAttribute.GetPluginName(testType));
    }

    [PluginInfo("21F90665-A929-47E2-BE48-1F9DA06F470C", "TestName")]
    private class TestClass
    { }
}