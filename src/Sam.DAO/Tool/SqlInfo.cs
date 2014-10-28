using System.Collections.Generic;
using System.Data.Common;

namespace Sam.DAO.Tool
{

    /// <summary>
    /// sql语句及相关参数对象
    /// </summary>
    public class SqlInfo
    {
        public string Sql { get; set; }

        public IList<DbParameter> Parameters { get;  set; }

        public SqlInfo()
        {
            this.Parameters = new List<DbParameter>();
        }
    }
}
