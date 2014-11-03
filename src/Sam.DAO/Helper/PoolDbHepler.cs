using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;
using Sam.DAO.config;
using Sam.DAO.Tool;

namespace Sam.DAO
{
    public class PoolDbHelper :DB, IDisposable
    {
      //  private DbProviderFactory _factory; //数据库服务提供工厂
      //  private readonly DbConfig _dbConfig; //数据库配置
        private readonly string _connectionString;
       // private Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        /// <param name="dbconfig"> </param>
        public PoolDbHelper(string connectionString, string providerName, DbConfig dbconfig):base(providerName,dbconfig)
        {
           // _dbConfig = dbconfig;
           // _factory = DaoDbProviderFactories.GetFactory(providerName);
            _connectionString = connectionString;
        }


        private IDbConnection CreateConnection()
        {
            IDbConnection conn = Factory.CreateConnection();
            conn.ConnectionString = _connectionString;
            return conn;
        }

        private void OpenConnetion(IDbConnection conn)
        {
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
        }

        /// <summary>
        /// 获取所有表名
        /// </summary>
        /// <returns></returns>
        public override DataTable GetTables()
        {
            //  OpenConnection();
            DataTable dt = null;
            using (var con = CreateConnection() as DbConnection)
            {
                OpenConnetion(con);
                string[] restrictions = new string[] {null, null, null, null};
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
                        restrictions = new string[] {DBConfig.UserId.ToUpper(), null};
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
        public override DataTable GetTableSchema(string tableName)
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
                    OpenConnetion(conn);
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
                OpenConnetion(conn);
                return cmd.ExecuteReader();
            }
            catch (DbException e)
            {
                throw e;
            }
        }

        public override DataTable ExecuteDataTable(string sql)
        {
            SqlInfo sqlInfo = new SqlInfo {Sql = sql, Parameters = null};
            return ExecuteDataTable(sqlInfo);
        }

        /// <summary>
        /// ExecuteDataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override DataTable ExecuteDataTable(SqlInfo sqlInfo)
        {
            using (IDbConnection conn = this.CreateConnection())
            {
                try
                {
                    OpenConnetion(conn);
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
                    dr.Close();
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
        public override int ExecuteNonQuery(SqlInfo sqlInfo)
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
                    OpenConnetion(conn);
                    return cmd.ExecuteNonQuery();
                }
                catch (DbException e)
                {
                    throw e;
                }
            }
        }

        public override object ExecuteScalar(SqlInfo sqlinfo)
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
                    OpenConnetion(conn);
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
        public override bool ExecuteTransaction(params SqlInfo[] sqlInfos)
        {
            using (IDbConnection conn = this.CreateConnection())
            {
                OpenConnetion(conn);
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
                }
                finally
                {
                    transaction.Dispose();
                }
            }
        }

        /// <summary>
        /// ExecuteTransaction
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public override bool ExecuteTransaction(params string[] sqls)
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

        #region Parameter Discovery Functions
        public override DbParameter[] GetSpParameterSet(string procedureName)
        {
            if (string.IsNullOrEmpty(procedureName)) throw new ArgumentNullException("procedureName");
            return GetSpParameterSetInternal(procedureName, false);
        }

        private DbParameter[] GetSpParameterSetInternal(string procedureName, bool includeReturnValueParameter)
        {
            string hashKey =  procedureName + (includeReturnValueParameter ? "_include ReturnValue Parameter" : "");
            DbParameter[] cachedParameters = ParamCache[hashKey] as DbParameter[];
            if (cachedParameters == null)
            {
                using(IDbConnection conn=this.CreateConnection())
                {
                    DbParameter[] spParameters = DiscoverSpParameterSet(conn, procedureName, includeReturnValueParameter);
                    ParamCache[hashKey] = spParameters;
                    cachedParameters = spParameters;
                }
            }
            return cachedParameters;
        }

        private DbParameter[] DiscoverSpParameterSet(IDbConnection conn, string procedureName, bool includeReturnValueParameter)
        {
            IDbCommand cmd = conn.CreateCommand();
            cmd.CommandText = procedureName;
            cmd.CommandType = CommandType.StoredProcedure;
           switch(DBConfig.DbType)
           {
               case DataBaseType.sqlServer:
                   OpenConnetion(conn);
                  System.Data.SqlClient.SqlCommandBuilder.DeriveParameters(cmd as System.Data.SqlClient.SqlCommand);
                   conn.Close();
                   break;
               case DataBaseType.Oracle:
                   OpenConnetion(conn);
                   Oracle.DataAccess.Client.OracleCommandBuilder.DeriveParameters(cmd as Oracle.DataAccess.Client.OracleCommand);
                   conn.Close();
                   break;
               case DataBaseType.mySql:
                   OpenConnetion(conn);
                   MySql.Data.MySqlClient.MySqlCommandBuilder.DeriveParameters(cmd as MySql.Data.MySqlClient.MySqlCommand);
                   conn.Close();
                   break;
               default:
                   throw new DAO.DAOException.DbException("暂时不支持该数据库类型");
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
            using (IDbConnection conn = this.CreateConnection())
            {
                try
                {
                    OpenConnetion(conn);
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
            Factory = null;
        }

    }
}
