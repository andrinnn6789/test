using System.Diagnostics.CodeAnalysis;

using IAG.Infrastructure.Authorization;
using IAG.Infrastructure.Globalisation.Enum;
using IAG.Infrastructure.Globalisation.ResourceProvider;
using IAG.Infrastructure.IdentityServer.Authorization.Model;

using JetBrains.Annotations;

namespace IAG.Infrastructure.Resource;

[UsedImplicitly]
[ExcludeFromCodeCoverage]
public class ResourceProviderInfrastructure : ResourceProvider
{
    public ResourceProviderInfrastructure()
    {
        AddTemplate(ResourceIds.AtlasMapperMissingProperty, "en", "Failed to get property '{0}'");
        AddTemplate(ResourceIds.JsonRestResponseDeserializeError, "en", "Failed to deserialize response");
        AddTemplate(ResourceIds.ResourceTemplateDuplicateExceptionMessage, "en", "Resource template with ID '{0}' and language '{1}' was already added");
        AddTemplate(ResourceIds.ConnectionToDatabaseFailed, "en", "Connection to Database failed({0})");
        AddTemplate(ResourceIds.CronInvalidString, "en", "Invalid Cron Expression: {0}");
        AddTemplate(ResourceIds.UserNotFound, "en", "Username {0} not found");
        AddTemplate(ResourceIds.PathNotFound, "en", "Path {0} not found");
        AddTemplate(ResourceIds.InvalidIsoCode, "en", "Code {0} is not a valid iso code");
        AddTemplate(ResourceIds.InvalidIsoCode, "de", "Code {0} ist kein gültiger Iso-Code");

        // Authentication
        AddTemplate(ResourceIds.PasswordCheckFailed, "en", "Password check for user {0} failed");
        AddTemplate(ResourceIds.NotAuthenticatedExceptionMessage, "en", "Authentication failed");
        AddTemplate(ResourceIds.NoClaimsGranted, "en", "No claims are granted");

        // DbExceptions
        AddTemplate(ResourceIds.DbUniqueConstraintExceptionMessage, "en", "Unique constraint violation");
        AddTemplate(ResourceIds.DbCannotInsertNullExceptionMessage, "en", "Cannot insert null");
        AddTemplate(ResourceIds.DbMaxLengthExceededExceptionMessage, "en", "Maximum length exceeded");
        AddTemplate(ResourceIds.DbNumericOverflowExceptionMessage, "en", "Numeric overflow");
        AddTemplate(ResourceIds.DbUndefinedTableExceptionMessage, "en", "Undefined table");
        AddTemplate(ResourceIds.DbRecordNotFoundExceptionMessage, "en", "No records in table {0} found");

        // Generic
        AddTemplate(ResourceIds.GenericError, "en", "Error in program: {0}");
        AddTemplate(ResourceIds.GenericOk, "en", "No errors");

        // Claim provider
        AddTemplate(ResourceIds.ClaimProviderDuplicateExceptionMessage, "en", "Claim '{0}' from scope '{1}' and tenant '{2}' in claims provider '{3}' was already defined by provider '{4}')");

        // Import/Export
        AddTemplate(ResourceIds.ImportExportWrongTypeExceptionMessage, "en", "Wrong type (expected: '{0}', actual: '{1}'");

        // Consistency
        AddTemplate(ResourceIds.ValueIsEmpty, "en", "Value for '{0}' is empty ({1})");
        AddTemplate(ResourceIds.ValueIsEmpty, "de", "Der Wert für '{0}' ist leer ({1})");
        AddTemplate(ResourceIds.ValueIsEmpty, "fr", "La valeur de '{0}' est vide ({1})");
        AddTemplate(ResourceIds.ValueIsEmpty, "it", "Il valore per '{0}' è vuoto ({1})");

        // Logging
        AddTemplate(ResourceIds.RequestMessage, "en", "Send HttpRequest:\r\n{0}");
        AddTemplate(ResourceIds.ResponseMessage, "en", "Received HttpResponse:\r\n{0}");

        // Scopes
        AddTemplate(ScopeNamesInfrastructure.AdminScope, "en", "Scope 'Admin' for infrastructure");
        AddTemplate(ScopeNamesInfrastructure.ReaderScope, "en", "Scope 'Reader' for infrastructure");
        AddTemplate(ScopeNamesInfrastructure.AtlasScope, "en", "Scope 'Atlas' for infrastructure");
        AddTemplate(ScopeNamesInfrastructure.ProcessEngine, "en", "Scope 'Reader' for infrastructure");

        // Claims
        AddTemplate(ClaimNamesInfrastructure.GeneralClaim, "en", "General claim for infrastructure");

        // enums
        AddEnumTemplates(typeof(MessageTypeEnum));
        AddEnumTemplates(typeof(PermissionKind));
    }
}