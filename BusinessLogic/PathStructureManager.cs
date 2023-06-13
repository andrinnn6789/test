using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using IAG.Infrastructure.Settings;

namespace IAG.InstallClient.BusinessLogic;

public class PathStructureManager
{
    // Installation path installer: {root}\Admin\Installer\Bin
    // Temp update path installer:  {root}\Admin\SelfUpdate
    // Backup path:                 {root}\Admin\Backup\Server\{instance}
    // Installation path service:   {root}\Server\{instance}
    private const string PathPartSelfUpdate = "SelfUpdate";
    private const string PathPartBpe = "BPE";
    private const string PathPartLog = "Logs";
    private const string PathPartServer = "Server";
    private const string PathPartBackup = "Backup";

    private string RootPath
    {
        get
        {
            var pathParts = Assembly.GetExecutingAssembly().Location.Split(Path.DirectorySeparatorChar);
            return string.Join(Path.DirectorySeparatorChar, pathParts.Take(pathParts.Length - 4));
        }
    }

    private string AdminPath
    {
        get
        {
            var pathParts = Assembly.GetExecutingAssembly().Location.Split(Path.DirectorySeparatorChar);
            return string.Join(Path.DirectorySeparatorChar, pathParts.Take(pathParts.Length - 3));
        }
    }

    public PathStructureManager()
    {
    }

    public PathStructureManager(string instanceName)
    {
        InstanceName = instanceName;
    }

    private string InstallerPath
    {
        get
        {
            var pathParts = Assembly.GetExecutingAssembly().Location.Split(Path.DirectorySeparatorChar);
            return string.Join(Path.DirectorySeparatorChar, pathParts.Take(pathParts.Length - 2));
        }
    }

    public IEnumerable<string> GetInstanceNames()
    {
        var serverPath = Path.Combine(RootPath, PathPartServer);
        return !Directory.Exists(serverPath) 
            ? Enumerable.Empty<string>() 
            : Directory.GetDirectories(serverPath).Select(d => d.Substring(serverPath.Length + 1));
    }

    public string InstanceName { get; set; }

    public string SelfUpdatePath => Path.Combine(InstallerPath, PathPartSelfUpdate);
        
    public string InstallerSettingsPath => new SettingsFinder().GetSettingsPath();

    public string InstancePath => Path.Combine(RootPath, PathPartServer, InstanceName);

    public string InstanceBinPath => Path.Combine(InstancePath, PathPartBpe);
        
    public string InstanceSettingsPath => Path.Combine(InstancePath, SettingsConst.SettingsPathName);
        
    public string InstanceLogPath => Path.Combine(InstancePath, PathPartLog);
        
    public string InstanceBackupPath => Path.Combine(AdminPath, PathPartBackup, PathPartServer, InstanceName);
        
    public string InstanceBackupSettingsPath => Path.Combine(InstanceBackupPath, SettingsConst.SettingsPathName);
        
    public string InstanceBackupBinPath => Path.Combine(InstanceBackupPath, PathPartBpe);
}