using System;
using System.Collections.Generic;
using System.Data.Common;
using Sam.DAO.Builder.Clause;
using Sam.DAO.config;

namespace Sam.DAO.Builder.Data
{
    internal abstract class Condition : IClause
    {
        public abstract string ToSql(ref ICollection<DbParameter> parameters, DB dbhelp);

        protected string GetValueString(object o, DbConfig config)
        {
            if (o == null)
                return "NULL";
            if (o is DateTime)
                return string.Format(config.DateFormat, ((DateTime) o ).ToString("yyyy-MM-dd HH:mm:ss"));
            if(o is string)
                return string.Format("'{0}'", o.ToString().Replace("'","''"));
            return o.ToString();
        }

        public static Condition operator &(Condition condition1, Condition condition2)
        {
            return new AndClause(condition1,condition2);
        }

        public static Condition operator |(Condition condition1, Condition condition2)
        {
            return new OrClause(condition1, condition2);
        }
    }
}
