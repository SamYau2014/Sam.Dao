using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sam.DAO.Builder.Clause;
using Sam.DAO.Builder.Data;
using Sam.DAO.Entity;
using Sam.DAO.InnerException;


namespace Sam.DAO.Linq
{
    public static class ExpressionParser<T> where T : BaseEntity
    {
        #region old code
        //public static Condition Parse(Expression<Func<T, bool>> expr)
        //{
        //    return Parse(expr.Body);
        //}

        // private static Condition Parse(Expression expr)
        //{
        //    if (expr is BinaryExpression)
        //    {
        //        return ParseBinary((BinaryExpression)expr);
        //    }
        //    else if (expr is MethodCallExpression)
        //    {
        //        return ParseMethodCall((MethodCallExpression)expr);
        //    }
        //    else
        //        throw new LinqException("不支持的表达式");
        //}

        //private static Condition ParseBinary(BinaryExpression e)
        //{
        //    switch (e.NodeType)
        //    {
        //        case ExpressionType.Equal:
        //            return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), "=");
        //        case ExpressionType.NotEqual:
        //            return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), "<>");
        //        case ExpressionType.GreaterThan:
        //            return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), ">");
        //        case ExpressionType.GreaterThanOrEqual:
        //            return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), ">=");
        //        case ExpressionType.LessThan:
        //            return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), "<");
        //        case ExpressionType.LessThanOrEqual:
        //            return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), "<=");
        //        case ExpressionType.AndAlso:
        //            return Parse(e.Left) & Parse(e.Right);
        //        case ExpressionType.OrElse:
        //            return Parse(e.Left) | Parse(e.Right);
        //        default:
        //            throw new LinqException("不支持的表达式表达式类型");
        //    }
        //}
        #endregion

        public static Condition Parse(Expression<Func<T, bool>> expr, int num = 0)
        {
            return Parse(expr.Body, num);
        }


        private static Condition Parse(Expression expr, int num)
        {
            if (expr is BinaryExpression)
            {
                return ParseBinary((BinaryExpression)expr, num);
            }
            if (expr is MethodCallExpression)
            {
                return ParseMethodCall((MethodCallExpression)expr);
            }
            throw new LinqException("不支持的表达式");
        }

        private static Condition ParseBinary(BinaryExpression e, int num)
        {

            switch (e.NodeType)
            {
                case ExpressionType.Equal:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), "=", num);
                case ExpressionType.NotEqual:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), "<>", num);
                case ExpressionType.GreaterThan:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), ">", num);
                case ExpressionType.GreaterThanOrEqual:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), ">=", num);
                case ExpressionType.LessThan:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), "<", num);
                case ExpressionType.LessThanOrEqual:
                    return new KeyValueClause(GetMemberName(e.Left), GetRightValue(e.Right), "<=", num);
                case ExpressionType.AndAlso:
                    return Parse(e.Left, ++num) & Parse(e.Right, ++num);
                case ExpressionType.OrElse:
                    return Parse(e.Left, ++num) | Parse(e.Right, ++num);
                default:
                    throw new LinqException("不支持的表达式表达式类型");
            }
        }

        private static Condition ParseMethodCall(MethodCallExpression e)
        {
            switch (e.Method.Name)
            {
                case "In":
                    return ParseInCall(e);
                case "NotIn":
                    return ParseInCall(e, false);
                case "Contains":
                    return ParseLikeCall(e, "%", "%");
                case "StartsWith":
                    return ParseLikeCall(e, "", "%");
                case "EndsWith":
                    return ParseLikeCall(e, "%", "");
                case"Between":
                    return ParseBetweenCall(e);
                case "NotBetween":
                    return ParseBetweenCall(e,false);
                case "IsNull":
                    return ParseNullCall(e);
                case "IsNotNull":
                    return ParseNullCall(e, false);
                default:
                    throw new LinqException("不支持的扩展方法:" + e.Method.Name);
            }
        }

        private static Condition ParseInCall(MethodCallExpression e, bool inFlag = true)
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

        private static Condition ParseLikeCall(MethodCallExpression e, string left, string rigth)
        {
            string key = GetMemberName(e.Object);
            if (e.Arguments.Count == 1)
            {
                object value = GetRightValue(e.Arguments[0]);
                if (value is string)
                    return new KeyValueClause(key, left + value + rigth, "like");
            }
            throw new LinqException("Like扩展方法只支持非空字符串");
        }

        private static Condition ParseBetweenCall(MethodCallExpression e, bool flag = true)
        {
            string key = GetMemberName(e.Arguments[0]);
            object left = GetRightValue(e.Arguments[1]);
            object right = GetRightValue(e.Arguments[2]);
            return new BetweenClause(key, left, right, flag);

        }

        private static Condition ParseNullCall(MethodCallExpression e, bool flag = true)
        {
            string key = GetMemberName(e.Arguments[0]);
            return new NullClause(key, flag);
        }

        private static object GetRightValue(Expression right)
        {
            object value
                = right.NodeType == ExpressionType.Constant
                      ? ((ConstantExpression)right).Value
                      : Expression.Lambda(right).Compile().DynamicInvoke();
            return value;
        }

        public static string GetMemberName(Expression expr)
        {
            T obj = Activator.CreateInstance<T>();
            MemberExpression me = ((MemberExpression)expr);
            return obj.GetColumnName(me.Member.Name);
        }
    }
}