using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace IAG.DataLayer.GenerateCreateScript.Helper;

internal class DynamicDbContextOptions : DbContextOptions
{
#pragma warning disable EF1001 // Internal EF Core API usage.
    internal DynamicDbContextOptions(Type dbContextType)
        : base(new Dictionary<Type, IDbContextOptionsExtension>())
    {
        ContextType = dbContextType;
    }

    internal DynamicDbContextOptions(Type dbContextType, [NotNull] IReadOnlyDictionary<Type, IDbContextOptionsExtension> extensions) : base(extensions)
    {
        ContextType = dbContextType;
    }
#pragma warning restore EF1001 // Internal EF Core API usage.

    public override DbContextOptions WithExtension<TExtension>(TExtension extension)
    {
        // Implementation from Microsoft.EntityFrameworkCore.DbContextOptions<TContext> 
        Dictionary<Type, IDbContextOptionsExtension> dictionary = Extensions.ToDictionary(p => p.GetType(), p => p);
        dictionary[typeof(TExtension)] = extension;
        return new DynamicDbContextOptions(ContextType, dictionary);
    }

    public override Type ContextType { get; }
}