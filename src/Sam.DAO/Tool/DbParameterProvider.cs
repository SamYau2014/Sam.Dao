using System;
using System.Data.Common;

namespace Sam.DAO.Tool
{
    public class DbParameterProvider:IDbParameterProvider
    {
        public void SetParameter(string paraName, object value, Type valueType,ref DbParameter para)
        {
            para.ParameterName = paraName;
            para.Value = value;
            para.DbType = TypeAdapter.GetDbType(TypeAdapter.GetNetType(valueType));
        }
    }
}
