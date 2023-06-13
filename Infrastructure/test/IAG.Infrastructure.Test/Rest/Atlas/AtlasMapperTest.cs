using System;
using System.Collections.Generic;

using FluentAssertions;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Rest.Atlas;
using IAG.Infrastructure.Rest.Atlas.Dto;

using Newtonsoft.Json;

using Xunit;

namespace IAG.Infrastructure.Test.Rest.Atlas;

public class AtlasMapperTest
{
    [Fact]
    public void MapBackAndForthTest()
    {
        var testProperties = new List<Property>
        {
            new() { Name = "NumberTest", Type = AtlasType.Number },
            new() { Name = "StringTest", Type = AtlasType.String },
            new() { Name = "BooleanTest", Type = AtlasType.Boolean },
            new() { Name = "DateTimeTest", Type = AtlasType.DateTime }
        };
        var testObject = new TestClass
        {
            NumberTest = 42,
            StringTest = "Test",
            BooleanTest = true,
            DateTimeTest = DateTime.Now
        };
        var testData = new List<object>
        {
            testObject.NumberTest,
            testObject.StringTest,
            testObject.BooleanTest,
            testObject.DateTimeTest
        };
        var testMapper = new AtlasMapper<TestClass>(testProperties);

        var testObjectBack = testMapper.GetObject(testData);
        var testDataBack = testMapper.GetData(testObject);

        testObject.Should().BeEquivalentTo(testObjectBack);
        testData.Should().BeEquivalentTo(testDataBack);
    }

    [Fact]
    public void JsonPropertyRenameTest()
    {
        var testData = "Test";
        var testProperties = new List<Property> { new() { Name = "NameInJson", Type = AtlasType.String } };
        var testMapper = new AtlasMapper<TestClass>(testProperties);

        var testObject = testMapper.GetObject(new List<object>() { testData });

        Assert.Equal(testData, testObject.JsonPropertyTest);
    }

    [Fact]
    public void NotAvailablePropertyTest()
    {
        var testProperties = new List<Property> { new() { Name = "WrongName", Type = AtlasType.Number } };
        var testMapper = new AtlasMapper<TestClass>(testProperties);

        testMapper.GetObject(new List<object>() { "Test" });
    }

    [Fact]
    public void ExceptionTest()
    {
        var testProperties = new List<Property> { new() { Name = "WrongName", Type = AtlasType.Number } };
        var testObject = new TestClass();
        var testMapper = new AtlasMapper<TestClass>(testProperties);

        Assert.Throws<LocalizableException>(() => testMapper.GetData(testObject));
        Assert.Throws<ArgumentException>(() => testMapper.GetObject(new List<object>() { "Test", 42 }));
    }

    private class TestClass
    {
        public int NumberTest { get; set; }

        public string StringTest { get; set; }

        public bool BooleanTest { get; set; }

        public DateTime DateTimeTest { get; set; }

        [JsonProperty("NameInJson")]
        public string JsonPropertyTest { get; set; }
    }
}