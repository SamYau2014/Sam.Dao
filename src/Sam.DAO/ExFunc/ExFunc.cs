using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sam.DAO.Attribute;
using Sam.DAO.Entity;

namespace Sam.DAO.ExFunc
{
    public static class ExFunc
    {
        internal static PropertyInfo[] GetProperties(this Type type, bool isAll = true)
         {
             PropertyInfo[] properties=type.GetProperties();
             if (isAll)
                 return properties;
             return (from propertie in properties
                          where propertie.GetCustomAttributes(typeof (PropertyAttribute), false).Length != 0
                          select propertie).ToArray();
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


        //public static bool Between(this int num, int startNum, int endNum)
        //{
        //    return true;
        //}
        //public static bool NotBetween(this int num, int startNum, int endNum)
        //{
        //    return true;
        //}
        //public static bool Between(this double num, double startNum, double endNum)
        //{
        //    return true;
        //}
        //public static bool NotBetween(this double num, double startNum, double endNum)
        //{
        //    return true;
        //}
        //public static bool Between(this float num, float startNum, float endNum)
        //{
        //    return true;
        //}
        //public static bool NotBetween(this float num, float startNum, float endNum)
        //{
        //    return true;
        //}
        //public static bool Between(this decimal num, decimal startNum, decimal endNum)
        //{
        //    return true;
        //}
        //public static bool NotBetween(this decimal num, decimal startNum, decimal endNum)
        //{
        //    return true;
        //}
        //public static bool Between(this long num, long startNum, long endNum)
        //{
        //    return true;
        //}
        //public static bool NotBetween(this long num, long startNum, long endNum)
        //{
        //    return true;
        //}
        //public static bool Like(this string str, string likeStr,LikeType likeType)
        //{
        //    return true;
        //}
        //public static bool NotLike(this string str, string likeStr, LikeType likeType)
        //{
        //    return true;
        //}
      
        //public static bool Between(this DateTime str, DateTime startDate,DateTime endDate)
        //{
        //    return true;
        //}

        //public static bool Compare(this DateTime date,DateTime compareDate,int seconds,CompareType compareType)
        //{
        //    return true;
        //}
       
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
        #region old code
        /*
        public static string Where<T>(this T entity, Expression<Func<T, bool>> func) where T : BaseEntity
        {
            if (func == null)
                return string.Empty;
            else if (func.Body is BinaryExpression)
            {
                BinaryExpression be = (BinaryExpression)func.Body;
                return BinarExpressionProvider(be.Left, be.Right, be.NodeType, entity);
            }
            else if (func.Body is MethodCallExpression)
            {
                return MethodCallExpressionProvider(func.Body, entity);
            }
            else
                return string.Empty;

            //return ExpressionParser<T>.Parse(func).ToSql();
        }

        static string MethodCallExpressionProvider(Expression exp, object obj)
        {
            return ExpressionRouter(exp,null,obj);
        }

        static string BinarExpressionProvider(Expression left, Expression right, ExpressionType type,object obj)
        {
            StringBuilder sb = new StringBuilder();
            //先处理左边
            string leftStr = ExpressionRouter(left, null, obj);

            string castStr = ExpressionTypeCast(type);

            //再处理右边
            string rightStr = ExpressionRouter(right,null,obj);
            if (castStr.Trim() == "AND" || castStr.Trim() == "OR")
            {
                sb.Append("(");
                if (leftStr == ""&&rightStr!="" )
                {
                    sb.Append(rightStr);
                }
                else if (leftStr != "" && rightStr == "")
                {
                    sb.Append(leftStr);
                }
                else if (leftStr != "" && rightStr != "")
                {
                    
                    sb.Append(leftStr);
                    sb.Append(castStr);
                    sb.Append(rightStr);
                    
                }
                sb.Append(")");
            }
            else
            {
                if (rightStr == "null" || rightStr == "")//如果提供的查询项为null，则忽略此查询条件
                {
                    //if (sb.EndsWith(" ="))
                    //    sb = sb.Substring(0, sb.Length - 2) + " is null";
                    //else if (sb.EndsWith("<>"))
                    //    sb = sb.Substring(0, sb.Length - 2) + " is not null";
                }
                else
                {
                    sb.Append("(");
                    sb.Append(leftStr);
                    sb.Append(castStr);
                    sb.Append(rightStr);
                    sb.Append(")");
                }
            }
            return sb.ToString();
        }
        */

        /*
        static string ExpressionRouter(Expression exp,object data,object obj)
        {
            string sb = string.Empty;
            if (exp is BinaryExpression)
            {
                BinaryExpression be = ((BinaryExpression)exp);
                return BinarExpressionProvider(be.Left, be.Right, be.NodeType,obj);
            }
            else if (exp is MemberExpression)
            {
                MemberExpression me = ((MemberExpression)exp);
                if (me.Expression is ConstantExpression)
                {
                    return ExpressionRouter(me.Expression, data == null ? me.Member.Name : me.Member.Name+"."+data, null);
                }
                else if (me.Expression is MemberExpression)
                {
                    return ExpressionRouter(me.Expression, me.Member.Name, obj);                   
                }
                else
                {
                    BaseEntity entity = obj as BaseEntity;
                    return entity.GetColumnName(me.Member.Name);
                }
            }
            else if (exp is NewArrayExpression)
            {
                NewArrayExpression ae = ((NewArrayExpression)exp);
                StringBuilder tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(ExpressionRouter(ex,null,obj));
                    tmpstr.Append(",");
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression)
            {
                MethodCallExpression mce = (MethodCallExpression)exp;
                if (mce.Method.Name == "Like" || mce.Method.Name == "NotLike")
                {
                    System.Reflection.ParameterInfo[] paras = mce.Method.GetParameters();
                    string columnName = ExpressionRouter(mce.Arguments[0], null, obj);
                    string likeStr = ExpressionRouter(mce.Arguments[1], null, obj);
                    string likeType = ExpressionRouter(mce.Arguments[2], null, obj);

                    likeStr = likeStr.Trim("'".ToCharArray());
                    if (likeStr == "null"||likeStr=="")
                        return "";
                    
                    string temp = "";
                    if (likeType == "Both")
                    {
                        temp = "%" + likeStr + "%";
                    }
                    else if (likeType == "Left")
                    {
                        temp = "%" + likeStr ;
                    }
                    else
                    {
                        temp = likeStr + "%";
                    }
                    return string.Format("({0} {1} '{2}')", columnName,mce.Method.Name,temp);
                }
                else if (mce.Method.Name == "In")
                {
                    return string.Format("{0} In ({1})", ExpressionRouter(mce.Arguments[0], null, obj), ExpressionRouter(mce.Arguments[1], null, obj));
                }
                else if (mce.Method.Name == "NotIn")
                    return string.Format("{0} Not In ({1})", ExpressionRouter(mce.Arguments[0], null, obj), ExpressionRouter(mce.Arguments[1], null, obj));
                else if (mce.Method.Name == "Between")
                {
                    return string.Format("{0} between {1} and {2}", ExpressionRouter(mce.Arguments[0], null, obj), ExpressionRouter(mce.Arguments[1], null, obj), ExpressionRouter(mce.Arguments[2], null, obj));
                }
                else if (mce.Method.Name == "Compare")
                {
                    System.Reflection.ParameterInfo[] paras = mce.Method.GetParameters();
                    string columnName = ExpressionRouter(mce.Arguments[0], null, obj);
                    MemberExpression me = mce.Arguments[1] as MemberExpression;
                    string compareDate="";
                    if (me.ToString() == "DateTime.Now")
                        compareDate = "now()";
                    else
                        compareDate = ExpressionRouter(mce.Arguments[1], null, obj);
                    string seconds = ExpressionRouter(mce.Arguments[2], null, obj);
                    string compareType = ExpressionRouter(mce.Arguments[3], null, obj);
                    switch (compareType)
                    {
                        case "GreaterThan":
                            return string.Format(" UNIX_TIMESTAMP({0})-UNIX_TIMESTAMP({1})>{2} ", columnName, compareDate, seconds);
                        case "LessThan":
                            return string.Format(" UNIX_TIMESTAMP({0})-UNIX_TIMESTAMP({1})<{2} ", compareDate, columnName, seconds);
                        case "Equal":
                            return string.Format(" ABS(UNIX_TIMESTAMP({0})-UNIX_TIMESTAMP({1}))={2} ", compareDate, columnName, seconds);
                        case "GreaterThanOrEqual":
                            return string.Format(" UNIX_TIMESTAMP({0})-UNIX_TIMESTAMP({1})>={2} ", columnName, compareDate, seconds);
                        case "LessThanOrEqual":
                            return string.Format(" UNIX_TIMESTAMP({0})-UNIX_TIMESTAMP({1})<={2} ", compareDate, columnName, seconds);
                        default:
                            return "";
                    }
                }

                else
                {
                    if (mce.Arguments != null && mce.Arguments.Count > 0)
                        return ExpressionRouter(mce.Arguments[0], null, obj);
                    else
                        return ExpressionRouter(mce.Object, null, obj);
                }
                   
            }
            else if (exp is ConstantExpression)
            {
                ConstantExpression ce = ((ConstantExpression)exp);
                return GetObjectValue(ce.Value, data==null?null:data.ToString());
            }
            else if (exp is UnaryExpression)
            {
                UnaryExpression ue = ((UnaryExpression)exp);
                return ExpressionRouter(ue.Operand,null,obj);
            }
            return null;
        }


        private static string GetObjectValue(object o,string name)
        {
            if(o==null)
                return "null";
            Type type = o.GetType();
            if (TypeAdapter.IsNetNumberType(type))
                return o.ToString();
            else if (TypeAdapter.IsNetStringType(type))
                return string.Format("'{0}'", o.ToString());
            else if (TypeAdapter.IsNetDateType(type))
                throw new Exception("不支持时间赋值");
            else if (type.IsGenericType)
            {
                StringBuilder sb = new StringBuilder();
                IEnumerable list = o as IEnumerable;
                foreach (object obj in list)
                {
                    if (TypeAdapter.IsNetNumberType(obj.GetType()))
                    {
                        sb.Append(obj.ToString());
                    }
                    else
                    {
                        sb.Append(string.Format("'{0}'", obj.ToString()));
                    }
                    sb.Append(",");
                }
                return sb.ToString(0, sb.Length - 1);
            }
            else if (type.IsEnum)
            {
                return o.ToString();
            }
            else if (type.IsArray)
            {
                Array arr = o as Array;
                StringBuilder sb = new StringBuilder();
                foreach (object obj in arr)
                {
                    if (TypeAdapter.IsNetNumberType(obj.GetType()))
                    {
                        sb.Append(obj.ToString());
                    }
                    else
                    {
                        sb.Append(string.Format("'{0}'", obj.ToString()));
                    }
                    sb.Append(",");
                }
                return sb.ToString(0, sb.Length - 1);
            }
            else
            {
                string[] arr = name.Split('.');
                object value = null;
                System.Reflection.FieldInfo fi = type.GetField(arr[0]);
                if (fi != null)
                    value = fi.GetValue(o);
                else
                {
                    System.Reflection.PropertyInfo pi = type.GetProperty(arr[0]);
                    if (pi != null)
                        value = pi.GetValue(o, null);
                }
                if (arr.Length > 1)
                    return GetObjectValue(value, name.Remove(0, arr[0].Length + 1));
                else
                    return GetObjectValue(value, null);
            }
        }

        static string ExpressionTypeCast(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " AND ";
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
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " Or ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                default:
                    return null;
            }
        }*/
        #endregion
    }
}
