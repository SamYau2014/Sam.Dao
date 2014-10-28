using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Sam.DAO.config;
using Sam.DAO.Tool;

namespace Sam.DAO
{
    public class PoolDbHepler : IDisposable
    {
        private DbProviderFactory _factory; //数据库服务提供工厂
        private readonly DbConfig _dbConfig; //数据库配置
        private readonly string _connectionString;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        /// <param name="dbconfig"> </param>
        public PoolDbHepler(string connectionString, string providerName, DbConfig dbconfig)
        {
            _dbConfig = dbconfig;
            _factory = DaoDbProviderFactories.GetFactory(providerName);
            _connectionString = connectionString;
        }

        /// <summary>
        /// 获取DbConfig
        /// </summary>
        public DbConfig GetDbConfig
        {
            get { return _dbConfig; }
        }

        private IDbConnection CreateConnection()
        {
            IDbConnection conn = _factory.CreateConnection();
            conn.ConnectionString = _connectionString;
            if (conn.State == ConnectionState.Closed)
            {
                try
                {
                    conn.Open();
                }
                catch (DbException e)
                {
                    throw e;
                }
            }
            return conn;
        }

        /// <summary>
        /// 获取所有表名
        /// </summary>
        /// <returns></returns>
        public DataTable GetTables()
        {
            //  OpenConnection();
            DataTable dt = null;
            using (var con = CreateConnection() as DbConnection)
            {
                string[] restrictions = new string[] {null, null, null, null};
                switch (_dbConfig.DbType)
                {
                    case DataBaseType.sqlServer:
                        restrictions[3] = "BASE TABLE";
                        dt = con.GetSchema("Tables", restrictions);
                        break;
                    case DataBaseType.mySql:
                        restrictions[1] = con.Database;
                        restrictions[3] = "BASE TABLE";
                        dt = con.GetSchema("Tables", restrictions);
                        break;
                    case DataBaseType.Oracle:
                        restrictions = new string[] {_dbConfig.UserId.ToUpper(), null};
                        dt = con.GetSchema("Tables", restrictions);
                        break;
                    case DataBaseType.Oledb:
                        restrictions[3] = "Table";
                        dt = con.GetSchema("Tables", restrictions);
                        break;
                    case DataBaseType.Odbc:
                        dt = con.GetSchema("Tables", restrictions);
                        break;
                    default:
                        break;
                }
            }
            // CloseConnection();
            return dt;
        }

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetTableSchema(string tableName)
        {

            // OpenConnection();
            using (IDbConnection conn = this.CreateConnection())
            {
                string sql = "select * from " + tableName + " where 1=0";
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                try
                {
                    IDataReader dr = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                    DataTable dt = dr.GetSchemaTable();
                    //DataTable dt1 = new DataTable();
                    //dt1.Load(dr);
                    dr.Close();
                    return dt;
                }
                catch (DbException e)
                {
                    throw e;
                }
            }
        }


        /// <summary>
        /// 创建参数
        /// </summary>
        /// <returns></returns>
        public DbParameter CreateParameter()
        {
            return _factory.CreateParameter();
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="paras"></param>
        private void AddParameters(IDbCommand cmd, DbParameter[] paras)
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
        /// ExecuteReader,带参数
        /// </summary>
        /// <param name="sqlFormat"></param>
        /// <param name="cmdType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        private IDataReader ExecuteReader(IDbConnection conn, string sqlFormat, CommandType cmdType, DbParameter[] paras = null)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlFormat;
            cmd.CommandType = cmdType;
            AddParameters(cmd, paras);
            try
            {
                return cmd.ExecuteReader();
            }
            catch (DbException e)
            {
                throw e;
            }
        }

        public DataTable ExecuteDataTable(string sql)
        {
            SqlInfo sqlInfo = new SqlInfo {Sql = sql, Parameters = null};
            return ExecuteDataTable(sqlInfo);
        }

        /// <summary>
        /// ExecuteDataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(SqlInfo sqlInfo)
        {
            using (IDbConnection conn = this.CreateConnection())
            {
                try
                {
                    IDataReader dr = null;
                    if (sqlInfo.Parameters == null)
                        dr = ExecuteReader(conn, sqlInfo.Sql, CommandType.Text);
                    else
                        dr = ExecuteReader(conn, sqlInfo.Sql, CommandType.Text, sqlInfo.Parameters.ToArray());
                    DataTable dt = new DataTable();
                    int fieldCount = dr.FieldCount;
                    for (int count = 0; count <= fieldCount - 1; count++)
                    {
                        dt.Columns.Add(dr.GetName(count), typeof (object));
                    }
                    //populate datatable
                    while (dr.Read())
                    {
                        DataRow datarow = dt.NewRow();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            datarow[i] = dr[i].ToString();
                        }
                        dt.Rows.Add(datarow);
                    }
                    dr.Dispose();
                    return dt.Rows.Count == 0 ? null : dt;
                }
                catch (DbException e)
                {
                    throw e;
                    return null;
                }
            }
        }


        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(SqlInfo sqlInfo)
        {
            return ExecuteNonQuery(sqlInfo.Sql, CommandType.Text,
                                   sqlInfo.Parameters == null ? null : sqlInfo.Parameters.ToArray());
        }


        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="sqlFormat"></param>
        /// <param name="cmdType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        private int ExecuteNonQuery(string sqlFormat, CommandType cmdType, DbParameter[] paras)
        {
            using (IDbConnection conn = this.CreateConnection())
            {
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlFormat;
                cmd.CommandType = cmdType;
                AddParameters(cmd, paras);
                try
                {
                    return cmd.ExecuteNonQuery();
                }
                catch (DbException e)
                {
                    throw e;
                }
            }
        }

        public object ExecuteScalar(SqlInfo sqlinfo)
        {
            using (IDbConnection conn = this.CreateConnection())
            {
                IDbCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlinfo.Sql;
                cmd.CommandType = CommandType.Text;
                if (sqlinfo.Parameters != null && sqlinfo.Parameters.Count > 0)
                    AddParameters(cmd, sqlinfo.Parameters.ToArray());
                try
                {
                    return cmd.ExecuteScalar();
                }
                catch (DbException e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// ExecuteTransaction
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExecuteTransaction(params SqlInfo[] sqlInfos)
        {
            using (IDbConnection conn = this.CreateConnection())
            {
                IDbTransaction transaction = conn.BeginTransaction();
                IDbCommand cmd = conn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;
                try
                {
                    foreach (SqlInfo sqlInfo in sqlInfos)
                    {
                        cmd.CommandText = sqlInfo.Sql;
                        AddParameters(cmd, sqlInfo.Parameters.ToArray());
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (DbException e)
                {
                    transaction.Rollback();
                    throw e;
                    return false;
                }
            }
        }

        /// <summary>
        /// ExecuteTransaction
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExecuteTransaction(params string[] sqls)
        {
            using (IDbConnection conn = this.CreateConnection())
            {
                IDbTransaction transaction = conn.BeginTransaction();
                IDbCommand cmd = conn.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.Text;
                try
                {
                    foreach (string sql in sqls)
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    return true;
                }
                catch (DbException e)
                {
                    transaction.Rollback();
                    throw e;
                    return false;
                }
            }
        }

        #region 存储过程

        public int RunSPNonQuery(string procedureName, params DbParameter[] paras)
        {
            return ExecuteNonQuery(procedureName, CommandType.StoredProcedure, paras);
        }

        public DataTable RunSPDataTable(string procedureName, params DbParameter[] paras)
        {
            using (IDbConnection conn = this.CreateConnection())
            {
                try
                {
                    IDataReader dr = ExecuteReader(conn, procedureName, CommandType.StoredProcedure, paras);
                    if (!dr.Read())
                    {
                        dr.Close();
                        return null;
                    }
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    dr.Close();
                    return dt;
                }
                catch (DbException e)
                {
                    throw e;
                }
            }
        }

        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _factory = null;
        }

    }
}
