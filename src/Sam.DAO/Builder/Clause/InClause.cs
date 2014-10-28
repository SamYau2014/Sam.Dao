using System.Text;
using Sam.DAO.Builder.Data;

namespace Sam.DAO.Builder.Clause
{
    public class InClause:Condition
    {
        private string _column;
        private object[] _values;
        private bool _inFlag;

        public InClause(string column,object[] values,bool inFlag)
        {
            _column = column;
            _values = values;
            _inFlag = inFlag;
        }

       /* public override string ToSql()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");
            sb.Append(_column);
            if (_inFlag)
                sb.Append(" in(");
            else
                sb.Append(" not in(");
            foreach(object o in _values)
            {
                sb.Append(base.GetValueString(o));
                sb.Append(",");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            sb.Append(") ");
            return sb.ToString();
        }*/

        public override string ToSql(ref System.Collections.Generic.IList<KeyValue> kvs,config.DbConfig config)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" ");
            sb.Append(_column);
            if (_inFlag)
                sb.Append(" in(");
            else
                sb.Append(" not in(");
            foreach (object o in _values)
            {
                sb.Append(base.GetValueString(o,config));
                sb.Append(",");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            sb.Append(") ");
            return sb.ToString();
        }
    }
}
