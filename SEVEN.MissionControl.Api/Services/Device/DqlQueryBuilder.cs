using System.Linq.Expressions;
using SEVEN.MissionControl.Api.Models.Device;

namespace SEVEN.MissionControl.Api.Services.Device;

public class DqlQueryBuilder {
    public static Dictionary<string, List<DqlToken>> GroupTokens(List<DqlToken> tokens) {
        var groups = new Dictionary<string, List<DqlToken>>();
        var currentGroup = new List<DqlToken>();

        var combinerGuid = Guid.NewGuid();

        foreach (var token in tokens) {
            if (token.Type == "logical") {
                if (currentGroup.Count > 0) {
                    groups.Add($"propertygroup_{combinerGuid}", currentGroup);
                    currentGroup = new List<DqlToken>();
                }

                combinerGuid = Guid.NewGuid();

                groups.Add($"logical_{combinerGuid}", new List<DqlToken> {
                    token
                });
            }
            else {
                currentGroup.Add(token);
            }
        }

        if (currentGroup.Count > 0) {
            groups.Add($"propertygroup_{combinerGuid}", currentGroup);
        }

        return groups;
    }

    public Expression<Func<TEntity, bool>> BuildExpression<TEntity>(Dictionary<string, List<DqlToken>> groups) {
        var parameter = Expression.Parameter(typeof(TEntity), "entity");

        var propertyExpressions = new List<Expression>();
        var logicalExpressions = new List<ExpressionType>();

        foreach (var group in groups) {
            if (group.Key.Contains("property")) {
                var propertyExpression = BuildPropertyExpression<TEntity>(group.Value, parameter);
                propertyExpressions.Add(propertyExpression);
            }
            else if (group.Key.Contains("logical")) {
                var logicalExpressionType = BuildLogicalOperatorExpression(group.Value[0].Value);
                logicalExpressions.Add(logicalExpressionType);
            }
        }

        return CombineExpressionsWithLogicalOperators<TEntity>(propertyExpressions, logicalExpressions, parameter);
    }

    private static Expression<Func<TEntity, bool>> CombineExpressionsWithLogicalOperators<TEntity>(IReadOnlyList<Expression> expressions,
        IReadOnlyList<ExpressionType> operators, ParameterExpression parameter) {
        if (expressions.Count == 0)
            throw new ArgumentException("List of expressions is empty.");

        if (expressions.Count != operators.Count + 1)
            throw new ArgumentException("Number of operators should be one less than the number of expressions.");

        var combinedExpression = expressions[0];

        for (var i = 0; i < operators.Count; i++) {
            combinedExpression = Expression.MakeBinary(operators[i], combinedExpression, expressions[i + 1]);
        }

        var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);

        return lambdaExpression;
    }

    private Expression BuildPropertyExpression<TEntity>(IReadOnlyList<DqlToken> group, ParameterExpression parameter) {
        var propertyName = group[0].Value;
        var operatorToken = group[1].Value;
        var propertyValueString = group[2].Value;

        var propertyInfo = typeof(TEntity).GetProperty(propertyName);
        if (propertyInfo == null) {
            throw new InvalidOperationException($"Property {propertyName} not found on type {typeof(TEntity).FullName}");
        }

        var propertyType = propertyInfo.PropertyType;
        var propertyValue = ConvertToPropertyValue(propertyValueString, propertyType);

        var propertyAccessExpr = Expression.PropertyOrField(parameter, propertyName);
        var operatorExpr = BuildOperatorExpression(propertyAccessExpr, operatorToken, propertyValue);

        return operatorExpr;
    }

    private static Expression BuildOperatorExpression(Expression propertyAccessExpr, string operatorToken, object propertyValue) {
        return operatorToken switch {
            "==" => Expression.Equal(propertyAccessExpr, Expression.Constant(propertyValue)),
            "!=" => Expression.NotEqual(propertyAccessExpr, Expression.Constant(propertyValue)),
            ">" => Expression.GreaterThan(propertyAccessExpr, Expression.Constant(propertyValue)),
            "<" => Expression.LessThan(propertyAccessExpr, Expression.Constant(propertyValue)),
            ">=" => Expression.GreaterThanOrEqual(propertyAccessExpr, Expression.Constant(propertyValue)),
            "<=" => Expression.LessThanOrEqual(propertyAccessExpr, Expression.Constant(propertyValue)),
            _ => throw new InvalidOperationException($"Operator {operatorToken} is not supported.")
        };
    }

    private static ExpressionType BuildLogicalOperatorExpression(string operatorToken) {
        return operatorToken switch {
            "AND" => ExpressionType.AndAlso,
            "OR" => ExpressionType.OrElse,
            _ => throw new InvalidOperationException($"Logical operator {operatorToken} is not supported.")
        };
    }

    private static object ConvertToPropertyValue(string propertyValueString, Type propertyType) {
        if (propertyType == typeof(Guid)) {
            if (Guid.TryParse(propertyValueString, out var guidValue)) {
                return guidValue;
            }

            throw new InvalidOperationException($"Invalid Guid format: {propertyValueString}");
        }

        if (propertyType.IsEnum) {
            if (Enum.TryParse(propertyType, propertyValueString, out var enumValue)) {
                return enumValue;
            }

            throw new InvalidOperationException($"Invalid enum value: {propertyValueString}");
        }

        return Convert.ChangeType(propertyValueString, propertyType);
    }
}