using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.DataLayer.Model.System;

[ExcludeFromCodeCoverage]
public class SystemLog : BaseEntity
{
    [UsedImplicitly]
    public SystemLogType LogType { get; set; }

    [UsedImplicitly]
    public string Message { get; set; }
}