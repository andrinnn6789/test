using System.Threading.Tasks;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Controllers;
using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Model;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IAG.IdentityServer.Controller;

[Route(InfrastructureEndpoints.Auth + "[controller]")]
public class RealmController : ControllerBase
{
    private readonly IRealmHandler _realmHandler;

    public RealmController(IRealmHandler realmHandler)
    {
        _realmHandler = realmHandler;
    }

    [AllowAnonymous]
    [HttpPost("RequestToken")]
    public async Task<ActionResult<RequestTokenResponse>> RequestToken(
        [FromBody]RequestTokenParameter parameter, 
        [FromServices]IAttackDetection attackDetection, 
        [FromServices]ITokenHandler tokenHandler)
    {
        return await _realmHandler.RequestToken(parameter, attackDetection, tokenHandler);
    }

    [AllowAnonymous]
    [HttpPost("CheckToken")]
    public IActionResult CheckToken(
        [FromBody]CheckTokenParameter parameter,
        [FromServices]ITokenHandler tokenHandler)
    {
        return _realmHandler.CheckToken(parameter, tokenHandler);
    }

    [HttpPost("ChangePassword")]
    public async Task<IActionResult> ChangePassword(
        [FromBody]ChangePasswordParameter parameter,
        [FromServices]IUserContext userContext,
        [FromServices]IAttackDetection attackDetection,
        [FromServices]IPasswordChecker passwordChecker)
    {
        return await _realmHandler.ChangePassword(parameter, userContext, attackDetection, passwordChecker);
    }

    [AllowAnonymous]
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordParameter parameter,
        [FromServices]IAttackDetection attackDetection,
        [FromServices] ITemplateHandler templateHandler,
        [FromServices] IPasswordGenerator passwordGenerator,
        [FromServices] IMailSender mailSender)
    {
        return await _realmHandler.ResetPassword(parameter, attackDetection, templateHandler, passwordGenerator, mailSender);
    }
}