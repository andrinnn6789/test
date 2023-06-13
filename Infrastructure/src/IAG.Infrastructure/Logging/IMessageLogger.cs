using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;

namespace IAG.Infrastructure.Logging;

public interface IMessageLogger
{
    void AddMessage(MessageTypeEnum type, string resourceId, params object[] args);

    void AddMessage(MessageStructure message);

    void AddMessage(System.Exception e);

    void ReportProgress(double progress);
}