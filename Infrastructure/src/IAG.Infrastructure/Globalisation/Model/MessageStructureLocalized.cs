using System;

using IAG.Infrastructure.Globalisation.Enum;

namespace IAG.Infrastructure.Globalisation.Model;

public class MessageStructureLocalized
{
    public MessageStructureLocalized(MessageTypeEnum type, string typeName, string message, DateTime timestamp)
    {
        Type = type;
        TypeName = typeName;
        Text = message;
        Timestamp = timestamp;
    }

    public string Text { get; }

    public string TypeName { get; }

    public MessageTypeEnum Type { get; }

    public DateTime Timestamp { get; }
}