using System;
using System.Data.Common;
using System.Data.OleDb;

namespace Sam.DAO.Tool
{
    public class OleDbParameterProvider:IDbParameterProvider
    {
        public void SetParameter(string paraName, object value, Type valueType, ref DbParameter para)
        {
            OleDbParameter oledbPara = para as OleDbParameter;
            oledbPara.ParameterName = paraName;
            oledbPara.Value = value;
            oledbPara.OleDbType = GetDbType(TypeAdapter.GetNetType(valueType));
        }

        private OleDbType GetDbType(Type type)
        {
            if (type == typeof(int) || type == typeof(Int32) || type == typeof(UInt32) || type == typeof(short))
                return OleDbType.Integer;
            if (type == typeof(Int64) || type == typeof(long) || type == typeof(ulong) || type == typeof(UInt64))
                return OleDbType.BigInt;
            if (type == typeof(Single))
                return OleDbType.Single;
            if (type == typeof(float) || type == typeof(double))
                return OleDbType.Double;
            if (type == typeof(decimal))
                return OleDbType.Decimal;
            if (type == typeof(string))
                return OleDbType.VarChar;
            if (type == typeof(byte) || type == typeof(Byte))
                return OleDbType.TinyInt;
            if (type == typeof(char))
                return OleDbType.Char;
            if (type == typeof(System.IO.Stream))
                return OleDbType.Binary;
            if (type == typeof(DateTime))
                return OleDbType.Date;
            if (type == typeof(object))
                return OleDbType.IUnknown;
            if (type == typeof(byte[]))
                return OleDbType.Binary;
            if (type == typeof(bool))
                return OleDbType.Boolean;
            throw new Exception("不可识别的类型:" + type.ToString());
        }
    }
}
