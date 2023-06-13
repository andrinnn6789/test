using System.Collections.Generic;
using System.Net.Mail;

using IAG.Infrastructure.IdentityServer.Model;

namespace IAG.Infrastructure.IdentityServer;

public interface ITemplateHandler
{
    MailMessage GetMessage(MailTemplateConfig templateConfig, Dictionary<string, string> replacements = null);
}