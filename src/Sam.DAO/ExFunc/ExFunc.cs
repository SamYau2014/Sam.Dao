#pragma warning disable 1591
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sam.DAO.Attribute;
using Sam.DAO.Entity;

namespace Sam.DAO.ExFunc
{
    public static class ExFunc
    {
        private static readonly Hashtable PropertyInfoPool = new Hashtable();

        internal static PropertyInfo[] GetProperties(this Type type, bool isAll = true)
        {
            string key = type.FullName + "_" + isAll;
            lock(PropertyInfoPool)
            {
                if(PropertyInfoPool.ContainsKey(key))
                {
                    return PropertyInfoPool[key] as PropertyInfo[];
                }

                PropertyInfo[] properties = type.GetProperties();
                if (isAll)
                    return (PropertyInfoPool[key] = properties) as PropertyInfo[];
                return (PropertyInfoPool[key] =(from propertie in properties
                        where propertie.GetCustomAttributes(typeof(PropertyAttribute), false).Length != 0
                                                select propertie).ToArray()) as PropertyInfo[];
            }
           
         }

        public static bool In<T>(this T obj, params T[] array) where T : struct
        {
            return true;
        }
        public static bool NotIn<T>(this T obj, params T[] array) where T : struct
        {
            return true;
        }
        public static bool IsNull<T>(this T obj) where T : struct
        {
            return true;
        }
        public static bool IsNotNull<T>(this T obj) where T : struct
        {
            return true;
        }
        public static bool Between<T>(this T obj, T startObj, T endObj) where T : struct
        {
            return true;
        }
        public static bool NotBetween<T>(this T obj, T startObj, T endObj) where T : struct
        {
            return true;
        }


        public static bool In(this string str, params string[] array) 
        {
            return true;
        }
        public static bool NotIn(this string str, params string[] array)
        {
            return true;
        }
        public static bool IsNull(this string str)
        {
            return true;
        }
        public static bool IsNotNull(this string str)
        {
            return true;
        }
        public static bool Between(this string str, string startStr, string endStr)
        {
            return true;
        }
        public static bool NotBetween(this string str, string startStr, string endStr)
        {
            return true;
        }
       
        public static string OrderBy<T>(this T entity, Expression<Func<T, object>> func, bool isAsc = true) where T : BaseEntity
        {
            if (func == null)
                return string.Empty;
            string columnName = ExpressionRouter(func.Body,entity); 
            if (isAsc)
                return columnName + " asc ";
            return columnName + " desc ";
        }
       static string ExpressionRouter(Expression exp, object obj)
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