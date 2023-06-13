using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.ControlCenter.Resource;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception;

namespace IAG.ControlCenter.Distribution.BusinessLayer;

public class FileCollector
{
    public string BasePath { get; }

    public FileCollector(string basePath)
    {
        BasePath = basePath;
    }

    public FileInfo VersionFileInfo =>
        new DirectoryInfo(BasePath)
            .GetFiles("IAG.Infrastructure.dll", SearchOption.TopDirectoryOnly)
            .FirstOrDefault();

    public string GetReleaseVersion()
    {
        if (VersionFileInfo == null)
        {
            throw new LocalizableException(ResourceIds.MainFileNotFoundError);
        }

        var fileVersionInfo = FileVersionInfo.GetVersionInfo(VersionFileInfo.FullName);
        if (string.IsNullOrEmpty(fileVersionInfo.ProductVersion))
        {
            throw new LocalizableException(ResourceIds.MainFileNoVersionError);
        }

        return AssemblyInspector.ExtractProductVersion(fileVersionInfo.ProductVersion);
    }

    public string GetPlatform()
    {
        var pathParts = BasePath.Split(Path.DirectorySeparatorChar);
        var platform = pathParts.LastOrDefault();
        if (platform != null && platform.ToLowerInvariant() == "publish")
        {
            platform = pathParts.SkipLast(1).LastOrDefault();
        }

        return platform;
    }

    public List<FileRegistration> GetFiles()
    {
        try
        {
            var dirInfo = new DirectoryInfo(BasePath);
            var fileInfos = dirInfo.GetFiles("*", SearchOption.TopDirectoryOnly);

            var fileInfoTasks = fileInfos.Select(fileInfo => Task.Run(()
                => GetFileRegistration(fileInfo))).ToList();
            Task.WaitAll(fileInfoTasks.ToArray<Task>());
            var results = fileInfoTasks.Select(t => t.Result).ToList();

            return results;
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.GetFilesError, ex, BasePath);
        }
    }

    public Task<byte[]> GetFileContentAsync(string fileName)
    {
        try
        {
            var filePath = Path.Combine(BasePath, fileName);
            return File.ReadAllBytesAsync(filePath);
        }
        catch (Exception ex)
        {
            throw new LocalizableException(ResourceIds.ReadFileContentError, ex, fileName);
        }
    }

    private FileRegistration GetFileRegistration(FileInfo fileInfo)
    {
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(fileInfo.FullName);
        var fileRegistration = new FileRegistration
        {
            Name = fileInfo.Name,
            FileVersion = fileVersionInfo.FileVersion,
            ProductVersion = fileVersionInfo.ProductVersion,
            FileLastModifiedDate = fileInfo.LastWriteTimeUtc
        };

        if (string.IsNullOrEmpty(fileRegistration.FileVersion) &&
            string.IsNullOrEmpty(fileRegistration.ProductVersion))
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(fileInfo.FullName);
            fileRegistration.Checksum = md5.ComputeHash(stream);
        }

        return fileRegistration;
    }
}