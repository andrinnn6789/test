using System;
using System.Text;

using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Gastivo.Common.DatabaseLayer.Domain;
using IAG.VinX.CDV.Resource;

using NHibernate;

namespace IAG.VinX.CDV.Gastivo.Common.BusinessLogic;

public static class ProcessErrorLogger
{
    public static void Log(IMessageLogger messageLogger, ISession databaseSession, string title, string message)
    {
        try
        {
            var messageAsByteArray = Encoding.ASCII.GetBytes(message);
            var errorLog = new ErrorLog
            {
                Title = title,
                LogDate = DateTime.Now,
                DateMillisecond = DateTime.Now.Millisecond,
                Occurence = "Gastivo",
                User = "Support",
                Description = messageAsByteArray
            };

            databaseSession.Save(errorLog);
        }
        catch (Exception e)
        {
            messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.GastivoLogError, e.Message);
        }
    }
}