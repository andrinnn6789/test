using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer;
using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.Infrastructure.Settings;
using IAG.InstallClient.BusinessLogic.Model;
using IAG.InstallClient.Resource;

namespace IAG.InstallClient.BusinessLogic;

public class InstallationManager : IInstallationManager
{
    private readonly IReleaseManager _releaseManager;

    public InstallationManager(IReleaseManager releaseManager)
    {
        _releaseManager = releaseManager;
    }

    public string CurrentSelfVersion => AssemblyInspector.GetProductVersion(Assembly.GetExecutingAssembly().Location);


    public async Task<IEnumerable<InstalledRelease>> GetInstallationsAsync()
    {
        var installations = new List<InstalledRelease>();
        var pathManager = new PathStructureManager();
        foreach (var instanceName in pathManager.GetInstanceNames())
        {
            pathManager.InstanceName = instanceName;
            var versionInformation = new VersionInformation(pathManager.InstanceBinPath);
            var bpeVersion = versionInformation.BpeVersion;
            if (bpeVersion != null)
            {
                var customerPluginName = await versionInformation.CustomerPluginNameAsync();
                installations.Add(new InstalledRelease
                {
                    InstanceName = instanceName,
                    ProductName = versionInformation.ProductName,
                    Version = bpeVersion,
                    CustomerPluginName = customerPluginName
                });
            }
        }

        return installations;
    }

    public async Task<string> CreateOrUpdateInstallationAsync(InstallationSetup setup, IMessageLogger messageLogger = null)
    {
        var pathManager = new PathStructureManager(setup.InstanceName);
        string backupDirectory = null;
        if (Directory.Exists(pathManager.InstancePath))
        {
            messageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationBackup);
            DoBackup(pathManager);
            messageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationCleanup);
            CleanupInstallation(pathManager);
            backupDirectory = pathManager.InstanceBackupBinPath;
        }
        else
        {
            Directory.CreateDirectory(pathManager.InstancePath);
        }

        var curProgress = 0.05;
        messageLogger?.ReportProgress(curProgress);
        var subProgressRange = 0.9 / (1.0 + (setup.CustomerExtensionReleaseId.HasValue ? 1.0 : 0.0) + (setup.ConfigurationReleaseId.HasValue ? 1.0 : 0.0));

        var subMessageLogger = messageLogger != null ? new SubMessageLogger(messageLogger, curProgress, curProgress + subProgressRange) : null;
        subMessageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationInstallProduct);
        await InstallFilesAsync(pathManager.InstanceBinPath, setup.CustomerId, setup.ProductId, setup.ReleaseId, backupDirectory, subMessageLogger);
        if (setup.CustomerExtensionReleaseId.HasValue)
        {
            curProgress += subProgressRange;
            subMessageLogger = messageLogger != null ? new SubMessageLogger(messageLogger, curProgress, curProgress + subProgressRange) : null;
            subMessageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationInstallExtension);
            await InstallFilesAsync(pathManager.InstanceBinPath, setup.CustomerId, setup.ProductId, setup.CustomerExtensionReleaseId.Value, backupDirectory, subMessageLogger);
        }
        if (setup.ConfigurationProductId.HasValue && setup.ConfigurationReleaseId.HasValue)
        {
            curProgress += subProgressRange;
            subMessageLogger = messageLogger != null ? new SubMessageLogger(messageLogger, curProgress, curProgress + subProgressRange) : null;
            subMessageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationInstallConfiguration);
            await InstallFilesAsync(pathManager.InstanceSettingsPath, setup.CustomerId, setup.ConfigurationProductId.Value, setup.ConfigurationReleaseId.Value, backupDirectory, subMessageLogger);
        }

        return pathManager.InstanceName;
    }

    public void TransferInstance(string sourceInstance, string targetInstance, IMessageLogger messageLogger = null)
    {
        var pathManagerSource = new PathStructureManager(sourceInstance);
        var pathManagerTarget = new PathStructureManager(targetInstance);
        messageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationBackup);
        DoBackup(pathManagerTarget);
        messageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationCleanup);
        CleanupInstallation(pathManagerTarget);

        messageLogger?.ReportProgress(0.05);
        messageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.TransferCopyFiles);
        var sourceFilePaths = Directory.EnumerateFiles(pathManagerSource.InstanceBinPath).ToList();
        for (var i = 0; i < sourceFilePaths.Count; i++)
        {
            var sourceFilePath = sourceFilePaths[i];
            var targetFilePath = Path.Combine(pathManagerTarget.InstanceBinPath, Path.GetFileName(sourceFilePath));
            File.Copy(sourceFilePath, targetFilePath);
            messageLogger?.ReportProgress(0.05 + 0.90 * i / sourceFilePaths.Count);
        }
    }

    public void DeleteInstance(string instanceName)
    {
        var pathManager = new PathStructureManager(instanceName);
        if (Directory.Exists(pathManager.InstanceBinPath))
        {
            Directory.Delete(pathManager.InstanceBinPath, true);
        }

        if (Directory.Exists(pathManager.InstanceBackupBinPath))
        {
            Directory.Delete(pathManager.InstanceBackupBinPath, true);
        }
    }

    public void DeleteInstanceDirectory(string instanceName)
    {
        var pathManager = new PathStructureManager(instanceName);
        var directories = Directory.EnumerateDirectories(pathManager.InstancePath).Select(d => d.ToLower()).ToHashSet();
        directories.Remove(pathManager.InstanceLogPath.ToLower());
        directories.Remove(pathManager.InstanceSettingsPath.ToLower());
        if (!directories.Any())
        {
            // no other applications
            Directory.Delete(pathManager.InstancePath, true);
        }
    }

    [ExcludeFromCodeCoverage]
    public async Task DoSelfUpdate(InstallationSetup setup, IMessageLogger messageLogger = null)
    {
        var pathStructureManager = new PathStructureManager();
        var tempDirectory = pathStructureManager.SelfUpdatePath ?? string.Empty;
        var installationDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, true);
        }

        var subMessageLogger = messageLogger != null ? new SubMessageLogger(messageLogger, 0.0, 0.8) : null;
        subMessageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationInstallProduct);
        await InstallFilesAsync(tempDirectory, setup.CustomerId, setup.ProductId, setup.ReleaseId, installationDirectory, subMessageLogger);
            
        var updateProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(tempDirectory, "Update.bat"),
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = true
            }
        };
        updateProcess.StartInfo.ArgumentList.Add(installationDirectory);

        if (setup.ConfigurationProductId.HasValue && setup.ConfigurationReleaseId.HasValue)
        {
            var configPath = Path.Combine(tempDirectory, "Settings");
            subMessageLogger = messageLogger != null ? new SubMessageLogger(messageLogger, 0.8, 0.9) : null;
            subMessageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationInstallConfiguration);
            await InstallFilesAsync(configPath, setup.CustomerId, setup.ConfigurationProductId.Value, setup.ConfigurationReleaseId.Value, configPath, subMessageLogger);
            updateProcess.StartInfo.ArgumentList.Add(Path.Combine(installationDirectory, pathStructureManager.InstallerSettingsPath));
        }

        messageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.SelfUpdateStartProcess);
        if (!updateProcess.Start())
        {
            throw new LocalizableException(ResourceIds.SelfUpdateStartProcessError);
        }
    }

    private async Task InstallFilesAsync(string installationPath, Guid customerId, Guid productId, Guid releaseId,
        string backupDirectory, IMessageLogger messageLogger = null)
    {
        Directory.CreateDirectory(installationPath);

        var backupFiles = backupDirectory != null
            ? new FileCollector(backupDirectory).GetFiles().AsQueryable()
            : Enumerable.Empty<FileRegistration>().AsQueryable();

        messageLogger?.AddMessage(MessageTypeEnum.Information, ResourceIds.InstallationGetFiles);
        var releaseFiles = (await _releaseManager.GetReleaseFilesAsync(customerId, productId, releaseId)).ToList();
        var progressStep = 1.0 / releaseFiles.Count;
        var currentProgress = 0.0;
        foreach (var releaseFile in releaseFiles)
        {
            Stream contentStream; 
            var backupFile = FileCompareLogic.GetMatchingFiles(backupFiles, releaseFile)
                .FirstOrDefault();
            if (backupFile != null)
            {
                // ReSharper disable once AssignNullToNotNullAttribute - No, FirstOrDefault wouldn't return a result...
                contentStream = File.OpenRead(Path.Combine(backupDirectory, backupFile.Name));
            }
            else
            {
                contentStream = await _releaseManager.GetFileContentStreamAsync(customerId, releaseFile.Id);
                contentStream.Seek(0, SeekOrigin.Begin);
            }
            await using var fileStream = File.OpenWrite(Path.Combine(installationPath, releaseFile.Name));
            await contentStream.CopyToAsync(fileStream);
            await contentStream.DisposeAsync();

            currentProgress += progressStep;
            messageLogger?.ReportProgress(currentProgress);
        }
    }

    private static void DoBackup(PathStructureManager pathManager)
    {
        if (Directory.Exists(pathManager.InstanceBackupPath))
        {
            Directory.Delete(pathManager.InstanceBackupPath, true);
        }

        CopyDirectory(pathManager.InstanceBinPath, pathManager.InstanceBackupBinPath);
        if (Directory.Exists(pathManager.InstanceSettingsPath))
            CopyDirectory(pathManager.InstanceSettingsPath, pathManager.InstanceBackupSettingsPath);
    }

    private static void CleanupInstallation(PathStructureManager pathManager)
    {
        var installationDirectory = new DirectoryInfo(pathManager.InstanceBinPath);
        foreach (FileInfo file in installationDirectory.EnumerateFiles())
        {
            file.Delete();
        }
    }

    private static void CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        var sourceDirectory = new DirectoryInfo(source);
        foreach (FileInfo file in sourceDirectory.EnumerateFiles())
        {
            var filePath = Path.Combine(destination, file.Name);
            file.CopyTo(filePath, false);
        }
    }
}