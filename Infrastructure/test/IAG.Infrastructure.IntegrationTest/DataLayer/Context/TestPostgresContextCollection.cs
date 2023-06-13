using Xunit;

namespace IAG.Infrastructure.IntegrationTest.DataLayer.Context;

[CollectionDefinition("PostgresContextCollection")]
public class TestPostgresContextCollection : ICollectionFixture<PostgresTestContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}