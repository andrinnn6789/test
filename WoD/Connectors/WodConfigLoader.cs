using System.Collections;
using System.Linq;

using IAG.Common.DataLayerSybase;
using IAG.Common.Dto;
using IAG.Common.Resource;
using IAG.Common.WoD.Interfaces;
using IAG.Infrastructure.Exception;

namespace IAG.Common.WoD.Connectors;

public class WodConfigLoader : IWodConfigLoader
{
    private readonly ISybaseConnection _connection;
        
    public WodConfigLoader(ISybaseConnection connection)
    {
        _connection = connection;
    }
 
    public ProviderSetting ProviderSetting()
    {
        var configs = _connection.GetQueryable<ProviderSetting>().Where(p => p.Type == ProviderTypeEnum.WoD).ToList();
        CheckList(configs);
        var config = configs[0];
        if (string.IsNullOrWhiteSpace(config.ParticipantId) || string.IsNullOrWhiteSpace(config.Password) ||
            string.IsNullOrWhiteSpace(config.UserName) || string.IsNullOrWhiteSpace(config.BaseUrl))
            throw new LocalizableException(ResourceIds.WoDLoginDataForIncomplete);
        return config;
    }

    private void CheckList(ICollection list)
    {
        switch (list.Count)
        {
            case 0:
                throw new LocalizableException(ResourceIds.WoDNoConfigFound);
            case 1:
                return;
            default:
                throw new LocalizableException(ResourceIds.WoDTooManyConfigsFound);
        }
    }
}