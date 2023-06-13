using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Exception.HttpException;

[ExcludeFromCodeCoverage]
public class DuplicateKeyException : ApplicationException
{
    [UsedImplicitly]
    public DuplicateKeyException(string message): base(message)
    {
    }
}