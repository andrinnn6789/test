using System;
using System.Text;

using IAG.Common.DataLayerSybase;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Logging;
using IAG.VinX.CDV.Resource;
using IAG.VinX.CDV.Wamas.Common.DataAccess.DbModel;

namespace IAG.VinX.CDV.Wamas.Common.BusinessLogic;

public static class ProcessErrorLogger
{
    public static void Log(IMessageLogger messageLogger, ISybaseConnection sybaseConnection, string title, string message)
    {
        try
        {
            var messageAsByteArray = Encoding.ASCII.GetBytes(message);
            var errorLog = new ErrorLog
            {
                Title = title,
                LogDate = DateTime.Now,
                DateMillisecond = DateTime.Now.Millisecond,
                Occurence = "WAMAS",
                User = "Support",
                Description = messageAsByteArray
            };

            sybaseConnection.Insert(errorLog);
        }
        catch (Exception e)
        {
            messageLogger.AddMessage(MessageTypeEnum.Error, ResourceIds.WamasLogError, e.Message);
        }
    }
}