using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Mail;

using IAG.IdentityServer.Configuration.Model.Config;
using IAG.IdentityServer.Resource;
using IAG.Infrastructure.Exception;
using IAG.Infrastructure.IdentityServer;

namespace IAG.IdentityServer.Mail;

[ExcludeFromCodeCoverage]
public class SmtpMailSender : IMailSender
{
    private readonly SmtpClient _client;

    public SmtpMailSender(ISmtpConfig config)
    {
        if (string.IsNullOrEmpty(config.Server))
        {
            throw new LocalizableException(ResourceIds.MailConfigErrorServer);
        }

        _client = new SmtpClient(config.Server)
        {
            EnableSsl = config.UseSsl
        };

        if (config.Port.HasValue)
        {
            _client.Port = config.Port.Value;
        }

        if (!string.IsNullOrEmpty(config.User))
        {
            _client.Credentials = new NetworkCredential(config.User, config.Password);
        }
    }

    public void Send(MailMessage message)
    {
        _client.Send(message);
    }
}