using System;
using System.Collections.Generic;
using System.Linq;

using IAG.ProcessEngine.Execution.Model;
using IAG.ProcessEngine.Resource;

namespace IAG.ProcessEngine.Execution.Condition;

public class CompareCondition : ICondition
{
    private readonly OperationPart _leftPart;
    private readonly CompareOperator _op;
    private readonly OperationPart _rightPart;

    public CompareCondition(string leftPart, CompareOperator op, string rightPart)
    {
        _leftPart = new OperationPart(leftPart);
        _op = op;
        _rightPart = new OperationPart(rightPart);
    }

    public bool Check(IJobInstance jobInstance)
    {
        string leftValue = _leftPart.GetValue(jobInstance).Single();
        HashSet<string> rightParts = _rightPart.GetValue(jobInstance);

        switch (_op)
        {
            case CompareOperator.Equal:
                return rightParts.Contains(leftValue);
            case CompareOperator.NotEqual:
                return !rightParts.Contains(leftValue);
            case CompareOperator.Lower:
                return String.Compare(leftValue, rightParts.Single(), StringComparison.OrdinalIgnoreCase) < 0;
            case CompareOperator.LowerEqual:
                return String.Compare(leftValue, rightParts.Single(), StringComparison.OrdinalIgnoreCase) <= 0;
            case CompareOperator.Greater:
                return String.Compare(leftValue, rightParts.Single(), StringComparison.OrdinalIgnoreCase) > 0;
            case CompareOperator.GreaterEqual:
                return String.Compare(leftValue, rightParts.Single(), StringComparison.OrdinalIgnoreCase) >= 0;
            default:
                throw new ParseException(ResourceIds.ConditionParseExceptionUnknownOperator, _op);
        }
    }
}


public class OperationPart
{
    private enum OperationPartType
    {
        Constant,
        JobResult,
        ExecutionState
    }

    private readonly OperationPartType _type;
    private readonly HashSet<string> _value = new();

    public OperationPart(string part)
    {
        if (System.Enum.IsDefined(typeof(OperationPartType), part))
        {
            _type = System.Enum.Parse<OperationPartType>(part, true);
            if (_type == OperationPartType.Constant)
            {
                throw new ParseException(ResourceIds.ConditionParseExceptionInvalidExpression);
            }
        }
        else
        {
            _type = OperationPartType.Constant;
            var parts = part.Split(Operator.Or, StringSplitOptions.RemoveEmptyEntries);
            if (!parts.Any())
            {
                throw new ParseException(ResourceIds.ConditionParseExceptionEmptyExpression);
            }
            foreach (var p in parts)
            {
                if (p.StartsWith('"') && p.EndsWith('"'))
                {
                    _value.Add(p.Substring(1, p.Length - 2));
                }
                else if (int.TryParse(p, out _))
                {
                    _value.Add(p);
                }
                else
                {
                    throw new ParseException(ResourceIds.ConditionParseExceptionInvalidExpression);
                }
            }
        }
    }

    public HashSet<string> GetValue(IJobInstance jobInstance)
    {
        switch (_type)
        {
            case OperationPartType.JobResult:
                return new HashSet<string>() { ((int)jobInstance.State.Result.Result).ToString() };
            case OperationPartType.ExecutionState:
                return new HashSet<string>() { ((int)jobInstance.State.ExecutionState).ToString() };
            default:
                return _value;
        }
    }
}