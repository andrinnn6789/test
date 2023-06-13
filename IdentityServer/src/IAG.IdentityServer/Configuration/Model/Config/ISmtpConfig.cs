namespace IAG.IdentityServer.Configuration.Model.Config;

public interface ISmtpConfig
{
    string Server { get; }
    int? Port { get; }
    string User { get; }
    string Password { get; }
    bool UseSsl { get; }
}