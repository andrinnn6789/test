using System.ComponentModel.DataAnnotations;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Context.TestContext;

[UsedImplicitly]
public class TestEntity : BaseEntity
{
    [UsedImplicitly]
    public int TestNumber { get; set; }

    [MaxLength(32)]
    [UsedImplicitly]
    public string TestString { get; set; }
}