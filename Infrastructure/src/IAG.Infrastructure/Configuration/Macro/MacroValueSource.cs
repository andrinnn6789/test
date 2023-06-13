using System.Linq;

using IAG.Infrastructure.Configuration.Context;
using IAG.Infrastructure.Settings;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;

namespace IAG.Infrastructure.Configuration.Macro;

public class MacroValueSource : IMacroValueSource
{
    private readonly ConfigCommonDbContext _configCommonDbContext;
    private readonly IConfigurationProvider _environmentVariablesProvider;
    private readonly IConfigurationProvider _appSettingsProvider;

    public MacroValueSource(ConfigCommonDbContext configCommonDbContext, IConfigurationRoot configurationRoot)
    {
        _configCommonDbContext = configCommonDbContext;
        _environmentVariablesProvider = configurationRoot?.Providers.FirstOrDefault(p => p.GetType().IsAssignableFrom(typeof(EnvironmentVariablesConfigurationProvider)));
        _appSettingsProvider = configurationRoot?.Providers.FirstOrDefault(p => p.GetType().IsAssignableFrom(typeof(JsonConfigurationProvider)));
    }

    public string GetValue(string placeholder)
    {
        var value = _configCommonDbContext.ConfigCommonEntries.AsNoTracking().FirstOrDefault(c => c.Name == placeholder)?.Data;
        if (value == null && !(_environmentVariablesProvider?.TryGet(placeholder, out value) ?? false))
        {
            _appSettingsProvider?.TryGet($"{SettingsConst.ApplicationSettingsKey}:{placeholder}", out value);
        }

        return value;
    }
}