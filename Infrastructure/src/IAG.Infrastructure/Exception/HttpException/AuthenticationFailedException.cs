using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Resource;

namespace IAG.Infrastructure.Exception.HttpException;

[ExcludeFromCodeCoverage]
public class AuthenticationFailedException : LocalizableException
{
    public AuthenticationFailedException() : base(ResourceIds.NotAuthenticatedExceptionMessage)
    {
    }

    public AuthenticationFailedException(string resourceId, params object[] args) : base(resourceId, args)
    {
    }
}