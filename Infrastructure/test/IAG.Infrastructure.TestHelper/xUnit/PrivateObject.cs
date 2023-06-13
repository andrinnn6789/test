using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace IAG.Infrastructure.TestHelper.xUnit;

/// <summary>
///  Code from https://github.com/douglasaguiar/BetterPrivateObject/blob/master/src/BetterPrivateObject/PrivateObject.cs
/// </summary>
public class PrivateObject<T> : DynamicObject
{
    public T Container { get; set; }

    public PrivateObject()
    {
        Container = Activator.CreateInstance<T>();
    }

    public PrivateObject(T container)
    {
        Container = container;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
        // See https://stackoverflow.com/questions/5492373/get-generic-type-of-call-to-method-in-dynamic-object
        // Or if you can japanese: http://neue.cc/category/programming :-)
        var csharpBinder = binder.GetType().GetInterface("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");
        result = null;
        if (csharpBinder == null) 
            return false;
        var genericTypeArgs = (csharpBinder.GetProperty("TypeArguments")?.GetValue(binder, null) as IList<Type>)?.ToArray();

        var method = typeof(T)
            .GetRuntimeMethods().FirstOrDefault(m => m.Name.Equals(binder.Name) && m.GetParameters().Length == args.Length
                && (!m.IsGenericMethod || (genericTypeArgs?.Length > 0)));

        if (method == null)
        {
            return false;
        }

        if (method.IsGenericMethod)
        {
            // ReSharper disable once AssignNullToNotNullAttribute -> No, it is already checked in GetRuntimeMethods...
            method = method.MakeGenericMethod(genericTypeArgs);
        }

        try
        {
            result = method.Invoke(Container, args);
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException != null)
            {
                throw ex.InnerException;
            }

            throw;
        }

        return true;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        PropertyInfo property = typeof(T).GetProperty(binder.Name,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property == null)
        {
            FieldInfo field = typeof(T).GetField(binder.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                result = null;
                return false;
            }

            result = field.GetValue(Container);
        }
        else
        {
            result = property.GetValue(Container, null);
        }

        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        PropertyInfo property = typeof(T).GetProperty(binder.Name,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (property == null)
        {
            FieldInfo field = typeof(T).GetField(binder.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                return false;
            }

            field.SetValue(Container, value);
        }
        else
        {
            property.SetValue(Container, value, null);
        }

        return true;
    }
}