using System;

namespace Sam.DAO.Builder.Data
{
    public class KeyValue
    {
        public string Key { get; set; }
        public object Value { get; set; }

        public string LinqKeyName { get; private set; }
        public Type ValueType { get; set; }

        public object NullableValue
        {
            get
            {
                return Value ?? DBNull.Value;
            }
        }

        protected KeyValue() { }

        protected KeyValue(object value) : this(null, value,0) { }

        public KeyValue(string key, object value,int num)
            : this(key, value,num, (value == null) ? typeof(DBNull) : value.GetType()) { }

        public KeyValue(string key, object value,int num, Type valueType)
        {
            this.Key = key;
            this.Value = value;
            this.ValueType = valueType;
            this.LinqKeyName = "Linq" + key + (num==0?"":num.ToString());
        }

    }
}
