using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Wamas.Common.BusinessLogic;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

using Moq;

using Xunit;

namespace IAG.VinX.CDV.Test.Wamas.Common.BusinessLogic;

public class ProcessErrorLoggerTest
{
    [Fact]
    public void Log_WhenSuccessful_ShouldHaveInsertedInDatabase()
    {
        // Arrange
        const string fakeTitle = "fakeTitle";
        const string fakeDescription = "fakeDescription";
        var fakeMessageLogger = new Mock<IMessageLogger>();
        var fakeSybaseConnection = new Mock<ISybaseConnection>();
        fakeSybaseConnection.Setup(s => s.Insert(It.IsAny<ErrorLog>()));

        // Act
        ProcessErrorLogger.Log(fakeMessageLogger.Object, fakeSybaseConnection.Object, fakeTitle, fakeDescription);
        
        // Assert
        fakeSybaseConnection.Verify(
            s => s.Insert(It.Is<ErrorLog>(e => e.Title == fakeTitle)), Times.Once);
    }
    
    [Fact]
    public void Log_WhenNotSuccessful_ShouldHaveLoggedException()
    {
        // Arrange
        const string  fakeTitle = "fakeTitle";
        const string  fakeDescription = "fakeDescription";
        var fakeException = new Exception("This is a fake exception");
        var fakeMessageLogger = new Mock<IMessageLogger>();
        fakeMessageLogger.Setup(l => l.AddMessage(MessageTypeEnum.Error, It.IsAny<string>(), It.IsAny<object[]>()));
        var fakeSybaseConnection = new Mock<ISybaseConnection>();
        fakeSybaseConnection.Setup(s => s.Insert(It.IsAny<ErrorLog>())).Throws(fakeException);

        // Act
        ProcessErrorLogger.Log(fakeMessageLogger.Object, fakeSybaseConnection.Object, fakeTitle, fakeDescription);
        
        // Assert
        fakeMessageLogger.Verify(
            s => s.AddMessage(MessageTypeEnum.Error, "CDV.Job.Wamas.Fehler beim Schreiben des Fehlerprotokolls an VinX, {0}", fakeException.Message),
            Times.Once);
    }
}