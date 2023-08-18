using System.Linq.Expressions;
using SEVEN.MissionControl.Api.Models.Device;

namespace SEVEN.MissionControl.Api.Services.Device;

public class DqlQueryBuilder {
public static Expression<Func<TEntity, bool>> BuildQuery<TEntity>(List<DqlToken> queryElements)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "entity");
        Dictionary<string, List<Expression>> expressionGroups = new Dictionary<string, List<Expression>>();
        List<Expression> currentGroup = null;

        foreach (var element in queryElements)
        {
            if (element.Type == "property")
            {
                currentGroup = new List<Expression>();
                expressionGroups[element.Value] = currentGroup;
            }
            else if (element.Type == "operator")
            {
                if (currentGroup == null)
                {
                    throw new InvalidOperationException("Operator token must follow a property token.");
                }
            }
            else if (element.Type == "value")
            {
                if (currentGroup == null)
                {
                    throw new InvalidOperationException("Value token must follow an operator token.");
                }

                Expression left = Expression.PropertyOrField(parameter, currentGroup.First().ToString());
                Expression right = Expression.Constant(Convert.ChangeType(element.Value, left.Type));
                Expression binaryExpression;

                switch (currentGroup[1].ToString())
                {
                    case "==":
                        binaryExpression = Expression.Equal(left, right);
                        break;
                    // Weitere Operatoren können hier hinzugefügt werden
                    default:
                        throw new NotSupportedException($"Operator '{currentGroup[1]}' is not supported.");
                }

                currentGroup.Clear();
                currentGroup.Add(binaryExpression);
            }
            else if (element.Type == "logical")
            {
                if (currentGroup.Count != 1)
                {
                    throw new InvalidOperationException("Logical token must have exactly one operand.");
                }

                string logicalOperator = element.Value;

                foreach (var group in expressionGroups.Values)
                {
                    if (group.Count != 1)
                    {
                        throw new InvalidOperationException("Logical token must have exactly one operand.");
                    }
                }

                Expression[] operandExpressions = expressionGroups.Values.Select(group => group[0]).ToArray();
                Expression logicalExpression;

                if (logicalOperator == "AND")
                {
                    logicalExpression = operandExpressions.Aggregate((left, right) => Expression.AndAlso(left, right));
                }
                // Weitere logische Operatoren können hier hinzugefügt werden
                else
                {
                    throw new NotSupportedException($"Logical operator '{logicalOperator}' is not supported.");
                }

                currentGroup.Clear();
                currentGroup.Add(logicalExpression);
                expressionGroups.Clear();
            }
        }

        if (currentGroup?.Count != 1)
        {
            throw new InvalidOperationException("Invalid expression.");
        }

        return Expression.Lambda<Func<TEntity, bool>>(currentGroup.First(), parameter);
    }
}