using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;
using Sam.DAO.config;
using Sam.DAO.Tool;
namespace Sam.DAO
{
    public class DbHelper:IDisposable
    {
        private IDbConnection _conn;                              //连接对象
        private DbProviderFactory _factory;                       //数据库服务提供工厂
        private DbConfig _dbConfig;                                //数据库配置
        private Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        public DbHelper(string connectionString, string providerName,DbConfig dbconfig)
        {
            _dbConfig = dbconfig;
            _factory = DaoDbProviderFactories.GetFactory(providerName);
            _conn = _factory.CreateConnection();
            _conn.ConnectionString = connectionString;
        }

        /// <summary>
        /// 获取DbConfig
        /// </summary>
        public DbConfig GetDbConfig
        {
            get
            {
                return _dbConfig;
            }
        }
        /// <summary>
        /// 获取所有表名
        /// </summary>
        /// <returns></returns>
        public DataTable GetTables()
        {
            OpenConnection();
            DataTable dt = null;
            DbConnection con = _conn as DbConnection;
            
            //Catalog,Owner,Table及TableType
            string[] restrictions = new string[] { null, null, null, null };
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
                    restrictions = new string[] { _dbConfig.UserId.ToUpper(), null };
                    dt = con.GetSchema("Tables",restrictions);
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
            CloseConnection();
            return dt;
        }

        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public DataTable GetTableSchema(string tableName)
        {
            
            OpenConnection();
            string sql = "select * from " + tableName+" where 1=0";
            IDbCommand cmd = _conn.CreateCommand();
            cmd.CommandType = CommandType.Text;                  
            cmd.CommandText = sql;
            try
            {
                IDataReader dr = cmd.ExecuteReader(CommandBehavior.KeyInfo);
                DataTable dt = dr.GetSchemaTable();
                DataTable dt1 = new DataTable();
                dt1.Load(dr);
                dr.Close();
                return dt;
            }
            catch(DbException ex)
            {
                throw ex;
              //  return null;
            }
            finally
            {
                CloseConnection();
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
        private void AddParameters(IDbCommand cmd,DbParameter[] paras)
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
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        private bool OpenConnection()
        {
            if (_conn.State == ConnectionState.Closed)
            {
                try
                {
                    _conn.Open();
                    return true;
                }
                catch (DbException)
                {
                    return false;
                }
            }
            else
                return true;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void CloseConnection()
        {
            if (_conn.State != ConnectionState.Closed)
                _conn.Close();
        }

        /// <summary>
        /// ExecuteReader,不带参数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private IDataReader ExecuteReader(string sql,CommandType cmdType)
        {
            return ExecuteReader(sql, cmdType, null);
        }

        /// <summary>
        /// ExecuteReader,带参数
        /// </summary>
        /// <param name="sqlFormat"></param>
        /// <param name="cmdType"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        private IDataReader ExecuteReader(string sqlFormat,CommandType cmdType,DbParameter[] paras)
        {
            if (!OpenConnection())
            {
                throw new Exception("连接数据库失败！");
            }
            IDbCommand cmd = _conn.CreateCommand();
            cmd.CommandText = sqlFormat;
            cmd.CommandType = cmdType;
            AddParameters(cmd, paras);
            try
            {
               return cmd.ExecuteReader();
            }
            catch (DbException e)
            {
                CloseConnection();
                throw e;
            }
        }


        public DataTable ExecuteDataTable(string sql)
        {
            SqlInfo sqlInfo = new SqlInfo { Sql = sql, Parameters = null };
            return ExecuteDataTable(sqlInfo);
        }
        /// <summary>
        /// ExecuteDataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(SqlInfo sqlInfo)
        {
            IDataReader dr = null;
            try
            {
                if (sqlInfo.Parameters == null)
                    dr = ExecuteReader(sqlInfo.Sql, CommandType.Text);
                else
                    dr = ExecuteReader(sqlInfo.Sql, CommandType.Text, sqlInfo.Parameters.ToArray());
                DataTable dt = new DataTable();
                int fieldCount = dr.FieldCount;//Access to the current line number of rows
                for (int count = 0; count <= fieldCount-1; count++)
                {
                    dt.Columns.Add(dr.GetName(count), typeof(object));
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
                //dt.Load(dr);
                return dt == null || dt.Rows.Count == 0 ? null : dt;
            }
            catch (DbException e)
            {
                throw e;
            }
            finally
            {
                dr.Close();
                dr.Dispose();
                CloseConnection();
            }
        }


        /// <summary>
        /// ExecuteNonQuery
        /// </summary>
        /// <param name="sqlInfo"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(SqlInfo sqlInfo)
        {
            return ExecuteNonQuery(sqlInfo.Sql, CommandType.Text, sqlInfo.Parameters==null?null: sqlInfo.Parameters.ToArray());
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
            if (!OpenConnection())
            {
                throw new Exception("连接数据库失败！");
            }
            IDbCommand cmd = _conn.CreateCommand();
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
            finally
            {
                CloseConnection();
            } 
        }

        public object ExecuteScalar(SqlInfo sqlinfo)
        {
            if (!OpenConnection())
            {
                throw new Exception("连接数据库失败！");
            }
            IDbCommand cmd = _conn.CreateCommand();
            cmd.CommandText = sqlinfo.Sql;
            cmd.CommandType = CommandType.Text;
            if (sqlinfo.Parameters != null&&sqlinfo.Parameters.Count>0)
                AddParameters(cmd, sqlinfo.Parameters.ToArray());
            try
            {
                return cmd.ExecuteScalar();
            }
            catch (DbException e)
            {
                throw e;
            }
            finally
            {
                CloseConnection();
            } 
        }

        /// <summary>
        /// ExecuteTransaction
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExecuteTransaction(params SqlInfo[] sqlInfos)
        {
            if (!OpenConnection())
            {
                throw new Exception("连接数据库失败！");
            }
            IDbTransaction transaction = _conn.BeginTransaction();
            IDbCommand cmd = _conn.CreateCommand();
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
            catch (DbException)
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        /// <summary>
        /// ExecuteTransaction
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public bool ExecuteTransaction(params string[] sqls)
        {
            if (!OpenConnection())
            {
                throw new Exception("连接数据库失败！");
            }
            IDbTransaction transaction = _conn.BeginTransaction();
            IDbCommand cmd = _conn.CreateCommand();
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
            catch (DbException)
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        #region 存储过程



        public int RunSPNonQuery(string procedureName, params DbParameter[] paras)
        {
            return ExecuteNonQuery(procedureName, CommandType.StoredProcedure, paras);
        }

        public DataTable RunSPDataTable(string procedureName, params DbParameter[] paras)
        {
            try
            {
                using (IDataReader dr = ExecuteReader(procedureName, CommandType.StoredProcedure, paras))
                {
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
            }
            catch (DbException e)
            {
                throw e;
            }
            finally
            {
                CloseConnection();
            }
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            CloseConnection();
            _conn.Dispose();
            _conn = null;
            _factory = null;
        }
    }
}
