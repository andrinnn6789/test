using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.IdentityServer.Configuration.DataLayer.State;

[ExcludeFromCodeCoverage]
public class RefreshTokenDb
{
    public Guid Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string PreviousRefreshToken { get; set; }

    public string User { get; set; }

    public Guid? Tenant { get; set; }

    public string AuthenticationToken { get; set; }
}