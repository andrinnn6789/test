using System.Linq;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;

namespace IAG.ControlCenter.Distribution.BusinessLayer;

public static class FileCompareLogic
{
    public static IQueryable<T> GetMatchingFiles<T>(IQueryable<T> fileList, IFileCompareInfo file)
        where T:IFileCompareInfo
    {
        fileList = fileList.Where(x => x.Name == file.Name);
        var versionCheckAdded = false;
        if (!string.IsNullOrEmpty(file.FileVersion))
        {
            fileList = fileList.Where(x => x.FileVersion == file.FileVersion);
            versionCheckAdded = true;
        }
        if (!string.IsNullOrEmpty(file.ProductVersion))
        {
            fileList = fileList.Where(x => x.ProductVersion == file.ProductVersion);
            versionCheckAdded = true;
        }

        if (!versionCheckAdded && file.Checksum?.Length > 0)
        {
            fileList = fileList.Where(x => x.Checksum != null && x.Checksum.SequenceEqual(file.Checksum));
        }

        return fileList;
    }
}