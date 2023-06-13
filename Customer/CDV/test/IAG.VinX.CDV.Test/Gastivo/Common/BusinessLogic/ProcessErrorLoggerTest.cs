using IAG.Common.DataLayerNHibernate;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Gastivo.Common.BusinessLogic;
using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;

using Moq;

using NHibernate;

using Xunit;

namespace IAG.VinX.CDV.Test.Gastivo.Common.BusinessLogic;

public class ProcessErrorLoggerTest
{
    [Fact]
    public void Log_WhenSuccessful_ShouldHaveInsertedInDatabase()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        const string fakeTitle = "fakeTitle";
        const string fakeDescription = "fakeDescription";
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeVinXSession = new Mock<ISession>();
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeVinXSessionContext.Setup(context => context.Session).Returns(fakeVinXSession.Object);
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);

        // Act
        ProcessErrorLogger.Log(fakeMessageLogger.Object, fakeVinXSession.Object, fakeTitle, fakeDescription);
        
        // Assert
        fakeVinXSession.Verify(
            s => s.Save(It.Is<ErrorLog>(e => e.Title == fakeTitle)), Times.Once);
    }
    
    [Fact]
    public void Log_WhenNotSuccessful_ShouldHaveLoggedException()
    {
        // Arrange
        const string connectionString = "fakeConnectionString";
        const string fakeTitle = "fakeTitle";
        const string fakeDescription = "fakeDescription";
        var fakeException = new Exception("This is a fake exception");
        var fakeMessageLogger = new Mock<IMessageLogger>();
        fakeMessageLogger.Setup(l => l.AddMessage(MessageTypeEnum.Error, It.IsAny<string>(), It.IsAny<object[]>()));
        var fakeVinXSession = new Mock<ISession>();
        fakeVinXSession.Setup(s => s.Save(It.IsAny<ErrorLog>())).Throws(fakeException);
        var fakeVinXSessionContext = new Mock<ISessionContext>();
        var fakeSessionFactory = new Mock<ISessionContextFactory>();
        fakeVinXSessionContext.Setup(context => context.Session).Returns(fakeVinXSession.Object);
        fakeSessionFactory
            .Setup(factory => factory.CreateSessionContext(connectionString, It.IsAny<IEnumerable<Type>>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .Returns(fakeVinXSessionContext.Object);

        // Act
        ProcessErrorLogger.Log(fakeMessageLogger.Object, fakeVinXSession.Object, fakeTitle, fakeDescription);
        
        // Assert
        fakeMessageLogger.Verify(
            s => s.AddMessage(MessageTypeEnum.Error, "CDV.Job.Gastivo.Fehler beim Schreiben des Fehlerprotokolls an VinX, {0}", fakeException.Message),
            Times.Once);
    }
}