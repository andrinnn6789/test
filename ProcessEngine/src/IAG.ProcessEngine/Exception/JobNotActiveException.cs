using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.ProcessEngine.Exception;

public class JobNotActiveException : ApplicationException
{
    [ExcludeFromCodeCoverage]
    public JobNotActiveException()
    {
    }
}