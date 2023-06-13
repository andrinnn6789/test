using System;
using System.Collections.Generic;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Localizer;

using Newtonsoft.Json;

namespace IAG.Infrastructure.Globalisation.Model;

public class MessageStructure : ILocalizableObject
{
    public MessageStructure(MessageTypeEnum type, string resourceId, params object[] @params)
    {
        Type = type;
        ResourceId = resourceId;
        Params = @params?.Length > 0 ? @params : null;
        Timestamp = DateTime.UtcNow;
    }

    [JsonConstructor]
    public MessageStructure(MessageTypeEnum type, string resourceId, DateTime timestamp, params object[] @params)
    {
        Type = type;
        ResourceId = resourceId;
        Params = @params?.Length > 0 ? @params : null;
        Timestamp = timestamp;
    }

    public MessageStructure(System.Exception exception)
    {
        var localizableParameter = LocalizableException.GetExceptionMessage(exception);
        var msgParams = new List<object>(localizableParameter.Params)
        {
            exception.StackTrace
        };

        Type = MessageTypeEnum.Debug;
        ResourceId = $"{localizableParameter.ResourceId}{Environment.NewLine}{{{msgParams.Count - 1}}}";
        Params = msgParams.ToArray();
        Timestamp = DateTime.UtcNow;
    }

    public string ResourceId { get; }

    public object[] Params { get; }

    public MessageTypeEnum Type { get; }

    public DateTime Timestamp { get; }
}