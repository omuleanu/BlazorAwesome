using Omu.BlazorAwesome.Core;
using System;
using System.Linq.Expressions;

namespace Omu.BlazorAwesome.Models.Utils;

internal static class AweExprUtil
{
    public static string GetExpressionStr(Expression expr)
    {
        MemberExpression memberExpr = null;

        if (expr is LambdaExpression lambda)
        {
            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }
        }
        else if (expr is MemberExpression)
        {
            memberExpr = expr as MemberExpression;
        }

        if (memberExpr == null)
            throw new ArgumentException("method");

        var res = memberExpr.Member.Name;

        if (memberExpr.Expression.NodeType == ExpressionType.MemberAccess)
        {
            res = GetExpressionStr(memberExpr.Expression) + "." + res;
        }

        return res;
    }

    public static MemberExpression GetBindMemberExpression<TModel>(ParameterExpression itmParam, string bind)
    {
        MemberExpression propAccess = null;
        if (bind.Contains("."))
        {
            var memberNames = bind.Split('.');

            var memberType = typeof(TModel);
            for (var i = 0; i < memberNames.Length; i++)
            {
                var property = memberType.GetProperty(memberNames[i]);
                memberType = property.PropertyType;

                if (propAccess is not null)
                {
                    propAccess = Expression.MakeMemberAccess(propAccess, property);
                }
                else
                {
                    propAccess = Expression.MakeMemberAccess(itmParam, property);
                }
            }
        }
        else
        {
            propAccess = Expression.Property(itmParam, typeof(TModel).GetProperty(bind));
        }

        return propAccess;
    }

    public static Expression<Func<TModel, bool>> ContainsExpr<TModel>(object val, string bindVal)
    {
        bool ignoreCase = AweSettings.StringFilterIgnoreCase;

        var sval = (string)val;

        var itmParam = Expression.Parameter(typeof(TModel));

        MemberExpression propAccess = AweExprUtil.GetBindMemberExpression<TModel>(itmParam, bindVal);

        var valConstExpr = Expression.Constant(sval, typeof(string));

        var compConstExpr = Expression.Constant(StringComparison.InvariantCultureIgnoreCase);

        var containsParamTypes = ignoreCase ? new[] { typeof(string), typeof(StringComparison) } : new[] { typeof(string) };

        var containsMethodInfo = typeof(string).GetMethod(nameof(string.Contains), containsParamTypes);

        var contCallExpressions = ignoreCase ? new[] { valConstExpr, compConstExpr } : new[] { valConstExpr };

        var containsExpr = Expression.Call(propAccess, containsMethodInfo, contCallExpressions);

        return Expression.Lambda<Func<TModel, bool>>(containsExpr, itmParam);
    }
}