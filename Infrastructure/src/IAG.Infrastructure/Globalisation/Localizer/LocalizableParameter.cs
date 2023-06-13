using System.Collections.Generic;

namespace IAG.Infrastructure.Globalisation.Localizer;

public class LocalizableParameter : ILocalizableObject
{
    public LocalizableParameter(string resourceId, params object[] args)
    {
        ResourceId = resourceId;
        Params = args;
    }

    // Setter needed for deserialization
    // ReSharper disable once MemberCanBePrivate.Global
    public string ResourceId { get; set; }

    // Setter needed for deserialization
    // ReSharper disable once MemberCanBePrivate.Global
    public object[] Params { get; set; }

    public LocalizableParameter Clone()
    {
        return new LocalizableParameter(ResourceId, CloneParams(Params));
    }

    private object[] CloneParams(IReadOnlyCollection<object> @params)
    {
        var clonedParams = new object[@params.Count];
        for (var i = 0; i < Params.Length; i++)
        {
            if (Params[i] is LocalizableParameter locParam)
                clonedParams[i] = locParam.Clone();
            else
                clonedParams[i] = Params[i];
        }

        return clonedParams;
    }
}