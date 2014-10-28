using System.Collections.Generic;
using Sam.DAO.Builder.Data;

namespace Sam.DAO.Builder.Clause
{
    public class KeyValueClause:Condition
    {
        private KeyValue _kv;
        private string _operator;
        private int _num = 0;

        public KeyValueClause(string key, object value, string op, int num)
            : this(new KeyValue(key, value,num), op)
        {
            _num = num;
        }
        public KeyValueClause(string key, object value, string op):this(new KeyValue(key,value,0),op)
        {

        }

        public KeyValueClause(KeyValue kv,string op)
        {
            _kv = kv;
            _operator = op;
        }

      /*  public override string ToSql()
        {
            if (_operator == "like")
                return string.Format(" {0} {1} {2} ", _kv.Key, _operator, base.GetValueString(_kv.Value));
            return string.Format(" {0} {1} Linq{0}{2} ", _kv.Key, _operator, _num);
        }*/

        public override string ToSql(ref IList<KeyValue> kvs, config.DbConfig config)
        {
            kvs.Add(_kv);
            //if (_operator == "like")
            //    return string.Format(" {0} {1} {2} ", _kv.Key, _operator, base.GetValueString(_kv.Value));
            return string.Format(" {0} {1} Linq{0}{2} ", _kv.Key, _operator, _num==0?"":_num.ToString());
        }

        public KeyValue GetKeyValue()
        {
            return this._kv;
        }
    }
}
