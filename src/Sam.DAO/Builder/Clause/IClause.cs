using System.Collections.Generic;
using Sam.DAO.Builder.Data;
namespace Sam.DAO.Builder.Clause
{
    public interface IClause
    {
       // string ToSql();

        string ToSql(ref IList<KeyValue> kvs,config.DbConfig config);
    }
}
