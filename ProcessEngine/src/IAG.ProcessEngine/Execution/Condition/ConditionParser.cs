using System;
using System.Linq;

using IAG.ProcessEngine.Resource;

namespace IAG.ProcessEngine.Execution.Condition;

public class ConditionParser : IConditionParser
{
    public ICondition Parse(string condition)
    {
        condition = RemoveParentheses(condition);
        LogicalOperator logicalOp = GetLogicOperationParts(condition, out var first, out var rest);
        if (logicalOp != LogicalOperator.None)
        {
            var firstCondition = Parse(first);
            var secondCondition = Parse(rest);
            return logicalOp == LogicalOperator.And
                ? new AndCondition(firstCondition, secondCondition)
                : new OrCondition(firstCondition, secondCondition);
        }

        var operationPos = condition.IndexOfAny(Operator.CompareOperators);
        if (operationPos < 0)
        {
            return HandleConstantCondition(condition);
        }

        var leftSide = condition.Substring(0, operationPos);
        char op = condition[operationPos];
        var rightSide = condition.Substring(operationPos + 1);
        var compareOp = PrepareCompareOperation(ref leftSide, op, ref rightSide);

        return new CompareCondition(leftSide, compareOp, rightSide);
    }

    private LogicalOperator GetLogicOperationParts(string condition, out string first, out string rest)
    {
        var level = 0;
        bool inString = false;
        int pos;
        for (pos = 0; pos < condition.Length; pos++)
        {
            if (condition[pos] == '"')
            {
                inString = ! inString;
                continue;
            }
            if (inString)
            {
                continue;
            }

            if (condition[pos] == '(')
            {
                level++;
            }
            else if (condition[pos] == ')')
            {
                level--;
            }
            else if ((condition[pos] == Operator.And || condition[pos] == Operator.Or) && (level == 0))
            {
                break;
            }
        }

        if (inString)
        {
            throw new ParseException(ResourceIds.ConditionParseExceptionMissingClosingQuote);
        }

        if (level != 0)
        {
            throw new ParseException(ResourceIds.ConditionParseExceptionParenthesisMismatch, level);
        }

        if (pos == condition.Length)
        {
            first = condition;
            rest = string.Empty;
            return LogicalOperator.None;
        }

        first = condition.Substring(0, pos);
        rest = condition.Substring(pos+1);
        return condition[pos] == Operator.And ? LogicalOperator.And : LogicalOperator.Or;
    }

    private ICondition HandleConstantCondition(string condition)
    {
        if (condition.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
        {
            return new TrueCondition();
        }

        if (condition.Equals(bool.FalseString, StringComparison.InvariantCultureIgnoreCase))
        {
            return new FalseCondition();
        }

        throw new ParseException(ResourceIds.ConditionParseExceptionUnknownCondition, condition);
    }

    private CompareOperator PrepareCompareOperation(ref string leftSide, char op, ref string rightSide)
    {
        CompareOperator compareOp;
        if (op == Operator.Equal)
        {
            compareOp = CompareOperator.Equal;
        }
        else if (op == Operator.Not)
        {
            if (rightSide.First() != Operator.Equal)
            {
                throw new ParseException(ResourceIds.ConditionParseExceptionInvalidOperatorUsage);
            }

            rightSide = rightSide.Substring(1);
            compareOp = CompareOperator.NotEqual;
        }
        else if (rightSide.First() == Operator.Equal)
        {
            rightSide = rightSide.Substring(1);
            compareOp = op == Operator.Greater ? CompareOperator.GreaterEqual : CompareOperator.LowerEqual;
        }
        else
        {
            compareOp = op == Operator.Greater ? CompareOperator.Greater : CompareOperator.Lower;
        }

        leftSide = RemoveParentheses(leftSide);
        rightSide = RemoveParentheses(rightSide);

        return compareOp;
    }

    private string RemoveParentheses(string s)
    {
        s = s.Trim();
        while (s.StartsWith('(') && s.EndsWith(')'))
        {
            s = s.Substring(1, s.Length - 2).Trim();
        }

        return s;
    }
}