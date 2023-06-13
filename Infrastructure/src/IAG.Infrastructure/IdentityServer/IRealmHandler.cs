using System.Collections.Generic;
using System.Threading.Tasks;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.AspNetCore.Mvc;

namespace IAG.Infrastructure.IdentityServer;

public interface IRealmHandler
{
    Task<ActionResult<RequestTokenResponse>> RequestToken(
        RequestTokenParameter parameter, IAttackDetection attackDetection, ITokenHandler tokenHandler);

    IActionResult CheckToken(CheckTokenParameter parameter, ITokenHandler tokenHandler);

    Task<IActionResult> ChangePassword(ChangePasswordParameter parameter, IUserContext userContext,
        IAttackDetection attackDetection, IPasswordChecker passwordChecker);

    Task<IActionResult> ResetPassword(ResetPasswordParameter parameter, IAttackDetection attackDetection,
        ITemplateHandler templateHandler, IPasswordGenerator passwordGenerator, IMailSender mailSender);

    List<IAuthenticationPlugin> GetAuthPlugins();
}