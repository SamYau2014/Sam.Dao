using System;
using Sam.DAO.DAOException;
using Sam.DAO.Tool;
using System.Configuration;

namespace Sam.DAO.config
{
    internal static class DbConfigLoader
    {

        /// <summary>
        /// 加载数据库配置信息
        /// </summary>
        /// <param name="name">连接字符串名称</param>
        /// <param name="dbconfig">数据库配置信息</param>
        public static void Load(string name,DbConfig dbconfig)
        {
            ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[name];
            if (css == null)
                throw new Sam.DAO.DAOException.DaoException("找不到指定的数据库连接信息");
            Load(css.ConnectionString, css.ProviderName, dbconfig);
        }

   
        /// <summary>
        /// 加载数据库配置信息
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="providerName">服务提供名</param>
        /// <param name="dbconfig">数据库配置信息</param>
        public static void Load(string connectionString, string providerName,DbConfig dbconfig)
        {
            Load(connectionString, GetDbType(providerName), dbconfig);
            ValidateProviderName(providerName, dbconfig.DbType);
        }


        /// <summary>
        /// 加载数据库配置信息
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="dbconfig">数据库配置信息</param>
        public static void Load(string connectionString, DataBaseType dbType, DbConfig dbconfig)
        {
            dbconfig.ProviderName = GetProviderName(dbType);
            dbconfig.ConnectionString = connectionString;
            dbconfig.DbType = dbType;
        }

        /// <summary>
        /// 验证providerName
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="dbType"></param>
        private static void ValidateProviderName(string providerName,DataBaseType dbType)
        {
            if (dbType == DataBaseType.unknown)
                throw new Exception("不支持ProviderName：" + providerName);
        }

        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="providerName"></param>
        /// <returns></returns>
        private static DataBaseType GetDbType(string providerName)
        {
            switch (providerName)
            {
                case "System.Data.SqlClient":
                    return DataBaseType.sqlServer;
                case "MySql.Data.MySqlClient":
                    return DataBaseType.mySql;
                case "System.Data.OracleClient":
                    return DataBaseType.SystemOracle;
                case "Oracle.DataAccess.Client":
                    return DataBaseType.Oracle;
                case "System.Data.OleDb":
                    return DataBaseType.Oledb;
                case "System.Data.Odbc":
                    return DataBaseType.Odbc;
                case"System.Data.SQLite":
                    return DataBaseType.SQLite;
                default:
                    return DataBaseType.unknown;
            }
        }

        /// <summary>
        /// 获取ProviderName
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        private static string GetProviderName(DataBaseType dbType)
        {
            switch (dbType)
            {
                case DataBaseType.sqlServer:
                    return "System.Data.SqlClient";
                case DataBaseType.mySql:
                    return "MySql.Data.MySqlClient";
                case DataBaseType.Oracle:
                    return "Oracle.DataAccess.Client";
                case DataBaseType.SystemOracle:
                    return "System.Data.OracleClient";
                case DataBaseType.Oledb:
                    return "System.Data.OleDb";
                case DataBaseType.Odbc:
                    return "System.Data.Odbc";
                default:
                    return "unknown";
            }
        }
    }
}
