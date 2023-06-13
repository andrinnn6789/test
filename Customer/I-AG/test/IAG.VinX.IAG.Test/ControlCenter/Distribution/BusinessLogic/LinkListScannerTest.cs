using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IAG.Infrastructure.Exception;
using IAG.VinX.IAG.ControlCenter.Distribution.BusinessLogic;

using Xunit;

namespace IAG.VinX.IAG.Test.ControlCenter.Distribution.BusinessLogic;

public class LinkListScannerTest
{
    [Fact]
    public async Task OverallLinkListScannerTest()
    {
        var testDir = Path.Combine(Path.GetTempPath(), "LinkListScannerTest");
        var testLinkList = Path.Combine(testDir, "linkList.json");
        var linkListContent = @"{""Links"":[
                { ""Name"": ""Notepad++"", ""Link"": ""https://notepad-plus-plus.org/downloads/""},
                { ""Name"": ""SQLite Browser"", ""Link"": ""https://sqlitebrowser.org/dl/"", ""Description"": ""Test"" }
            ]}";
    
        List<LinkData> links;
        try
        {
            Directory.CreateDirectory(testDir);
            await File.WriteAllTextAsync(testLinkList, linkListContent, Encoding.UTF8);

            var scanner = new LinkListScanner();
            links = (await scanner.ScanAsync(testDir)).ToList();
        }
        finally
        {
            Directory.Delete(testDir, true);
        }

        Assert.NotNull(links);
        Assert.Equal(2, links.Count);
        Assert.Single(links, a => a.Name == "Notepad++" && a.Link == "https://notepad-plus-plus.org/downloads/");
        Assert.Single(links, a => a.Name == "SQLite Browser" && a.Link == "https://sqlitebrowser.org/dl/" && a.Description == "Test");
    }

    [Fact]
    public void LinkListScannerErrorTest()
    {
        Assert.ThrowsAsync<LocalizableException>(() => new LinkListScanner().ScanAsync("FantasyDirectory"));
    }
}