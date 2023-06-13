namespace IAG.Infrastructure.IdentityServer.Model;

public class MailTemplateConfig
{
    public string Sender { get; set; }

    public string Language { get; set; }

    public string Subject { get; set; }

    public string PathToTemplate { get; set; }
}