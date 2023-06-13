using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.Exception.HttpException;

[ExcludeFromCodeCoverage]
public class BadRequestException : LocalizableException
{
    public BadRequestException(string resourceId, params object[] args) : base(resourceId, args)
    {
    }
}