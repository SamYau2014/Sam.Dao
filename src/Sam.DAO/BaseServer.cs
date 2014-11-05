using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Sam.DAO.Entity;
using Sam.DAO.Tool;

namespace Sam.DAO
{
    public class BaseServer : IBaseServer
    {
        private static readonly IDaoContext DefaultDB = new DaoContext();

        protected virtual IDaoContext GetDaoContext()
        {
            return DefaultDB;
        }

        #region 查找

        public virtual int Count<T>(Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Count<T>(func);
        }

        /// <summary>
        /// 根据主键返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public virtual T Find<T>(params object[] keyValues) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Find<T>(keyValues);
        }

        public virtual IEnumerable<CombinationEntity<T1, T2>> Select<T1, T2>(Expression<Func<T1, bool>> func = null)
            where T1 : BaseEntity, new()
            where T2 : BaseEntity, new()
        {
            return this.GetDaoContext().Select<T1,T2>(func);
        }

        public virtual IEnumerable<T> Select<T>(string sql, params DbParameter[] parameters) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Select<T>(sql,parameters);
        }

        public virtual IEnumerable<T> Select<T>(Expression<Func<T, bool>> func = null) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Select(func);
        }

        public IEnumerable<T> Select<T>(Expression<Func<T, bool>> func, string[] properties) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Select(func, properties);
        }

        #region 分页查询
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
        public virtual IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, Expression<Func<T, object>> orderFunc, out int recordCount, bool isAsc = true) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Select(pageIndex, pageSize, func, orderFunc, out recordCount, isAsc);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageIndex">当前页数，从1开始算</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="func">查询条件</param>
        /// <param name="recordCount">返回总数</param>
        /// <param name="orderFuncs">排序条件组合</param>
        /// <returns></returns>
        public virtual IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, out int recordCount, params OrderFunction<T>[] orderFuncs) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Select(pageIndex, pageSize, func, out recordCount, orderFuncs);
        }


        public IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, Expression<Func<T, object>> orderFunc, bool isAsc = true) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Select(pageIndex, pageSize,func, orderFunc,isAsc);
        }

        public IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, params OrderFunction<T>[] orderFuncs) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Select(pageIndex, pageSize, func, orderFuncs);
        }
        #endregion 分页查询
        #endregion 查找

        #region 添加
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>受影响行数</returns>
        public virtual int Insert<T>(T entity) where T : BaseEntity
        {
            return this.GetDaoContext().Insert(entity);
        }

        /// <summary>
        /// 添加，适用于有自增字段的表，成功后返回主键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Insert2<T>(T entity) where T : BaseEntity
        {
            return this.GetDaoContext().Insert2(entity);
        }

        /// <summary>
        /// 事务添加多个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns>是否成功执行</returns>
        public virtual bool Insert<T>(T[] entities) where T : BaseEntity
        {
            return this.GetDaoContext().Insert(entities);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public virtual int Update<T>(T entity) where T : BaseEntity
        {
            return this.GetDaoContext().Update(entity);
        }

        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="properties">要修改的属性集合</param>
        public virtual int Update<T>(T entity, string[] properties) where T : BaseEntity
        {
            return this.GetDaoContext().Update(entity, properties);
        }

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="whereFunc">修改条件，若条件为null，则修改整个表的数据（请谨慎使用）</param>
        /// <param name="properties">要修改的属性集合</param>
        public virtual int Update<T>(T entity, Expression<Func<T, bool>> whereFunc, string[] properties) where T : BaseEntity
        {
            return this.GetDaoContext().Update(entity, whereFunc, properties);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public virtual int Delete<T>(T entity) where T : BaseEntity
        {
            return this.GetDaoContext().Delete(entity);
        }

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereFunc">删除条件，若条件为null，则删除整个表的数据（请谨慎使用）</param>
        public virtual int Delete<T>(Expression<Func<T, bool>> whereFunc) where T : BaseEntity, new()
        {
            return this.GetDaoContext().Delete(whereFunc);
        }
        #endregion

        #region 使用sql直接操作数据库
        public virtual DbParameter CreateParameter(string paraName, object value, Type valueType)
        {
            return this.GetDaoContext().CreateParameter(paraName, value, valueType);
        }

        public virtual int ExecuteNonQuery(string sqlFormat, DbParameter[] paras)
        {
            return this.GetDaoContext().ExecuteNonQuery(sqlFormat, paras);
        }

        public virtual int ExecuteNonQuery(SqlInfo sqlInfo)
        {
            return this.GetDaoContext().ExecuteNonQuery(sqlInfo);
        }

        /// <summary>
        /// 执行事务（注意：mysql若要支持事务，必须把引擎改为InnoDB，MyISAM是不支持事务的）
        /// </summary>
        public virtual bool ExecuteTransaction(params SqlInfo[] sqlInfos)
        {
            return this.GetDaoContext().ExecuteTransaction(sqlInfos);
        }

        public virtual bool ExecuteTransaction(params string[] sqls)
        {
            return this.GetDaoContext().ExecuteTransaction(sqls);
        }

        public virtual DataTable ExecuteDataTable(string sql)
        {
            return this.GetDaoContext().ExecuteDataTable(sql);
        }

        public virtual DataTable ExecuteDataTable(SqlInfo sqlInfo)
        {
            return this.GetDaoContext().ExecuteDataTable(sqlInfo);
        }

        public int GetSerial(string tableName)
        {
            return this.GetDaoContext().GetSerial(tableName);
        }

        public DataTable GetTables()
        {
            return this.GetDaoContext().GetTables();
        }

        public IEnumerable<ColumnInfo> GetTableSchema(string tableName)
        {
            return this.GetDaoContext().GetTableSchema(tableName);
        }

        public virtual object ExecuteScalar(string sql)
        {
            return this.GetDaoContext().ExecuteScalar(sql);
        }

        public virtual object ExecuteScalar(SqlInfo sqlInfo)
        {
            return this.GetDaoContext().ExecuteScalar(sqlInfo);
        }

        public virtual DbParameter[] GetSpParameterSet(string procedureName)
        {
            return this.GetDaoContext().GetSpParameterSet(procedureName);
        }

        public virtual int RunSPNonQuery(string procedureName, params DbParameter[] paras)
        {
            return this.GetDaoContext().RunSPNonQuery(procedureName, paras);
        }

        public virtual DataTable RunSPDataTable(string procedureName, params DbParameter[] paras)
        {
            return this.GetDaoContext().RunSPDataTable(procedureName, paras);
        }

        #endregion

    }
}

