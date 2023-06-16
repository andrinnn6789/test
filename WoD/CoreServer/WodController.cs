using System.Threading.Tasks;
using IAG.Common.WoD.Dto;
using IAG.Common.WoD.Interfaces;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.Globalisation.Localizer;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace IAG.Common.WoD.CoreServer;

[Route(ControllerEndpoints.Wod)]
[ApiExplorerSettings(GroupName = ApiExplorerDefaults.DefaultGroup)]
public class WodController : ControllerBase
{
    private readonly IWodConnector _wodConnector;
    private readonly IStringLocalizer<WodController> _localizer;

    public WodController([FromServices] IWodConnector wodConnector, IStringLocalizer<WodController> localizer)
    {
        _wodConnector = wodConnector;
        _localizer = localizer;
    }

    [HttpPost("CheckWodConnection")]
    public async Task<ActionResult<WodConnectionResult<string>>> CheckWodConnectionAsync()
    {
        return  LocalizeWodConnectionResult(await _wodConnector.CheckWodConnectionAsync());
    }

    private WodConnectionResult<string> LocalizeWodConnectionResult(WodConnectionResult<LocalizableParameter> resultLocalizable)
    {
        return new()
        {
            Success = resultLocalizable.Success,
            Info = _localizer.GetString(resultLocalizable.Info.ResourceId, resultLocalizable.Info.Params)
        };
    }
}