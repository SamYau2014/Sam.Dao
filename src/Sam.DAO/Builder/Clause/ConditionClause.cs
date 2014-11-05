using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Sam.DAO.Builder.Data;

namespace Sam.DAO.Builder.Clause
{
    internal class ConditionClause : Condition
    {
        private readonly string _condition;
        private readonly ArrayList _list = new ArrayList();

        protected ConditionClause(string condition)
        {
            this._condition = condition;
        }

        protected ConditionClause(string condition, params Condition[] ics)
            : this(condition)
        {
            foreach (Condition ic in ics)
            {
                if (ic != null)
                {
                    Add(ic);
                }
            }
        }

        public void Add(Condition ic)
        {
            _list.Add(ic);
        }

        public Condition this[int index]
        {
            get { return (Condition)_list[index]; }
            set { _list[index] = value; }
        }

        public override string ToSql(ref ICollection<DbParameter> parameters,DB dbhelp)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < _list.Count; i++)
            {
                sb.Append(" (");
                sb.Append(this[i].ToSql(ref parameters, dbhelp));
                sb.Append(") ");
                if (i < _list.Count - 1)
                    sb.Append(_condition);
            }
            return sb.ToString();
        }
    }
}
