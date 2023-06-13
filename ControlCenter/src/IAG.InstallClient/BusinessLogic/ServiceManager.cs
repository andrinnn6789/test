using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Management;
using System.ServiceProcess;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.InstallClient.BusinessLogic.Model;
using IAG.InstallClient.Resource;

namespace IAG.InstallClient.BusinessLogic;

[ExcludeFromCodeCoverage]   // tests won't work on Linux build server...
public class ServiceManager : IServiceManager
{
    private const string ServiceNamePattern = "{0} Business Process Engine {1}";    // {0}=product name   {1}=instance name

    public string GetServiceName(string instanceName)
    {
        var pathManager = new PathStructureManager(instanceName);
        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Service");
        var service = searcher
            .Get()
            .Cast<ManagementBaseObject>()
            .FirstOrDefault(x => x["PathName"]?.ToString()?.StartsWith(pathManager.InstanceBinPath) == true);

        return service?["Name"]?.ToString();
    }

    public string InstallService(string productName, string instanceName)
    {
        var pathManager = new PathStructureManager(instanceName);
        var serviceName = GetServiceName(pathManager.InstanceBinPath);
        if (!string.IsNullOrEmpty(serviceName))
        {
            return serviceName;
        }

        var exeFiles = Directory.GetFiles(pathManager.InstanceBinPath, "IAG.*.exe");
        if (exeFiles.Length == 0)
        {
            throw new LocalizableException(ResourceIds.InstallServiceNoExeError);
        }
        if (exeFiles.Length > 1)
        {
            throw new LocalizableException(ResourceIds.InstallServiceTooMuchExesError);
        }

        serviceName = string.Format(ServiceNamePattern, productName, instanceName);
        ExecServiceControl("create", serviceName, $"binPath= \"{exeFiles[0]}\" start=delayed-auto");

        return serviceName;
    }

    public void UninstallService(string serviceName)
    {
        if (GetWindowsServiceState(serviceName) != ServiceControllerStatus.Stopped)
        {
            StopService(serviceName);
        }
        ExecServiceControl("delete", serviceName);
    }

    public ServiceStatus? GetServiceState(string serviceName)
    {

        return (ServiceStatus?)GetWindowsServiceState(serviceName);
    }

    public void StartService(string serviceName)
    {
        try
        {
            GetService(serviceName)?.Start();
        }
        catch (Exception)
        {
            ExecServiceControl("start", serviceName);
        }
    }

    public void StopService(string serviceName)
    {
        try
        {
            GetService(serviceName)?.Stop();
        }
        catch (Exception)
        {
            ExecServiceControl("stop", serviceName);
        }
    }

    private static ServiceController GetService(string serviceName)
    {
        return ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
    }

    private void ExecServiceControl(string command, string serviceName, string additionalParameters = null)
    {
        additionalParameters = additionalParameters == null ? string.Empty : " " + additionalParameters;
        var serviceControlProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "sc.exe",
                Arguments = $"{command} \"{serviceName}\"{additionalParameters}",
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = true
            }
        };

        serviceControlProcess.Start();
        serviceControlProcess.WaitForExit();
        if (serviceControlProcess.ExitCode != 0)
        {
            var messageResourceId = ResourceIds.ServiceControlErrorPrefix + char.ToUpperInvariant(command[0]) + command[1..];
            var reasonParameter = ResourceIds.ServiceControlErrorCodes.Contains(serviceControlProcess.ExitCode)
                ? new LocalizableParameter(ResourceIds.ServiceControlErrorCode+serviceControlProcess.ExitCode)
                : new LocalizableParameter(ResourceIds.ServiceControlErrorCodeOther, serviceControlProcess.ExitCode);

            throw new LocalizableException(messageResourceId, reasonParameter);
        }
    }

    private static ServiceControllerStatus? GetWindowsServiceState(string serviceName)
    {
        return GetService(serviceName)?.Status;
    }
}