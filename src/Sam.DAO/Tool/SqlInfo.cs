using System.Collections.Generic;
using System.Data.Common;

namespace Sam.DAO.Tool
{

    /// <summary>
    /// sql语句及相关参数对象
    /// </summary>
    public class SqlInfo
    {
        /// <summary>
        /// sql语句
        /// </summary>
        public string Sql { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public ICollection<DbParameter> Parameters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SqlInfo()
        {
            this.Parameters = new List<DbParameter>();
        }
    }
}
