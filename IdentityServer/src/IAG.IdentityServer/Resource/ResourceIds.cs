namespace IAG.IdentityServer.Resource;

internal static class ResourceIds
{
    private const string ResourcePrefix = "IdentityServer.";

    internal const string TokenHandlerConfigErrorExpiration = ResourcePrefix + "TokenHandler.ConfigError.Expiration";

    internal const string TokenHandlerConfigErrorClockSkew = ResourcePrefix + "TokenHandler.ConfigError.ClockSkew";

    internal const string MailConfigErrorTemplate = ResourcePrefix + "Mail.ConfigError.Template";

    internal const string MailConfigErrorServer = ResourcePrefix + "Mail.ConfigError.Server";

    internal const string ResetPasswordErrorNoEMail = ResourcePrefix + "ResetPassword.Error.NoEMail";

    internal const string ChangePasswordErrorPolicyViolation = ResourcePrefix + "ChangePassword.Error.PolicyViolation";

    internal const string AttackDetectionWarning = ResourcePrefix + "AttackDetection.Warning";

    internal const string PasswordPolicyErrorRequiredLength = ResourcePrefix + "PasswordPolicy.Error.RequiredLength";

    internal const string PasswordPolicyErrorRequiredUniqueChars = ResourcePrefix + "PasswordPolicy.Error.RequiredUniqueChars";

    internal const string PasswordPolicyErrorRequireNonAlphanumeric = ResourcePrefix + "PasswordPolicy.Error.RequireNonAlphanumeric";

    internal const string PasswordPolicyErrorRequireLowercase = ResourcePrefix + "PasswordPolicy.Error.RequireLowercase";

    internal const string PasswordPolicyErrorRequireUppercase = ResourcePrefix + "PasswordPolicy.Error.RequireUppercase";

    internal const string PasswordPolicyErrorRequireDigit = ResourcePrefix + "PasswordPolicy.Error.RequireDigit";

    internal const string RealmNotFoundException = ResourcePrefix + "Realm.NotFoundException";
    internal const string AuthenticationPluginNotFoundException = ResourcePrefix + "AuthenticationPlugin.NotFoundException";
    internal const string RefreshTokenException = ResourcePrefix + "Realm.RefreshTokenException";
}