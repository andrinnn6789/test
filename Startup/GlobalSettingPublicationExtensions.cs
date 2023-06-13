using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using IAG.Infrastructure.Settings;
using IAG.InstallClient.BusinessLogic;

namespace IAG.InstallClient.Startup;

[ExcludeFromCodeCoverage]
public static class GlobalSettingPublicationExtensions
{
    public static IApplicationBuilder PublishGlobalSettings(this IApplicationBuilder app)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        var globalSettings = configuration.GetSection(SettingsConst.GlobalSettingsKey);
        var environmentVars = new List<Tuple<string, string>>();

        foreach (var setting in globalSettings?.GetChildren() ?? Enumerable.Empty<IConfigurationSection>())
        {
            environmentVars.Add(new Tuple<string, string>(setting.Key, setting.Value));
        }

        var customerManager = app.ApplicationServices.GetRequiredService<ICustomerManager>();
        var customerInfo = customerManager.GetCurrentCustomerInformationAsync().Result;
        if (customerInfo != null)
        {
            environmentVars.Add(new Tuple<string, string>(SettingsConst.CustomerName, customerInfo.CustomerName));
            environmentVars.Add(new Tuple<string, string>(SettingsConst.CustomerIdentification, customerInfo.Id.ToString()));
        }


        foreach (var (key, value) in environmentVars)
        {
            foreach (var target in Enum.GetValues<EnvironmentVariableTarget>())
            {
                Environment.SetEnvironmentVariable(key, value, target);
            }
        }

        return app;
    }
}