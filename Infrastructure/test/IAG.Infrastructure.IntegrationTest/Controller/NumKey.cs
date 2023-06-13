using System.ComponentModel.DataAnnotations;

using IAG.Infrastructure.Crud;

using JetBrains.Annotations;

namespace IAG.Infrastructure.IntegrationTest.Controller;

public class NumKey : IEntityNumKey
{
    [UsedImplicitly]
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Hint { get; set; }
}