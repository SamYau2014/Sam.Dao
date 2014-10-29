using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Sam.DAO.Entity;
using System.Data.Common;
using Sam.DAO.Tool;
using System.Data;

namespace Sam.DAO
{
    public interface IDaoContext
    {
        int Count<T>(Expression<Func<T, bool>> func) where T : BaseEntity, new();

        T Find<T>(params object[] keyValues) where T : BaseEntity, new();

        IEnumerable<CombinationEntity<T1, T2>> Select<T1, T2>(Expression<Func<T1, bool>> func = null)
            where T1 : BaseEntity, new()
            where T2 : BaseEntity, new();

        IEnumerable<T> Select<T>(string sql, params DbParameter[] parameters) where T : BaseEntity, new();

        IEnumerable<T> Select<T>(Expression<Func<T, bool>> func) where T : BaseEntity, new();

        IEnumerable<T> Select<T>(Expression<Func<T, bool>> func, string[] properties) where T : BaseEntity, new();

        IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, Expression<Func<T, object>> orderFunc, out int recordCount, bool isAsc) where T : BaseEntity, new();

        IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func,
                                 Expression<Func<T, object>> orderFunc, bool isAsc) where T : BaseEntity, new();

        IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func,
                                 params OrderFunction<T>[] orderFuncs) where T : BaseEntity, new();
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">当前页数，从1开始算</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="func">查询条件</param>
        /// <param name="recordCount">返回总数</param>
        /// <param name="orderFuncs">排序条件组合，仅支持数值类型</param>
        /// <returns></returns>
        IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, out int recordCount, params OrderFunction<T>[] orderFuncs) where T : BaseEntity, new();

        int Insert<T>(T entity) where T : BaseEntity;

        bool Insert<T>(T[] entities) where T : BaseEntity;

        /// <summary>
        /// 添加，适用于有自增字段的表，成功后返回主键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Insert2<T>(T entity) where T : BaseEntity;

        int Update<T>(T entity) where T : BaseEntity;

        int Update<T>(T entity, string[] properties) where T : BaseEntity;

        int Update<T>(T entity, Expression<Func<T, bool>> whereFunc, string[] properties) where T : BaseEntity;

        int Delete<T>(T entity) where T : BaseEntity;

        int Delete<T>(Expression<Func<T, bool>> whereFunc) where T : BaseEntity, new();

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
