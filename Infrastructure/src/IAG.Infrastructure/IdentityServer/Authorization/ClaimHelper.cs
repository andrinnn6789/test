using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IAG.Infrastructure.IdentityServer.Authorization.Model;

namespace IAG.Infrastructure.IdentityServer.Authorization;

public static class ClaimHelper
{
    private const char PermissionDelimiter = '|';
    private const char ClaimDelimiter = ':';
    private const char ScopeDelimiter = '@';

    public static string ToString(string scopeName, string claimName, PermissionKind permissions)
    {
        var allPermissions = new List<PermissionKind>() { PermissionKind.Create, PermissionKind.Read, PermissionKind.Update, PermissionKind.Delete, PermissionKind.Execute };
        var permissionList = allPermissions.Where(e => permissions.HasFlag(e)).ToList();
        if (permissionList.Count == 0)
        {
            permissionList.Add(PermissionKind.None);
        }

        var result = new StringBuilder();
        result.Append(string.Join(PermissionDelimiter, permissionList)).Append(ClaimDelimiter).Append(claimName);
        if (!string.IsNullOrEmpty(scopeName))
        {
            result.Append(ScopeDelimiter).Append(scopeName);
        }

        return result.ToString();
    }

    public static void FromString(string claimString, out string scopeName, out string claimName, out PermissionKind permissions)
    {
        permissions = PermissionKind.None;
        var permissionEnd = claimString.IndexOf(ClaimDelimiter);
        if (permissionEnd > 0)
        {
            permissions = ParsePermissions(claimString.Substring(0, permissionEnd));
            claimString = claimString.Substring(permissionEnd + 1);
        }

        var claimEnd = claimString.LastIndexOf(ScopeDelimiter);
        if (claimEnd < 0)
        {
            scopeName = string.Empty;
            claimName = claimString;
        }
        else
        {
            scopeName = claimString.Substring(claimEnd + 1);
            claimName = claimString.Substring(0, claimEnd);
        }
    }

    public static bool MatchesClaimAndScope(string claimString, string scopeName, string claimName)
    {
        return claimString.EndsWith($"{ClaimDelimiter}{claimName}{ScopeDelimiter}{scopeName}");
    }

    public static PermissionKind GetPermissions(string claimString)
    {
        var permissionEnd = claimString.IndexOf(ClaimDelimiter);
        if (permissionEnd > 0)
        {
            return ParsePermissions(claimString.Substring(0, permissionEnd));
        }

        return PermissionKind.None;
    }

    private static PermissionKind ParsePermissions(string permissionString)
    {
        var permissions = PermissionKind.None;
        var permissionsList = permissionString.Split(PermissionDelimiter, StringSplitOptions.RemoveEmptyEntries);
        foreach (var permission in permissionsList)
        {
            if (Enum.TryParse(permission, true, out PermissionKind p))
            {
                permissions |= p;
            }
        }

        return permissions;
    }
}