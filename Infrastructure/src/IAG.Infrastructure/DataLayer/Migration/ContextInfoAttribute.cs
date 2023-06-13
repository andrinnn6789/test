using System;

namespace IAG.Infrastructure.DataLayer.Migration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ContextInfoAttribute : Attribute
{
    public string Name { get; }

    public Type Type { get; }

    public ContextInfoAttribute(string name, Type type = null)
    {
        Name = name;
        Type = type;
    }
}