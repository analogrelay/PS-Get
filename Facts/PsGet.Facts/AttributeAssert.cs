using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Linq.Expressions;
using System.Reflection;

namespace PsGet.Facts   
{
    public static class AttributeAssert
    {
        public static void Has<T>(MemberInfo member, T attr) where T : Attribute
        {
            T[] attrs = (T[])member.GetCustomAttributes(typeof(T), inherit: false);
            Assert.Equal(1, attrs.Length);
            Assert.Equal(attr, attrs[0]);
        }

        public static void Has<T>(Expression<Func<object>> expr, T attr) where T : Attribute
        {
            Has(ExtractMember(expr), attr);
        }

        private static MemberInfo ExtractMember(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Lambda:
                    return ExtractMember(((LambdaExpression)expr).Body);
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expr).Member;
                case ExpressionType.Convert:
                    return ExtractMember(((UnaryExpression)expr).Operand);
            }
            throw new NotSupportedException(String.Format("Expression of type {0}[{1}] is not supported", expr.NodeType, expr.GetType().Name));
        }
    }
}
