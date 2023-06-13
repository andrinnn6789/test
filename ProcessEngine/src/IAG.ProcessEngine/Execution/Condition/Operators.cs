namespace IAG.ProcessEngine.Execution.Condition;

public enum LogicalOperator
{
    None,
    And,
    Or
}

public enum CompareOperator
{
    Lower,
    LowerEqual,
    Equal,
    NotEqual,
    GreaterEqual,
    Greater
}

public static class Operator
{
    public const char And = '&';
    public const char Or = '|';
    public const char Equal = '=';
    public const char Lower = '<';
    public const char Greater = '>';
    public const char Not = '!';

    public static readonly char[] CompareOperators = {Not, Equal, Lower, Greater};
}