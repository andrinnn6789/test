using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.Exception.HttpException;

[ExcludeFromCodeCoverage]
public class NotFoundException : LocalizableException
{
    public NotFoundException(string resourceId, params object[] args) : base(resourceId, args)
    {
    }
}