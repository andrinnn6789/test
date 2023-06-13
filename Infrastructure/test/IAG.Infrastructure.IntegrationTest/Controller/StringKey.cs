using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using IAG.Infrastructure.Crud;
using IAG.Infrastructure.Globalisation.Model;

using JetBrains.Annotations;

namespace IAG.Infrastructure.IntegrationTest.Controller;

public class StringKey : IEntityStringKey
{
    [UsedImplicitly]
    [Key]
    public string Id { get; set; }

    public string Name { get; set; }

    [UsedImplicitly]
    public List<Translation> Translations { get; set; }

    public StringKey()
    {
        Translations = new List<Translation>();
    }
}