using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.Infrastructure.Exception;

using Xunit;

namespace IAG.ControlCenter.Test.Distribution.BusinessLayer;

public class FileCollectorTest
{
    [Fact]
    public async Task WorkingFileCollectorTest()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fileCollector = new FileCollector(path);
        var files = fileCollector.GetFiles();

        Assert.Equal(path, fileCollector.BasePath);
        Assert.NotNull(fileCollector.GetReleaseVersion());
        Assert.NotNull(fileCollector.GetPlatform());
        Assert.NotNull(files);
        Assert.NotEmpty(files);
        Assert.All(files, file =>
        {
            Assert.NotNull(file);
            Assert.NotNull(file.Name);

            var content = fileCollector.GetFileContentAsync(file.Name);
            Assert.NotNull(content);
        });

        Assert.Throws<LocalizableException>(() => new FileCollector("FantasyPath").GetFiles());
        await Assert.ThrowsAsync<LocalizableException>(() => fileCollector.GetFileContentAsync("A not existing file.txt"));
    }

    [Fact]
    public async Task FileCollectorExceptionTests()
    {
        var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var emptyPath = Path.Combine(testPath, "Empty");
            var wrongPath = Path.Combine(testPath, "Wrong");
            var publicPath = Path.Combine(emptyPath, "Publish");
            Directory.CreateDirectory(emptyPath);
            Directory.CreateDirectory(wrongPath);
            Directory.CreateDirectory(publicPath);
            await File.WriteAllTextAsync(Path.Combine(wrongPath, "IAG.Infrastructure.dll"), "Useless content...");

            var emptyFileCollector = new FileCollector(testPath);
            var wrongFileCollector = new FileCollector(wrongPath);
            var publicFileCollector = new FileCollector(publicPath);

            Assert.Empty(emptyFileCollector.GetFiles());
            Assert.Equal("Empty", publicFileCollector.GetPlatform());
            Assert.Throws<LocalizableException>(() => emptyFileCollector.GetReleaseVersion());
            Assert.Throws<LocalizableException>(() => wrongFileCollector.GetReleaseVersion());
        }
        finally
        {
            Directory.Delete(testPath, true);
        }
    }
}