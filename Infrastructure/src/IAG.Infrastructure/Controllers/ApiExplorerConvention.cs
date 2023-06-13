using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace IAG.Infrastructure.Controllers;

public static class ApiExplorerDefaults
{
    public const string DefaultGroup = "Base";
    public const string AppGroup = "App";
    public const string AppGroupV20 = "AppV20";
    public const string ControlCenter = "ControlCenter";
}

public class ApiExplorerDefaultGroupConvention : IApplicationModelConvention
{
    private readonly string _name;

    public ApiExplorerDefaultGroupConvention(string name)
    {
        _name = name;
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            if (!string.IsNullOrWhiteSpace(controller.ApiExplorer.GroupName)) 
                continue;
            controller.ApiExplorer.GroupName = _name;
        }
    }
}