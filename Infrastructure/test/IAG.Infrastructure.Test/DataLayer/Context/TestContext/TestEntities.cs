using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using IAG.Infrastructure.DataLayer.Model.Base;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Test.DataLayer.Context.TestContext;

[UsedImplicitly]
public class TestAEntity : BaseEntity
{
    [UsedImplicitly]
    public int TestNumber { get; set; }
}

[Table("TestBetaEntity")]
public class TestBEntity : BaseEntity
{
    [MaxLength(32)]
    [UsedImplicitly]
    public string TestString { get; set; }
}

public class TestGenericEntity<T> : BaseEntity
{
    [UsedImplicitly]
    public T TestField { get; set; }
}