using System;
using System.Collections.Generic;
using Sam.DAO.Builder.Clause;

namespace Sam.DAO.Builder.Data
{
    public abstract class Condition:IClause
    {

        //public abstract string ToSql();

        public abstract string ToSql(ref IList<KeyValue> kvs, config.DbConfig config);

        protected string GetValueString(object o, config.DbConfig config)
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
