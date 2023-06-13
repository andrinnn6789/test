using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.VinX.Schüwo.Resource;
using IAG.VinX.Schüwo.SV.ProcessEngine.UploadImages;

namespace IAG.VinX.Schüwo.SV.BusinessLogic;

public class ImageSyncer
{
    private readonly FtpConnector _ftpConnector;
    private readonly IMessageLogger _msgLogger;
    private readonly UploadImagesJobResult _jobResult;
    private List<int> _activeArticles;
    private string _archivePath;

    private readonly List<string> _supportedImageExtensions = new()
    {
        ".jpg",
        ".png",
        ".gif"
    };

    public ImageSyncer(FtpConnector ftpConnector, IMessageLogger msgLogger, UploadImagesJobResult jobResult)
    {
        _ftpConnector = ftpConnector;
        _msgLogger = msgLogger;
        _jobResult = jobResult;
    }

    public void SyncImagesToFtp(List<int> activeArticles, string sourcePath, string archivePath, DateTime lastSync)
    {
        _activeArticles = activeArticles;
        _archivePath = archivePath;

        var sourceFileInfoList = GetFileInfoList(lastSync, sourcePath);
        var destinationImageList = _ftpConnector.GetImageList();

        foreach (var sourceFileInfo in sourceFileInfoList)
        {
            ExecuteInErrorhandler(
                () =>
                {
                    if (!CheckFileExtension(sourceFileInfo) || !CheckFileOfActiveArticle(sourceFileInfo)) 
                        return;

                    if (destinationImageList.Contains(sourceFileInfo.Name))
                        destinationImageList.Remove(sourceFileInfo.Name);

                    using var imageStream = new FileStream(sourceFileInfo.FullName, FileMode.Open);
                    _ftpConnector.UploadImage(imageStream, sourceFileInfo.Name);

                    _jobResult.ResultCounts.SuccessCount++;
                }, 
                ResourceIds.SyncWarningSyncImageFormatMessage, 
                sourceFileInfo.Name);
        }

        if (destinationImageList.Count > 0)
            DeleteImages(destinationImageList);

        _jobResult.ArticleImagesCount = sourceFileInfoList.Count;
    }

    private static List<FileInfo> GetFileInfoList(DateTime lastSync, string sourcePath)
    {
        var dir = new DirectoryInfo(Path.GetFullPath(sourcePath));
        return dir.GetFiles().Where(f => f.LastWriteTime > lastSync).ToList();
    }

    /// <summary>
    /// Valid file names are equal to the article number. Names with an underscore are valid, but not sent by ftp
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    private bool CheckFileOfActiveArticle(FileInfo fileInfo)
    {
        var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
        var posUnderscore = fileName.IndexOf('_');
        if (posUnderscore > -1)
            fileName = fileName[..posUnderscore];
        if (int.TryParse(fileName, out var artId) && _activeArticles.Contains(artId))
            return (posUnderscore == -1);

        ArchiveImage(fileInfo);

        return false;
    }

    private void ArchiveImage(FileInfo fileInfo)
    {
        var sourceFile = fileInfo.FullName;
        var destFile = Path.Combine(_archivePath, fileInfo.Name);
        ExecuteInErrorhandler(
            () =>
            {
                File.Move(sourceFile, destFile);
                _jobResult.ImagesMovedCount++;
            }, 
            Infrastructure.Resource.ResourceIds.GenericError, 
            fileInfo.FullName);
    }

    private bool CheckFileExtension(FileInfo fileInfo)
    {
        if (_supportedImageExtensions.Contains(fileInfo.Extension.ToLower()))
            return true;

        _msgLogger.AddMessage(MessageTypeEnum.Warning, ResourceIds.SyncWarningUnsupportedImageExtensionFormatMessage, fileInfo.Name);
        _jobResult.ResultCounts.WarningCount++;
        return false;
    }

    private void DeleteImages(IEnumerable<string> fileNames)
    {
        foreach (var fileName in fileNames)
        {
            ExecuteInErrorhandler(
                () => _ftpConnector.DeleteImage(fileName), 
                ResourceIds.SyncWarningDeleteImageFormatMessage, 
                fileName);
        }
    }

    private void ExecuteInErrorhandler(Action a, string resource, string info)
    {
        try
        {
            a();
        }
        catch (Exception e)
        {
            _msgLogger.AddMessage(MessageTypeEnum.Error, resource, info, e.Message);
            _msgLogger.AddMessage(MessageTypeEnum.Debug, resource, info, e);
            _jobResult.ErrorCount++;
        }
    }
}