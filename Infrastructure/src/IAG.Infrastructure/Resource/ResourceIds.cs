namespace IAG.Infrastructure.Resource;

public static class ResourceIds
{
    private const string ResourcePrefix = "Infrastructure.";

    public const string AtlasMapperMissingProperty = ResourcePrefix + "AtlasMapper.MissingProperty";
    public const string JsonRestResponseDeserializeError = ResourcePrefix + "JsonRestResponse.DeserializeError";
    public const string ResourceTemplateDuplicateExceptionMessage = ResourcePrefix + "ResourceTemplateDuplicateException.Message";
    public const string ConnectionToDatabaseFailed = ResourcePrefix + "ConnectionToDatabaseFailed";
    public const string CronInvalidString = ResourcePrefix + "Cron.InvalidString";
    public const string UserNotFound = ResourcePrefix + "UserNotFound";
    public const string PathNotFound = ResourcePrefix + "PathNotFound";
    public const string InvalidIsoCode = ResourcePrefix + "InvalidCountryCode";

    // Authentication
    public const string NotAuthenticatedExceptionMessage = ResourcePrefix + "NotAuthenticatedExceptionMessage";
    public const string PasswordCheckFailed = ResourcePrefix + "PasswordCheckFailed";
    public const string NoClaimsGranted = ResourcePrefix + "NoClaimsGranted";
        
    // DB-Error
    public const string DbUniqueConstraintExceptionMessage = ResourcePrefix + "DbException.UniqueConstraintException";
    public const string DbCannotInsertNullExceptionMessage = ResourcePrefix + "DbException.CannotInsertNullException";
    public const string DbMaxLengthExceededExceptionMessage = ResourcePrefix + "DbException.MaxLengthExceededException";
    public const string DbNumericOverflowExceptionMessage = ResourcePrefix + "DbException.NumericOverflowException";
    public const string DbUndefinedTableExceptionMessage = ResourcePrefix + "DbException.UndefinedTableException";
    public const string DbGenericExceptionMessage = ResourcePrefix + "DbException.GenericDbException";
    public const string DbSyntaxErrorMessage = ResourcePrefix + "DbException.DbSyntaxError";
    public const string DbCommitWithoutTransaction = ResourcePrefix + "DbException.DbCommitWithoutTransaction";
    public const string DbRecordNotFoundExceptionMessage = ResourcePrefix + "DbException.DbRecordNotFound";
    public const string DbNoRowUpdated = ResourcePrefix + "DbException.DbNoRowUpdated: Affected records: {0}";
    public const string DbNoRowDeleted = ResourcePrefix + "DbException.DbNoRowDeleted: Affected records: {0}";

    // Claim provider
    public const string ClaimProviderDuplicateExceptionMessage = ResourcePrefix + "ClaimProvider.DuplicateException";
    
    // Import/Export
    public const string ImportExportWrongTypeExceptionMessage = ResourcePrefix + "ImportExport.WrongTypeException";

    // Generic 
    public const string GenericError = ResourcePrefix + "GenericError";
    public const string GenericOk = ResourcePrefix + "GenericOk";

    // Consistency
    public const string ValueIsEmpty = ResourcePrefix + "ValueIsEmpty";
 
    // Logging
    private const string ResourcePrefixLog = ResourcePrefix + "Logging.";
    public const string RequestMessage = ResourcePrefixLog + "RequestMessage";
    public const string ResponseMessage = ResourcePrefixLog + "ResponseMessage";
}