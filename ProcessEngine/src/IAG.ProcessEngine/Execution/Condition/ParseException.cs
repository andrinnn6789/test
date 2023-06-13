using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Exception;

namespace IAG.ProcessEngine.Execution.Condition;

[ExcludeFromCodeCoverage]
public class ParseException : LocalizableException
{
    public ParseException(string resourceId, params object[] args) : base(resourceId, args)
    {
    }

    public ParseException(string resourceId, System.Exception innerException, params object[] args) : base(resourceId, innerException, args)
    {
    }
}