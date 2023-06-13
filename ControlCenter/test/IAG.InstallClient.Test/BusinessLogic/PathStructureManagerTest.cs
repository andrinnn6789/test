using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using IAG.InstallClient.BusinessLogic;

using Xunit;

namespace IAG.InstallClient.Test.BusinessLogic;

public class PathStructureManagerTest
{
    [Fact]
    public void GetInstancesTest()
    {
        var pathManager = new PathStructureManager(Guid.NewGuid().ToString());
        IList<string> testProductInstanceNames;
        try
        {
            Directory.CreateDirectory(pathManager.InstancePath);
            testProductInstanceNames = pathManager.GetInstanceNames().ToList();
        }
        finally
        {
            Directory.Delete(pathManager.InstancePath, true);
        }

        Assert.NotEmpty(testProductInstanceNames);
        Assert.Contains(pathManager.InstanceName, testProductInstanceNames);
    }

    [Fact]
    public void GetInstallationBackupPathTest()
    {
        var pathManager = new PathStructureManager(Guid.NewGuid().ToString());
        var installerPathParts = Assembly.GetExecutingAssembly().Location.Split(Path.DirectorySeparatorChar);
        var adminPath = string.Join(Path.DirectorySeparatorChar, installerPathParts.Take(installerPathParts.Length - 3));
        var backupPath = pathManager.InstanceBackupPath;
        var expectedBackupPath = Path.Combine(adminPath, "Backup", "Server", pathManager.InstanceName);

        Assert.NotEmpty(backupPath);
        Assert.Equal(expectedBackupPath, backupPath);
    }

    [Fact]
    public void GetInstallerPathsTest()
    {
        var pathManager = new PathStructureManager(Guid.NewGuid().ToString());
        var selfUpdatePath = pathManager.SelfUpdatePath;
        var installerSettingsPath = pathManager.InstallerSettingsPath;

        Assert.NotEmpty(selfUpdatePath);
        Assert.NotEmpty(installerSettingsPath);
    }
}