using IAG.ProcessEngine.Execution.Condition;

using Xunit;

namespace IAG.ProcessEngine.Test.Execution.Condition;

public class ConditionParserTest
{
    private readonly ConditionParser _parser = new();

    [Fact]
    public void SimpleParseTest()
    {
        Assert.True(Assert.IsType<TrueCondition>(_parser.Parse("true")).Check(null));
        Assert.True(Assert.IsType<TrueCondition>(_parser.Parse("True")).Check(null));
        Assert.False(Assert.IsType<FalseCondition>(_parser.Parse("false")).Check(null));
        Assert.True(Assert.IsType<TrueCondition>(_parser.Parse("(true)")).Check(null));
        Assert.True(Assert.IsType<CompareCondition>(_parser.Parse("42=42")).Check(null));
        Assert.True(Assert.IsType<CompareCondition>(_parser.Parse("\"Test\"=\"Test\"")).Check(null));
        Assert.True(Assert.IsType<CompareCondition>(_parser.Parse("(((42=42)))")).Check(null));
        Assert.True(Assert.IsType<CompareCondition>(_parser.Parse("42!=23")).Check(null));
        Assert.True(Assert.IsType<CompareCondition>(_parser.Parse("42>23")).Check(null));
        Assert.True(Assert.IsType<CompareCondition>(_parser.Parse("42>=23")).Check(null));
        Assert.True(Assert.IsType<CompareCondition>(_parser.Parse("42<77")).Check(null));
        Assert.True(Assert.IsType<CompareCondition>(_parser.Parse("42<=77")).Check(null));
    }

    [Fact]
    public void ComplexParseTest()
    {
        Assert.True(Assert.IsType<AndCondition>(_parser.Parse("42=42 & true")).Check(null));
        Assert.True(Assert.IsType<AndCondition>(_parser.Parse("(42=42) & true")).Check(null));
        Assert.True(Assert.IsType<AndCondition>(_parser.Parse("(42=42 & true)")).Check(null));
        Assert.True(Assert.IsType<AndCondition>(_parser.Parse("42=42 & true | false")).Check(null));
        Assert.True(Assert.IsType<OrCondition>(_parser.Parse("42=42 | false")).Check(null));
        Assert.True(Assert.IsType<OrCondition>(_parser.Parse("(42=42) | false")).Check(null));
        Assert.True(Assert.IsType<OrCondition>(_parser.Parse("(42=42 | false)")).Check(null));
        Assert.True(Assert.IsType<OrCondition>(_parser.Parse("42=42 | false | true")).Check(null));
    }

    [Fact]
    public void ParseExceptionTest()
    {
        Assert.Throws<ParseException>(() => _parser.Parse("Nonsense"));
        Assert.Throws<ParseException>(() => _parser.Parse("(42=42"));
        Assert.Throws<ParseException>(() => _parser.Parse("42=42)"));
        Assert.Throws<ParseException>(() => _parser.Parse("(42=42))"));
        Assert.Throws<ParseException>(() => _parser.Parse("true)"));
        Assert.Throws<ParseException>(() => _parser.Parse("42!<42"));
        Assert.Throws<ParseException>(() => _parser.Parse("42!=\"Test"));
        Assert.Throws<ParseException>(() => _parser.Parse("\"Test!=42"));
    }
}