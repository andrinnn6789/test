using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Globalisation.ResourceProvider;

using JetBrains.Annotations;

namespace IAG.IdentityServer.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProviderProcessEngine : ResourceProvider
{
    public ResourceProviderProcessEngine()
    {
        AddTemplate(ResourceIds.TokenHandlerConfigErrorExpiration, "en", "No or invalid expiration duration for authenticator manager configured");
        AddTemplate(ResourceIds.TokenHandlerConfigErrorClockSkew, "en", "No or invalid token validation clock screw for authenticator manager configured");
            
        AddTemplate(ResourceIds.MailConfigErrorTemplate, "en", "No templates configured for realm {0}");
        AddTemplate(ResourceIds.MailConfigErrorServer, "en", "No SMTP server configured");
            
        AddTemplate(ResourceIds.ResetPasswordErrorNoEMail, "en", "User has no e-mail address");
        AddTemplate(ResourceIds.ChangePasswordErrorPolicyViolation, "en", "The password does not fulfill the configured password policy"); AddTemplate(ResourceIds.PasswordPolicyErrorRequiredLength, "en", "Required length {0}");

        AddTemplate(ResourceIds.AttackDetectionWarning, "en", "Too many requests. Patience you must have my young hacker...");
            
        AddTemplate(ResourceIds.PasswordPolicyErrorRequiredUniqueChars, "en", "Required unique chars {0}");
        AddTemplate(ResourceIds.PasswordPolicyErrorRequireNonAlphanumeric, "en", "Non alphanumeric required");
        AddTemplate(ResourceIds.PasswordPolicyErrorRequireLowercase, "en", "Lower case required");
        AddTemplate(ResourceIds.PasswordPolicyErrorRequireUppercase, "en", "Upper case required");
        AddTemplate(ResourceIds.PasswordPolicyErrorRequireDigit, "en", "Digit required");

        AddTemplate(ResourceIds.RealmNotFoundException, "en", "Realm '{0}' not found");
        AddTemplate(ResourceIds.AuthenticationPluginNotFoundException, "en", "Authentication plugin '{0}' not found");
        AddTemplate(ResourceIds.RefreshTokenException, "en", "Refresh token is invalid");
    }
}