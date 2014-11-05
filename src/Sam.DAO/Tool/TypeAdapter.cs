using System;
using System.Collections.Generic;
using System.Data;
namespace Sam.DAO.Tool
{
    /// <summary>
    /// 数据库类型和NET类型转换帮助类
    /// </summary>
    public class TypeAdapter
    {
        #region 数据库类型集
        private static IDictionary<string, string> NetTypeToDbTypeMap;
        /// <summary>
        /// 数据库中的数值类型集
        /// 只枚举出可能的类型
        /// 对未知类型，可能在转换时得不到正确的类型
        /// </summary>
        private static List<string> dbNumberTypeList;

        /// <summary>
        /// 数据库中的字符串类型集(含字符)
        /// 只枚举出可能的类型
        /// 对未知类型，可能在转换时得不到正确的类型
        /// </summary>
        private static List<string> dbStringTypeList;

        /// <summary>
        /// 数据库中的时间类型集
        /// 只枚举出可能的类型
        /// 对未知类型，可能在转换时得不到正确的类型
        /// </summary>
        private static List<string> dbDateTypeList;

        /// <summary>
        /// 数据库中的流类型集
        /// 只枚举出可能的类型
        /// 对未知类型，可能在转换时得不到正确的类型
        /// </summary>
        private static List<string> dbStreamTypeList;
        #endregion

        #region NET类型集
        /// <summary>
        /// NET中的数值类型集
        /// </summary>
        private static readonly List<Type> NetNumberTypeList;

        /// <summary>
        /// NET中的字符串类型
        /// </summary>
        private static readonly List<Type> NetStringTypeList;

        /// <summary>
        /// NET中的时间类型
        /// </summary>
        private static readonly List<Type> NetDateTypeList;
        #endregion

        #region 静态构造函数 初始化各NET类型和数据库类型
        static TypeAdapter()
        {
            NetTypeToDbTypeMap = new Dictionary<string, string>();
            //分别添加各种数据库类型
            //对每一数据库，最好尽可能添加相应的类型
            //保证在转换时可得到相应的NET类型
            dbNumberTypeList = new List<string>();
            dbStringTypeList = new List<string>();
            dbDateTypeList = new List<string>();
            dbStreamTypeList = new List<string>();
            dbNumberTypeList.Add("bit");
            dbNumberTypeList.Add("tinyint");
            dbNumberTypeList.Add("smallint");
            dbNumberTypeList.Add("mediumint");
            dbNumberTypeList.Add("int");
            dbNumberTypeList.Add("bigint");
            dbNumberTypeList.Add("float");
            dbNumberTypeList.Add("double");
            dbNumberTypeList.Add("decimal");
            dbNumberTypeList.Add("smallmoney");
            dbNumberTypeList.Add("money");
            dbNumberTypeList.Add("numeric");
            dbNumberTypeList.Add("number");
            dbNumberTypeList.Add("real");



            dbStringTypeList.Add("char");
            dbStringTypeList.Add("nchar");
            dbStringTypeList.Add("ntext");
            dbStringTypeList.Add("nvarchar");
            dbStringTypeList.Add("text");
            dbStringTypeList.Add("varchar");
            dbStringTypeList.Add("varchar2");
            dbStringTypeList.Add("nvarchar");
            dbStringTypeList.Add("nvarchar2");
            dbStringTypeList.Add("tinytext");
            dbStringTypeList.Add("longtext");
            dbStringTypeList.Add("mediumtext");

            dbDateTypeList.Add("datetime");
            dbDateTypeList.Add("smalldatetime");
            dbDateTypeList.Add("date");
            dbDateTypeList.Add("time");
            dbDateTypeList.Add("timestamp(6)");
            dbDateTypeList.Add("timestamp");


            dbStreamTypeList.Add("binary");
            dbStreamTypeList.Add("varbinary");
            dbStreamTypeList.Add("blob");
            dbStreamTypeList.Add("longblob");
            dbStreamTypeList.Add("mediumblob");

            NetNumberTypeList = new List<Type>();
            NetNumberTypeList.Add(typeof(byte));
            NetNumberTypeList.Add(typeof(System.Single));
            NetNumberTypeList.Add(typeof(short));
            NetNumberTypeList.Add(typeof(ushort));
            NetNumberTypeList.Add(typeof(int));
            NetNumberTypeList.Add(typeof(uint));
            NetNumberTypeList.Add(typeof(float));
            NetNumberTypeList.Add(typeof(long));
            NetNumberTypeList.Add(typeof(ulong));
            NetNumberTypeList.Add(typeof(double));
            NetNumberTypeList.Add(typeof(decimal));
            NetNumberTypeList.Add(typeof(System.Int32));
            NetNumberTypeList.Add(typeof(System.Int64));
            NetNumberTypeList.Add(typeof(System.UInt32));
            NetNumberTypeList.Add(typeof(System.UInt64));

            NetStringTypeList = new List<Type>();
            NetStringTypeList.Add(typeof(System.String));
            NetStringTypeList.Add(typeof(System.Char));

            NetDateTypeList = new List<Type>();
            NetDateTypeList.Add(typeof(DateTime));
        }
        #endregion

        #region IsDbValueType
        /// <summary>
        /// 判断当前数据库类型是否值类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public static bool IsDbValueType(string dbType)
        {
            dbType = GetOnlyDbType(dbType);
            return (dbNumberTypeList.Contains(dbType) || dbDateTypeList.Contains(dbType));
        }
        #endregion

        #region GetNetType
        /// <summary>
        /// 将当前数据库类型转换成NET类型
        /// 若未知类型，默认转成object类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="numeric_scale"></param>
        /// <returns></returns>
        public static string GetNetType(string dbType, int numeric_scale)
        {
            dbType = GetOnlyDbType(dbType);
            if (dbNumberTypeList.Contains(dbType))
            {
                #region 数值类型
                switch (dbType)
                {
                    case "bigint":
                        return "long";
                    case "bit":
                    case "int":
                    case "smallint":
                    case "tinyint":
                        return "int";
                    case "decimal":
                    case "money":
                    case "real":
                    case "smallmoney":
                    case "numeric":
                    case "number":
                        if (numeric_scale == 0)
                            return "long";
                        else
                            return "decimal";
                    case "float":
                        if (numeric_scale == 0)
                            return "long";
                        else
                            return "float";
                    case "double":
                        return "double";
                }
                #endregion
            }
            if (dbStringTypeList.Contains(dbType))
            {
                return "string";
            }
            if (dbDateTypeList.Contains(dbType))
                return "DateTime";
            if (dbStreamTypeList.Contains(dbType))
                return "byte[]";
            return "object";
        }

        public static Type GetNetType(Type type)
        {
            if (type.IsGenericType)
                return type.GetGenericArguments()[0];
            else
                return type;
        }
        #endregion

        #region GetDbType
        /// <summary>
        /// 将.net的类型转换为数据库类型
        /// </summary>
        /// <param name="type">.net的内置类型</param>
        /// <returns></returns>
        public static DbType GetDbType(Type type)
        {
            if (type == typeof(int) || type == typeof(Int32) || type == typeof(UInt32) || type == typeof(short))
                return DbType.Int32;
            if (type == typeof(string))
                return DbType.String;
            if (type == typeof(DateTime))
                return DbType.DateTime;
            if (type == typeof(Int64) || type == typeof(long) || type == typeof(ulong) || type == typeof(UInt64))
                return DbType.Int64;
            if (type == typeof(Single))
                return DbType.Single;
            if (type == typeof(float) || type == typeof(double))
                return DbType.Double;
            if (type == typeof(decimal))
                return DbType.Decimal;
            if (type == typeof(byte) || type == typeof(Byte))
                return DbType.Byte;
            if (type == typeof(char))
                return DbType.AnsiString;
            if (type == typeof(System.IO.Stream))
                return DbType.Binary;
            if (type == typeof(object))
                return DbType.Object;
            if (type == typeof(byte[]))
                return DbType.Binary;
            throw new Exception("不可识别的类型:" + type.ToString());
        }
        #endregion

        #region GetOnlyDbType
        /// <summary>
        /// 仅获取数据库的类型的小写类型串
        /// 比如数据库类型是timestamp(6)将会返回timestamp
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        private static string GetOnlyDbType(string dbType)
        {
            if (dbType.IndexOf('(') > 0)
            {
                dbType = dbType.Substring(0, dbType.IndexOf('('));
            }
            return dbType.ToLower();
        }
        #endregion

        #region 类型判断
        /// <summary>
        /// 是否为数值类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNetNumberType(Type type)
        {
            return NetNumberTypeList.Contains(type);
        }

        /// <summary>
        /// 是否为字符或字符串类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNetStringType(Type type)
        {
            return NetStringTypeList.Contains(type);
        }

        /// <summary>
        /// 是否时间类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNetDateType(Type type)
        {
            return NetDateTypeList.Contains(type);

        }


        #endregion
    }
}
