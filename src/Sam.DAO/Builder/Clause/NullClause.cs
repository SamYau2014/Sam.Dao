
using System.Text;
using Sam.DAO.Builder.Data;

namespace Sam.DAO.Builder.Clause
{
   public class NullClause:Condition
    {
       private string _column;
       private bool _flag;
       public NullClause(string column,bool flag)
       {
           _column = column;
           _flag = flag;
       }

      /* public override string ToSql()
       {
           StringBuilder sb = new StringBuilder();
           sb.Append(" ");
           sb.Append(_column);
           if (_flag)
               sb.Append(" is null ");
           else
               sb.Append(" is not null ");
           return sb.ToString();
       }*/

       public override string ToSql(ref System.Collections.Generic.IList<KeyValue> kvs, config.DbConfig config)
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
