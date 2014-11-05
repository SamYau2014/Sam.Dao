using System;
using Sam.DAO.Tool;

namespace Sam.DAO.config
{
    internal class DbConfig
    {
        #region 分页sql
        private const string _mysqlPageSql = "select * from {0} {3} order by {4} limit {1},{2}";
        private const string _sqlServerPageSql = @"select * from
(
	select A.*,ROW_NUMBER() over(order by {4}) as rownum from {0} as A
	{3}
) T
where rownum>{1} and rownum<={2}";
        private const string _oraclePageSql = @"select * from {0}
where rowid in
(
    select rid from 
    (
        select rownum rn,rid from
        (
            select rowid rid,T.* from 
            {0} T {3} order by {4}
        ) 
        where rownum<={2}
    ) 
    where rn>{1}
)";
        #endregion

        private DataBaseType _dbType;
        private string _connectionString;
        public string ProviderName { get; set; }
        public DataBaseType DbType
        {
            get
            {
                return _dbType;
            }
            set
            {
                switch (value)
                {
                    case DataBaseType.mySql:
                        PreParameterChar = "?";
                        DateFormat = "'{0}'";
                        PageSql = _mysqlPageSql;
                        break;
                    case DataBaseType.sqlServer:
                        PreParameterChar = "@";
                        DateFormat = "'{0}'";
                        PageSql = _sqlServerPageSql;
                        break;
                    case DataBaseType.Odbc:
                    case DataBaseType.SQLite:
                        PreParameterChar = ":";
                        DateFormat = "'{0}'";
                        break;
                    case DataBaseType.Oracle:
                        PreParameterChar = ":";
                        DateFormat = " to_date('{0}', 'yyyy-mm-dd hh24:mi:ss')";
                        PageSql = _oraclePageSql;
                        break;
                    case DataBaseType.Oledb:
                        PreParameterChar = "@";
                        DateFormat = "#{0}#";
                        break;
                }
                _dbType = value;
            }
        }
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                if (value != "")
                {
                    string[] strs = value.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in strs)
                    {
                        if (str.Contains("uid") || str.Contains("user") || str.Contains("user id"))
                        {
                            UserId = str.Split('=')[1];
                        }
                        if (str.Contains("pwd") || str.Contains("password"))
                        {
                            Password = str.Split('=')[1];
                        }
                    }
                }
                _connectionString = value;
            }
        }
        public string UserId { get; private set; }
        public string Password { get; private set; }
        public string DateFormat { get; private set; }
        /// <summary>
        /// 参数前缀 
        /// SQL Server为【@】 
        /// MySql 为【?】
        /// Oracle为【：】
        /// </summary>
        public string PreParameterChar { get; private set; }

        /// <summary>
        /// 分页语句
        /// </summary>
        internal string PageSql { get; private set; }

        /// <summary>
        /// 判断是否设置了数据库信息
        /// </summary>
        public bool HasDataBase
        {
            get
            {
                return ProviderName != null;
            }
        }
    }
}
