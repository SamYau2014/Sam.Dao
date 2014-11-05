using System.Collections.Generic;
using System.Data.Common;
using Sam.DAO.Builder.Data;
using Sam.DAO.Tool;

namespace Sam.DAO.Builder.Clause
{
    internal class KeyValueClause : Condition
    {
        private KeyValue _kv;
        private readonly string _operator;

        public KeyValueClause(string key, object value, string op, int num)
            : this(new KeyValue(key, value,num), op)
        {
        }
        public KeyValueClause(string key, object value, string op):this(new KeyValue(key,value,0),op)
        {

        }

        public KeyValueClause(KeyValue kv,string op)
        {
            _kv = kv;
            _operator = op;
        }


        public override string ToSql(ref ICollection<DbParameter> parameters, DB dbhelp)
        {
            DbParameter parameter = dbhelp.CreateParameter();
            DbParameterProviderFactory.CreateParameterProvider(dbhelp.GetDbConfig.DbType).SetParameter(
                dbhelp.GetDbConfig.PreParameterChar + _kv.LinqKeyName, _kv.NullableValue, _kv.ValueType, ref parameter);
            parameters.Add(parameter);
            return string.Format(" {0} {1} {3}{2} ", _kv.Key, _operator, _kv.LinqKeyName, dbhelp.GetDbConfig.PreParameterChar);
        }
    }
}
