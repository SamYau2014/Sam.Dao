using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sam.DAO.Builder.Clause;
using Sam.DAO.Builder.Data;
using Sam.DAO.Entity;
using Sam.DAO.DAOException;


namespace Sam.DAO.Linq
{
    public static class ExpressionParser<T> where T : BaseEntity
    {
        public static Condition Parse(Expression<Func<T, bool>> expr)
        {
            return Parse(expr.Body, 0, true);
        }

        private static Condition Parse(Expression expr, int num,bool flag)
        {
            if (expr is BinaryExpression)
            {
                return ParseBinary((BinaryExpression)expr, num,flag);
            }
            if (expr is UnaryExpression)
            {
                if (expr.NodeType == ExpressionType.Not)
                {
                    UnaryExpression ue = ((UnaryExpression)expr);
                    return Parse(ue.Operand, num, !(flag ^ false));
                }
            }
            if (expr is MethodCallExpression)
            {
                return ParseMethodCall((MethodCallExpression)expr, flag);
            }

            throw new LinqException("不支持的表达式");
        }

        private static Condition ParseBinary(BinaryExpression e, int num,bool flag)
        {

            switch (e.NodeType)
            {
                case ExpressionType.Equal:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), GetCompareType(ExpressionType.Equal+(flag?0:22)), num);
                case ExpressionType.GreaterThan:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), GetCompareType(ExpressionType.GreaterThan + (flag ? 0 : 6)), num);
                case ExpressionType.GreaterThanOrEqual:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), GetCompareType(ExpressionType.GreaterThan + (flag ? 0 : 4)), num);
                case ExpressionType.LessThan:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), GetCompareType(ExpressionType.LessThan - (flag ? 0 : 4)), num);
                case ExpressionType.LessThanOrEqual:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), GetCompareType(ExpressionType.LessThanOrEqual - (flag ? 0 : 6)), num);
                case ExpressionType.NotEqual:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), GetCompareType(ExpressionType.NotEqual - (flag ? 0 : 22)), num);
                case ExpressionType.AndAlso:
                    return Parse(e.Left, ++num, flag) & Parse(e.Right, ++num, flag);
                case ExpressionType.OrElse:
                    return Parse(e.Left, ++num, flag) | Parse(e.Right, ++num, flag);
                default:
                    throw new LinqException("不支持的表达式类型");
            }
        }

        private static Condition ParseMethodCall(MethodCallExpression e,bool flag)
        {
            switch (e.Method.Name)
            {
                case "In":
                    return ParseInCall(e,flag);
                case "NotIn":
                    return ParseInCall(e, !(false^flag));
                case "Contains":
                    return ParseLikeCall(e, "%", "%", flag);
                case "StartsWith":
                    return ParseLikeCall(e, "", "%", flag);
                case "EndsWith":
                    return ParseLikeCall(e, "%", "", flag);
                case"Between":
                    return ParseBetweenCall(e, flag);
                case "NotBetween":
                    return ParseBetweenCall(e, !(false ^ flag));
                case "IsNull":
                    return ParseNullCall(e, flag);
                case "IsNotNull":
                    return ParseNullCall(e, !(false ^ flag));
                default:
                    throw new LinqException("不支持的扩展方法:" + e.Method.Name);
            }
        }

        private static Condition ParseInCall(MethodCallExpression e, bool inFlag)
        {
            string key = GetMemberName(e.Arguments[0]);
            var list = new List<object>();
            var ie = GetRightValue(e.Arguments[1]);
            if (ie is IEnumerable)
            {
                foreach (var obj in (IEnumerable)GetRightValue(e.Arguments[1]))
                {
                    list.Add(obj);
                }
            }
            else
            {
                list.Add(ie);
            }
            return new InClause(key, list.ToArray(), inFlag);
        }

        private static Condition ParseLikeCall(MethodCallExpression e, string left, string rigth,bool likeFlag)
        {
            string key = GetMemberName(e.Object);
            if (e.Arguments.Count == 1)
            {
                object value = GetRightValue(e.Arguments[0]);
                if (value is string)
                {
                    if (likeFlag)
                        return new KeyValueClause(key, left + value + rigth, "like");
                    return new KeyValueClause(key, left + value + rigth, "not like");
                }
            }
            throw new LinqException("Like扩展方法只支持非空字符串");
        }

        private static Condition ParseBetweenCall(MethodCallExpression e, bool flag)
        {
            string key = GetMemberName(e.Arguments[0]);
            object left = GetRightValue(e.Arguments[1]);
            object right = GetRightValue(e.Arguments[2]);
            return new BetweenClause(key, left, right, flag);

        }

        private static Condition ParseNullCall(MethodCallExpression e, bool flag)
        {
            return new NullClause(GetMemberName(e.Arguments[0]), flag);
        }

        private static object GetRightValue(Expression right)
        {
            return right.NodeType == ExpressionType.Constant
                       ? ((ConstantExpression) right).Value
                       : Expression.Lambda(right).Compile().DynamicInvoke();
        }

        private static string GetMemberName(Expression expr)
        {
            T obj = Activator.CreateInstance<T>();
            MemberExpression me = ((MemberExpression)expr);
            return obj.GetColumnName(me.Member.Name);
        }

        private static string GetCompareType(ExpressionType type)
        {
            switch(type)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                default:
                    throw new LinqException("不支持的表达式类型");
            }
        }
    }
}