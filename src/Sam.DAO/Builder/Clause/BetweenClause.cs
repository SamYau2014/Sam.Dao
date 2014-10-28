using System.Text;
using Sam.DAO.Builder.Data;

namespace Sam.DAO.Builder.Clause
{
    public class BetweenClause : Condition
    {
      private string _column;
      private object _left;
      private object _right;
      private bool _flag;

      public BetweenClause(string column,object left,object right,bool flag)
      {
          _column = column;
          _left = left;
          _right = right;
          _flag = flag;
      }

      /*public override string ToSql()
      {
          StringBuilder sb = new StringBuilder();
          sb.Append(" ");
          sb.Append(_column);
          if (!_flag)
              sb.Append(" Not ");
          sb.Append(" Between ");
          sb.Append(base.GetValueString(_left));
          sb.Append(" and ");
          sb.Append(base.GetValueString(_right));
          return sb.ToString();
      }*/

      public override string ToSql(ref System.Collections.Generic.IList<KeyValue> kvs, config.DbConfig config)
      {
          kvs.Add(new KeyValue(_column + "left", _left, 0));
          kvs.Add(new KeyValue(_column + "right", _right, 0));
          StringBuilder sb = new StringBuilder();
          sb.Append(" ");
          sb.Append(_column);
          if (!_flag)
              sb.Append(" Not ");
          sb.Append(" Between ");
       //   sb.Append(base.GetValueString(_left));
          sb.Append("Linq" + _column + "left");
          sb.Append(" and ");
          sb.Append("Linq" + _column + "right");
        //  sb.Append(base.GetValueString(_right));
          return sb.ToString();
      }
    }
}
