﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using Sam.DAO.config;
using Sam.DAO.Entity;
using Sam.DAO.InnerException;
using Sam.DAO.Tool;

namespace Sam.DAO
{
    /* 简单的针对实体及lamda表达式封装的数据库访问层
     * 平台版本：.net 4.0
     * 时间：2014年9月
     */
    public sealed class DaoContext:IDaoContext,IDisposable
    {
       // private DbHelper _dbHelper;
        private PoolDbHepler _dbHelper;
        private EntityHelper _entityHelper;
        private readonly DbConfig _dbConfig;

        #region 构造函数
        /// <summary>
        /// 默认构造函数，使用预先使用DbConfigLoader加载数据库信息
        /// </summary>
        public DaoContext()
        {
            _dbConfig = new DbConfig();
            string dbName = ConfigurationManager.AppSettings["defaultDatabase"];
            if (string.IsNullOrEmpty(dbName))
            {
                ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[0];
                if(string.IsNullOrEmpty(css.Name))
                    throw new DaoException("未设置数据库配置信息");
                dbName = css.Name;
            }
            DbConfigLoader.Load(dbName, _dbConfig);
          //  _dbHelper = new DbHelper(_dbConfig.ConnectionString, _dbConfig.ProviderName,_dbConfig);
            _dbHelper = new PoolDbHepler(_dbConfig.ConnectionString, _dbConfig.ProviderName, _dbConfig);
            _entityHelper = new EntityHelper(_dbHelper);
        }

        public DaoContext(string name)
        {
            _dbConfig = new DbConfig();
            DbConfigLoader.Load(name, _dbConfig);
            _dbHelper = new PoolDbHepler(_dbConfig.ConnectionString, _dbConfig.ProviderName, _dbConfig);
            _entityHelper = new EntityHelper(_dbHelper);
        }

        public DaoContext(string connectionString, string providerName)
        {
             _dbConfig = new DbConfig();
            DbConfigLoader.Load(connectionString, providerName,_dbConfig);
            _dbHelper = new PoolDbHepler(_dbConfig.ConnectionString, _dbConfig.ProviderName, _dbConfig);
            _entityHelper = new EntityHelper(_dbHelper);
        }

        public DaoContext(string connectionString, DataBaseType dbType)
        {
            _dbConfig = new DbConfig();
            DbConfigLoader.Load(connectionString, dbType,_dbConfig);
            _dbHelper = new PoolDbHepler(_dbConfig.ConnectionString, _dbConfig.ProviderName, _dbConfig);
            _entityHelper = new EntityHelper(_dbHelper);
        }

        #endregion

        #region 获取表结构 支持mysql，sqlserver，oracle，其他数据库未测试
        public DataTable GetTables()
        {
            return _entityHelper.GetTables();
        }

        public IEnumerable<ColumnInfo> GetTableSchema(string tableName)
        {
            return _entityHelper.GetTableSchema(tableName);
        }
        #endregion

        #region 实体操作
        #region 查询
        /// <summary>
        /// 根据主键返回一个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public T Find<T>(params object[] keyValues) where T : BaseEntity, new()
        {
            return _entityHelper.Find<T>(keyValues);
        }

        public IEnumerable<CombinationEntity<T1, T2>> Select<T1, T2>(Expression<Func<T1, bool>> func = null)
            where T1 : BaseEntity, new()
            where T2 : BaseEntity, new()
        {
            return _entityHelper.Select<T1, T2>(func);
        }

        public IEnumerable<T> Select<T>(string sql, params DbParameter[] parameters) where T : BaseEntity, new()
        {
            return _entityHelper.Select<T>(sql,parameters);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public IEnumerable<T> Select<T>(Expression<Func<T, bool>> func = null) where T : BaseEntity, new()
        {
            return _entityHelper.Select(func);
        }

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
        public IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, Expression<Func<T, object>> orderFunc,out int recordCount, bool isAsc = true) where T : BaseEntity, new()
        {
            return _entityHelper.Select(pageIndex, pageSize, func, orderFunc,out recordCount, isAsc);
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
        public IEnumerable<T> Select<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> func, out int recordCount,params OrderFunction<T>[] orderFuncs ) where T : BaseEntity, new()
        {
            return _entityHelper.Select(pageIndex, pageSize, func, out recordCount, orderFuncs);
        }

        public int Count<T>(Expression<Func<T, bool>> func) where T : BaseEntity, new()
        {
            return _entityHelper.Count<T>(func);
        }
        #endregion

        #region 添加
        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public int Insert<T>(T entity) where T : BaseEntity
        {
         return  _entityHelper.Insert(entity);
        }

        /// <summary>
        /// 添加，适用于有自增字段的表，成功后返回主键值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert2<T>(T entity) where T : BaseEntity
        {
            return _entityHelper.Insert2(entity);
        }

        /// <summary>
        /// 事务添加多个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public bool Insert<T>(T[] entities) where T : BaseEntity
        {
           return _entityHelper.Insert(entities);
        }
        #endregion

        #region 修改
        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public int Update<T>(T entity) where T : BaseEntity
        {
           return _entityHelper.Update(entity);
        }

        /// <summary>
        /// 根据主键修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="properties">要修改的属性集合</param>
        public int Update<T>(T entity, string[] properties) where T : BaseEntity
        {
           return  _entityHelper.Update(entity, properties);
        }

        /// <summary>
        /// 根据条件修改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="whereFunc">修改条件，若条件为null，则修改整个表的数据（请谨慎使用）</param>
        /// <param name="properties">要修改的属性集合</param>
        public int Update<T>(T entity, Expression<Func<T, bool>> whereFunc,string[] properties) where T : BaseEntity
        {
         return _entityHelper.Update(entity, whereFunc, properties);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 按主键删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public int Delete<T>(T entity) where T : BaseEntity
        {
            return _entityHelper.Delete(entity);
        }

        /// <summary>
        /// 按条件删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="whereFunc">删除条件，若条件为null，则删除整个表的数据（请谨慎使用）</param>
        public int  Delete<T>(Expression<Func<T, bool>> whereFunc) where T : BaseEntity,new()
        {
           return _entityHelper.Delete(whereFunc);
        }
        #endregion
        #endregion

        #region 使用sql直接操作数据库
        public DbParameter CreateParameter(string paraName,object value,Type valueType)
        {
            DbParameter para = _dbHelper.CreateParameter();
            DbParameterProviderFactory.CreateParameterProvider(_dbConfig.DbType).SetParameter(paraName, value, valueType, ref para);
            return para;
        }

        public int ExecuteNonQuery(string sqlFormat, DbParameter[] paras)
        {
            SqlInfo sqlInfo = new SqlInfo();
            sqlInfo.Sql = sqlFormat;
            sqlInfo.Parameters = paras;
            return _dbHelper.ExecuteNonQuery(sqlInfo);
        }

        public int ExecuteNonQuery(SqlInfo sqlInfo)
        {
            return _dbHelper.ExecuteNonQuery(sqlInfo);
        }

        /// <summary>
        /// 执行事务（注意：mysql若要支持事务，必须把引擎改为InnoDB，MyISAM是不支持事务的）
        /// </summary>
        public bool ExecuteTransaction(params SqlInfo[] sqlInfos)
        {
            return _dbHelper.ExecuteTransaction(sqlInfos);
        }

        public bool ExecuteTransaction(params string[] sqls)
        {
            return _dbHelper.ExecuteTransaction(sqls);
        }

        public DataTable ExecuteDataTable(string sql)
        {
            return _dbHelper.ExecuteDataTable(sql);
        }

        public DataTable ExecuteDataTable(SqlInfo sqlInfo)
        {
            return _dbHelper.ExecuteDataTable(sqlInfo);
        }

        public int GetSerial(string tableName)
        {
            int id = 0;
            DbParameter para1 = CreateParameter(_dbConfig.PreParameterChar+"v_seq_name", tableName, typeof(string));
            para1.Direction = ParameterDirection.Input;
            DbParameter para2 = CreateParameter(_dbConfig.PreParameterChar+"v_seq_value", null, typeof(string));
            para2.Direction = ParameterDirection.Output;
            _dbHelper.RunSPDataTable("get_sequence", para1, para2);
            return Int32.TryParse(para2.Value.ToString(), out id) == true ? id : 0;
        }

        public object ExecuteScalar(string sql)
        {
            SqlInfo sqlInfo = new SqlInfo { Sql = sql, Parameters = null };
            return _dbHelper.ExecuteScalar(sqlInfo);
        }

        public object ExecuteScalar(SqlInfo sqlInfo)
        {
            return _dbHelper.ExecuteScalar(sqlInfo);
        }

        public int RunSPNonQuery(string procedureName, params DbParameter[] paras)
        {
            return _dbHelper.RunSPNonQuery(procedureName, paras);
        }

        public DataTable RunSPDataTable(string procedureName, params DbParameter[] paras)
        {
            return _dbHelper.RunSPDataTable(procedureName, paras);
        }

        #endregion

        #region IDisposable 成员
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _dbHelper.Dispose();
            _dbHelper = null;
            _entityHelper = null;
        }
        #endregion
    }
}