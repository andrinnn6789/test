namespace IAG.Infrastructure.Controllers;

public static class InfrastructureEndpoints
{
    public const string Auth = "api/Identity/";

    public const string AuthToken = InfrastructureEndpoints.Auth + "Realm/RequestToken";

    public const string Resource = "api/Core/Resource/";

    public const string CoreAdmin = "api/Core/Admin/";

    public const string Process = "api/Process/";

    public const string ProcessJobService = InfrastructureEndpoints.Process + "JobService/";

    public const string Log = "api/Log";
}