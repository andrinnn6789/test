using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.ProcessEngine.Store.Model;

[ExcludeFromCodeCoverage]
public class EngineConfig : IEngineConfig
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int ArchiveDays { get; set; }

    public int ArchiveDaysErrors { get; set; }

    public string SerializedData { get; set; }

    public int JobShutdownDelay { get; set; } = 3000;

    public EngineConfig()
    {
        Id = Guid.NewGuid();
        Name = "Configuration Executer";
        ArchiveDays = 10;
        ArchiveDaysErrors = 100;
    }

}