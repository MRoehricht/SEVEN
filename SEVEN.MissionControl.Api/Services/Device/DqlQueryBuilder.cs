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

        Expression<Func<TEntity, bool>> finalExpression = null;

        foreach (var group in groups) {
            if (group.Key.Contains("property")) {
                var propertyExpression = BuildPropertyExpression<TEntity>(group.Value, parameter);

                var logicalGroupKey = group.Key.Split("_")[1];
                var logicalGroup = groups.TryGetValue($"logical_{logicalGroupKey}", out var logicalTokens)
                    ? logicalTokens
                    : new List<DqlToken>();

                finalExpression = finalExpression == null
                    ? propertyExpression
                    : CombineExpressions(finalExpression, propertyExpression, logicalGroup);
            }
        }

        return finalExpression;
    }

    private Expression<Func<TEntity, bool>> CombineExpressions<TEntity>(Expression<Func<TEntity, bool>> left, Expression<Func<TEntity, bool>> right,
        IReadOnlyList<DqlToken> logicalTokens) {
        var logicalExpression = logicalTokens[0].Value;
        var leftExpr = left.Body;
        var rightExpr = right.Body;

        var combinedExpression = logicalExpression switch {
            "AND" => Expression.AndAlso(leftExpr, rightExpr),
            "OR" => Expression.OrElse(leftExpr, rightExpr),
            _ => throw new InvalidOperationException($"Logical operator {logicalExpression} is not supported.")
        };

        return Expression.Lambda<Func<TEntity, bool>>(combinedExpression, left.Parameters[0]);
    }

    private Expression<Func<TEntity, bool>> BuildPropertyExpression<TEntity>(IReadOnlyList<DqlToken> group, ParameterExpression parameter) {
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

        return Expression.Lambda<Func<TEntity, bool>>(operatorExpr, parameter);
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