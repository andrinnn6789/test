using IAG.Infrastructure.ImportExport;
using IAG.InstallClient.Authentication;

using Moq;

using Xunit;

namespace IAG.InstallClient.Test.Authentication;

public class InstallerRoleCreatorTest
{
    [Fact]
    public void CreateRoleTest()
    {
        InstallerRoleCreator.CreateRole(new Mock<ISeedImporter>().Object);
    }
}