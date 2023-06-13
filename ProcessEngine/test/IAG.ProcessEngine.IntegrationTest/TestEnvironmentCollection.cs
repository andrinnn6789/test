using IAG.Infrastructure.TestHelper.Startup;

using Xunit;

namespace IAG.ProcessEngine.IntegrationTest;

[CollectionDefinition("ProcessEngineController")]
public class TestEnvironmentCollection : ICollectionFixture<TestServerEnvironment>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}