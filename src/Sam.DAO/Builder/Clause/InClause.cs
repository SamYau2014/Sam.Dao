using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Sam.DAO.Builder.Data;

namespace Sam.DAO.Builder.Clause
{
    internal class InClause : Condition
    {
        private readonly string _column;
        private readonly object[] _values;
        private readonly bool _inFlag;

        public InClause(string column,object[] values,bool inFlag)
        {
            _column = column;
            _values = values;
            _inFlag = inFlag;
        }

        public override string ToSql(ref ICollection<DbParameter> parameters, DB dbhelp)
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
                sb.Append(GetValueString(o, dbhelp.GetDbConfig));
                sb.Append(",");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            sb.Append(") ");
            return sb.ToString();
        }
    }
}
