using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using IAG.ControlCenter.Distribution.BusinessLayer.Model;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.TestHelper.Startup;
using IAG.InstallClient.BusinessLogic;
using IAG.InstallClient.BusinessLogic.Model;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.BusinessLogic;

// Just a dummy class to simulate a customer plugin...
[CustomerPluginInfo("BBC73C7C-801C-4D9D-B2C3-FA8D01CBD73A")]
public static class CustomerPluginInfo
{
}

public class InstallationManagerTest
{
    private readonly IInstallationManager _installationManager;

    public InstallationManagerTest()
    {
        _installationManager = new InstallationManager(new Mock<IReleaseManager>().Object);
    }

    [Fact]
    public void CurrentSelfVersionTest()
    {
        Assert.NotEmpty(_installationManager.CurrentSelfVersion);
    }

    [Fact]
    public async Task NotExistingBasePathGetInstallationsTest()
    {
        var installedReleases = await _installationManager.GetInstallationsAsync();
        Assert.Empty(installedReleases);
    }

    [Fact]
    public async Task BasePathWithoutServerProductGetInstallationsTest()
    {
        var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var testProductPath = Path.Combine(testPath, "FantasyProduct");
        IEnumerable<InstalledRelease> installedReleases;
        try
        {
            Directory.CreateDirectory(testProductPath);
            installedReleases = await _installationManager.GetInstallationsAsync();
        }
        finally
        {
            Directory.Delete(testPath, true);
        }

        Assert.Empty(installedReleases);
    }

    [Fact]
    public async Task BasePathWithoutBpeGetInstallationsTest()
    {
        var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var testProductPath = Path.Combine(testPath, "VinX");
        var testInstallationPath = Path.Combine(testProductPath, "Server", "Prod");
        IEnumerable<InstalledRelease> installedReleases;
        try
        {
            Directory.CreateDirectory(testInstallationPath);
            installedReleases = await _installationManager.GetInstallationsAsync();
        }
        finally
        {
            Directory.Delete(testPath, true);
        }

        Assert.Empty(installedReleases);
    }

    [Fact]
    public async Task BasePathWithEmptyBpeGetInstallationsTest()
    {
        var testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var testProductPath = Path.Combine(testPath, "VinX");
        var testInstallationPath = Path.Combine(testProductPath, "Server", "Prod", "BPE");
        IEnumerable<InstalledRelease> installedReleases;
        try
        {
            Directory.CreateDirectory(testInstallationPath);
            installedReleases = await _installationManager.GetInstallationsAsync();
        }
        finally
        {
            Directory.Delete(testPath, true);
        }

        Assert.Empty(installedReleases);
    }

    [Fact]
    public async Task GetInstallationsTest()
    {
        var testInstanceName = Guid.NewGuid().ToString();
        var pathManager = new PathStructureManager(testInstanceName);
        var assemblyDirectory = Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty;
        InstalledRelease installedRelease;
        try
        {
            Directory.CreateDirectory(pathManager.InstanceBinPath);
            foreach (var file in Directory.GetFiles(assemblyDirectory))
            {
                File.Copy(file, Path.Combine(pathManager.InstanceBinPath, Path.GetFileName(file)));
            }

            installedRelease = (await _installationManager.GetInstallationsAsync()).FirstOrDefault(r => r.InstanceName == testInstanceName);
        }
        finally
        {
            Directory.Delete(pathManager.InstancePath, true);
        }

        Assert.NotNull(installedRelease);
        Assert.Equal(testInstanceName, installedRelease.InstanceName);
        Assert.NotEmpty(installedRelease.Version);
        Assert.NotEmpty(installedRelease.CustomerPluginName);
    }

    [Fact]
    public async Task GetInstallationsWithoutCustomerPluginTest()
    {
        var testInstanceName = Guid.NewGuid().ToString();
        var pathManager = new PathStructureManager(testInstanceName);
        var assemblyPath = GetType().Assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyPath) ?? string.Empty;
        var infrastructureTestAssemblyPath = typeof(TestServerEnvironment).Assembly.Location;
        List<InstalledRelease> installedReleases;
        try
        {
            Directory.CreateDirectory(pathManager.InstanceBinPath);
            foreach (var file in Directory.GetFiles(assemblyDirectory).Except(new []{assemblyPath, infrastructureTestAssemblyPath }))
            {
                File.Copy(file, Path.Combine(pathManager.InstanceBinPath, Path.GetFileName(file)));
            }

            installedReleases = (await _installationManager.GetInstallationsAsync()).ToList();
        }
        finally
        {
            Directory.Delete(pathManager.InstancePath, true);
        }

        Assert.NotNull(installedReleases);
        var installedRelease = installedReleases.FirstOrDefault(r => r.InstanceName == testInstanceName);
        Assert.NotNull(installedRelease);
        Assert.Equal(testInstanceName, installedRelease.InstanceName);
        Assert.NotEmpty(installedRelease.Version);
        Assert.Null(installedRelease.CustomerPluginName);
    }

    [Fact]
    public async Task CreateInstallationTest()
    {
        var testInstanceName = Guid.NewGuid().ToString();
        var pathManager = new PathStructureManager(testInstanceName);
        var testCustomerId = Guid.NewGuid();
        var testProductId = Guid.NewGuid();
        var testReleaseId = Guid.NewGuid();
        var testCustomerPluginId = Guid.NewGuid();
        var testConfigId = Guid.NewGuid();
        var releaseManagerMock = new Mock<IReleaseManager>();
        var testFileContent = "TestContent";
        var testReleaseFile = new FileMetaInfo
        {
            Id = Guid.NewGuid(),
            Name = "TestReleaseFile.txt"
        };
        var customerFileContent = "CustomerContent";
        var testCustomerFile = new FileMetaInfo
        {
            Id = Guid.NewGuid(),
            Name = "TestCustomerFile.txt"
        };
        var configFileContent = "ConfigContent";
        var testConfigFile = new FileMetaInfo
        {
            Id = Guid.NewGuid(),
            Name = "TestConfigFile.txt"
        };

        releaseManagerMock.Setup(m => m.GetReleaseFilesAsync(testCustomerId, testProductId, testReleaseId))
            .ReturnsAsync(new[] {testReleaseFile});
        releaseManagerMock.Setup(m => m.GetReleaseFilesAsync(testCustomerId, testProductId, testCustomerPluginId))
            .ReturnsAsync(new[] { testCustomerFile });
        releaseManagerMock.Setup(m => m.GetReleaseFilesAsync(testCustomerId, testProductId, testConfigId))
            .ReturnsAsync(new[] { testConfigFile });
        releaseManagerMock.Setup(m => m.GetFileContentStreamAsync(testCustomerId, testReleaseFile.Id))
            .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(testFileContent)));
        releaseManagerMock.Setup(m => m.GetFileContentStreamAsync(testCustomerId, testCustomerFile.Id))
            .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(customerFileContent)));
        releaseManagerMock.Setup(m => m.GetFileContentStreamAsync(testCustomerId, testConfigFile.Id))
            .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(configFileContent)));

        var installationManager = new InstallationManager(releaseManagerMock.Object);
        var testSetup = new InstallationSetup
        {
            InstanceName = testInstanceName,
            CustomerId = testCustomerId,
            ProductId = testProductId,
            ReleaseId = testReleaseId,
            CustomerExtensionReleaseId = testCustomerPluginId,
            ConfigurationProductId = testProductId,
            ConfigurationReleaseId = testConfigId,
        };

        var installedTestFilePath = Path.Combine(pathManager.InstanceBinPath, testReleaseFile.Name);
        var installedCustomerFilePath = Path.Combine(pathManager.InstanceBinPath, testCustomerFile.Name);
        var installedConfigFilePath = Path.Combine(pathManager.InstanceSettingsPath, testConfigFile.Name);
        string installedTestFileContent, installedCustomerFileContent, installedConfigFileContent;
        try
        {
            await installationManager.CreateOrUpdateInstallationAsync(testSetup);

            installedTestFileContent = await File.ReadAllTextAsync(installedTestFilePath);
            installedCustomerFileContent = await File.ReadAllTextAsync(installedCustomerFilePath);
            installedConfigFileContent = await File.ReadAllTextAsync(installedConfigFilePath);
        }
        finally
        {
            Directory.Delete(pathManager.InstancePath, true);
        }

        Assert.NotNull(installedTestFileContent);
        Assert.Equal(testFileContent, installedTestFileContent);
        Assert.Equal(customerFileContent, installedCustomerFileContent);
        Assert.Equal(configFileContent, installedConfigFileContent);
    }

    [Fact]
    public async Task UpdateInstallationTest()
    {
        var testInstanceName = Guid.NewGuid().ToString();
        var pathManager = new PathStructureManager(testInstanceName);
        var testCustomerId = Guid.NewGuid();
        var testProductId = Guid.NewGuid();
        var testReleaseId = Guid.NewGuid();
        var releaseManagerMock = new Mock<IReleaseManager>();
        var testFileContent = "TestContent";
        var testFileContentBytes = Encoding.UTF8.GetBytes(testFileContent);
        var testFile = new FileMetaInfo()
        {
            Id = Guid.NewGuid(),
            Name = "TestFile.txt",
            Checksum = MD5.Create().ComputeHash(testFileContentBytes)
        };

        releaseManagerMock.Setup(m => m.GetReleaseFilesAsync(testCustomerId, testProductId, testReleaseId))
            .ReturnsAsync(new[] { testFile });

        releaseManagerMock.Setup(m => m.GetFileContentStreamAsync(testCustomerId, testFile.Id))
            .ReturnsAsync(new MemoryStream(testFileContentBytes));

        var installationManager = new InstallationManager(releaseManagerMock.Object);
        var installedTestFilePath = Path.Combine(pathManager.InstanceBinPath, testFile.Name);
        var backupTestFilePath = Path.Combine(pathManager.InstanceBackupBinPath, testFile.Name);
        var settingsFilePath = Path.Combine(pathManager.InstanceSettingsPath, "testConfig.json");
        var testSetup = new InstallationSetup
        {
            InstanceName = testInstanceName,
            CustomerId = testCustomerId,
            ProductId = testProductId,
            ReleaseId = testReleaseId,
        };

        string installedTestFileContent, backupTestFileContent, settingsFileContent;
        bool backupSettingsExists;
        try
        {
            Directory.CreateDirectory(pathManager.InstanceBinPath);
            await File.WriteAllTextAsync(installedTestFilePath, "PreviousVersionContent");

            Directory.CreateDirectory(Path.Combine(pathManager.InstanceBackupBinPath));
            await File.WriteAllTextAsync(backupTestFilePath, "VeryOldVersionContent");

            Directory.CreateDirectory(pathManager.InstanceSettingsPath);
            await File.WriteAllTextAsync(settingsFilePath, "OriginalSetting");

            await installationManager.CreateOrUpdateInstallationAsync(testSetup);

            installedTestFileContent = await File.ReadAllTextAsync(installedTestFilePath);
            backupTestFileContent = await File.ReadAllTextAsync(backupTestFilePath);
            settingsFileContent = await File.ReadAllTextAsync(settingsFilePath);

            backupSettingsExists = File.Exists(Path.Combine(pathManager.InstanceBackupSettingsPath, "testConfig.json"));

            // do it again should not call GetFileContentStreamAsync...
            await installationManager.CreateOrUpdateInstallationAsync(testSetup);
        }
        finally
        {
            Directory.Delete(pathManager.InstancePath, true);
            Directory.Delete(pathManager.InstanceBackupPath, true);
        }

        releaseManagerMock.Verify(rm => rm.GetFileContentStreamAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Exactly(1));
        Assert.NotNull(installedTestFileContent);
        Assert.Equal(testFileContent, installedTestFileContent);
        Assert.Equal("PreviousVersionContent", backupTestFileContent);
        Assert.Equal("OriginalSetting", settingsFileContent);
        Assert.True(backupSettingsExists);
    }

    [Fact]
    public async Task TransferInstallationTest()
    {
        var instanceNameSource = Guid.NewGuid().ToString();
        var instanceNameTarget = Guid.NewGuid().ToString();
        var testFileName = "Test.txt";
        var oldFileName = "Old.txt";
        var testFileTargetContent = "TargetVersionContent";
        var testFileSourceContent = "SourceVersionContent";
        var pathManagerSource = new PathStructureManager(instanceNameSource);
        var pathManagerTarget = new PathStructureManager(instanceNameTarget);
        var sourceTestFilePath = Path.Combine(pathManagerSource.InstanceBinPath, testFileName);
        var backupTestFilePath = Path.Combine(pathManagerTarget.InstanceBackupBinPath, testFileName);
        var targetTestFilePath = Path.Combine(pathManagerTarget.InstanceBinPath, testFileName);
        var targetOldFilePath = Path.Combine(pathManagerTarget.InstanceBinPath, oldFileName);
        var installationManager = new InstallationManager(new Mock<IReleaseManager>().Object);
        string transferredTestFileContent, backupTestFileContent;
        bool oldFileDeleted;
        try
        {
            Directory.CreateDirectory(pathManagerSource.InstanceBinPath);
            Directory.CreateDirectory(pathManagerTarget.InstanceBinPath);
            await File.WriteAllTextAsync(sourceTestFilePath, testFileSourceContent);
            await File.WriteAllTextAsync(targetTestFilePath, testFileTargetContent);
            await File.WriteAllTextAsync(targetOldFilePath, "OldContent");

            installationManager.TransferInstance(instanceNameSource, instanceNameTarget);

            transferredTestFileContent = await File.ReadAllTextAsync(targetTestFilePath);
            backupTestFileContent = await File.ReadAllTextAsync(backupTestFilePath);
            oldFileDeleted = !File.Exists(targetOldFilePath);
        }
        finally
        {
            Directory.Delete(pathManagerSource.InstancePath, true);
            Directory.Delete(pathManagerTarget.InstanceBackupPath, true);
            Directory.Delete(pathManagerTarget.InstancePath, true);
        }

        Assert.NotNull(transferredTestFileContent);
        Assert.Equal(testFileSourceContent, transferredTestFileContent);
        Assert.Equal(testFileTargetContent, backupTestFileContent);
        Assert.True(oldFileDeleted);
    }

    [Fact]
    public void DeleteInstallationTest()
    {
        var testInstanceName = Guid.NewGuid().ToString();
        var pathManager = new PathStructureManager(testInstanceName);
        var assemblyPath = GetType().Assembly.Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyPath) ?? string.Empty;
        bool testInstallationPathExists, testBackupPathExists;
        try
        {
            Directory.CreateDirectory(pathManager.InstanceBinPath);
            foreach (var file in Directory.GetFiles(assemblyDirectory).Except(new[] { assemblyPath }))
            {
                File.Copy(file, Path.Combine(pathManager.InstanceBinPath, Path.GetFileName(file)));
            }

            Directory.CreateDirectory(pathManager.InstanceBackupBinPath);
            foreach (var file in Directory.GetFiles(assemblyDirectory).Except(new[] { assemblyPath }))
            {
                File.Copy(file, Path.Combine(pathManager.InstanceBackupBinPath, Path.GetFileName(file)));
            }

            _installationManager.DeleteInstance(testInstanceName);
            testInstallationPathExists = Directory.Exists(pathManager.InstanceBinPath);
            testBackupPathExists = Directory.Exists(pathManager.InstanceBackupBinPath);
            // should not throw Exception if directory does not exists...
            _installationManager.DeleteInstance(testInstanceName);
        }
        finally
        {
            Directory.Delete(pathManager.InstancePath, true);
            Directory.Delete(pathManager.InstanceBackupPath, true);
        }

        Assert.False(testInstallationPathExists);
        Assert.False(testBackupPathExists);
    }

    [Fact]
    public void TryDeleteInstanceDirectory()
    {
        var testInstanceName = Guid.NewGuid().ToString();
        var pathManager = new PathStructureManager(testInstanceName);
        var otherAppPath = Path.Combine(pathManager.InstancePath, "OtherApp");
        Directory.CreateDirectory(pathManager.InstancePath);
        Directory.CreateDirectory(otherAppPath);
        Directory.CreateDirectory(pathManager.InstanceLogPath);
        Directory.CreateDirectory(pathManager.InstanceSettingsPath);

        _installationManager.DeleteInstanceDirectory(testInstanceName);
        var instanceDirExistsWithOtherApp = Directory.Exists(pathManager.InstancePath);
            
        Directory.Delete(otherAppPath);
        _installationManager.DeleteInstanceDirectory(testInstanceName);
        var instanceDirExistsWithoutOtherApp = Directory.Exists(pathManager.InstancePath);

        Assert.True(instanceDirExistsWithOtherApp);
        Assert.False(instanceDirExistsWithoutOtherApp);
    }

#pragma warning disable xUnit1004 
    [Fact(Skip = "Only executed manually")]
#pragma warning restore xUnit1004 
    public async Task DoSelfUpdateTest()
    {
        var selfUpdatePath = new PathStructureManager().SelfUpdatePath;
        var testUpdateLogPath = Path.Combine(selfUpdatePath, "update.log");
        var testCustomerId = Guid.NewGuid();
        var testProductId = Guid.NewGuid();
        var testReleaseId = Guid.NewGuid();
        var updateFileContent = $"echo SelfUpdate %1 and %2> \"{testUpdateLogPath}\"";
        var updateFile = new FileMetaInfo
        {
            Id = Guid.NewGuid(),
            Name = "Update.bat"
        };
        var testConfigProductId = Guid.NewGuid();
        var testConfigReleaseId = Guid.NewGuid();
        var configFileContent = "TestConfig";
        var configFile = new FileMetaInfo
        {
            Id = Guid.NewGuid(),
            Name = "config.json"
        };

        var releaseManagerMock = new Mock<IReleaseManager>();
        releaseManagerMock.Setup(m => m.GetReleaseFilesAsync(testCustomerId, testProductId, testReleaseId))
            .ReturnsAsync(new[] { updateFile });
        releaseManagerMock.Setup(m => m.GetFileContentStreamAsync(testCustomerId, updateFile.Id))
            .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(updateFileContent)));
        releaseManagerMock.Setup(m => m.GetReleaseFilesAsync(testCustomerId, testConfigProductId, testConfigReleaseId))
            .ReturnsAsync(new[] { configFile });
        releaseManagerMock.Setup(m => m.GetFileContentStreamAsync(testCustomerId, configFile.Id))
            .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes(configFileContent)));
        var installationManager = new InstallationManager(releaseManagerMock.Object);

        string updateLogContent;
        try
        {
            Directory.CreateDirectory(selfUpdatePath);
            var setup = new InstallationSetup()
            {
                CustomerId = testCustomerId,
                ProductId = testProductId,
                ReleaseId = testReleaseId,
                ConfigurationProductId = testConfigProductId,
                ConfigurationReleaseId = testConfigReleaseId
            };

            await installationManager.DoSelfUpdate(setup);
            await Task.Delay(1000); // wait for finishing batch update.bat...

            updateLogContent = await File.ReadAllTextAsync(testUpdateLogPath);
        }
        finally
        {
            Directory.Delete(selfUpdatePath, true);
        }

        var productPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        Assert.NotNull(updateLogContent);
        Assert.Contains(productPath, updateLogContent);
        Assert.Contains("Settings", updateLogContent);
    }
}