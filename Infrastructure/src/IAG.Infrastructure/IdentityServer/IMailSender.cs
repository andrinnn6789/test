using System.Net.Mail;

namespace IAG.Infrastructure.IdentityServer;

public interface IMailSender
{
    void Send(MailMessage message);
}