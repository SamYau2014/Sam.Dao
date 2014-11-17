using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;
using Sam.DAO.config;
using Sam.DAO.Tool;

namespace Sam.DAO
{
    internal class DbHelper : DB, IDisposable
    {
        private IDbConnection _conn;                              //连接对象

        /// <summary>
        /// 构造函数
        /// </summary>
        public DbHelper(DbConfig dbconfig)
            : base(dbconfig)
        {
            this._conn = this.Factory.CreateConnection();
            this._conn.ConnectionString = dbconfig.ConnectionString;
        }

        /// <summary>
        /// 获取所有表名
        /// </summary>
        /// <returns></returns>
        public override DataTable GetTables()
        {
            OpenConnection();
            DataTable dt = null;
            DbConnection con = _conn as DbConnection;
            
            //Catalog,Owner,Table及TableType
            string[] restrictions = new string[] { null, null, null, null };
            switch (DBConfig.DbType)
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
                    restrictions = new string[] { DBConfig.UserId.ToUpper(), null };
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
        public override DataTable GetTableSchema(string tableName)
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
            catch(DbException e)
            {
                throw e;
            }
            finally
            {
                CloseConnection();
            }
        }

    
        /// <summary>
        /// 打开连接
        /// </summary>
        /// <returns></returns>
        private bool OpenConnection()
        {
            if (_conn.State == ConnectionState.Broken)
                _conn.Close();

            if (_conn.State == ConnectionState.Closed)
            {
                try
                {
                    _conn.Open();
                    return true;
                }
                catch (DbException e)
                {
                    throw e;
                }
            }
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


        public override DataTable ExecuteDataTable(string sql)
        {
            SqlInfo sqlInfo = new SqlInfo { Sql = sql, Parameters = null };
            return ExecuteDataTable(sqlInfo);
        }

        /// <summary>
        /// ExecuteDataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DataTable ExecuteDataTable(SqlInfo sqlInfo)
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
        public override int ExecuteNonQuery(SqlInfo sqlInfo)
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

        public override object ExecuteScalar(SqlInfo sqlinfo)
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

        public override bool ExecuteTransaction(IsolationLevel isolationLevel, params SqlInfo[] sqlInfos)
        {
            if (!OpenConnection())
            {
                throw new Exception("连接数据库失败！");
            }
            IDbTransaction transaction = _conn.BeginTransaction(isolationLevel);
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

        public override bool ExecuteTransaction(IsolationLevel isolationLevel, params string[] sqls)
        {
            if (!OpenConnection())
            {
                throw new Exception("连接数据库失败！");
            }
            IDbTransaction transaction = _conn.BeginTransaction(isolationLevel);
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

        #region Parameter Discovery Functions
        public override DbParameter[] GetSpParameterSet(string procedureName)
        {
            if (string.IsNullOrEmpty(procedureName)) throw new ArgumentNullException("procedureName");
            return GetSpParameterSetInternal(procedureName, false);
        }

        private DbParameter[] GetSpParameterSetInternal(string procedureName, bool includeReturnValueParameter)
        {
            string hashKey = procedureName + (includeReturnValueParameter ? "_include ReturnValue Parameter" : "");
            DbParameter[] cachedParameters = ParamCache[hashKey] as DbParameter[];
            if (cachedParameters == null)
            {

                    DbParameter[] spParameters = DiscoverSpParameterSet(_conn, procedureName, includeReturnValueParameter);
                    ParamCache[hashKey] = spParameters;
                    cachedParameters = spParameters;
            }
            return cachedParameters;
        }

        private DbParameter[] DiscoverSpParameterSet(IDbConnection conn, string procedureName, bool includeReturnValueParameter)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
            switch (DBConfig.DbType)
            {
                case DataBaseType.sqlServer:
                    OpenConnection();
                    System.Data.SqlClient.SqlCommandBuilder.DeriveParameters(cmd as System.Data.SqlClient.SqlCommand);
                    CloseConnection();
                    break;
                case DataBaseType.Oracle:
                case DataBaseType.SystemOracle:
                    OpenConnection();
                    Oracle.DataAccess.Client.OracleCommandBuilder.DeriveParameters(cmd as Oracle.DataAccess.Client.OracleCommand);
                    CloseConnection();
                    break;
                case DataBaseType.mySql:
                    OpenConnection();
                    MySql.Data.MySqlClient.MySqlCommandBuilder.DeriveParameters(cmd as MySql.Data.MySqlClient.MySqlCommand);
                    CloseConnection();
                    break;
                default:
                    throw new DAOException.DbException("暂时不支持该数据库类型");
            }
            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            DbParameter[] discoveredParameters = new DbParameter[cmd.Parameters.Count];
            cmd.Parameters.CopyTo(discoveredParameters, 0);
            foreach (DbParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }
        #endregion Parameter Discovery Functions

        public override int RunSPNonQuery(string procedureName, params DbParameter[] paras)
        {
            return ExecuteNonQuery(procedureName, CommandType.StoredProcedure, paras);
        }

        public override DataTable RunSPDataTable(string procedureName, params DbParameter[] paras)
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
        public new void Dispose()
        {
            CloseConnection();
            _conn.Dispose();
            _conn = null;
            base.Dispose();
        }
    }
}
