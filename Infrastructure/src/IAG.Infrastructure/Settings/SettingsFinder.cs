using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IAG.Infrastructure.Settings;

public class SettingsFinder
{
    private const string AppsettingsJson = "appsettings.json";

    public string GetSettingsFilePath(string configFile = AppsettingsJson, string settingsPathName = SettingsConst.SettingsPathName)
    {
        configFile ??= AppsettingsJson;
        string configFilePath = null;
        if (!File.Exists(configFile))
        {
            var currentDirParentPath = Directory.GetParent(Environment.CurrentDirectory)?.FullName ?? string.Empty;
            var priorities = new List<string>
            {
                Path.Combine(currentDirParentPath, settingsPathName, new DirectoryInfo(Environment.CurrentDirectory).Name),
                Path.Combine(currentDirParentPath, settingsPathName)
            };

            var found = false;
            foreach (var priority in priorities.Where(Directory.Exists))
            {
                configFilePath = Directory.GetFiles(priority, configFile, SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (configFilePath != null)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                configFilePath = Directory.GetFiles(Environment.CurrentDirectory, configFile, SearchOption.AllDirectories).FirstOrDefault();
        }

        return configFilePath ?? configFile;
    }

    public string GetSettingsPath(string configFile = AppsettingsJson, string settingsPathName = SettingsConst.SettingsPathName)
    {
        var settingsFilePath = GetSettingsFilePath(configFile, settingsPathName);
        var settingsPath = Path.GetDirectoryName(Path.GetRelativePath(Environment.CurrentDirectory, settingsFilePath));

        return string.IsNullOrEmpty(settingsPath) ? "." : settingsPath;
    }
}