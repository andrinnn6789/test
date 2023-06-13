using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using IAG.Common.DataLayerSybase;
using IAG.Common.DataLayerSybase.AtlasType;
using IAG.Common.WoD.Interfaces;
using IAG.Infrastructure.Atlas;
using IAG.Infrastructure.Formatter;
using IAG.Infrastructure.Logging;
using IAG.VinX.Smith.SalesPdf.Dto;

using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;

using SearchOption = Microsoft.VisualBasic.FileIO.SearchOption;

namespace IAG.VinX.Smith.SalesPdf;

public class DataExtractor
{
    private const string WinePdfFolder = "Weinbeschriebe";
    private const string WinePictureFolder = "Flaschenbilder";
    private const string WineFolderForeign1 = "Bilder";
    private const string WineFolderForeign2 = "Presse";
    private const string WineFolderForeign3 = "Videos";
    private const string WineFolderForeign4 = "Schulungsunterlagen";

    private readonly ISybaseConnection _connection;
    private readonly IWodConnector _wodConnector;
    private readonly ILogger _logger;
    private readonly ExtractorWodConfig _wodConfig;
    private DateTime _lastCheck;
    private DateTime _currentCheck;
    private List<SyncResult> _syncResults;
    private readonly object _staticLock = 42;
    private readonly SemaphoreSlim _maxWodConnections;
    private readonly ErrorLogger _errorLogger = new();


    public DataExtractor(ISybaseConnection connection, IWodConnector wodConnector, ILogger logger, ExtractorWodConfig wodConfig)
    {
        _connection = connection;
        _wodConnector = wodConnector;
        _logger = logger;
        _wodConfig = wodConfig;
        _maxWodConnections = new SemaphoreSlim(_wodConfig.MaxThreads);
    }

    public List<SyncResult> Extract(DateTime lastCheck)
    {
        _syncResults = new List<SyncResult>();
        try
        {
            _lastCheck = lastCheck;
            _currentCheck = DateTime.Now;
            if (!Directory.Exists(_wodConfig.ExportRoot))
                Directory.CreateDirectory(_wodConfig.ExportRoot);

            ExtractProducerInfo();
            ExtractWineInfo();
        }
        catch (Exception e)
        {
            _errorLogger.LogException(_logger, e);
            _syncResults[^1].ErrorCount++;
        }

        return _syncResults;
    }

    private void ExtractProducerInfo()
    {
        var syncResult = new SyncResult
        {
            SyncName = nameof(Producer)
        };
        _syncResults.Add(syncResult);
        var producers = _connection.GetQueryable<Producer>()
            .Where(w => w.LastChange > _lastCheck && w.LastChange < _currentCheck)
            .OrderBy(w => w.Bezeichnung);
        foreach (var producer in producers)
        {
            producer.Geschichte = RtfCleaner.Clean(producer.Geschichte);

            var path = CheckProducerFolder(producer.Bezeichnung, producer.Land);
            var fileBaseName = string.Join("", producer.Bezeichnung.Split(Path.GetInvalidFileNameChars()));
            var fileBaseWithPath = Path.Combine(path, fileBaseName);
            var bytesChecked = XmlCleaner.Serialize(producer, Encoding.UTF8);

            ExtractPicture(producer.Logo, fileBaseWithPath + "-Logo");
            ExtractPicture(producer.Foto1, fileBaseWithPath + "-1");
            ExtractPicture(producer.Foto2, fileBaseWithPath + "-2");
            ExtractPicture(producer.Foto3, fileBaseWithPath + "-3");
            ExtractPicture(producer.Foto4, fileBaseWithPath + "-4");
            ExtractPicture(producer.Foto5, fileBaseWithPath + "-5");
            ExtractPicture(producer.Foto6, fileBaseWithPath + "-6");
            syncResult.ExportCount++;
            _ = CreatePdf(path, path, bytesChecked, fileBaseName, syncResult, _wodConfig.Producer);
        }

        while (_maxWodConnections.CurrentCount != _wodConfig.MaxThreads)
        {
            Thread.Sleep(200);
        }
    }

    private void ExtractWineInfo()
    {
        var syncResult = new SyncResult
        {
            SyncName = nameof(WineInfo)
        };
        _syncResults.Add(syncResult);
        var wineInfos = _connection.GetQueryable<WineInfo>()
            .Where(w => w.LastChange > _lastCheck && w.LastChange < _currentCheck)
            .OrderBy(w => w.Produzent);
        foreach (var wineInfo in wineInfos)
        {
            var (pathPdf, pathPictures) = CheckWineFolder(wineInfo.Produzent, wineInfo.Land);
            var fileBaseName = string.Join("", (wineInfo.Bezeichnung + " " + wineInfo.Jahrgang).Split(Path.GetInvalidFileNameChars()));
            var pictureBaseWithPath = Path.Combine(pathPictures, fileBaseName);

            if (wineInfo.Aktiv)
            {
                wineInfo.Charakter = RtfCleaner.Clean(wineInfo.Charakter);
                wineInfo.Geschichte = RtfCleaner.Clean(wineInfo.Geschichte);
                wineInfo.Terroir = RtfCleaner.Clean(wineInfo.Terroir);
                wineInfo.Vinifikation = RtfCleaner.Clean(wineInfo.Vinifikation);
                var bytesChecked = XmlCleaner.Serialize(wineInfo, Encoding.UTF8);
                ExtractPicture(wineInfo.Foto1, pictureBaseWithPath + "-1");
                ExtractPicture(wineInfo.Foto2, pictureBaseWithPath + "-2");
                ExtractPicture(wineInfo.Foto3, pictureBaseWithPath + "-3");
                syncResult.ExportCount++;
                _ = CreatePdf(pathPdf, pathPictures, bytesChecked, fileBaseName, syncResult, _wodConfig.WineInfo);
            }
            else
            {
                foreach (var fileName in Directory.GetFiles(pathPdf, wineInfo.ArtikelNummer + "*.*"))
                {
                    File.Delete(fileName);
                }
                foreach (var fileName in Directory.GetFiles(pathPictures, wineInfo.ArtikelNummer + "*.*"))
                {
                    File.Delete(fileName);
                }
            }
        }

        while (_maxWodConnections.CurrentCount != _wodConfig.MaxThreads)
        {
            Thread.Sleep(200);
        }
    }

    private static void ExtractPicture(byte[] picture, string fileBaseName)
    {
        var picData = PictureHandler.ExtractPicture(picture);
        if (picData == null)
            return;
        using var fs = new FileStream(fileBaseName + ".jpg", FileMode.OpenOrCreate);
        fs.Write(picData);
        fs.Close();
    }

    private (string pathPdf, string pathPictures) CheckWineFolder(string producer, string country)
    {
        var pathProducer = CheckProducerFolder(producer, country);
        Directory.CreateDirectory(Path.Combine(pathProducer, WinePictureFolder));
        Directory.CreateDirectory(Path.Combine(pathProducer, WineFolderForeign1));
        Directory.CreateDirectory(Path.Combine(pathProducer, WineFolderForeign2));
        Directory.CreateDirectory(Path.Combine(pathProducer, WineFolderForeign3));
        Directory.CreateDirectory(Path.Combine(pathProducer, WineFolderForeign4));
        var pathPdf = Path.Combine(pathProducer, WinePdfFolder);
        Directory.CreateDirectory(pathPdf);
        var pathPictures = Path.Combine(pathProducer, WinePictureFolder);
        Directory.CreateDirectory(pathPictures);
        return (pathPdf, pathPictures);
    }

    private string CheckProducerFolder(string producer, string country)
    {
        var path = Path.Combine(_wodConfig.ExportRoot, string.Join("", country.Split(Path.GetInvalidPathChars()))).Trim();
        Directory.CreateDirectory(path);
        path = Path.Combine(path, string.Join("", producer.Split(Path.GetInvalidPathChars()))).Trim();
        Directory.CreateDirectory(path);
        return path;
    }

    private async Task CreatePdf(string pathPdf, string pathPictures, byte[] xmlData, string fileBaseName, SyncResult syncResult, string jobName)
    {
        await _maxWodConnections.WaitAsync();
        try
        {
            if (_wodConfig.WithXml)
                await File.WriteAllBytesAsync(Path.Combine(pathPdf, fileBaseName + ".xml"), xmlData);
            var pdf = await _wodConnector.SubmitJob(CreateZip(pathPictures, fileBaseName + ".xml", xmlData, new[] {fileBaseName + "*.jpg"}), jobName);
            await File.WriteAllBytesAsync(Path.Combine(pathPdf, fileBaseName + ".pdf"), pdf);
            lock (_staticLock)
            {
                syncResult.SuccessCount++;
            }
        }
        catch (Exception e)
        {
            lock (_staticLock)
            {
                _errorLogger.LogException(_logger, e);
                syncResult.ErrorCount++;
            }
        }
        finally
        {
            _maxWodConnections.Release();
        }
    }

    private static byte[] CreateZip(string path, string xmlName, byte[] xmlData, string[] extensions)
    {
        var files = FileSystem.GetFiles(path, SearchOption.SearchTopLevelOnly, extensions);
        using var zipData = new MemoryStream();
        using (var zipArch = new ZipArchive(zipData, ZipArchiveMode.Create))
        {
            AddZipEntry(zipArch, xmlName, xmlData);
            foreach (var file in files)
            {
                AddZipEntry(zipArch, Path.GetFileName(file), File.ReadAllBytes(file));
            }
        }

        return zipData.ToArray();
    }

    private static void AddZipEntry(ZipArchive arch, string name, byte[] data)
    {
        var zipFileEntry = arch.CreateEntry(name);
        using Stream zipEntryStream = zipFileEntry.Open();
        using BinaryWriter zipFileBinary = new(zipEntryStream);
        zipFileBinary.Write(data);
    }
}