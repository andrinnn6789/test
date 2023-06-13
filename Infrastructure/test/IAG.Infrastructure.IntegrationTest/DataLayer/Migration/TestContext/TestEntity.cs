using System;

using JetBrains.Annotations;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Migration.TestContext;

[UsedImplicitly]
public class TestEntity
{
    [UsedImplicitly]
    public Guid Id { get; set; }

    [UsedImplicitly]
    public int TestNumber { get; set; }

    [UsedImplicitly]
    public string TestString { get; set; }
}