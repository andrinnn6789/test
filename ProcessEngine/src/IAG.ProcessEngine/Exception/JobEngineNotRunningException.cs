using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.ProcessEngine.Exception;

public class JobEngineNotRunningException :ApplicationException
{
    [ExcludeFromCodeCoverage]
    public JobEngineNotRunningException()
    {
    }
}