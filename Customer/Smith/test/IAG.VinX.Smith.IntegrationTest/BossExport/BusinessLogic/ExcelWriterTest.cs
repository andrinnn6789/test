using System;
using System.IO;
using System.Linq;

using IAG.Common.TestHelper.Arrange;
using IAG.VinX.Smith.BossExport.BusinessLogic;
using IAG.VinX.Smith.BossExport.Dto;

using Xunit;

namespace IAG.VinX.Smith.IntegrationTest.BossExport.BusinessLogic;

public class ExcelWriterTest
{
    [Fact]
    public void AddressTest()
    {
        var connection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
        var exporter = new ExcelWriter();
        var data = exporter.GetExcel(connection.GetQueryable<ArticleBoss>().ToList());
        Assert.NotEmpty(data);
        var testPath = Path.Combine(Environment.CurrentDirectory, "BossExport");
        if (!Directory.Exists(testPath))
            Directory.CreateDirectory(testPath);
        File.WriteAllBytes(Path.Combine(testPath, "ArticleBoss.xlsx"), data);
    }
}