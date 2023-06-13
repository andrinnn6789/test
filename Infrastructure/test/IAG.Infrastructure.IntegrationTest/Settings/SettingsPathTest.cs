using System;
using System.IO;

using IAG.Infrastructure.Settings;
using IAG.Infrastructure.TestHelper.xUnit;

using Xunit;

namespace IAG.Infrastructure.IntegrationTest.Settings;

public class SettingsPathTest
{
    private const string SettingsFile = "TestSettings.json";

    [Fact, TestPriority(1)]
    public void TestSettingsCurrentDir()
    {
        File.WriteAllText(SettingsFile, "");
        try
        {
            var settingsFinder = new SettingsFinder();
            Assert.Equal(".", settingsFinder.GetSettingsPath(SettingsFile));
            Assert.Equal(SettingsFile, settingsFinder.GetSettingsFilePath(SettingsFile));
        }
        finally
        {
            File.Delete(SettingsFile);
        }
    }

    [Fact, TestPriority(2)]
    public void TestSettingsSubDir()
    {
        const string subDir = "TestSettings";
        var settingsFilePath = Path.Combine(subDir, SettingsFile);
        if (!Directory.Exists(subDir))
        {
            Directory.CreateDirectory(subDir);
        }
        File.WriteAllText(settingsFilePath, "");

        try
        {
            var settingsFinder = new SettingsFinder();
            Assert.Equal(subDir, settingsFinder.GetSettingsPath(SettingsFile, subDir));
            Assert.Equal(Path.GetFullPath(Path.Combine(subDir, SettingsFile)), settingsFinder.GetSettingsFilePath(SettingsFile));
        }
        finally
        {
            File.Delete(settingsFilePath);
            Directory.Delete(subDir);
        }
    }

    [Fact, TestPriority(3)]
    public void TestSettingsParentDir()
    {
        const string settingsPath = "TestSettings";
        var currentDirParentPath = Directory.GetParent(Environment.CurrentDirectory)?.FullName ?? string.Empty;
        var parentDir = Path.Combine(currentDirParentPath, settingsPath);
        var parentDirSub = Path.Combine(currentDirParentPath, settingsPath, new DirectoryInfo(Environment.CurrentDirectory).Name);
        var settingsFilePath = Path.Combine(parentDir, SettingsFile);
        if (!Directory.Exists(parentDir))
        {
            Directory.CreateDirectory(parentDir);
        }
        if (!Directory.Exists(parentDirSub))
        {
            Directory.CreateDirectory(parentDirSub);
        }
        File.WriteAllText(settingsFilePath, "");

        try
        {
            var settingsFinder = new SettingsFinder();
            Assert.Equal(Path.Combine(Path.GetDirectoryName(Path.GetRelativePath(Environment.CurrentDirectory, parentDir)) ?? string.Empty, settingsPath), 
                settingsFinder.GetSettingsPath(SettingsFile, "TestSettings"));
            Assert.Equal(Path.GetFullPath(Path.Combine(parentDir, SettingsFile)), settingsFinder.GetSettingsFilePath(SettingsFile, settingsPath));
        }
        finally
        {
            File.Delete(settingsFilePath);
            Directory.Delete(parentDirSub);
            Directory.Delete(parentDir);
        }
    }
}