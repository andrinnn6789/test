using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.Infrastructure.Settings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Settings;

public class ConfigurationExtensionsTest
{
    [Fact]
    public void GetAllConfigValuesFromProviderTest()
    {
        var configurationProvider = new JsonStreamConfigurationProvider(new JsonStreamConfigurationSource() { Stream = File.OpenRead($"{Environment.CurrentDirectory}\\Settings\\appsettings.json") });
        configurationProvider.Load();

        var configEntries = configurationProvider.GetAllConfigValues().ToList();

        Assert.Equal(3, configEntries.Count);
        Assert.Contains(configEntries, x => x.Key == "Resources:providerName");
        Assert.Contains(configEntries, x => x.Key == "Authentication:Mode");
        Assert.All(configEntries, x => Assert.NotNull(x.Key));
    }

    [Fact]
    public void GetAllConfigValuesFromConfigRootTest()
    {
        var configurationProvider = new JsonStreamConfigurationProvider(new JsonStreamConfigurationSource() { Stream = File.OpenRead($"{Environment.CurrentDirectory}\\Settings\\appsettings.json") });
        var configRoot = new ConfigurationRoot(new List<IConfigurationProvider>() { configurationProvider, new EnvironmentVariablesConfigurationProvider() });

        var configEntries = configRoot.GetAllConfigValues().ToList();

        Assert.True(configEntries.Count > 3);
    }

    [Fact]
    public void GetConfigAsJsonTest()
    {
        var configurationProvider = new JsonStreamConfigurationProvider(new JsonStreamConfigurationSource() { Stream = File.OpenRead($"{Environment.CurrentDirectory}\\Settings\\BodyTest.json") });
        var configRoot = new ConfigurationRoot(new List<IConfigurationProvider>() { configurationProvider, new EnvironmentVariablesConfigurationProvider() });

        var emptyBody = configRoot.GetSection("EmptyBody").GetConfigAsJson();
        var emptyStringBody = configRoot.GetSection("EmptyStringBody").GetConfigAsJson();
        var stringBody = configRoot.GetSection("StringBody").GetConfigAsJson();
        var arrayBody = configRoot.GetSection("ArrayBody").GetConfigAsJson();
        var objectBody = configRoot.GetSection("ObjectBody").GetConfigAsJson();

        Assert.NotNull(emptyBody);
        Assert.NotNull(emptyStringBody);
        Assert.NotNull(stringBody);
        Assert.NotNull(arrayBody);
        Assert.Empty(emptyBody.ToString());
        Assert.Empty(emptyStringBody.ToString());
        Assert.Equal("TestString", stringBody.ToString());
        Assert.IsType<JArray>(arrayBody);
        Assert.Equal("[1,2,3]", arrayBody.ToString(Formatting.None));
        Assert.IsType<JObject>(objectBody);
        Assert.Equal("{\"Decimal\":3.14,\"Number\":42,\"Object\":{\"Array\":[\"a\",\"b\",\"c\"],\"Boolean\":true},\"String\":\"Test\"}", objectBody.ToString(Formatting.None));
    }
}