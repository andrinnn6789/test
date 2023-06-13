using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.IdentityServer.Resource;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Localizer;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;
using IAG.Infrastructure.Startup.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace IAG.IdentityServer.Authentication;

public class RealmHandler : IRealmHandler
{
    private readonly IRealmCatalogue _catalogue;
    private readonly IPluginCatalogue _pluginCatalogue;
    private readonly IRefreshTokenManager _refreshTokenManager;
    private readonly IStringLocalizer _localizer;

    public RealmHandler(IRealmCatalogue catalogue, IPluginCatalogue pluginCatalogue, IRefreshTokenManager refreshTokenManager, IStringLocalizer<RealmHandler> localizer)
    {
        _catalogue = catalogue;
        _pluginCatalogue = pluginCatalogue;
        _refreshTokenManager = refreshTokenManager;
        _localizer = localizer;
    }

    public async Task<ActionResult<RequestTokenResponse>> RequestToken(
        RequestTokenParameter parameter, IAttackDetection attackDetection, ITokenHandler tokenHandler)
    {
        if (parameter == null)
        {
            return new BadRequestResult();
        }

        IAuthenticationToken authenticationToken;
        if (parameter.GrantType == GrantTypes.RefreshToken)
        {
            authenticationToken = await _refreshTokenManager.CheckRefreshTokenAsync(parameter.RefreshToken);
            if (authenticationToken == null)
            {
                var msg = _localizer.GetString(ResourceIds.RefreshTokenException);
                return new ObjectResult(msg)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }
            return AuthorizationTokenToResponse(tokenHandler, authenticationToken);
        }

        var authenticationPlugin = FindAuthenticationPlugin(parameter.Realm, out IRealmConfig _, out ActionResult actionResult);
        if (authenticationPlugin == null)
        {
            return actionResult;
        }

        if (!await attackDetection.CheckRequest(parameter.Realm, parameter.Username, "CheckPassword"))
        {
            string msg = _localizer.GetString(ResourceIds.AttackDetectionWarning);
            return new ObjectResult(msg)
            {
                StatusCode = (int) HttpStatusCode.TooManyRequests
            };
        }

        try
        {
            authenticationToken = authenticationPlugin.Authenticate(parameter);
        }
        catch (Exception ex)
        {
            await attackDetection.AddFailedRequest(parameter.Realm, parameter.Username, "CheckPassword");
            var msg = new MessageLocalizer(_localizer).LocalizeException(ex);
            return new ObjectResult(msg)
            {
                StatusCode = (int) HttpStatusCode.Forbidden
            };
        }

        authenticationToken.Username ??= parameter.Username;
        authenticationToken.Tenant ??= parameter.TenantId;
        authenticationToken.ValidFor ??= authenticationPlugin.Config?.ValidityDuration;
        authenticationToken.RefreshToken ??= await _refreshTokenManager.CreateRefreshTokenAsync(authenticationToken);

        return AuthorizationTokenToResponse(tokenHandler, authenticationToken);
    }

    public IActionResult CheckToken(CheckTokenParameter parameter, ITokenHandler tokenHandler)
    {
        if (parameter == null || string.IsNullOrEmpty(parameter.Token))
        {
            return new BadRequestResult();
        }

        try
        {
            tokenHandler.CheckJwtToken(parameter.Token, AuthenticationExtensions.Issuer);
        }
        catch (Exception ex)
        {
            var msg = new MessageLocalizer(_localizer).LocalizeException(ex);

            return new ObjectResult(msg)
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }

        return new OkResult();
    }

    public async Task<IActionResult> ChangePassword(ChangePasswordParameter parameter, IUserContext userContext,
        IAttackDetection attackDetection, IPasswordChecker passwordChecker)
    {
        if (parameter == null)
        {
            return new BadRequestResult();
        }

        var authenticationPlugin = FindAuthenticationPlugin(parameter.Realm, out var realmConfig, out ActionResult actionResult);
        if (authenticationPlugin == null)
        {
            // ReSharper disable once StyleCop.SA1126
            return actionResult;
        }

        if (!await attackDetection.CheckRequest(parameter.Realm, userContext.UserName, "CheckPassword"))
        {
            var msg = _localizer.GetString(ResourceIds.AttackDetectionWarning);

            return new ObjectResult(msg)
            {
                StatusCode = (int)HttpStatusCode.TooManyRequests
            };
        }

        try
        {
            var authenticationParameter = RequestTokenParameter.ForPasswordGrant(userContext.UserName, parameter.OldPassword, userContext.TenantId);
            authenticationPlugin.Authenticate(authenticationParameter);
        }
        catch (Exception ex)
        {
            await attackDetection.AddFailedRequest(parameter.Realm, userContext.UserName, "CheckPassword");

            var msg = new MessageLocalizer(_localizer).LocalizeException(ex);

            return new ObjectResult(msg)
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }

        if (realmConfig.PasswordPolicy != null && !passwordChecker.IsValidPassword(parameter.NewPassword, realmConfig.PasswordPolicy))
        {
            var messageLocalizer = new MessageLocalizer(_localizer);
            var msg = messageLocalizer.Localize(new MessageStructure(MessageTypeEnum.Error, ResourceIds.ChangePasswordErrorPolicyViolation)).Text;
            foreach (var msgStruct in passwordChecker.PasswordPolicyMessages)
            {
                msg += "\\n" + messageLocalizer.Localize(msgStruct).Text;
            }

            return new ObjectResult(msg)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        authenticationPlugin.ChangePassword(userContext.UserName, userContext.TenantId, parameter.NewPassword, false);

        return new NoContentResult();
    }

    public async Task<IActionResult> ResetPassword(ResetPasswordParameter parameter, IAttackDetection attackDetection,
        ITemplateHandler templateHandler, IPasswordGenerator passwordGenerator, IMailSender mailSender)
    {
        if (parameter == null)
        {
            return new BadRequestResult();
        }

        var authenticationPlugin = FindAuthenticationPlugin(parameter.Realm, out IRealmConfig realmConfig, out ActionResult actionResult);
        if (authenticationPlugin == null)
        {
            // ReSharper disable once StyleCop.SA1126
            return actionResult;
        }

        if (!await attackDetection.CheckRequest(parameter.Realm, parameter.User, "ResetPassword"))
        {
            var msg = _localizer.GetString(ResourceIds.AttackDetectionWarning);

            return new ObjectResult(msg)
            {
                StatusCode = (int)HttpStatusCode.TooManyRequests
            };
        }

        try
        {
            var email = authenticationPlugin.GetEMail(parameter.User, parameter.TenantId);
            if (string.IsNullOrEmpty(email))
            {
                throw new LocalizableException(ResourceIds.ResetPasswordErrorNoEMail);
            }

            var config = realmConfig.ResetPasswordMailTemplateConfig?.FirstOrDefault(c => string.Equals(c.Language, parameter.UserLanguage, StringComparison.InvariantCultureIgnoreCase));
            config ??= realmConfig.ResetPasswordMailTemplateConfig?.FirstOrDefault();

            if (config == null)
            {
                throw new LocalizableException(ResourceIds.MailConfigErrorTemplate, parameter.Realm);
            }

            string newPassword = passwordGenerator.GenerateRandomPassword();
            authenticationPlugin.ChangePassword(parameter.User, parameter.TenantId, newPassword, true);

            var replacement = new Dictionary<string, string> { { "password", newPassword } };
            var message = templateHandler.GetMessage(config, replacement);

            message.To.Add(email);
            mailSender.Send(message);

            // add "failed" request to attackDetection
            await attackDetection.AddFailedRequest(parameter.Realm, parameter.User, "ResetPassword");
        }
        catch (Exception ex)
        {
            string msg = new MessageLocalizer(_localizer).LocalizeException(ex);

            return new ObjectResult(msg)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        return new NoContentResult();
    }

    public List<IAuthenticationPlugin> GetAuthPlugins()
    {
        return _catalogue.Realms
            .Where(r => r.AuthenticationPluginConfig.Active)
            .Select(realm => FindAuthenticationPlugin(realm.Realm, out _, out _))
            .ToList();
    }

    private IAuthenticationPlugin FindAuthenticationPlugin(string id, out IRealmConfig realmConfig, out ActionResult actionResult)
    {
        if (string.IsNullOrEmpty(id))
        {
            realmConfig = null;
            actionResult = new BadRequestResult();
            return null;
        }

        realmConfig = _catalogue.GetRealm(id);
        if (realmConfig == null)
        {
            actionResult = new NotFoundObjectResult($"realm '{id}'");
            return null;
        }

        var authenticationPlugin = _pluginCatalogue.GetAuthenticationPlugin(realmConfig.AuthenticationPluginId);
        if (authenticationPlugin == null)
        {
            actionResult = new NotFoundObjectResult($"auth-plugin '{realmConfig.AuthenticationPluginId}'");
            return null;
        }

        actionResult = null;
        authenticationPlugin.Config = realmConfig.AuthenticationPluginConfig;
        return authenticationPlugin;
    }

    private static ActionResult<RequestTokenResponse> AuthorizationTokenToResponse(ITokenHandler tokenHandler, IAuthenticationToken authenticationToken)
    {
        var response = new RequestTokenResponse
        {
            AccessToken = tokenHandler.GetJwtToken(authenticationToken, AuthenticationExtensions.Issuer),
            RefreshToken = authenticationToken.RefreshToken,
            Username = authenticationToken.Username,
            UserLanguage = authenticationToken.UserLanguage,
            UserRole = authenticationToken.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
            Email = authenticationToken.Email,
            UserShouldChangePassword = authenticationToken.UserShouldChangePassword
        };

        return response;
    }
}