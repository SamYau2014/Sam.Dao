using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Sam.DAO.Tool;
using Sam.DAO.config;

namespace Sam.DAO
{
    public abstract class DB : IDisposable
    {
        protected DbProviderFactory Factory; //数据库服务提供工厂
        protected DbConfig DBConfig; //数据库配置
        protected Hashtable ParamCache;

        protected DB(DbConfig dbconfig)
        {
            this.DBConfig= dbconfig;
            this.Factory = DaoDbProviderFactories.GetFactory(dbconfig.ProviderName);
            this.ParamCache = Hashtable.Synchronized(new Hashtable());
        }

        /// <summary>
        /// 获取DbConfig
        /// </summary>
        public DbConfig GetDbConfig
        {
            get
            {
                return DBConfig;
            }
        }

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateParameter()
        {
            return Factory.CreateParameter();
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paras"></param>
        protected void AddParameters(IDbCommand cmd, DbParameter[] paras)
        {
            if (paras != null)
            {
                foreach (DbParameter para in paras)
                {
                    cmd.Parameters.Add(para);
                }
            }
        }

        /// <summary>
        /// 获取所有表名
        /// </summary>
        /// <returns></returns>
        public abstract DataTable GetTables();

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract DataTable GetTableSchema(string tableName);

        public abstract DataTable ExecuteDataTable(string sql);

        public abstract DataTable ExecuteDataTable(SqlInfo sqlInfo);

        public abstract IDbConnection GetCollection();


        public abstract IDataReader ExecuteReader(IDbConnection conn,string sql, CommandType cmdType);

        public abstract IDataReader ExecuteReader(IDbConnection conn, SqlInfo sqlInfo);

        public abstract int ExecuteNonQuery(SqlInfo sqlInfo);

        public abstract object ExecuteScalar(SqlInfo sqlinfo);

        public abstract bool ExecuteTransaction(IsolationLevel isolationLevel,params SqlInfo[] sqlInfos);

        public abstract bool ExecuteTransaction(IsolationLevel isolationLevel, params string[] sqls);

        public abstract DbParameter[] GetSpParameterSet(string procedureName);

        public abstract int RunSPNonQuery(string procedureName, params DbParameter[] paras);

        public abstract DataTable RunSPDataTable(string procedureName, params DbParameter[] paras);

        public void Dispose()
        {
            DBConfig = null;
            Factory = null;
            ParamCache = null;
        }
    }
}
