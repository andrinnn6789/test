using System;
using System.IO;

using IAG.Infrastructure.Configuration.Context;
using IAG.Infrastructure.Configuration.Macro;
using IAG.Infrastructure.Configuration.Model;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Xunit;

namespace IAG.Infrastructure.Test.Configuration.Macro;

public class MacroValueSourceTest
{
    private readonly ConfigCommonDbContext _context;

    public MacroValueSourceTest()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigCommonDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());
        _context = new ConfigCommonDbContext(optionsBuilder.Options, new ExplicitUserContext("test", null));
        _context.ConfigCommonEntries.Add(new ConfigCommon() { Name = "Number", Data = "42" });
        _context.SaveChanges();
    }

    [Fact]
    public void GetValueFromDbOnlyTest()
    {
        var configBuilder = new ConfigurationBuilder();

        var source = new MacroValueSource(_context, configBuilder.Build());
        var valueForNumber = source.GetValue("Number");
        var valueForPath = source.GetValue("Path");
        var valueNotFound = source.GetValue($"NotExisting{Guid.NewGuid().ToString()}");

        Assert.Equal("42", valueForNumber);
        Assert.Null(valueForPath);
        Assert.Null(valueNotFound);
    }

    [Fact]
    public void GetValueFromDbAndEnvironmentTest()
    {
        var configBuilder = new ConfigurationBuilder().AddEnvironmentVariables();

        var source = new MacroValueSource(_context, configBuilder.Build());
        var valueForNumber = source.GetValue("Number");
        var valueForPath = source.GetValue("Path");
        var valueNotFound = source.GetValue($"NotExisting{Guid.NewGuid().ToString()}");

        Assert.Equal("42", valueForNumber);
        Assert.NotEmpty(valueForPath);
        Assert.Null(valueNotFound);
    }

    [Fact]
    public void GetValueFromDbAndAppSettingsTest()
    {
        var tempConfigFile = Path.GetTempFileName();
        var appSettings = "{ \"Path\": \"WrongPath\", \"ApplicationSettings\" : { \"Text\" : \"FooBar\"}, \"Number\": \"WrongNumber\" }";
        File.WriteAllText(tempConfigFile, appSettings);
        var configBuilder = new ConfigurationBuilder().AddJsonFile(tempConfigFile);

        var source = new MacroValueSource(_context, configBuilder.Build());
        var valueForNumber = source.GetValue("Number");
        var valueForText = source.GetValue("Text");
        var valueForPath = source.GetValue("Path");
        var valueNotFound = source.GetValue($"NotExisting{Guid.NewGuid().ToString()}");

        File.Delete(tempConfigFile);

        Assert.Equal("42", valueForNumber);
        Assert.Equal("FooBar", valueForText);
        Assert.Null(valueForPath);
        Assert.Null(valueNotFound);
    }

    [Fact]
    public void GetValueFromAllSourcesTest()
    {
        var tempConfigFile = Path.GetTempFileName();
        var appSettings = "{ \"Path\": \"WrongPath\", \"ApplicationSettings\" : { \"Text\" : \"FooBar\"}, \"Number\": \"WrongNumber\" }";
        File.WriteAllText(tempConfigFile, appSettings);
        var configBuilder = new ConfigurationBuilder().AddEnvironmentVariables().AddJsonFile(tempConfigFile);

        var source = new MacroValueSource(_context, configBuilder.Build());
        var valueForNumber = source.GetValue("Number");
        var valueForText = source.GetValue("Text");
        var valueForPath = source.GetValue("Path");
        var valueNotFound = source.GetValue($"NotExisting{Guid.NewGuid().ToString()}");

        File.Delete(tempConfigFile);

        Assert.Equal("42", valueForNumber);
        Assert.Equal("FooBar", valueForText);
        Assert.NotEqual("WrongPath", valueForPath);
        Assert.NotEmpty(valueForPath);
        Assert.Null(valueNotFound);
    }
}