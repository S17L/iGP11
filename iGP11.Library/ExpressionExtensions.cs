using System;
using System.Linq.Expressions;
using System.Reflection;

namespace iGP11.Library
{
    public static class ExpressionExtensions
    {
        public static MemberInfo GetMemberInfo<TComponent, TMember>(this Expression<Func<TComponent, TMember>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            MemberExpression memberExpression = null;
            var body = expression.Body as MemberExpression;
            if (body != null)
            {
                memberExpression = body;
            }
            else if (expression.Body is UnaryExpression)
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new NotSupportedException("expression is not supported");
            }

            return memberExpression.Member;
        }

        public static MemberInfo GetMemberInfo<TMember>(this Expression<Func<TMember>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            MemberExpression memberExpression = null;
            var body = expression.Body as MemberExpression;
            if (body != null)
            {
                memberExpression = body;
            }
            else if (expression.Body is UnaryExpression)
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new NotSupportedException("expression is not supported");
            }

            return memberExpression.Member;
        }

        public static string GetMemberName<TMember>(this Expression<Func<TMember>> expression)
        {
            return expression.GetMemberInfo().Name;
        }

        public static object GetSource<TDestination>(this Expression<Func<TDestination>> expression)
        {
            Expression body = expression;
            if (body is LambdaExpression)
            {
                body = ((LambdaExpression)body).Body;
            }

            if (body.NodeType != ExpressionType.MemberAccess)
            {
                throw new NotSupportedException($"expression: {expression} is not supported");
            }

            var valueExpression = ((MemberExpression)body).Expression;
            switch (valueExpression.NodeType)
            {
                case ExpressionType.Constant:
                    return ((ConstantExpression)valueExpression).Value;
                case ExpressionType.MemberAccess:
                    return Expression.Lambda(valueExpression).Compile().DynamicInvoke();
                default:
                    throw new NotSupportedException($"expression: {expression} is not supported");
            }
        }
    }
}