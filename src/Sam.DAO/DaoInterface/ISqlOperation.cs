#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Sam.DAO.Entity;
using Sam.DAO.Tool;

namespace Sam.DAO.DaoInterface
{
   /// <summary>
   /// Sql操作
   /// </summary>
   public interface ISqlOperation
    {
        /// <summary>
        /// 创建一个DbParameter
        /// </summary>
        /// <param name="paraName">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="valueType">参数类型（.net下的类型）</param>
        /// <returns></returns>
        DbParameter CreateParameter(string paraName, object value, Type valueType);

       /// <summary>
       /// 执行一条Sql语句
       /// </summary>
        /// <param name="sql">Sql语句</param>
       /// <returns>受影响的行</returns>
       int ExecuteNonQuery(string sql);

       /// <summary>
       /// 执行一条Sql语句
       /// </summary>
       /// <param name="sql">Sql语句</param>
       /// <param name="paras">存储过程参数</param>
       /// <returns>受影响的行</returns>
       int ExecuteNonQuery(string sql, DbParameter[] paras);

        /// <summary>
       /// 执行事务（注意：mysql若要支持事务，必须把引擎改为InnoDB，MyISAM是不支持事务的）
        /// </summary>
        /// <param name="sqlInfos">Sql语句集合</param>
        /// <returns>成功或者失败</returns>
        bool ExecuteTransaction(params SqlInfo[] sqlInfos);

        /// <summary>
        /// 执行事务（注意：mysql若要支持事务，必须把引擎改为InnoDB，MyISAM是不支持事务的）
        /// </summary>
        /// <param name="sqls">Sql语句集合</param>
        /// <returns>成功或者失败</returns>
        bool ExecuteTransaction(params string[] sqls);

        /// <summary>
        /// 执行事务（注意：mysql若要支持事务，必须把引擎改为InnoDB，MyISAM是不支持事务的）
        /// </summary>
        /// <param name="isolationLevel">隔离级别</param>
        /// <param name="sqlInfos">Sql语句集合</param>
        /// <returns>成功或者失败</returns>
        bool ExecuteTransaction(IsolationLevel isolationLevel, params SqlInfo[] sqlInfos);

        /// <summary>
        /// 执行事务（注意：mysql若要支持事务，必须把引擎改为InnoDB，MyISAM是不支持事务的）
        /// </summary>
        /// <param name="isolationLevel">隔离级别</param>
        /// <param name="sqls">Sql语句集合</param>
        /// <returns>成功或者失败</returns>
        bool ExecuteTransaction(IsolationLevel isolationLevel, params string[] sqls);

        /// <summary>
        /// 执行一条Sql语句
        /// </summary>
        /// <param name="sql">Sql语句</param>
        /// <returns>DataTable</returns>
        DataTable ExecuteDataTable(string sql);

        /// <summary>
        /// 执行一条Sql语句
        /// </summary>
        /// <param name="sqlInfo">Sql语句</param>
        /// <returns>DataTable</returns>
        DataTable ExecuteDataTable(SqlInfo sqlInfo);


        /// <summary>
        /// 获取数据库的表集合信息
        /// </summary>
        /// <returns>表集合信息</returns>
        DataTable GetTables();

        IEnumerable<ColumnInfo> GetTableSchema(string tableName);

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <returns>结果集中第一行的第一列。</returns>
        object ExecuteScalar(string sql);

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="sqlInfo">查询语句</param>
        /// <returns>结果集中第一行的第一列。</returns>
        object ExecuteScalar(SqlInfo sqlInfo);

        DbParameter[] GetSpParameterSet(string procedureName);

        int RunSPNonQuery(string procedureName, params DbParameter[] paras);

        DataTable RunSPDataTable(string procedureName, params DbParameter[] paras);
    }
}
#pragma warning restore 1591