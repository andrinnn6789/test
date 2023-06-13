using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.IdentityServer.Configuration.DataLayer.State;

[ExcludeFromCodeCoverage]
public class FailedRequestDb
{
    public Guid Id { get; set; }

    public string Realm { get; set; }

    public string User { get; set; }

    public DateTime Timestamp { get; set; }

    public string Request { get; set; }
}