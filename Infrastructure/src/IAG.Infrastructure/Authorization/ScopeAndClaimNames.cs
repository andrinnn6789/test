namespace IAG.Infrastructure.Authorization;

public static class ScopeNamesInfrastructure
{
    private const string ScopePrefix = "Base.";

    public const string SystemScope = ScopePrefix + "System";
    public const string AdminScope = ScopePrefix + "Admin";
    public const string ReaderScope = ScopePrefix + "Reader";
    public const string AtlasScope = ScopePrefix + "Atlas";
    public const string ProcessEngine = ScopePrefix + "ProcessEngine";
    public const string InstallClient = ScopePrefix + "Install";
}

public static class ClaimNamesInfrastructure
{
    public const string GeneralClaim = "General";
}