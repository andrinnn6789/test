using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IAG.Infrastructure.Globalisation.Localizer;

namespace IAG.Infrastructure.Exception;

public class LocalizableException : ApplicationException
{
    public LocalizableException(string resourceId, params object[] args) : base(resourceId)
    {
        LocalizableParameter = new LocalizableParameter(resourceId, args);
    }

    public LocalizableException(string resourceId, System.Exception innerException, params object[] args) : base(resourceId, innerException)
    {
        LocalizableParameter = new LocalizableParameter(resourceId, args);
    }

    public LocalizableParameter LocalizableParameter { get; }

    public static LocalizableParameter GetExceptionMessage(System.Exception ex)
    {
        var message = new StringBuilder();
        var msgParameters = new List<object>();

        if (ex is AggregateException aggEx)
        {
            foreach (System.Exception innerException in aggEx.InnerExceptions)
            {
                HandleException(innerException, message, msgParameters);
            }
        }
        else
        {
            if (ex.InnerException != null)
            {
                HandleException(ex.InnerException, message, msgParameters);
            }

            if (ex is LocalizableException locEx)
            { 
                if (msgParameters.Any())
                    AddException(message, msgParameters, locEx.LocalizableParameter);
                else
                    return locEx.LocalizableParameter;
            }
            else
            {
                AddException(message, msgParameters, new LocalizableParameter(ex.Message));
            }
        }

        if (msgParameters.Count == 1 && msgParameters.First() is LocalizableParameter localizableParameter)
            return localizableParameter;

        return new LocalizableParameter(message.ToString(), msgParameters.ToArray());
    }

    private static void HandleException(System.Exception exception, StringBuilder message, ICollection<object> msgParameters)
    {
        var localizableParameter = GetExceptionMessage(exception);
        AddException(message, msgParameters, localizableParameter);
    }

    private static void AddException(StringBuilder message, ICollection<object> msgParameters, LocalizableParameter localizableParameter)
    {
        if (msgParameters.Any())
            message.AppendLine();

        message.Append($"{{{msgParameters.Count}}}");
        msgParameters.Add(localizableParameter);
    }
}