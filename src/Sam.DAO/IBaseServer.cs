using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Sam.DAO.Entity;
using Sam.DAO.Tool;

namespace Sam.DAO
{
    public interface IBaseServer
    {
        #region 查找
        int Count<T>(Expression<Func<T, bool>> func) where T : BaseEntity, new();

        T Find<T>(params object[] keyValues) where T : BaseEntity, new();

        IEnumerable<CombinationEntity<T1, T2>> Select<T1, T2>(Expression<Func<T1, bool>> func = null)
            where T1 : BaseEntity, new()
            where T2 : BaseEntity, new();

        IEnumerable<T> Select<T>(string sql, params DbParameter[] parameters) where T : BaseEntity, new();

        IEnumerable<T> Select<T>(Expression<Func<T, bool>> func = null) where T : BaseEntity, new();

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">当前页数，从1开始算</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="func">查询条件</param>
        /// <param name="orderFunc">排序条件,仅支持单字段排序</param>
        /// <param name="isAsc">是否升序</param>
        /// <returns></returns>
        IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, Expression<Func<T, object>> orderFunc, out int recordCount, bool isAsc = true) where T : BaseEntity, new();

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

        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        int Insert<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// 添加，适用于有自增字段的表，成功后返回主键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        int Insert2<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// 事务添加多个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        bool Insert<T>(T[] entities) where T : BaseEntity;
        #endregion

        #region 修改
        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        int Update<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="properties">要修改的属性集合</param>
        int Update<T>(T entity, string[] properties) where T : BaseEntity;

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="whereFunc">修改条件，若条件为null，则修改整个表的数据（请谨慎使用）</param>
        /// <param name="properties">要修改的属性集合</param>
        int Update<T>(T entity, Expression<Func<T, bool>> whereFunc, string[] properties) where T : BaseEntity;
        #endregion

        #region 删除
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        int Delete<T>(T entity) where T : BaseEntity;

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereFunc">删除条件，若条件为null，则删除整个表的数据（请谨慎使用）</param>
        int Delete<T>(Expression<Func<T, bool>> whereFunc) where T : BaseEntity, new();
        #endregion

        #region sql直接操作数据库
        DbParameter CreateParameter(string paraName, object value, Type valueType);

        int ExecuteNonQuery(string sqlFormat, DbParameter[] paras);

        int ExecuteNonQuery(SqlInfo sqlInfo);

        bool ExecuteTransaction(params SqlInfo[] sqlInfos);

        bool ExecuteTransaction(params string[] sqls);

        DataTable ExecuteDataTable(string sql);

        DataTable ExecuteDataTable(SqlInfo sqlInfo);

        object ExecuteScalar(string sql);

        object ExecuteScalar(SqlInfo sqlInfo);

        int RunSPNonQuery(string procedureName, params DbParameter[] paras);

        DataTable RunSPDataTable(string procedureName, params DbParameter[] paras);
        #endregion

    }
}
