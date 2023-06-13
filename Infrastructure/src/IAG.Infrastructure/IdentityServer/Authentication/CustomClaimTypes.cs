
namespace IAG.Infrastructure.IdentityServer.Authentication;

public static class CustomClaimTypes
{
    private static readonly string CustomClaimTypePrefix = "ch.iag";

    public static readonly string Tenant = $"{CustomClaimTypePrefix}.Tenant";
}