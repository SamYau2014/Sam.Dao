
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Sam.DAO.Builder.Data;

namespace Sam.DAO.Builder.Clause
{
    internal class NullClause : Condition
    {
       private readonly string _column;
       private readonly bool _flag;
       public NullClause(string column,bool flag)
       {
           _column = column;
           _flag = flag;
       }


       public override string ToSql(ref ICollection<DbParameter> parameters, DB dbhelp)
       {
           StringBuilder sb = new StringBuilder();
           sb.Append(" ");
           sb.Append(_column);
           if (_flag)
               sb.Append(" is null ");
           else
               sb.Append(" is not null ");
           return sb.ToString();
       }
    }
}
