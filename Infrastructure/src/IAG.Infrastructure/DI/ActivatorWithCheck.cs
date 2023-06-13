using System;

namespace IAG.Infrastructure.DI;

public static class ActivatorWithCheck
{
    public static T CreateInstance<T>(Type type)
    {
        var instance = (T) Activator.CreateInstance(type);
        if (instance == null)
            throw  new System.Exception($"(cannot instantiate {type.FullName}");
        return instance;
    }
}