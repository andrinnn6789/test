using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;

namespace IAG.Infrastructure.TestHelper.xUnit;

[ExcludeFromCodeCoverage]
public class MockILogger<T> : ILogger<T>
{
    public bool EnabledState = true;

    public List<string> LogEntries { get; }

    public MockILogger(List<string> logEntries = null)
    {
        LogEntries = logEntries ?? new List<string>();
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception exception, Func<TState, System.Exception, string> formatter)
    {
        lock (LogEntries)
        {
            LogEntries.Add(state.ToString());
        }
        Debug.WriteLine(state.ToString());
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return EnabledState;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}