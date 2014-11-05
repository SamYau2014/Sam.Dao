using System.Collections.Generic;
using System.Data.Common;
using Sam.DAO.Builder.Data;
namespace Sam.DAO.Builder.Clause
{
    internal interface IClause
    {
        string ToSql(ref ICollection<DbParameter> parameters, DB dbhelp);
    }
}
