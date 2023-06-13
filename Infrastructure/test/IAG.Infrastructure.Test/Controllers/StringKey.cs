using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Crud;
using IAG.Infrastructure.Globalisation.Model;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Test.Controllers;

public class StringKey : IEntityStringKey
{
    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    public string Id { get; set; }

    public string Name { get; set; }

    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    public List<Translation> Translations { get; set; }
}