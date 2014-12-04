#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Sam.DAO.Attribute;
using Sam.DAO.Entity;
using Sam.DAO.Linq;
using Sam.DAO.Tool;

namespace Sam.DAO.ExFunc
{
    public static class AggregateFunc
    {
        private const string SumSql = "select sum({0}) from {1} {2}";
        private const string MinSql = "select min({0}) from {1} {2}";
        private const string MaxSql = "select max({0}) from {1} {2}";
        private const string AvgSql = "select avg({0}) from {1} {2}";

        public static object Sum<T>(this IBaseServer server, string propertieName, Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
            var sqlinfo = BuildSqlInfo(server, SumSql, propertieName, func);
            return server.ExecuteScalar(sqlinfo);
        }

        public static object Min<T>(this IBaseServer server, string propertieName, Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
            var sqlinfo = BuildSqlInfo(server, MinSql, propertieName, func);
            return server.ExecuteScalar(sqlinfo);
        }

        public static object Max<T>(this IBaseServer server, string propertieName, Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
            var sqlinfo = BuildSqlInfo(server, MaxSql, propertieName, func);
            return server.ExecuteScalar(sqlinfo);
        }

        public static object Avg<T>(this IBaseServer server, string propertieName, Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
            var sqlinfo = BuildSqlInfo(server, AvgSql, propertieName, func);
            return server.ExecuteScalar(sqlinfo);
        }

        /// <summary>
        /// 求和
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOut">输出类型</typeparam>
        /// <param name="server"></param>
        /// <param name="propertie">属性</param>
        /// <param name="func">查询条件</param>
        /// <returns></returns>
        public static TOut Sum<T, TOut>(this IBaseServer server, Expression<Func<T, TOut>> propertie, Expression<Func<T, bool>> func)
            where T : BaseEntity, new()
            where TOut : struct
        {
            var sqlinfo = BuildSqlInfo(server, SumSql, propertie, func);
            return (TOut)server.ExecuteScalar(sqlinfo);
        }

        /// <summary>
        /// 求最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOut">输出类型</typeparam>
        /// <param name="server"></param>
        /// <param name="propertie">属性</param>
        /// <param name="func">查询条件</param>
        /// <returns></returns>
        public static TOut Min<T, TOut>(this IBaseServer server, Expression<Func<T, TOut>> propertie, Expression<Func<T, bool>> func)
            where T : BaseEntity, new()
            where TOut : struct
        {
            var sqlinfo = BuildSqlInfo(server, MinSql, propertie, func);
            return (TOut)server.ExecuteScalar(sqlinfo);
        }

        /// <summary>
        /// 求最大值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOut">输出类型</typeparam>
        /// <param name="server"></param>
        /// <param name="propertie">属性</param>
        /// <param name="func">查询条件</param>
        /// <returns></returns>
        public static TOut Max<T, TOut>(this IBaseServer server, Expression<Func<T, TOut>> propertie, Expression<Func<T, bool>> func)
            where T : BaseEntity, new()
            where TOut : struct
        {
            var sqlinfo = BuildSqlInfo(server, MaxSql, propertie, func);
            return (TOut)server.ExecuteScalar(sqlinfo);
        }

        /// <summary>
        /// 求平均值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOut">输出类型</typeparam>
        /// <param name="server"></param>
        /// <param name="propertie">属性</param>
        /// <param name="func">查询条件</param>
        /// <returns></returns>
        public static TOut Avg<T, TOut>(this IBaseServer server, Expression<Func<T, TOut>> propertie, Expression<Func<T, bool>> func)
            where T : BaseEntity, new()
            where TOut : struct
        {
            var sqlinfo = BuildSqlInfo(server, AvgSql, propertie, func);
            return (TOut)server.ExecuteScalar(sqlinfo);
        }

        private static SqlInfo BuildSqlInfo<T>(IBaseServer server, string sqlType, string propertieName, Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
            if (string.IsNullOrEmpty(propertieName)) throw new ArgumentNullException("propertieName");
            T entity = new T();
            var property = entity.GetType().GetProperty(propertieName);
            if (property == null)
                throw new DAOException.LinqException(propertieName + " is not " + entity.GetType() + "'s propertie");
            if (!TypeAdapter.IsNetNumberType(property.GetType()))
                throw new DAOException.LinqException(propertieName + " is not NumberType.");
            string columnName;
            object[] attributes = property.GetCustomAttributes(typeof(PropertyAttribute), false);
            if (attributes.Length == 0)
                columnName = propertieName;
            else
            {
                PropertyAttribute propertyAttribute = (PropertyAttribute)attributes[0];
                columnName = propertyAttribute == null ? propertieName : propertyAttribute.Name;
            }
            SqlInfo sqlinfo = new SqlInfo();
            string whereSql = string.Empty;
            if (func != null)
            {
                ICollection<DbParameter> parameters = new List<DbParameter>();
                whereSql = ExpressionParser<T>.Parse(func).ToSql(ref parameters, server.GetDbHelper());
                if (whereSql != string.Empty)
                {
                    whereSql = " where " + whereSql;
                    sqlinfo.Parameters = parameters;
                }
            }
            sqlinfo.Sql = string.Format(sqlType, entity.GetColumnName(columnName), entity.GetTableName(), whereSql);
            return sqlinfo;
        }

        private static SqlInfo BuildSqlInfo<T, TOut>(IBaseServer server, string sqlType, Expression<Func<T, TOut>> propertie, Expression<Func<T, bool>> func) where T : Entity.BaseEntity, new()
        {
            if (propertie == null) throw new ArgumentNullException("propertie");
            T entity = new T();
            string columnName = ExpressionRouter(propertie.Body, entity);
            if (string.IsNullOrEmpty(columnName)) throw new DAOException.LinqException("propertie format error");
            SqlInfo sqlinfo = new SqlInfo();
            string whereSql = string.Empty;
            if (func != null)
            {
                ICollection<DbParameter> parameters = new List<DbParameter>();
                whereSql = ExpressionParser<T>.Parse(func).ToSql(ref parameters, server.GetDbHelper());
                if (whereSql != string.Empty)
                {
                    whereSql = " where " + whereSql;
                    sqlinfo.Parameters = parameters;
                }
            }
            sqlinfo.Sql = string.Format(sqlType, entity.GetColumnName(columnName), entity.GetTableName(), whereSql);
            return sqlinfo;
        }

        private static string ExpressionRouter(Expression exp, object obj)
        {
            if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (me.Expression is ConstantExpression)
                    return string.Empty;
                if (me.Expression is MemberExpression)
                    return ExpressionRouter(me.Expression, obj);
                var entity = obj as BaseEntity;
                return entity.GetColumnName(me.Member.Name);
            }
            if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                return ExpressionRouter(ue.Operand, obj);
            }
            return string.Empty;
        }
    }
}
#pragma warning restore 1591