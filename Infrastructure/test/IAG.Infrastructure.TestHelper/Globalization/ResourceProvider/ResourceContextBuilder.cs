using System;

using IAG.Infrastructure.Globalisation.Context;
using IAG.Infrastructure.IdentityServer.Authentication;

using Microsoft.EntityFrameworkCore;

namespace IAG.Infrastructure.TestHelper.Globalization.ResourceProvider;

public static class ResourceContextBuilder
{
    public static ResourceContext GetNewContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ResourceContext>();
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        return new ResourceContext(optionsBuilder.Options, new ExplicitUserContext("test", null));
    }
}