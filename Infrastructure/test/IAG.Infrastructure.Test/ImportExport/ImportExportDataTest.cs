using IAG.Infrastructure.Exception;
using IAG.Infrastructure.ImportExport;

using Xunit;

namespace IAG.Infrastructure.Test.ImportExport;

public class ImportExportDataTest
{
    [Fact]
    public void CheckTypeTest()
    {
        var importData = new TestImportData();

        importData.CheckType();
        importData.Type = "SomethingElse...";

        Assert.Throws<LocalizableException>(() => importData.CheckType());
    }

    private class TestImportData : ImportExportData
    {
    }
}