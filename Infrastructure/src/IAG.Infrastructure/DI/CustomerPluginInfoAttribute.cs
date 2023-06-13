using System;
using System.Diagnostics.CodeAnalysis;

namespace IAG.Infrastructure.DI;

[ExcludeFromCodeCoverage]
[AttributeUsage(AttributeTargets.Class)]
public class CustomerPluginInfoAttribute : Attribute
{
    public Guid CustomerId { get; }

    public CustomerPluginInfoAttribute(string customerId)
    {
        CustomerId = Guid.Parse(customerId);
    }
}