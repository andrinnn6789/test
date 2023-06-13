using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

using IAG.Infrastructure.IdentityServer;
using IAG.Infrastructure.IdentityServer.Model;

namespace IAG.IdentityServer.Mail;

public class TemplateHandler : ITemplateHandler
{
    public MailMessage GetMessage(MailTemplateConfig templateConfig, Dictionary<string, string> replacements = null)
    {
        string subject = templateConfig.Subject;
        string message = File.ReadAllText(templateConfig.PathToTemplate);

        if (replacements != null)
        {
            foreach (KeyValuePair<string, string> replacement in replacements)
            {
                subject = subject.Replace($"{{{replacement.Key}}}", replacement.Value);
                message = message.Replace($"{{{replacement.Key}}}", replacement.Value);
            }
        }

        return new MailMessage()
        {
            From = new MailAddress(templateConfig.Sender),
            Subject = subject,
            Body = message,
            IsBodyHtml = templateConfig.PathToTemplate.EndsWith(".html", StringComparison.InvariantCultureIgnoreCase)
        };
    }
}