using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace IAG.InstallClient.Authorization;

public class CookieJwtBearerPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string name, JwtBearerOptions options)
    {
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = OnMessageReceived,
        };

        new JwtBearerPostConfigureOptions().PostConfigure(name, options);
    }

    private async Task OnMessageReceived(MessageReceivedContext arg)
    {
        var token = BearerTokenCookieHandler.GetBearerToken(arg.HttpContext);
        if (string.IsNullOrEmpty(token))
        {
            arg.Token = null;
            return;
        }

        arg.Token = token;
        if (arg.Options.Configuration == null && arg.Options.ConfigurationManager != null)
        {
            arg.Options.Configuration = await arg.Options.ConfigurationManager.GetConfigurationAsync(arg.HttpContext.RequestAborted);
        }
        if (arg.Options.Configuration?.SigningKeys.Any() == false)
        {
            foreach (var key in arg.Options.Configuration.JsonWebKeySet.GetSigningKeys())
            {
                arg.Options.Configuration.SigningKeys.Add(key);
            }
        }
    }
}