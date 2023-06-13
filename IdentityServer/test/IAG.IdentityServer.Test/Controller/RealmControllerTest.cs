using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

using IAG.IdentityServer.Authentication;
using IAG.IdentityServer.Configuration;
using IAG.IdentityServer.Configuration.Model.Realm;
using IAG.IdentityServer.Controller;
using IAG.IdentityServer.Test.Configuration.Mock;
using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.DI;
using IAG.Infrastructure.Exception.HttpException;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Authentication;
using IAG.Infrastructure.IdentityServer.Model;
using IAG.Infrastructure.IdentityServer.Plugin;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

using Moq;

using Xunit;

namespace IAG.IdentityServer.Test.Controller;

public class RealmControllerTest
{
    private string _password;
    private string _email;
    private MailMessage _sentMessage;
    private IRealmConfig _realmConfig;
    private List<IRealmConfig> _realms;
    private List<IPluginMetadata> _plugins;
    private Mock<ITokenHandler> _tokenHandler;
    private Mock<ITemplateHandler> _templateHandler;
    private Mock<IPasswordGenerator> _passwordGenerator;
    private Mock<IPasswordChecker> _passwordChecker;
    private Mock<IMailSender> _mailSender;

    private readonly IUserContext _userContext = new ExplicitUserContext("user", null);
    private readonly AttackDetectionDummy _attackDetection = new();
    private readonly RealmController _controller;

    public RealmControllerTest()
    {
        InitMocks();
        _controller = CreateRealmController();
    }

    [Fact]
    public async void RequestTokenOkTest()
    {
        var requestParam = RequestTokenParameter.ForPasswordGrant("user", "TopSecret").ForRealm(_realmConfig.Realm);

        var result = await _controller.RequestToken(requestParam, _attackDetection, _tokenHandler.Object);

        Assert.NotNull(result);
        Assert.Equal("JwtToken", Assert.IsAssignableFrom<RequestTokenResponse>(result.Value).AccessToken);
    }

    [Fact]
    public async void RequestTokenWrongPasswordTest()
    {
        var requestParam = RequestTokenParameter.ForPasswordGrant("user", "WrongPassword").ForRealm(_realmConfig.Realm);

        var result = await _controller.RequestToken(requestParam, _attackDetection, _tokenHandler.Object);

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.Forbidden, Assert.IsAssignableFrom<ObjectResult>(result.Result).StatusCode);
    }

    [Fact]
    public async void RequestTokenAttackDetectionFailedTest()
    {
        var requestParam = RequestTokenParameter.ForPasswordGrant("user", "TopSecret").ForRealm(_realmConfig.Realm);

        _attackDetection.ShouldFail = true;

        var result = await _controller.RequestToken(requestParam, _attackDetection, _tokenHandler.Object);

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.TooManyRequests, Assert.IsAssignableFrom<ObjectResult>(result.Result).StatusCode);
    }

    [Fact]
    public async void RequestTokenBadRequestsTest()
    {
        var resultNoParam = await _controller.RequestToken(null, _attackDetection, _tokenHandler.Object);
        var resultNoRealm = await _controller.RequestToken(new RequestTokenParameter(), _attackDetection, _tokenHandler.Object);

        Assert.NotNull(resultNoParam);
        Assert.NotNull(resultNoRealm);
        Assert.Null(resultNoParam.Value);
        Assert.Null(resultNoRealm.Value);
        Assert.IsAssignableFrom<BadRequestResult>(resultNoParam.Result);
        Assert.IsAssignableFrom<BadRequestResult>(resultNoRealm.Result);
    }

    [Fact]
    public async void RequestTokenRealmNotFoundTest()
    {
        var result = await _controller.RequestToken(new RequestTokenParameter().ForRealm("UnknownRealm"), _attackDetection, _tokenHandler.Object);

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NotFound, Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result).StatusCode);
        Assert.Contains("UnknownRealm", Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result).Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public async void RequestTokenAuthPluginNotFoundTest()
    {
        _plugins.Clear();

        var result = await _controller.RequestToken(new RequestTokenParameter().ForRealm(_realmConfig.Realm), _attackDetection, _tokenHandler.Object);

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.NotFound, Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result).StatusCode);
        Assert.Contains(_realmConfig.AuthenticationPluginId.ToString(), Assert.IsAssignableFrom<NotFoundObjectResult>(result.Result).Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public void CheckTokenOkTest()
    {
        var requestParam = new CheckTokenParameter() { Token = "JwtToken" };

        var result = _controller.CheckToken(requestParam, _tokenHandler.Object);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<OkResult>(result);
    }

    [Fact]
    public void CheckTokenBadRequestsTest()
    {
        var resultNoParam = _controller.CheckToken(null, _tokenHandler.Object);

        Assert.NotNull(resultNoParam);
        Assert.IsAssignableFrom<BadRequestResult>(resultNoParam);
    }

    [Fact]
    public void CheckTokenFailedTest()
    {
        var requestParam = new CheckTokenParameter() { Token = "BadToken" };

        var result = _controller.CheckToken(requestParam, _tokenHandler.Object) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async void ChangePasswordOkTest()
    {
        var requestParam = new ChangePasswordParameter() { Realm = _realmConfig.Realm, OldPassword = "TopSecret", NewPassword = "NoLongerSecret" };

        var result = await _controller.ChangePassword(requestParam, _userContext, _attackDetection, _passwordChecker.Object);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.Equal(requestParam.NewPassword, _password);
    }

    [Fact]
    public async void ChangePasswordPolicyViolationTest()
    {
        var requestParam = new ChangePasswordParameter() { Realm = _realmConfig.Realm, OldPassword = "TopSecret", NewPassword = "TooEasy" };

        var result = await _controller.ChangePassword(requestParam, _userContext, _attackDetection, _passwordChecker.Object) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [Fact]
    public async void ChangePasswordAuthFailedTest()
    {
        var requestParam = new ChangePasswordParameter() { Realm = _realmConfig.Realm, OldPassword = "WrongPassword", NewPassword = "NoLongerSecret" };

        var result = await _controller.ChangePassword(requestParam, _userContext, _attackDetection, _passwordChecker.Object) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.Forbidden, result.StatusCode);
    }

    [Fact]
    public async void ChangePasswordAttackDetectionFailedTest()
    {
        var requestParam = new ChangePasswordParameter() { Realm = _realmConfig.Realm, OldPassword = "WrongPassword", NewPassword = "NoLongerSecret" };

        _attackDetection.ShouldFail = true;

        var result = await _controller.ChangePassword(requestParam, _userContext, _attackDetection, _passwordChecker.Object) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.TooManyRequests, result.StatusCode);
    }

    [Fact]
    public async void ChangePasswordBadRequestsTest()
    {
        var resultNoParam = await _controller.ChangePassword(null, _userContext, _attackDetection, _passwordChecker.Object);
        var resultNoRealm = await _controller.ChangePassword(new ChangePasswordParameter(), _userContext, _attackDetection, _passwordChecker.Object);

        Assert.NotNull(resultNoParam);
        Assert.NotNull(resultNoRealm);
        Assert.IsAssignableFrom<BadRequestResult>(resultNoParam);
        Assert.IsAssignableFrom<BadRequestResult>(resultNoRealm);
    }

    [Fact]
    public async void ChangePasswordRealmNotFoundTest()
    {
        var result = await _controller.ChangePassword(new ChangePasswordParameter() {Realm = "UnknownRealm"}, _userContext, _attackDetection, _passwordChecker.Object) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Contains("UnknownRealm", Assert.IsAssignableFrom<string>(result.Value));
    }

    [Fact]
    public async void ChangePasswordAuthPluginNotFoundTest()
    {
        _plugins.Clear();

        var result = await _controller.ChangePassword(new ChangePasswordParameter() { Realm = _realmConfig.Realm }, _userContext, _attackDetection, _passwordChecker.Object) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Contains(_realmConfig.AuthenticationPluginId.ToString(), Assert.IsAssignableFrom<string>(result.Value));
    }

    [Fact]
    public async void ResetPasswordOkTest()
    {
        var requestParam = new ResetPasswordParameter() { Realm = _realmConfig.Realm, User = "user", UserLanguage = "de" };

        var result = await _controller.ResetPassword(requestParam, _attackDetection, _templateHandler.Object, _passwordGenerator.Object, _mailSender.Object);

        Assert.NotNull(result);
        Assert.IsAssignableFrom<NoContentResult>(result);
        Assert.Equal("NewRandomPassword", _password);
        Assert.NotNull(_sentMessage);
        Assert.Equal(_email, _sentMessage.To.Single().Address);
    }

    [Fact]
    public async void ResetPasswordNoMailTest()
    {
        var requestParam = new ResetPasswordParameter() { Realm = _realmConfig.Realm, User = "user", UserLanguage = "de" };
        _email = null;

        var result = await _controller.ResetPassword(requestParam, _attackDetection, _templateHandler.Object, _passwordGenerator.Object, _mailSender.Object) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [Fact]
    public async void ResetPasswordNoConfigTest()
    {
        var requestParam = new ResetPasswordParameter() { Realm = _realmConfig.Realm, User = "user", UserLanguage = "de" };
        _realmConfig.ResetPasswordMailTemplateConfig.Clear();

        var result = await _controller.ResetPassword(requestParam, _attackDetection, _templateHandler.Object, _passwordGenerator.Object, _mailSender.Object) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [Fact]
    public async void ResetPasswordBadRequestsTest()
    {
        var resultNoParam = await _controller.ResetPassword(null, _attackDetection, _templateHandler.Object, _passwordGenerator.Object, _mailSender.Object);
        var resultNoRealm = await _controller.ResetPassword(new ResetPasswordParameter(), _attackDetection, _templateHandler.Object, _passwordGenerator.Object, _mailSender.Object);

        Assert.NotNull(resultNoParam);
        Assert.NotNull(resultNoRealm);
        Assert.IsAssignableFrom<BadRequestResult>(resultNoParam);
        Assert.IsAssignableFrom<BadRequestResult>(resultNoRealm);
    }

    [Fact]
    public async void ResetPasswordRealmNotFoundTest()
    {
        var result = await _controller.ResetPassword(new ResetPasswordParameter() { Realm = "UnknownRealm" }, _attackDetection, _templateHandler.Object, _passwordGenerator.Object, _mailSender.Object) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Contains("UnknownRealm", Assert.IsAssignableFrom<string>(result.Value));
    }

    [Fact]
    public async void ResetPasswordAuthPluginNotFoundTest()
    {
        _plugins.Clear();

        var result = await _controller.ResetPassword(new ResetPasswordParameter() { Realm = _realmConfig.Realm }, _attackDetection, _templateHandler.Object, _passwordGenerator.Object, _mailSender.Object) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Contains(_realmConfig.AuthenticationPluginId.ToString(), Assert.IsAssignableFrom<string>(result.Value));
    }

    [Fact]
    public async void ResetPasswordAttackDetectionFailedTest()
    {
        var requestParam = new ResetPasswordParameter() { Realm = _realmConfig.Realm, User = "user", UserLanguage = "de" };

        _attackDetection.ShouldFail = true;

        var result = await _controller.ResetPassword(requestParam, _attackDetection, _templateHandler.Object, _passwordGenerator.Object, _mailSender.Object) as ObjectResult;

        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.TooManyRequests, result.StatusCode);
    }


    private void InitMocks()
    {
        var pluginMeta = new Mock<IPluginMetadata>();
        pluginMeta.SetupGet(m => m.PluginId).Returns(TestPlugin.Id);
        pluginMeta.SetupGet(m => m.PluginConfigType).Returns(typeof(TestPluginConfig));
        _plugins = new List<IPluginMetadata> { pluginMeta.Object };

        _password = "TopSecret";
        _email = "test@example.com";

        _realmConfig = new TestRealmConfig();
        _realms = new List<IRealmConfig>() { _realmConfig };

        _tokenHandler = new Mock<ITokenHandler>();
        _tokenHandler.Setup(m => m.GetJwtToken(It.IsAny<IAuthenticationToken>(), It.IsAny<string>())).Returns("JwtToken");
        _tokenHandler.Setup(m => m.CheckJwtToken(It.IsAny<string>(), It.IsAny<string>())).Callback<string, string>(
            (token, _) =>
            {
                if (token != "JwtToken") throw new Exception("Bad token");
            }
        );

        _templateHandler = new Mock<ITemplateHandler>();
        _templateHandler
            .Setup(m => m.GetMessage(It.IsAny<MailTemplateConfig>(), It.IsAny<Dictionary<string, string>>()))
            .Returns(new MailMessage() { Body = "TestMail" });

        _passwordGenerator = new Mock<IPasswordGenerator>();
        _passwordGenerator.Setup(m => m.GenerateRandomPassword(It.IsAny<PasswordOptions>()))
            .Returns("NewRandomPassword");

        _passwordChecker = new Mock<IPasswordChecker>();
        _passwordChecker.Setup(m => m.IsValidPassword(It.IsAny<string>(), It.IsAny<PasswordOptions>()))
            .Returns<string, PasswordOptions>((pwd, _) => pwd != "TooEasy");
        _passwordChecker.SetupGet(m => m.PasswordPolicyMessages).Returns(() => new List<MessageStructure>() { new(MessageTypeEnum.Error, "Error") });

        _mailSender = new Mock<IMailSender>();
        _mailSender.Setup(m => m.Send(It.IsAny<MailMessage>()))
            .Callback<MailMessage>((msg) => { _sentMessage = msg; });
    }

    private RealmController CreateRealmController()
    {
        var authenticationPlugin = new Mock<IAuthenticationPlugin>();
        authenticationPlugin.Setup(m => m.Authenticate(It.IsAny<IRequestTokenParameter>())).Returns<IRequestTokenParameter>(
            (authParam) =>
            {
                if (authParam.Username != "user" || authParam.Password != _password) throw new AuthenticationFailedException();
                return new AuthenticationToken() { Claims = { new Claim("foo", "bar") } };
            });
        authenticationPlugin.Setup(m => m.GetEMail(It.IsAny<string>(), It.IsAny<Guid?>())).Returns(() => _email);
        authenticationPlugin.Setup(m => m.ChangePassword(It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<string>(), It.IsAny<bool>())).Callback<string, Guid?, string, bool>(
            (_, _, pwd, _) =>
            {
                _password = pwd;
            });

        var pluginCatalogue = new Mock<IPluginCatalogue>();
        pluginCatalogue.SetupGet(m => m.Plugins).Returns(_plugins);
        pluginCatalogue.Setup(m => m.GetAuthenticationPlugin(It.IsAny<Guid>())).Returns<Guid>(id => _plugins.Any(p => p.PluginId == id) ? authenticationPlugin.Object : null);
        pluginCatalogue.Setup(m => m.GetPluginMeta(It.IsAny<Guid>()))
            .Returns<Guid>(
                id => { return _plugins.FirstOrDefault(p => p.PluginId == id); });

        var realmCatalogue = new Mock<IRealmCatalogue>();
        realmCatalogue.SetupGet(m => m.Realms).Returns(_realms);
        realmCatalogue.Setup(m => m.GetRealm(It.IsAny<string>()))
            .Returns<string>(id => { return _realms.FirstOrDefault(p => p.Realm == id); });
        realmCatalogue.Setup(m => m.Save(It.IsAny<IRealmConfig>())).Callback<IRealmConfig>(
            config =>
            {
                _realms.RemoveAll(r => r.Realm == config.Realm);
                _realms.Add(config);
            });
        realmCatalogue.Setup(m => m.Delete(It.IsAny<string>())).Callback<string>(
            (id) =>
            {
                if (_realms.RemoveAll(r => r.Realm == id) == 0)
                    throw new NotFoundException(id);
            });

        var refreshTokenManager = new Mock<IRefreshTokenManager>();


        var stringLocalizer = new Mock<IStringLocalizer<RealmHandler>>();
        var realmHandler = new RealmHandler(realmCatalogue.Object, pluginCatalogue.Object, refreshTokenManager.Object, stringLocalizer.Object);
        var controller = new RealmController(realmHandler)
        {
            ControllerContext = new ControllerContext {HttpContext = new DefaultHttpContext()}
        };

        return controller;
    }

    private class AttackDetectionDummy : IAttackDetection
    {
        public bool ShouldFail { get; set; }

        public Task<bool> CheckRequest(string realm, string user, string request = null) => Task.FromResult(!ShouldFail);

        public Task AddFailedRequest(string realm, string user, string request) => Task.CompletedTask;

        [ExcludeFromCodeCoverage]
        public Task ClearFailedRequests(string realm, string user, string request = null) => Task.CompletedTask;
    }
}