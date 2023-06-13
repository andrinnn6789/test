using System;
using System.Collections.Generic;
using System.Linq;

using IAG.Infrastructure.Exception;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.Model;
using IAG.Infrastructure.Resource;
using Microsoft.Extensions.Localization;

namespace IAG.Infrastructure.Globalisation.Localizer;

public class MessageLocalizer : IMessageLocalizer
{
    private readonly IStringLocalizer _localizer;

    public MessageLocalizer(IStringLocalizer localizer)
    {
        _localizer = localizer;
    }

    public MessageStructureLocalized Localize(MessageStructure msg)
    {
        var workingParams = new List<object>();
        if (msg.Params != null)
            foreach (var param in msg.Params)
            {
                if (param is LocalizableParameter locParam)
                    workingParams.Add(locParam.Clone());
                else
                    workingParams.Add(param);
            }

        var message = LocalizeMessage(new LocalizableParameter(msg.ResourceId, workingParams.ToArray()));
        return new(
            msg.Type,
            _localizer.GetString(ResourceProvider.ResourceProvider.GetEnumResourceId(msg.Type)), 
            message,
            msg.Timestamp); 
    }

    public List<MessageStructureLocalized> Localize(IEnumerable<MessageStructure> msgs)
    {
        var messageStructures = msgs.ToList();
        var localizedMessages = messageStructures.Select(Localize).ToList();

        var summaryMsg = localizedMessages.SingleOrDefault(m => m.Type == MessageTypeEnum.Summary);
        if (summaryMsg != null)
        {
            if (localizedMessages.IndexOf(summaryMsg) != 0)
            {
                localizedMessages.Remove(summaryMsg);
                localizedMessages.Insert(0, summaryMsg);
            }
            return localizedMessages;
        }

        var summary = _localizer.GetString(ResourceIds.GenericOk).Value;
        var msg = localizedMessages
            .Where(m => m.Type == MessageTypeEnum.Error || m.Type == MessageTypeEnum.Warning || m.Type == MessageTypeEnum.Success)
            .OrderByDescending(m => m.Type)
            .FirstOrDefault();
        if (msg != null)
        {
            summary = msg.Text;
        }

        localizedMessages.Insert(
            0, 
            new MessageStructureLocalized(
                MessageTypeEnum.Summary,
                _localizer.GetString(ResourceProvider.ResourceProvider.GetEnumResourceId(MessageTypeEnum.Summary)),
                summary,
                DateTime.UtcNow));

        return localizedMessages;
    }

    public string LocalizeException(System.Exception exception)
    {
        return LocalizeMessage(LocalizableException.GetExceptionMessage(exception));
    }

    private string LocalizeMessage(LocalizableParameter localizableParameter)
    {
        if (localizableParameter.Params != null && localizableParameter.Params.Any())
        {
            for (var i = 0; i < localizableParameter.Params.Length; i++)
            {
                if (localizableParameter.Params[i] is LocalizableParameter parameter)
                {
                    localizableParameter.Params[i] = LocalizeMessage(parameter) + Environment.NewLine;
                }
            }

            return _localizer.GetString(localizableParameter.ResourceId, localizableParameter.Params);
        }

        return _localizer.GetString(localizableParameter.ResourceId?? string.Empty);
    }
}