using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json.Serialization;

namespace IAG.Infrastructure.Startup.Extensions;

public class JsonContractResolver : DefaultContractResolver
{
    private readonly Func<IHttpContextAccessor> _httpContextAccessorProvider;

    public const string MemberFilterKey = "MemberFilter";

    public JsonContractResolver(Func<IHttpContextAccessor> httpContextAccessorProvider)
    {
        _httpContextAccessorProvider = httpContextAccessorProvider;
        NamingStrategy = new DefaultNamingStrategy();
    }

    public override JsonContract ResolveContract(Type type)
    {
        var httpContext = _httpContextAccessorProvider().HttpContext;
        return httpContext?.Items.ContainsKey(MemberFilterKey) == true ? CreateContract(type) : base.ResolveContract(type);
    }

    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        var httpContext = _httpContextAccessorProvider().HttpContext;
        if (httpContext?.Items.ContainsKey(MemberFilterKey) == true)
        {
            return (List<MemberInfo>)httpContext.Items[MemberFilterKey];
        }

        return base.GetSerializableMembers(objectType);
    }
}