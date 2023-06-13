using System;

using JetBrains.Annotations;

namespace IAG.VinX.CDV.Wamas.Common.BusinessLogic;

public class CastHelper
{
    public static dynamic Cast(object src, Type t)
    {
        var castMethod = typeof(CastHelper).GetMethod("CastGeneric")?.MakeGenericMethod(t);
        return castMethod?.Invoke(null, new[] { src });
    }

    [UsedImplicitly]
    public static T CastGeneric<T>(object src)
    {
        return (T)Convert.ChangeType(src, typeof(T));
    }
}