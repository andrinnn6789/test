using System;
using System.IO;

using IAG.Common.TestHelper.Arrange;
using IAG.Infrastructure.TestHelper.xUnit;
using IAG.VinX.Smith.SalesPdf;

using Xunit;

namespace IAG.VinX.Smith.IntegrationTest.SalesPdf;

public class DataExtractorTest
{
    [Fact]
    public void ExtractTest()
    {
        var wodConfig = new ExtractorWodConfig();
        var extractor = new DataExtractor(SybaseConnectionFactoryHelper.CreateFactory().CreateConnection(), ConfigHelper.WodConnector, new MockILogger<DataExtractorTest>(), wodConfig);
        var testPath = Path.Combine(Environment.CurrentDirectory, wodConfig.ExportRoot);
        if (!Directory.Exists(testPath))
            Directory.CreateDirectory(testPath);
        var results = extractor.Extract(DateTime.MinValue);
        Assert.Equal(2, results.Count);
        Assert.Equal(1,results[1].ErrorCount);
    }
}