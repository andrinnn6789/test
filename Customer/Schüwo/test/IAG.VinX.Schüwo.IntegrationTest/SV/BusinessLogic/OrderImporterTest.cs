using System.Linq;
using System.Reflection;

using IAG.Common.Dto;
using IAG.Common.TestHelper.Arrange;
using IAG.Common.TestHelper.DataLayerSybase;
using IAG.Infrastructure.Logging;
using IAG.VinX.Schüwo.SV.BusinessLogic;
using IAG.VinX.Schüwo.SV.Dto;

using Moq;

using Xunit;

namespace IAG.VinX.Schüwo.IntegrationTest.SV.BusinessLogic;

public class OrderImporterTest
{
    [Fact]
    public void OrderImportTest()
    {
        var resultsCount = new ResultCounts();
        var connection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
        // ReSharper disable once PossibleInvalidOperationException
        var providerId = connection.GetQueryable<ProviderSetting>().First().Id;
        var orderImporter = new OrderImporter(connection, new Mock<IMessageLogger>().Object, resultsCount, providerId);
        SybaseTransctionHelper.ExecuteInRollbackTransaction(
            connection,
            () => orderImporter.Import(Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType().Namespace + ".orderfile.txt")));
        Assert.Equal(1, resultsCount.SuccessCount);
        Assert.Equal(0, resultsCount.WarningCount);
    }

    [Fact]
    public void OrderImportFail()
    {
        var resultsCount = new ResultCounts();
        var connection = SybaseConnectionFactoryHelper.CreateFactory().CreateConnection();
        // ReSharper disable once PossibleInvalidOperationException
        var providerId = connection.GetQueryable<ProviderSetting>().First().Id;
        var orderImporter = new OrderImporter(connection, new Mock<IMessageLogger>().Object, resultsCount, providerId);
        SybaseTransctionHelper.ExecuteInRollbackTransaction(
            connection,
            () => orderImporter.Import(Assembly.GetExecutingAssembly().GetManifestResourceStream(GetType().Namespace + ".orderfilefail.txt")));
        Assert.Equal(0, resultsCount.SuccessCount);
        Assert.Equal(1, resultsCount.WarningCount);
    }
}