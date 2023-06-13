using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Resource;

namespace IAG.Infrastructure.Exception.HttpException;

[ExcludeFromCodeCoverage]
public class AuthorizationFailedException : LocalizableException
{
    public AuthorizationFailedException() : base(ResourceIds.NotAuthenticatedExceptionMessage)
    {
    }

    public AuthorizationFailedException(string resourceId, params object[] args) : base(resourceId, args)
    {
    }
}