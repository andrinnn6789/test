using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace IAG.Infrastructure.IdentityServer.Authorization.PolicyAuthorization;

public class ClaimAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly IAuthorizationPolicyProvider _backupPolicyProvider;

    public ClaimAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _backupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(ClaimAuthorizationAttribute.PolicyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var requirement = ClaimAuthorizationRequirement.FromString(policyName.Substring(ClaimAuthorizationAttribute.PolicyPrefix.Length));
            var policyBuilder = new AuthorizationPolicyBuilder();
            policyBuilder.AddRequirements(requirement);

            return Task.FromResult(policyBuilder.Build());
        }

        return Task.FromResult<AuthorizationPolicy>(null);
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return await _backupPolicyProvider.GetDefaultPolicyAsync();
    }

    public async Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    {
        return await _backupPolicyProvider.GetFallbackPolicyAsync();
    }
}