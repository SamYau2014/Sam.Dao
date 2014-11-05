using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Sam.DAO.Entity;
using Sam.DAO.Tool;

namespace Sam.DAO.DaoInterface
{
   public interface ISqlOperation
    {
        DbParameter CreateParameter(string paraName, object value, Type valueType);

        int ExecuteNonQuery(string sqlFormat, DbParameter[] paras);

        int ExecuteNonQuery(SqlInfo sqlInfo);

        bool ExecuteTransaction(params SqlInfo[] sqlInfos);

        bool ExecuteTransaction(params string[] sqls);

        DataTable ExecuteDataTable(string sql);

        DataTable ExecuteDataTable(SqlInfo sqlInfo);

        int GetSerial(string tableName);

        DataTable GetTables();

        IEnumerable<ColumnInfo> GetTableSchema(string tableName);

        object ExecuteScalar(string sql);

        object ExecuteScalar(SqlInfo sqlInfo);

        DbParameter[] GetSpParameterSet(string procedureName);

        int RunSPNonQuery(string procedureName, params DbParameter[] paras);

        DataTable RunSPDataTable(string procedureName, params DbParameter[] paras);
    }
}
